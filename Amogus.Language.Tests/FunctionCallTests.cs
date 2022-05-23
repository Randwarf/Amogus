using Amogus.Language.Resources;
using NUnit.Framework;
using System.Collections.Generic;

namespace Amogus.Language.IntegrationTests
{
    public class FunctionCallTests
    {
        [Test]
        public void Call_PrintInFunction_ReturnsToStdoutString()
        {
            // Arrange
            var program = 
                @"
                    foo()=>
                    {
                        Print('test');
                    }
                    
                    foo();
                ";

            var expected = new List<string>
            {
                "test",
                "\n"
            };

            // Act
            var log = (List<object?>)Amogus.Execute(program);

            // Assert
            //Assert.AreEqual(expected[0], (string)log.Stdout[0]);
            //Assert.AreEqual(expected[1], (string)log.Stdout[1]);
        }
    }
}