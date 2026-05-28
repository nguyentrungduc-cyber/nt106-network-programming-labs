using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private Color currentColor = Color.Black;
        private float currentThickness = 2.0f;
        private bool isEraser = false;
        private bool isConnected = false;

        public Form1()
        {
            InitializeComponent();
            InitializeWhiteboard();
        }

        private void InitializeWhiteboard()
        {
            whiteboard = new Bitmap(pictureBoxWhiteboard.Width, pictureBoxWhiteboard.Height);
            whiteboardGraphics = Graphics.FromImage(whiteboard);
            whiteboardGraphics.Clear(Color.White);
            pictureBoxWhiteboard.Image = whiteboard;
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (isConnected)
            {
                MessageBox.Show("Already connected to server");
                return;
            }

            string serverIP = textBoxServerIP.Text.Trim();
            if (string.IsNullOrEmpty(serverIP))
            {
                MessageBox.Show("Please enter server IP address");
                return;
            }

            try
            {
                client = new TcpClient();
                client.Connect(serverIP, 8888);
                stream = client.GetStream();
                isConnected = true;
                
                labelStatus.Text = "Connected to server";
                buttonConnect.Enabled = false;
                textBoxServerIP.Enabled = false;
                
                // Start receiving messages
                Thread receiveThread = new Thread(ReceiveMessages);
                receiveThread.IsBackground = true;
                receiveThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to server: " + ex.Message);
            }
        }

        private void ReceiveMessages()
        {
            try
            {
                while (client.Connected && isConnected)
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
                    ProcessMessage(msgType, data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection lost: " + ex.Message);
                this.Invoke((MethodInvoker)delegate {
                    labelStatus.Text = "Disconnected from server";
                    isConnected = false;
                });
            }
        }

        private void ProcessMessage(MessageType type, byte[] data)
        {
            switch (type)
            {
                case MessageType.DrawLine:
                    DrawData drawData = MessageSerializer.Deserialize<DrawData>(data);
                    DrawOnWhiteboard(drawData);
                    break;
                    
                case MessageType.InsertImage:
                    ImageData imageData = MessageSerializer.Deserialize<ImageData>(data);
                    DrawImageOnWhiteboard(imageData);
                    break;
                    
                case MessageType.Clear:
                    ClearWhiteboard();
                    break;
                    
                case MessageType.ClientCount:
                    int count = BitConverter.ToInt32(data, 0);
                    UpdateClientCount(count);
                    break;
                    
                case MessageType.SyncData:
                    SyncData syncData = MessageSerializer.Deserialize<SyncData>(data);
                    ApplySyncData(syncData);
                    break;
                    
                case MessageType.End:
                    SaveWhiteboardImage();
                    this.Invoke((MethodInvoker)delegate {
                        Application.Exit();
                    });
                    break;
            }
        }

        private void ApplySyncData(SyncData syncData)
        {
            if (pictureBoxWhiteboard.InvokeRequired)
            {
                pictureBoxWhiteboard.Invoke((MethodInvoker)delegate {
                    ApplySyncData(syncData);
                });
                return;
            }

            // Clear and redraw everything
            whiteboardGraphics.Clear(Color.White);
            
            // Draw all lines
            if (syncData.DrawHistory != null)
            {
                foreach (DrawData drawData in syncData.DrawHistory)
                {
                    using (Pen pen = new Pen(drawData.GetColor(), drawData.Thickness))
                    {
                        if (drawData.IsEraser)
                        {
                            pen.Color = Color.White;
                        }
                        whiteboardGraphics.DrawLine(pen, drawData.X1, drawData.Y1, drawData.X2, drawData.Y2);
                    }
                }
            }
            
            // Draw all images
            if (syncData.ImageHistory != null)
            {
                foreach (ImageData imageData in syncData.ImageHistory)
                {
                    using (MemoryStream ms = new MemoryStream(imageData.ImageBytes))
                    {
                        Image img = Image.FromStream(ms);
                        whiteboardGraphics.DrawImage(img, imageData.X, imageData.Y, imageData.Width, imageData.Height);
                    }
                }
            }
            
            pictureBoxWhiteboard.Refresh();
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

        private void UpdateClientCount(int count)
        {
            if (labelClientCount.InvokeRequired)
            {
                labelClientCount.Invoke((MethodInvoker)delegate {
                    UpdateClientCount(count);
                });
                return;
            }

            labelClientCount.Text = $"Connected Clients: {count}";
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
                if (data != null && data.Length > 0)
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending message: " + ex.Message);
                isConnected = false;
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
            if (!isConnected) return;
            isDrawing = true;
            lastPoint = e.Location;
        }

        private void pictureBoxWhiteboard_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isConnected || !isDrawing) return;

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
            
            DrawOnWhiteboard(data);
            
            byte[] msgData = MessageSerializer.Serialize(data);
            SendMessage(MessageType.DrawLine, msgData);
            
            lastPoint = e.Location;
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
            if (!isConnected) return;
            ClearWhiteboard();
            SendMessage(MessageType.Clear, null);
        }

        private void buttonEnd_Click(object sender, EventArgs e)
        {
            if (!isConnected) return;
            SaveWhiteboardImage();
            SendMessage(MessageType.End, null);
            Application.Exit();
        }

        private void buttonEraser_Click(object sender, EventArgs e)
        {
            isEraser = !isEraser;
            buttonEraser.BackColor = isEraser ? Color.LightGray : SystemColors.Control;
        }

        private void buttonInsertImage_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("Please connect to server first");
                return;
            }

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
                        
                        DrawImageOnWhiteboard(imageData);
                        
                        byte[] msgData = MessageSerializer.Serialize(imageData);
                        SendMessage(MessageType.InsertImage, msgData);
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
            
            if (isConnected && client != null)
            {
                client.Close();
            }
        }
    }
}
