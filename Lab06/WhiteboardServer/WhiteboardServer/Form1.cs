using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private Color currentColor = Color.Black;
        private float currentThickness = 2.0f;
        private bool isEraser = false;

        public Form1()
        {
            InitializeComponent();
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
                labelStatus.Text = "Server started on port 8888";
                
                Thread acceptThread = new Thread(AcceptClients);
                acceptThread.IsBackground = true;
                acceptThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting server: " + ex.Message);
            }
        }

        private void AcceptClients()
        {
            while (true)
            {
                try
                {
                    TcpClient client = server.AcceptTcpClient();
                    
                    lock (lockObj)
                    {
                        clients.Add(client);
                        UpdateClientCount();
                        
                        // Check if limit reached and send email
                        if (clients.Count >= MAX_CLIENTS && !emailSent)
                        {
                            SendEmailAlert();
                            emailSent = true;
                        }
                    }

                    // Send current whiteboard state to new client
                    SendSyncData(client);

                    // Start handling this client
                    Thread clientThread = new Thread(() => HandleClient(client));
                    clientThread.IsBackground = true;
                    clientThread.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error accepting client: " + ex.Message);
                }
            }
        }

        private void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            
            try
            {
                while (client.Connected)
                {
                    // Read message type
                    byte[] typeBuffer = new byte[4];
                    int bytesRead = stream.Read(typeBuffer, 0, 4);
                    if (bytesRead == 0) break;
                    
                    MessageType msgType = (MessageType)BitConverter.ToInt32(typeBuffer, 0);
                    
                    // Read data length
                    byte[] lengthBuffer = new byte[4];
                    stream.Read(lengthBuffer, 0, 4);
                    int dataLength = BitConverter.ToInt32(lengthBuffer, 0);
                    
                    // Read data
                    byte[] data = null;
                    if (dataLength > 0)
                    {
                        data = new byte[dataLength];
                        int totalRead = 0;
                        while (totalRead < dataLength)
                        {
                            int read = stream.Read(data, totalRead, dataLength - totalRead);
                            if (read == 0) break;
                            totalRead += read;
                        }
                    }
                    
                    // Process message
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
                    UpdateClientCount();
                }
                client.Close();
            }
        }

        private void ProcessMessage(MessageType type, byte[] data, TcpClient sender)
        {
            switch (type)
            {
                case MessageType.DrawLine:
                    DrawData drawData = MessageSerializer.Deserialize<DrawData>(data);
                    lock (lockObj)
                    {
                        drawHistory.Add(drawData);
                    }
                    DrawOnWhiteboard(drawData);
                    BroadcastMessage(type, data, sender);
                    break;
                    
                case MessageType.InsertImage:
                    ImageData imageData = MessageSerializer.Deserialize<ImageData>(data);
                    lock (lockObj)
                    {
                        imageHistory.Add(imageData);
                    }
                    DrawImageOnWhiteboard(imageData);
                    BroadcastMessage(type, data, sender);
                    break;
                    
                case MessageType.Clear:
                    lock (lockObj)
                    {
                        drawHistory.Clear();
                        imageHistory.Clear();
                    }
                    ClearWhiteboard();
                    BroadcastMessage(type, data, sender);
                    break;
                    
                case MessageType.End:
                    SaveWhiteboardImage();
                    BroadcastMessage(type, data, null);
                    this.Invoke((MethodInvoker)delegate {
                        Application.Exit();
                    });
                    break;
                    
                case MessageType.RequestSync:
                    SendSyncData(sender);
                    break;
            }
        }

        private void DrawOnWhiteboard(DrawData data)
        {
            if (pictureBoxWhiteboard.InvokeRequired)
            {
                pictureBoxWhiteboard.Invoke((MethodInvoker)delegate {
                    DrawOnWhiteboard(data);
                });
                return;
            }

            using (Pen pen = new Pen(data.GetColor(), data.Thickness))
            {
                if (data.IsEraser)
                {
                    pen.Color = Color.White;
                }
                whiteboardGraphics.DrawLine(pen, data.X1, data.Y1, data.X2, data.Y2);
            }
            pictureBoxWhiteboard.Refresh();
        }

        private void DrawImageOnWhiteboard(ImageData data)
        {
            if (pictureBoxWhiteboard.InvokeRequired)
            {
                pictureBoxWhiteboard.Invoke((MethodInvoker)delegate {
                    DrawImageOnWhiteboard(data);
                });
                return;
            }

            using (MemoryStream ms = new MemoryStream(data.ImageBytes))
            {
                Image img = Image.FromStream(ms);
                whiteboardGraphics.DrawImage(img, data.X, data.Y, data.Width, data.Height);
            }
            pictureBoxWhiteboard.Refresh();
        }

        private void ClearWhiteboard()
        {
            if (pictureBoxWhiteboard.InvokeRequired)
            {
                pictureBoxWhiteboard.Invoke((MethodInvoker)delegate {
                    ClearWhiteboard();
                });
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
                        NetworkStream stream = client.GetStream();
                        stream.Write(typeBytes, 0, 4);
                        stream.Write(lengthBytes, 0, 4);
                        if (data != null && data.Length > 0)
                        {
                            stream.Write(data, 0, data.Length);
                        }
                    }
                    catch
                    {
                        disconnected.Add(client);
                    }
                }
                
                foreach (TcpClient client in disconnected)
                {
                    clients.Remove(client);
                    client.Close();
                }
                
                if (disconnected.Count > 0)
                {
                    UpdateClientCount();
                }
            }
        }

        private void SendSyncData(TcpClient client)
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
                NetworkStream stream = client.GetStream();
                stream.Write(typeBytes, 0, 4);
                stream.Write(lengthBytes, 0, 4);
                stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending sync data: " + ex.Message);
            }
        }

        private void UpdateClientCount()
        {
            if (labelClientCount.InvokeRequired)
            {
                labelClientCount.Invoke((MethodInvoker)delegate {
                    UpdateClientCount();
                });
                return;
            }

            labelClientCount.Text = $"Connected Clients: {clients.Count}";
            
            // Broadcast client count to all clients
            byte[] countData = BitConverter.GetBytes(clients.Count);
            BroadcastMessage(MessageType.ClientCount, countData, null);
        }

        private void SendEmailAlert()
        {
            try
            {
                // Configure your email settings here
                string smtpServer = "smtp.gmail.com";
                int smtpPort = 587;
                string fromEmail = "your-email@gmail.com"; // Change this
                string toEmail = "admin@example.com"; // Change this
                string password = "your-app-password"; // Change this
                
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(fromEmail);
                mail.To.Add(toEmail);
                mail.Subject = "Whiteboard Server Alert: Client Limit Reached";
                mail.Body = $"The whiteboard server has reached the maximum client limit of {MAX_CLIENTS} clients at {DateTime.Now}.";
                
                SmtpClient smtp = new SmtpClient(smtpServer, smtpPort);
                smtp.Credentials = new System.Net.NetworkCredential(fromEmail, password);
                smtp.EnableSsl = true;
                
                smtp.Send(mail);
                
                Console.WriteLine("Email alert sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
                // Don't crash the server if email fails
            }
        }

        private void SaveWhiteboardImage()
        {
            try
            {
                string fileName = $"Whiteboard_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
                whiteboard.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving image: " + ex.Message);
            }
        }

        private void pictureBoxWhiteboard_MouseDown(object sender, MouseEventArgs e)
        {
            isDrawing = true;
            lastPoint = e.Location;
        }

        private void pictureBoxWhiteboard_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                DrawData data = new DrawData
                {
                    X1 = lastPoint.X,
                    Y1 = lastPoint.Y,
                    X2 = e.X,
                    Y2 = e.Y,
                    Thickness = currentThickness,
                    IsEraser = isEraser
                };
                data.SetColor(currentColor);
                
                lock (lockObj)
                {
                    drawHistory.Add(data);
                }
                
                DrawOnWhiteboard(data);
                
                byte[] msgData = MessageSerializer.Serialize(data);
                BroadcastMessage(MessageType.DrawLine, msgData, null);
                
                lastPoint = e.Location;
            }
        }

        private void pictureBoxWhiteboard_MouseUp(object sender, MouseEventArgs e)
        {
            isDrawing = false;
        }

        private void buttonColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                currentColor = colorDialog.Color;
                panelColorPreview.BackColor = currentColor;
                isEraser = false;
            }
        }

        private void trackBarThickness_Scroll(object sender, EventArgs e)
        {
            currentThickness = trackBarThickness.Value;
            labelThickness.Text = $"Thickness: {currentThickness}";
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            lock (lockObj)
            {
                drawHistory.Clear();
                imageHistory.Clear();
            }
            ClearWhiteboard();
            BroadcastMessage(MessageType.Clear, null, null);
        }

        private void buttonEnd_Click(object sender, EventArgs e)
        {
            SaveWhiteboardImage();
            BroadcastMessage(MessageType.End, null, null);
            Application.Exit();
        }

        private void buttonEraser_Click(object sender, EventArgs e)
        {
            isEraser = !isEraser;
            buttonEraser.BackColor = isEraser ? Color.LightGray : SystemColors.Control;
        }

        private void buttonInsertImage_Click(object sender, EventArgs e)
        {
            string url = textBoxImageUrl.Text.Trim();
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("Please enter an image URL");
                return;
            }

            try
            {
                System.Net.WebClient webClient = new System.Net.WebClient();
                byte[] imageBytes = webClient.DownloadData(url);
                
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    Image img = Image.FromStream(ms);
                    
                    // Resize image if needed
                    int maxWidth = 300;
                    int maxHeight = 300;
                    int newWidth = img.Width;
                    int newHeight = img.Height;
                    
                    if (img.Width > maxWidth || img.Height > maxHeight)
                    {
                        double ratioX = (double)maxWidth / img.Width;
                        double ratioY = (double)maxHeight / img.Height;
                        double ratio = Math.Min(ratioX, ratioY);
                        
                        newWidth = (int)(img.Width * ratio);
                        newHeight = (int)(img.Height * ratio);
                    }
                    
                    Bitmap resized = new Bitmap(img, newWidth, newHeight);
                    using (MemoryStream resizedMs = new MemoryStream())
                    {
                        resized.Save(resizedMs, System.Drawing.Imaging.ImageFormat.Png);
                        
                        ImageData imageData = new ImageData
                        {
                            ImageBytes = resizedMs.ToArray(),
                            X = 50,
                            Y = 50,
                            Width = newWidth,
                            Height = newHeight
                        };
                        
                        lock (lockObj)
                        {
                            imageHistory.Add(imageData);
                        }
                        
                        DrawImageOnWhiteboard(imageData);
                        
                        byte[] msgData = MessageSerializer.Serialize(imageData);
                        BroadcastMessage(MessageType.InsertImage, msgData, null);
                    }
                }
                
                textBoxImageUrl.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading image: " + ex.Message);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            
            // Notify all clients to close
            BroadcastMessage(MessageType.End, null, null);
            
            // Clean up
            if (server != null)
            {
                server.Stop();
            }
            
            lock (lockObj)
            {
                foreach (TcpClient client in clients)
                {
                    client.Close();
                }
                clients.Clear();
            }
        }
    }
}
