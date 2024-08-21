using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace MATAppDesktop.Services
{
    public class NetworkScanner
    {
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

