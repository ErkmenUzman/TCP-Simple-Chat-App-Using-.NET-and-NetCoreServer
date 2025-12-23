using Microsoft.VisualBasic;
using System.Configuration;
using System.Net;
using System.Text;

namespace ChatTCPIP
{
    public partial class FrmChat : Form
    {

        private static FrmChat? _instance;
        protected NetCoreServer.TcpClient? _tcpClient;

        public static FrmChat Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new FrmChat();
                }
                return _instance;
            }
        }

        public FrmChat()
        {
            InitializeComponent();

            // Read values with fallbacks to avoid crashes
            string host = System.Configuration.ConfigurationManager.AppSettings["Host"] ?? "127.0.0.1";
            string portRaw = System.Configuration.ConfigurationManager.AppSettings["Port"] ?? "9000";

            // Safely convert the port
            if (!int.TryParse(portRaw, out int port))
            {
                port = 9000; // Fallback port
            }

            _tcpClient = new ChatClient(host, port);
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            if (UsersListBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a user.");
                return;
            }

            var targetUser = UsersListBox.SelectedItem.ToString();
            var message = txtMessageBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(message)) return;

            ((ChatClient)_tcpClient)?.SendPacket($"MSG|{targetUser}|{message}");

            //AppendLog($"To {targetUser}: {message}");
            txtMessageBox.Clear();
        }

        private void FrmChat_Load(object sender, EventArgs e)
        {
            this.Text = $"Joined as: {AppSession.Username}";
            _tcpClient?.ConnectAsync();
        }

        // This is called from the Client thread when the connection is REAL
        public void OnServerConnected()
        {
            // NetCoreServer runs on a background thread, so we MUST Invoke to touch UI
            this.Invoke(new Action(() =>
            {
                AppendLog("Connected to the server.");
                var joinMessage = $"JOIN|{AppSession.Username}";
                ((ChatClient)_tcpClient)?.SendPacket(joinMessage);
                SendButton.Enabled = true;
            }));
        }

        public void OnServerDisconnected()
        {
            this.Invoke(new Action(() =>
            {
                AppendLog("Disconnected from the server.");
                SendButton.Enabled = false;
            }));
        }

        public void OnDataReceived(byte[] buffer, long offset, long size)
        {
            // Convert the byte array to a string
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);

            if (message.StartsWith("USERLIST|"))
            {
                // Extract the part after the pipe and split by comma
                string userPayload = message.Substring("USERLIST|".Length);
                string[] users = userPayload.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                this.Invoke(new Action(() =>
                {
                    UsersListBox.BeginUpdate(); // Prevents flickering during mass update
                    UsersListBox.Items.Clear();

                    foreach (var user in users)
                    {
                        string trimmedUser = user.Trim();
                        // Only add if it's not the current logged-in user
                        if (!string.Equals(trimmedUser, AppSession.Username, StringComparison.OrdinalIgnoreCase))
                        {
                            UsersListBox.Items.Add(trimmedUser);
                        }
                    }
                    UsersListBox.EndUpdate();
                }));
            }
            else
            {
                // If it's not a user list, it's a regular message
                AppendLog(message);
            }
        }

        private void AppendLog(string message)
        {
            if (MessageLogBox.InvokeRequired) { MessageLogBox.Invoke(new Action(() => AppendLog(message))); }
            else { MessageLogBox.AppendText(message + Environment.NewLine); }
        }
    }
}
