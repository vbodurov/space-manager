using NUnit.Framework;
using com.bodurov.NdSpace;
using com.bodurov.NdSpace.Interface;
using com.bodurov.NdSpace.Model;
using YouVisio.Unity3D.AI.Tests.SampleExtensions;

namespace YouVisio.Unity3D.AI.Tests
{
    [TestFixture]
    public class SpaceManagerTests
    {

        [Test]
        public void CanFindSpacePointByCoordinates()
        {
            ISpaceManager sm = new SpaceManager();
            var space = sm.CreateSpace<string>(new Space3DConfig());

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
            ISpaceManager sm = new SpaceManager();
            var space = sm.CreateSpace<string>(new Space3DConfig());

            Helper.PopulatePoints(sm, space);

            SpacePoint<string> center;
            sm.TryFindFirstSpacePoint(space, out center, 0, 0, 0);

            var point = sm.FindNearest(center, 10);

            Assert.That(point, Is.Not.Null);
            Assert.That(point.Value, Is.EqualTo(Helper.p2));
        }

        [Test]
        public void CanFindNearestSpacePointCase2()
        {
            ISpaceManager sm = new SpaceManager();
            var space = sm.CreateSpace<string>(new Space3DConfig());

            Helper.PopulatePoints(sm, space);

            SpacePoint<string> center;
            sm.TryFindFirstSpacePoint(space, out center, 1, -1, -1);

            var point = sm.FindNearest(center, 10);
            Assert.That(point, Is.Not.Null);
            Assert.That(point.Value, Is.EqualTo(Helper.p0));
        }

        [Test]
        public void CanRepositionPoint()
        {
            ISpaceManager sm = new SpaceManager();
            var space = sm.CreateSpace<string>(new Space3DConfig());

            Helper.PopulatePoints(sm, space);

            SpacePoint<string> point;
            sm.TryFindFirstSpacePoint(space, out point, 0, 0, 0);

            var r = sm.Reposition(point, 9.5f, -9.5f, 9.5f);

            Assert.That(r, Is.True,"Expeted to return true from Reposition method");
            Assert.That(point.Dimensions[0], Is.Not.Null, "Dim 0 should not be null");
            Assert.That(point.Dimensions[1], Is.Not.Null, "Dim 1 should not be null");
            Assert.That(point.Dimensions[2], Is.Not.Null, "Dim 2 should not be null");
            Assert.That(point.Dimensions[0].Position, Is.EqualTo(9.5));
            Assert.That(point.Dimensions[1].Position, Is.EqualTo(-9.5));
            Assert.That(point.Dimensions[2].Position, Is.EqualTo(9.5));
        }

        
    }
}