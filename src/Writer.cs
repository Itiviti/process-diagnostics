using System.IO;

namespace ProcDiag
{
    public class Writer : IWriter
    {
        private readonly TextWriter _writer;

        public Writer(TextWriter writer)
        {
            _writer = writer;
        }

        public void WriteLine(string line)
        {
            _writer.WriteLine(line);
        }

        public void Write(string line)
        {
            _writer.Write(line);
        }
    }
}