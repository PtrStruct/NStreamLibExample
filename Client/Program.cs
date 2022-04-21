using NStreamLib;
using System;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    internal class Program
    {
        private static TcpClient _tcpClient;
        private static NStream _nStream;
        static void Main(string[] args)
        {
            _tcpClient = new TcpClient();
            _tcpClient.Connect("127.0.0.1", 5757);
            _nStream = new NStream(_tcpClient.GetStream());
            
            new Thread(ListenForData).Start();
            
            while (true)
            {
                for (int i = 0; i < 2; i++)
                {
                    _nStream.WriteString("Hello World!");
                    _tcpClient.Client.Send(_nStream.ToArray());
                }
                Thread.Sleep(800);
            }
        }

        private static void ListenForData()
        {
            while (true)
            {
                Console.WriteLine(_nStream.ReadString());
            }
        }
    }
}