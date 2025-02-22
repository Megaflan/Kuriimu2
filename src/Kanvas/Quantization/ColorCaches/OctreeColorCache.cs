﻿using System.Collections.Generic;
using System.Linq;
using Kanvas.MoreEnumerable;
using Kanvas.Quantization.Models.ColorCache;
using SixLabors.ImageSharp.PixelFormats;

namespace Kanvas.Quantization.ColorCaches
{
    public class OctreeColorCache : BaseColorCache
    {
        private readonly OctreeCacheNode _root;

        public OctreeColorCache(IList<Rgba32> palette) :
            base(palette)
        {
            _root = new OctreeCacheNode();

            Palette.ForEach((c, i) => _root.AddColor(c, i, 0));
        }

        /// <inheritdoc />
        public override int GetPaletteIndex(Rgba32 color)
        {
            var candidates = _root.GetPaletteIndex(color, 0);

            var candidateColors = candidates.Values.ToArray();
            var colorIndex = EuclideanHelper.GetSmallestEuclideanDistanceIndex(candidateColors, color);

            return candidates.ElementAt(colorIndex).Key;
        }
    }
}
