using Kanvas.Contract.DataClasses;

namespace Kanvas.Contract.Configuration
{
    public delegate IImageSwizzle CreatePixelRemapperDelegate(SwizzleOptions context);

    public interface IRemapPixelsConfigurationBuilder
    {
        IImageConfigurationBuilder With(CreatePixelRemapperDelegate remapDelegate);
    }
}
