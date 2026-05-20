namespace Lab5
{
    partial class Bai03
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
            label7 = new Label();
            label6 = new Label();
            label5 = new Label();
            tbBody = new TextBox();
            tbSubject = new TextBox();
            btnSend = new Button();
            tbTo = new TextBox();
            label4 = new Label();
            tbPassword = new TextBox();
            label3 = new Label();
            tbFrom = new TextBox();
            label1 = new Label();
            label2 = new Label();
            rtbFile = new RichTextBox();
            btnIMG = new Button();
            SuspendLayout();
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.ForeColor = SystemColors.ControlDarkDark;
            label7.Location = new Point(122, 179);
            label7.Name = "label7";
            label7.Size = new Size(981, 40);
            label7.TabIndex = 31;
            label7.Text = "------------------------------------------------------------------------------------------------------------------------------------------------------------------\r\n\r\n";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label6.Location = new Point(72, 261);
            label6.Name = "label6";
            label6.Size = new Size(46, 18);
            label6.TabIndex = 30;
            label6.Text = "Body:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.Location = new Point(55, 201);
            label5.Name = "label5";
            label5.Size = new Size(61, 18);
            label5.TabIndex = 29;
            label5.Text = "Subject:";
            // 
            // tbBody
            // 
            tbBody.Location = new Point(127, 261);
            tbBody.Margin = new Padding(3, 2, 3, 2);
            tbBody.Multiline = true;
            tbBody.Name = "tbBody";
            tbBody.Size = new Size(559, 336);
            tbBody.TabIndex = 28;
            // 
            // tbSubject
            // 
            tbSubject.Location = new Point(127, 200);
            tbSubject.Margin = new Padding(3, 2, 3, 2);
            tbSubject.Multiline = true;
            tbSubject.Name = "tbSubject";
            tbSubject.Size = new Size(721, 39);
            tbSubject.TabIndex = 27;
            // 
            // btnSend
            // 
            btnSend.BackColor = Color.CornflowerBlue;
            btnSend.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSend.ForeColor = SystemColors.ButtonHighlight;
            btnSend.ImageAlign = ContentAlignment.TopLeft;
            btnSend.Location = new Point(756, 112);
            btnSend.Margin = new Padding(3, 2, 3, 2);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(96, 75);
            btnSend.TabIndex = 26;
            btnSend.Text = "SEND";
            btnSend.UseVisualStyleBackColor = false;
            btnSend.Click += btnSend_Click;
            // 
            // tbTo
            // 
            tbTo.Location = new Point(547, 75);
            tbTo.Margin = new Padding(3, 2, 3, 2);
            tbTo.Name = "tbTo";
            tbTo.Size = new Size(303, 27);
            tbTo.TabIndex = 25;
            tbTo.Text = "nguyentrungduc190906@gmail.com";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.Location = new Point(509, 79);
            label4.Name = "label4";
            label4.Size = new Size(30, 18);
            label4.TabIndex = 24;
            label4.Text = "To:";
            // 
            // tbPassword
            // 
            tbPassword.Location = new Point(127, 119);
            tbPassword.Margin = new Padding(3, 2, 3, 2);
            tbPassword.Name = "tbPassword";
            tbPassword.PasswordChar = '*';
            tbPassword.Size = new Size(303, 27);
            tbPassword.TabIndex = 23;
            tbPassword.Text = "zwna npqj iyqs uizd";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(37, 122);
            label3.Name = "label3";
            label3.Size = new Size(79, 18);
            label3.TabIndex = 22;
            label3.Text = "Password:";
            // 
            // tbFrom
            // 
            tbFrom.Location = new Point(127, 75);
            tbFrom.Margin = new Padding(3, 2, 3, 2);
            tbFrom.Name = "tbFrom";
            tbFrom.Size = new Size(303, 27);
            tbFrom.TabIndex = 21;
            tbFrom.Text = "24520324@gm.uit.edu.vn";
            tbFrom.TextChanged += tbFrom_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(72, 79);
            label1.Name = "label1";
            label1.Size = new Size(48, 18);
            label1.TabIndex = 20;
            label1.Text = "From:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(347, 15);
            label2.Name = "label2";
            label2.Size = new Size(246, 31);
            label2.TabIndex = 19;
            label2.Text = "SEND VIA GMAIL";
            // 
            // rtbFile
            // 
            rtbFile.BorderStyle = BorderStyle.None;
            rtbFile.Location = new Point(692, 261);
            rtbFile.Margin = new Padding(3, 5, 3, 5);
            rtbFile.Name = "rtbFile";
            rtbFile.Size = new Size(160, 258);
            rtbFile.TabIndex = 33;
            rtbFile.Text = "";
            // 
            // btnIMG
            // 
            btnIMG.Location = new Point(692, 525);
            btnIMG.Margin = new Padding(3, 5, 3, 5);
            btnIMG.Name = "btnIMG";
            btnIMG.Size = new Size(159, 72);
            btnIMG.TabIndex = 32;
            btnIMG.Text = "images/files";
            btnIMG.UseVisualStyleBackColor = true;
            btnIMG.Click += btnIMG_Click;
            // 
            // Bai03
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(927, 655);
            Controls.Add(rtbFile);
            Controls.Add(btnIMG);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(tbBody);
            Controls.Add(tbSubject);
            Controls.Add(btnSend);
            Controls.Add(tbTo);
            Controls.Add(label4);
            Controls.Add(tbPassword);
            Controls.Add(label3);
            Controls.Add(tbFrom);
            Controls.Add(label1);
            Controls.Add(label2);
            Controls.Add(label7);
            Margin = new Padding(3, 2, 3, 2);
            Name = "Bai03";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Bài 03";
            Load += Bai03_Load;
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbBody;
        private System.Windows.Forms.TextBox tbSubject;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox tbTo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbFrom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox rtbFile;
        private System.Windows.Forms.Button btnIMG;
    }
}