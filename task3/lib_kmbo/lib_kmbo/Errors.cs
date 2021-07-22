using System;
using System.Collections.Generic;
using _Kernel32;
using System.Runtime.InteropServices;
using System.Globalization;

namespace Errs
{
    public class BErr
    {
        public const int RUS = 0;
        public const int ENG = 1;

        public static Dictionary<UInt64, String[]> errs;

        public static String getByCode(UInt64 code, int lang)
        {
            if (errs.ContainsKey(code))
            {
                if (lang == RUS)
                    return "Код = " + code.ToString() + ". " + errs[code][1];
                else
                    return "Code = " + code.ToString() + ". " + errs[code][0];
            }
            else
            {
                IntPtr lpMsgBuf = IntPtr.Zero;

                if (TKernel32.FormatMessage(TKernel32.FormatMessageFlags.FORMAT_MESSAGE_ALLOCATE_BUFFER |
                                            TKernel32.FormatMessageFlags.FORMAT_MESSAGE_FROM_SYSTEM |
                                            TKernel32.FormatMessageFlags.FORMAT_MESSAGE_IGNORE_INSERTS,
                                            IntPtr.Zero,
                                            (uint)code,
                                            0,
                                            ref lpMsgBuf,
                                            0,
                                            IntPtr.Zero) == 0)
                    if (lang == RUS)
                        return "Код = " + code.ToString() + ". Ошибка не определена";
                    else
                        return "Code = " + code.ToString() + ". Not defined mistake";

                if (lang == RUS)
                    return "Код = " + code.ToString() + ". " + Marshal.PtrToStringAnsi(lpMsgBuf);
                else
                    return "Code = " + code.ToString() + ". " + Marshal.PtrToStringAnsi(lpMsgBuf);
            }
        }

        public static void Add(UInt64 code, String msgEng, String msgRus)
        {
            errs.Add(code, new String[2] { msgEng, msgRus });
        }
    }

    public class RObj
    {
        private UInt64 _code = 0;
        private String _msg;

        public UInt64 Code
        {
            get
            {
                return _code;
            }

            set
            {
                _code = value;
                _msg = BErr.getByCode(_code, BErr.ENG);
            }
        }

        public String Msg
        {
            get
            {
                return _msg;
            }
        }

        public RObj()
        {
            _msg = BErr.getByCode(_code, BErr.ENG);
        }

        public RObj(UInt64 code)
        {
            this._code = code;
            _msg = BErr.getByCode(code, BErr.ENG);
        }

        /*public RObj(int lang)
        {
            _msg = BErr.getByCode(_code, lang);
        }*/

        public RObj(UInt64 code, int lang)
        {
            this._code = code;
            _msg = BErr.getByCode(code, lang);
        }
    }
}
