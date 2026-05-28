namespace WhiteboardClient
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pictureBoxWhiteboard = new System.Windows.Forms.PictureBox();
            this.panelToolbar = new System.Windows.Forms.Panel();
            this.panelEndSession = new System.Windows.Forms.Panel();
            this.btnEnd = new System.Windows.Forms.Button();
            this.panelSeparator5 = new System.Windows.Forms.Panel();
            this.panelImageSection = new System.Windows.Forms.Panel();
            this.btnInsertImage = new System.Windows.Forms.Button();
            this.txtImageUrl = new System.Windows.Forms.TextBox();
            this.panelSeparator4 = new System.Windows.Forms.Panel();
            this.panelActions = new System.Windows.Forms.Panel();
            this.btnEraser = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.panelSeparator3 = new System.Windows.Forms.Panel();
            this.panelColors = new System.Windows.Forms.Panel();
            this.flowPresetColors = new System.Windows.Forms.FlowLayoutPanel();
            this.labelColors = new System.Windows.Forms.Label();
            this.panelColorPreview = new System.Windows.Forms.Panel();
            this.btnColor = new System.Windows.Forms.Button();
            this.panelSeparator2 = new System.Windows.Forms.Panel();
            this.panelThickness = new System.Windows.Forms.Panel();
            this.labelThicknessTitle = new System.Windows.Forms.Label();
            this.trackBarThickness = new System.Windows.Forms.TrackBar();
            this.labelThicknessVal = new System.Windows.Forms.Label();
            this.panelSeparator1 = new System.Windows.Forms.Panel();
            this.panelConnection = new System.Windows.Forms.Panel();
            this.panelClientCount = new System.Windows.Forms.Panel();
            this.labelClientCountTitle = new System.Windows.Forms.Label();
            this.labelClientCount = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtServerIP = new System.Windows.Forms.TextBox();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.labelTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWhiteboard)).BeginInit();
            this.panelToolbar.SuspendLayout();
            this.panelEndSession.SuspendLayout();
            this.panelImageSection.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.panelColors.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarThickness)).BeginInit();
            this.panelThickness.SuspendLayout();
            this.panelConnection.SuspendLayout();
            this.panelClientCount.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBoxWhiteboard
            // 
            this.pictureBoxWhiteboard.BackColor = System.Drawing.Color.White;
            this.pictureBoxWhiteboard.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.pictureBoxWhiteboard.Location = new System.Drawing.Point(0, 50);
            this.pictureBoxWhiteboard.Name = "pictureBoxWhiteboard";
            this.pictureBoxWhiteboard.Size = new System.Drawing.Size(930, 640);
            this.pictureBoxWhiteboard.TabIndex = 0;
            this.pictureBoxWhiteboard.TabStop = false;
            this.pictureBoxWhiteboard.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxWhiteboard_MouseDown);
            this.pictureBoxWhiteboard.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxWhiteboard_MouseMove);
            this.pictureBoxWhiteboard.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxWhiteboard_MouseUp);
            // 
            // panelToolbar
            // 
            this.panelToolbar.BackColor = System.Drawing.Color.FromArgb(44, 62, 80);
            this.panelToolbar.Controls.Add(this.panelEndSession);
            this.panelToolbar.Controls.Add(this.panelSeparator5);
            this.panelToolbar.Controls.Add(this.panelImageSection);
            this.panelToolbar.Controls.Add(this.panelSeparator4);
            this.panelToolbar.Controls.Add(this.panelActions);
            this.panelToolbar.Controls.Add(this.panelSeparator3);
            this.panelToolbar.Controls.Add(this.panelColors);
            this.panelToolbar.Controls.Add(this.panelSeparator2);
            this.panelToolbar.Controls.Add(this.panelThickness);
            this.panelToolbar.Controls.Add(this.panelSeparator1);
            this.panelToolbar.Controls.Add(this.panelConnection);
            this.panelToolbar.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelToolbar.Location = new System.Drawing.Point(928, 50);
            this.panelToolbar.Name = "panelToolbar";
            this.panelToolbar.Size = new System.Drawing.Size(250, 642);
            this.panelToolbar.TabIndex = 1;
            // 
            // panelEndSession
            // 
            this.panelEndSession.Controls.Add(this.btnEnd);
            this.panelEndSession.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelEndSession.Location = new System.Drawing.Point(0, 582);
            this.panelEndSession.Name = "panelEndSession";
            this.panelEndSession.Padding = new System.Windows.Forms.Padding(10);
            this.panelEndSession.Size = new System.Drawing.Size(250, 60);
            this.panelEndSession.TabIndex = 10;
            // 
            // btnEnd
            // 
            this.btnEnd.BackColor = System.Drawing.Color.FromArgb(192, 57, 43);
            this.btnEnd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEnd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnEnd.FlatAppearance.BorderSize = 0;
            this.btnEnd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEnd.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnEnd.ForeColor = System.Drawing.Color.White;
            this.btnEnd.Location = new System.Drawing.Point(10, 10);
            this.btnEnd.Name = "btnEnd";
            this.btnEnd.Size = new System.Drawing.Size(230, 40);
            this.btnEnd.TabIndex = 0;
            this.btnEnd.Text = "✕ END SESSION";
            this.btnEnd.UseVisualStyleBackColor = false;
            this.btnEnd.Click += new System.EventHandler(this.btnEnd_Click);
            // 
            // panelSeparator5
            // 
            this.panelSeparator5.BackColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.panelSeparator5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelSeparator5.Location = new System.Drawing.Point(0, 580);
            this.panelSeparator5.Name = "panelSeparator5";
            this.panelSeparator5.Size = new System.Drawing.Size(250, 2);
            this.panelSeparator5.TabIndex = 9;
            // 
            // panelImageSection
            // 
            this.panelImageSection.Controls.Add(this.btnInsertImage);
            this.panelImageSection.Controls.Add(this.txtImageUrl);
            this.panelImageSection.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelImageSection.Location = new System.Drawing.Point(0, 488);
            this.panelImageSection.Name = "panelImageSection";
            this.panelImageSection.Padding = new System.Windows.Forms.Padding(10, 5, 10, 10);
            this.panelImageSection.Size = new System.Drawing.Size(250, 92);
            this.panelImageSection.TabIndex = 8;
            // 
            // btnInsertImage
            // 
            this.btnInsertImage.BackColor = System.Drawing.Color.FromArgb(39, 174, 96);
            this.btnInsertImage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnInsertImage.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnInsertImage.FlatAppearance.BorderSize = 0;
            this.btnInsertImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInsertImage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnInsertImage.ForeColor = System.Drawing.Color.White;
            this.btnInsertImage.Location = new System.Drawing.Point(10, 49);
            this.btnInsertImage.Name = "btnInsertImage";
            this.btnInsertImage.Size = new System.Drawing.Size(230, 30);
            this.btnInsertImage.TabIndex = 1;
            this.btnInsertImage.Text = "🖼 Insert Image";
            this.btnInsertImage.UseVisualStyleBackColor = false;
            this.btnInsertImage.Click += new System.EventHandler(this.btnInsertImage_Click);
            // 
            // txtImageUrl
            // 
            this.txtImageUrl.BackColor = System.Drawing.Color.FromArgb(236, 240, 241);
            this.txtImageUrl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtImageUrl.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtImageUrl.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtImageUrl.Location = new System.Drawing.Point(10, 29);
            this.txtImageUrl.Name = "txtImageUrl";
            this.txtImageUrl.Size = new System.Drawing.Size(230, 16);
            this.txtImageUrl.TabIndex = 0;
            this.txtImageUrl.Text = "Paste image URL here...";
            this.txtImageUrl.Enter += new System.EventHandler(this.txtImageUrl_Enter);
            this.txtImageUrl.Leave += new System.EventHandler(this.txtImageUrl_Leave);
            // 
            // panelSeparator4
            // 
            this.panelSeparator4.BackColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.panelSeparator4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSeparator4.Location = new System.Drawing.Point(0, 486);
            this.panelSeparator4.Name = "panelSeparator4";
            this.panelSeparator4.Size = new System.Drawing.Size(250, 2);
            this.panelSeparator4.TabIndex = 7;
            // 
            // panelActions
            // 
            this.panelActions.Controls.Add(this.btnEraser);
            this.panelActions.Controls.Add(this.btnClear);
            this.panelActions.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelActions.Location = new System.Drawing.Point(0, 410);
            this.panelActions.Name = "panelActions";
            this.panelActions.Padding = new System.Windows.Forms.Padding(10, 8, 10, 8);
            this.panelActions.Size = new System.Drawing.Size(250, 76);
            this.panelActions.TabIndex = 6;
            // 
            // btnEraser
            // 
            this.btnEraser.BackColor = System.Drawing.Color.FromArgb(236, 240, 241);
            this.btnEraser.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEraser.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnEraser.FlatAppearance.BorderSize = 0;
            this.btnEraser.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEraser.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnEraser.Location = new System.Drawing.Point(10, 8);
            this.btnEraser.Name = "btnEraser";
            this.btnEraser.Size = new System.Drawing.Size(110, 60);
            this.btnEraser.TabIndex = 0;
            this.btnEraser.Text = "🧹 Eraser";
            this.btnEraser.UseVisualStyleBackColor = false;
            this.btnEraser.Click += new System.EventHandler(this.btnEraser_Click);
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.Color.FromArgb(243, 156, 18);
            this.btnClear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClear.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnClear.FlatAppearance.BorderSize = 0;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnClear.ForeColor = System.Drawing.Color.White;
            this.btnClear.Location = new System.Drawing.Point(130, 8);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(110, 60);
            this.btnClear.TabIndex = 1;
            this.btnClear.Text = "🗑 Clear";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // panelSeparator3
            // 
            this.panelSeparator3.BackColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.panelSeparator3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSeparator3.Location = new System.Drawing.Point(0, 408);
            this.panelSeparator3.Name = "panelSeparator3";
            this.panelSeparator3.Size = new System.Drawing.Size(250, 2);
            this.panelSeparator3.TabIndex = 5;
            // 
            // panelColors
            // 
            this.panelColors.Controls.Add(this.flowPresetColors);
            this.panelColors.Controls.Add(this.labelColors);
            this.panelColors.Controls.Add(this.panelColorPreview);
            this.panelColors.Controls.Add(this.btnColor);
            this.panelColors.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelColors.Location = new System.Drawing.Point(0, 212);
            this.panelColors.Name = "panelColors";
            this.panelColors.Padding = new System.Windows.Forms.Padding(10, 5, 10, 10);
            this.panelColors.Size = new System.Drawing.Size(250, 196);
            this.panelColors.TabIndex = 4;
            // 
            // flowPresetColors
            // 
            this.flowPresetColors.BackColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.flowPresetColors.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowPresetColors.Location = new System.Drawing.Point(10, 101);
            this.flowPresetColors.Name = "flowPresetColors";
            this.flowPresetColors.Padding = new System.Windows.Forms.Padding(5);
            this.flowPresetColors.Size = new System.Drawing.Size(230, 84);
            this.flowPresetColors.TabIndex = 3;
            // 
            // labelColors
            // 
            this.labelColors.AutoSize = true;
            this.labelColors.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelColors.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelColors.ForeColor = System.Drawing.Color.FromArgb(189, 195, 199);
            this.labelColors.Location = new System.Drawing.Point(10, 85);
            this.labelColors.Name = "labelColors";
            this.labelColors.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.labelColors.Size = new System.Drawing.Size(114, 19);
            this.labelColors.TabIndex = 2;
            this.labelColors.Text = "🎨 PRESET COLORS";
            // 
            // panelColorPreview
            // 
            this.panelColorPreview.BackColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.panelColorPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelColorPreview.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelColorPreview.Location = new System.Drawing.Point(10, 42);
            this.panelColorPreview.Name = "panelColorPreview";
            this.panelColorPreview.Size = new System.Drawing.Size(230, 43);
            this.panelColorPreview.TabIndex = 1;
            // 
            // btnColor
            // 
            this.btnColor.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            this.btnColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnColor.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnColor.FlatAppearance.BorderSize = 0;
            this.btnColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnColor.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnColor.ForeColor = System.Drawing.Color.White;
            this.btnColor.Location = new System.Drawing.Point(10, 5);
            this.btnColor.Name = "btnColor";
            this.btnColor.Size = new System.Drawing.Size(230, 37);
            this.btnColor.TabIndex = 0;
            this.btnColor.Text = "🌈 Custom Color";
            this.btnColor.UseVisualStyleBackColor = false;
            this.btnColor.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // panelSeparator2
            // 
            this.panelSeparator2.BackColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.panelSeparator2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSeparator2.Location = new System.Drawing.Point(0, 210);
            this.panelSeparator2.Name = "panelSeparator2";
            this.panelSeparator2.Size = new System.Drawing.Size(250, 2);
            this.panelSeparator2.TabIndex = 3;
            // 
            // panelThickness
            // 
            this.panelThickness.Controls.Add(this.labelThicknessTitle);
            this.panelThickness.Controls.Add(this.trackBarThickness);
            this.panelThickness.Controls.Add(this.labelThicknessVal);
            this.panelThickness.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelThickness.Location = new System.Drawing.Point(0, 130);
            this.panelThickness.Name = "panelThickness";
            this.panelThickness.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.panelThickness.Size = new System.Drawing.Size(250, 80);
            this.panelThickness.TabIndex = 2;
            // 
            // labelThicknessTitle
            // 
            this.labelThicknessTitle.AutoSize = true;
            this.labelThicknessTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelThicknessTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelThicknessTitle.ForeColor = System.Drawing.Color.FromArgb(189, 195, 199);
            this.labelThicknessTitle.Location = new System.Drawing.Point(10, 5);
            this.labelThicknessTitle.Name = "labelThicknessTitle";
            this.labelThicknessTitle.Size = new System.Drawing.Size(73, 15);
            this.labelThicknessTitle.TabIndex = 0;
            this.labelThicknessTitle.Text = "✏️ THICKNESS";
            // 
            // trackBarThickness
            // 
            this.trackBarThickness.BackColor = System.Drawing.Color.FromArgb(44, 62, 80);
            this.trackBarThickness.Dock = System.Windows.Forms.DockStyle.Top;
            this.trackBarThickness.LargeChange = 2;
            this.trackBarThickness.Location = new System.Drawing.Point(10, 24);
            this.trackBarThickness.Maximum = 20;
            this.trackBarThickness.Minimum = 1;
            this.trackBarThickness.Name = "trackBarThickness";
            this.trackBarThickness.Size = new System.Drawing.Size(230, 45);
            this.trackBarThickness.TabIndex = 1;
            this.trackBarThickness.Value = 3;
            this.trackBarThickness.Scroll += new System.EventHandler(this.trackBarThickness_Scroll);
            // 
            // labelThicknessVal
            // 
            this.labelThicknessVal.AutoSize = true;
            this.labelThicknessVal.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelThicknessVal.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.labelThicknessVal.ForeColor = System.Drawing.Color.FromArgb(46, 204, 113);
            this.labelThicknessVal.Location = new System.Drawing.Point(10, 69);
            this.labelThicknessVal.Name = "labelThicknessVal";
            this.labelThicknessVal.Size = new System.Drawing.Size(22, 25);
            this.labelThicknessVal.TabIndex = 2;
            this.labelThicknessVal.Text = "3";
            // 
            // panelSeparator1
            // 
            this.panelSeparator1.BackColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.panelSeparator1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSeparator1.Location = new System.Drawing.Point(0, 128);
            this.panelSeparator1.Name = "panelSeparator1";
            this.panelSeparator1.Size = new System.Drawing.Size(250, 2);
            this.panelSeparator1.TabIndex = 1;
            // 
            // panelConnection
            // 
            this.panelConnection.Controls.Add(this.panelClientCount);
            this.panelConnection.Controls.Add(this.labelStatus);
            this.panelConnection.Controls.Add(this.btnConnect);
            this.panelConnection.Controls.Add(this.txtServerIP);
            this.panelConnection.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelConnection.Location = new System.Drawing.Point(0, 0);
            this.panelConnection.Name = "panelConnection";
            this.panelConnection.Padding = new System.Windows.Forms.Padding(10);
            this.panelConnection.Size = new System.Drawing.Size(250, 128);
            this.panelConnection.TabIndex = 0;
            // 
            // panelClientCount
            // 
            this.panelClientCount.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            this.panelClientCount.Controls.Add(this.labelClientCountTitle);
            this.panelClientCount.Controls.Add(this.labelClientCount);
            this.panelClientCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelClientCount.Location = new System.Drawing.Point(10, 73);
            this.panelClientCount.Name = "panelClientCount";
            this.panelClientCount.Padding = new System.Windows.Forms.Padding(10);
            this.panelClientCount.Size = new System.Drawing.Size(230, 45);
            this.panelClientCount.TabIndex = 4;
            // 
            // labelClientCountTitle
            // 
            this.labelClientCountTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelClientCountTitle.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.labelClientCountTitle.ForeColor = System.Drawing.Color.White;
            this.labelClientCountTitle.Location = new System.Drawing.Point(10, 10);
            this.labelClientCountTitle.Name = "labelClientCountTitle";
            this.labelClientCountTitle.Size = new System.Drawing.Size(210, 13);
            this.labelClientCountTitle.TabIndex = 0;
            this.labelClientCountTitle.Text = "👥 Online";
            this.labelClientCountTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelClientCount
            // 
            this.labelClientCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelClientCount.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.labelClientCount.ForeColor = System.Drawing.Color.White;
            this.labelClientCount.Location = new System.Drawing.Point(10, 23);
            this.labelClientCount.Name = "labelClientCount";
            this.labelClientCount.Size = new System.Drawing.Size(210, 12);
            this.labelClientCount.TabIndex = 1;
            this.labelClientCount.Text = "0";
            this.labelClientCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelStatus
            // 
            this.labelStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelStatus.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(149, 165, 166);
            this.labelStatus.Location = new System.Drawing.Point(10, 10);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(230, 15);
            this.labelStatus.TabIndex = 0;
            this.labelStatus.Text = "Not connected";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnConnect
            // 
            this.btnConnect.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            this.btnConnect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConnect.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnConnect.FlatAppearance.BorderSize = 0;
            this.btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConnect.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnConnect.ForeColor = System.Drawing.Color.White;
            this.btnConnect.Location = new System.Drawing.Point(10, 42);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(230, 31);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "🔌 Connect";
            this.btnConnect.UseVisualStyleBackColor = false;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtServerIP
            // 
            this.txtServerIP.BackColor = System.Drawing.Color.FromArgb(236, 240, 241);
            this.txtServerIP.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtServerIP.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtServerIP.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtServerIP.Location = new System.Drawing.Point(10, 25);
            this.txtServerIP.Name = "txtServerIP";
            this.txtServerIP.Size = new System.Drawing.Size(230, 16);
            this.txtServerIP.TabIndex = 3;
            this.txtServerIP.Text = "Enter server IP...";
            this.txtServerIP.Enter += new System.EventHandler(this.txtServerIP_Enter);
            this.txtServerIP.Leave += new System.EventHandler(this.txtServerIP_Leave);
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(44, 62, 80);
            this.panelHeader.Controls.Add(this.labelTitle);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1178, 50);
            this.panelHeader.TabIndex = 2;
            // 
            // labelTitle
            // 
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.labelTitle.ForeColor = System.Drawing.Color.FromArgb(236, 240, 241);
            this.labelTitle.Location = new System.Drawing.Point(0, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(1178, 50);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "📋 Whiteboard Client";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelTitle.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(236, 240, 241);
            this.ClientSize = new System.Drawing.Size(1178, 692);
            this.Controls.Add(this.pictureBoxWhiteboard);
            this.Controls.Add(this.panelToolbar);
            this.Controls.Add(this.panelHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Whiteboard Client";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWhiteboard)).EndInit();
            this.panelToolbar.ResumeLayout(false);
            this.panelEndSession.ResumeLayout(false);
            this.panelImageSection.ResumeLayout(false);
            this.panelImageSection.PerformLayout();
            this.panelActions.ResumeLayout(false);
            this.panelColors.ResumeLayout(false);
            this.panelColors.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarThickness)).EndInit();
            this.panelThickness.ResumeLayout(false);
            this.panelThickness.PerformLayout();
            this.panelConnection.ResumeLayout(false);
            this.panelConnection.PerformLayout();
            this.panelClientCount.ResumeLayout(false);
            this.panelHeader.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.PictureBox pictureBoxWhiteboard;
        private System.Windows.Forms.Panel panelToolbar;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Panel panelConnection;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtServerIP;
        private System.Windows.Forms.Panel panelClientCount;
        private System.Windows.Forms.Label labelClientCountTitle;
        private System.Windows.Forms.Label labelClientCount;
        private System.Windows.Forms.Panel panelSeparator1;
        private System.Windows.Forms.Panel panelThickness;
        private System.Windows.Forms.Label labelThicknessTitle;
        private System.Windows.Forms.TrackBar trackBarThickness;
        private System.Windows.Forms.Label labelThicknessVal;
        private System.Windows.Forms.Panel panelSeparator2;
        private System.Windows.Forms.Panel panelColors;
        private System.Windows.Forms.Button btnColor;
        private System.Windows.Forms.Panel panelColorPreview;
        private System.Windows.Forms.Label labelColors;
        private System.Windows.Forms.FlowLayoutPanel flowPresetColors;
        private System.Windows.Forms.Panel panelSeparator3;
        private System.Windows.Forms.Panel panelActions;
        private System.Windows.Forms.Button btnEraser;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Panel panelSeparator4;
        private System.Windows.Forms.Panel panelImageSection;
        private System.Windows.Forms.TextBox txtImageUrl;
        private System.Windows.Forms.Button btnInsertImage;
        private System.Windows.Forms.Panel panelSeparator5;
        private System.Windows.Forms.Panel panelEndSession;
        private System.Windows.Forms.Button btnEnd;
    }
}
