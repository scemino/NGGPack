//
// GGBinaryWriter.cs
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

namespace NGGPack
{
    public class GGBinaryWriter : GGWriter
    {
        private BinaryWriter _writer;
        private List<string> _keys = new List<string>();

        public GGBinaryWriter(BinaryWriter writer)
        {
            _writer = writer;
            _writer.Write(0x04030201);
            _writer.Write(1);
            _writer.Write(0);
        }

        public override void WriteArrayDelimiter()
        {
        }

        public override void WriteDouble(double value)
        {
            _writer.Write((byte)6);
            WriteKeyIndex(value.ToString());
        }

        public override void WriteEndArray()
        {
            _writer.Write((byte)3);
        }

        public override void WriteEndHash()
        {
            _writer.Write((byte)2);
        }

        public override void WriteHashDelimiter()
        {
        }

        public override void WriteHashName(string key)
        {
            WriteKeyIndex(key);
        }

        public override void WriteInt(int value)
        {
            _writer.Write((byte)5);
            WriteKeyIndex(value.ToString());
        }

        public override void WriteNull()
        {
            _writer.Write((byte)1);
        }

        public override void WriteStartArray(int count)
        {
            _writer.Write((byte)3);
            _writer.Write(count);
        }

        public override void WriteStartHash(int numPairs)
        {
            _writer.Write((byte)2);
            _writer.Write(numPairs);
        }

        public override void WriteString(string value)
        {
            _writer.Write((byte)4);
            WriteKeyIndex(value);
        }

        protected override void Dispose(bool disposing)
        {
            WriteKeys();
            base.Dispose(disposing);
        }

        private void WriteKeyIndex(string key)
        {
            _writer.Write(_keys.Count);
            _keys.Add(key);
        }

        private void WriteKeys()
        {
            var offset = (int)_writer.BaseStream.Position;
            _writer.BaseStream.Seek(8, SeekOrigin.Begin);
            _writer.Write(offset);
            _writer.BaseStream.Seek(offset, SeekOrigin.Begin);

            _writer.Write((byte)7);
            offset++;
            var lengths = new List<int>();
            foreach (var key in _keys)
            {
                var count = System.Text.Encoding.UTF8.GetByteCount(key) + 1;
                lengths.Add(count);
                offset += 4;
            }
            offset += 4;
            foreach (var length in lengths)
            {
                _writer.Write(offset);
                offset += length;
            }
            _writer.Write(0xFFFFFFFF);
            foreach (var key in _keys)
            {
                WriteKey(key);
            }
        }

        private void WriteKey(string value)
        {
            var data = System.Text.Encoding.UTF8.GetBytes(value);
            _writer.Write(data, 0, data.Length);
            _writer.Write((byte)0);
        }
    }
}