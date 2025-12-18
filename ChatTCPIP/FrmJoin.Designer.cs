namespace ChatTCPIP
{
    partial class FrmJoin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            JoinTextBox = new TextBox();
            JoinLabel = new Label();
            JoinButton = new Button();
            SuspendLayout();
            // 
            // JoinTextBox
            // 
            JoinTextBox.Location = new Point(81, 16);
            JoinTextBox.Name = "JoinTextBox";
            JoinTextBox.Size = new Size(246, 23);
            JoinTextBox.TabIndex = 0;
            JoinTextBox.TextChanged += JoinTextBox_TextChanged;
            // 
            // JoinLabel
            // 
            JoinLabel.AutoSize = true;
            JoinLabel.Location = new Point(12, 19);
            JoinLabel.Name = "JoinLabel";
            JoinLabel.Size = new Size(63, 15);
            JoinLabel.TabIndex = 1;
            JoinLabel.Text = "Username:";
            // 
            // JoinButton
            // 
            JoinButton.Cursor = Cursors.Hand;
            JoinButton.Enabled = false;
            JoinButton.Location = new Point(242, 45);
            JoinButton.Name = "JoinButton";
            JoinButton.Size = new Size(85, 23);
            JoinButton.TabIndex = 2;
            JoinButton.Text = "Join Session";
            JoinButton.UseVisualStyleBackColor = true;
            JoinButton.Click += JoinButton_Click;
            // 
            // FrmJoin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(349, 72);
            Controls.Add(JoinButton);
            Controls.Add(JoinLabel);
            Controls.Add(JoinTextBox);
            Name = "FrmJoin";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "FrmJoin";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox JoinTextBox;
        private Label JoinLabel;
        private Button JoinButton;
    }
}