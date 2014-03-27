using com.bodurov.NdSpace.Interface;

namespace com.bodurov.NdSpace
{
    
    public class Space3DConfig : ISpaceConfig
    {
        int ISpaceConfig.NumDimensions { get { return 3; } }
        double ISpaceConfig.DefaultEpsilon { get { return 0.25; } }
    }
}