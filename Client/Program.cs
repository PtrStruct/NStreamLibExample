using NStreamLib;
using System.Net.Sockets;

namespace Client
{
    internal class Program
    {
        private static TcpClient _tcpClient;
        private static NStream _nStream;
        private static List<string> _remotePlayers = new List<string>();

        static void Main(string[] args)
        {
            _tcpClient = new TcpClient();
            _tcpClient.Connect("127.0.0.1", 5757);
            _nStream = new NStream(_tcpClient.GetStream());

            new Thread(ListenForData).Start();

            while (true)
            {
                _nStream.WriteByte(10);
                _nStream.WriteString(Console.ReadLine());
                _tcpClient.Client.Send(_nStream.ToArray());
            }
        }

        private static void ListenForData()
        {
            while (true)
            {
                var opcode = _nStream.ReadByte();
                switch (opcode)
                {
                    case 1:
                        var count = _nStream.ReadHWord();
                        Console.Title = count.ToString();
                        break;
                    case 3:
                        var j = _nStream.ReadString();
                        _remotePlayers.Add(j);
                        Console.WriteLine($"Player Connected! {j}");
                        break;
                    case 10:
                        Console.WriteLine(_nStream.ReadString());
                        break;
                    default:
                        Console.WriteLine($"[{opcode}] - wat?");
                        break;
                }
            }
        }
    }
}