//
// CharacterReader.cs
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
    public class CharacterReader
    {
        private BinaryReader _reader;

        public int Offset => (int)_reader.BaseStream.Position;

        public bool IsAtEnd => Offset >= _reader.BaseStream.Length;

        public CharacterReader(BinaryReader reader)
        {
            _reader = reader;
        }

        public char Read()
        {
            if (IsAtEnd) return char.MaxValue;
            return _reader.ReadChar();
        }

        public char Peek()
        {
            if (IsAtEnd) return char.MaxValue;
            var c = _reader.ReadChar();
            _reader.BaseStream.Position--;
            return c;
        }

        public string GetText(int startOffset, int length)
        {
            var offset = Offset;
            _reader.BaseStream.Position = startOffset;
            var buf = new char[length];
            _reader.Read(buf, 0, length);
            _reader.BaseStream.Position = offset;
            return new string(buf);
        }
    }
}
