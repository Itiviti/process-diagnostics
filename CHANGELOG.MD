# Changelog 

## 1.3.1

* bug fix release

## 1.3.0

* Arguments overhaul (BREAKING CHANGES)
    * -p/--pid: now also suports process name not only process pid
    * --threadsonly: dropped in favor of --threads, can be combined with other arguments
    * --stats: added for heap statistics, can be combined with other arguments
    * --full: added for a full memory dump, can be combined with other argument
    * Specifying a dump location with -o or --out will imply --full option, also specifing only -p/--pid argument implies --full option

## 1.2.1

* Bug fix release

## 1.2.0

* Embed x86 dump utility into one exe

## 1.1.0

* Support for x86 processes as a separate utility

## 1.0.0

* Dump Thread stacks feature --threadsonly
* Full memory dump on specified location
* Memory stats dump on full memory dump