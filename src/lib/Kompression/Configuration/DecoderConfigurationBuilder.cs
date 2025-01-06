using Kompression.Contract.Configuration;
using Kompression.DataClasses.Configuration;

namespace Kompression.Configuration
{
    internal class DecoderConfigurationBuilder : IDecoderConfigurationBuilder
    {
        private readonly CompressionConfigurationBuilder _parent;
        private readonly DecoderConfigurationOptions _options;

        public DecoderConfigurationBuilder(CompressionConfigurationBuilder parent, DecoderConfigurationOptions options)
        {
            _parent = parent;
            _options = options;
        }

        public ICompressionConfigurationBuilder With(CreateDecoderDelegate decoderDelegate)
        {
            _options.DecoderDelegate = decoderDelegate;
            return _parent;
        }
    }
}
