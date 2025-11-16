using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace TableParser
{
    [TestFixture]
    public class FieldParserTaskTests
    {
        public static void Test(string input, string[] expectedResult)
        {
            var actualResult = FieldsParserTask.ParseLine(input);
            Assert.AreEqual(expectedResult.Length, actualResult.Count);
            for (int i = 0; i < expectedResult.Length; ++i)
            {
                Assert.AreEqual(expectedResult[i], actualResult[i].Value);
            }
        }

        [TestCase("a", new[] { "a" })]
        [TestCase("hello world", new[] { "hello", "world" })]
        [TestCase("a b c", new[] { "a", "b", "c" })]
        [TestCase("ab cd ef", new[] { "ab", "cd", "ef" })]
        [TestCase("\\", new[] { "\\" })]
        [TestCase("''", new[] { "" })]
        [TestCase("'a'", new[] { "a" })]
        [TestCase("'abc def'", new[] { "abc def" })]
        [TestCase("'a\\'b'", new[] { "a'b" })]
        [TestCase("\"a 'b' c\"", new[] { "a 'b' c" })]
        [TestCase("'a \"b\" c'", new[] { "a \"b\" c" })]
        [TestCase("\"a \\\"b\\\" c\"", new[] { "a \"b\" c" })]
        [TestCase("\"abc", new[] { "abc" })]
        [TestCase("\"\"", new[] { "" })]
        [TestCase("'a' b", new[] { "a", "b" })]
        [TestCase("abc ''", new[] { "abc", "" })]
        [TestCase("   a   ", new[] { "a" })]
        [TestCase("a    b", new[] { "a", "b" })]
        [TestCase("'abc' def", new[] { "abc", "def" })]
        [TestCase("'a'", new[] { "a" })]
        [TestCase("'a' 'b'", new[] { "a", "b" })]
        [TestCase("\"b c\"", new[] { "b c" })]
        [TestCase("'a'\"b\"", new[] { "a", "b" })]
        [TestCase("\"a \\\\\"", new[] { "a \\" })]
        [TestCase("'\\\\'", new[] { "\\" })]
        [TestCase("", new string[0])]
        [TestCase("\"a b ", new[] { "a b " })]
        public static void RunTests(string input, string[] expectedOutput)
        {
            Test(input, expectedOutput);
        }
    }

    public class FieldsParserTask
    {
        public static List<Token> ParseLine(string line)
        {
            var list = new List<Token>();
            for (var i = 0; i < line.Length; i++)
            {
                if (line[i] == ' ')
                    continue;
                var token = TakeToken(line, i);
                list.Add(token);
                i = token.GetIndexNextToToken() - 1;
            }
            return list;
        }

        public static Token TakeToken(string line, int i)
        {
            if (line[i] == '\'' || line[i] == '\"')
                return QuotedFieldTask.ReadQuotedField(line, i);
            return ReadField(line, i);
        }

        private static Token ReadField(string line, int startIndex)
        {
            var str = new StringBuilder();
            for (var i = startIndex; i < line.Length; i++)
            {
                if (line[i] == '\'' || line[i] == '\"' || line[i] == ' ')
                    break;
                str.Append(line[i]);
            }
            return new Token(str.ToString(), startIndex, str.Length);
        }
    }
}