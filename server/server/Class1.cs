using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;

namespace class1
{
    public class ConnectedClient
    {
        public TcpClient Client { get; set; }
        public string Name { get; set; }

        public ConnectedClient(TcpClient client, string name)
        {
            Client = client;
            Name = name;
        }
    }
}
