using System;
using System.Net;
using System.Text;
using NetCoreServer;
using Buffer = System.Buffer;
using System.Configuration;
using System.Collections.Concurrent;

namespace ChatTCPIP
{

    class ChatSession : TcpSession
    {

        private readonly List<byte> _receiveBuffer = new List<byte>();

        public ChatSession(TcpServer server) : base(server) { }

        public bool SendPacket(string message)
        {
            byte[] payload = Encoding.UTF8.GetBytes(message);

            byte[] header = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(payload.Length));

            byte[] packet = new byte[header.Length + payload.Length];
            Buffer.BlockCopy(header, 0, packet, 0, header.Length);
            Buffer.BlockCopy(payload, 0, packet, header.Length, payload.Length);

            return SendAsync(packet);
        }

        protected override void OnConnected()
        {
            Console.WriteLine($"Chat session with ID {Id} connected!");
        }

        protected override void OnDisconnected()
        {
            var server = (ChatServer)Server;

            if (server.ConnectedUsers.TryRemove(Id, out string? username))
            {
                Console.WriteLine($"[LEAVE] {username} (ID: {Id}) disconnected.");

                server.UpdateUserList();
            }
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            // 1. Add new data to our persistent buffer
            byte[] incoming = new byte[size];
            Buffer.BlockCopy(buffer, (int)offset, incoming, 0, (int)size);
            _receiveBuffer.AddRange(incoming);

            // 2. Try to process as many packets as possible
            while (_receiveBuffer.Count >= 4)
            {
                // Read the 4-byte header
                byte[] headerBytes = _receiveBuffer.GetRange(0, 4).ToArray();
                int packetSize = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(headerBytes, 0));

                // Do we have the full payload yet?
                if (_receiveBuffer.Count >= 4 + packetSize)
                {
                    // Extract payload
                    byte[] payloadBytes = _receiveBuffer.GetRange(4, packetSize).ToArray();
                    string message = Encoding.UTF8.GetString(payloadBytes);

                    // Remove processed data from buffer
                    _receiveBuffer.RemoveRange(0, 4 + packetSize);

                    // Handle the logic
                    ProcessMessage(message);
                }
                else
                {
                    // Wait for more data
                    break;
                }
            }
        }

        private void ProcessMessage(string message)
        {
            var server = (ChatServer)Server;

            if (message.StartsWith("JOIN|"))
            {
                string username = message.Substring(5).Trim();
                server.ConnectedUsers[Id] = username;
                server.UpdateUserList();
            }
            else if (message.StartsWith("MSG|"))
            {
                var parts = message.Split('|', 3);
                if (parts.Length >= 3)
                {
                    string targetName = parts[1];
                    string content = parts[2];
                    string senderName = server.ConnectedUsers.GetValueOrDefault(Id, "Unknown");

                    var targetSession = server.GetSessionByName(targetName);
                    if (targetSession != null)
                    {
                        // Send to the Target
                        targetSession.SendPacket($"[PM from {senderName}]: {content}");

                        // Also send a confirmation back to the Sender
                        this.SendPacket($"[To {targetName}]: {content}");
                    }
                    else
                    {
                        this.SendPacket($"[System]: User '{targetName}' is not online.");
                    }
                }
            }
        }

        protected override void OnError(System.Net.Sockets.SocketError error)
        {
            Console.WriteLine($"Chat session error: {error}");
        }
    }

    // 2. Logic for the Server itself
    class ChatServer : TcpServer
    {
        // --- DEFINITION START ---
        // Guid: The unique ID NetCoreServer gives every connection (Session.Id)
        // string: The Username the client sent in the "JOIN" message
        public ConcurrentDictionary<Guid, string> ConnectedUsers = new ConcurrentDictionary<Guid, string>();
        // --- DEFINITION END ---

        public ChatServer(IPAddress address, int port) : base(address, port) { }

        protected override TcpSession CreateSession() { return new ChatSession(this); }

        protected override void OnError(System.Net.Sockets.SocketError error)
        {
            Console.WriteLine($"Chat server error: {error}");
        }

        public ChatSession? GetSessionByName(string name)
        {
            var match = ConnectedUsers
                .FirstOrDefault(x => string.Equals(x.Value, name, StringComparison.OrdinalIgnoreCase));

            if (match.Equals(default(KeyValuePair<Guid, string>)))
                return null;

            return (ChatSession?)FindSession(match.Key);

        }

        public void UpdateUserList()
        {
            var names = ConnectedUsers.Values.Where(n => !string.IsNullOrEmpty(n)).ToList();
            string listMessage = "USERLIST|" + string.Join(",", names);

            Console.WriteLine($"[UPDATE] Total Clients: {names.Count} | List: {string.Join(", ", names)}");

            foreach (var session in Sessions.Values)
            {
                if (session.IsConnected)
                {
                    ((ChatSession)session).SendPacket(listMessage);
                }
            }
        }
    }

    // 3. Entry point to run the server
    class Program
    {
        static void Main(string[] args)
        {
            // 1. Read from App.config with default fallbacks
            string host = ConfigurationManager.AppSettings["Host"] ?? "127.0.0.1";
            string portRaw = ConfigurationManager.AppSettings["Port"] ?? "9000";

            // 2. Parse values safely
            if (!int.TryParse(portRaw, out int port)) port = 9000;

            // Convert string IP to IPAddress object
            if (!IPAddress.TryParse(host, out IPAddress? ipAddress))
            {
                ipAddress = IPAddress.Any; // Fallback to 0.0.0.0
            }

            // 3. Start the Server
            var server = new ChatServer(ipAddress, port);

            Console.WriteLine($"Server starting on port {port}...");
            server.Start();
            Console.WriteLine("Server is live. Press Enter to shut down.");

            Console.ReadLine();

            Console.WriteLine("Stopping server...");
            server.Stop();
        }
    }
}