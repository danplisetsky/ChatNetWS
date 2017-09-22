using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using ChatNetWS.Models;

namespace ChatNetWS.Helpers
{
    public static class TcpHelper
    {
        private static TcpListener listener;
        private static bool accept = false;

        private static List<User> clients = new List<User>();

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


                    //WorkWithClients();

                    if (clientTask.Result != null)
                    {
                        var client = clientTask.Result;
                        clients.Add(new User(client.Client.RemoteEndPoint, client));
                        System.Console.WriteLine($"Client {client.Client.RemoteEndPoint} connected");

                        var stream = client.GetStream();

                        var requestList = new List<byte>();

                        //wait until there is data in the stream
                        while (!stream.DataAvailable) { }

                        //read everything in the stream
                        while (stream.DataAvailable)
                        {
                            requestList.Add((byte)stream.ReadByte());
                        }
                        //send response
                        System.Console.WriteLine(Encoding.UTF8.GetString(requestList.ToArray()));
                        byte[] response = HandshakeHelper.GenerateResponse(requestList.ToArray());
                        stream.Write(response, 0, response.Length);

                        System.Console.WriteLine("Response:");
                        System.Console.WriteLine(Encoding.UTF8.GetString(response.ToArray()));
                    }
                }
            }
        }

        private static void WorkWithClients()
        {
            clients.Where(user => !user.Listening).ToList().ForEach(async user =>
            {
                user.Listening = true;
                await WaitForMessage(user);
            });
        }

        async static Task WaitForMessage(User user)
        {
            string message = "";
            await Task.Run(() =>
                       {
                           var client = user.TcpClient;
                           try
                           {
                               while (message != null && !message.Contains("/quit"))
                               {
                                   //byte[] data = Encoding.ASCII.GetBytes($"HipChat: ");
                                   //client.GetStream().Write(data, 0, data.Length);

                                   byte[] buffer = new byte[1024];
                                   client.GetStream().Read(buffer, 0, buffer.Length);

                                   message = $"{DateTime.Now.ToShortTimeString()} {Encoding.ASCII.GetString(buffer)}";
                                   Console.WriteLine(message);

                                   clients.Where(other => user != other).ToList()
                                    .ForEach(u =>
                                    {
                                        var nl = Encoding.ASCII.GetBytes(Environment.NewLine);
                                        u.TcpClient.GetStream().Write(nl, 0, nl.Length);
                                        u.TcpClient.GetStream().Write(buffer, 0, buffer.Length);
                                    });
                               }
                           }
                           finally
                           {
                               Console.WriteLine($"Closing connection with {user.EndPoint}");
                               client.GetStream().Dispose();
                               clients.Remove(user);
                           }
                       });
        }
    }
}
