using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Runtime.Interop;

namespace ProcDiag
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Options options = new Options();

            if (!CommandLine.Parser.Default.ParseArguments(args, options))
                return;

            Dictionary<ClrType, HeapStatsEntry> stats = null;
            IEnumerable<string> threads = null;
            using (var dataTarget = DataTarget.AttachToProcess(options.ProcessId, 5000, AttachFlag.Invasive))
            {
                ClrInfo version = dataTarget.ClrVersions.FirstOrDefault();
                if (version != null)
                {
                    var runtime = version.CreateRuntime();
                    threads = ThreadsDump(runtime);
                    if (!options.ThreadsOnly) stats = HeapDump(runtime);
                }

                if (!options.ThreadsOnly)
                {
                    IDebugClient client = dataTarget.DebuggerInterface;
                    client.WriteDumpFile(Path.Combine(options.OutputFolder?? Environment.CurrentDirectory, GetDumpFileName(options.ProcessId)), DEBUG_DUMP.DEFAULT);
                }
            }

            if (threads != null)
            {
                Console.WriteLine("Thread dump:");
                foreach (var thread in threads)
                {
                    Console.WriteLine(thread);
                }
                Console.WriteLine("Thread dump finished");
            }

            if (stats != null)
            {
                Console.WriteLine("Heap stats:");
                Console.WriteLine("{0,12} {1,12} {2}", "Size", "Count", "Type");
                foreach (var entry in from entry in stats.Values orderby entry.Size select entry)
                    Console.WriteLine("{0,12:n0} {1,12:n0} {2}", entry.Size, entry.Count, entry.Name);
                Console.WriteLine("Heap stats finished");
            }
        }

        private static IEnumerable<string> ThreadsDump(ClrRuntime runtime)
        {
            List<string> threads = new List<string>();
            foreach (ClrThread thread in runtime.Threads.Where(thread => thread.IsAlive))
            {
                ThreadStackParser parser = new ThreadStackParser(thread);
                foreach (ClrStackFrame frame in thread.StackTrace)
                    parser.ParseLine(
                        String.Format("{0,12:X} {1,12:X} {2} ", frame.StackPointer, frame.InstructionPointer, frame)
                            .Trim());
                threads.AddRange(parser.GetOutput());
            }
            return threads;
        }

        private static Dictionary<ClrType, HeapStatsEntry> HeapDump(ClrRuntime runtime)
        {
            Dictionary<ClrType, HeapStatsEntry> stats = new Dictionary<ClrType, HeapStatsEntry>();
            ClrHeap heap = runtime.GetHeap();
            foreach (ClrSegment seg in heap.Segments)
            {
                for (ulong obj = seg.FirstObject; obj != 0; obj = seg.NextObject(obj))
                {
                    ClrType type = heap.GetObjectType(obj);
                    if (type == null) continue;

                    HeapStatsEntry entry;
                    if (!stats.TryGetValue(type, out entry))
                    {
                        entry = new HeapStatsEntry {Name = type.Name};
                        stats[type] = entry;
                    }
                    entry.Count++;
                    entry.Size += type.GetSize(obj);
                }
            }

            return stats;
        }

        private static string GetDumpFileName(int pid)
        {
            var process = System.Diagnostics.Process.GetProcessById(pid);
            var time = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            return String.Format("{0}-dump-{1}.dmp", process.ProcessName, time);
        }
    }
}