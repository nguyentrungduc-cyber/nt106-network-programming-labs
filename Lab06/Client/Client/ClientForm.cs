using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

enum DrawingMode
{
    FreeHand,
    Rectangle,
    Ellipse,
    Line
}

namespace Client
{
    public partial class ClientForm : Form
    {
        private Rectangle currentImageRect = Rectangle.Empty;
        private string currentImageBase64 = ""; // THÊM DÒNG NÀY để theo dõi chuỗi dữ liệu ảnh hiện tại
        private Rectangle GetRectangleFromPoints(Point p1, Point p2)
        {
            return new Rectangle(
                Math.Min(p1.X, p2.X),
                Math.Min(p1.Y, p2.Y),
                Math.Abs(p1.X - p2.X),
                Math.Abs(p1.Y - p2.Y));
        }
        List<Point> currentLine = null; 
        List<List<Point>> lines = new List<List<Point>>();

        private DateTime lastSendTime = DateTime.MinValue;

        private TcpClient client;
        private NetworkStream stream;

        private Point startPoint;    // điểm bắt đầu vẽ (MouseDown)
        private Point lastPoint;     // điểm cuối cùng vẽ (dùng cho FreeHand hoặc vẽ)
        private Point currentPoint;  // điểm hiện tại khi kéo chuột (MouseMove) dùng để vẽ tạm thời
        private bool isDrawing = false;
        private DrawingMode currentMode;  // enum bạn tự định nghĩa như Line, Rectangle, Ellipse, FreeHand

        private Bitmap drawingBitmap;
        private Graphics graphics;

        private Color previousColor = Color.Black;

        private Color currentColor = Color.Black;
        private int penThickness = 2;

        private Image currentImage = null;
        // private Rectangle currentImageRect = Rectangle.Empty;
        private bool isMovingImage = false;
        private bool isResizingImage = false;
        private Point mouseDownPos;
        private Point imageMoveStartPos;
        private Point resizeStartPos;

        private const int resizeHandleSize = 10;
        private const int MaxImageWidth = 200;
        private const int MaxImageHeight = 200;

        public ClientForm()
        {
            InitializeComponent();

            drawingBitmap = new Bitmap(panelWhiteboard.Width, panelWhiteboard.Height);
            graphics = Graphics.FromImage(drawingBitmap);
            graphics.Clear(Color.White);

          
            panelWhiteboard.BackgroundImageLayout = ImageLayout.None;

            ConnectToServer();
            // Đặt màu trắng của drawingBitmap thành trong suốt để không che mất ảnh nằm phía dưới
            drawingBitmap.MakeTransparent(Color.White);
            numericUpDownThickness.Minimum = 1;
            numericUpDownThickness.Maximum = 10;
            numericUpDownThickness.Value = 2;
            numericUpDownThickness.ValueChanged += NumericUpDownThickness_ValueChanged;

            panelWhiteboard.MouseDown += PanelWhiteboard_MouseDown;
            panelWhiteboard.MouseMove += PanelWhiteboard_MouseMove;
            panelWhiteboard.MouseUp += panelWhiteboard_MouseUp;

            btnInsertImage.Click += btnInsertImage_Click;
            // buttonEnd.Click += buttonEnd_Click;
            btnChooseColor.Click += btnChooseColor_Click;

            comboBoxDrawMode.Items.Add("Freehand");
            comboBoxDrawMode.Items.Add("Rectangle");
            comboBoxDrawMode.Items.Add("Ellipse");
            comboBoxDrawMode.Items.Add("Line");
            comboBoxDrawMode.SelectedIndex = 0; // chọn mặc định là Freehand
            comboBoxDrawMode.SelectedIndexChanged += ComboBoxDrawMode_SelectedIndexChanged;

        }
        private void SendCurrentImagePositionThrottled()
        {
            var now = DateTime.Now;
            if ((now - lastSendTime).TotalMilliseconds > 50) // cách nhau ít nhất 50ms (20 lần/s)
            {
                SendCurrentImagePosition();
                lastSendTime = now;
            }
        }
        private void ConnectToServer()
        {
            try
            {
                client = new TcpClient("127.0.0.1", 9000);
                stream = client.GetStream();

                Thread listenThread = new Thread(ListenData);
                listenThread.IsBackground = true;
                listenThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot connect to server: " + ex.Message);
                Environment.Exit(0);
            }
        }
        private void ListenData()
        {
            byte[] buffer = new byte[4096];
            StringBuilder sb = new StringBuilder();

            while (client != null && client.Connected)
            {
                try
                {
                    if (stream.DataAvailable)
                    {
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 0) break;

                        string chunk = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        sb.Append(chunk);

                        string allData = sb.ToString();
                        int newlineIndex;
                        while ((newlineIndex = allData.IndexOf('\n')) >= 0)
                        {
                            string line = allData.Substring(0, newlineIndex).Trim();
                            if (line.Length > 0)
                                ProcessDrawingMessage(line);

                            allData = allData.Substring(newlineIndex + 1);
                        }
                        sb.Clear();
                        sb.Append(allData);
                    }
                    Thread.Sleep(10);
                }
                catch (IOException ex)
                {
                    Console.WriteLine("IO Error: " + ex.Message);
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    break;
                }
            }

