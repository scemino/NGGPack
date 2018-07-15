//
// TokenReader.cs
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
    public class TokenReader
    {
        private CharacterReader _reader;

        public bool IsAtEnd => _reader.IsAtEnd;

        public Token Token { get; private set; }

        public TokenReader(CharacterReader reader)
        {
            _reader = reader;
            Next();
        }

        public void Next()
        {
            if (!IsAtEnd)
            {
                Token = GetNextToken();
                if (Token.Id == TokenId.Whitespace)
                    Next();
            }
        }

        public string GetTokenText(Token token)
        {
            return _reader.GetText(token.StartOffset, token.Length);
        }

        private TokenId GetNextTokenId()
        {
            char ch = _reader.Read();

            if ((ch == '\n') || (char.IsWhiteSpace(ch)))
            {
                while ((_reader.Peek() == '\n') || (char.IsWhiteSpace(_reader.Peek())))
                    _reader.Read();
                return TokenId.Whitespace;
            }
            switch (ch)
            {
                case ',':
                    return TokenId.Comma;
                case ':':
                    return TokenId.Colon;
                case '[':
                    return TokenId.OpenSquareBrace;
                case ']':
                    return TokenId.CloseSquareBrace;
                case '{':
                    return TokenId.OpenCurlyBrace;
                case '}':
                    return TokenId.CloseCurlyBrace;
                case '\"':
                    return ParseString(_reader);
                case 'n':
                    return ParseNull(_reader);
                default:
                    if ((ch == '-') || (ch >= '0') && (ch <= '9'))
                    {
                        return ParseNumber(_reader);
                    }
                    return TokenId.None;
            }
        }

        private Token GetNextToken()
        {
            if (IsAtEnd) return null;

            var startOffset = _reader.Offset;
            var id = GetNextTokenId();
            var endOffset = _reader.Offset;
            var token = new Token(id, startOffset, endOffset);
            return token;
        }

        private TokenId ParseString(CharacterReader reader)
        {
            while (reader.Peek() != '\"')
                reader.Read();
            reader.Read();
            return TokenId.String;
        }

        private TokenId ParseNull(CharacterReader reader)
        {
            if (reader.Peek() != 'u') throw new InvalidOperationException();
            reader.Read();
            if (reader.Peek() != 'l') throw new InvalidOperationException();
            reader.Read();
            if (reader.Peek() != 'l') throw new InvalidOperationException();
            reader.Read();
            return TokenId.Null;
        }

        private TokenId ParseNumber(CharacterReader reader)
        {
            while (char.IsNumber(reader.Peek()) || (reader.Peek() == '.'))
                reader.Read();
            return TokenId.Number;
        }
    }
}
