using System.Net;
using System.Net.Sockets;
using System.Text;

namespace VideoStreaming
{
    class Client
    {
        private int _port;
        private int _clientId;

        private EndPoint? _ip;
        private IPEndPoint _serverIp;
        private Socket _clientSocket;
        private int MAX_MB = 1;

        public Client(int port, int clientID)
        {
            this._port = port;
            this._clientId = clientID;
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[0];
            this._serverIp = new IPEndPoint(ipAddr, this._port);
            this._clientSocket = new Socket(ipAddr.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

            this._ip = this._clientSocket.RemoteEndPoint;
        }

        public void SendFile(string fileName)
        {
            try
            {
                byte[] file = System.IO.File.ReadAllBytes(fileName);
                byte[] clientData = new byte[1024 * 1024 * this.MAX_MB];

                this._clientSocket.Send(file);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void Connect()
        {
            try
            {
                this._clientSocket.Connect(this._serverIp);
                Console.WriteLine("Client connected : {0} {1}", this._ip, this._clientId);
                SendFile("design.pdf");
                // byte[] endMessage = Encoding.ASCII.GetBytes("<EOF>");
                // this._clientSocket.Send(endMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void Main(String[] args)
        {
            Client client = new Client(8000, 10001);
            client.Connect();
        }

    } 
}

