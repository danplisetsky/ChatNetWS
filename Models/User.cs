using System.Net;
using System.Net.Sockets;

namespace ChatNetWS.Models
{
    public class User
    {
        public EndPoint EndPoint {get; }
        public TcpClient TcpClient {get; }        
        public bool Listening { get; set; } = false;

        public User(EndPoint endPoint, TcpClient tcpClient)
        {
            this.EndPoint = endPoint;
            this.TcpClient = tcpClient;            
        }
    }
}