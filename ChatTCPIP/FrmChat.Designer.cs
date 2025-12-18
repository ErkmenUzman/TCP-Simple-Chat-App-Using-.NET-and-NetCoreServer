namespace ChatTCPIP
{
    partial class FrmChat
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            UsersListBox = new ListBox();
            MessageLogBox = new RichTextBox();
            txtMessageBox = new TextBox();
            UsersLabel = new Label();
            SendButton = new Button();
            ChatLabel = new Label();
            SuspendLayout();
            // 
            // UsersListBox
            // 
            UsersListBox.FormattingEnabled = true;
            UsersListBox.ItemHeight = 15;
            UsersListBox.Location = new Point(12, 27);
            UsersListBox.Name = "UsersListBox";
            UsersListBox.Size = new Size(144, 349);
            UsersListBox.TabIndex = 0;
            // 
            // MessageLogBox
            // 
            MessageLogBox.Location = new Point(162, 27);
            MessageLogBox.Name = "MessageLogBox";
            MessageLogBox.ReadOnly = true;
            MessageLogBox.Size = new Size(638, 319);
            MessageLogBox.TabIndex = 1;
            MessageLogBox.Text = "";
            MessageLogBox.TextChanged += MessageLogBox_TextChanged;
            // 
            // txtMessageBox
            // 
            txtMessageBox.Location = new Point(162, 352);
            txtMessageBox.Name = "txtMessageBox";
            txtMessageBox.Size = new Size(557, 23);
            txtMessageBox.TabIndex = 2;
            // 
            // UsersLabel
            // 
            UsersLabel.AutoSize = true;
            UsersLabel.Location = new Point(12, 9);
            UsersLabel.Name = "UsersLabel";
            UsersLabel.Size = new Size(35, 15);
            UsersLabel.TabIndex = 3;
            UsersLabel.Text = "Users";
            // 
            // SendButton
            // 
            SendButton.Cursor = Cursors.Hand;
            SendButton.Enabled = false;
            SendButton.Location = new Point(725, 352);
            SendButton.Name = "SendButton";
            SendButton.Size = new Size(75, 23);
            SendButton.TabIndex = 4;
            SendButton.Text = "Send";
            SendButton.UseVisualStyleBackColor = true;
            SendButton.Click += SendButton_Click;
            // 
            // ChatLabel
            // 
            ChatLabel.AutoSize = true;
            ChatLabel.Location = new Point(162, 9);
            ChatLabel.Name = "ChatLabel";
            ChatLabel.Size = new Size(32, 15);
            ChatLabel.TabIndex = 5;
            ChatLabel.Text = "Chat";
            // 
            // FrmChat
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(814, 385);
            Controls.Add(ChatLabel);
            Controls.Add(SendButton);
            Controls.Add(UsersLabel);
            Controls.Add(txtMessageBox);
            Controls.Add(MessageLogBox);
            Controls.Add(UsersListBox);
            Name = "FrmChat";
            StartPosition = FormStartPosition.CenterParent;
            Text = "ChatSession";
            Load += FrmChat_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox UsersListBox;
        private RichTextBox MessageLogBox;
        private TextBox txtMessageBox;
        private Label UsersLabel;
        private Button SendButton;
        private Label ChatLabel;
    }
}
