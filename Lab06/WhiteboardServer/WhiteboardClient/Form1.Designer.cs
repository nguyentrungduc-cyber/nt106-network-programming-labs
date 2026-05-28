namespace WhiteboardClient
{
    partial class Form1
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
            this.pictureBoxWhiteboard = new System.Windows.Forms.PictureBox();
            this.panelControls = new System.Windows.Forms.Panel();
            this.buttonInsertImage = new System.Windows.Forms.Button();
            this.textBoxImageUrl = new System.Windows.Forms.TextBox();
            this.labelImageUrl = new System.Windows.Forms.Label();
            this.buttonEraser = new System.Windows.Forms.Button();
            this.buttonEnd = new System.Windows.Forms.Button();
            this.buttonClear = new System.Windows.Forms.Button();
            this.labelThickness = new System.Windows.Forms.Label();
            this.trackBarThickness = new System.Windows.Forms.TrackBar();
            this.panelColorPreview = new System.Windows.Forms.Panel();
            this.buttonColor = new System.Windows.Forms.Button();
            this.labelClientCount = new System.Windows.Forms.Label();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.textBoxServerIP = new System.Windows.Forms.TextBox();
            this.labelServerIP = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWhiteboard)).BeginInit();
            this.panelControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarThickness)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxWhiteboard
            // 
            this.pictureBoxWhiteboard.BackColor = System.Drawing.Color.White;
            this.pictureBoxWhiteboard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxWhiteboard.Location = new System.Drawing.Point(12, 12);
            this.pictureBoxWhiteboard.Name = "pictureBoxWhiteboard";
            this.pictureBoxWhiteboard.Size = new System.Drawing.Size(800, 600);
            this.pictureBoxWhiteboard.TabIndex = 0;
            this.pictureBoxWhiteboard.TabStop = false;
            this.pictureBoxWhiteboard.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxWhiteboard_MouseDown);
            this.pictureBoxWhiteboard.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxWhiteboard_MouseMove);
            this.pictureBoxWhiteboard.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxWhiteboard_MouseUp);
            // 
            // panelControls
            // 
            this.panelControls.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelControls.Controls.Add(this.buttonInsertImage);
            this.panelControls.Controls.Add(this.textBoxImageUrl);
            this.panelControls.Controls.Add(this.labelImageUrl);
            this.panelControls.Controls.Add(this.buttonEraser);
            this.panelControls.Controls.Add(this.buttonEnd);
            this.panelControls.Controls.Add(this.buttonClear);
            this.panelControls.Controls.Add(this.labelThickness);
            this.panelControls.Controls.Add(this.trackBarThickness);
            this.panelControls.Controls.Add(this.panelColorPreview);
            this.panelControls.Controls.Add(this.buttonColor);
            this.panelControls.Controls.Add(this.labelClientCount);
            this.panelControls.Controls.Add(this.buttonConnect);
            this.panelControls.Controls.Add(this.textBoxServerIP);
            this.panelControls.Controls.Add(this.labelServerIP);
            this.panelControls.Controls.Add(this.labelStatus);
            this.panelControls.Location = new System.Drawing.Point(818, 12);
            this.panelControls.Name = "panelControls";
            this.panelControls.Size = new System.Drawing.Size(250, 600);
            this.panelControls.TabIndex = 1;
            // 
            // buttonInsertImage
            // 
            this.buttonInsertImage.Location = new System.Drawing.Point(15, 430);
            this.buttonInsertImage.Name = "buttonInsertImage";
            this.buttonInsertImage.Size = new System.Drawing.Size(220, 30);
            this.buttonInsertImage.TabIndex = 14;
            this.buttonInsertImage.Text = "Insert Image";
            this.buttonInsertImage.UseVisualStyleBackColor = true;
            this.buttonInsertImage.Click += new System.EventHandler(this.buttonInsertImage_Click);
            // 
            // textBoxImageUrl
            // 
            this.textBoxImageUrl.Location = new System.Drawing.Point(15, 404);
            this.textBoxImageUrl.Name = "textBoxImageUrl";
            this.textBoxImageUrl.Size = new System.Drawing.Size(220, 20);
            this.textBoxImageUrl.TabIndex = 13;
            // 
            // labelImageUrl
            // 
            this.labelImageUrl.AutoSize = true;
            this.labelImageUrl.Location = new System.Drawing.Point(12, 388);
            this.labelImageUrl.Name = "labelImageUrl";
            this.labelImageUrl.Size = new System.Drawing.Size(64, 13);
            this.labelImageUrl.TabIndex = 12;
            this.labelImageUrl.Text = "Image URL:";
            // 
            // buttonEraser
            // 
            this.buttonEraser.Location = new System.Drawing.Point(15, 340);
            this.buttonEraser.Name = "buttonEraser";
            this.buttonEraser.Size = new System.Drawing.Size(220, 35);
            this.buttonEraser.TabIndex = 11;
            this.buttonEraser.Text = "Eraser";
            this.buttonEraser.UseVisualStyleBackColor = true;
            this.buttonEraser.Click += new System.EventHandler(this.buttonEraser_Click);
            // 
            // buttonEnd
            // 
            this.buttonEnd.BackColor = System.Drawing.Color.IndianRed;
            this.buttonEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEnd.ForeColor = System.Drawing.Color.White;
            this.buttonEnd.Location = new System.Drawing.Point(15, 540);
            this.buttonEnd.Name = "buttonEnd";
            this.buttonEnd.Size = new System.Drawing.Size(220, 45);
            this.buttonEnd.TabIndex = 10;
            this.buttonEnd.Text = "End Session";
            this.buttonEnd.UseVisualStyleBackColor = false;
            this.buttonEnd.Click += new System.EventHandler(this.buttonEnd_Click);
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(15, 490);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(220, 35);
            this.buttonClear.TabIndex = 9;
            this.buttonClear.Text = "Clear Whiteboard";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // labelThickness
            // 
            this.labelThickness.AutoSize = true;
            this.labelThickness.Location = new System.Drawing.Point(12, 250);
            this.labelThickness.Name = "labelThickness";
            this.labelThickness.Size = new System.Drawing.Size(69, 13);
            this.labelThickness.TabIndex = 8;
            this.labelThickness.Text = "Thickness: 2";
            // 
            // trackBarThickness
            // 
            this.trackBarThickness.Location = new System.Drawing.Point(15, 266);
            this.trackBarThickness.Maximum = 20;
            this.trackBarThickness.Minimum = 1;
            this.trackBarThickness.Name = "trackBarThickness";
            this.trackBarThickness.Size = new System.Drawing.Size(220, 45);
            this.trackBarThickness.TabIndex = 7;
            this.trackBarThickness.Value = 2;
            this.trackBarThickness.Scroll += new System.EventHandler(this.trackBarThickness_Scroll);
            // 
            // panelColorPreview
            // 
            this.panelColorPreview.BackColor = System.Drawing.Color.Black;
            this.panelColorPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelColorPreview.Location = new System.Drawing.Point(15, 210);
            this.panelColorPreview.Name = "panelColorPreview";
            this.panelColorPreview.Size = new System.Drawing.Size(220, 30);
            this.panelColorPreview.TabIndex = 6;
            // 
            // buttonColor
            // 
            this.buttonColor.Location = new System.Drawing.Point(15, 170);
            this.buttonColor.Name = "buttonColor";
            this.buttonColor.Size = new System.Drawing.Size(220, 35);
            this.buttonColor.TabIndex = 5;
            this.buttonColor.Text = "Choose Color";
            this.buttonColor.UseVisualStyleBackColor = true;
            this.buttonColor.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // labelClientCount
            // 
            this.labelClientCount.AutoSize = true;
            this.labelClientCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelClientCount.Location = new System.Drawing.Point(12, 140);
            this.labelClientCount.Name = "labelClientCount";
            this.labelClientCount.Size = new System.Drawing.Size(142, 16);
            this.labelClientCount.TabIndex = 4;
            this.labelClientCount.Text = "Connected Clients: 0";
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(15, 90);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(220, 35);
            this.buttonConnect.TabIndex = 3;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // textBoxServerIP
            // 
            this.textBoxServerIP.Location = new System.Drawing.Point(15, 64);
            this.textBoxServerIP.Name = "textBoxServerIP";
            this.textBoxServerIP.Size = new System.Drawing.Size(220, 20);
            this.textBoxServerIP.TabIndex = 2;
            this.textBoxServerIP.Text = "127.0.0.1";
            // 
            // labelServerIP
            // 
            this.labelServerIP.AutoSize = true;
            this.labelServerIP.Location = new System.Drawing.Point(12, 48);
            this.labelServerIP.Name = "labelServerIP";
            this.labelServerIP.Size = new System.Drawing.Size(55, 13);
            this.labelServerIP.TabIndex = 1;
            this.labelServerIP.Text = "Server IP:";
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStatus.Location = new System.Drawing.Point(12, 15);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(101, 16);
            this.labelStatus.TabIndex = 0;
            this.labelStatus.Text = "Not Connected";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1080, 624);
            this.Controls.Add(this.panelControls);
            this.Controls.Add(this.pictureBoxWhiteboard);
            this.Name = "Form1";
            this.Text = "Whiteboard Client";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWhiteboard)).EndInit();
            this.panelControls.ResumeLayout(false);
            this.panelControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarThickness)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxWhiteboard;
        private System.Windows.Forms.Panel panelControls;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Label labelServerIP;
        private System.Windows.Forms.TextBox textBoxServerIP;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Label labelClientCount;
        private System.Windows.Forms.Button buttonColor;
        private System.Windows.Forms.Panel panelColorPreview;
        private System.Windows.Forms.TrackBar trackBarThickness;
        private System.Windows.Forms.Label labelThickness;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.Button buttonEnd;
        private System.Windows.Forms.Button buttonEraser;
        private System.Windows.Forms.Label labelImageUrl;
        private System.Windows.Forms.TextBox textBoxImageUrl;
        private System.Windows.Forms.Button buttonInsertImage;
    }
}

