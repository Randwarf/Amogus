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

            var inputStream = new AntlrInputStream(fileContents);
            var amogusLexer = new AmogusLexer(inputStream);
            var commonTokenStream = new CommonTokenStream(amogusLexer);
            var amogusParser = new AmogusParser(commonTokenStream);
            var amogusContext = amogusParser.program();
            var visitor = new AmogusVisitor();

            visitor.Visit(amogusContext);
        }

        public static object? Execute(string programText)
        {
            var inputStream = new AntlrInputStream(programText);
            var amogusLexer = new AmogusLexer(inputStream);
            var commonTokenStream = new CommonTokenStream(amogusLexer);
            var amogusParser = new AmogusParser(commonTokenStream);

            var amogusContext = amogusParser.program();
            var visitor = new AmogusVisitor();

            var x = visitor.Visit(amogusContext);

            return x;
        }
    }
}
