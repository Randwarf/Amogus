using Amogus.Language.Content;
using Amogus.Language.Resources;
using Antlr4.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace Amogus.Language.Tests
{
    [TestClass]
    public class FunctionCallTests
    {
        [TestMethod]
        public void Call_PrintInFunction_ShouldOutputResult()
        {
            // Arrange
            var program =
                "foo()=> { Print('test'); } foo();";

            var expected = "test";

            // Act
            var log = (Log)Amogus.Execute(program);


            // Assert
            var output = (string)(log.SystemOut.First());

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void Call_AssignedValue_ShouldOutputResult()
        {
            // Arrange
            var program =
                "foo()=> { " +
                "   Print('test'); " +
                "} " +
                "x=foo();" +
                "x()";

            var expected = "test";

            // Act
            var log = (Log)Amogus.Execute(program);


            // Assert
            var output = (string)(log.SystemOut.First());

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void Call_AssignedValueAndReassigned_ShouldOutputResult()
        {
            // Arrange
            var program =
                "foo()=> { " +
                "   Print('test'); " +
                "} " +
                "x=foo();" +
                "x=10;" +
                "x=foo()";

            var expected = "test";

            // Act
            var log = (Log)Amogus.Execute(program);


            // Assert
            var output = (string)(log.SystemOut.First());

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void Call_TwoDistinctFunctionAssignments_ShouldOutputResult()
        {
            // Arrange
            var program =
                "foo()=>" +
                "{ " +
                "Print('test');" +
                "}" +
                "" +
                "boo()=>" +
                "{" +
                "Print('test2');" +
                "}" +
                "x=foo;" +
                "y=boo;" +
                "y();" +
                "x();";

            // Act
            var log = (Log)Amogus.Execute(program);

            // Assert
            var output1 = (string)(log.SystemOut[0]);
            var output2 = (string)(log.SystemOut[2]);

            Assert.AreEqual("test2", output1);
            Assert.AreEqual("test", output2);
        }

        [TestMethod]
        public void Call_FunctionInsideFunction_ShouldOutputResult()
        {
            // Arrange
            var program =
                "a()=>" +
                "{" +
                "Print('a call');" +
                "}" +
                "" +
                "b()=>" +
                "{" +
                "Print('b call');" +
                "}" +
                "" +
                "foo(boo1,boo2)=>" +
                "{" +
                "boo1();" +
                "boo2();" +
                "}" +
                "" +
                "foo(a,b);";

            // Act
            var log = (Log)Amogus.Execute(program);

            // Assert
            var output1 = (string)(log.SystemOut[0]);
            var output2 = (string)(log.SystemOut[2]);

            Assert.AreEqual("a call", output1);
            Assert.AreEqual("b call", output2);
        }

        [TestMethod]
        public void Call_ThreeNestedFunctionCalls_ShouldOutputResult()
        {
            // Arrange
            var program =
                "first(func1,func2)=>" +
                "{" +
                "func1(func2);" +
                "}"+
                "f1(f2)=>" +
                "{" +
                "f2();" +
                "}" +
                "f3()=>" +
                "{" +
                "Print('test');" +
                "}" +
                "first(f1,f3);";

            // Act
            var log = (Log)Amogus.Execute(program);

            // Assert
            var output = (string)(log.SystemOut[0]);

            Assert.AreEqual("test", output);
        }

        [TestMethod]
        public void Call_Recursion_ShouldOutputResult()
        {
            // Arrange
            var program =
                "recursion(value)=>" +
                "{" +
                "if value<=0" +
                "{" +
                "return 0;" +
                "}" +
                "" +
                "return value + recursion(value-1);" +
                "}" +
                "" +
                "answer=recursion(10);" +
                "Print(answer);";

            // Act
            var log = (Log)Amogus.Execute(program);

            // Assert
            var output = (string)(log.SystemOut[0]);

            Assert.AreEqual("10", output);
        }
    }
}
