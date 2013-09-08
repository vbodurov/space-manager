namespace com.bodurov.NdSpace.Model
{
    public class SpacePoint<T> : BaseSpaceObject
    {

        public SpacePoint(Space<T> space)
        {
            Space = space;
            Dimensions = new DimensionPoint<T>[space.Dimensions.Length];
        }

        public Space<T> Space { get; private set; }
        public DimensionPoint<T>[] Dimensions { get; private set; }
        public T Value { get; set; }
    }
}