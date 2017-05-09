using Microsoft.Diagnostics.Runtime;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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

            Process process = null;
            try
            {
                process = GetProcess(options.Process);
            }
            catch (ArgumentException ex)
            {
                ConsoleMixins.WriteError(ex.Message);
                Environment.ExitCode = -1;
                return;
            }


            if (RedirectToX86(process, Console.Out, Console.Error))
                return;

            if (!string.IsNullOrEmpty(options.OutputFolder) || (!options.DumpStats && !options.DumpThreads))
                options.FullDump = true;
            using (var output = GetOutput(options))
            try
            {
                Dumper.Start(options, process, output);
            }
            catch (ClrDiagnosticsException cex ) when(cex.HResult == (int)ClrDiagnosticsException.HR.DebuggerError)
            {
                ConsoleMixins.WriteError("Error attaching to process: {0}. Is another debugger attached?{3}Additional details: {2}", process.ProcessName, process.Id, cex.Message, Environment.NewLine);
                Environment.ExitCode = -1;
            }
}

        private static IWriter GetOutput(Options options)
        {
            if (options.Xml)
                return new XmlOutputdWriter(Console.Out);
            else
                return new StandardWriter(Console.Out);
        }

        private static Process GetProcess(string process)
        {
            int pid;
            if (int.TryParse(process, out pid))
                return Process.GetProcessById(pid);

            var processes = GetProcessByName(process).ToList();
            if (processes.Count == 0)
                throw new ArgumentException($"Process '{process}' not found!");

            if (processes.Count > 1)
                throw new ArgumentException($"Multiple processes with same name found! Consider using pid to connect. {Environment.NewLine}{process} pids: {GetPids(processes)}");
            

            return processes[0];
        }

        private static string GetPids(System.Collections.Generic.List<Process> processes)
        {
            return string.Join(", ", processes.Select(p => p.Id.ToString()));
        }

        private static Process[] GetProcessByName(string process)
        {
            if (process.ToLower().EndsWith(".exe"))
                process = process.Remove(process.Length - 4);
            return Process.GetProcessesByName(process);
        }

        private static bool RedirectToX86(Process process, TextWriter outWriter, TextWriter errorWriter)
        {
            if (IntPtr.Size != 8 || !IsWin64Emulator(process)) return false;

            string x86Wrapper = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "procdiag.x86.exe");
            using (new TemporaryFile(x86Wrapper, Resources.procdiag_x86))
            {
                var processStartInfo = new ProcessStartInfo(x86Wrapper, Environment.CommandLine)
                {
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };

                var wrapperProcess = Process.Start(processStartInfo);
                outWriter.WriteLine(wrapperProcess.StandardOutput.ReadToEnd());
                errorWriter.WriteLine(wrapperProcess.StandardError.ReadToEnd());
                wrapperProcess.WaitForExit();
                Environment.ExitCode = wrapperProcess.ExitCode;
            }

            return true;
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