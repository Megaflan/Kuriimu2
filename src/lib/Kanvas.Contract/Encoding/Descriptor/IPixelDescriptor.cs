using SixLabors.ImageSharp.PixelFormats;

namespace Kanvas.Contract.Encoding.Descriptor
{
    public interface IPixelDescriptor
    {
        string GetPixelName();

        int GetBitDepth();

        Rgba32 GetColor(long value);

        long GetValue(Rgba32 color);
    }
}
