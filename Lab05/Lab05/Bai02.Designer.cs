namespace Lab05
{
    partial class Bai02
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
            components = new System.ComponentModel.Container();
            label1 = new Label();
            label2 = new Label();
            contextMenuStrip1 = new ContextMenuStrip(components);
            txtEmail = new TextBox();
            txtPassword = new TextBox();
            btnLogin = new Button();
            label3 = new Label();
            label4 = new Label();
            lblTotalCount = new Label();
            lblRecentCount = new Label();
            lsvMails = new ListView();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(33, 37);
            label1.Name = "label1";
            label1.Size = new Size(49, 20);
            label1.TabIndex = 0;
            label1.Text = "Email:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(33, 72);
            label2.Name = "label2";
            label2.Size = new Size(73, 20);
            label2.TabIndex = 1;
            label2.Text = "Password:";
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new Size(20, 20);
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(61, 4);
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(112, 30);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(412, 27);
            txtEmail.TabIndex = 3;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(112, 69);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(412, 27);
            txtPassword.TabIndex = 4;
            // 
            // btnLogin
            // 
            btnLogin.Location = new Point(658, 48);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(94, 29);
            btnLogin.TabIndex = 5;
            btnLogin.Text = "Login";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(56, 156);
            label3.Name = "label3";
            label3.Size = new Size(45, 20);
            label3.TabIndex = 6;
            label3.Text = "Total:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(211, 156);
            label4.Name = "label4";
            label4.Size = new Size(57, 20);
            label4.TabIndex = 7;
            label4.Text = "Recent:";
            // 
            // lblTotalCount
            // 
            lblTotalCount.AutoSize = true;
            lblTotalCount.Location = new Point(107, 156);
            lblTotalCount.Name = "lblTotalCount";
            lblTotalCount.Size = new Size(17, 20);
            lblTotalCount.TabIndex = 8;
            lblTotalCount.Text = "0";
            // 
            // lblRecentCount
            // 
            lblRecentCount.AutoSize = true;
            lblRecentCount.Location = new Point(274, 156);
            lblRecentCount.Name = "lblRecentCount";
            lblRecentCount.Size = new Size(17, 20);
            lblRecentCount.TabIndex = 9;
            lblRecentCount.Text = "0";
            // 
            // lsvMails
            // 
            lsvMails.FullRowSelect = true;
            lsvMails.GridLines = true;
            lsvMails.Location = new Point(33, 195);
            lsvMails.Name = "lsvMails";
            lsvMails.Size = new Size(719, 231);
            lsvMails.TabIndex = 10;
            lsvMails.UseCompatibleStateImageBehavior = false;
            lsvMails.View = View.Details;
            // 
            // Bai02
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lsvMails);
            Controls.Add(lblRecentCount);
            Controls.Add(lblTotalCount);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(btnLogin);
            Controls.Add(txtPassword);
            Controls.Add(txtEmail);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "Bai02";
            Text = "Bai02";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private ContextMenuStrip contextMenuStrip1;
        private TextBox txtEmail;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label label3;
        private Label label4;
        private Label lblTotalCount;
        private Label lblRecentCount;
        private ListView lsvMails;
    }
}