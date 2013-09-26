using com.bodurov.NdSpace.Interface;

namespace com.bodurov.NdSpace
{
    
    public class SpaceFactory : ISpaceFactory
    {
        public static readonly ISpaceFactory Current = new SpaceFactory();
        private SpaceFactory()
        {
            _config = new Space3DConfig();
            _spaceManager = new SpaceManager(_config);
        }
        private readonly ISpaceConfig _config;
        ISpaceConfig ISpaceFactory.SpaceConfig { get { return _config; } }
        private readonly ISpaceManager _spaceManager;
        ISpaceManager ISpaceFactory.DimensionManager { get { return _spaceManager; } }
    }
}