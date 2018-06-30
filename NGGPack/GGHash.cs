//
// GGHash.cs
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
using System.Linq;
using System.Text;

namespace NGGPack
{
    public class GGHash : GGValue
    {
        public GGHash(Dictionary<string, GGValue> pairs)
        {
            Pairs = pairs;
        }

        public Dictionary<string, GGValue> Pairs { get; }

        public GGValue this[string key] => Pairs[key];

        public override void WriteTo(GGWriter writer)
        {
            writer.WriteStartHash();
            if (Pairs.Count > 0)
            {
                foreach (var pair in Pairs.Take(Pairs.Count - 1))
                {
                    writer.WriteHashName(pair.Key);
                    pair.Value.WriteTo(writer);
                    writer.WriteHashDelimiter();
                }
                var lastPair = Pairs.Last();
                writer.WriteHashName(lastPair.Key);
                lastPair.Value.WriteTo(writer);
            }
            writer.WriteEndHash();
        }
    }
}