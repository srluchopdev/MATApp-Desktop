using CoreOSC;
using System;
using System.IO;
using System.Net.Sockets;

namespace MATAppDesktop.Services
{
    internal class ManagedUdpSender : IDisposable
    {
        private readonly UdpClient _udpClient;
        private readonly string _ipAddress;
        private readonly int _port;
        private readonly int _maxPacketSize;

        public ManagedUdpSender(string ipAddress, int port, int maxPacketSize = 1024)
        {
            _ipAddress = ipAddress;
            _port = port;
            _maxPacketSize = maxPacketSize;
            _udpClient = new UdpClient();
            _udpClient.Connect(_ipAddress, _port);
        }

        internal void Send(OscManager.MyMessageData messageData)
        {
            try
            {
                byte[] data = ConvertMessageToBytes(messageData);
                if (data.Length > _maxPacketSize)
                {
                    Console.WriteLine("El paquete excede el tamaño máximo permitido.");
                    return;
                }
                _udpClient.Send(data, data.Length);
            }
            catch (IOException ex)
            {
                Console.WriteLine("Error al escribir en el flujo: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inesperado: " + ex.Message);
            }
        }

        private byte[] ConvertMessageToBytes(OscManager.MyMessageData messageData)
        {
            using (var memoryStream = new MemoryStream())
            {
                var writer = new BinaryWriter(memoryStream);
                messageData.Write(writer); // Cambiado a Write
                return memoryStream.ToArray();
            }
        }

        public void Dispose()
        {
            _udpClient?.Dispose();
        }
    }
}