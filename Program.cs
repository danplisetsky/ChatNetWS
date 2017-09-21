using System;
using System.Net;
using System.Net.NetworkInformation;
using ChatNet.Helpers;

namespace ChatNet
{
    class Program
    {
        static void Main(string[] args)
        {
            var ip = IpHelper.GetLocalIp();
            TcpHelper.StartServer(ip, 7777);
            TcpHelper.Listen();
        }
    }
}