            this.Invoke((MethodInvoker)delegate
            {
                MessageBox.Show("Lost connection to server.");
                Application.Exit();
            });
        }
        private void ProcessDrawingMessage(string msg)
        {
            if (msg.StartsWith("DRAW"))
            {
                Console.WriteLine("[CLIENT] Received DRAW message: " + msg);

                string[] parts = msg.Split(';');
                if (parts.Length != 8) return; 

                string shape = parts[1];

                if (int.TryParse(parts[2], out int argb) &&
                    float.TryParse(parts[3], out float thickness) &&
                    int.TryParse(parts[4], out int x1) &&
                    int.TryParse(parts[5], out int y1) &&
                    int.TryParse(parts[6], out int x2) &&
                    int.TryParse(parts[7], out int y2))
                {
                    Color c = Color.FromArgb(argb);
                    Point p1 = new Point(x1, y1);
                    Point p2 = new Point(x2, y2);

                    this.Invoke((MethodInvoker)delegate
                    {
                        lock (drawingBitmap)
                        {
                            using (Graphics g = Graphics.FromImage(drawingBitmap))
                            using (Pen pen = new Pen(c, thickness))
                            {
                                switch (shape)
                                {
                                    case "Line":
                                        g.DrawLine(pen, p1, p2);
                                        break;
                                    case "Rectangle":
                                        g.DrawRectangle(pen, GetRectangleFromPoints(p1, p2));
                                        break;
                                    case "Ellipse":
                                        g.DrawEllipse(pen, GetRectangleFromPoints(p1, p2));
                                        break;
                                    case "FreeHand":
                                        g.DrawLine(pen, p1, p2); 
                                        break;
                                }
                            }
                            panelWhiteboard.Invalidate();
                        }
                    });
                }
            }
            else if (msg.StartsWith("IMAGE"))
            {
                string[] parts = msg.Split(';');
                if (parts.Length < 6) return;

                if (int.TryParse(parts[1], out int x) &&
                    int.TryParse(parts[2], out int y) &&
                    int.TryParse(parts[3], out int w) &&
                    int.TryParse(parts[4], out int h))
                {
                    string base64Image = parts[5];
                    try
                    {
                        // Kiểm tra xem dữ liệu ảnh nhận về có phải là một ảnh mới hoàn toàn không
                        bool isNewImage = (base64Image != currentImageBase64);

                        byte[] imgBytes = Convert.FromBase64String(base64Image);
                        using (MemoryStream ms = new MemoryStream(imgBytes))
                        {
                            Image img = Image.FromStream(ms);

                            this.Invoke((MethodInvoker)delegate
                            {
                                lock (drawingBitmap)
                                {
                                    // Nếu phát hiện là ảnh mới VÀ đang có một ảnh cũ hiển thị:
                                    // Hãy nướng ảnh cũ vào nền vẽ tĩnh của các Client khác trước khi ghi đè ảnh mới
                                    if (isNewImage && currentImage != null && currentImageRect != Rectangle.Empty)
                                    {
                                        using (Graphics g = Graphics.FromImage(drawingBitmap))
                                        {
                                            g.DrawImage(currentImage, currentImageRect);
                                        }
                                    }

                                    currentImage = img; // Cập nhật ảnh hoạt động mới
                                    currentImageRect = new Rectangle(x, y, w, h); // Cập nhật vị trí mới
                                    currentImageBase64 = base64Image; // Đồng bộ chuỗi Base64 đang quản lý
                                }
                                panelWhiteboard.Invalidate(); // Gọi cập nhật giao diện sạch sẽ
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error processing image: " + ex.Message);
                    }
                }
            }
        }
        private void SendMessage(string msg)
        {
            try
            {
                if (client != null && client.Connected && stream != null && stream.CanWrite)
                {
                    byte[] data = Encoding.UTF8.GetBytes(msg + "\n");
                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                }
                else
                {
                    Console.WriteLine("Cannot send - connection not available");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Send error: " + ex.Message);
            }
        }
        private void PanelWhiteboard_MouseDown(object sender, MouseEventArgs e)
        {
            if (IsNearResizeHandle(e.Location))
            {
                isResizingImage = true;
                resizeStartPos = e.Location;
            }
            else if (currentImageRect.Contains(e.Location))
            {
                isMovingImage = true;
                mouseDownPos = e.Location;
                imageMoveStartPos = new Point(currentImageRect.X, currentImageRect.Y);
            }
            else if (e.Button == MouseButtons.Left)
            {
                isDrawing = true;
                startPoint = e.Location;   // điểm bắt đầu vẽ cho các kiểu
                lastPoint = e.Location;    // dùng cho vẽ FreeHand
                currentPoint = e.Location; // cập nhật điểm hiện tại cho vẽ tạm thời
            }
        }
        private string GetPicturesFolderPath()
        {
            try
            {
                string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;

                string exeDirectory = Path.GetDirectoryName(exePath);

                DirectoryInfo dir = Directory.GetParent(exeDirectory); 
                dir = Directory.GetParent(dir.FullName); 
                dir = Directory.GetParent(dir.FullName);
                dir = Directory.GetParent(dir.FullName); 

                if (dir == null)
                    throw new Exception("Không tìm thấy thư mục gốc dự án");

                // Kết hợp với thư mục Pictures
                string picturesPath = Path.Combine(dir.FullName, "Pictures");

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(picturesPath))
                {
                    Directory.CreateDirectory(picturesPath);
                }

                return picturesPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xác định thư mục lưu ảnh: {ex.Message}\nẢnh sẽ được lưu vào thư mục ứng dụng");
                return Application.StartupPath;
            }
        }
        private void SaveWhiteboardImage()
        {
            try
            {
                string picturesPath = GetPicturesFolderPath();

                // Đảm bảo thư mục tồn tại
                if (!Directory.Exists(picturesPath))
                {
                    Directory.CreateDirectory(picturesPath);
                }

                string fileName = $"Whiteboard_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                string fullPath = Path.Combine(picturesPath, fileName);

                // Lưu ảnh
                lock (drawingBitmap)
                {
                    drawingBitmap.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);
                }

                // Hiển thị thông báo
                MessageBox.Show($"Đã lưu ảnh thành công tại:\n{fullPath}", "Thông báo",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Mở thư mục chứa ảnh
                System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{fullPath}\"");
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Lỗi: Không có quyền truy cập thư mục đích.", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu ảnh:\n{ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void PanelWhiteboard_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                if (currentMode == DrawingMode.FreeHand)
                {
                    using (Graphics g = Graphics.FromImage(drawingBitmap))
                    {
                        using (Pen pen = new Pen(currentColor, penThickness))
                        {
                            g.DrawLine(pen, lastPoint, e.Location);
                        }
                    }

                    SendDrawCommand("FreeHand", lastPoint, e.Location, currentColor, penThickness);

                    lastPoint = e.Location;
                    panelWhiteboard.Invalidate();
                }
                else
                {
                    currentPoint = e.Location;
                    panelWhiteboard.Invalidate();
                }
            }
            else if (isResizingImage)
            {
                int dx = e.X - resizeStartPos.X;
                int dy = e.Y - resizeStartPos.Y;
                currentImageRect.Width = Math.Max(20, currentImageRect.Width + dx);
                currentImageRect.Height = Math.Max(20, currentImageRect.Height + dy);
                resizeStartPos = e.Location;
                RedrawWhiteboard();
            }
            else if (isMovingImage)
            {
                int dx = e.X - mouseDownPos.X;
                int dy = e.Y - mouseDownPos.Y;
                currentImageRect.X = imageMoveStartPos.X + dx;
                currentImageRect.Y = imageMoveStartPos.Y + dy;
                RedrawWhiteboard();
            }

        }

        private void SendDrawCommand(string shape, Point start, Point end, Color color, float thickness)
        {
            string message = $"DRAW;{shape};{color.ToArgb()};{thickness};{start.X};{start.Y};{end.X};{end.Y}";
            SendMessage(message);
        }
        private void panelWhiteboard_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                isDrawing = false;

                if (currentMode != DrawingMode.FreeHand)
                {
                    using (Graphics g = Graphics.FromImage(drawingBitmap))
                    using (Pen pen = new Pen(currentColor, penThickness))
                    {
                        switch (currentMode)
                        {
                            case DrawingMode.Line:
                                g.DrawLine(pen, startPoint, e.Location);
                                break;
                            case DrawingMode.Rectangle:
                                g.DrawRectangle(pen, GetRectangleFromPoints(startPoint, e.Location));
                                break;
                            case DrawingMode.Ellipse:
                                g.DrawEllipse(pen, GetRectangleFromPoints(startPoint, e.Location));
                                break;
                        }
                    }

                    SendDrawCommand(currentMode.ToString(), startPoint, e.Location, currentColor, penThickness);
                    panelWhiteboard.Invalidate();
                }
            }

            if (isResizingImage || isMovingImage)
            {
                SendCurrentImagePosition();
            }

            isResizingImage = false;
            isMovingImage = false;
        }
        private bool IsNearResizeHandle(Point pt)
        {
            Rectangle handle = new Rectangle(
                currentImageRect.Right - resizeHandleSize,
                currentImageRect.Bottom - resizeHandleSize,
                resizeHandleSize, resizeHandleSize);
            return handle.Contains(pt);
        }

