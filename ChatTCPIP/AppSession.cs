using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using TcpClient = NetCoreServer.TcpClient;
using System.Threading.Tasks;
using System.Net;

namespace ChatTCPIP
{
    public static class AppSession
    {
        public static string Username { get; set; } = string.Empty;
    }

    class ChatClient : TcpClient
    {
        public ChatClient(string address, int port) : base(address, port) { }

        private readonly List<byte> _receiveBuffer = new List<byte>();

        // NEW: Use this instead of the base SendAsync to include the 4-byte header
        public bool SendPacket(string message)
        {
            byte[] payload = Encoding.UTF8.GetBytes(message);

            // 1. Get the length and convert to Network Byte Order (Big Endian)
            // IPAddress.HostToNetworkOrder is the C# version of htonl()
            byte[] header = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(payload.Length));

            // 2. Combine Header (4 bytes) + Payload
            byte[] packet = new byte[header.Length + payload.Length];
            Buffer.BlockCopy(header, 0, packet, 0, header.Length);
            Buffer.BlockCopy(payload, 0, packet, header.Length, payload.Length);

            // 3. Send the full packet
            return SendAsync(packet);
        }

        public async Task DisconnectAndStopAsync()
        {
            _stop = true;
            DisconnectAsync();
            await Task.Delay(200);
        }

        protected override void OnConnected()
        {
            //Console.WriteLine($"Chat TCP client connected a new session with Id {Id}");
            // Use the Singleton Instance to update the UI safely
            FrmChat.Instance.OnServerConnected();
        }

        protected override void OnDisconnected()
        {
            //Console.WriteLine($"Chat TCP client disconnected a session with Id {Id}");

            // Wait for a while...
            //Thread.Sleep(1000);

            //// Try to connect again
            //if (!_stop)
            //    ConnectAsync();

            FrmChat.Instance.OnServerDisconnected();
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            byte[] incoming = new byte[size];
            Buffer.BlockCopy(buffer, (int)offset, incoming, 0, (int)size);
            _receiveBuffer.AddRange(incoming);

            while (_receiveBuffer.Count >= 4)
            {
                byte[] headerBytes = _receiveBuffer.GetRange(0, 4).ToArray();
                int messageLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(headerBytes, 0));

                if (_receiveBuffer.Count >= 4 + messageLength)
                {
                    byte[] payload = _receiveBuffer.GetRange(4, messageLength).ToArray();
                    _receiveBuffer.RemoveRange(0, 4 + messageLength);

                    FrmChat.Instance.OnDataReceived(payload, 0, payload.Length);
                }
                else
                {
                    break;
                }
            }
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP client caught an error with code {error}");
        }

        private bool _stop;
    }
}
