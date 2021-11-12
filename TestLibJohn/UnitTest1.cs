using LibJohn;
using NUnit.Framework;
using System;

namespace TestLibJohn
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var file = new TileMap();
            try
            {
                file.LoadFile(".\\TestData\\TestSimple.map");
            }catch(Exception e)
            {
                Assert.Fail();
            }
        }
    }
}