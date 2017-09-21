using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatNet.Helpers
{
    public static class TcpHelper
    {
        private static TcpListener listener;
        private static bool accept = false;

        private static Dictionary<EndPoint, TcpClient> clients = new Dictionary<EndPoint, TcpClient>();

        public static void StartServer(string ip, int port)
        {
            IPAddress address = IPAddress.Parse(ip);
            listener = new TcpListener(address, port);

            listener.Start();
            accept = true;

            Console.WriteLine($"Server started. Listening to TCP clients at {ip}:{port}");
        }

        public static void Listen()
        {
            if (listener != null && accept)
            {
                Console.WriteLine("Waiting for clients...");
                while (true)
                {
                    var clientTask = listener.AcceptTcpClientAsync();

                    WorkWithClients();

                    if (clientTask.Result != null)
                    {
                        var client = clientTask.Result;
                        clients.Add(client.Client.RemoteEndPoint, client);
                        System.Console.WriteLine($"Client {client.Client.RemoteEndPoint} connected");
                    }
                }
            }
        }

        private static void WorkWithClients()
        {
            clients.Keys.ToList().ForEach(async ep =>
            {
                await WaitForMessage(clients[ep]);
            });
        }

        async static Task WaitForMessage(TcpClient client)
        {
            string message = "";
            await Task.Run(() =>
                       {
                           try
                           {
                               while (message != null && !message.Contains("/quit"))
                               {
                                   byte[] data = Encoding.ASCII.GetBytes($"Chat: ");
                                   client.GetStream().Write(data, 0, data.Length);

                                   byte[] buffer = new byte[1024];
                                   client.GetStream().Read(buffer, 0, buffer.Length);

                                   message = $"{DateTime.Now.ToShortTimeString()} {Encoding.ASCII.GetString(buffer)}";
                                   Console.WriteLine(message);
                               }
                           }
                           finally
                           {
                               Console.WriteLine("Closing connection.");
                               client.GetStream().Dispose();
                           }
                       });
        }
    }
}
