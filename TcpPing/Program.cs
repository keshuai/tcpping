using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TcpPing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("args error, use ping host port, like: ping www.baidu.com 80");
                return;
            }

            var ipAddressStr = args[0];
            var portStr = args[1];

            if (IPAddress.TryParse(ipAddressStr, out var ipAddress))
            {
            }
            else if (TryGetIpAddress(ipAddressStr, out ipAddress))
            {
            }
            else
            {
                Console.WriteLine($"ip error: {ipAddressStr}");
                return;
            }

            if (!ushort.TryParse(portStr, out var port))
            {
                Console.WriteLine($"port error: {portStr}");
            }

            var remote = new IPEndPoint(ipAddress, port);
            Ping(remote, $"ping {remote} ");

            Thread.CurrentThread.Join();
        }

        static bool TryGetIpAddress(string host, out IPAddress ipAddress)
        {
            ipAddress = null;
            try
            {
                var ipAddresses = Dns.GetHostAddresses(host);
                if (ipAddresses.Length > 0)
                {
                    ipAddress = ipAddresses[0];
                    return true;
                }
            }
            catch
            {
            }
            
            return false;
        }

        static async void Ping(IPEndPoint remote, string logPrefix)
        {
            while (true)
            {
                var cost = await ConnectAsync(remote, 1000, logPrefix);
                var delay = 1000 - cost;
                if (delay > 0)
                {
                    await Task.Delay(delay);
                }
            }
        }

        static async Task<int> ConnectAsync(IPEndPoint remote, int timeout, string logPrefix)
        {
            int time = -1;
            try
            {
                var ticks = DateTime.UtcNow.Ticks;
                var socket = new Socket(remote.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                var connectTask = socket.ConnectAsync(remote);
                var timeOutTask = Task.Delay(timeout);
                await Task.WhenAny(connectTask, timeOutTask);

                time = (int)((DateTime.UtcNow.Ticks - ticks) / 10000);
                
                if (socket.Connected)
                {
                    Console.WriteLine($"{logPrefix}success: {time}ms");
                }
                else
                {
                    Console.WriteLine($"{logPrefix}timeout: {time}ms");
                }
                
                socket.Close();
            }
            catch
            {
            }

            return time;
        }
    }
}
