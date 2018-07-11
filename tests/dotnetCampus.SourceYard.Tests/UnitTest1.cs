using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.Extensions.Contracts;

namespace dotnetCampus.SourceYard.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [ContractTestCase]
        public void TestMethod1()
        {
            "Please type a test contract here...".Test(() =>
            {
                // Arrange
                // TODO: Write your test case here...

                // Action

                // Assert
            });
        }
    }
}
