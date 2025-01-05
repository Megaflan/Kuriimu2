using Kanvas.Contract.Configuration;
using Kanvas.Contract.Encoding;
using Kanvas.DataClasses.Configuration;

namespace Kanvas.Configuration
{
    internal class EncodingConfigurationBuilder : IEncodingConfigurationBuilder
    {
        private readonly IIndexedImageConfigurationBuilder _parent;
        private readonly EncodingConfigurationOptions _options;

        public EncodingConfigurationBuilder(IIndexedImageConfigurationBuilder parent, EncodingConfigurationOptions options)
        {
            _parent = parent;
            _options = options;
        }

        public IImageConfigurationBuilder With(IColorEncoding encoding)
        {
            _options.ColorEncoding = encoding;
            _options.IndexEncoding = null;

            return _parent;
        }

        public IIndexedImageConfigurationBuilder With(IIndexEncoding encoding)
        {
            _options.IndexEncoding = encoding;
            _options.ColorEncoding = null;

            return _parent;
        }
    }
}
