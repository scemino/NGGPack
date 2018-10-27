//
// GGPackWriter.cs
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

namespace NGGPack
{
    public class GGPackWriter : IDisposable
    {
        private readonly BinaryWriter _bw;
        private readonly GGArray _gFiles;
        private int _offset;

        public GGPackWriter(BinaryWriter bw)
        {
            _bw = bw;
            _offset = 8;
            _gFiles = new GGArray();
            _bw.Write(0);
            _bw.Write(0);
        }

        public GGPackWriter WriteFile(string path)
        {
            var filename = Path.GetFileName(path);
            using (var fs = File.OpenRead(path))
            {
                if (string.Equals(Path.GetExtension(filename), ".nut", StringComparison.OrdinalIgnoreCase))
                {
                    filename = Path.ChangeExtension(filename, ".bnut");
                }
                var hash = new GGHash
                {
                    {"filename", new GGLiteral(filename)},
                    {"offset", new GGLiteral(_offset)},
                    {"size", new GGLiteral((int)fs.Length)},
                };
                _gFiles.Add(hash);
                _offset += (int)fs.Length;
            }

            var data = File.ReadAllBytes(path);
            if (string.Equals(Path.GetExtension(filename), ".nut", StringComparison.OrdinalIgnoreCase))
            {
                GGPack.DecryptBnut(data);
            }
            else if (string.Equals(Path.GetExtension(filename), ".wimpy", StringComparison.OrdinalIgnoreCase))
            {
                GGHash hash;
                using (var ms = new MemoryStream(data))
                {
                    var parser = new GGParser(ms);
                    hash = parser.ParseHash();
                }
				using (var ms = new MemoryStream())
				{
                    using (var writer = new GGBinaryWriter(new BinaryWriter(ms)))
                    {
                        hash.WriteTo(writer);
                    }
					data = ms.ToArray();
				}
            }
            GGBinaryReader.EncodeUnbreakableXor(data);
            _bw.Write(data);

            return this;
        }

        public GGPackWriter WriteFiles(params string[] pathes)
        {
            Array.ForEach(pathes, path => WriteFile(path));
            return this;
        }

        public void Dispose()
        {
            var directory = new GGHash(new Dictionary<string, GGValue> { { "files", _gFiles } });

            byte[] directoryData;
            var dirOffset = (int)_bw.BaseStream.Position;
            using (var ms = new MemoryStream())
            {
                using (var gbw = new GGBinaryWriter(new BinaryWriter(ms)))
                {
                    directory.WriteTo(gbw);
                }
                directoryData = ms.ToArray();
                GGBinaryReader.EncodeUnbreakableXor(directoryData);
            }
            _bw.Write(directoryData);
            var size = directoryData.Length;
            _bw.BaseStream.Position = 0;
            _bw.Write(dirOffset);
            _bw.Write(size);
        }
    }
}
