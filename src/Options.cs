using CommandLine;
using CommandLine.Text;

namespace ProcDiag
{
    internal class Options
    {
        [Option('p', "pid", Required = true, HelpText = "Process id/name.")]
        public string Process { get; set; }

        [Option('o', "out", HelpText = "The folder where minidump will be created. Setting it implies --full option")]
        public string OutputFolder { get; set; }

        [VerbOption("threads", HelpText = "Dump thread callstacks.")]
        public bool DumpThreads { get; set; }

        [VerbOption("xml", HelpText = "XML output.")]
        public bool Xml { get; set; }

        [VerbOption("stats", HelpText = "Dump heap stats.")]
        public bool DumpStats { get; set; }

        [VerbOption("full", HelpText = "Create memory dump.")]
        public bool FullDump { get; set; }

        [VerbOption("dumpheapbytype", HelpText = "Dumps the objects of a given type from the managed heap.")]
        public string DumpHeapByType { get; set; }

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