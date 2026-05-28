using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace WhiteboardClient
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        private NetworkStream stream;
        private Bitmap whiteboard;
        private Graphics whiteboardGraphics;
        private Point lastPoint;
        private bool isDrawing = false;
        private Color currentColor = Color.FromArgb(52, 73, 94);
        private float currentThickness = 3.0f;
        private bool isEraser = false;
        private bool isConnected = false;
        private bool isEnding = false;
        private bool connectionFailed = false;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            InitializeWhiteboard();
        }

        private void InitializeWhiteboard()
        {
            whiteboard = new Bitmap(pictureBoxWhiteboard.Width, pictureBoxWhiteboard.Height);
            whiteboardGraphics = Graphics.FromImage(whiteboard);
            whiteboardGraphics.Clear(Color.White);
            pictureBoxWhiteboard.Image = whiteboard;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (isConnected)
            {
                MessageBox.Show("Already connected to server.", "Connect", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string serverIP = txtServerIP.Text.Trim();
            if (string.IsNullOrEmpty(serverIP) || serverIP == "Enter server IP...")
            {
                MessageBox.Show("Please enter server IP address.", "Connect", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                client = new TcpClient();
                connectionFailed = false;
                client.Connect(serverIP, 8888);
                stream = client.GetStream();
                isConnected = true;

                UpdateStatus("Connected ✓", StatusType.Connected);
                btnConnect.Enabled = false;
                txtServerIP.Enabled = false;
                btnConnect.Text = "Connected ✓";

                Thread receiveThread = new Thread(ReceiveMessages);
                receiveThread.IsBackground = true;
                receiveThread.Start();
            }
            catch (Exception ex)
            {
                connectionFailed = true;
                MessageBox.Show("Cannot connect to server:\n" + ex.Message, "Connection Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus("Connection failed ✗", StatusType.Error);
            }
        }

        private enum StatusType { Connected, Disconnected, Info, Error }

        private void UpdateStatus(string text, StatusType type = StatusType.Info)
        {
            if (labelStatus.InvokeRequired)
            {
                labelStatus.Invoke((MethodInvoker)delegate { UpdateStatus(text, type); });
                return;
            }
            labelStatus.Text = text;
            switch (type)
            {
                case StatusType.Connected: labelStatus.ForeColor = Color.FromArgb(39, 174, 96); break;
                case StatusType.Info: labelStatus.ForeColor = Color.FromArgb(52, 73, 94); break;
                case StatusType.Error: labelStatus.ForeColor = Color.FromArgb(231, 76, 60); break;
                case StatusType.Disconnected: labelStatus.ForeColor = Color.FromArgb(149, 165, 166); break;
            }
        }

        private void ReceiveMessages()
        {
            try
            {
                byte[] typeBuffer = new byte[4];
                while (client.Connected && isConnected && !isEnding)
                {
                    int bytesRead = ReadFully(stream, typeBuffer, 0, 4);
                    if (bytesRead == 0) break;

                    MessageType msgType = (MessageType)BitConverter.ToInt32(typeBuffer, 0);

                    byte[] lengthBuffer = new byte[4];
                    if (ReadFully(stream, lengthBuffer, 0, 4) == 0) break;
                    int dataLength = BitConverter.ToInt32(lengthBuffer, 0);

                    byte[] data = null;
                    if (dataLength > 0)
                    {
                        data = new byte[dataLength];
                        if (ReadFully(stream, data, 0, dataLength) == 0) break;
                    }

                    ProcessMessage(msgType, data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection lost: " + ex.Message);
            }
            finally
            {
                if (!isEnding)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        UpdateStatus("Disconnected ✗", StatusType.Disconnected);
                        isConnected = false;
                        btnConnect.Enabled = true;
                        txtServerIP.Enabled = true;
                        btnConnect.Text = "🔄 Reconnect";
                    });
                }
            }
        }

        private int ReadFully(NetworkStream s, byte[] buffer, int offset, int size)
        {
            int totalRead = 0;
            while (totalRead < size)
            {
                int read = s.Read(buffer, offset + totalRead, size - totalRead);
                if (read == 0) return totalRead;
                totalRead += read;
            }
            return totalRead;
        }

        private void ProcessMessage(MessageType type, byte[] data)
        {
            switch (type)
            {
                case MessageType.DrawLine:
                    DrawData drawData = MessageSerializer.Deserialize<DrawData>(data);
                    DrawOnWhiteboardSafe(drawData);
                    break;

                case MessageType.InsertImage:
                    ImageData imageData = MessageSerializer.Deserialize<ImageData>(data);
                    DrawImageOnWhiteboardSafe(imageData);
                    break;

                case MessageType.Clear:
                    ClearWhiteboardSafe();
                    break;

                case MessageType.ClientCount:
                    if (data != null && data.Length >= 4)
                    {
                        int count = BitConverter.ToInt32(data, 0);
                        UpdateClientCountSafe(count);
                    }
                    break;

                case MessageType.SyncData:
                    SyncData syncData = MessageSerializer.Deserialize<SyncData>(data);
                    ApplySyncDataSafe(syncData);
                    break;

                case MessageType.End:
                    isEnding = true;
                    SaveWhiteboardImageSafe();
                    this.Invoke((MethodInvoker)delegate { Application.Exit(); });
                    break;
            }
        }

        private void ApplySyncDataSafe(SyncData syncData)
        {
            if (pictureBoxWhiteboard.InvokeRequired)
            {
                pictureBoxWhiteboard.Invoke((MethodInvoker)delegate { ApplySyncDataSafe(syncData); });
                return;
            }

            whiteboardGraphics.Clear(Color.White);

            if (syncData.DrawHistory != null)
            {
                foreach (DrawData d in syncData.DrawHistory)
                {
                    using (Pen pen = new Pen(d.IsEraser ? Color.White : d.GetColor(), d.Thickness))
                    {
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;
                        whiteboardGraphics.DrawLine(pen, d.X1, d.Y1, d.X2, d.Y2);
                    }
                }
            }

            if (syncData.ImageHistory != null)
            {
                foreach (ImageData imgData in syncData.ImageHistory)
                {
                    using (MemoryStream ms = new MemoryStream(imgData.ImageBytes))
                    using (Image img = Image.FromStream(ms))
                    {
                        whiteboardGraphics.DrawImage(img, imgData.X, imgData.Y, imgData.Width, imgData.Height);
                    }
                }
            }

            pictureBoxWhiteboard.Refresh();
        }

        private void DrawOnWhiteboardSafe(DrawData data)
        {
            if (pictureBoxWhiteboard.InvokeRequired)
            {
                pictureBoxWhiteboard.Invoke((MethodInvoker)delegate { DrawOnWhiteboardSafe(data); });
                return;
            }
            using (Pen pen = new Pen(data.IsEraser ? Color.White : data.GetColor(), data.Thickness))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                whiteboardGraphics.DrawLine(pen, data.X1, data.Y1, data.X2, data.Y2);
            }
            pictureBoxWhiteboard.Refresh();
        }

        private void DrawImageOnWhiteboardSafe(ImageData data)
        {
            if (pictureBoxWhiteboard.InvokeRequired)
            {
                pictureBoxWhiteboard.Invoke((MethodInvoker)delegate { DrawImageOnWhiteboardSafe(data); });
                return;
            }
            using (MemoryStream ms = new MemoryStream(data.ImageBytes))
            using (Image img = Image.FromStream(ms))
            {
                whiteboardGraphics.DrawImage(img, data.X, data.Y, data.Width, data.Height);
            }
            pictureBoxWhiteboard.Refresh();
        }

        private void ClearWhiteboardSafe()
        {
            if (pictureBoxWhiteboard.InvokeRequired)
            {
                pictureBoxWhiteboard.Invoke((MethodInvoker)delegate { ClearWhiteboardSafe(); });
                return;
            }
            whiteboardGraphics.Clear(Color.White);
            pictureBoxWhiteboard.Refresh();
        }

        private void UpdateClientCountSafe(int count)
        {
            if (panelClientCount.InvokeRequired)
            {
                panelClientCount.Invoke((MethodInvoker)delegate { UpdateClientCountSafe(count); });
                return;
            }
            labelClientCount.Text = count.ToString();
        }

        private void SaveWhiteboardImageSafe()
        {
            if (pictureBoxWhiteboard.InvokeRequired)
            {
                pictureBoxWhiteboard.Invoke((MethodInvoker)delegate { SaveWhiteboardImageSafe(); });
                return;
            }
            try
            {
                string fileName = $"Whiteboard_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
                whiteboard.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception ex) { Console.WriteLine("Save error: " + ex.Message); }
        }

        private void SendMessage(MessageType type, byte[] data)
        {
            if (!isConnected || stream == null) return;
            try
            {
                byte[] typeBytes = BitConverter.GetBytes((int)type);
                byte[] lengthBytes = BitConverter.GetBytes(data?.Length ?? 0);
                stream.Write(typeBytes, 0, 4);
                stream.Write(lengthBytes, 0, 4);
                if (data?.Length > 0) stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Send error: " + ex.Message);
                isConnected = false;
            }
        }

        private void SetCurrentColor(Color color)
        {
            currentColor = color;
            panelColorPreview.BackColor = color;
            isEraser = false;
            UpdateEraserButtonState();
        }

        private void UpdateEraserButtonState()
        {
            btnEraser.BackColor = isEraser ? Color.FromArgb(231, 76, 60) : Color.FromArgb(236, 240, 241);
            btnEraser.ForeColor = isEraser ? Color.White : Color.FromArgb(52, 73, 94);
        }

        private void pictureBoxWhiteboard_MouseDown(object sender, MouseEventArgs e)
        {
            if (!isConnected) return;
            isDrawing = true;
            lastPoint = e.Location;
        }

        private void pictureBoxWhiteboard_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isConnected || !isDrawing) return;

            DrawData data = new DrawData
            {
                X1 = lastPoint.X, Y1 = lastPoint.Y,
                X2 = e.X, Y2 = e.Y,
                Thickness = currentThickness,
                IsEraser = isEraser
            };
            data.SetColor(currentColor);

            DrawOnWhiteboardSafe(data);

            byte[] msgData = MessageSerializer.Serialize(data);
            SendMessage(MessageType.DrawLine, msgData);

            lastPoint = e.Location;
        }

        private void pictureBoxWhiteboard_MouseUp(object sender, MouseEventArgs e) { isDrawing = false; }

        private void btnColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                if (cd.ShowDialog() == DialogResult.OK)
                    SetCurrentColor(cd.Color);
            }
        }

        private void presetColor_Click(object sender, EventArgs e)
        {
            if (sender is Panel p)
                SetCurrentColor(p.BackColor);
        }

        private void trackBarThickness_Scroll(object sender, EventArgs e)
        {
            currentThickness = trackBarThickness.Value;
            labelThicknessVal.Text = currentThickness.ToString();
        }

        private void btnEraser_Click(object sender, EventArgs e)
        {
            isEraser = !isEraser;
            UpdateEraserButtonState();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (!isConnected) return;
            DialogResult dr = MessageBox.Show("Clear the entire whiteboard?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr != DialogResult.Yes) return;

            ClearWhiteboardSafe();
            SendMessage(MessageType.Clear, null);
        }

        private void btnInsertImage_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("Please connect to server first.", "Not Connected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string url = txtImageUrl.Text.Trim();
            if (string.IsNullOrEmpty(url) || url == "Paste image URL here...")
            {
                MessageBox.Show("Please enter an image URL.", "Insert Image", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (System.Net.WebClient webClient = new System.Net.WebClient())
                {
                    byte[] imageBytes = webClient.DownloadData(url);
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    using (Image img = Image.FromStream(ms))
                    {
                        int maxW = 300, maxH = 300;
                        int newW = img.Width, newH = img.Height;
                        if (newW > maxW || newH > maxH)
                        {
                            double r = Math.Min((double)maxW / newW, (double)maxH / newH);
                            newW = (int)(newW * r);
                            newH = (int)(newH * r);
                        }
                        using (Bitmap resized = new Bitmap(img, newW, newH))
                        using (MemoryStream rms = new MemoryStream())
                        {
                            resized.Save(rms, System.Drawing.Imaging.ImageFormat.Png);
                            ImageData imgData = new ImageData
                            {
                                ImageBytes = rms.ToArray(),
                                X = 50, Y = 50,
                                Width = newW, Height = newH
                            };
                            DrawImageOnWhiteboardSafe(imgData);
                            byte[] msgData = MessageSerializer.Serialize(imgData);
                            SendMessage(MessageType.InsertImage, msgData);
                        }
                    }
                }
                txtImageUrl.Clear();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message, "Image Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnEnd_Click(object sender, EventArgs e)
        {
            if (!isConnected) return;
            if (isEnding) return;

            DialogResult dr = MessageBox.Show("End session for ALL users?\nWhiteboard will be saved as PNG.",
                "End Session", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr != DialogResult.Yes) return;

            // Don't exit immediately - wait for server's End broadcast which will trigger the save
            isEnding = true;
            SendMessage(MessageType.End, null);

            // Start a timer - if server doesn't respond in 3s, save and exit anyway
            Timer timeout = new Timer();
            timeout.Interval = 3000;
            timeout.Tick += (s, args) =>
            {
                timeout.Stop();
                SaveWhiteboardImageSafe();
                this.Invoke((MethodInvoker)delegate { Application.Exit(); });
            };
            timeout.Start();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!isEnding && isConnected)
            {
                try { SendMessage(MessageType.End, null); } catch { }
            }

            if (stream != null) try { stream.Close(); } catch { }
            if (client != null) try { client.Close(); } catch { }

            if (whiteboardGraphics != null) whiteboardGraphics.Dispose();
            if (whiteboard != null) whiteboard.Dispose();

            base.OnFormClosing(e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetupColorPalette();
            trackBarThickness_Scroll(null, null);
            UpdateEraserButtonState();
            UpdateStatus("Not connected", StatusType.Disconnected);
        }

        private void SetupColorPalette()
        {
            Color[] colors = {
                Color.FromArgb(52, 73, 94),
                Color.FromArgb(44, 62, 80),
                Color.FromArgb(231, 76, 60),
                Color.FromArgb(230, 126, 34),
                Color.FromArgb(241, 196, 15),
                Color.FromArgb(46, 204, 113),
                Color.FromArgb(155, 89, 182),
                Color.FromArgb(52, 152, 219),
                Color.FromArgb(26, 188, 156),
                Color.FromArgb(149, 165, 166),
                Color.Black,
                Color.White
            };

            flowPresetColors.Controls.Clear();
            foreach (Color c in colors)
            {
                Panel p = new Panel
                {
                    Size = new Size(30, 30),
                    BackColor = c,
                    BorderStyle = BorderStyle.FixedSingle,
                    Cursor = Cursors.Hand,
                    Margin = new Padding(3)
                };
                p.Click += presetColor_Click;
                flowPresetColors.Controls.Add(p);
            }
        }

        private void txtServerIP_Enter(object sender, EventArgs e)
        {
            if (txtServerIP.Text == "Enter server IP...")
                txtServerIP.Text = "";
        }

        private void txtServerIP_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtServerIP.Text))
                txtServerIP.Text = "Enter server IP...";
        }

        private void txtImageUrl_Enter(object sender, EventArgs e)
        {
            if (txtImageUrl.Text == "Paste image URL here...")
                txtImageUrl.Text = "";
        }

        private void txtImageUrl_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtImageUrl.Text))
                txtImageUrl.Text = "Paste image URL here...";
        }
    }
}
