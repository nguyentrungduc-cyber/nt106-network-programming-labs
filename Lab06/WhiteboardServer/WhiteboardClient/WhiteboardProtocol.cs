using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace WhiteboardClient
{
    // Message types for communication
    public enum MessageType
    {
        DrawLine,
        Clear,
        ClientCount,
        End,
        InsertImage,
        RequestSync,
        SyncData,
        Erase
    }

    // Base message class
    [Serializable]
    public class WhiteboardMessage
    {
        public MessageType Type { get; set; }
        public byte[] Data { get; set; }

        public WhiteboardMessage(MessageType type, byte[] data = null)
        {
            Type = type;
            Data = data;
        }
    }

    // Drawing data
    [Serializable]
    public class DrawData
    {
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
        public int ColorArgb { get; set; }
        public float Thickness { get; set; }
        public bool IsEraser { get; set; }

        public Color GetColor()
        {
            return Color.FromArgb(ColorArgb);
        }

        public void SetColor(Color color)
        {
            ColorArgb = color.ToArgb();
        }
    }

    // Image insertion data
    [Serializable]
    public class ImageData
    {
        public byte[] ImageBytes { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    // Sync data containing all drawing history
    [Serializable]
    public class SyncData
    {
        public DrawData[] DrawHistory { get; set; }
        public ImageData[] ImageHistory { get; set; }
    }

    // Serialization helper
    public static class MessageSerializer
    {
        public static byte[] Serialize(object obj)
        {
            if (obj == null) return null;
            
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] data)
        {
            if (data == null) return default(T);
            
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
