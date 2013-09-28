namespace com.bodurov.NdSpace.Model
{
    public class PointNfo<T>
    {
        public PointNfo(){}
        public PointNfo(SpacePoint<T> point, float distance)
        {
            Point = point;
            Distance = distance;
        }
        public SpacePoint<T> Point { get; set; }
        public float Distance { get; set; }
    }
}