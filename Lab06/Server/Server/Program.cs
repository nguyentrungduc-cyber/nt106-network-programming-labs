using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Threading;


class Server
{
    private TcpListener listener;
    private List<TcpClient> clients = new List<TcpClient>();
    private int port = 9000;
    private readonly object _lock = new object();
    private const int maxClients = 5;
    private volatile bool isRunning = true;

    private List<string> whiteboardHistory = new List<string>();

    public void Start()
    {
        try
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine("Server started on port " + port);
            ListenForClients();
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
        {
            Console.WriteLine("Lỗi: Cổng " + port + " đã được sử dụng. Vui lòng chọn cổng khác.");
            Console.WriteLine("Nhấn Enter để thoát...");
            Console.ReadLine();
        }
        catch (SocketException ex)
        {
            Console.WriteLine("Lỗi socket khi khởi động server: " + ex.Message);
            Console.WriteLine("Nhấn Enter để thoát...");
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi khởi động server: " + ex.Message);
            Console.WriteLine("Nhấn Enter để thoát...");
            Console.ReadLine();
        }
    }

    private void ListenForClients()
    {
        while (isRunning)
        {
            try
            {
                TcpClient client = listener.AcceptTcpClient();
                if (!isRunning) break;

                lock (_lock)
                {
                    if (clients.Count >= maxClients)
                    {
                        Console.WriteLine("Client rejected - max clients reached");
                        try { client.Close(); } catch { }
                        continue;
                    }

                    clients.Add(client);
                    Console.WriteLine("Client connected. Total clients: " + clients.Count);

                    if (clients.Count == maxClients)
                    {
                        Thread emailThread = new Thread(SendAlertEmail);
                        emailThread.IsBackground = true;
                        emailThread.Start();
                    }

                    SendHistoryToClient(client);
                }

                Thread clientThread = new Thread(HandleClient);
                clientThread.IsBackground = true;
                clientThread.Start(client);
            }
            catch (ObjectDisposedException)
            {
                break;
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Socket error accepting client: " + ex.Message);
                if (!isRunning) break;
                Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error accepting client: " + ex.Message);
                if (!isRunning) break;
                Thread.Sleep(100);
            }
        }
    }

    private void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        NetworkStream stream = null;
        byte[] buffer = new byte[8192];
        StringBuilder sb = new StringBuilder();

        try
        {
            stream = client.GetStream();
            while (isRunning)
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
                        string message = allData.Substring(0, newlineIndex).Trim();
                        if (!string.IsNullOrEmpty(message))
                        {
                            Console.WriteLine("Received: " + message);

                            lock (_lock)
                            {
                                if (message.StartsWith("DRAW") || message.StartsWith("IMAGE"))
                                {
                                    whiteboardHistory.Add(message);
                                }
                                else if (message.StartsWith("DELETE"))
                                {
                                    try
                                    {
                                        string[] parts = message.Split(';');
                                        if (parts.Length >= 2)
                                        {
                                            string target = parts[1];
                                            whiteboardHistory.RemoveAll(msg => msg.Contains(target));
                                            Console.WriteLine("Deleted from history: " + target);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("Delete history error: " + ex.Message);
                                    }
                                }
                            }

                            Broadcast(message, client);
                        }

                        allData = allData.Substring(newlineIndex + 1);
                    }
                    sb.Clear();
                    sb.Append(allData);
                }
                Thread.Sleep(10);
            }
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine("Client error (InvalidOperation): " + ex.Message);
        }
        catch (IOException ex)
        {
            Console.WriteLine("Client disconnected (IO): " + ex.Message);
        }
        catch (SocketException ex)
        {
            Console.WriteLine("Client disconnected (Socket): " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Client error: " + ex.Message);
        }
        finally
        {
            lock (_lock)
            {
                if (clients.Contains(client))
                {
                    clients.Remove(client);
                    Console.WriteLine("Client disconnected. Total clients: " + clients.Count);
                }
            }
            if (stream != null) try { stream.Close(); } catch { }
            try { client.Close(); } catch { }
        }
    }

    private void Broadcast(string message, TcpClient excludeClient)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message + "\n");
            List<TcpClient> disconnectedClients = new List<TcpClient>();

            lock (_lock)
            {
                foreach (var c in clients)
                {
                    if (c != excludeClient)
                    {
                        try
                        {
                            NetworkStream stream = c.GetStream();
                            if (stream.CanWrite)
                            {
                                stream.Write(data, 0, data.Length);
                                stream.Flush();
                            }
                        }
                        catch (ObjectDisposedException)
                        {
                            disconnectedClients.Add(c);
                        }
                        catch (IOException)
                        {
                            disconnectedClients.Add(c);
                        }
                        catch (SocketException)
                        {
                            disconnectedClients.Add(c);
                        }
                        catch (Exception)
                        {
                            disconnectedClients.Add(c);
                        }
                    }
                }

                foreach (var dc in disconnectedClients)
                {
                    clients.Remove(dc);
                    try { dc.Close(); } catch { }
                    Console.WriteLine("Client disconnected. Total clients: " + clients.Count);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Broadcast error: " + ex.Message);
        }
    }

    private void SendHistoryToClient(TcpClient client)
    {
        try
        {
            NetworkStream stream = client.GetStream();
            foreach (string msg in whiteboardHistory)
            {
                byte[] data = Encoding.UTF8.GetBytes(msg + "\n");
                stream.Write(data, 0, data.Length);
                stream.Flush();
                Thread.Sleep(1);
            }
            Console.WriteLine("Sent whiteboard history to new client.");
        }
        catch (IOException ex)
        {
            Console.WriteLine("Lỗi gửi lịch sử (IO): " + ex.Message);
        }
        catch (SocketException ex)
        {
            Console.WriteLine("Lỗi gửi lịch sử (Socket): " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi khi gửi lịch sử cho client mới: " + ex.Message);
        }
    }

    private void SendAlertEmail()
    {
        try
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("nguyentrungduc190906@gmail.com");
                mail.To.Add("nguyentrungduc190906@gmail.com");
                mail.Subject = "Cảnh báo: Đã có đủ 5 Client kết nối";
                mail.Body = "Hiện tại Server đã có 5 Client đang hoạt động.";

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("nguyentrungduc190906@gmail.com", "hhxu vtbr iwdm hrmo");
                    smtp.EnableSsl = true;

                    System.Net.ServicePointManager.ServerCertificateValidationCallback =
                        (sender, certificate, chain, sslPolicyErrors) => true;

                    smtp.Send(mail);
                    Console.WriteLine("Alert message was sent to the admin successfully.");
                }
            }
        }
        catch (SmtpException ex)
        {
            Console.WriteLine("Lỗi SMTP khi gửi email: " + ex.Message);
            if (ex.InnerException != null)
                Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi khi gửi email: " + ex.Message);
            if (ex.InnerException != null)
                Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
        }
    }



    static void Main()
    {
        Server s = new Server();
        s.Start();
    }
}
