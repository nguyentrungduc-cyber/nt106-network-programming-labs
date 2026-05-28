using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Threading;

namespace WhiteboardServer
{
    public class Server
    {
        private TcpListener listener;
        private List<TcpClient> clients = new List<TcpClient>();
        private List<DrawData> drawHistory = new List<DrawData>();
        private List<ImageData> imageHistory = new List<ImageData>();
        private const int MAX_CLIENTS = 5;
        private bool emailSent = false;
        private object lockObj = new object();
        private bool isRunning = false;

        public void Start()
        {
            try
            {
                listener = new TcpListener(IPAddress.Any, 8888);
                listener.Start();
                isRunning = true;

                Log("========================================", ConsoleColor.Cyan);
                Log("  Whiteboard Server v2.0", ConsoleColor.Cyan);
                Log("========================================", ConsoleColor.Cyan);
                Log("");
                Log($"[{DateTime.Now:HH:mm:ss}] Server started on port 8888", ConsoleColor.Green);
                Log($"[{DateTime.Now:HH:mm:ss}] Max clients: {MAX_CLIENTS}", ConsoleColor.Gray);
                Log($"[{DateTime.Now:HH:mm:ss}] Waiting for connections...", ConsoleColor.Yellow);
                Log("");

                Thread acceptThread = new Thread(AcceptClients);
                acceptThread.IsBackground = true;
                acceptThread.Start();
            }
            catch (Exception ex)
            {
                Log($"[ERROR] Failed to start server: {ex.Message}", ConsoleColor.Red);
            }
        }

        public void Stop()
        {
            isRunning = false;

            Log("", ConsoleColor.Gray);
            Log("========================================", ConsoleColor.Yellow);
            Log("  Server shutting down...", ConsoleColor.Yellow);
            Log("========================================", ConsoleColor.Yellow);

            try { listener?.Stop(); }
            catch { }

            lock (lockObj)
            {
                foreach (TcpClient c in clients)
                {
                    try { c.Close(); }
                    catch { }
                }
                clients.Clear();
            }

            Log($"[{DateTime.Now:HH:mm:ss}] Server stopped", ConsoleColor.Red);
        }

        private void AcceptClients()
        {
            while (isRunning)
            {
                try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    string clientEndpoint = client.Client.RemoteEndPoint.ToString();

                    lock (lockObj)
                    {
                        clients.Add(client);
                        Log($"[{DateTime.Now:HH:mm:ss}] Client connected: {clientEndpoint} (Total: {clients.Count})", ConsoleColor.Green);

                        if (clients.Count >= MAX_CLIENTS && !emailSent)
                        {
                            SendEmailAlert();
                            emailSent = true;
                        }
                    }

                    BroadcastClientCount();
                    SendSyncData(client);

                    Thread clientThread = new Thread(() => HandleClient(client, clientEndpoint));
                    clientThread.IsBackground = true;
                    clientThread.Start();
                }
                catch (ObjectDisposedException) { break; }
                catch (Exception ex)
                {
                    if (isRunning)
                        Log($"[ERROR] Accept: {ex.Message}", ConsoleColor.Red);
                }
            }
        }

