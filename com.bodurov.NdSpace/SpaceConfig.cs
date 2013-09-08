﻿using com.bodurov.NdSpace.Interface;

namespace com.bodurov.NdSpace
{
    
    public class SpaceConfig : ISpaceConfig
    {
        int ISpaceConfig.NumDimensions { get { return 3; } }
        double ISpaceConfig.DefaultEpsilon { get { return 0.5; } }
    }
}