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
        public string OscAddress { get; set; } // Cambié el nombre para evitar ambigüedad
        public object[] OscData { get; set; } // Cambié el nombre para evitar ambigüedad
        private readonly string _ipAddress;
        private readonly int _port;
        private ManagedUdpSender _sender;
        private UdpClient _udpClient;
        private IPEndPoint _localEndPoint;

        public OscManager(string ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;

            // Inicializa el ManagedUdpSender aquí
            try
            {
                _sender = new ManagedUdpSender(_ipAddress, _port);
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Error al crear ManagedUdpSender: {ex.Message}");
                // Maneja adecuadamente la excepción de creación del socket
            }

            // Solo continúa si _sender fue inicializado correctamente
            if (_sender != null)
            {
                _udpClient = new UdpClient(_port);
                _udpClient.BeginReceive(OnOscMessageReceived, null);
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
                _sender.Send(messageData); // Asegúrate de que _sender no sea null
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

            public void Write(BinaryWriter writer) // Asegúrate de que este método está definido
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
                var colliders = ParseColliderListFromMessage(message);

                // Llama al evento cuando se reciban los colliders
                OnColliderListReceived?.Invoke(colliders);

                // Continuar recibiendo datos
                _udpClient.BeginReceive(OnOscMessageReceived, null);
            }
            catch (Exception ex)
            {
                // Manejar excepciones aquí si es necesario
                Console.WriteLine($"Error recibiendo datos OSC: {ex.Message}");
            }
        }

        private List<string> ParseColliderListFromMessage(string message)
        {
            // Asume que los colliders están en una lista separada por comas en el mensaje recibido
            return message.Split(',').ToList();
        }
    }
}
