namespace Lab05
{
    partial class Bai01
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
            button1 = new Button();
            label1 = new Label();
            label2 = new Label();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            textBox3 = new TextBox();
            label3 = new Label();
            richTextBox1 = new RichTextBox();
            label4 = new Label();
            textBox4 = new TextBox();
            label5 = new Label();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(35, 32);
            button1.Name = "button1";
            button1.Size = new Size(71, 46);
            button1.TabIndex = 0;
            button1.Text = "GỬI";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(124, 25);
            label1.Name = "label1";
            label1.Size = new Size(81, 20);
            label1.TabIndex = 1;
            label1.Text = "Địa chỉ gửi";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(124, 58);
            label2.Name = "label2";
            label2.Size = new Size(91, 20);
            label2.TabIndex = 2;
            label2.Text = "Địa chỉ nhận";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(229, 22);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(283, 27);
            textBox1.TabIndex = 3;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(229, 55);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(283, 27);
            textBox2.TabIndex = 4;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(621, 22);
            textBox3.Name = "textBox3";
            textBox3.PasswordChar = '*';
            textBox3.Size = new Size(148, 27);
            textBox3.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(545, 25);
            label3.Name = "label3";
            label3.Size = new Size(70, 20);
            label3.TabIndex = 6;
            label3.Text = "Mật khẩu";
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(124, 145);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(645, 278);
            richTextBox1.TabIndex = 7;
            richTextBox1.Text = "";
            richTextBox1.TextChanged += richTextBox1_TextChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(35, 148);
            label4.Name = "label4";
            label4.Size = new Size(71, 20);
            label4.TabIndex = 8;
            label4.Text = "Nội dung";
            // 
            // textBox4
            // 
            textBox4.Location = new Point(124, 112);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(645, 27);
            textBox4.TabIndex = 9;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(36, 115);
            label5.Name = "label5";
            label5.Size = new Size(58, 20);
            label5.TabIndex = 10;
            label5.Text = "Tiêu đề";
            // 
            // Bai01
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label5);
            Controls.Add(textBox4);
            Controls.Add(label4);
            Controls.Add(richTextBox1);
            Controls.Add(label3);
            Controls.Add(textBox3);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(button1);
            Name = "Bai01";
            Text = "Bai01";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Label label1;
        private Label label2;
        private TextBox textBox1;
        private TextBox textBox2;
        private TextBox textBox3;
        private Label label3;
        private RichTextBox richTextBox1;
        private Label label4;
        private TextBox textBox4;
        private Label label5;
    }
}