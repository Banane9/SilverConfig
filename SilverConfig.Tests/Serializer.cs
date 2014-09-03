using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SilverConfig.Tests
{
    [TestClass]
    public class Serializer
    {
        [TestMethod]
        public void Deserializes()
        {
            var sC = new SilverConfigXmlSerializer<TestConfig>();
            var config = sC.Deserialize(TestConfig.Output);

            Assert.AreEqual(config.Test, "Test1");
            Assert.AreEqual(config.Test2, "Test2");
            Assert.AreEqual(config.TestInt, 10);
        }

        [TestMethod]
        public void Serializes()
        {
            var sC = new SilverConfigXmlSerializer<TestConfig>();
            Assert.AreEqual(TestConfig.Output, sC.Serialize(new TestConfig()));
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
  <TestInt>10</TestInt>
  <TestInts>
    <TestItem>1</TestItem>
    <TestItem>1</TestItem>
    <TestItem>2</TestItem>
    <TestItem>3</TestItem>
    <TestItem>5</TestItem>
    <TestItem>8</TestItem>
    <TestItem>13</TestItem>
  </TestInts>
</TestConfig>";

            [SilverConfigElement(Index = 1, NewLineBefore = true, Comment =
@"Test
Comment")]
            public string Test = "Test1";

            [SilverConfigElement()]
            public string Test2 = "Test2";

            [SilverConfigElement(Index = 2)]
            public int TestInt = 10;

            [SilverConfigArrayElement(ArrayItemName = "TestItem", Index = 3)]
            public int[] TestInts = { 1, 1, 2, 3, 5, 8, 13 };
        }
    }
}