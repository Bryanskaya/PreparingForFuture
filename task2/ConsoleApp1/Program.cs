using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ConsoleApplication1
{
    class Server
    {
        static int port;
        static Socket listenSocket;
        static List<IPEndPoint> clientsArr = new List<IPEndPoint>();
        static void Main(string[] args)
        {
            try
            {
                port = Int32.Parse("32123"); //TODO уточнить
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
                listenThread.Wait();
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
            bool isNew;

            try
            {
                IPEndPoint localIP = new IPEndPoint(IPAddress.Parse("0.0.0.0"), port);
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

                    isNew = true;

                    for (int i = 0; i < clientsArr.Count; i++)
                    {
                        if (clientsArr[i].Address.ToString() == remoteFull.Address.ToString())
                        {
                            isNew = false;
                        }
                    }

                    if (isNew)
                    {
                        clientsArr.Add(remoteFull);
                    }

                    sendAll(str.ToString(), remoteFull.Address.ToString());
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

        private static void sendAll(string str, string ip)
        {
            byte[] data = Encoding.Unicode.GetBytes(str);

            for (int i = 0; i < clientsArr.Count; i++)
            {
                if (clientsArr[i].Address.ToString() != ip)
                {
                    listenSocket.SendTo(data, clientsArr[i]);
                }
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

            Console.WriteLine(">>> Server was died");
        }
    }
}