        private void HandleClient(TcpClient client, string endpoint)
        {
            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();
                byte[] typeBuffer = new byte[4];

                while (client.Connected && isRunning)
                {
                    if (ReadFully(stream, typeBuffer, 0, 4) == 0) break;

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

                    ProcessMessage(msgType, data, client, endpoint);
                }
            }
            catch (Exception ex)
            {
                Log($"[{DateTime.Now:HH:mm:ss}] Client lost: {endpoint} - {ex.Message}", ConsoleColor.Yellow);
            }
            finally
            {
                lock (lockObj)
                {
                    clients.Remove(client);
                    Log($"[{DateTime.Now:HH:mm:ss}] Client disconnected: {endpoint} (Total: {clients.Count})", ConsoleColor.Magenta);
                }
                BroadcastClientCount();
                client.Close();
            }
        }

        private int ReadFully(NetworkStream s, byte[] buffer, int offset, int size)
        {
            int total = 0;
            while (total < size)
            {
                int read = s.Read(buffer, offset + total, size - total);
                if (read == 0) return total;
                total += read;
            }
            return total;
        }

        private void ProcessMessage(MessageType type, byte[] data, TcpClient sender, string endpoint)
        {
            switch (type)
            {
                case MessageType.DrawLine:
                    var drawData = MessageSerializer.Deserialize<DrawData>(data);
                    lock (lockObj) { drawHistory.Add(drawData); }
                    Log($"[{DateTime.Now:HH:mm:ss}] Draw from {endpoint}", ConsoleColor.Cyan);
                    BroadcastMessage(type, data, sender);
                    break;

                case MessageType.InsertImage:
                    var imgData = MessageSerializer.Deserialize<ImageData>(data);
                    lock (lockObj) { imageHistory.Add(imgData); }
                    Log($"[{DateTime.Now:HH:mm:ss}] Image from {endpoint}", ConsoleColor.Cyan);
                    BroadcastMessage(type, data, sender);
                    break;

                case MessageType.Clear:
                    lock (lockObj) { drawHistory.Clear(); imageHistory.Clear(); }
                    Log($"[{DateTime.Now:HH:mm:ss}] Clear by {endpoint}", ConsoleColor.Yellow);
                    BroadcastMessage(type, data, sender);
                    break;

                case MessageType.End:
                    Log($"[{DateTime.Now:HH:mm:ss}] END SESSION from {endpoint}", ConsoleColor.Red);
                    BroadcastMessage(MessageType.End, null, null);
                    isRunning = false;
                    Stop();
                    Environment.Exit(0);
                    break;

                case MessageType.RequestSync:
                    SendSyncData(sender);
                    break;
            }
        }

        private void BroadcastMessage(MessageType type, byte[] data, TcpClient exclude)
        {
            byte[] typeBytes = BitConverter.GetBytes((int)type);
            byte[] lengthBytes = BitConverter.GetBytes(data?.Length ?? 0);

            lock (lockObj)
            {
                var disconnected = new List<TcpClient>();
                foreach (TcpClient c in clients)
                {
                    if (c == exclude) continue;
                    try
                    {
                        var s = c.GetStream();
                        s.Write(typeBytes, 0, 4);
                        s.Write(lengthBytes, 0, 4);
                        if (data?.Length > 0) s.Write(data, 0, data.Length);
                    }
                    catch { disconnected.Add(c); }
                }
                foreach (TcpClient c in disconnected)
                {
                    clients.Remove(c);
                    try { c.Close(); } catch { }
                }
                if (disconnected.Count > 0) BroadcastClientCount();
            }
        }

        private void BroadcastClientCount()
        {
            int count;
            lock (lockObj) { count = clients.Count; }
            byte[] countData = BitConverter.GetBytes(count);
            BroadcastMessage(MessageType.ClientCount, countData, null);
        }

        private void SendSyncData(TcpClient client)
        {
            lock (lockObj)
            {
                var syncData = new SyncData
                {
                    DrawHistory = drawHistory.ToArray(),
                    ImageHistory = imageHistory.ToArray()
                };
                byte[] data = MessageSerializer.Serialize(syncData);
                byte[] typeBytes = BitConverter.GetBytes((int)MessageType.SyncData);
                byte[] lengthBytes = BitConverter.GetBytes(data.Length);
                try
                {
                    var s = client.GetStream();
                    s.Write(typeBytes, 0, 4);
                    s.Write(lengthBytes, 0, 4);
                    s.Write(data, 0, data.Length);
                    Log($"[{DateTime.Now:HH:mm:ss}] Sync sent to new client", ConsoleColor.Gray);
                }
                catch (Exception ex) { Log($"[ERROR] Sync: {ex.Message}", ConsoleColor.Red); }
            }
        }

        private void SendEmailAlert()
        {
            try
            {
                string smtpServer = "smtp.gmail.com";
                int smtpPort = 587;
                string fromEmail = "your-email@gmail.com";
                string toEmail = "admin@example.com";
                string password = "your-app-password";

                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(fromEmail);
                    mail.To.Add(toEmail);
                    mail.Subject = "Whiteboard Alert: Max Clients Reached";
                    mail.Body = $"Maximum client limit of {MAX_CLIENTS} reached at {DateTime.Now:yyyy-MM-dd HH:mm:ss}.";

                    using (var smtp = new SmtpClient(smtpServer, smtpPort))
                    {
                        smtp.Credentials = new NetworkCredential(fromEmail, password);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
                Log($"[{DateTime.Now:HH:mm:ss}] EMAIL ALERT SENT to {toEmail}", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                Log($"[WARN] Email failed: {ex.Message}", ConsoleColor.Yellow);
            }
        }

        private void Log(string message, ConsoleColor color = ConsoleColor.Gray)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = prev;
        }
    }
}
