﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanvas.Quantization.Models.Quantizer
{
    class DistinctColorInfo
    {
        private const int Factor = 5000000;

        public int Count { get; private set; }

        public int Color { get; }

        public int Hue { get; }

        public int Saturation { get; }

        public int Brightness { get; }

        public DistinctColorInfo(Color color)
        {
            Color = color.ToArgb();

            Hue = Convert.ToInt32(color.GetHue() * Factor);
            Saturation = Convert.ToInt32(color.GetSaturation() * Factor);
            Brightness = Convert.ToInt32(color.GetBrightness()* Factor);

            Count = 1;
        }

        public DistinctColorInfo IncreaseCount()
        {
            Count++;
            return this;
        }
    }
}
