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
                // Continue listening.  
                while (true)
                {
                    Console.WriteLine("Waiting for client...");
                    var clientTask = listener.AcceptTcpClientAsync(); // Get the client  

                    WorkWithClients();

                    if (clientTask.Result != null)
                    {
                        var client = clientTask.Result;
                        clients.Add(client.Client.RemoteEndPoint, client);
                        System.Console.WriteLine("You are connected");

                        // Console.WriteLine("Closing connection.");
                        // client.GetStream().Dispose();


                        //System.Console.WriteLine(clientTask.Result);
                        // //clientTask.Result.



                        // Console.WriteLine("Client connected. Waiting for data.");
                        // var client = clientTask.Result;
                        // string message = "";

                        // //while (message != null && !message.Contains("quit"))
                        // {
                        //     byte[] data = Encoding.ASCII.GetBytes($"Chat: ");
                        //     client.GetStream().Write(data, 0, data.Length);

                        //     byte[] buffer = new byte[1024];
                        //     client.GetStream().Read(buffer, 0, buffer.Length);

                        //     message = $"{DateTime.Now.ToShortTimeString()} {Encoding.ASCII.GetString(buffer)}";                            
                        //     Console.WriteLine(message);
                        // }
                        //Console.WriteLine("Closing connection.");
                        //client.GetStream().Dispose();
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
                           while (message != null && !message.Contains("quit"))
                           {
                               byte[] data = Encoding.ASCII.GetBytes($"Chat: ");
                               client.GetStream().Write(data, 0, data.Length);

                               byte[] buffer = new byte[1024];
                               client.GetStream().Read(buffer, 0, buffer.Length);

                               message = $"{DateTime.Now.ToShortTimeString()} {Encoding.ASCII.GetString(buffer)}";
                               Console.WriteLine(message);
                           }
                           Console.WriteLine("Closing connection.");
                           client.GetStream().Dispose();
                       });
        }
    }
}
