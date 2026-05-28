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

        private TcpClient client;
        private NetworkStream stream;
        private bool isDisconnecting = false;

        private Point startPoint;
        private Point lastPoint;
        private Point currentPoint;
        private bool isDrawing = false;
        private DrawingMode currentMode;

        private Bitmap drawingBitmap;
        private Graphics graphics;

        private Color previousColor = Color.Black;

        private Color currentColor = Color.Black;
        private int penThickness = 2;

        private class ImageItem
        {
            public string Id { get; set; }
            public Image Image { get; set; }
            public Rectangle Rect { get; set; }
            public string Base64 { get; set; }
        }
        private List<ImageItem> imageItems = new List<ImageItem>();
        private int selectedImageIndex = -1;
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
            try
            {
                InitializeComponent();

                drawingBitmap = new Bitmap(panelWhiteboard.Width, panelWhiteboard.Height);
                graphics = Graphics.FromImage(drawingBitmap);
                graphics.Clear(Color.White);

                panelWhiteboard.BackgroundImageLayout = ImageLayout.None;

                ConnectToServer();
                numericUpDownThickness.Minimum = 1;
                numericUpDownThickness.Maximum = 10;
                numericUpDownThickness.Value = 2;
                numericUpDownThickness.ValueChanged += NumericUpDownThickness_ValueChanged;

                panelWhiteboard.MouseDown += PanelWhiteboard_MouseDown;
                panelWhiteboard.MouseMove += PanelWhiteboard_MouseMove;
                panelWhiteboard.MouseUp += panelWhiteboard_MouseUp;

                btnInsertImage.Click += btnInsertImage_Click;
                btnChooseColor.Click += btnChooseColor_Click;

                comboBoxDrawMode.Items.Add("Freehand");
                comboBoxDrawMode.Items.Add("Rectangle");
                comboBoxDrawMode.Items.Add("Ellipse");
                comboBoxDrawMode.Items.Add("Line");
                comboBoxDrawMode.SelectedIndex = 0;
                comboBoxDrawMode.SelectedIndexChanged += ComboBoxDrawMode_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khởi tạo form: " + ex.Message, "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
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
            catch (SocketException ex)
            {
                MessageBox.Show("Không thể kết nối đến server: " + ex.Message, "Lỗi kết nối",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message, "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
        }
        private void ListenData()
        {
            byte[] buffer = new byte[4096];
            StringBuilder sb = new StringBuilder();

            while (client != null && client.Connected && !isDisconnecting)
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
                catch (ObjectDisposedException)
                {
                    break;
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

            if (!isDisconnecting)
            {
                try
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show("Mất kết nối đến server.", "Thông báo",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Application.Exit();
                    });
                }
                catch (ObjectDisposedException) { }
                catch (InvalidOperationException) { }
            }
        }
        private void ProcessDrawingMessage(string msg)
        {
            try
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

                        if (x1 < 0 || y1 < 0 || x2 < 0 || y2 < 0 ||
                            x1 > 5000 || y1 > 5000 || x2 > 5000 || y2 > 5000)
                            return;

                        Point p1 = new Point(x1, y1);
                        Point p2 = new Point(x2, y2);

                        this.Invoke((MethodInvoker)delegate
                        {
                            try
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
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error drawing on bitmap: " + ex.Message);
                            }
                        });
                    }
                }
                else if (msg.StartsWith("IMAGE"))
                {
                    string[] parts = msg.Split(';');
                    if (parts.Length < 7) return;

                    string id = parts[1];
                    if (string.IsNullOrEmpty(id)) return;

                    if (int.TryParse(parts[2], out int x) &&
                        int.TryParse(parts[3], out int y) &&
                        int.TryParse(parts[4], out int w) &&
                        int.TryParse(parts[5], out int h))
                    {
                        if (w <= 0 || h <= 0 || w > 2000 || h > 2000) return;

                        string base64Image = parts[6];
                        if (string.IsNullOrEmpty(base64Image)) return;

                        try
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                try
                                {
                                    ImageItem existing = null;
                                    foreach (var item in imageItems)
                                    {
                                        if (item.Id == id) { existing = item; break; }
                                    }

                                    if (existing != null)
                                    {
                                        existing.Rect = new Rectangle(x, y, w, h);
                                    }
                                    else
                                    {
                                        byte[] imgBytes = Convert.FromBase64String(base64Image);
                                        using (MemoryStream ms = new MemoryStream(imgBytes))
                                        {
                                            Image img = Image.FromStream(ms);
                                            var newItem = new ImageItem
                                            {
                                                Id = id,
                                                Image = img,
                                                Rect = new Rectangle(x, y, w, h),
                                                Base64 = base64Image
                                            };
                                            imageItems.Add(newItem);
                                        }
                                    }
                                    panelWhiteboard.Invalidate();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Error processing image on UI thread: " + ex.Message);
                                }
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error processing image: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ProcessDrawingMessage error: " + ex.Message);
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
            catch (ObjectDisposedException)
            {
                Console.WriteLine("Cannot send - connection disposed");
            }
            catch (IOException ex)
            {
                Console.WriteLine("Send IO error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Send error: " + ex.Message);
            }
        }
        private void PanelWhiteboard_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e == null) return;

                selectedImageIndex = -1;
                for (int i = imageItems.Count - 1; i >= 0; i--)
                {
                    if (imageItems[i].Rect.Contains(e.Location))
                    {
                        selectedImageIndex = i;
                        break;
                    }
                }

                if (selectedImageIndex >= 0 && IsNearResizeHandle(imageItems[selectedImageIndex].Rect, e.Location))
                {
                    isResizingImage = true;
                    resizeStartPos = e.Location;
                }
                else if (selectedImageIndex >= 0)
                {
                    isMovingImage = true;
                    mouseDownPos = e.Location;
                    imageMoveStartPos = new Point(imageItems[selectedImageIndex].Rect.X, imageItems[selectedImageIndex].Rect.Y);
                }
                else if (e.Button == MouseButtons.Left)
                {
                    isDrawing = true;
                    startPoint = e.Location;
                    lastPoint = e.Location;
                    currentPoint = e.Location;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MouseDown error: " + ex.Message);
                isMovingImage = false;
                isResizingImage = false;
                selectedImageIndex = -1;
            }
        }
        private string GetPicturesFolderPath()
        {
            try
            {
                string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;

                string exeDirectory = Path.GetDirectoryName(exePath);
                if (exeDirectory == null)
                    throw new Exception("Không xác định được thư mục chứa ứng dụng");

                DirectoryInfo dir = Directory.GetParent(exeDirectory); 
                if (dir == null) throw new Exception("Không tìm thấy thư mục gốc dự án");
                dir = Directory.GetParent(dir.FullName); 
                if (dir == null) throw new Exception("Không tìm thấy thư mục gốc dự án");
                dir = Directory.GetParent(dir.FullName);
                if (dir == null) throw new Exception("Không tìm thấy thư mục gốc dự án");
                dir = Directory.GetParent(dir.FullName); 

                if (dir == null)
                    throw new Exception("Không tìm thấy thư mục gốc dự án");

                string picturesPath = Path.Combine(dir.FullName, "Pictures");

                if (!Directory.Exists(picturesPath))
                {
                    Directory.CreateDirectory(picturesPath);
                }

                return picturesPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetPicturesFolderPath error: " + ex.Message);
                MessageBox.Show("Lỗi khi xác định thư mục lưu ảnh: " + ex.Message +
                                "\nẢnh sẽ được lưu vào thư mục ứng dụng", "Cảnh báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return Application.StartupPath;
            }
        }
        private void SaveWhiteboardImage()
        {
            try
            {
                string picturesPath = GetPicturesFolderPath();

                if (!Directory.Exists(picturesPath))
                {
                    Directory.CreateDirectory(picturesPath);
                }

                string fileName = $"Whiteboard_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                string fullPath = Path.Combine(picturesPath, fileName);

                int w = panelWhiteboard.Width;
                int h = panelWhiteboard.Height;
                if (w <= 0 || h <= 0)
                {
                    MessageBox.Show("Kích thước bảng vẽ không hợp lệ.", "Lỗi",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (Bitmap saveBitmap = new Bitmap(w, h))
                using (Graphics g = Graphics.FromImage(saveBitmap))
                {
                    g.Clear(Color.White);

                    foreach (var item in imageItems)
                    {
                        if (item != null && item.Image != null)
                        {
                            g.DrawImage(item.Image, item.Rect);
                        }
                    }

                    lock (drawingBitmap)
                    {
                        if (drawingBitmap == null)
                        {
                            MessageBox.Show("Không có dữ liệu để lưu.", "Thông báo",
                                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        using (var attr = new System.Drawing.Imaging.ImageAttributes())
                        {
                            attr.SetColorKey(Color.White, Color.White);
                            Rectangle fullRect = new Rectangle(0, 0, drawingBitmap.Width, drawingBitmap.Height);
                            g.DrawImage(drawingBitmap, fullRect, 0, 0, drawingBitmap.Width, drawingBitmap.Height, GraphicsUnit.Pixel, attr);
                        }
                    }

                    saveBitmap.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);
                }

                MessageBox.Show("Đã lưu ảnh thành công tại:\n" + fullPath, "Thông báo",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);

                try
                {
                    System.Diagnostics.Process.Start("explorer.exe", "/select,\"" + fullPath + "\"");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Không thể mở explorer: " + ex.Message);
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Lỗi: Không có quyền truy cập thư mục đích.", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show("Lỗi: Không tìm thấy thư mục.\n" + ex.Message, "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException ex)
            {
                MessageBox.Show("Lỗi khi ghi file: " + ex.Message, "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu ảnh:\n" + ex.Message, "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void PanelWhiteboard_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e == null) return;

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
                else if (isResizingImage && selectedImageIndex >= 0 && selectedImageIndex < imageItems.Count)
                {
                    int dx = e.X - resizeStartPos.X;
                    int dy = e.Y - resizeStartPos.Y;
                    var rect = imageItems[selectedImageIndex].Rect;
                    rect.Width = Math.Max(20, rect.Width + dx);
                    rect.Height = Math.Max(20, rect.Height + dy);
                    imageItems[selectedImageIndex].Rect = rect;
                    resizeStartPos = e.Location;
                    RedrawWhiteboard();
                }
                else if (isMovingImage && selectedImageIndex >= 0 && selectedImageIndex < imageItems.Count)
                {
                    int dx = e.X - mouseDownPos.X;
                    int dy = e.Y - mouseDownPos.Y;
                    var rect = imageItems[selectedImageIndex].Rect;
                    rect.X = imageMoveStartPos.X + dx;
                    rect.Y = imageMoveStartPos.Y + dy;
                    imageItems[selectedImageIndex].Rect = rect;
                    RedrawWhiteboard();
                }
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine("MouseMove null reference: " + ex.Message);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine("MouseMove out of range: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("MouseMove error: " + ex.Message);
            }
        }

        private void SendDrawCommand(string shape, Point start, Point end, Color color, float thickness)
        {
            try
            {
                string message = string.Format("DRAW;{0};{1};{2};{3};{4};{5};{6}",
                    shape, color.ToArgb(), thickness,
                    start.X, start.Y, end.X, end.Y);
                SendMessage(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SendDrawCommand error: " + ex.Message);
            }
        }
        private void panelWhiteboard_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (e == null) return;

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

                if ((isResizingImage || isMovingImage) && selectedImageIndex >= 0 && selectedImageIndex < imageItems.Count)
                {
                    var item = imageItems[selectedImageIndex];
                    string msg = string.Format("IMAGE;{0};{1};{2};{3};{4};{5}",
                        item.Id, item.Rect.X, item.Rect.Y, item.Rect.Width, item.Rect.Height, item.Base64);
                    SendMessage(msg);
                }

                isResizingImage = false;
                isMovingImage = false;
                selectedImageIndex = -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine("MouseUp error: " + ex.Message);
                isResizingImage = false;
                isMovingImage = false;
                selectedImageIndex = -1;
            }
        }
        private bool IsNearResizeHandle(Rectangle rect, Point pt)
        {
            try
            {
                Rectangle handle = new Rectangle(
                    rect.Right - resizeHandleSize,
                    rect.Bottom - resizeHandleSize,
                    resizeHandleSize, resizeHandleSize);
                return handle.Contains(pt);
            }
            catch (Exception ex)
            {
                Console.WriteLine("IsNearResizeHandle error: " + ex.Message);
                return false;
            }
        }

        private async void btnInsertImage_Click(object sender, EventArgs e)
        {
            try
            {
                string url = txtImageUrl.Text.Trim();
                if (string.IsNullOrEmpty(url))
                {
                    MessageBox.Show("Vui lòng nhập URL ảnh", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri) ||
                    (uri.Scheme != "http" && uri.Scheme != "https"))
                {
                    MessageBox.Show("URL không hợp lệ. Vui lòng nhập URL http hoặc https.", "Lỗi",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                btnInsertImage.Enabled = false;
                btnInsertImage.Text = "Đang tải...";

                Image img = await LoadImageFromUrl(url);
                if (img == null)
                {
                    MessageBox.Show("Không tải được ảnh từ URL", "Lỗi",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnInsertImage.Enabled = true;
                    btnInsertImage.Text = "INSERT";
                    return;
                }

                Size newSize = ResizeToFit(img.Size, MaxImageWidth, MaxImageHeight);
                Bitmap resizedImage = new Bitmap(img, newSize);

                string base64;
                using (MemoryStream ms = new MemoryStream())
                {
                    resizedImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    base64 = Convert.ToBase64String(ms.ToArray());
                }

                var item = new ImageItem
                {
                    Id = Guid.NewGuid().ToString(),
                    Image = resizedImage,
                    Rect = new Rectangle(50, 50, resizedImage.Width, resizedImage.Height),
                    Base64 = base64
                };

                imageItems.Add(item);

                RedrawWhiteboard();
                SendMessage(string.Format("IMAGE;{0};{1};{2};{3};{4};{5}",
                    item.Id, item.Rect.X, item.Rect.Y, item.Rect.Width, item.Rect.Height, item.Base64));

                btnInsertImage.Enabled = true;
                btnInsertImage.Text = "INSERT";
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Lỗi mạng khi tải ảnh: " + ex.Message, "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnInsertImage.Enabled = true;
                btnInsertImage.Text = "INSERT";
            }
            catch (TaskCanceledException)
            {
                MessageBox.Show("Quá thời gian chờ tải ảnh.", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnInsertImage.Enabled = true;
                btnInsertImage.Text = "INSERT";
            }
            catch (OutOfMemoryException)
            {
                MessageBox.Show("Ảnh quá lớn, không thể xử lý.", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnInsertImage.Enabled = true;
                btnInsertImage.Text = "INSERT";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải ảnh: " + ex.Message, "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnInsertImage.Enabled = true;
                btnInsertImage.Text = "INSERT";
            }
        }

        private Size ResizeToFit(Size original, int maxWidth, int maxHeight)
        {
            try
            {
                if (original.Width <= 0 || original.Height <= 0)
                    return new Size(maxWidth, maxHeight);

                if (original.Width <= maxWidth && original.Height <= maxHeight)
                    return original;

                float ratioX = (float)maxWidth / original.Width;
                float ratioY = (float)maxHeight / original.Height;
                float ratio = Math.Min(ratioX, ratioY);
                return new Size((int)(original.Width * ratio), (int)(original.Height * ratio));
            }
            catch (Exception ex)
            {
                Console.WriteLine("ResizeToFit error: " + ex.Message);
                return new Size(maxWidth, maxHeight);
            }
        }

        private async Task<Image> LoadImageFromUrl(string url)
        {
            HttpClient http = null;
            try
            {
                http = new HttpClient();
                http.Timeout = TimeSpan.FromSeconds(15);

                var response = await http.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("HTTP error: " + response.StatusCode);
                    return null;
                }

                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    return Image.FromStream(stream);
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("HTTP request error: " + ex.Message);
                return null;
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Request timed out");
                return null;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Invalid image data: " + ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("LoadImage error: " + ex.Message);
                return null;
            }
            finally
            {
                if (http != null)
                    http.Dispose();
            }
        }

        private void RedrawWhiteboard()
        {
            try
            {
                if (panelWhiteboard.IsHandleCreated && !panelWhiteboard.IsDisposed)
                {
                    panelWhiteboard.Invalidate();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("RedrawWhiteboard error: " + ex.Message);
            }
        }

        private void NumericUpDownThickness_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                penThickness = (int)numericUpDownThickness.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Thickness change error: " + ex.Message);
            }
        }

        private void btnChooseColor_Click(object sender, EventArgs e)
        {
            try
            {
                using (ColorDialog dlg = new ColorDialog())
                {
                    dlg.Color = currentColor;
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        currentColor = dlg.Color;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Color dialog error: " + ex.Message);
            }
        }

        private void buttonEnd_Click(object sender, EventArgs e)
        {
            try
            {
                isDisconnecting = true;
                SaveWhiteboardImage();

                if (client != null && client.Connected)
                {
                    try
                    {
                        SendMessage("DISCONNECT");
                    }
                    catch { }
                    try
                    {
                        client.Close();
                    }
                    catch { }
                }

                if (stream != null)
                {
                    try { stream.Close(); }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("End error: " + ex.Message);
            }
            finally
            {
                Application.Exit();
            }
        }

        private void btnIncreaseThickness_Click(object sender, EventArgs e)
        {
            try
            {
                if (penThickness < numericUpDownThickness.Maximum)
                {
                    penThickness++;
                    numericUpDownThickness.Value = penThickness;  
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Increase thickness error: " + ex.Message);
            }
        }

        private void btnDecreaseThickness_Click(object sender, EventArgs e)
        {
            try
            {
                if (penThickness > numericUpDownThickness.Minimum)
                {
                    penThickness--;
                    numericUpDownThickness.Value = penThickness;  
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Decrease thickness error: " + ex.Message);
            }
        }

        private void panelWhiteboard_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (e == null || e.Graphics == null) return;

                lock (drawingBitmap)
                {
                    e.Graphics.Clear(Color.White);

                    foreach (var item in imageItems)
                    {
                        try
                        {
                            if (item != null && item.Image != null)
                            {
                                e.Graphics.DrawImage(item.Image, item.Rect);

                                e.Graphics.FillRectangle(Brushes.Gray,
                                    item.Rect.Right - resizeHandleSize,
                                    item.Rect.Bottom - resizeHandleSize,
                                    resizeHandleSize, resizeHandleSize);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Paint image item error: " + ex.Message);
                        }
                    }

                    try
                    {
                        using (var attr = new System.Drawing.Imaging.ImageAttributes())
                        {
                            attr.SetColorKey(Color.White, Color.White);
                            Rectangle fullRect = new Rectangle(0, 0, drawingBitmap.Width, drawingBitmap.Height);
                            e.Graphics.DrawImage(drawingBitmap, fullRect, 0, 0, drawingBitmap.Width, drawingBitmap.Height, GraphicsUnit.Pixel, attr);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Paint bitmap error: " + ex.Message);
                    }

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
            catch (Exception ex)
            {
                Console.WriteLine("Paint error: " + ex.Message);
            }
        }
        private void ClientForm_Load(object sender, EventArgs e)
        {
            try
            {
                comboBoxDrawMode.Items.Clear();

                comboBoxDrawMode.Items.Add("FreeHand");
                comboBoxDrawMode.Items.Add("Line");
                comboBoxDrawMode.Items.Add("Rectangle");
                comboBoxDrawMode.Items.Add("Ellipse");

                comboBoxDrawMode.SelectedIndex = 0;

                panelWhiteboard.Paint += panelWhiteboard_Paint;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Form Load error: " + ex.Message);
            }
        }

        private void ComboBoxDrawMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBoxDrawMode.SelectedItem == null) return;

                string selectedMode = comboBoxDrawMode.SelectedItem.ToString();
                if (string.IsNullOrEmpty(selectedMode)) return;

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
            catch (Exception ex)
            {
                Console.WriteLine("ComboBox change error: " + ex.Message);
                currentMode = DrawingMode.FreeHand;
            }
        }

        private void chkEraser_CheckedChanged(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine("Eraser error: " + ex.Message);
            }
        }
    }
}
