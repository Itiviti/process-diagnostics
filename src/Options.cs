using CommandLine;
using CommandLine.Text;

namespace ProcDiag
{
    internal class Options
    {
        [Option('p', "pid", Required = true, HelpText = "Process id.")]
        public int ProcessId { get; set; }

        [Option('o', "out", HelpText = "The folder where minidump will be created.")]
        public string OutputFolder { get; set; }

        [VerbOption("threadsonly", HelpText = "Only dump threads.")]
        public bool ThreadsOnly { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
                current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}