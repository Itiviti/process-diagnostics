using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Diagnostics.Runtime;

namespace ProcDiag
{
	public class DumpWorker
    {
        public void DumpThreadStacks(IList<Tuple<int, string>> threads)
        {
            if (threads.Count == 0)
                return;

            try
            {
                _clrRuntime.Value.Flush();
                foreach (var stack in threads.Select(i => DumpThreadStack(i.Item1, i.Item2, _clrRuntime.Value)))
                {
                    Console.WriteLine(stack);
                }
            }
            catch (Exception ex)
            {
                ConsoleMixins.WriteError(ex.Message);
            }
        }

        private readonly Lazy<ClrRuntime> _clrRuntime = new Lazy<ClrRuntime>(AttachToProcess);
        private static int _id;

        private static ClrRuntime AttachToProcess()
        {
            return DataTarget.AttachToProcess(_id, msecTimeout: 5000, attachFlag: AttachFlag.Passive).ClrVersions[0].CreateRuntime();
        }

        private string DumpThreadStack(int managedThreadId, string prefix, ClrRuntime clrRuntime)
        {
            const string noStack = "No stacktrace available";

            var runtimeThread = clrRuntime.Threads
                .FirstOrDefault(th => th.ManagedThreadId == managedThreadId);
            var callstack = runtimeThread?.StackTrace == null
                ? noStack
                : string.Join("\t", runtimeThread.StackTrace);

            return prefix + callstack;
        }

        public void Attach(int processId)
        {
            _id = processId;
        }
    }
}
