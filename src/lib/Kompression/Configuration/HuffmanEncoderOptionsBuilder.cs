using Kompression.Contract.Configuration;
using Kompression.DataClasses.Configuration;

namespace Kompression.Configuration
{
    internal class HuffmanEncoderOptionsBuilder : IHuffmanEncoderOptionsBuilder
    {
        private readonly HuffmanOptions _options;
        private readonly HuffmanEncoderOptions _encoderOptions;

        public HuffmanEncoderOptionsBuilder(HuffmanOptions options)
        {
            _options = options;
            _encoderOptions = new HuffmanEncoderOptions();
        }
    }
}
