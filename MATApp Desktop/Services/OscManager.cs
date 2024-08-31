using CoreOSC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MATAppDesktop.Services
{
    public class OscManager
    {
        public string OscAddress { get; set; }
        public object[] OscData { get; set; }
        private readonly string _ipAddress;
        private readonly int _port;
        private ManagedUdpSender _sender;
        private UdpClient _udpClient;
        private IPEndPoint _localEndPoint;

        public OscManager(string ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;

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
                    _udpClient.ExclusiveAddressUse = false; // Permitir reutilización del socket
                    _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    _udpClient.Client.Bind(_localEndPoint); // Enlazar al puerto
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
            try
            {
                IPEndPoint remoteEP = null;
                byte[] data = _udpClient.EndReceive(ar, ref remoteEP);

                // Decodifica el mensaje OSC
                var message = Encoding.UTF8.GetString(data);
                Console.WriteLine("Mensaje OSC recibido: " + message); // Log del mensaje recibido

                // Procesar la lista de colliders desde el mensaje
                var colliders = ParseColliderListFromMessage(message);

                // Verifica si la lista de colliders es la esperada
                if (colliders != null && colliders.Count > 0)
                {
                    Console.WriteLine("Colliders recibidos correctamente:");
                    foreach (var collider in colliders)
                    {
                        Console.WriteLine(collider); // Imprime cada collider recibido
                    }
                    OnColliderListReceived?.Invoke(colliders); // Invoca la acción con la lista de colliders
                }
                else
                {
                    Console.WriteLine("No se recibieron colliders o la lista está vacía.");
                }

                // Continuar recibiendo datos
                _udpClient.BeginReceive(OnOscMessageReceived, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error recibiendo datos OSC: {ex.Message}");
            }
        }

        private List<string> ParseColliderListFromMessage(string message)
        {
            // Supongamos que los colliders están separados por comas
            return message.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}