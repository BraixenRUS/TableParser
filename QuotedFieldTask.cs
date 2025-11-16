using System.Text;
using NUnit.Framework;
//using NUnit.Framework.Legacy;

namespace TableParser
{
    class QuotedFieldTask
    {
        public static Token ReadQuotedField(string line, int startIndex)
        {
            var str = new StringBuilder();
            var length = 1;
            for (var i = startIndex + 1; i < line.Length; i++)
            {
                if (line[i] == line[startIndex])
                {
                    length++;
                    break;
                }
                if (line[i] == '\\' && i + 1 < line.Length)
                {
                    AppendEscaping(str, line, startIndex, i);
                    length += 2;
                    i++;
                }
                else
                {
                    str.Append(line[i]);
                    length++;
                }
            }
            return new Token(str.ToString(), startIndex, length);
        }

        static void AppendEscaping(StringBuilder str, string line, int startIndex, int i)
        {
            var next = line[i + 1];
            if (next == line[startIndex] || next == '\\')
                str.Append(next);
            else
            {
                str.Append('\\');
                str.Append(next);
            }
        }
    }

    [TestFixture]
        public class QuotedFieldTaskTests
        {
            [TestCase("''", 0, "", 2)]
            [TestCase("'a'", 0, "a", 3)]
            [TestCase("\"a\"", 0, "a", 3)]
            [TestCase("'abc def'", 0, "abc def", 9)]
            [TestCase("'a\\'b'", 0, "a'b", 6)]
            [TestCase("'a\\nb'", 0, "a\\nb", 6)]
            [TestCase("\"a 'b' c\"", 0, "a 'b' c", 9)]
            public void Test(string line, int startIndex, string expectedValue, int expectedLength)
            {
                var token = QuotedFieldTask.ReadQuotedField(line, startIndex);
                Assert.AreEqual(new Token(expectedValue, startIndex, expectedLength), token);
            }
        }
}