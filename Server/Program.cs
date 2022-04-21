using Client.Models;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
                /* Per Cycle */
                /* - Accept 10 Connections */
                /* - Gather Up To 10 Full Packets Per Connected Client */
                /* Broadcast To All Clients */

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
                _players.Add(new Player(_tcpListener.AcceptTcpClient()));

            foreach (var player in _players)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (player.Socket.Available <= 0)
                    {
                        Console.WriteLine("No more from this socket");
                        continue;
                    }

                    var msg = player.NStream.ReadString();
                    Console.WriteLine($"Received: {msg} from {player.Socket.Client.RemoteEndPoint}");

                    for (int r = 0; r < _players.Count; r++)
                    {
                        if (_players[r].ID == player.ID)
                            continue;

                        player.NStream.WriteString(msg);
                        _players[r].Socket.Client.Send(player.NStream.ToArray());
                    }
                }
            }
        }
    }
}