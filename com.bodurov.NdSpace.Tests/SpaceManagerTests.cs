using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Shouldly;
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

        [Test]
        public void CanAddPoints()
        {
            ISpaceManager sm = new SpaceManager();
            var space = sm.CreateSpace<string>(new Space3DConfig());
            var dims = space.Dimensions;

            sm.AddPoint(space, "17,-51", 17, 0, -51);
            AssertBounds(dims, 17, 17, -51, -51);

            sm.AddPoint(space, "19,-55", 19, 0, -55);
            AssertBounds(dims, 17, 19, -55, -51);

            sm.AddPoint(space, "15,-61", 15, 0, -61);
            AssertBounds(dims, 15, 19, -61, -51);

            sm.AddPoint(space, "21,-57", 21, 0, -57);
            AssertBounds(dims, 15, 21, -61, -51);

            sm.AddPoint(space, "22,-60", 22, 0, -60);
            AssertBounds(dims, 15, 22, -61, -51);

            sm.AddPoint(space, "22,-55", 22, 0, -55);
            AssertBounds(dims, 15, 22, -61, -51);

            sm.AddPoint(space, "23,-52", 23, 0, -52);
            AssertBounds(dims, 15, 23, -61, -51);

            sm.AddPoint(space, "24,-52", 24, 0, -52);
            AssertBounds(dims, 15, 24, -61, -51);
        }

        //dimension.HeadDimPoint.TailLink

        private static void AssertBounds(Dimension<string>[] dims, float xHead, float xTail, float yHead, float yTail)
        {
            dims[0].HeadDimPoint.Position.ShouldBe(xHead);
            dims[0].TailDimPoint.Position.ShouldBe(xTail);
            dims[2].HeadDimPoint.Position.ShouldBe(yHead);
            dims[2].TailDimPoint.Position.ShouldBe(yTail);
        }

        [Test]
        public void CanAssignTailLinks()
        {
            ISpaceManager sm = new SpaceManager();
            var space = sm.CreateSpace<string>(new Space3DConfig());
            var dims = space.Dimensions;

            sm.AddPoint(space, "17,-51", 17, 0, -51);
            sm.AddPoint(space, "19,-55", 19, 0, -55);
            sm.AddPoint(space, "15,-61", 15, 0, -61);
            sm.AddPoint(space, "21,-57", 21, 0, -57);
            sm.AddPoint(space, "22,-60", 22, 0, -60);
            sm.AddPoint(space, "22,-55", 22, 0, -55);
            sm.AddPoint(space, "23,-52", 23, 0, -52);
            sm.AddPoint(space, "24,-52", 24, 0, -52);

            dims[0].HeadDimPoint.TailLink.ToString().ShouldBe("DimensionLink(15)");
            dims[0].TailDimPoint.TailLink.ToString().ShouldBe("DimensionLink(24)");
            dims[2].HeadDimPoint.TailLink.ToString().ShouldBe("DimensionLink(-61)");
            dims[2].TailDimPoint.TailLink.ToString().ShouldBe("DimensionLink(-51)");
        }

        [Test]
        public void CanFindClosestDimensionPoints()
        {
            ISpaceManager sm = new SpaceManager();
            var space = sm.CreateSpace<string>(new Space3DConfig());
            var dims = space.Dimensions;

            sm.AddPoint(space, "17,-51", 17, 52, -51);
            sm.AddPoint(space, "19,-55", 19, -20, -55);//IN
            sm.AddPoint(space, "15,-61", 15, 0, -61);
            sm.AddPoint(space, "21,-57", 21, 300, -57);//IN
            sm.AddPoint(space, "22,-60", 22, -59, -60);
            sm.AddPoint(space, "22,-55", 22, 2, -55);//IN
            sm.AddPoint(space, "23,-52", 23, -500, -52);//IN
            sm.AddPoint(space, "24,-52", 24, 100, -52);
            var center = new[] { 20.5f, 0f, -55.5f };

            DimensionPoint<string> left;
            DimensionPoint<string> right;
            var result = sm.TryFindDimensionPoint(dims[0], center[0], out left, out right);

            result.ShouldBe(false);
            left.ToString().ShouldBe("DimensionPoint(19)");
            right.ToString().ShouldBe("DimensionPoint(21)");

            result = sm.TryFindDimensionPoint(dims[2], center[2], out left, out right);

            result.ShouldBe(false);
            left.ToString().ShouldBe("DimensionPoint(-57)");
            right.ToString().ShouldBe("DimensionPoint(-55)");

        }

        [Test]
        public void CanFindSpacePointsWithin()
        {
            ISpaceManager sm = new SpaceManager();
            var space = sm.CreateSpace<string>(new Space3DConfig());

            sm.AddPoint(space, "17,-51", 17, 52, -51);
            sm.AddPoint(space, "19,-55", 19, -20, -55);//IN
            sm.AddPoint(space, "15,-61", 15, 0, -61);
            sm.AddPoint(space, "21,-57", 21, 300, -57);//IN
            sm.AddPoint(space, "22,-60", 22, -59, -60);
            sm.AddPoint(space, "22,-55", 22, 2, -55);//IN
            sm.AddPoint(space, "23,-52", 23, -500, -52);//IN
            sm.AddPoint(space, "24,-52", 24, 100, -52);
            var center = new[] {20.5f, 0f, -55.5f};


            var points = 
                sm.FindSpacePointsWithin(space, 4.5f, DimensionSelector.OnlyDimXZ, center);

            var list = new List<SpacePoint<string>>(points);

            list.Count.ShouldBe(4);
            list.Select(sp => sp.Value).ShouldContain("19,-55");
            list.Select(sp => sp.Value).ShouldContain("21,-57");
            list.Select(sp => sp.Value).ShouldContain("22,-55");
            list.Select(sp => sp.Value).ShouldContain("23,-52");
            list.Select(sp => sp.Value).ShouldNotContain("15,-61");
            list.Select(sp => sp.Value).ShouldNotContain("17,-51");
            list.Select(sp => sp.Value).ShouldNotContain("22,-60");
            list.Select(sp => sp.Value).ShouldNotContain("24,-52");
        }
        
    }
}