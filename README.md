# process-diagnostics
Command line utility to create memory and call stack dumps based on CLRMD

[![Build status](https://ci.appveyor.com/api/projects/status/94v4xsk7vrn7x0be?svg=true)](https://ci.appveyor.com/project/adrianrus/process-diagnostics)

Features
* Dump callstacks
* Dump heapstats
* Can attach to both 32bit and 64bit processes
* Single executable

# Available options

```
  -p, --pid           Required. Process id/name.

  -o, --out           The folder where minidump will be created. Setting it
                      implies --full option

  --threads           Dump thread callstacks.

  --xml               XML output.

  --stats             Dump heap stats.

  --full              Create memory dump.

  --passive           Use passive attach to process instead of NonInvasive
                      which is the default. Target process will not be paused
                      but results might be inaccurate.

  --dumpheapbytype    Dumps the objects of a given type from the managed heap.

  --monitor           Procdiag attaches and stays attached to a running process
                      then listens for console input in the form
                      managed_thread_id|other_info and outputs to console
                      other_info concatenaed with the stack for the thread.

  --help              Display this help screen.
```