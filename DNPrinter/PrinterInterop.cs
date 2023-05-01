using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DNPrinter
{
    /* This needs cleaning up -- but who's got time for that crap? I mostly ripped the wrapper and modified it. It provides an alternative to pointer arithmetic though...*/
    /*
    typedef struct _REMOTE_NAME_INFO
    {
        LPTSTR lpUniversalName;
        LPTSTR lpConnectionName;
        LPTSTR lpRemainingPath;
    } REMOTE_NAME_INFO;
    */

    [StructLayout(LayoutKind.Sequential)]
    struct _REMOTE_NAME_INFO
    {
        public IntPtr lpUniversalName;
        public IntPtr lpConnectionName;
        public IntPtr lpRemainingPath;
    }

    public struct RemoteNameInfo
    {
        public string universalName;
        public string connectionName;
        public string remainingPath;
    }

    public class WNet
    {
        const int UNIVERSAL_NAME_INFO_LEVEL = 0x00000001;
        const int REMOTE_NAME_INFO_LEVEL = 0x00000002;

        const int ERROR_MORE_DATA = 234;
        const int ERROR_NOT_CONNECTED = 2250;
        const int NOERROR = 0;

        [DllImport("mpr.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U4)]
        private static extern int WNetGetUniversalNameW(
            string lpLocalPath,
            [MarshalAs(UnmanagedType.U4)] int dwInfoLevel,
            IntPtr lpBuffer,
            [MarshalAs(UnmanagedType.U4)] ref int lpBufferSize);


        public static RemoteNameInfo GetRemoteNameInfo(string localPath)
        {
            // The return value.
            RemoteNameInfo retVal;
            _REMOTE_NAME_INFO rni;

            // The pointer in memory to the structure.
            IntPtr buffer = IntPtr.Zero;

            // Wrap in a try/catch block for cleanup.
            try
            {
                // First, call WNetGetUniversalName to get the size.
                int size = 0;

                // Make the call.
                // Pass IntPtr.Size because the API doesn't like null, even though
                // size is zero.  We know that IntPtr.Size will be
                // aligned correctly.
                int apiRetVal = WNetGetUniversalNameW(localPath, REMOTE_NAME_INFO_LEVEL, (IntPtr)IntPtr.Size, ref size);

                //  if the return value is ERROR_NOT_CONNECTED, then
                //  this is a local path
                if (apiRetVal == ERROR_NOT_CONNECTED)
                {
                    retVal = new RemoteNameInfo();
                    retVal.connectionName = Path.GetPathRoot(localPath);
                    retVal.remainingPath = localPath.Substring(Path.GetPathRoot(localPath).Length);
                    retVal.universalName = localPath;
                    return retVal;
                }

                // If the return value is not ERROR_MORE_DATA, then
                // raise an exception.
                if (apiRetVal != ERROR_MORE_DATA)
                    // Throw an exception.
                    throw new System.ComponentModel.Win32Exception();

                // Allocate the memory.
                buffer = Marshal.AllocCoTaskMem(size);

                // Now make the call.
                apiRetVal = WNetGetUniversalNameW(localPath, REMOTE_NAME_INFO_LEVEL, buffer, ref size);

                // If it didn't succeed, then throw.
                if (apiRetVal != NOERROR)
                    // Throw an exception.
                    throw new System.ComponentModel.Win32Exception();

                // Now get the string.  It's all in the same buffer, but
                // the pointer is first, so offset the pointer by IntPtr.Size
                // and pass to PtrToStringAuto.
                //retVal = Marshal.PtrToStringAuto(new IntPtr(buffer.ToInt64() + IntPtr.Size));

                rni = (_REMOTE_NAME_INFO)Marshal.PtrToStructure(buffer, typeof(_REMOTE_NAME_INFO));

                retVal.connectionName = Marshal.PtrToStringAuto(rni.lpConnectionName);
                retVal.remainingPath = Marshal.PtrToStringAuto(rni.lpRemainingPath);
                retVal.universalName = Marshal.PtrToStringAuto(rni.lpUniversalName);

                return retVal;
            }
            finally
            {
                // Release the buffer.
                Marshal.FreeCoTaskMem(buffer);
            }

            // First, allocate the memory for the structure.
            // That's all folks.
        }
    }


    public static class PrinterInterop
    {
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetDefaultPrinter(string Name);

    }

    public static class WinInteropInterop
    {
        const int UNIVERSAL_NAME_INFO_LEVEL = 0x00000001;
        const int REMOTE_NAME_INFO_LEVEL = 0x00000002;

        const int ERROR_MORE_DATA = 234;
        const int NOERROR = 0;

        [DllImport("mpr.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U4)]
        static extern int WNetGetUniversalName(
                string lpLocalPath,
                [MarshalAs(UnmanagedType.U4)] int dwInfoLevel,
                IntPtr lpBuffer,
                [MarshalAs(UnmanagedType.U4)] ref int lpBufferSize);

        public static string GetUniversalName(string localPath)
        {
            // The return value.
            string retVal = null;

            // The pointer in memory to the structure.
            IntPtr buffer = IntPtr.Zero;

            // Wrap in a try/catch block for cleanup.
            try
            {
                // First, call WNetGetUniversalName to get the size.
                int size = 0;

                // Make the call.
                // Pass IntPtr.Size because the API doesn't like null, even though
                // size is zero.  We know that IntPtr.Size will be
                // aligned correctly.
                int apiRetVal = WNetGetUniversalName(localPath, UNIVERSAL_NAME_INFO_LEVEL, (IntPtr)IntPtr.Size, ref size);

                // If the return value is not ERROR_MORE_DATA, then
                // raise an exception.
                if (apiRetVal != ERROR_MORE_DATA)
                    // Throw an exception.
                    throw new Win32Exception(apiRetVal);

                // Allocate the memory.
                buffer = Marshal.AllocCoTaskMem(size);

                // Now make the call.
                apiRetVal = WNetGetUniversalName(localPath, UNIVERSAL_NAME_INFO_LEVEL, buffer, ref size);

                // If it didn't succeed, then throw.
                if (apiRetVal != NOERROR)
                    // Throw an exception.
                    throw new Win32Exception(apiRetVal);

                // Now get the string.  It's all in the same buffer, but
                // the pointer is first, so offset the pointer by IntPtr.Size
                // and pass to PtrToStringAnsi.
                retVal = Marshal.PtrToStringAuto(new IntPtr(buffer.ToInt64() + IntPtr.Size), size);
                retVal = retVal.Substring(0, retVal.IndexOf('\0'));
            }
            finally
            {
                // Release the buffer.
                Marshal.FreeCoTaskMem(buffer);
            }

            // First, allocate the memory for the structure.

            // That's all folks.
            return retVal;
        }
    }
}
