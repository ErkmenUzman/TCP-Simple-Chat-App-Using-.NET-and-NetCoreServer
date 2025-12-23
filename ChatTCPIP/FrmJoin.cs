using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatTCPIP
{
    public partial class FrmJoin : Form
    {
        public FrmJoin()
        {
            InitializeComponent();
        }

        private void JoinButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(JoinTextBox.Text))
            {
                MessageBox.Show("Please enter a username.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string name = JoinTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || name.Contains('|') || name.Contains(','))
            {
                MessageBox.Show("Invalid username.");
                return;
            }

            AppSession.Username = name;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void JoinTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(JoinTextBox.Text))
            {
                JoinButton.Enabled = true;
            }
        }
    }
}
