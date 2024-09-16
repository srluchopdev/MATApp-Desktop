using OscCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace MATAppDesktop.Services
{
    public class OscManager : IDisposable
    {
        private bool _disposed = false; // Para detectar llamadas redundantes
        private UdpClient _udpClient;
        private IPEndPoint _endPoint;
        private System.Threading.Timer _timer; // Si estás usando un temporizador

        public OscManager(string ipAddress, int port, int sendPort, MATApp_Desktop.Form1 form1)
        {
            _udpClient = new UdpClient();
            _endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            _timer = new System.Threading.Timer(SendPeriodicMessage, null, 0, 1000); // Enviar mensaje cada segundo
        }

        public Action<List<string>> OnColliderListReceived { get; internal set; }

        // Implementación del método Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Liberar recursos administrados
                _udpClient?.Close();
                _timer?.Dispose();
            }

            // Liberar recursos no administrados (si hay)

            _disposed = true;
        }

        ~OscManager()
        {
            Dispose(false);
        }

        private void SendPeriodicMessage(object state)
        {
            SendMessage("/avatar/requestColliders", 1); // 1 es solo un valor de ejemplo para iniciar la solicitud
        }

        public void SendMessage(string address, params object[] args)
        {
            var message = new OscMessage(address, args);
            byte[] data = PackOscMessage(message);
            _udpClient.Send(data, data.Length, _endPoint);
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
    }
}