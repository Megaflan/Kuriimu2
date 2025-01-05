using Kanvas.Contract.Configuration;
using Kanvas.Contract.Encoding;
using Kanvas.DataClasses.Configuration;

namespace Kanvas.Configuration
{
    internal class PaletteEncodingConfigurationBuilder : IPaletteEncodingConfigurationBuilder
    {
        private readonly IIndexedImageConfigurationBuilder _parent;
        private readonly EncodingConfigurationOptions _options;

        public PaletteEncodingConfigurationBuilder(IIndexedImageConfigurationBuilder parent, EncodingConfigurationOptions options)
        {
            _parent = parent;
            _options = options;
        }

        public IIndexedImageConfigurationBuilder With(IColorEncoding encoding)
        {
            _options.PaletteEncoding = encoding;
            return _parent;
        }
    }
}
