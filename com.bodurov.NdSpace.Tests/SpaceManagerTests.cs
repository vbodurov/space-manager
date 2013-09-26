using NUnit.Framework;
using com.bodurov.NdSpace;
using com.bodurov.NdSpace.Interface;
using com.bodurov.NdSpace.Model;

namespace YouVisio.Unity3D.AI.Tests
{
    [TestFixture]
    public class SpaceManagerTests
    {

        [Test]
        public void CanFindSpacePointByCoordinates()
        {
            ISpaceManager sm = new SpaceManager(new Space3DConfig());
            var space = sm.CreateSpace<string>();

            sm.AddPoint(space, "A", 0, 0, 0);
            sm.AddPoint(space, "B", 1, 5, -3);
            sm.AddPoint(space, "C", 2, -6, 1);
            sm.AddPoint(space, "D", 0, 8, 8);

            SpacePoint<string> point;
            var r = sm.TryFindFirstSpacePoint(space, out point, 2, -6, 1);

            Assert.That(r, Is.True);
            Assert.That(point, Is.Not.Null);
            Assert.That(point.Value, Is.EqualTo("C"));
        }

        [Test]
        public void CanFindNearestSpacePointCase1()
        {
            ISpaceManager sm = new SpaceManager(new Space3DConfig());
            var space = sm.CreateSpace<string>();

            PopulatePoints(sm, space);

            SpacePoint<string> center;
            sm.TryFindFirstSpacePoint(space, out center, 0, 0, 0);

            var point = sm.FindNearest(center, 10);
            Assert.That(point, Is.Not.Null);
            Assert.That(point.Value, Is.EqualTo(p2));
        }

        [Test]
        public void CanFindNearestSpacePointCase2()
        {
            ISpaceManager sm = new SpaceManager(new Space3DConfig());
            var space = sm.CreateSpace<string>();

            PopulatePoints(sm, space);

            SpacePoint<string> center;
            sm.TryFindFirstSpacePoint(space, out center, 1, -1, -1);

            var point = sm.FindNearest(center, 10);
            Assert.That(point, Is.Not.Null);
            Assert.That(point.Value, Is.EqualTo(p0));
        }

        private const string p0 = "p0 (0, 0, 0)";
        private const string p1 = "p1 (1, 8, 3)";
        private const string p2 = "p2 (1,-1,-1)";
        private const string p3 = "p3 (5, 5, 5)";
        private const string p4 = "p4 (7, 1, 1)";
        private static void PopulatePoints(ISpaceManager sm, Space<string> space)
        {
            sm.AddPoint(space, p0, 0, 0, 0);
            sm.AddPoint(space, p1, 1, 8, 3);
            sm.AddPoint(space, p3, 5, 5, 5);
            sm.AddPoint(space, p2, 1, -1, -1);
            sm.AddPoint(space, p4, 7, 1, 1);
        }
    }
}