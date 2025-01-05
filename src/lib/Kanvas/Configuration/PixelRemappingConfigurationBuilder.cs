using Kanvas.Contract.Configuration;
using Kanvas.DataClasses.Configuration;

namespace Kanvas.Configuration
{
    internal class PixelRemappingConfigurationBuilder : IRemapPixelsConfigurationBuilder
    {
        private readonly IImageConfigurationBuilder _parent;
        private readonly PixelRemappingConfigurationOptions _options;

        public PixelRemappingConfigurationBuilder(IImageConfigurationBuilder parent, PixelRemappingConfigurationOptions options)
        {
            _parent = parent;
            _options = options;
        }

        public IImageConfigurationBuilder With(CreatePixelRemapperDelegate remapDelegate)
        {
            _options.PixelRemappingDelegate = remapDelegate;
            return _parent;
        }
    }
}
