using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Runtime.Interop;
using System.Text;

namespace ProcDiag
{

    internal class Dumper
    {
        public static void Start(Options options, Process process, IWriter outWriter)
        {
            Dictionary<ClrType, HeapStatsEntry> stats = null;
            IEnumerable<ThreadData> threads = null;
            using (var dataTarget = DataTarget.AttachToProcess(process.Id, 5000, AttachFlag.Invasive))
            {
                ClrInfo version = dataTarget.ClrVersions.FirstOrDefault();
                if (version != null)
                {
                    var runtime = version.CreateRuntime();
                    if (options.DumpThreads)
                        threads = ThreadsDump(runtime);

                    if (options.DumpStats)
                        stats = HeapDump(runtime);
                }

                if (options.FullDump)
                {
                    IDebugClient client = dataTarget.DebuggerInterface;
                    var fileName = Path.Combine(options.OutputFolder ?? Environment.CurrentDirectory, GetDumpFileName(process));
                    outWriter.WriteHint($"Writing memory dump to: {fileName}");
                    client.WriteDumpFile(fileName, DEBUG_DUMP.DEFAULT);
                }
            }

            if (threads != null)
            {
                outWriter.WriteHint("Thread dump:");
                foreach (var thread in threads)
                {
                    outWriter.WriteLine(thread.ToString(), "thread");
                }
                outWriter.WriteHint("Thread dump finished.");
            }

            if (stats != null)
            {
                outWriter.WriteHint("Heap stats:");
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("{0,12} {1,12} {2}", "Size", "Count", "Type"));
                foreach (var entry in from entry in stats.Values orderby entry.Size select entry)
                    sb.AppendLine(string.Format("{0,12:n0} {1,12:n0} {2}", entry.Size, entry.Count, entry.Name));
                outWriter.WriteLine(sb.ToString(), "Heap");
                outWriter.WriteHint("Heap stats finished.");
            }
        }

        private static IEnumerable<ThreadData> ThreadsDump(ClrRuntime runtime)
        {
            List<ThreadData> threads = new List<ThreadData>();
            foreach (ClrThread thread in runtime.Threads.Where(thread => thread.IsAlive))
            {
                var parser = new ThreadStackParser(thread);

                foreach (ClrStackFrame frame in thread.StackTrace)
                    parser.ParseLine(frame.ToString());

                threads.Add(parser.GetOutput());
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
                        entry = new HeapStatsEntry { Name = type.Name };
                        stats[type] = entry;
                    }
                    entry.Count++;
                    entry.Size += type.GetSize(obj);
                }
            }

            return stats;
        }

        private static string GetDumpFileName(Process process)
        {
            var time = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            return $"{process.ProcessName}-dump-{time}.dmp";
        }
    }
}