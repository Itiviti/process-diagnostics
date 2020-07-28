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

        [VerbOption("passive", HelpText = "Use passive attach to process instead of NonInvasive which is the default. Target process will not be paused but results might be inaccurate.")]
        public bool PassiveAttach { get; set; }

        [VerbOption("dumpheapbytype", HelpText = "Dumps the objects of a given type from the managed heap.")]
        public string DumpHeapByType { get; set; }

        [VerbOption("monitor", HelpText = "Procdiag attaches and stays attached to a running process and listens for console input in the form managed_thread_id|other_info and outputs to console other_info and the stack for that thread.")]
        public bool Monitor { get; set; }

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