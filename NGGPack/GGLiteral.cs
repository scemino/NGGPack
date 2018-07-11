//
// GGLiteral.cs
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

namespace NGGPack
{
    public class GGLiteral : GGValue
    {
        public GGLiteral(string value)
        {
            Value = value;
        }

        public GGLiteral(int value)
        {
            Value = value;
        }

        public GGLiteral(double value)
        {
            Value = value;
        }

        public object Value { get; }

        public override void WriteTo(GGWriter writer)
        {
            if (Value is string) { writer.WriteString((string)Value); return; }
            if (Value is int) { writer.WriteInt((int)Value); return; }
            if (Value is double) { writer.WriteDouble((double)Value); return; }
            if (Value == null) { writer.WriteNull(); return; }
            throw new InvalidOperationException();
        }
    }
}