using System;
using System.Linq;
using NUnit.Framework;
using com.bodurov.NdSpace.Model;

namespace YouVisio.Unity3D.AI.Tests
{
    [TestFixture]
    public class DimensionTests
    {
        [Test]
        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(2, 1)]
        [TestCase(3, 2)]
        [TestCase(4, 2)]
        [TestCase(5, 3)]
        [TestCase(6, 3)]
        [TestCase(7, 3)]
        [TestCase(8, 3)]
        [TestCase(9, 4)]
        [TestCase(16, 4)]
        [TestCase(17, 5)]
        [TestCase(32, 5)]
        [TestCase(33, 6)]
        [TestCase(64, 6)]
        [TestCase(65, 7)]
        [TestCase(128, 7)]
        [TestCase(129, 8)]
        [TestCase(256, 8)]
        [TestCase(257, 9)]
        [TestCase(512, 9)]
        [TestCase(513, 10)]
        [TestCase(1024, 10)]
        [TestCase(1025, 11)]
        [TestCase(2048, 11)]
        [TestCase(2049, 12)]
        public void NumberLevels_BasedOnGivenCount_ReturnsExpected(int count, int expectedLevels)
        {
            var dim = new Dimension<int>(0, null) {Count = count};

            Assert.That(dim.NumberLevels, Is.EqualTo(expectedLevels));
        }



        [Test]
        public void Test1()
        {
            var arr = Enumerable.Range(0, 10).ToArray();

            arr.Aggregate((a, b) => { Console.WriteLine(a + "|" + b);
                                        return b;
            });

            //Assert.That(y, Is.EqualTo(lvl));
        }
    }
}

