using Amogus.Language.Content;
using Amogus.Language.Resources;
using Antlr4.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }
}
