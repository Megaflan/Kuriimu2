using Kanvas.Contract.Encoding;

namespace Kanvas.Contract.Configuration
{
    public interface IPaletteEncodingConfigurationBuilder
    {
        IIndexedImageConfigurationBuilder With(IColorEncoding encoding);
    }
}
