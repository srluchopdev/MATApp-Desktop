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

            // Configuración del punto de enlace local (EndPoint)
            _localEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), _port);

            // Creación del UdpClient
            _udpClient = new UdpClient();

            // Configurar el socket para reutilización de la dirección
            _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            // Asociar el UdpClient al punto de enlace local
            _udpClient.Client.Bind(_localEndPoint);

            // Comenzar a recibir datos
            _udpClient.BeginReceive(OnOscMessageReceived, null);
        }

        public Action<List<string>> OnColliderListReceived { get; internal set; }

        public void SendMessage(string address, params object[] args)
        {
            // Create a custom data structure to encapsulate address and data
            var messageData = new MyMessageData { MessageAddress = address, MessageData = args };

            // Assuming CoreOSC provides a method to send messages with custom data
            _sender.Send(messageData); // Replace with the appropriate method from CoreOSC
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
