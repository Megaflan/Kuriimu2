using Kanvas.Contract.Encoding;

namespace Kanvas.Contract.Configuration
{
    public interface IEncodingConfigurationBuilder
    {
        IImageConfigurationBuilder With(IColorEncoding encoding);

        IIndexedImageConfigurationBuilder With(IIndexEncoding encoding);
    }
}
