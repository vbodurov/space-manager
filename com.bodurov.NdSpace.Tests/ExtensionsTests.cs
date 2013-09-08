using NUnit.Framework;
using com.bodurov.NdSpace.Extensions;

namespace YouVisio.Unity3D.AI.Tests
{
    [TestFixture]
    public class ExtensionsTests
    {
        [Test]
        [TestCase(1f, 1f, true)]
        [TestCase(-1f, -1f, true)]
        [TestCase(1.12345f, 1.12345f, true)]
        [TestCase(-1.12345f, -1.12345f, true)]
        [TestCase(1.12345f + SingleExtensions.Epsillon / 2, 1.12345f, true)]
        [TestCase(1f, 2f, false)]
        [TestCase(-1f, -2f, false)]
        [TestCase(1.12345f + SingleExtensions.Epsillon, 1.12345f, false)]
        public void IsEqualTo_WhenCalled_AktsAsExpected(float a, float b, bool expect)
        {
             Assert.That(a.IsEqualTo(b), Is.EqualTo(expect));
        }

//        [Test]
//        public void T()
//        {
//            var n = 100000.0;
//            for (var i = 0; i < 40; ++i)
//            {
//                n *= 1.06;
//                Console.WriteLine(i + "=" + n);
//            }   
//        }
    }
}