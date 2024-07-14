using System;
using System.IO;
using System.Runtime.InteropServices;

namespace pbn.dds;


public static class DdsInterop
{
    private static IntPtr DllHandle;

    static DdsInterop()
    {

            string dllFileName;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                dllFileName = "dds.dll";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                dllFileName = "libdds.so";
            }
            else
            {
                throw new PlatformNotSupportedException("Unsupported platform");
            }

            var pathsToCheck = new string?[] { Environment.GetEnvironmentVariable("DDS_LIB_PATH"), AppDomain.CurrentDomain.BaseDirectory };
            var success = false;
            string dllPath = "";
            foreach (var libPath in pathsToCheck)
            {
                if (string.IsNullOrEmpty(libPath))
                {
                    continue;
                }

                dllPath = Path.Combine(libPath, dllFileName);
                if (File.Exists(dllPath))
                {
                    success = true;
                    break;
                }
            }
            if (!success && File.Exists(dllFileName))
            {
                dllPath = dllFileName;
                success = true;
            }
             if(!success)
                {
                    throw new DllNotFoundException($"Unable to find the DLL: {dllFileName}");
                }
            

        DllHandle = NativeLibrary.Load(dllPath);
        CalcAllTablesPBNFunction = Marshal.GetDelegateForFunctionPointer<CalcAllTablesPBNDelegate>(NativeLibrary.GetExport(DllHandle, "CalcAllTablesPBN"));
        SetThreadingFunction = Marshal.GetDelegateForFunctionPointer<SetThreadingDelegate>(
        NativeLibrary.GetExport(DllHandle, "SetThreading"));
        SetMaxThreadsFunction = Marshal.GetDelegateForFunctionPointer<SetMaxThreadsDelegate>(
            NativeLibrary.GetExport(DllHandle, "SetMaxThreads"));

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // On Linux and OSX, we need to call this function to initialize the library
            // I should be sending a love letter to Tim Anderson for this one, see
            // https://www.itwriting.com/blog/12169-fixing-a-net-p-invoke-issue-on-apple-silicon-including-a-tangle-with-xcode.html
            SetMaxThreads(0);
        }
    }

    private delegate int CalcAllTablesPBNDelegate(
        ref DdsTypes.ddTableDealsPBN dealsp,
        int mode,
        int[] trumpFilter,
        ref DdsTypes.ddTablesRes resp,
        ref DdsTypes.allParResults presp);

    private static CalcAllTablesPBNDelegate CalcAllTablesPBNFunction;

     /// <summary>
    ///     Call DDS to calculate double dummy and par results.
    ///     See DDS docs for details.
    /// </summary>
    public static int CalcAllTablesPBN(
        ref DdsTypes.ddTableDealsPBN dealsp,
        int mode,
        int[] trumpFilter,
        ref DdsTypes.ddTablesRes resp,
        ref DdsTypes.allParResults presp)
    {
        return CalcAllTablesPBNFunction(ref dealsp, mode, trumpFilter, ref resp, ref presp);
    }

         // Delegate and import for SetThreading
    private delegate int SetThreadingDelegate(int code);

    private static SetThreadingDelegate SetThreadingFunction;

    private static int SetThreading(int code)
    {
        return SetThreadingFunction(code);
    }

     // Delegate and import for SetMaxThreads
        private delegate void SetMaxThreadsDelegate(int userThreads);

        private static SetMaxThreadsDelegate SetMaxThreadsFunction;

        public static void SetMaxThreads(int userThreads)
        {
            SetMaxThreadsFunction(userThreads);
        }
}