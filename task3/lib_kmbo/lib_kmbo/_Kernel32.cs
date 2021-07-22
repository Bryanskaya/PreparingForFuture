using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace _Kernel32
{
    public class TKernel32
    {
        //библиотека Kernel32
        #region kernel32.dll
        [DllImport("kernel32.dll", SetLastError = true)] //CreateFile
        public static extern IntPtr CreateFile(string FileName, uint DesiredAccess, uint ShareMode, uint SecurityAttributes,
                                                      uint CreationDisposition, uint FlagsAndAttributes, int hTemplateFile);
        [DllImport("kernel32.dll", SetLastError = true)]//CloseHandle
        public static extern bool CloseHandle(IntPtr hObject);
        [DllImport("kernel32.dll", SetLastError = true)]//DeviceIoControl
        unsafe public static extern bool DeviceIoControl(IntPtr hFile, UInt32 Set_rejim, UInt16* lpInBuffer, UInt32 InSizeBuffer, UInt16* lpOutBuffer, UInt32 OutSizeBuffer, UInt32* name3, UInt32 name4);
        [DllImport("kernel32.dll", SetLastError = true)]//CreateEvent
        public static extern IntPtr CreateEvent(uint num, bool num1, bool num2, uint num3);
        [DllImport("kernel32.dll", SetLastError = true)]//WaitForSingleObject 
        public static extern uint WaitForSingleObject(IntPtr hObject, uint Milliseconds);
        [DllImport("kernel32.dll", SetLastError = true)]//WaitForMultipleObjects 
        public static extern uint WaitForMultipleObjects(uint nCount, IntPtr[] IpHandles, bool bWailAll, uint dwMilliseconds);
        [DllImport("kernel32.dll", SetLastError = true)]//ResetEvent
        public static extern bool ResetEvent(IntPtr hObject);
        [DllImport("kernel32.dll", SetLastError = true)]//GetLastError
        public static extern uint GetLastError();
        [DllImport("kernel32.dll", SetLastError = true)]//FormatMessage
        public static extern uint FormatMessage(FormatMessageFlags dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, ref IntPtr lpBuffer, uint nSize, IntPtr Arguments);

        [Flags]
        public enum FormatMessageFlags : uint
        {
            FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100,
            FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200,
            FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000,
            FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x00002000,
            FORMAT_MESSAGE_FROM_HMODULE = 0x00000800,
            FORMAT_MESSAGE_FROM_STRING = 0x00000400,
        }

        [DllImport("kernel32.dll")]//OutputDebugString
        public static extern void OutputDebugStringW(string String_Message);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern void OutputDebugString(string message);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        unsafe public static extern bool QueryPerformanceCounter(Int64* lpPerfomanceCounter);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        unsafe public static extern bool QueryPerformanceFrequency(Int64 *lpFrequency);
        #endregion
    }
}
