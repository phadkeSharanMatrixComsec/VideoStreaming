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
        private int MAX_MB = 10;

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
                // byte[] clientData = new byte[1024 * 1024 * this.MAX_MB];
                byte[] fileInfoBuffer = new byte[32];
                byte[] fileInfo = Encoding.ASCII.GetBytes(fileName);
                Buffer.BlockCopy(fileInfo, 0, fileInfoBuffer, 0, fileInfo.Length);
                Console.WriteLine($"file : {Encoding.ASCII.GetString(fileInfoBuffer)}");
                byte[] filedata = new byte[file.Length + 32];
                Console.WriteLine($"fileInfoBufferLenght {fileInfoBuffer.Length}");
                Buffer.BlockCopy(fileInfoBuffer, 0, filedata, 0, 32);
                Buffer.BlockCopy(file, 0, filedata, 32, file.Length);
                this._clientSocket.Send(filedata);
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
                SendFile("cow.mp4");
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

