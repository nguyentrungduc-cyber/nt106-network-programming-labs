# Quick Start Guide - Collaborative Whiteboard

## 🚀 Quick Setup (5 minutes)

### Step 1: Open the Solution
1. Navigate to: `/home/minhiw/Thuc_hanh-NT106/Lab06/WhiteboardServer/`
2. Open `WhiteboardServer.sln` in Visual Studio

### Step 2: Configure Email (Optional)
If you want email alerts when 5 clients connect:

Edit `WhiteboardServer/Form1.cs` lines 347-351:
```csharp
string smtpServer = "smtp.gmail.com";
int smtpPort = 587;
string fromEmail = "your-email@gmail.com";      // ← Change this
string toEmail = "admin@example.com";            // ← Change this
string password = "your-app-password";           // ← Change this (use App Password for Gmail)
```

**Skip this if you just want to test the whiteboard functionality.**

### Step 3: Run the Server
1. In Visual Studio, right-click `WhiteboardServer` project → Set as Startup Project
2. Press F5 or click Start
3. Server window opens and shows "Server started on port 8888"

### Step 4: Run Client(s)
**Option A: Multiple instances from Visual Studio**
1. Right-click `WhiteboardClient` project → Debug → Start New Instance
2. Enter server IP: `127.0.0.1` (for localhost)
3. Click "Connect"
4. Repeat for more clients

**Option B: Run compiled executables**
1. Build the solution (Ctrl+Shift+B)
2. Navigate to: `WhiteboardClient/bin/Debug/WhiteboardClient.exe`
3. Run multiple copies
4. Each connects to `127.0.0.1:8888`

### Step 5: Start Drawing!
- Click and drag on the whiteboard to draw
- Use "Choose Color" to change colors
- Adjust thickness slider
- All connected clients see changes in real-time

## 🎨 Features to Try

### Basic Drawing
- **Draw**: Click and drag on whiteboard
- **Change Color**: Click "Choose Color" button
- **Change Thickness**: Move the slider (1-20 pixels)
- **Erase**: Click "Eraser" button, then draw (draws in white)

### Advanced Features
- **Insert Image**: 
  1. Paste image URL in text box
  2. Click "Insert Image"
  3. Image appears on all clients (auto-resized to max 300x300)
  
- **Clear All**: Click "Clear Whiteboard" to erase everything

- **End Session**: 
  1. Click "End Session" button
  2. All clients save whiteboard as PNG to Desktop
  3. All applications close automatically

### Test Synchronization
1. Open server + 2 clients
2. Draw on client 1 → appears on client 2 and server ✓
3. Draw on server → appears on both clients ✓
4. Connect a 3rd client → it receives all previous drawings ✓

### Test Client Limit Alert
1. Connect 5 clients
2. Check server console/email for alert message
3. (Email only works if configured in Step 2)

## 📁 File Locations

### Source Code
```
WhiteboardServer/
├── WhiteboardServer/
│   ├── Form1.cs              ← Server logic
│   ├── Form1.Designer.cs     ← Server UI
│   └── WhiteboardProtocol.cs ← Shared protocol
├── WhiteboardClient/
│   ├── Form1.cs              ← Client logic
│   ├── Form1.Designer.cs     ← Client UI
│   └── WhiteboardProtocol.cs ← Shared protocol
└── WhiteboardServer.sln      ← Open this in Visual Studio
```

### Saved Images
Whiteboard images are saved to Desktop as:
- `Whiteboard_YYYYMMDD_HHMMSS.png`

## 🔧 Troubleshooting

### "Server already running" error
- Another instance is using port 8888
- Close all instances and restart
- Or change port in code (see README.md)

### Client can't connect
- Check server is running
- Verify IP address (use `127.0.0.1` for localhost)
- Check firewall settings
- Ensure port 8888 is not blocked

### Drawing not syncing
- Check network connection
- Verify all clients are connected (check client count)
- Restart server and reconnect clients

### Image won't load
- Check URL is valid and accessible
- Ensure image format is supported (PNG, JPG, GIF, BMP)
- Check internet connection

### Email not sending
- Verify SMTP settings are correct
- For Gmail: Use App Password, not regular password
- Enable "Less secure app access" or use App Password
- Check internet connection

## 🎯 Testing Checklist

Quick tests to verify everything works:

- [ ] Server starts without errors
- [ ] Client connects successfully
- [ ] Drawing appears on all clients in real-time
- [ ] Color picker works
- [ ] Thickness slider works
- [ ] Eraser works
- [ ] Clear whiteboard works
- [ ] Image insertion works
- [ ] New client receives existing drawings
- [ ] Client count displays correctly
- [ ] End button saves PNG and closes all

## 📞 Common Questions

**Q: Can I run server and client on different computers?**
A: Yes! Just use the server's IP address instead of 127.0.0.1

**Q: How many clients can connect?**
A: Unlimited, but email alert triggers at 5 clients

**Q: Can I change the port?**
A: Yes, edit port 8888 in both server and client code

**Q: Where are images saved?**
A: Desktop folder with timestamp filename

**Q: Can I undo drawings?**
A: No, use Clear Whiteboard to start over

**Q: Do I need to configure email?**
A: No, it's optional. App works fine without it.

## 🎓 For Demonstration

### Scenario 1: Basic Collaboration (2 minutes)
1. Start server
2. Connect 2 clients
3. Draw on client 1 (red circle)
4. Draw on client 2 (blue square)
5. Show both see each other's drawings

### Scenario 2: Late Joiner (1 minute)
1. With existing drawings on screen
2. Connect a new client
3. Show it receives all previous content

### Scenario 3: Image Insertion (1 minute)
1. Use URL: `https://picsum.photos/500/500`
2. Click Insert Image
3. Show image appears on all clients

### Scenario 4: End Session (30 seconds)
1. Click End Session
2. Show PNG saved to Desktop
3. All applications close

## 📚 Additional Documentation

- `README.md` - Complete documentation
- `LOGIC_VERIFICATION.md` - Detailed code review and verification

---

**Ready to start? Open Visual Studio and press F5!** 🚀
