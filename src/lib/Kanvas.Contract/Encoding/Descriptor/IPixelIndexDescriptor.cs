using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;

namespace Kanvas.Contract.Encoding.Descriptor
{
    public interface IPixelIndexDescriptor
    {
        string GetPixelName();

        int GetBitDepth();

        Rgba32 GetColor(long value, IList<Rgba32> palette);

        long GetValue(int index, IList<Rgba32> palette);
    }
}
