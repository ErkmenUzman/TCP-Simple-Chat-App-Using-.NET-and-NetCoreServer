using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using TcpClient = NetCoreServer.TcpClient;
using System.Threading.Tasks;

namespace ChatTCPIP
{
    public static class AppSession
    {
        public static string Username { get; set; } = string.Empty;
    }

    class ChatClient : TcpClient
    {
        public ChatClient(string address, int port) : base(address, port) { }

        public void DisconnectAndStop()
        {
            _stop = true;
            DisconnectAsync();
            while (IsConnected)
                Thread.Yield();
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
            // Pass the raw data straight to the Form's handler
            FrmChat.Instance.OnDataReceived(buffer, offset, size);
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP client caught an error with code {error}");
        }

        private bool _stop;
    }
}
