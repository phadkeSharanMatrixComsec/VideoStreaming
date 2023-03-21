using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace VideoStreaming
{
    class Server
    {
        private int _port;
        private IPEndPoint _ip;

        private List<Socket>? _clientList;

        private Socket _serverSocket;
        private int MAX_MB = 10;

        public Server(int port)
        {
            this._port = port;
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ip = ipHost.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ip, _port);
            this._ip = localEndPoint;
            this._serverSocket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this._serverSocket.Bind(localEndPoint);
        }

        public bool checkEOF(byte[] clientData)
        {
            string endString = Encoding.ASCII.GetString(clientData, 0, 5);
            return endString == "<EOF>";
        }

        public void GetData(Socket clientSocket)
        {
            int total = 0;
            byte[] clientData = new Byte[1024 * 1024 * MAX_MB];
            // int numBytes = clientSocket.Receive(clientData);
            byte[] buffer = new Byte[1024 * 1024 * MAX_MB];
            int recvBytes;
            BinaryWriter writer;

            if((recvBytes = clientSocket.Receive(buffer)) > 0)
            {
                
                string fileName = Encoding.ASCII.GetString(buffer.TakeWhile(x => x != 0).ToArray());
                // string fileName = Encoding.ASCII.GetString(buffer, 0, 32);
                Console.WriteLine($"filename : {fileName}");
                writer = new BinaryWriter(File.Open(fileName, FileMode.Append));
                Buffer.BlockCopy(buffer, 32, clientData, 0, recvBytes-32);
                Console.WriteLine($"size : {recvBytes}");
                writer.Write(clientData, 0, recvBytes-32);
                total += recvBytes;
                Array.Clear(buffer, 0, 1024*1024*MAX_MB);
                Array.Clear(clientData, 0, 1024*1024*MAX_MB);
            }
            else
            {
                Console.WriteLine("else --------->");
                writer = new BinaryWriter(File.Open("cow.mp3", FileMode.Append));
            }
            while((recvBytes = clientSocket.Receive(buffer)) > 0)
            {
                Buffer.BlockCopy(buffer, 0, clientData, 0, recvBytes);
                Console.WriteLine($"size : {recvBytes}");
                writer.Write(clientData, 0, recvBytes);
                total += recvBytes;
                Array.Clear(buffer, 0, 1024*1024*MAX_MB);
                Array.Clear(clientData, 0, 1024*1024*MAX_MB);
            }

            Console.WriteLine(total);
        }

        public void Connect()
        {
            try
            {
                this._serverSocket.Listen(10);

                while(true)
                {
                    Console.WriteLine("Waiting for client connection");
                    Socket clientSocket = this._serverSocket.Accept();
                    Console.WriteLine($"Connected to : {clientSocket.RemoteEndPoint.ToString()}");

                    new Thread(()=>{
                        GetData(clientSocket);
                    }).Start();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void Main(String[] args)
        {
            Server server = new Server(8000);
            server.Connect();
        }
    }
}

