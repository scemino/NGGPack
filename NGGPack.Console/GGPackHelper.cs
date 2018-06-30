//
// GGPackHelper.cs
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace NGGPack.Console
{
    internal enum Action
    {
        Help,
        List,
        Cat,
        Extract
    }

    internal class GGPackHelper
    {
        private GGPackReader _reader;
        private GGPack _pack;

        public GGPackHelper(Stream stream)
        {
            _reader = new GGPackReader();
            _pack = _reader.ReadPack(stream);
        }

        public void List(List<string> cmdArgs)
        {
            var predicate = GetPredicate(cmdArgs);
            foreach (var entry in _pack.Entries)
            {
                if (!predicate(entry.Name)) continue;
                System.Console.WriteLine($"{entry.Name}");
            }
        }

        public void Cat(List<string> cmdArgs)
        {
            var predicate = GetPredicate(cmdArgs);
            var entry = _pack.Entries.FirstOrDefault(e => predicate(e.Name));
            if (entry == null)
            {
                System.Console.Error.WriteLine($"No entry found matching");
                return;
            }

            var ext = Path.GetExtension(entry.Name);
            if (ext == ".wimpy")
            {
                var entryStream = _pack.GetEntryStream(entry.Name);
                var values = _reader.ReadHash(entryStream);
                System.Console.WriteLine(values);
            }
            else
            {
                var entryStream = _pack.GetEntryStream(entry.Name);
                var text = new StreamReader(entryStream).ReadToEnd();
                System.Console.WriteLine(text);
            }
        }

        public void Extract(List<string> cmdArgs)
        {
            var predicate = GetPredicate(cmdArgs);
            foreach (var entry in _pack.Entries)
            {
                if (!predicate(entry.Name)) continue;

                var ext = Path.GetExtension(entry.Name);
                if (ext == ".wimpy")
                {
                    var entryStream = _pack.GetEntryStream(entry.Name);
                    var content = _reader.ReadHash(entryStream);
                    File.WriteAllText(entry.Name, content.ToString());
                }
                else
                {
                    var entryStream = _pack.GetEntryStream(entry.Name);
                    using (var fs = File.OpenWrite(GetEntryName(entry.Name)))
                    {
                        entryStream.CopyTo(fs);
                    }
                }
            }
        }

        private static string GetEntryName(string name)
        {
            var ext = Path.GetExtension(name);
            if (ext == ".bnut")
            {
                return Path.ChangeExtension(name, ".nut");
            }
            return name;
        }

        private static Func<string, bool> GetPredicate(List<string> cmdArgs)
        {
            Func<string, bool> predicate;
            if (cmdArgs.Count > 1)
            {
                var regex = new Regex(StringHelper.WildcardToRegex(cmdArgs[1]));
                predicate = regex.IsMatch;
            }
            else
            {
                predicate = text => true;
            }

            return predicate;
        }
    }
}
