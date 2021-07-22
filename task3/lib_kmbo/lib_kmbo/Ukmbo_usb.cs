using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTD2XX_NET;
using _Kernel32;
using Errs;

namespace Kmbo
{
    class Ukmbo_usb
    {
        FTDI ftdi = new FTDI();

        public RObj USB_Open()
        {
            FTDI.FT_STATUS ftStatus;                                    //возвращаемые значения функций FTD2XX

            ftStatus = ftdi.OpenByDescription("USB <-> Serial Cable A");

            if (ftStatus == FTDI.FT_STATUS.FT_OK)                       //контроллер USB обнаружен
            {
                ftStatus = ftdi.ResetDevice();                          //Уст. уст-ва в исходн. состояние
                if (ftStatus != FTDI.FT_STATUS.FT_OK) return new RObj((ulong)ftStatus); 

                ftStatus = ftdi.SetBitMode(0xff, 0x00);                 //Mask = 0xff, Mode = 0x00;
                if (ftStatus != FTDI.FT_STATUS.FT_OK) return new RObj((ulong)ftStatus);   

                //Установка таймера времени ожидания
                ftStatus = ftdi.SetTimeouts(1, 1); // Lt = 0;
                if (ftStatus != FTDI.FT_STATUS.FT_OK) return new RObj((ulong)ftStatus);

                return new RObj(0);
            }
            //else return -1;
            else return new RObj(2);
        }

        public RObj USB_Close()
        {
            return new RObj((ulong)ftdi.Close());
        }

        public RObj USB_Write(TKmboWrite lpWrite)
        {
            UInt64 res = 0;
            Int16 Reg = 0;
            FTDI.FT_STATUS ftStatus;

            UInt16 Adr;
            UInt16 N;
            UInt16[] m = new ushort[62];
            bool s;

            Adr = lpWrite.address;
            N = (UInt16)lpWrite.count;

            Buffer.BlockCopy(lpWrite.lpBuffer, 0, m, 1, (int)(lpWrite.count * sizeof(UInt16)));

            if (lpWrite.synchro == 1) s = true;
            else s = false;

            UInt32 RxBytes;                             //число байт в очереди приема
            byte[] TxBuffer = new byte[256];            //передаваемые данные
            byte[] RxBuffer = new byte[2];              //принимаемые данные
            UInt32 dwBytesToWrite = 0;                  //число байт, запис. в у-во
            UInt32 dwBytesWritten = 0;                  //число байт, реально запис. в у-во
            UInt32 dwBytesReceived = 0;                 //число реально принятых байт
            
            //формирование заголовка
            if (N > 62) N = 62;
            m[0] = (UInt16)((Adr << 8) + (N << 1));

            UInt16 n = 0;
            UInt16 a;

            for (int i = 1; i <= 15; i++)               //формирование БЧЗ
            {
                a = (UInt16)(m[0] >> i);
                if ((a & 0x0001) == 0x0001) n++;
            }
            if (n % 2 == 0) m[0]++;

            //подготовка к передаче
            dwBytesToWrite = (UInt32)(N + 1) * 2 + 1;   //число байт
            Reg |= 1;                                   //пуск

            if (s == false) Reg &= ~2;
            else Reg |= 2;

            TxBuffer[0] = (byte)Reg;
            for (int i = 1; i <= ((N + 1) * 2); i++)
                if (i % 2 == 0)
                    TxBuffer[i] = (byte)(m[(i >> 1) - 1] & 0x00FF);
                else
                    TxBuffer[i] = (byte)(m[i >> 1] >> 8);
            
            //передача
            ftStatus = ftdi.Write(TxBuffer, dwBytesToWrite, ref dwBytesWritten);
            if (ftStatus != FTDI.FT_STATUS.FT_OK) return new RObj((ulong)ftStatus); 

            //прием слова состояния
            RxBytes = 1;
            ftStatus = ftdi.Read(RxBuffer, RxBytes, ref dwBytesReceived);
            if (ftStatus != FTDI.FT_STATUS.FT_OK) return new RObj((ulong)ftStatus);

            if (RxBytes != dwBytesReceived) res = 19;   //слово состояния не принято
            if ((RxBuffer[0] & 0xF9) != 0x20) res = 20; //ошибка слова состояния
            if ((RxBuffer[0] & 0x04) == 0x04) res = 21; //встречная работа
            if ((RxBuffer[0] & 0x02) == 0x02) res = 22; //ошибка бита четности

            return new RObj(res);
        }

        public RObj USB_Read(ref TKmboRead lpRead)
        {
            UInt64 res = 0;
            Int16 Reg = 0;
            FTDI.FT_STATUS ftStatus;

            UInt16 Adr;
            UInt16 N;
            UInt16[] m = new UInt16[62];
            bool s;

            Adr = lpRead.address;
            N = (UInt16)lpRead.count;
            if (lpRead.synchro == 1) s = true;
            else s = false;

            UInt32 RxBytes;                             //число байт в очереди приема
            byte[] TxBuffer = new byte[3];              //передаваемые данные
            byte[] RxBuffer = new byte[129];            //принимаемые данные
            UInt32 dwBytesToWrite = 0;                  //число байт, запис. в у-во
            UInt32 dwBytesWritten = 0;                  //число байт, реально запис. в у-во
            UInt32 dwBytesReceived = 0;                 //число реально принятых байт
                                  
            //формирование заголовка
            if (N > 63) N = 0;
            m[0] = (UInt16)((Adr << 8) + (N << 1) + 0x0080);

            UInt16 n = 0;
            UInt16 a;

            for (int i = 1; i <= 15; i++)               //формирование БЧЗ
            {
                a = (UInt16)(m[0] >> i);
                if ((a & 0x0001) == 0x0001) n++;
            }
            if (n % 2 == 0) m[0]++;

            //подготовка к передаче
            if (N == 0) N = 64;
            dwBytesToWrite = 3;                         //число байт
            Reg |= 1;                                   //пуск

            if (s == false) Reg &= ~2;
            else Reg |= 2;
            TxBuffer[0] = (byte)Reg;
            TxBuffer[1] = (byte)(m[0] >> 8);
            TxBuffer[2] = (byte)(m[0] & 0x00FF);

            //передача
            ftStatus = ftdi.Write(TxBuffer, dwBytesToWrite, ref dwBytesWritten);
            if (ftStatus != FTDI.FT_STATUS.FT_OK) return new RObj((ulong)ftStatus);

            //прием
            RxBytes = (UInt32)(2 * N + 1);
            ftStatus = ftdi.Read(RxBuffer, RxBytes, ref dwBytesReceived);
            if (ftStatus != FTDI.FT_STATUS.FT_OK) return new RObj((ulong)ftStatus);
            if (RxBytes != dwBytesReceived) res = 19;   //слово состояния не принято

            for (int i = 1; i <= N; i++)                //запись принятых данных в массив
                m[i] = (UInt16)((RxBuffer[2 * i - 2] << 8) +(RxBuffer[2 * i - 1] & 0x00FF));

            Buffer.BlockCopy(m, 1, lpRead.lpBuffer, 0, (int)(lpRead.count * sizeof(UInt16)));
            //memcpy(lpRead->lpBuffer, &m[1], lpRead->Count * sizeof(WORD));

            if ((RxBuffer[2 * N] & 0xF9) != 0x20) res = 20; //ошибка слова состояния
            if ((RxBuffer[2 * N] & 0x04) == 0x04) res = 21; //встречная работа
            if ((RxBuffer[2 * N] & 0x02) == 0x02) res = 22; //ошибка бита четности

            return new RObj(res);
        }
    }

}
