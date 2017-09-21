using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatNet.Helpers
{
    public static class TcpHelper
    {
        private static TcpListener listener { get; set; }
        private static bool accept { get; set; } = false;

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

                    if (listener.Pending())
                    {
                        TcpClient tcpClient = listener.AcceptTcpClient();
                        Console.WriteLine($"I am connecting to " + 
                            IPAddress.Parse(((IPEndPoint)listener.LocalEndpoint).Address.ToString()));
                    } 
                    // var clientTask = listener.AcceptTcpClientAsync(); // Get the client  
     
                    // if (clientTask.Result != null)
                    // {
                    //     Console.WriteLine("Client connected. Waiting for data.");
                    //     var client = clientTask.Result;
                    //     string message = "";

                    //     while (message != null && !message.Contains("quit"))
                    //     {
                    //         byte[] data = Encoding.ASCII.GetBytes($"Chat: ");
                    //         client.GetStream().Write(data, 0, data.Length);

                    //         byte[] buffer = new byte[1024];
                    //         client.GetStream().Read(buffer, 0, buffer.Length);

                    //         message = $"{DateTime.Now.ToShortTimeString()} {Encoding.ASCII.GetString(buffer)}";                            
                    //         Console.WriteLine(message);
                    //     }
                    //     Console.WriteLine("Closing connection.");
                    //     client.GetStream().Dispose();
                    // }
                }
            }
        }
    }
}
