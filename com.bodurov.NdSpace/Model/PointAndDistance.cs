namespace com.bodurov.NdSpace.Model
{
    public class PointAndDistance<T>
    {
        public PointAndDistance(){}
        public PointAndDistance(SpacePoint<T> point, float distance)
        {
            Point = point;
            Distance = distance;
        }
        public SpacePoint<T> Point { get; set; }
        public float Distance { get; set; }
    }
}