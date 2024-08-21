using Jaeger.Senders.Thrift;

namespace MATAppDesktop.Services
{
    internal class UDPSender : UdpSender
    {
        private string ipAddress;
        private int port;

        public UDPSender(string ipAddress, int port)
        {
            this.ipAddress = ipAddress;
            this.port = port;
        }

        internal void Send(Message message)
        {
            throw new NotImplementedException();
        }

        internal void Send(OscManager.MyMessageData messageData)
        {
            throw new NotImplementedException();
        }
    }
}