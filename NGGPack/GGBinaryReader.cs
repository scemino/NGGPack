//
// GGPackReader.cs
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
using System.Globalization;
using System.IO;
using System.Linq;

namespace NGGPack
{
    public class GGBinaryReader: IDisposable
    {
        private int[] _plo;
        private static readonly byte[] _magicBytes = { 0x4F, 0xD0, 0xA0, 0xAC, 0x4A, 0x5B, 0xB9, 0xE5, 0x93, 0x79, 0x45, 0xA5, 0xC1, 0xCB, 0x31, 0x93 };
        private BinaryReader _reader;

        public GGBinaryReader(BinaryReader reader)
        {
            _reader = reader;
        }

        public void Dispose()
        {
            _reader.Dispose();
        }

        public GGPack ReadPack()
        {
            var dataOffset = _reader.ReadInt32();
            var dataSize = _reader.ReadInt32();
            _reader.BaseStream.Seek(dataOffset, SeekOrigin.Begin);
            var buf = _reader.ReadBytes(dataSize);
            DecodeUnbreakableXor(buf);
            return new GGPack(ReadDirectory(buf), _reader.BaseStream);
        }

        public GGHash ReadDirectory()
        {
            var buf = new byte[_reader.BaseStream.Length - _reader.BaseStream.Position];
            _reader.Read(buf, 0, buf.Length);
            return ReadDirectory(buf);
        }

        private GGHash ReadDirectory(byte[] buf)
        {
            if (BitConverter.ToInt32(buf, 0) != 0x04030201)
                throw new InvalidOperationException("GGPack directory signature incorrect: " + BitConverter.ToInt32(buf, 0));

            var tmp = BitConverter.ToInt32(buf, 4);

            // read ptr list offset & point to first file name offset
            var plo = BitConverter.ToInt32(buf, 8);
            if (plo < 12 || plo >= buf.Length - 4)
                throw new InvalidOperationException("GGPack plo out of range: " + plo);

            if (buf[plo] != 7)
            {
                throw new InvalidOperationException("GGPack cannot find plo: " + plo);
            }

            plo++;
            // convert index ptr list to list of fname, offset, size values
            _plo = GetOffsets(buf, plo).ToArray();

            var off = 12;
            return ReadHash(buf, ref off);
        }

        internal static byte[] DecodeUnbreakableXor(byte[] buffer)
        {
            var previous = buffer.Length & 0xff;
            for (var i = 0; i < buffer.Length; i++)
            {
                var x = (byte)(buffer[i] ^ _magicBytes[i & 0xf] ^ (i * 0x6d));
                buffer[i] = (byte)(x ^ previous);
                previous = x;
            }
            return buffer;
        }

        public static byte[] EncodeUnbreakableXor(byte[] buffer)
        {
            var previous = buffer.Length & 0xff;
            for (var i = 0; i < buffer.Length; i++)
            {
                var x = (byte)(buffer[i] ^ previous);
                buffer[i] = (byte)(x ^ _magicBytes[i & 0xf] ^ (i * 0x6d));
                previous = x;
            }
            return buffer;
        }

        private static IEnumerable<int> GetOffsets(byte[] buf, int plo)
        {
            while (BitConverter.ToUInt32(buf, plo) != 0xFFFFFFFF)
            {
                var off = BitConverter.ToInt32(buf, plo);
                plo += 4;
                yield return off;
            }
        }

        private GGHash ReadHash(byte[] buf, ref int off)
        {
            if (buf[off] != 2)
            {
                throw new InvalidOperationException("trying to parse a non-hash");
            }
            off++;
            var n_pairs = BitConverter.ToInt32(buf, off);
            if (n_pairs == 0)
            {
                throw new InvalidOperationException("empty hash");
            }
            off += 4;
            var hash = new Dictionary<string, GGValue>();
            for (var i = 0; i < n_pairs; i++)
            {
                var key_plo_idx = BitConverter.ToInt32(buf, off);
                off += 4;

                var key = ReadString(buf, key_plo_idx);
                var value = ReadValue(buf, ref off);
                hash.Add(key, value);
            }

            if (buf[off] != 2)
                throw new InvalidOperationException("unterminated hash");
            off++;

            return new GGHash(hash);
        }

        private string ReadString(byte[] buf, int offset)
        {
            offset = _plo[offset];
            var off = offset;
            while (off < buf.Length && buf[off++] != 0) { }
            return System.Text.Encoding.UTF8.GetString(buf, offset, off - offset - 1);
        }

        private GGValue ReadValue(byte[] buf, ref int off)
        {
            var type = buf[off];
            switch (type)
            {
                case 1:
                    // null
                    off++;
                    return new GGLiteral(null);
                case 2:
                    // hash
                    return ReadHash(buf, ref off);
                case 3:
                    // array
                    {
                        off++;
                        var length = BitConverter.ToInt32(buf, off);
                        off += 4;
                        var arr = new GGValue[length];
                        for (int i = 0; i < length; i++)
                        {
                            arr[i] = ReadValue(buf, ref off);
                        }
                        if (buf[off] != 3)
                            throw new InvalidOperationException("unterminated array");
                        off++;
                        return new GGArray(arr);
                    }
                case 4:
                    // string
                    {
                        off++;
                        var plo_idx_int = BitConverter.ToInt32(buf, off);
                        off += 4;
                        var num_str = ReadString(buf, plo_idx_int);
                        return new GGLiteral(num_str);
                    }
                case 5:
                case 6:
                    {
                        // int
                        // double
                        off++;
                        var plo_idx_int = BitConverter.ToInt32(buf, off);
                        off += 4;
                        var num_str = ReadString(buf, plo_idx_int);
                        if (type == 5) return new GGLiteral(int.Parse(num_str, CultureInfo.InvariantCulture));
                        return new GGLiteral(double.Parse(num_str, CultureInfo.InvariantCulture));
                    }
                default:
                    throw new NotImplementedException();
            }
        }
    }
}