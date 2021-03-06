﻿using com.bodurov.NdSpace.Interface;

namespace com.bodurov.NdSpace.Model
{
    public class Space<T> : BaseSpaceObject
    {

        public Space(ISpaceConfig config, ISpaceManager spaceManager)
        {
            Config = config;
            Manager = spaceManager;
            Dimensions = new Dimension<T>[Config.NumDimensions];
        }

        public Dimension<T>[] Dimensions { get; private set; }
        public ISpaceConfig Config { get; private set; }
        public ISpaceManager Manager { get; private set; }
        public int Count { get { return Dimensions[0].Count; } }
    }
}