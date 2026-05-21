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
using System.Text.RegularExpressions;

namespace Lab05
{
    public partial class Bai02 : Form
    {
        public Bai02()
        {
            InitializeComponent();

            // 1. Xóa toàn bộ cột cũ (nếu có lỡ tạo trong giao diện Designer) để đảm bảo chỉ có đúng 3 cột
            lsvMails.Columns.Clear();

            // 2. Setup đúng 3 cột cho ListView
            lsvMails.Columns.Add("Email", 250);     // Thực chất là hiển thị Subject
            lsvMails.Columns.Add("From", 150);
            lsvMails.Columns.Add("Thời gian", 150); // Cột Date

            lsvMails.View = View.Details;
            lsvMails.GridLines = true;
            lsvMails.FullRowSelect = true;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            string server = "127.0.0.1"; // Chạy localhost
            int port = 143;

            try
            {
                TcpClient client = new TcpClient(server, port);
                NetworkStream stream = client.GetStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                // Đọc lời chào
                string response = reader.ReadLine();

                // Gửi lệnh LOGIN
                writer.WriteLine($"A01 LOGIN {email} {password}");
                response = ReadUntil(reader, "A01");

                if (!response.Contains("OK"))
                {
                    MessageBox.Show("Đăng nhập thất bại. Kiểm tra lại thông tin!");
                    client.Close();
                    return;
                }

                // Chọn hộp thư INBOX
                writer.WriteLine("A02 SELECT INBOX");
                response = ReadUntil(reader, "A02");

                Match matchTotal = Regex.Match(response, @"\* (\d+) EXISTS");
                int totalMails = 0;

                if (matchTotal.Success)
                {
                    totalMails = int.Parse(matchTotal.Groups[1].Value);
                    lblTotalCount.Text = totalMails.ToString();
                }

                lsvMails.Items.Clear();

                // Lấy Header (FETCH)
                if (totalMails > 0)
                {
                    writer.WriteLine($"A03 FETCH 1:{totalMails} (BODY.PEEK[HEADER.FIELDS (SUBJECT FROM DATE)])");
                    string fetchResponse = ReadUntil(reader, "A03");

                    // Cắt các email dựa trên định dạng trả về của lệnh FETCH
                    string[] rawEmails = Regex.Split(fetchResponse, @"\* \d+ FETCH");

                    int todayMailsCount = 0;

                    foreach (string rawData in rawEmails)
                    {
                        // Bỏ qua các mẩu chuỗi rác hoặc chuỗi kết thúc A03 OK
                        if (string.IsNullOrWhiteSpace(rawData) || rawData.Trim().StartsWith("A03")) continue;

                        // Logic mới: Bóc tách chính xác từng dòng Header và Giải mã UTF-8
                        string subject = DecodeMimeWords(ExtractHeader(rawData, "Subject"));
                        string from = DecodeMimeWords(ExtractHeader(rawData, "From"));
                        string date = ExtractHeader(rawData, "Date"); // Date không cần giải mã

                        // Nếu không lấy được cả Subject và From thì khả năng cao đây là chuỗi rác, bỏ qua
                        if (string.IsNullOrEmpty(subject) && string.IsNullOrEmpty(from)) continue;

                        // Đếm mail của ngày hôm nay
                        if (IsMailFromToday(date))
                        {
                            todayMailsCount++;
                        }

                        // Add dữ liệu vào ListView (Đúng 3 cột)
                        ListViewItem item = new ListViewItem(subject); // Cột 1: Email (Subject)
                        item.SubItems.Add(from);                       // Cột 2: From
                        item.SubItems.Add(date);                       // Cột 3: Thời gian
                        lsvMails.Items.Add(item);
                    }

                    // Gán tổng số mail mới trong ngày cho Recent
                    lblRecentCount.Text = todayMailsCount.ToString();
                }
                else
                {
                    lblRecentCount.Text = "0";
                }

                // Đăng xuất
                writer.WriteLine("A04 LOGOUT");
                client.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private string ReadUntil(StreamReader reader, string tag)
        {
            string result = "";
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                result += line + "\r\n";
                if (line.StartsWith(tag)) break;
            }
            return result;
        }

        // Logic bóc tách Header
        private string ExtractHeader(string rawData, string headerName)
        {
            // Dùng (?im) để quét dòng bắt đầu bằng TênHeader:, bỏ qua phân biệt hoa thường và lấy phần nội dung phía sau.
            Match match = Regex.Match(rawData, $@"(?im)^{headerName}:\s*(.*?)\r?$", RegexOptions.Multiline);

            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }

            // Trả về chuỗi rỗng để ListView không bị hiển thị các ô trắng lỗi
            return "";
        }

        // Hàm giải mã MIME Encoded-Word (UTF-8)
        private string DecodeMimeWords(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            // Dùng Regex tìm chuỗi mã hóa có định dạng: =?charset?encoding?data?=
            return Regex.Replace(input, @"=\?(.*?)\?(B|Q|b|q)\?(.*?)\?=", match =>
            {
                string charset = match.Groups[1].Value;
                string encoding = match.Groups[2].Value.ToUpper();
                string data = match.Groups[3].Value;

                try
                {
                    Encoding enc = Encoding.GetEncoding(charset);

                    // Giải mã Base64
                    if (encoding == "B")
                    {
                        byte[] bytes = Convert.FromBase64String(data);
                        return enc.GetString(bytes);
                    }
                    // Giải mã Quoted-Printable
                    else if (encoding == "Q")
                    {
                        data = data.Replace("_", " ");
                        var bytes = new List<byte>();
                        for (int i = 0; i < data.Length; i++)
                        {
                            if (data[i] == '=' && i + 2 < data.Length)
                            {
                                try
                                {
                                    bytes.Add(Convert.ToByte(data.Substring(i + 1, 2), 16));
                                    i += 2;
                                }
                                catch { bytes.Add((byte)data[i]); }
                            }
                            else
                            {
                                bytes.Add((byte)data[i]);
                            }
                        }
                        return enc.GetString(bytes.ToArray());
                    }
                }
                catch
                {
                    // Nếu có lỗi parse thì bỏ qua, giữ nguyên chuỗi gốc
                }

                return match.Value;
            });
        }

        private bool IsMailFromToday(string dateHeader)
        {
            if (string.IsNullOrWhiteSpace(dateHeader)) return false;

            // Lọc bỏ múi giờ phụ bằng chữ ở đuôi (vd: "(ICT)") để hàm Parse hoạt động tốt hơn
            string cleanDate = Regex.Replace(dateHeader, @"\s*\([^)]*\)$", "").Trim();

            if (DateTime.TryParse(cleanDate, out DateTime parsedDate))
            {
                return parsedDate.Date == DateTime.Today;
            }

            // Fallback nếu TryParse thất bại
            string todayStr = DateTime.Today.ToString("dd MMM yyyy", new System.Globalization.CultureInfo("en-US"));
            string todayStrShort = DateTime.Today.ToString("d MMM yyyy", new System.Globalization.CultureInfo("en-US"));

            return dateHeader.Contains(todayStr) || dateHeader.Contains(todayStrShort);
        }
    }
}