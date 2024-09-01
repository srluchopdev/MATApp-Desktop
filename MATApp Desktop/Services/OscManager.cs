using CoreOSC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace MATAppDesktop.Services
{
    public class OscManager : IDisposable
    {
        private System.Timers.Timer _oscSendTimer;
        public string OscAddress { get; set; }
        public object[] OscData { get; set; }
        private readonly string _ipAddress;
        private readonly int _port;
        private ManagedUdpSender _sender;
        private UdpClient _udpClient;
        private IPEndPoint _localEndPoint;
        private readonly Form _form;
        private bool _disposed = false;
        private bool _formLoaded;

        public OscManager(string ipAddress, int port, Form form)
        {
            _ipAddress = ipAddress;
            _port = port;
            _form = form ?? throw new ArgumentNullException(nameof(form));

            try
            {
                // Inicializa ManagedUdpSender
                _sender = new ManagedUdpSender(_ipAddress, _port);
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Error al crear ManagedUdpSender: {ex.Message}");
            }

            if (_sender != null)
            {
                try
                {
                    // Inicializa UdpClient para recibir mensajes
                    _localEndPoint = new IPEndPoint(IPAddress.Any, _port);
                    _udpClient = new UdpClient();
                    _udpClient.ExclusiveAddressUse = false;
                    _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    _udpClient.Client.Bind(_localEndPoint);
                    _udpClient.BeginReceive(OnOscMessageReceived, null);
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"Error al crear UdpClient: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("El ManagedUdpSender no pudo ser inicializado.");
            }

            // Configurar el temporizador para enviar mensajes OSC periódicamente
            _oscSendTimer = new System.Timers.Timer(1000); // Enviar mensaje cada 1 segundo
            _oscSendTimer.Elapsed += (sender, e) => SendMessage("/avatar/ping", 1); // Envía un mensaje de ping, por ejemplo
            _oscSendTimer.Start();
        }

        public Action<List<string>> OnColliderListReceived { get; internal set; }

        public void SendMessage(string address, params object[] args)
        {
            if (_sender != null)
            {
                var messageData = new MyMessageData { MessageAddress = address, MessageData = args };
                _sender.Send(messageData);
            }
            else
            {
                Console.WriteLine("No se puede enviar el mensaje porque _sender es null.");
            }
        }

        public class MyMessageData
        {
            public string MessageAddress { get; set; }
            public object[] MessageData { get; set; }

            public void Write(BinaryWriter writer)
            {
                writer.Write(MessageAddress);
                foreach (var item in MessageData)
                {
                    if (item is int intValue)
                    {
                        writer.Write(intValue);
                    }
                    else if (item is float floatValue)
                    {
                        writer.Write(floatValue);
                    }
                    else if (item is string strValue)
                    {
                        writer.Write(strValue);
                    }
                    else
                    {
                        throw new InvalidOperationException("Tipo de dato no soportado.");
                    }
                }
            }
        }

        private void OnOscMessageReceived(IAsyncResult ar)
        {
            if (!_formLoaded)
            {
                // Si el formulario no está listo, evitar llamar Invoke
                return;
            }

            try
            {
                IPEndPoint remoteEP = null;
                byte[] data = _udpClient.EndReceive(ar, ref remoteEP);

                var message = Encoding.UTF8.GetString(data);
                Console.WriteLine("Mensaje OSC recibido: " + message);

                var colliders = ParseColliderListFromMessage(message);

                if (colliders != null && colliders.Count > 0)
                {
                    try
                    {
                        if (_form != null && !_form.IsDisposed && _form.IsHandleCreated)
                        {
                            _form.Invoke(new Action(() =>
                            {
                                Console.WriteLine("Colliders recibidos correctamente:");
                                foreach (var collider in colliders)
                                {
                                    Console.WriteLine(collider);
                                }
                                OnColliderListReceived?.Invoke(colliders);
                            }));
                        }
                    }
                    catch (InvalidAsynchronousStateException ex)
                    {
                        Console.WriteLine($"Error invocando el método en el formulario: {ex.Message}");
                    }
                }
                else
                {
                    try
                    {
                        if (_form != null && !_form.IsDisposed && _form.IsHandleCreated)
                        {
                            _form.Invoke(new Action(() =>
                            {
                                Console.WriteLine("No se recibieron colliders o la lista está vacía.");
                            }));
                        }
                    }
                    catch (InvalidAsynchronousStateException ex)
                    {
                        Console.WriteLine($"Error invocando el método en el formulario: {ex.Message}");
                    }
                }

                _udpClient.BeginReceive(OnOscMessageReceived, null);
            }
            catch (Exception ex)
            {
                try
                {
                    if (_form != null && !_form.IsDisposed && _form.IsHandleCreated)
                    {
                        _form.Invoke(new Action(() =>
                        {
                            Console.WriteLine($"Error recibiendo datos OSC: {ex.Message}");
                        }));
                    }
                }
                catch (InvalidAsynchronousStateException invokeEx)
                {
                    Console.WriteLine($"Error invocando el método en el formulario: {invokeEx.Message}");
                }
            }
        }

        private List<string> ParseColliderListFromMessage(string message)
        {
            return message.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        // Implementación de IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    _udpClient?.Close();
                    _udpClient?.Dispose();
                    _sender?.Dispose();
                }

                // Dispose unmanaged resources

                _disposed = true;
            }
        }
    }
}