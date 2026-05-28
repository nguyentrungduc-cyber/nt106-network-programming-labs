using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace WhiteboardProtocol
{
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

        public Color GetColor() => Color.FromArgb(ColorArgb);
        public void SetColor(Color color) => ColorArgb = color.ToArgb();
    }

    [Serializable]
    public class ImageData
    {
        public byte[] ImageBytes { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    [Serializable]
    public class SyncData
    {
        public DrawData[] DrawHistory { get; set; }
        public ImageData[] ImageHistory { get; set; }
    }

    public sealed class WhiteboardBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            // Resolve type regardless of which assembly (server or client) serialized it
            string currentAsm = typeof(WhiteboardBinder).Assembly.FullName;
            Type type = Type.GetType($"{typeName}, {currentAsm}");
            if (type != null) return type;
            return Type.GetType(typeName);
        }
    }

    public static class MessageSerializer
    {
        private static readonly WhiteboardBinder Binder = new WhiteboardBinder();

        public static byte[] Serialize(object obj)
        {
            if (obj == null) return null;
            var formatter = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                formatter.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] data)
        {
            if (data == null) return default(T);
            var formatter = new BinaryFormatter();
            formatter.Binder = Binder;
            using (var ms = new MemoryStream(data))
            {
                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
