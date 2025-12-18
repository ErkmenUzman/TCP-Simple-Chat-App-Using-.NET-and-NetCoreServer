using System;
using System.Net;
using System.Text;
using NetCoreServer;
using System.Configuration;
using System.Collections.Concurrent;

namespace ChatTCPIP
{
    // 1. Logic for an individual user connection
    class ChatSession : TcpSession
    {
        public ChatSession(TcpServer server) : base(server) { }

        protected override void OnConnected()
        {
            Console.WriteLine($"Chat session with ID {Id} connected!");
        }

        protected override void OnDisconnected()
        {
            var server = (ChatServer)Server;

            // Remove the user so they aren't in the list anymore
            if (server.ConnectedUsers.TryRemove(Id, out string? username))
            {
                Console.WriteLine($"{username} left. Updating list...");
                server.UpdateUserList();
            }
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            var server = (ChatServer)Server;

            if (message.StartsWith("JOIN|"))
            {
                string username = message.Substring(5).Trim();
                server.ConnectedUsers[Id] = username;
                server.UpdateUserList();
            }
            else if (message.StartsWith("MSG|"))
            {
                // Split: [0]MSG, [1]TargetUser, [2]Content
                var parts = message.Split('|');
                if (parts.Length >= 3)
                {
                    string targetName = parts[1];
                    string content = parts[2];
                    string senderName = server.ConnectedUsers.ContainsKey(Id) ? server.ConnectedUsers[Id] : "Unknown";

                    // Find the ID of the target user
                    var targetSessionId = server.ConnectedUsers.FirstOrDefault(x => x.Value == targetName).Key;

                    if (targetSessionId != Guid.Empty)
                    {
                        // Send only to the target
                        server.FindSession(targetSessionId)?.SendAsync($"{senderName}: {content}");
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

        public void UpdateUserList()
        {
            // Get all names that aren't empty
            var names = ConnectedUsers.Values.Where(n => !string.IsNullOrEmpty(n)).ToList();

            // Create the protocol string
            string listMessage = "USERLIST|" + string.Join(",", names);

            // Send to everyone
            Multicast(listMessage);

            Console.WriteLine($"[BROADCAST] Sent user list: {string.Join(", ", names)}");
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