        private async void btnInsertImage_Click(object sender, EventArgs e)
        {
            string url = txtImageUrl.Text.Trim();
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("Vui lòng nhập URL ảnh");
                return;
            }

            try
            {
                Image img = await LoadImageFromUrl(url);
                if (img == null)
                {
                    MessageBox.Show("Không tải được ảnh từ URL");
                    return;
                }

                // ================== THÊM ĐOẠN CODE NÀY VÀO ĐÂY ==================
                // Nếu đã có ảnh trước đó, nướng cố định ảnh đó vào nền vẽ tĩnh trước khi nạp ảnh mới
                if (currentImage != null && currentImageRect != Rectangle.Empty)
                {
                    lock (drawingBitmap)
                    {
                        using (Graphics g = Graphics.FromImage(drawingBitmap))
                        {
                            g.DrawImage(currentImage, currentImageRect);
                        }
                    }
                }
                // ===============================================================

                Size newSize = ResizeToFit(img.Size, MaxImageWidth, MaxImageHeight);
                Bitmap resizedImage = new Bitmap(img, newSize);
                currentImage = resizedImage;
                currentImageRect = new Rectangle(50, 50, resizedImage.Width, resizedImage.Height);

                RedrawWhiteboard();
                SendCurrentImagePosition();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải ảnh: " + ex.Message);
            }
        }

        private Size ResizeToFit(Size original, int maxWidth, int maxHeight)
        {
            float ratioX = (float)maxWidth / original.Width;
            float ratioY = (float)maxHeight / original.Height;
            float ratio = Math.Min(ratioX, ratioY);
            return new Size((int)(original.Width * ratio), (int)(original.Height * ratio));
        }

        private async Task<Image> LoadImageFromUrl(string url)
        {
            using (HttpClient http = new HttpClient())
            {
                var response = await http.GetAsync(url);
                if (!response.IsSuccessStatusCode) return null;
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    return Image.FromStream(stream);
                }
            }
        }

        private void RedrawWhiteboard()
        {
            
            panelWhiteboard.Invalidate();
        }

        private void SendCurrentImagePosition()
        {
            if (currentImage == null || currentImageRect == Rectangle.Empty) return;

            try
            {
                string base64;
                using (MemoryStream ms = new MemoryStream())
                {
                    if (currentImage is Bitmap bmp)
                    {
                        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    else
                    {
                        using (Bitmap temp = new Bitmap(currentImage))
                        {
                            temp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        }
                    }
                    base64 = Convert.ToBase64String(ms.ToArray());
                }

                currentImageBase64 = base64; // THÊM DÒNG NÀY: Lưu lại dữ liệu của chính mình vừa gửi

                string msg = $"IMAGE;{currentImageRect.X};{currentImageRect.Y};{currentImageRect.Width};{currentImageRect.Height};{base64}";
                SendMessage(msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending image: " + ex.Message);
            }
        }
        private void NumericUpDownThickness_ValueChanged(object sender, EventArgs e)
        {
            penThickness = (int)numericUpDownThickness.Value;
        }

        private void btnChooseColor_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                currentColor = dlg.Color;
            }
        }

        private void buttonEnd_Click(object sender, EventArgs e)
        {
            SaveWhiteboardImage();

            try
            {
                if (client != null && client.Connected)
                {
                    SendMessage("DISCONNECT");
                    client.Close();
                }
            }
            catch { }

            Application.Exit();
        }

        private void btnIncreaseThickness_Click(object sender, EventArgs e)
        {
            if (penThickness < numericUpDownThickness.Maximum)
            {
                penThickness++;
                numericUpDownThickness.Value = penThickness;  
            }
        }

        private void btnDecreaseThickness_Click(object sender, EventArgs e)
        {
            if (penThickness > numericUpDownThickness.Minimum)
            {
                penThickness--;
                numericUpDownThickness.Value = penThickness;  
            }
        }

        private void panelWhiteboard_Paint(object sender, PaintEventArgs e)
        {
            lock (drawingBitmap)
            {
                // 1. XÓA TRẮNG GIAO DIỆN TRƯỚC
                e.Graphics.Clear(Color.White);

                // 2. VẼ HÌNH ẢNH XUỐNG DƯỚI CÙNG (LỚP NỀN)
                if (currentImage != null && currentImageRect != Rectangle.Empty)
                {
                    e.Graphics.DrawImage(currentImage, currentImageRect);

                    // Vẽ nút vuông nhỏ màu xám để kéo giãn kích thước ảnh
                    e.Graphics.FillRectangle(Brushes.Gray,
                        currentImageRect.Right - resizeHandleSize,
                        currentImageRect.Bottom - resizeHandleSize,
                        resizeHandleSize, resizeHandleSize);
                }

                // 3. VẼ TẤM NỀN NÉT VẼ ĐÈ LÊN TRÊN ẢNH (LỚP GIỮA)
                // Để nét vẽ không bị nền trắng của drawingBitmap che mất ảnh, ta vẽ không dùng Unscaled đè clear
                e.Graphics.DrawImageUnscaled(drawingBitmap, Point.Empty);

                // 4. VẼ HÌNH KHỐI TẠM THỜI KHI ĐANG KÉO CHUỘT (LỚP TRÊN CÙNG)
                if (isDrawing && currentMode != DrawingMode.FreeHand)
                {
                    using (Pen pen = new Pen(currentColor, penThickness))
                    {
                        Rectangle rect = GetRectangleFromPoints(startPoint, currentPoint);
                        switch (currentMode)
                        {
                            case DrawingMode.Line:
                                e.Graphics.DrawLine(pen, startPoint, currentPoint);
                                break;
                            case DrawingMode.Rectangle:
                                e.Graphics.DrawRectangle(pen, rect);
                                break;
                            case DrawingMode.Ellipse:
                                e.Graphics.DrawEllipse(pen, rect);
                                break;
                        }
                    }
                }
            }
        }
        private void ClientForm_Load(object sender, EventArgs e)
        {
            comboBoxDrawMode.Items.Clear();

            comboBoxDrawMode.Items.Add("FreeHand");
            comboBoxDrawMode.Items.Add("Line");
            comboBoxDrawMode.Items.Add("Rectangle");
            comboBoxDrawMode.Items.Add("Ellipse");

            comboBoxDrawMode.SelectedIndex = 0;

            panelWhiteboard.Paint += panelWhiteboard_Paint;
        }

        private void ComboBoxDrawMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedMode = comboBoxDrawMode.SelectedItem.ToString();

            switch (selectedMode)
            {
                case "FreeHand":
                    currentMode = DrawingMode.FreeHand;
                    break;
                case "Line":
                    currentMode = DrawingMode.Line;
                    break;
                case "Rectangle":
                    currentMode = DrawingMode.Rectangle;
                    break;
                case "Ellipse":
                    currentMode = DrawingMode.Ellipse;
                    break;
                default:
                    currentMode = DrawingMode.FreeHand;
                    break;
            }
        }

        private void chkEraser_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEraser.Checked)
            {
                previousColor = currentColor;  
                currentColor = Color.White;   
                currentMode = DrawingMode.FreeHand; 
            }
            else
            {
                currentColor = previousColor; 
            }   
        }
    }
}
