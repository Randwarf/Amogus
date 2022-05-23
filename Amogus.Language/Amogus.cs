using Amogus.Language.Content;
using Antlr4.Runtime;
using System.IO;

namespace Amogus.Language
{
    public class Amogus
    {
        public static void Main()
        {
            var fileName = "Content\\test.amg";
            var fileContents = File.ReadAllText(fileName);

            Execute(fileContents);
        }
        public static object? Execute(string programText)
        {
            var inputStream = new AntlrInputStream(programText);
            var amogusLexer = new AmogusLexer(inputStream);
            var commonTokenStream = new CommonTokenStream(amogusLexer);
            var amogusParser = new AmogusParser(commonTokenStream);

            var amogusContext = amogusParser.program();
            var visitor = new AmogusVisitor();

            return visitor.Visit(amogusContext);
        }
    }
}
