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
    public class GGLiteral : GGValue, IConvertible, IEquatable<GGLiteral>
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

        public TypeCode GetTypeCode()
        {
            return Convertible.GetTypeCode();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return Convertible.ToBoolean(provider);
        }

        public byte ToByte(IFormatProvider provider)
        {
            return Convertible.ToByte(provider);
        }

        public char ToChar(IFormatProvider provider)
        {
            return Convertible.ToChar(provider);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return Convertible.ToDateTime(provider);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return Convertible.ToDecimal(provider);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return Convertible.ToDouble(provider);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return Convertible.ToInt16(provider);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return Convertible.ToInt32(provider);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return Convertible.ToInt64(provider);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return Convertible.ToSByte(provider);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return Convertible.ToSingle(provider);
        }

        public string ToString(IFormatProvider provider)
        {
            return Convertible.ToString(provider);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return Convertible.ToType(conversionType, provider);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return Convertible.ToUInt16(provider);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return Convertible.ToUInt32(provider);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return Convertible.ToUInt64(provider);
        }

        public override void WriteTo(GGWriter writer)
        {
            if (Value is string) { writer.WriteString((string)Value); return; }
            if (Value is int) { writer.WriteInt((int)Value); return; }
            if (Value is double) { writer.WriteDouble((double)Value); return; }
            if (Value == null) { writer.WriteNull(); return; }
            throw new InvalidOperationException();
        }

        public bool Equals(GGLiteral other)
        {
            return Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var otherLiteral = obj as GGLiteral;
            if (otherLiteral == null) return false;
            return Equals(otherLiteral);
        }

        private IConvertible Convertible => (IConvertible)Value;

    }
}