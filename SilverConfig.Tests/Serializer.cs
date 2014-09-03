using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SilverConfig.Tests
{
    [TestClass]
    public class Serializer
    {
        [TestMethod]
        public void Test()
        {
            var sC = new SilverConfigXmlSerializer<TestConfig>();
            Assert.AreEqual(sC.Serialize(new TestConfig { Test = "Test1", Test2 = "Test2" }), TestConfig.Output);
        }

        [SilverConfig]
        private class TestConfig
        {
            public const string Output =
@"<TestConfig>
  <Test2>Test2</Test2>

  <!-- Test
       Comment -->
  <Test>Test1</Test>
</TestConfig>";

            [SilverConfigElement(Index = 1, NewLineBefore = true, Comment =
@"Test
Comment")]
            public string Test;

            [SilverConfigElement()]
            public string Test2;
        }
    }
}