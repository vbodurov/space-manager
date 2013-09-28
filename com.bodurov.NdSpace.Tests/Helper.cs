using com.bodurov.NdSpace.Interface;
using com.bodurov.NdSpace.Model;

namespace YouVisio.Unity3D.AI.Tests
{
    public static class Helper
    {
        public const string p0 = "p0 (0, 0, 0)";
        public const string p1 = "p1 (1, 8, 3)";
        public const string p2 = "p2 (1,-1,-1)";
        public const string p3 = "p3 (5, 5, 5)";
        public const string p4 = "p4 (7, 1, 1)";
        public static void PopulatePoints(ISpaceManager sm, Space<string> space)
        {
            sm.AddPoint(space, p0, 0, 0, 0);
            sm.AddPoint(space, p1, 1, 8, 3);
            sm.AddPoint(space, p3, 5, 5, 5);
            sm.AddPoint(space, p2, 1, -1, -1);
            sm.AddPoint(space, p4, 7, 1, 1);
        }
    }
}