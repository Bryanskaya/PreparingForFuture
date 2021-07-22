using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using Errs;

namespace Kmbo
{
    public class Ukmbo_mc
    {
        static UInt64 init_MC_UDP = 0xC0000007L;         //признак инициализации модуля МС 0 - успех
        static UInt64 init_MC_Kmbo = 0xC0000007L;        //признак инициализации модуля МС линия КМБО 0 - успех

        static int usPort_MKIO = 0x4001;                 //порт инициализации
        static int usPort_KMBO = 0x4002;                 //порт КМБО

        static Socket sSocket_UDP;
        static Socket sSocket_KMBO;

        const int lenStrIpDest = 16;
        static string strIpDest;

        //начало кадра - 0xabcd, слово флагов - 0x0001, режим МКИО контроллер - 0x0001(оконечник 0x0000), слово адреса мкио 0x0001(адрес = 1), конец кадра - 0xdcba
        static byte[] NU_MIL_BC = new byte[12] { 0xCD, 0xAB, 0x01, 0x00, 0x01, 0x00,
                                                 0x01, 0x00, 0x15, 0x00, 0xBA, 0xDC };//инициализация МС в режиме контроллера (адрес = 1)
        public static long time_on_wr;
        public static long time_on_rd;
        long time_max_wr = 0;
        long time_max_rd = 0;

        public RObj MC_Open()
        {
            RObj res = new RObj();

            Deinit();
            if (Init("192.168.1.71").Code == 0) //1
            {
                DeinitCom_Kmbo();
                res.Code = (ulong)InitCom_Kmbo("192.168.1.71").Code;
            }
            else
            {
                res.Code = 0xC0000006L;
            }

            return res;
        }
        public RObj MC_Close()
        {
            RObj res = new RObj();

            res.Code = Deinit().Code;
            res.Code += DeinitCom_Kmbo().Code;

            return res;
        }
        public RObj MC_Read(TKmboRead argData)
        {
            long li_Freq;
            Stopwatch stopwatch;
            long li_TimeNow;

            byte[] bufferReceive = new byte[1460];
            UInt16[] bf_Receive = new UInt16[730];

            int result = 0;
            UInt16 Number_Frame_KMBO = 0;

            TKmboRead kr = (TKmboRead)argData.Clone();

            EndPoint ipSender;
            IPEndPoint ipDest;

            try
            {
                ipSender = new IPEndPoint(IPAddress.Any, 0);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString() + "\n" + ex.Message);
                return new RObj(0xC0000005L);
            }

            RObj res = new RObj(0xC0000007L);

            if (init_MC_Kmbo != 0) //1
            {
                return res;
            }

            ArrayList al = new ArrayList();

            al.Add((UInt16)0xabcd);
            al.Add((UInt16)0x0000);
            al.Add((UInt16)0x0001);//кол-во кадров

            al.Add((UInt16)(kr.count & 0x00ff));
            al.Add((UInt16)(((kr.address << 8) & 0xff00) | 0x0080));
           
            Number_Frame_KMBO++;

            al[2] = (UInt16)Number_Frame_KMBO;
            al.Add((UInt16)0xdcba);

            try
            {
                ipDest = new IPEndPoint(IPAddress.Parse(strIpDest), usPort_KMBO);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString() + "\n" + ex.Message);
                return new RObj(0xC0000005L);
            }

            sSocket_KMBO.SendTo(StructToByteArray(al), ipDest);

            /*li_Freq = Stopwatch.Frequency;
            stopwatch = new Stopwatch();
            stopwatch.Start();*/

            /*do
            {*/
            result = sSocket_KMBO.ReceiveFrom(bufferReceive, ref ipSender);
            //if (result > 0) break;

            //stopwatch.Stop();
            /*li_TimeNow = stopwatch.ElapsedTicks / li_Freq * 1000; //тики / тики/секунда + перевод в миллисекунды*/
            //li_TimeNow.QuadPart = ((li_TimeNow.QuadPart - li_TimeStart.QuadPart) * 1000000) / li_Freq.QuadPart;

