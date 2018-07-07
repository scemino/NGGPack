//
// GGTextWriter.cs
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

using System.IO;

namespace NGGPack
{
    public class GGTextWriter : GGWriter
    {
        private TextWriter _writer;
        private int _indent;

        public GGTextWriter(TextWriter writer)
        {
            _writer = writer;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _writer.Dispose();
            }
            base.Dispose(disposing);
        }

        public override void WriteArrayDelimiter()
        {
            _writer.Write(",");
            WriteIndent();
        }

        private void WriteIndent()
        {
            _writer.WriteLine();
            _writer.Write(new string(' ', _indent * 2));
        }

        public override void WriteDouble(double value)
        {
            _writer.Write(value);
        }

        public override void WriteStartArray()
        {

            _writer.Write("[");
            _indent++;
            WriteIndent();
        }

        public override void WriteEndArray()
        {
            _indent--;
            WriteIndent();
            _writer.Write("]");
        }

        public override void WriteHashDelimiter()
        {
            _writer.Write(',');
            WriteIndent();
        }

        public override void WriteHashName(string key)
        {
            WriteString(key);
            _writer.Write(": ");
        }

        public override void WriteStartHash()
        {
            _indent++;
            _writer.Write("{");
            WriteIndent();
        }

        public override void WriteEndHash()
        {
            _indent--;
            WriteIndent();
            _writer.Write("}");
        }

        public override void WriteInt(int value)
        {
            _writer.Write(value);
        }

        public override void WriteNull()
        {
            _writer.Write("null");
        }

        public override void WriteString(string value)
        {
            _writer.Write('\"');
            _writer.Write(value);
            _writer.Write('\"');
        }
    }
}