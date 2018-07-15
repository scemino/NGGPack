//
// GGParser.cs
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

namespace NGGPack
{
    public class GGParser
    {
        TokenReader reader;
        public GGParser(Stream stream)
            : this(new TokenReader(new CharacterReader(new BinaryReader(stream))))
        {
        }

        public GGParser(TokenReader reader)
        {
            this.reader = reader;
        }

        public static GGValue Parse(string content)
        {
            using (var ms = new MemoryStream())
            {
                var writer = new StreamWriter(ms);
                writer.Write(content);
                writer.Flush();
                ms.Position = 0;
                var parser = new GGParser(ms);
                return parser.ParseValue();
            }
        }

        public GGValue ParseValue()
        {
            switch (reader.Token.Id)
            {
                case TokenId.OpenCurlyBrace:
                    return ParseHash();
                case TokenId.OpenSquareBrace:
                    return ParseArray();
                case TokenId.String:
                    return ParseString();
                case TokenId.Number:
                    return ParseNumber();
                case TokenId.Null:
                    return ParseNull();
                default:
                    return null;
            }
        }

        private GGValue ParseNumber()
        {
            var text = reader.GetTokenText(reader.Token);
            if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
            {
                var literal = new GGLiteral(value);
                reader.Next();
                return literal;
            }
            return ParseDouble();
        }

        public GGLiteral ParseString()
        {
            var text = reader.GetTokenText(reader.Token);
            var literal = new GGLiteral(text.Substring(1, text.Length - 2));
            reader.Next();
            return literal;
        }

        public GGLiteral ParseDouble()
        {
            var literal = new GGLiteral(double.Parse(reader.GetTokenText(reader.Token), CultureInfo.InvariantCulture));
            reader.Next();
            return literal;
        }

        public GGLiteral ParseInt()
        {
            var literal = new GGLiteral(int.Parse(reader.GetTokenText(reader.Token), CultureInfo.InvariantCulture));
            reader.Next();
            return literal;
        }

        public GGLiteral ParseNull()
        {
            if (reader.Token.Id != TokenId.Null) throw new InvalidOperationException();
            reader.Next();
            return new GGLiteral(null);

        }

        public GGArray ParseArray()
        {
            reader.Next();
            var values = new List<GGValue>();
            do
            {
                if (reader.Token.Id == TokenId.CloseSquareBrace) { reader.Next(); break; }
                values.Add(ParseValue());
                if (reader.Token.Id == TokenId.CloseSquareBrace) { reader.Next(); break; }
                if (reader.Token.Id != TokenId.Comma) return null;
                reader.Next();
            }
            while (true);
            return new GGArray(values);
        }

        public GGHash ParseHash()
        {
            reader.Next();
            var values = new Dictionary<string, GGValue>();
            do
            {
                if (reader.Token.Id != TokenId.String) return null;
                var key = reader.GetTokenText(reader.Token);
                key = key.Substring(1, key.Length - 2);
                reader.Next();
                if (reader.Token.Id != TokenId.Colon) return null;
                reader.Next();
                values.Add(key, ParseValue());
                if (reader.Token.Id == TokenId.CloseCurlyBrace)
                {
                    reader.Next();
                    break;
                }
                if (reader.Token.Id != TokenId.Comma) return null;
                reader.Next();
            }
            while (true);
            return new GGHash(values);
        }
    }
}
