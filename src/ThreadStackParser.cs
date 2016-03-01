using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Diagnostics.Runtime;

namespace ProcDiag
{
    internal class ThreadStackParser
    {
        // sample:
        // Child-SP         RetAddr          Call Site
        // Child SP IP       Call Site
        // ESP       EIP

        // 0f81f2f0 6a56c93b [InlinedCallFrame: 0f81f2f0] System.Net.UnsafeNclNativeMethods+OSSOCK.recvfrom(IntPtr, Byte*, Int32, System.Net.Sockets.SocketFlags, Byte[], Int32 ByRef)
        // 0f81f6fc 6b4421bb [GCFrame: 0f81f6fc]
        // 0f81f9c0 6b4421bb [DebuggerU2MCatchHandlerFrame: 0f81f9c0]
        // 2f08ec5c 2a0eaf33 JetBrains.Threading.JetDispatcher+<>c__DisplayClass5.<CreateDispatcherThread>b__1()
        // 000000002dc4e6e0 000007fef33913b2 System.Threading.ThreadHelper.ThreadStart(System.Object)
        private static readonly Regex StackLineRegEx =
            new Regex(@"^[0-9a-fA-F]+\s+[0-9a-fA-F]+\s+(?:\[[^\]]*\]\s*)?([^ \[].*)$",
                RegexOptions.Compiled | RegexOptions.RightToLeft);

        private readonly List<string> _output = new List<string>();
        private readonly StringBuilder _thread = new StringBuilder();
        private int _lineParsed;

        public ThreadStackParser(ClrThread thread)
        {
            _thread.AppendFormat("Thread Id: {0:X} ({1}) ", thread.OSThreadId, thread.ManagedThreadId);
        }

        public void ParseLine(string line)
        {
            if (String.IsNullOrEmpty(line)) return;

            if (line.StartsWith("OS Thread "))
            {
                _output.Add(_thread.ToString());
                _lineParsed = 0;
                _thread.Length = 0;
                _thread.Append(line.Substring(3));
            }
            else
            {
                Match ret = StackLineRegEx.Match(line);
                if (!ret.Success)
                    return;
                if (_lineParsed > 200)
                {
                    // Avoid building too large stack traces.
                    return;
                }
                _thread.Append(' ').Append(ret.Groups[1].Value);
                _lineParsed++;
                if (_lineParsed > 200)
                {
                    _thread.Append(" stack trace truncated.");
                }
            }
        }

        public IEnumerable<string> GetOutput()
        {
            if (_lineParsed == 0) return Enumerable.Empty<string>();

            _output.Add(_thread.ToString());
            return _output;
        }
    }
}