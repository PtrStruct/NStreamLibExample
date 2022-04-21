using NStreamLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    internal class Player
    {
        public Guid ID { get; set; }
        public TcpClient Socket { get; set; }
        public NStream NStream { get; set; }

        public Player(TcpClient client)
        {
            ID = Guid.NewGuid();
            Socket = client;
            NStream = new NStream(Socket.GetStream());
        }

    }
}
