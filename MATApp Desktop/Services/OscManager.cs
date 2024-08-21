using CoreOSC;
using System.Net;

namespace MATAppDesktop.Services
{
    public class OscManager
    {
        private readonly string _ipAddress;
        private readonly int _port;
        private UDPSender _sender;

        public OscManager(string ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
            _sender = new UDPSender(_ipAddress, _port);
        }

        public void SendMessage(string address, params object[] args)
        {
            // Create a custom data structure to encapsulate address and data
            var messageData = new MyMessageData { Address = address, Data = args };

            // Assuming CoreOSC provides a method to send messages with custom data
            _sender.Send(messageData); // Replace with the appropriate method from CoreOSC
        }

        public class MyMessageData
        {
            public required string Address { get; set; }
            public required object[] Data { get; set; }
        }
    }
}
