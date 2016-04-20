# process-diagnostics
Command line utility to create memory and call stack dumps based on CLRMD

[![Build status](https://ci.appveyor.com/api/projects/status/94v4xsk7vrn7x0be?svg=true)](https://ci.appveyor.com/project/adrianrus/process-diagnostics)


# Available options

  -p, --pid    Required. Process id/name to attach to.

  -o, --out    The folder where memory dump will be created. Setting it implies --full option.

  --threads    Dump thread callstacks.

  --stats      Dump heap stats.

  --full       Create memory dump.

  --help       Display this help screen.
  