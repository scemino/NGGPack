using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NGGPack.Tests
{
    public class ParserTests
    {
        [Fact]
        public void NumberDouble()
        {
            var literal = new GGLiteral(42.42);
            var actualLiteral = GGParser.Parse(literal.ToString());
            Assert.Equal(literal, actualLiteral);

            literal = new GGLiteral(-42.42);
            actualLiteral = GGParser.Parse(literal.ToString());
            Assert.Equal(literal, actualLiteral);
        }

        [Fact]
        public void NumberInteger()
        {
            var literal = new GGLiteral(42);
            var actualLiteral = GGParser.Parse(literal.ToString());
            Assert.Equal(literal, actualLiteral);

            literal = new GGLiteral(-42);
            actualLiteral = GGParser.Parse(literal.ToString());
            Assert.Equal(literal, actualLiteral);
        }

        [Fact]
        public void Null()
        {
            var literal = new GGLiteral(null);
            var actualLiteral = GGParser.Parse(literal.ToString());
            Assert.Equal(literal, actualLiteral);
        }

        [Fact]
        public void String()
        {
            var literal = new GGLiteral("This is a string");
            var actualLiteral = GGParser.Parse(literal.ToString());
            Assert.Equal(literal, actualLiteral);
        }

        [Fact]
        public void Array()
        {
            var array = new double[] { 1.1, 2.2, 3.3 };
            var gArray = new GGArray(array.Select(o => new GGLiteral(o)));
            var actualLiteral = GGParser.Parse(gArray.ToString());
            Assert.Equal(gArray, actualLiteral);

            var arrayStrings = new string[] { "a", "b", "c", "d" };
            gArray = new GGArray(arrayStrings.Select(o => new GGLiteral(o)));
            actualLiteral = GGParser.Parse(gArray.ToString());
            Assert.Equal(gArray, actualLiteral);
        }

        [Fact]
        public void Hash()
        {
            var values = new Dictionary<string, GGValue>{
                {"int", new GGLiteral(42)},
                {"double", new GGLiteral(3.14159)},
                {"string", new GGLiteral("My string")},
                {"array", new GGArray(new GGLiteral(0),new GGLiteral(1))},
                {"null", new GGLiteral(null)}
            };
            var gHash = new GGHash(values);
            var actualLiteral = GGParser.Parse(gHash.ToString());
            Assert.Equal(gHash, actualLiteral);
        }
    }
}
