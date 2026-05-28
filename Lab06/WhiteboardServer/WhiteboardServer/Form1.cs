using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Net.Mail;
using System.Threading;

namespace WhiteboardServer
{
    public partial class Form1 : Form
    {
        private TcpListener server;
        private List<TcpClient> clients = new List<TcpClient>();
        private List<DrawData> drawHistory = new List<DrawData>();
        private List<ImageData> imageHistory = new List<ImageData>();
        private const int MAX_CLIENTS = 5;
        private bool emailSent = false;
        private object lockObj = new object();
        private Bitmap whiteboard;
        private Graphics whiteboardGraphics;
        private Point lastPoint;
        private bool isDrawing = false;
        private Color currentColor = Color.FromArgb(52, 73, 94);
        private float currentThickness = 3.0f;
        private bool isEraser = false;
        private bool isServerRunning = false;
        private bool isEnding = false;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            InitializeWhiteboard();
            StartServer();
        }

        private void InitializeWhiteboard()
        {
            whiteboard = new Bitmap(pictureBoxWhiteboard.Width, pictureBoxWhiteboard.Height);
            whiteboardGraphics = Graphics.FromImage(whiteboard);
            whiteboardGraphics.Clear(Color.White);
            pictureBoxWhiteboard.Image = whiteboard;
        }

        private void StartServer()
        {
            try
            {
                server = new TcpListener(IPAddress.Any, 8888);
                server.Start();
                isServerRunning = true;
                UpdateStatus("Server running on port 8888", StatusType.Running);

                Thread acceptThread = new Thread(AcceptClients);
                acceptThread.IsBackground = true;
                acceptThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting server: " + ex.Message);
            }
        }

