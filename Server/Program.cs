using Client.Models;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    internal class Program
    {
        private static List<Player> _players = new List<Player>();
        private static TcpListener _tcpListener;
        static void Main(string[] args)
        {
            _tcpListener = new TcpListener(IPAddress.Any, 5757);
            _tcpListener.Start();

            Run();

            Console.ReadLine();
        }

        private static void Run()
        {
            var sw = new Stopwatch();
            var tickRate = 1500;

            while (true)
            {
                sw.Start();

                Cycle();

                sw.Stop();
                var sleepDelta = tickRate - (int)sw.ElapsedMilliseconds;
                if (sleepDelta > 0)
                    Thread.Sleep(sleepDelta);
                else
                    Console.WriteLine($"Server can't keep up! {(int)sw.ElapsedMilliseconds}"); /* Not Threadsafe */

                sw.Reset();
            }
        }

        private static void Cycle()
        {
            if (_tcpListener.Pending())
            {
                var connectedPlayer = new Player(_tcpListener.AcceptTcpClient());
                _players.Add(connectedPlayer);

                foreach (var player in _players)
                {
                    player.NStream.WriteByte(1);
                    player.NStream.WriteHWord((short)_players.Count);
                    player.Socket.Client.Send(player.NStream.ToArray());

                    if (player.ID == connectedPlayer.ID) continue;
                    player.NStream.WriteByte(3);
                    player.NStream.WriteString(connectedPlayer.ID.ToString());
                    player.Socket.Client.Send(player.NStream.ToArray());
                }
            }

            foreach (var player in _players)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (player.Socket.Available <= 0) continue;
                    var opcode = player.NStream.ReadByte();

                    switch (opcode)
                    {
                        case 10:
                            var msg = player.NStream.ReadString();
                            Console.WriteLine($"Received: {msg} from {player.Socket.Client.RemoteEndPoint}");

                            for (int r = 0; r < _players.Count; r++)
                            {
                                if (_players[r].ID == player.ID)
                                    continue;

                                player.NStream.WriteByte(10);
                                player.NStream.WriteString($"From {player.ID}: {msg}");
                                _players[r].Socket.Client.Send(player.NStream.ToArray());
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}