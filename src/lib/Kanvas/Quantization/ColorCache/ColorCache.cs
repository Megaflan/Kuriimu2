using Kanvas.Contract.Quantization.ColorCache;
using SixLabors.ImageSharp.PixelFormats;

namespace Kanvas.Quantization.ColorCache
{
    public abstract class ColorCache : IColorCache
    {
        /// <inheritdoc />
        public IList<Rgba32> Palette { get; }

        public ColorCache(IList<Rgba32> palette)
        {
            Palette = palette;
        }

        /// <inheritdoc />
        public abstract int GetPaletteIndex(Rgba32 color);
    }
}
