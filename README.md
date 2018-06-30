# NGGDump

This is a tool to list, view and extract files from **ggpack** data files used by the [Thimbleweed Park](https://thimbleweedpark.com/) game, which is an awesome adventure game, go buy it right now, you won't regret it ([Steam](http://store.steampowered.com/app/569860/Thimbleweed_Park/), [GOG](https://www.gog.com/game/thimbleweed_park)).

## Prerequisites

You need to have donet core installed, you can get it here: https://www.microsoft.com/net/learn/get-started/

## Build & Run

* First clone the project: `git clone https://github.com/scemino/NGGPack.git`
* Then run: `dotnet run ThimbleweedPark.ggpack1 *.bnut`
* or to create the executable
    * on Windows 10: `dotnet publish -c Release -r win10-x64`
    * on Ubuntu: `dotnet publish -c Release -r ubuntu.16.10-x64`
    * on MacOS: `dotnet publish -c Release -r osx-x64`
* That's it

## Usage

    usage: NGGPack.Console [-h] [-l] [-c] ggpack_file search_pattern

        -h, -?, --help             show this help message and exit
        -l, --list                 list files that match the pattern
        -x, --extract              extract files that match the pattern
        -c, --cat                  output content of the first file that match the
                                    pattern
        Example: NGGPack.Console ThimbleweedPark.ggpack1 B*.bnut

## Thanks

This project has been adapted from the awesome projects https://github.com/mrmacete/r2-ggpack and twp-ggdump https://github.com/mstr-/twp-ggdump

## Features
* Browse all files into the ggpack
* Search file with wildcards
* Show the content of a file
* Extract files from the ggpack
* Dump **wimpy** files
* Deobfuscate the bnut files
