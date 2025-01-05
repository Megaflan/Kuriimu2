using SixLabors.ImageSharp.PixelFormats;

namespace Kanvas.Contract
{
    public interface IColorShader
    {
        Rgba32 Read(Rgba32 c);

        Rgba32 Write(Rgba32 c);
    }
}
