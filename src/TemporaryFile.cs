using System;
using System.IO;

namespace ProcDiag
{
    public class TemporaryFile : IDisposable
    {
        private readonly string _location;

        public TemporaryFile(string location, byte[] content)
        {
            _location = location;
            File.WriteAllBytes(location, content);
        }

        public void Dispose()
        {
            File.Delete(_location);
        }
    }
}