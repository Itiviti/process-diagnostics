using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Diagnostics.Runtime;
using static System.String;

namespace ProcDiag
{
    internal class ThreadStackParser
    {
        private readonly ThreadData _thread = new ThreadData();
        private int _lineParsed = 0;

        public ThreadStackParser(ClrThread thread)
        {
            _thread.Name = $"Thread Id: {thread.OSThreadId:X} ({thread.ManagedThreadId}) ";
        }

        public void ParseLine(string line)
        {
            if (IsNullOrEmpty(line)) return;

            _lineParsed++;
            if (_lineParsed > 200)
            {
                _thread.Append("stack trace truncated.");
            }
            else
            {
                _thread.Append(line);
            }
        }

        public ThreadData GetOutput()
        {
            return _thread;
        }
    }

    internal class ThreadData
    {
        private readonly List<string> _stackTrace = new List<string>();

        public string Name { get; set; }

        public void Append(string line)
        {
            _stackTrace.Add(line);
        }

        public override string ToString()
        {
            if (!_stackTrace.Any())
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Name);
            foreach (var line in _stackTrace)
            {
                sb.AppendLine(line);
            }
            return sb.ToString();
        }
    }
}