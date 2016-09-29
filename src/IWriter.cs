namespace ProcDiag
{
    public interface IWriter
    {
        void WriteLine(string line);

        void Write(string line);
    }
}