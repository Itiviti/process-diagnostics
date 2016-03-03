using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ProcDiag
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Options options = new Options();

            if (!CommandLine.Parser.Default.ParseArguments(args, options))
                return;

            var process = Process.GetProcessById(options.ProcessId);
            if (process == null)
                throw new ArgumentException($"Process with pid: {options.ProcessId} not found.");

            if (RedirectToX86(process, args))
                return;

            Dumper.Start(options, process, Console.Out);
        }

        private static bool RedirectToX86(Process process, string[] args)
        {
            if (IntPtr.Size == 8 && IsWin64Emulator(process))
            {
                var processStartInfo = new ProcessStartInfo(@"procdiag.x86.exe", string.Join(" ", args))
                {
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };

                var wrapperProcess = Process.Start(processStartInfo);
                Console.Out.Write(wrapperProcess.StandardOutput.ReadToEnd());
                Console.Error.Write(wrapperProcess.StandardError.ReadToEnd());
                return true;
            }

            return false;
        }

        private static bool IsWin64Emulator(Process process)
        {
            if ((Environment.OSVersion.Version.Major > 5)
                || ((Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor >= 1)))
            {
                bool retVal;

                return NativeMethods.IsWow64Process(process.Handle, out retVal) && retVal;
            }

            return false; // not on 64-bit Windows Emulator
        }

        internal static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool IsWow64Process([In] IntPtr process, [Out] out bool wow64Process);
        }
    }
}