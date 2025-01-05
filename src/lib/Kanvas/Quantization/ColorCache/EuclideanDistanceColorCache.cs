using System.Collections.Concurrent;
using Kanvas.Contract.Quantization.ColorCache;
using Kanvas.Quantization.ColorCache.EuclideanDistance;
using SixLabors.ImageSharp.PixelFormats;

namespace Kanvas.Quantization.ColorCache
{
    /// <summary>
    /// The <see cref="IColorCache"/> to search colors with euclidean distance.
    /// </summary>
    public class EuclideanDistanceColorCache : ColorCache
    {
        private readonly ConcurrentDictionary<Rgba32, int> _cache;

        public EuclideanDistanceColorCache(IList<Rgba32> palette) :
            base(palette)
        {
            _cache = new ConcurrentDictionary<Rgba32, int>();
        }

        /// <inheritdoc />
        public override int GetPaletteIndex(Rgba32 color)
        {
            return _cache.AddOrUpdate(color,
                colorKey =>
                {
                    int paletteIndexInside = CalculatePaletteIndexInternal(color);
                    return paletteIndexInside;
                },
                (colorKey, inputIndex) => inputIndex);
        }

        private int CalculatePaletteIndexInternal(Rgba32 color)
        {
            return EuclideanHelper.GetSmallestEuclideanDistanceIndex(Palette, color);
        }
    }
}
