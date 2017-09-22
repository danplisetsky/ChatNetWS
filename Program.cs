using System;
using System.Net;
using System.Net.NetworkInformation;
using ChatNetWS.Helpers;

using WebSocketSharp;
using WebSocketSharp.Server;

namespace ChatNetWS
{
    class Program
    {
        public class Laputa : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {
                var msg = e.Data == "BALUS"
                          ? "I've been balused already..."
                          : "I'm not available now.";

                Send(msg);
            }
        }

        static void Main(string[] args)
        {
            var ip = IpHelper.GetLocalIp();

            var wssv = new WebSocketServer($"ws://127.0.0.1:7777");
            wssv.AddWebSocketService<Laputa>("/Laputa");
            wssv.Start();
            Console.ReadLine();
            wssv.Stop();


            //TcpHelper.StartServer(ip, 7777);
            //TcpHelper.Listen();
        }
    }
}
