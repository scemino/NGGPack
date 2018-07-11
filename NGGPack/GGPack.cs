﻿//
// GGPack.cs
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

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NGGPack
{
    public partial class GGPack
    {
        private readonly GGHash _directory;
        private readonly Stream _stream;
        private List<GGPackEntry> _entries;

        public IList<GGPackEntry> Entries => _entries;

        public GGPack(GGHash directory, Stream stream)
        {
            _directory = directory;
            _stream = stream;
            var files = _directory["files"] as GGArray;
            _entries = files.Cast<GGHash>()
                            .Select(o => new GGPackEntry((string)o["filename"], (int)o["offset"], (int)o["size"]))
                            .ToList();
        }

        public Stream GetEntryStream(string name)
        {
            var entry = Entries.First(o => Equals(o.Name, name));
            _stream.Position = entry.Offset;
            var buf = new byte[entry.Size];
            _stream.Read(buf, 0, entry.Size);
            GGBinaryReader.DecodeUnbreakableXor(buf);
            var extension = Path.GetExtension(name);
            if (string.Equals(extension, ".bnut", System.StringComparison.OrdinalIgnoreCase))
            {
                DecryptBnut(buf);
            }
            return new MemoryStream(buf);
        }

        private static void DecryptBnut(byte[] data)
        {
            int cursor = data.Length & 0xff;
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= _pass2[cursor];
                cursor = (cursor + 1) % _pass2.Length;
            }
        }
    }
}