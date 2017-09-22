using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ChatNetWS.Helpers
{
    public static class HandshakeHelper
    {
        private static readonly string _serverUrl = $"ws://{IpHelper.GetLocalIp()}:7777/";

        public static byte[] GenerateResponse(byte[] request)
        {
            //extract request token from end of request
            byte[] requestToken = new byte[8];
            Array.Copy(request, request.Length - 8, requestToken, 0, 8);

            string requestString = Encoding.UTF8.GetString(request);
            StringBuilder response = new StringBuilder();
            response.Append("HTTP/1.1 101 Switching Protocols\r\n");
            response.Append("Upgrade: websocket\r\n");
            response.Append("Connection: Upgrade\r\n");
            //response.AppendFormat("Sec-WebSocket-Origin: {0}\r\n", GetOrigin(requestString));
            //response.AppendFormat("Sec-WebSocket-Location: {0}\r\n", _serverUrl);
            //response.Append("\r\n");

            string responseToken = GenerateResponseToken(GetKey1(requestString), requestToken);
            response.Append($"Sec-WebSocket-Accept: {responseToken}\r\n");
            response.Append("Sec-WebSocket-Extensions: permessage-deflate; client_max_window_bits");
            return Encoding.UTF8.GetBytes(response.ToString()).ToArray();
        }

        public static string GetOrigin(string request)
        {
            return Regex.Match(request, @"(?<=Origin:\s).*(?=\r\n)").Value;
        }

        public static string GetKey1(string request)
        {
            return Regex.Match(request, @"(?<=Sec-WebSocket-Key:\s).*(?=\r\n)").Value;
        }

        public static string GetKey2(string request)
        {
            return Regex.Match(request, @"(?<=Sec-WebSocket-Key:\s).*(?=\r\n)").Value;
        }

        public static string GenerateResponseToken(string key, byte[] request_token)
        {
            string magicKey = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

            string finKey = key + magicKey;

            SHA1 hashProvider = new SHA1CryptoServiceProvider();
            byte[] keyForBase64 = hashProvider.ComputeHash(Encoding.UTF8.GetBytes(finKey));

            return Convert.ToBase64String(keyForBase64);

            // int part1 = (int)(ExtractNums(key1) / 3);//CountSpaces(key1));
            // int part2 = (int)(ExtractNums(key2) / 4);//CountSpaces(key2));
            // byte[] key1CalcBytes = ReverseBytes(BitConverter.GetBytes(part1));
            // byte[] key2CalcBytes = ReverseBytes(BitConverter.GetBytes(part2));
            // byte[] sum = key1CalcBytes
            //             .Concat(key2CalcBytes)
            //             .Concat(request_token).ToArray();

            // return new MD5CryptoServiceProvider().ComputeHash(sum);
        }

        public static int CountSpaces(string key)
        {
            return key.Count(c => c == ' ');
        }

        public static long ExtractNums(string key)
        {
            char[] nums = key.Where(c => Char.IsNumber(c)).ToArray();
            return long.Parse(new String(nums));
        }

        //converts to big endian
        private static byte[] ReverseBytes(byte[] inArray)
        {
            byte temp;
            int highCtr = inArray.Length - 1;

            for (int ctr = 0; ctr < inArray.Length / 2; ctr++)
        {
                temp = inArray[ctr];
                inArray[ctr] = inArray[highCtr];
                inArray[highCtr] = temp;
                highCtr -= 1;
            }
            return inArray;
        }
    }
}