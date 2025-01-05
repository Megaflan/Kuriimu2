using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Kanvas.Contract.Quantization
{
    public interface IQuantizer
    {
        Image<Rgba32> ProcessImage(Image<Rgba32> image);

        (IEnumerable<int>, IList<Rgba32>) Process(IEnumerable<Rgba32> colors, Size imageSize);
    }
}
