using System;
using System.Linq;
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

        [Test]
        public void T()
        {
            var arr1 = @"
FancyBox logo1.jpg
FancyBox logo2-01.jpg
FancyBox logo3-01.jpg
FancyBox website1-01.jpg
FancyBox website2-01.jpg
FancyBox website3-01.jpg
FancyBox logo1.jpg
FancyBox logo2-01.jpg
FancyBox logo3-01.jpg
FancyBox website1-01.jpg
FancyBox website2-01.jpg
FancyBox website3-01.jpg".Split('\n').Select(s => s.Trim()).ToArray();
            var arr2 = @"
Tile 150 logo1.jpg
Tile 150 logo2-01.jpg
Tile 150 logo3-01.jpg
Tile 150 website1-01.jpg
Tile 150 website2-01.jpg
Tile 150 website3-01.jpg
Tile 150 logo1.jpg
Tile 150 logo2-01.jpg
Tile 150 logo3-01.jpg
Tile 150 website1-01.jpg
Tile 150 website2-01.jpg
Tile 150 website3-01.jpg".Split('\n').Select(s => s.Trim()).ToArray();

            for (var i = 0; i < arr1.Length; ++i)
            {
                Console.WriteLine(@"
                <div class=""tile"">
                    <a class=""fancybox"" href=""images/{0}"">
                        <img src=""images/{1}"" alt=""""/>
                    </a>
                </div>", arr1[i], arr2[i]);
            }

                
        }
    }
}