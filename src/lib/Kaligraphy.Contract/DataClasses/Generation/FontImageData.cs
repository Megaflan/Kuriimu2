﻿using Kaligraphy.Contract.DataClasses.Generation.Packing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Kaligraphy.Contract.DataClasses.Generation
{
    public class FontImageData
    {
        public required Image<Rgba32> Image { get; init; }

        public required IList<PackedGylphData> Glyphs { get; init; }
    }
}
