using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace WindowsFormsApplication2
{
    class Client
    {
        static IPAddress idAddr;
        static int port;
        static Socket listenSocket;

        static void Main(string[] args)
        {
            string msg;
            byte[] data;

            try
            {
                idAddr = IPAddress.Parse("127.0.0.1");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString() + "\n" + ex.Message);
            }

            try
            {
                port = Int32.Parse("8000");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString() + "\n" + ex.Message);
            }

            try
            {
                listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                Task listenThread = new Task(listen);
                listenThread.Start();

                while (true)
                {
                    msg = Console.ReadLine();
                    data = Encoding.Unicode.GetBytes(msg);

                    EndPoint pnt = new IPEndPoint(idAddr, port);

                    listenSocket.SendTo(data, pnt);
                    data = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString() + "\n" + ex.Message);
            }
            finally
            {
                close();
            }
        }

        private static void listen()
        {
            StringBuilder str = new StringBuilder();
            IPEndPoint remoteFull;
            int count;
            byte[] data = new byte[256];

            try
            {
                IPEndPoint localIP = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 0);
                listenSocket.Bind(localIP);

                while (true)
                {
                    EndPoint remoteIP = new IPEndPoint(IPAddress.Any, 0);

                    do
                    {
                        count = listenSocket.ReceiveFrom(data, ref remoteIP);
                        str.Append(Encoding.Unicode.GetString(data, 0, count));
                    }
                    while (listenSocket.Available > 0);

                    remoteFull = remoteIP as IPEndPoint; // TODO через ()

                    Console.WriteLine("{0}:{1} -> {2}", remoteFull.Address.ToString(),
                                                        remoteFull.Port.ToString(),
                                                        str.ToString());

                    str.Clear();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString() + "\n" + ex.Message);
            }
            finally
            {
                close();
            }
        }

        private static void close()
        {
            if (listenSocket != null)
            {
                listenSocket.Shutdown(SocketShutdown.Both);
                listenSocket.Close();
                listenSocket = null;
            }

            Console.WriteLine(">>> Client was died");
        }
    }
}
