using Kanvas.Contract.Quantization.ColorCache;
using SixLabors.ImageSharp.PixelFormats;

namespace Kanvas.Contract.Quantization.ColorDitherer
{
    /// <summary>
    /// Describes methods to quantize and dither a collection of colors.
    /// </summary>
    public interface IColorDitherer
    {
        /// <summary>
        /// Quantizes and dithers a collection of colors.
        /// </summary>
        /// <param name="colors">The collection to quantize and dither.</param>
        /// <param name="colorCache"></param>
        /// <returns></returns>
        IEnumerable<int> Process(IEnumerable<Rgba32> colors, IColorCache colorCache);
    }
}
