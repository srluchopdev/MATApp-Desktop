using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

namespace MATAppDesktop.Services
{
    public class NetworkScanner
    {
        private System.Threading.Timer _timer;
        private readonly TimeSpan _scanInterval = TimeSpan.FromSeconds(90); // 1 minuto y medio

        public NetworkScanner()
        {
            _timer = new System.Threading.Timer(ScanNetwork, null, TimeSpan.Zero, _scanInterval);
        }

        private void ScanNetwork(object state)
        {
            List<string> ips = ScanNetworkForSlimeVRDevices();
            // Aquí puedes hacer algo con las IPs encontradas, como actualizar una UI o almacenar en un archivo
            Console.WriteLine("IPs encontradas: " + string.Join(", ", ips));
        }

        public List<string> ScanNetworkForSlimeVRDevices()
        {
            List<string> ips = new List<string>();

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        // Lógica para encontrar dispositivos SlimeVR en la red
                        ips.Add(ip.Address.ToString());
                    }
                }
            }
            return ips;
        }
    }
}