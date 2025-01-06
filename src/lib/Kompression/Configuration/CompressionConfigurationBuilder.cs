using Kompression.Contract;
using Kompression.Contract.Configuration;
using Kompression.DataClasses.Configuration;

namespace Kompression.Configuration
{
    /// <summary>
    /// The main configuration to configure an <see cref="ICompression"/>.
    /// </summary>
    public class CompressionConfigurationBuilder : ILempelZivConfigurationBuilder, IHuffmanConfigurationBuilder, ILempelZivHuffmanConfigurationBuilder
    {
        private readonly CompressionConfigurationOptions _options;

        private readonly EncoderConfigurationBuilder _encoderConfigurationBuilder;
        private readonly DecoderConfigurationBuilder _decoderConfigurationBuilder;
        private readonly LempelZivOptionsBuilder _lempelZivBuilder;
        private readonly HuffmanOptionsBuilder _huffmanBuilder;

        public IEncoderConfigurationBuilder Encode => _encoderConfigurationBuilder;
        public IDecoderConfigurationBuilder Decode => _decoderConfigurationBuilder;

        public CompressionConfigurationBuilder() : this(new CompressionConfigurationOptions())
        {
        }

        private CompressionConfigurationBuilder(CompressionConfigurationOptions options)
        {
            _options = options;

            _encoderConfigurationBuilder = new EncoderConfigurationBuilder(this, _options.EncoderOptions);
            _decoderConfigurationBuilder = new DecoderConfigurationBuilder(this, _options.DecoderOptions);
            _lempelZivBuilder = new LempelZivOptionsBuilder(_options.LempelZiv);
            _huffmanBuilder = new HuffmanOptionsBuilder(_options.Huffman);
        }

        ICompressionConfigurationBuilder ILempelZivConfigurationBuilder.ConfigureLempelZiv(ConfigureLempelZivOptions configure)
        {
            configure(_lempelZivBuilder);
            return this;
        }

        ICompressionConfigurationBuilder IHuffmanConfigurationBuilder.ConfigureHuffman(ConfigureHuffmanOptions configure)
        {
            configure(_huffmanBuilder);
            return this;
        }

        ILempelZivConfigurationBuilder ILempelZivHuffmanConfigurationBuilder.ConfigureHuffman(ConfigureHuffmanOptions configure)
        {
            configure(_huffmanBuilder);
            return this;
        }

        IHuffmanConfigurationBuilder ILempelZivHuffmanConfigurationBuilder.ConfigureLempelZiv(ConfigureLempelZivOptions configure)
        {
            configure(_lempelZivBuilder);
            return this;
        }

        /// <summary>
        /// Builds the current configuration to an <see cref="ICompression"/>.
        /// </summary>
        /// <returns>The <see cref="ICompression"/> for this configuration.</returns>
        public ICompression Build() => new Compression(_options);

        public ICompressionConfigurationBuilder Clone()
        {
            var options = new CompressionConfigurationOptions
            {
                EncoderOptions =
                {
                    EncoderDelegate = _options.EncoderOptions.EncoderDelegate,
                    LempelZivEncoderDelegate = _options.EncoderOptions.LempelZivEncoderDelegate,
                    HuffmanEncoderDelegate = _options.EncoderOptions.HuffmanEncoderDelegate,
                    LempelZivHuffmanEncoderDelegate = _options.EncoderOptions.LempelZivHuffmanEncoderDelegate
                },
                DecoderOptions =
                {
                    DecoderDelegate = _options.DecoderOptions.DecoderDelegate
                },
                LempelZiv =
                {
                    TaskCount = _options.LempelZiv.TaskCount,
                    CreateMatchParserDelegate = _options.LempelZiv.CreateMatchParserDelegate
                },
                Huffman =
                {
                    TreeBuilderDelegate = _options.Huffman.TreeBuilderDelegate
                }
            };

            return new CompressionConfigurationBuilder(options);
        }
    }
}
