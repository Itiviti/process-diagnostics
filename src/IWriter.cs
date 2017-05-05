using System;

namespace ProcDiag
{
    public interface IWriter : IDisposable
    {
        void WriteLine(string line);
        void WriteLine(string line, string label);
        void WriteHint(string line);
    }
}