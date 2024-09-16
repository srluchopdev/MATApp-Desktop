using OscCore;
using System;
using System.IO;
using System.Net.Sockets;
using System.Net;

namespace MATAppDesktop.Services
{
    internal class ManagedUdpSender : IDisposable
    {
        private readonly UdpClient _udpClient;
        private readonly IPEndPoint _endPoint;
        private readonly int _maxPacketSize;

        public ManagedUdpSender(string ipAddress, int port, int maxPacketSize = 1024)
        {
            _maxPacketSize = maxPacketSize;
            _udpClient = new UdpClient();
            _endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
        }

        internal void Send(OscMessage message)
        {
            try
            {
                byte[] data = PackOscMessage(message);
                if (data.Length > _maxPacketSize)
                {
                    Console.WriteLine("El paquete excede el tamaño máximo permitido.");
                    return;
                }
                _udpClient.Send(data, data.Length, _endPoint);
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

        private byte[] PackOscMessage(OscMessage message)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(memoryStream))
                {
                    writer.Write(message.Address);
                    writer.Write((byte)',');
                    foreach (var arg in message)
                    {
                        if (arg is int intValue)
                        {
                            writer.Write('i');
                            writer.Write(intValue);
                        }
                        else if (arg is float floatValue)
                        {
                            writer.Write('f');
                            writer.Write(floatValue);
                        }
                        else if (arg is string stringValue)
                        {
                            writer.Write('s');
                            writer.Write(stringValue);
                        }
                        else if (arg is byte[] blobValue)
                        {
                            writer.Write('b');
                            writer.Write(blobValue.Length);
                            writer.Write(blobValue);
                        }
                        else
                        {
                            throw new InvalidOperationException("Tipo de argumento OSC no soportado.");
                        }
                    }
                }
                return memoryStream.ToArray();
            }
        }

        public void Dispose()
        {
            _udpClient?.Dispose();
        }
    }
}