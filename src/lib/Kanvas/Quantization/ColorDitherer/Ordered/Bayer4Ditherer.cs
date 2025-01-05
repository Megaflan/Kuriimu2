﻿using SixLabors.ImageSharp;

namespace Kanvas.Quantization.ColorDitherer.Ordered
{
    public class Bayer4Ditherer : OrderedDitherer
    {
        protected override byte[,] Matrix => new byte[,]
        {
            {1, 9, 3, 11},
            {13, 5, 15, 7},
            {4, 12, 2, 10},
            {16, 8, 14, 6}
        };

        public Bayer4Ditherer(Size imageSize, int taskCount) :
            base(imageSize, taskCount)
        {
        }
    }
}
