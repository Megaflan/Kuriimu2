﻿using SixLabors.ImageSharp;

namespace Kanvas.Quantization.Ditherers.Ordered
{
    public class Bayer2Ditherer : BaseOrderedDitherer
    {
        protected override byte[,] Matrix => new byte[,]
        {
            {1, 3},
            {4, 2}
        };

        public Bayer2Ditherer(Size imageSize, int taskCount) :
            base(imageSize, taskCount)
        {
        }
    }
}
