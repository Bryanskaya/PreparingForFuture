using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Errs;
using _Kernel32;

namespace Kmbo
{
    public class GlobalVar
    {
        public const int KMBO_MAXIMUM_WORDS = 64;       //Максимальное количество 16-ти разрядных слов для приема\передачи
        public static int Type_Device = -1;
    }

    public class TKMBO : TKernel32
    {
        public const UInt32 GENERIC_READ = 0x80000000;
        public const UInt32 GENERIC_WRITE = 0x80000000;
        public const UInt16 OPEN_EXISTING = 3;
        const UInt16 CREATE_NEW = 1;
        public const UInt16 NULL = 0;
        public const UInt16 FILE_SHARE_READ = 0;
        public const UInt16 FILE_SHARE_WRITE = 0;
        public const UInt16 FILE_ATTRIBUTE_NORMAL = 0;
        public const UInt16 FIRST_IOCTL_INDEX = 0x0800;
        public const UInt16 FILE_ANY_ACCESS = 0x0000;
        public const UInt32 FILE_DEVICE_KMBO = 0x00008000;
        public const UInt16 METHOD_BUFFERED = 0;
        const UInt16 METHOD_IN_DIRECT = 1;
        const UInt16 METHOD_OUT_DIRECT = 2;
        const UInt16 METHOD_NEITHER = 3;
        const UInt16 REGIM_REST = 0;                                         //Режим "Покой"
        const UInt16 REGIM_CONTROLLER = 0x1;                                 //Режим "Контроллер"
    }

    public class TKmboRead
    {
        public UInt16 address;                          //Адрес ОУ
        public UInt32 count;                            //Количество слов, которые нужно прочитать 1..64
        public UInt16 synchro;                          //Если 0, то сигнал синхронизации на осциллограф не выводится
        public UInt16[] lpBuffer = new UInt16[GlobalVar.KMBO_MAXIMUM_WORDS];

        public object Clone()
        {
            UInt16[] temp = new UInt16[this.lpBuffer.Length];
            Array.Copy(this.lpBuffer, temp, this.lpBuffer.Length);

            return new TKmboRead
            {
                address = this.address,
                count = this.count,
                synchro = this.synchro,
                lpBuffer = temp
            };
        }
    }

    public class TKmboWrite                             //Структура данных для записи
    {
        public UInt16 address;                          //Адрес ОУ
        public UInt32 count;                            //Количество записываемых слов 1..64
        public UInt16 synchro;                          //Если 0, то сигнал синхронизации на осциллограф не выводится
        public UInt16[] lpBuffer = new UInt16[GlobalVar.KMBO_MAXIMUM_WORDS];    //Массив записываемых слов

        public object Clone()
        {
            UInt16[] temp = new UInt16[this.lpBuffer.Length];
            Array.Copy(this.lpBuffer, temp, this.lpBuffer.Length);

            return new TKmboWrite
            {
                address = this.address,
                count = this.count,
                synchro = this.synchro,
                lpBuffer = temp
            };
        }
    }

    public class Ukmbo
    {
        Ukmbo_mc UkmboMC = new Ukmbo_mc();
        Ukmbo_usb UkmboUSB = new Ukmbo_usb();
        Ukmbo_pci UkmboPCI = new Ukmbo_pci();
        public int KmboOpen(ref IntPtr pHandle)
        {
            Errs_mc.Init();

            if (UkmboMC.MC_Open().Code == 0)
            {
                pHandle = (IntPtr)null;
                GlobalVar.Type_Device = 2;
            }
            else
            {
                ErrsUSB.Init();

                if (UkmboUSB.USB_Open().Code == 0)
                    GlobalVar.Type_Device = 1;
                else
                {
                    ErrsPCI.Init();

                    if (UkmboPCI.PCI_Open(ref pHandle).Code == 0)
                        GlobalVar.Type_Device = 0;
                    else
                        GlobalVar.Type_Device = -1;
                }
            }

            return GlobalVar.Type_Device;
        }

        public RObj KmboClose(IntPtr handle)
        {
            RObj res = new RObj();

            switch (GlobalVar.Type_Device)
            {
                case 0:
                    res.Code = UkmboPCI.PCI_Close(handle).Code;
                    break;
                case 1:
                    res.Code = UkmboUSB.USB_Close().Code;
                    break;
                case 2:
                    res.Code = UkmboMC.MC_Close().Code;
                    break;
            }

            return res;
        }

        public RObj KmboWrite(IntPtr handle, ref TKmboWrite lpWrite)
        {
  
            RObj res = new RObj();

            switch (GlobalVar.Type_Device)
            {
                case 0:
                    res.Code = UkmboPCI.PCI_Write(handle, ref lpWrite).Code;
                    break;
                case 1:
                    res.Code = UkmboUSB.USB_Write(lpWrite).Code;
                    break;
                case 2:
                    res.Code = UkmboMC.MC_Write(lpWrite).Code;
                    break;
            }

            return res;
        }

        public RObj KmboRead(IntPtr handle, ref TKmboRead lpRead)
        {
            RObj res = new RObj();

            switch (GlobalVar.Type_Device)
            {
                case 0:
                    res.Code = UkmboPCI.PCI_Read(handle, ref lpRead).Code;
                    break;
                case 1:
                    res.Code = UkmboUSB.USB_Read(ref lpRead).Code;
                    break;
                case 2:
                    res.Code = UkmboMC.MC_Read(lpRead).Code;
                    break;
            }

            return res;
        }
    }
}
