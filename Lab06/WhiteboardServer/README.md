# Collaborative Whiteboard Application

A real-time collaborative whiteboard application built with C# and Windows Forms, using TCP socket communication for synchronization between multiple clients.

## Features

### Core Requirements ✓
- **Server-Client Architecture**: Built on TCP socket communication
- **Real-time Drawing Synchronization**: All clients see changes instantly
- **Mouse Drawing**: Draw freely with the mouse
- **Auto-save and Close**: End button saves whiteboard as PNG/JPG and closes all clients
- **Customizable Drawing Tools**: 
  - Color picker for line colors
  - Adjustable line thickness (1-20 pixels)
- **Connected Client Counter**: Displays number of active clients

### Extension Features ✓
- **Image Insertion from URL**: Insert images from the internet with automatic resizing
- **New Client Synchronization**: Newly joined clients receive the current whiteboard state
- **Email Alert System**: Sends email to administrator when client limit (5) is reached
- **Eraser Tool**: Toggle between drawing and erasing

## Architecture

### Network Protocol
- **Transport**: TCP on port 8888
- **Message Format**: 
  - 4 bytes: Message Type (int)
  - 4 bytes: Data Length (int)
  - N bytes: Serialized Data (binary)

### Message Types
1. `DrawLine` - Drawing action from a client
2. `Clear` - Clear whiteboard command
3. `ClientCount` - Update connected client count
4. `End` - End session and save
5. `InsertImage` - Insert image from URL
6. `RequestSync` - Request current whiteboard state
7. `SyncData` - Send complete whiteboard state
8. `Erase` - Eraser action

### Thread Safety
- All client list operations are protected with locks
- UI updates use `Invoke` for cross-thread safety
- Drawing history is synchronized across all operations

## Project Structure

```
WhiteboardServer/
├── WhiteboardServer/          # Server application
│   ├── Form1.cs              # Server UI and logic
│   ├── Form1.Designer.cs     # Server UI designer
│   ├── WhiteboardProtocol.cs # Shared protocol definitions
│   └── Program.cs            # Entry point
├── WhiteboardClient/          # Client application
│   ├── Form1.cs              # Client UI and logic
│   ├── Form1.Designer.cs     # Client UI designer
│   ├── WhiteboardProtocol.cs # Shared protocol definitions
│   └── Program.cs            # Entry point
└── WhiteboardServer.sln       # Visual Studio solution
```

## How to Use

### Starting the Server
1. Open `WhiteboardServer.sln` in Visual Studio
2. Set `WhiteboardServer` as startup project
3. Run the application
4. Server automatically starts on port 8888
5. Server can also draw on the whiteboard

### Connecting Clients
1. Set `WhiteboardClient` as startup project (or run multiple instances)
2. Enter server IP address (default: 127.0.0.1 for localhost)
3. Click "Connect" button
4. Start drawing - all connected clients will see your changes in real-time

### Drawing Tools
- **Choose Color**: Opens color picker dialog
- **Thickness Slider**: Adjust line thickness (1-20 pixels)
- **Eraser**: Toggle eraser mode (draws in white)
- **Clear Whiteboard**: Clears entire whiteboard for all clients
- **Insert Image**: Enter image URL and click to insert (auto-resizes to max 300x300)
- **End Session**: Saves whiteboard as PNG to Desktop and closes all clients

## Configuration

### Email Alert Settings
Edit `WhiteboardServer/Form1.cs` line 347-351:
```csharp
string smtpServer = "smtp.gmail.com";
int smtpPort = 587;
string fromEmail = "your-email@gmail.com";
string toEmail = "admin@example.com";
string password = "your-app-password";
```

**Note**: For Gmail, use an App Password, not your regular password.

### Client Limit
Edit `WhiteboardServer/Form1.cs` line 24:
```csharp
private const int MAX_CLIENTS = 5;
```

### Server Port
Edit both files at:
- `WhiteboardServer/Form1.cs` line 54
- `WhiteboardClient/Form1.cs` line 67
```csharp
server = new TcpListener(IPAddress.Any, 8888);  // Server
client.Connect(serverIP, 8888);                  // Client
```

## Technical Details

### Synchronization Logic
1. **New Client Joins**:
   - Server accepts connection
   - Adds client to list
   - Sends complete drawing history (SyncData)
   - Updates all clients with new count
   - Checks if limit reached for email alert

2. **Drawing Action**:
   - Client sends DrawLine message to server
   - Server adds to history
   - Server draws on its whiteboard
   - Server broadcasts to all other clients
   - Each client draws on their whiteboard

3. **Image Insertion**:
   - Client/Server downloads image from URL
   - Resizes if larger than 300x300 (maintains aspect ratio)
   - Converts to byte array
   - Broadcasts to all clients
   - All clients render the image

4. **End Session**:
   - Any client/server clicks End
   - Saves whiteboard to Desktop as PNG
   - Broadcasts End message to all
   - All clients save and exit
   - Server saves and exits

### Error Handling
- Network disconnections are handled gracefully
- Failed clients are removed from list automatically
- Email failures don't crash the server
- Image download errors show user-friendly messages

## Requirements

- .NET Framework 4.7.2
- Windows OS (Windows Forms)
- Visual Studio 2017 or later
- Network connectivity for multi-machine testing

## Testing Checklist

- [x] Server starts successfully
- [x] Client connects to server
- [x] Drawing synchronizes in real-time
- [x] Color and thickness changes work
- [x] Eraser functions correctly
- [x] Clear whiteboard works for all clients
- [x] Image insertion from URL works
- [x] Image auto-resizing works
- [x] New clients receive current whiteboard state
- [x] Client count displays correctly
- [x] End button saves PNG and closes all
- [x] Email alert triggers at 5 clients
- [x] Thread safety (no race conditions)
- [x] Graceful disconnect handling

## Known Limitations

1. **Email Configuration Required**: Email alerts require manual SMTP configuration
2. **Fixed Image Position**: Inserted images always appear at (50, 50)
3. **No Undo/Redo**: Drawing actions cannot be undone
4. **No User Authentication**: No login system
5. **Single Whiteboard**: Only one whiteboard session per server

## Future Enhancements

- Drag and drop image positioning
- Multiple whiteboard pages
- User authentication and permissions
- Undo/Redo functionality
- Shape tools (rectangle, circle, line)
- Text tool
- Save/Load whiteboard sessions
- WebSocket support for web clients

## License

This project is created for educational purposes (NT106 Lab 06).

## Author

Created for NT106 - Network Programming Course
