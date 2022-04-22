using System.Net.Sockets;
using System.Text;

namespace NStreamLib
{
    public class NStream : BinaryReader
    {
        private MemoryStream _writer;
        public NStream(NetworkStream stream) : base(stream)
        {
            _writer = new MemoryStream();
        }

        public void WriteString(string s)
        {
            var data = Encoding.ASCII.GetBytes(s, 0, s.Length);
            if (data.Length > 255)
                throw new ArgumentException("String can't exceed 255 bytes.");

            WriteByte((byte)data.Length);
            _writer.Write(data);
        }

        public string ReadString()
        {
            var stringLength = ReadByte();
            var stringBuffer = ReadBytes(stringLength);
            return Encoding.ASCII.GetString(stringBuffer);
        }

        public void WriteByte(byte b)
        {
            _writer.WriteByte(b);
        }

        public void WriteHWord(short s)
        {
            _writer.WriteByte((byte)(s >> 8));
            _writer.WriteByte((byte)(s & 0xFF));
        }

        public void WriteWord(int i)
        {
            _writer.WriteByte((byte)(i >> 24));
            _writer.WriteByte((byte)(i >> 16));
            _writer.WriteByte((byte)(i >> 8));
            _writer.WriteByte((byte)(i & 0xFF));
        }

        public short ReadHWord()
        {
            return (short)(ReadByte() << 8 | ReadByte());
        }

        public int ReadWord()
        {
            return ReadByte() << 24 |
                   ReadByte() << 16 |
                   ReadByte() << 8 |
                   ReadByte();
        }

        public byte[] ToArray()
        {
            var buffer = new MemoryStream();
            _writer.WriteTo(buffer);
            _writer.SetLength(0);
            return buffer.ToArray();
        }
    }
}