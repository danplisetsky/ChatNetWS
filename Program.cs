using System;
using System.Net;
using System.Net.Sockets;

namespace ChatNet
{
    class Program
    {        
        static void Main(string[] args)
        {
            TcpHelper.StartServer(7777);
            TcpHelper.Listen();
        }
    }
}
