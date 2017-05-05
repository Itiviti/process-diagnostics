using System.IO;

namespace ProcDiag
{
    public class StandardWriter : IWriter
    {
        TextWriter _writer;

        public StandardWriter(TextWriter writer)
        {
            _writer = writer;
        }

        public void Dispose()
        {
            
        }

        public void WriteHint(string line)
        {
            _writer.WriteLine(line);
        }

        public void WriteLine(string line)
        {
            _writer.WriteLine(line);
        }

        public void WriteLine(string line, string label)
        {
            _writer.WriteLine(line);
        }
    }
}