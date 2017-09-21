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
            //TcpHelper.StartServer(7777);
            //TcpHelper.Listen();
            System.Console.WriteLine(IpHelper.GetLocalIp());
        }
    }
}
