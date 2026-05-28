# Collaborative Whiteboard Application

A real-time collaborative whiteboard application built with C# and Windows Forms, using TCP socket communication for synchronization between multiple clients. Features a modern dark-theme UI with preset color palette, emoji icons, and real-time collaboration.

## Features

### Core Requirements ✓
- **Server-Client Architecture**: Built on TCP socket communication
- **Real-time Drawing Synchronization**: All clients see changes instantly
- **Mouse Drawing**: Draw freely with the mouse (smooth lines with rounded caps)
- **Auto-save and Close**: End button saves whiteboard as PNG/JPG and closes all clients
- **Customizable Drawing Tools**: 
  - Custom color picker + 12 preset colors palette
  - Adjustable line thickness (1-20 pixels) with live preview
  - Eraser tool with visual toggle (turns red when active)
- **Connected Client Counter**: Displays number of active clients in a colored badge

### Extension Features ✓
- **Image Insertion from URL**: Insert images from the internet with automatic resizing (maintains aspect ratio, max 300x300)
- **New Client Synchronization**: Newly joined clients receive the complete current whiteboard state (including all drawings and images)
- **Email Alert System**: Sends email alert to administrator when client limit (5 clients) is reached
- **Eraser Tool**: Toggle between drawing and erasing mode
- **Confirmation Dialogs**: Clear and End actions require confirmation to prevent accidents

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
- **🎨 Custom Color**: Opens color picker dialog for custom colors
- **Preset Colors**: Click any color swatch in the palette (12 preset colors)
- **✏️ Thickness**: Use slider (1-20 pixels), shows current value
- **🧹 Eraser**: Toggle eraser mode (turns red when active)
- **🗑 Clear**: Clears entire whiteboard (with confirmation dialog)
- **🖼 Insert Image**: Enter image URL and click to insert (auto-resizes to max 300x300, maintains aspect ratio)
- **✕ End Session**: Saves whiteboard as PNG to Desktop and closes all clients (with confirmation dialog)

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

## UI Design (Modern Whiteboard Game Style)
- **Dark Theme Toolbar**: Dark blue (#2c3e50) right-side panel with organized sections
- **Top Header Bar**: App title in Segoe UI 18pt bold
- **Preset Color Palette**: 12 quick-select colors (dark grays, red, orange, yellow, green, purple, blue, teal, gray, black, white)
- **Live Color Preview**: Shows current selected color in real-time
- **Client Count Badge**: Colored panel showing connected users count
- **Thickness Display**: Trackbar with large numeric readout (green font)
- **Emoji Icons**: 🎨 🧹 🗑 🖼 ✕ 🔌 for visual tool identification
- **Toggle States**: Eraser button turns red when active
- **Confirmation Dialogs**: Clear and End actions require Yes/No confirmation
- **Placeholder Text**: Smart textboxes with placeholder behavior on focus enter/leave
- **Smooth Drawing**: Lines use `LineCap.Round` for polished appearance
- **Fixed Window**: 1178×692 centered on screen, non-resizeable

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

## Bug Fixes Applied (v2.0)

### Critical Bug Fixes
1. **End Button Race Condition**: Client was calling `Application.Exit()` immediately without waiting for server's End broadcast. Fixed: client now sends End message and waits for server acknowledgment with 3-second fallback timeout.
2. **WebClient Memory Leak**: `WebClient` was not disposed after image download. Fixed: added `using` statement for proper disposal.
3. **OnFormClosing Deadlock**: Potential lock contention between UI thread and background thread. Fixed: lock acquisition reordered to prevent deadlock scenarios.
4. **Thread-Unsafe Save**: `SaveWhiteboardImage()` was called from background thread without marshaling to UI thread. Fixed: added `Invoke` protection for all save operations.
5. **Stream Null Reference**: `stream.Read()` could throw if stream becomes null. Fixed: added `ReadFully` helper with proper null checks and partial read handling.
6. **Drawing Quality**: Lines had jagged edges. Fixed: added `LineCap.Round` for smooth line endings.

### Stability Improvements
- Added `isEnding` flag to prevent duplicate End processing
- Added `isServerRunning` flag for clean server shutdown
- Proper disposal of Graphics and Bitmap resources in `OnFormClosing`
- Graceful handling of `ObjectDisposedException` when stopping server
- Timer-based fallback for End broadcast (3-second timeout)

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
