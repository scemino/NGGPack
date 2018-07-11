//
// Program.cs
//
// Author:
//       scemino <scemino74@gmail.com>
//
// Copyright (c) 2018 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.IO;
using Mono.Options;

namespace NGGPack.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var action = Action.Create;
            var options = new OptionSet
            {
                {"h|?|help", "show this help message and exit", v => action = Action.Help},
                {"l|list", "list files that match the pattern", v => action = Action.List},
                {"x|extract", "extract files that match the pattern", v => action = Action.Extract},
                {"c|cat", "output content of the first file that match the pattern", v => action = Action.Cat},
                {"g|gui", "use the GUI", v => action = Action.Gui}
            };

            var cmdArgs = options.Parse(args);

            if (action == Action.Help)
            {
                GGPackHelper.ShowHelp(options);
                return;
            }
            if (action == Action.Gui)
            {
                GGPackHelper.ShowGui();
                return;
            }

            if (cmdArgs.Count == 0) action = Action.Help;

            if (action == Action.Create)
            {
                GGPackHelper.Create(cmdArgs);
                return;
            }

            if (!File.Exists(cmdArgs[0]))
            {
                System.Console.Error.WriteLine($"File '{cmdArgs[0]}' does not exist");
                return;
            }

            try
            {
                using (var fs = File.OpenRead(cmdArgs[0]))
                {
                    var helper = new GGPackHelper(fs);

                    switch (action)
                    {
                        case Action.List:
                            helper.List(cmdArgs);
                            break;
                        case Action.Cat:
                            helper.Cat(cmdArgs);
                            break;
                        case Action.Extract:
                            helper.Extract(cmdArgs);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                System.Console.Error.WriteLine(e.Message);
            }
        }
    }
}