            /*time_max_rd = li_TimeNow;
        }
        while (li_TimeNow < 32600); //TODO

        if (time_max_rd > time_on_rd) time_on_rd = time_max_rd;*/

            if (result > 0)
            {
                Buffer.BlockCopy(bufferReceive, 0, bf_Receive, 0, bufferReceive.Length);

                if (bf_Receive[0] == 0xabcd)
                {
                    for (int i = 4; i < bf_Receive.Length / 2; i++)
                    {
                        if ((bf_Receive[i] & 0xff00) == (((kr.address << 8) & 0xff00) /*| 0x0080*/))
                        {
                            Buffer.BlockCopy(bf_Receive, i + 1, kr.lpBuffer, 0, (int)kr.count);
                            //memcpy(kr.lpBuffer, &bf_Receive[i + 1], kr.Count * 2);

                            kr = argData;
                            res.Code = 0;
                            break;
                        }
                        if (bf_Receive[i] == 0xdcba)
                        {
                            break;
                        }
                    }
                }
            }

            return res;
        }
        public RObj MC_Write(TKmboWrite argData)
        {
            long li_Freq;
            Stopwatch stopwatch;
            long li_TimeNow;

            RObj res = new RObj(0xC0000007L);

            byte[] bufferReceive = new byte[1460];
            UInt16[] bf_Receive = new UInt16[730];

            int result;
            UInt16 Number_Frame_KMBO = 0;

            TKmboWrite wr = (TKmboWrite)argData.Clone();

            EndPoint ipSender;

            try
            {
                ipSender = new IPEndPoint(IPAddress.Any, 0);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString() + "\n" + ex.Message);
                return new RObj(0xC0000005L);
            }

            if (init_MC_Kmbo != 0)
            {
                return res;
            }

            ArrayList al = new ArrayList();

            al.Add((UInt16)0xabcd);
            al.Add((UInt16)0x0000);
            al.Add((UInt16)0x0001);//кол-во кадров

            al.Add((UInt16)(wr.count & 0x00ff));
            al.Add((UInt16)((wr.address << 8) & 0xff00));

            for (int i = 0; i < wr.count; i++) al.Add(wr.lpBuffer[i]);
            Number_Frame_KMBO++;

            al[2] = (UInt16)Number_Frame_KMBO;
            al.Add((UInt16)0xdcba);

            IPEndPoint ipDest;

            try
            {
                ipDest = new IPEndPoint(IPAddress.Parse(strIpDest), usPort_KMBO);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString() + "\n" + ex.Message);
                return new RObj(0xC0000005L);
            }

            sSocket_KMBO.SendTo(StructToByteArray(al), ipDest);

            /*li_Freq = Stopwatch.Frequency;
            stopwatch = new Stopwatch();
            stopwatch.Start();*/

            /*do
            {*/
                result = sSocket_KMBO.ReceiveFrom(bufferReceive, ref ipSender);
                /*if (result > 0) break;*/

                /*stopwatch.Stop();*/
                /*li_TimeNow = stopwatch.ElapsedTicks / li_Freq * 1000; //тики / тики/секунда -> миллисекунды
                //li_TimeNow.QuadPart = ((li_TimeNow.QuadPart - li_TimeStart.QuadPart) * 1000000) / li_Freq.QuadPart;

                time_max_wr = li_TimeNow;
            }
            while (li_TimeNow < 32600);*/

            if (time_max_wr > time_on_wr) time_on_wr = time_max_wr;

            if (result > 0)
            {
                Buffer.BlockCopy(bufferReceive, 0, bf_Receive, 0, bufferReceive.Length);
                if (bf_Receive[0] == 0xabcd) res.Code = 0;
            }

            return res;
        }

        private RObj Init(string argStrIpDest)
        {
            if (init_MC_UDP == 0) return new RObj(0);

            strIpDest = argStrIpDest;

            try
            {
                sSocket_UDP = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString() + "\n" + ex.Message);
                return new RObj(0xC0000001L);
            }

            try
            {
                sSocket_UDP.ReceiveTimeout = 50; // tv_sec = 25
            }
            catch (SocketException ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString() + "\n" + ex.Message);
                return new RObj(0xC0000003L);
            }

            try
            {
                sSocket_UDP.SendTimeout = 50; // time_out.tv_usec = 500;
            }
            catch (SocketException ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString() + "\n" + ex.Message);
                return new RObj(0xC0000004L);
            }

            Send_NU_UDP();
            init_MC_UDP = Read_NU_UDP().Code;

            return new RObj(init_MC_UDP);
        }

        private RObj Send_NU_UDP()
        {
            IPEndPoint ipDest;

            try
            {
                ipDest = new IPEndPoint(IPAddress.Parse(strIpDest), usPort_MKIO);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString() + "\n" + ex.Message);
                return new RObj(0xC0000005L);
            }

            sSocket_UDP.SendTo(NU_MIL_BC, ipDest);

            return new RObj(0);
        }
        private RObj Read_NU_UDP()
        {
            int result;
            EndPoint remoteIP;

            try
            {
                remoteIP = new IPEndPoint(IPAddress.Any, 0);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString() + "\n" + ex.Message);
                return new RObj(0xC0000005L);
            }
            
            byte[] bufferReceive = new byte[1460];
            StringBuilder str = new StringBuilder();

            result = sSocket_UDP.ReceiveFrom(bufferReceive, ref remoteIP);

            if (result > 0)
            {
                str.Append(Encoding.Unicode.GetString(bufferReceive, 0, result));
                return new RObj(0);
            }
            else return new RObj(0xC0000008L);
        }

        private RObj Deinit()
        {
            if (init_MC_UDP != 0) return new RObj(0xC0000007L);

            sSocket_UDP.Close();
            init_MC_UDP = 0xC0000007L;

            return new RObj(init_MC_UDP);
        }

        private RObj InitCom_Kmbo(string argStrIpDest)
        {
            RObj result = new RObj();
            EndPoint ipAddr;

            if (init_MC_Kmbo == 0) return result;

            strIpDest = argStrIpDest;

            try
            {
                sSocket_KMBO = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString() + "\n" + ex.Message);
                return new RObj(0xC0000001L);
            }

            try
            {
                ipAddr = new IPEndPoint(IPAddress.Any, 0x5003);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString() + "\n" + ex.Message);
                return new RObj(0xC0000005L);
            }

            try
            {
                sSocket_KMBO.Bind(ipAddr);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString() + "\n" + ex.Message);
                return new RObj(0xC0000002L);
            }

            /*try
            {
                sSocket_KMBO.Blocking = false; 
            }
            catch (SocketException ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString() + "\n" + ex.Message);
                return -140;
            }*/

            //result = ioctlsocket(sSocket_KMBO, FIONBIO, (unsigned long *) & l);

            init_MC_Kmbo = 0;

            return result;
        }

        private RObj DeinitCom_Kmbo()
        {
            if (init_MC_Kmbo != 0) return new RObj(0xC0000007L);

            sSocket_KMBO.Close();

            init_MC_Kmbo = 0xC0000007L;

            return new RObj(init_MC_Kmbo);
        }

        static public byte[] StructToByteArray(ArrayList arr)
        {
            object[] obj = (arr.ToArray());

            int sizeInBytes;

            byte[] outByte = new byte[0];

            for (int i = 0; i < obj.Count(); i++)
            {
                sizeInBytes = Marshal.SizeOf(obj[i]);

                byte[] outArray = new byte[sizeInBytes];

                IntPtr ptr = Marshal.AllocHGlobal(sizeInBytes);

                Marshal.StructureToPtr(obj[i], ptr, true);
                Marshal.Copy(ptr, outArray, 0, sizeInBytes);
                Marshal.FreeHGlobal(ptr);
                outByte = outByte.Concat(outArray).ToArray();
            }
            return outByte;
        }
    }
}
