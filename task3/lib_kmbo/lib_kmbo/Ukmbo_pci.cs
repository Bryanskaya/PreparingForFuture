using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Kernel32;
using System.Runtime.InteropServices;
using Errs;

namespace Kmbo
{
    public class Ukmbo_pci
    {
        public class ConstPCI
        {
            //Допустимые значения поля TKmboSetRegim.Regim
            public const int KMBO_SET_REGIM_REST = 0;		    //Режим "Покой"
            public const int KMBO_SET_REGIM_CONTROLLER = 0x1;	//Режим "Контроллер"
        }
        public struct TKmboSetRegim
        {
            public ulong regim;                                 //Признак режима
        }
        public RObj PCI_Open(ref IntPtr pHandle)
        {
            TKmboSetRegim SetRegim;
            pHandle = TKernel32.CreateFile("\\\\.\\kmbo",
                                    TKMBO.GENERIC_READ | TKMBO.GENERIC_WRITE,
                                    TKMBO.FILE_SHARE_READ | TKMBO.FILE_SHARE_WRITE,
                                    TKMBO.NULL,
                                    TKMBO.OPEN_EXISTING,
                                    TKMBO.FILE_ATTRIBUTE_NORMAL,
                                    TKMBO.NULL);
            if (pHandle == (IntPtr)(-1))
                return new RObj(TKernel32.GetLastError());

            SetRegim.regim = ConstPCI.KMBO_SET_REGIM_CONTROLLER;
            return KmboSetRegim(pHandle, ref SetRegim);
        }

        public RObj PCI_Close(IntPtr handle)
        {
            RObj res = new RObj();

            if (handle != (IntPtr)(-1))                         //Если объект приемника таймерных прерываний был создан ранее - освобождаем его
            {
                if (TKernel32.CloseHandle(handle) == false)
                    res.Code = TKernel32.GetLastError();
                return res;
            }

            return res;
        }

        unsafe public RObj PCI_Write(IntPtr handle, ref TKmboWrite lpWrite)
        {
            UInt32 ret = 0;
            RObj res = new RObj();

            UInt32 lu_SetRegime = (UInt32)((TKMBO.FILE_DEVICE_KMBO << 16) | 
                                           (TKMBO.FILE_ANY_ACCESS << 14) |
                                           ((TKMBO.FIRST_IOCTL_INDEX + 1) << 2) | 
                                            (TKMBO.METHOD_BUFFERED)); //+1 attention

            if (handle != (IntPtr)(-1))
            {
                fixed (UInt16* Arr = &lpWrite.address)
                {
                    if (!TKernel32.DeviceIoControl(handle,
                            lu_SetRegime,
                            (UInt16*)Arr,
                            (UInt32)Marshal.SizeOf(typeof(TKmboWrite)),
                            null,
                            0,
                            &ret,
                            TKMBO.NULL))
                        res.Code = TKernel32.GetLastError();
                }
            }

            return res;
        }

        unsafe public RObj PCI_Read(IntPtr handle, ref TKmboRead lpRead)
        {
            UInt32 ret;
            RObj res = new RObj();

            UInt32 lu_SetRegime = (UInt32)((TKMBO.FILE_DEVICE_KMBO << 16) | 
                                           (TKMBO.FILE_ANY_ACCESS << 14) |
                                           ((TKMBO.FIRST_IOCTL_INDEX) << 2) | 
                                            (TKMBO.METHOD_BUFFERED)); 
            if (handle != (IntPtr)(-1))
            {
                fixed (UInt16* Arr = &lpRead.address)
                {
                    if (!TKernel32.DeviceIoControl(handle,
                            lu_SetRegime,
                            (UInt16*)Arr,
                            (UInt32)Marshal.SizeOf(typeof(TKmboRead)),
                            (UInt16*)Arr,
                            (UInt32)Marshal.SizeOf(typeof(TKmboRead)),
                            &ret,
                            TKMBO.NULL))
                        res.Code = TKernel32.GetLastError();
                }
            }

            return res;
        }

        unsafe RObj KmboSetRegim(IntPtr handle, ref TKmboSetRegim lpSetRegim)
        {
            UInt32 ret;
            RObj res = new RObj();

            if (handle != (IntPtr)(-1))
            {
                UInt32 lu_SetRegime = (UInt32)((TKMBO.FILE_DEVICE_KMBO << 16) | 
                                               (TKMBO.FILE_ANY_ACCESS << 14) |
                                               ((TKMBO.FIRST_IOCTL_INDEX + 5) << 2) | 
                                                (TKMBO.METHOD_BUFFERED));

                fixed (TKmboSetRegim* Arr = &lpSetRegim)
                {
                    if (!TKernel32.DeviceIoControl(handle,
                            lu_SetRegime,
                            (UInt16*)Arr,
                            (UInt32)Marshal.SizeOf(typeof(TKmboSetRegim)),
                            null,
                            0,
                            &ret,
                            TKMBO.NULL))
                        res.Code = TKernel32.GetLastError();
                }
            }
            return res;
        }
    }
}