        private enum StatusType { Running, Info, Warning, Error }

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
                case StatusType.Running: labelStatus.ForeColor = Color.FromArgb(39, 174, 96); break;
                case StatusType.Info: labelStatus.ForeColor = Color.FromArgb(52, 73, 94); break;
                case StatusType.Warning: labelStatus.ForeColor = Color.FromArgb(243, 156, 18); break;
                case StatusType.Error: labelStatus.ForeColor = Color.FromArgb(231, 76, 60); break;
            }
        }

        private void AcceptClients()
        {
            while (isServerRunning)
            {
                try
                {
                    TcpClient client = server.AcceptTcpClient();

                    lock (lockObj)
                    {
                        clients.Add(client);
                        UpdateClientCountUI();

                        if (clients.Count >= MAX_CLIENTS && !emailSent)
                        {
                            SendEmailAlert();
                            emailSent = true;
                        }
                    }

                    SendSyncData(client);

                    Thread clientThread = new Thread(() => HandleClient(client));
                    clientThread.IsBackground = true;
                    clientThread.Start();
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Accept error: " + ex.Message);
                }
            }
        }

        private void HandleClient(TcpClient client)
        {
            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();
                byte[] typeBuffer = new byte[4];

                while (client.Connected && isServerRunning)
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

                    ProcessMessage(msgType, data, client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Client disconnected: " + ex.Message);
            }
            finally
            {
                lock (lockObj)
                {
                    clients.Remove(client);
                    UpdateClientCountUI();
                }
                client.Close();
            }
        }

        private int ReadFully(NetworkStream stream, byte[] buffer, int offset, int size)
        {
            int totalRead = 0;
            while (totalRead < size)
            {
                int read = stream.Read(buffer, offset + totalRead, size - totalRead);
                if (read == 0) return totalRead;
                totalRead += read;
            }
            return totalRead;
        }

        private void ProcessMessage(MessageType type, byte[] data, TcpClient sender)
        {
            switch (type)
            {
                case MessageType.DrawLine:
                    DrawData drawData = MessageSerializer.Deserialize<DrawData>(data);
                    lock (lockObj) { drawHistory.Add(drawData); }
                    DrawOnWhiteboardSafe(drawData);
                    BroadcastMessage(type, data, sender);
                    break;

                case MessageType.InsertImage:
                    ImageData imageData = MessageSerializer.Deserialize<ImageData>(data);
                    lock (lockObj) { imageHistory.Add(imageData); }
                    DrawImageOnWhiteboardSafe(imageData);
                    BroadcastMessage(type, data, sender);
                    break;

                case MessageType.Clear:
                    lock (lockObj) { drawHistory.Clear(); imageHistory.Clear(); }
                    ClearWhiteboardSafe();
                    BroadcastMessage(type, data, sender);
                    break;

                case MessageType.End:
                    isEnding = true;
                    SaveWhiteboardImageSafe();
                    BroadcastMessage(MessageType.End, null, null);
                    this.Invoke((MethodInvoker)delegate { Application.Exit(); });
                    break;

                case MessageType.RequestSync:
                    SendSyncData(sender);
                    break;
            }
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

        private void BroadcastMessage(MessageType type, byte[] data, TcpClient excludeClient)
        {
            byte[] typeBytes = BitConverter.GetBytes((int)type);
            byte[] lengthBytes = BitConverter.GetBytes(data?.Length ?? 0);

            lock (lockObj)
            {
                List<TcpClient> disconnected = new List<TcpClient>();
                foreach (TcpClient client in clients)
                {
                    if (client == excludeClient) continue;
                    try
                    {
                        NetworkStream s = client.GetStream();
                        s.Write(typeBytes, 0, 4);
                        s.Write(lengthBytes, 0, 4);
                        if (data?.Length > 0) s.Write(data, 0, data.Length);
                    }
                    catch { disconnected.Add(client); }
                }
                foreach (TcpClient c in disconnected) { clients.Remove(c); c.Close(); }
                if (disconnected.Count > 0) UpdateClientCountUI();
            }
        }

        private void SendSyncData(TcpClient client)
        {
            lock (lockObj)
            {
                SyncData syncData = new SyncData
                {
                    DrawHistory = drawHistory.ToArray(),
                    ImageHistory = imageHistory.ToArray()
                };
                byte[] data = MessageSerializer.Serialize(syncData);
                byte[] typeBytes = BitConverter.GetBytes((int)MessageType.SyncData);
                byte[] lengthBytes = BitConverter.GetBytes(data.Length);
                try
                {
                    NetworkStream s = client.GetStream();
                    s.Write(typeBytes, 0, 4);
                    s.Write(lengthBytes, 0, 4);
                    s.Write(data, 0, data.Length);
                }
                catch (Exception ex) { Console.WriteLine("Sync error: " + ex.Message); }
            }
        }

        private void UpdateClientCountUI()
        {
            if (panelClientCount.InvokeRequired)
            {
                panelClientCount.Invoke((MethodInvoker)delegate { UpdateClientCountUI(); });
                return;
            }
            int count = clients.Count;
            labelClientCount.Text = count.ToString();

            byte[] countData = BitConverter.GetBytes(count);
            BroadcastMessage(MessageType.ClientCount, countData, null);
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

        private void SendEmailAlert()
        {
            try
            {
                // Configure your email settings below
                string smtpServer = "smtp.gmail.com";
                int smtpPort = 587;
                string fromEmail = "your-email@gmail.com";
                string toEmail = "admin@example.com";
                string password = "your-app-password";

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(fromEmail);
                    mail.To.Add(toEmail);
                    mail.Subject = "Whiteboard Alert: Max Clients Reached";
                    mail.Body = $"Maximum client limit of {MAX_CLIENTS} reached at {DateTime.Now:yyyy-MM-dd HH:mm:ss}.";

                    using (SmtpClient smtp = new SmtpClient(smtpServer, smtpPort))
                    {
                        smtp.Credentials = new NetworkCredential(fromEmail, password);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
                Console.WriteLine("Email alert sent");
            }
            catch (Exception ex) { Console.WriteLine("Email error: " + ex.Message); }
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
            if (!isServerRunning) return;
            isDrawing = true;
            lastPoint = e.Location;
        }

        private void pictureBoxWhiteboard_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isServerRunning || !isDrawing) return;

            DrawData data = new DrawData
            {
                X1 = lastPoint.X, Y1 = lastPoint.Y,
                X2 = e.X, Y2 = e.Y,
                Thickness = currentThickness,
                IsEraser = isEraser
            };
            data.SetColor(currentColor);

            lock (lockObj) { drawHistory.Add(data); }

            DrawOnWhiteboardSafe(data);

            byte[] msgData = MessageSerializer.Serialize(data);
            BroadcastMessage(MessageType.DrawLine, msgData, null);

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
            DialogResult dr = MessageBox.Show("Clear the entire whiteboard?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr != DialogResult.Yes) return;

            lock (lockObj) { drawHistory.Clear(); imageHistory.Clear(); }
            ClearWhiteboardSafe();
            BroadcastMessage(MessageType.Clear, null, null);
        }

        private void btnInsertImage_Click(object sender, EventArgs e)
        {
            string url = txtImageUrl.Text.Trim();
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("Please enter an image URL", "Insert Image", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                            lock (lockObj) { imageHistory.Add(imgData); }
                            DrawImageOnWhiteboardSafe(imgData);
                            byte[] msgData = MessageSerializer.Serialize(imgData);
                            BroadcastMessage(MessageType.InsertImage, msgData, null);
                        }
                    }
                }
                txtImageUrl.Clear();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message, "Image Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnEnd_Click(object sender, EventArgs e)
        {
            if (isEnding) return;
            DialogResult dr = MessageBox.Show("End session for ALL clients?\nWhiteboard will be saved as PNG.",
                "End Session", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr != DialogResult.Yes) return;

            isEnding = true;
            SaveWhiteboardImageSafe();
            BroadcastMessage(MessageType.End, null, null);
            Application.Exit();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            isServerRunning = false;

            if (!isEnding)
            {
                try { BroadcastMessage(MessageType.End, null, null); } catch { }
            }

            if (server != null) { try { server.Stop(); } catch { } }

            lock (lockObj)
            {
                foreach (TcpClient c in clients) { try { c.Close(); } catch { } }
                clients.Clear();
            }

            if (whiteboardGraphics != null) whiteboardGraphics.Dispose();
            if (whiteboard != null) whiteboard.Dispose();

            base.OnFormClosing(e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetupColorPalette();
            trackBarThickness_Scroll(null, null);
            UpdateEraserButtonState();
        }

        private void SetupColorPalette()
        {
            Color[] colors = {
                Color.FromArgb(52, 73, 94),  // Dark gray
                Color.FromArgb(44, 62, 80),  // Darker gray
                Color.FromArgb(231, 76, 60), // Red
                Color.FromArgb(230, 126, 34), // Orange
                Color.FromArgb(241, 196, 15), // Yellow
                Color.FromArgb(46, 204, 113), // Green
                Color.FromArgb(155, 89, 182), // Purple
                Color.FromArgb(52, 152, 219), // Blue
                Color.FromArgb(26, 188, 156), // Teal
                Color.FromArgb(149, 165, 166), // Gray
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
                if (c == Color.White) p.BorderStyle = BorderStyle.FixedSingle;
                p.Click += presetColor_Click;
                flowPresetColors.Controls.Add(p);
            }
        }
    }
}
