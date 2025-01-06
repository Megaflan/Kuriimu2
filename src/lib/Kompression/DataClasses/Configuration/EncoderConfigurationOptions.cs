using Kompression.Contract.Configuration;

namespace Kompression.DataClasses.Configuration
{
    internal class EncoderConfigurationOptions
    {
        public CreateEncoderDelegate? EncoderDelegate { get; set; }
        public CreateLempelZivEncoderDelegate? LempelZivEncoderDelegate { get; set; }
        public CreateHuffmanEncoderDelegate? HuffmanEncoderDelegate { get; set; }
        public CreateLempelZivHuffmanEncoderDelegate? LempelZivHuffmanEncoderDelegate { get; set; }
    }
}
