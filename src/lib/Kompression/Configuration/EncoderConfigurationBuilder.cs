using Kompression.Contract.Configuration;
using Kompression.DataClasses.Configuration;

namespace Kompression.Configuration
{
    internal class EncoderConfigurationBuilder : IEncoderConfigurationBuilder
    {
        private readonly CompressionConfigurationBuilder _parent;
        private readonly EncoderConfigurationOptions _options;

        public EncoderConfigurationBuilder(CompressionConfigurationBuilder parent, EncoderConfigurationOptions options)
        {
            _parent = parent;
            _options = options;
        }

        public ICompressionConfigurationBuilder With(CreateEncoderDelegate encoderDelegate)
        {
            _options.EncoderDelegate = encoderDelegate;
            _options.LempelZivEncoderDelegate = null;
            _options.HuffmanEncoderDelegate = null;
            _options.LempelZivHuffmanEncoderDelegate = null;

            return _parent;
        }

        public ILempelZivConfigurationBuilder With(CreateLempelZivEncoderDelegate encoderDelegate)
        {
            _options.EncoderDelegate = null;
            _options.LempelZivEncoderDelegate = encoderDelegate;
            _options.HuffmanEncoderDelegate = null;
            _options.LempelZivHuffmanEncoderDelegate = null;

            return _parent;
        }

        public IHuffmanConfigurationBuilder With(CreateHuffmanEncoderDelegate encoderDelegate)
        {
            _options.EncoderDelegate = null;
            _options.LempelZivEncoderDelegate = null;
            _options.HuffmanEncoderDelegate = encoderDelegate;
            _options.LempelZivHuffmanEncoderDelegate = null;

            return _parent;
        }

        public ILempelZivHuffmanConfigurationBuilder With(CreateLempelZivHuffmanEncoderDelegate encoderDelegate)
        {
            _options.EncoderDelegate = null;
            _options.LempelZivEncoderDelegate = null;
            _options.HuffmanEncoderDelegate = null;
            _options.LempelZivHuffmanEncoderDelegate = encoderDelegate;

            return _parent;
        }
    }
}
