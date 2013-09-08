namespace com.bodurov.NdSpace.Interface
{
    public interface ISpaceFactory
    {
        ISpaceConfig SpaceConfig { get; }
        ISpaceManager DimensionManager { get; }
    }
}