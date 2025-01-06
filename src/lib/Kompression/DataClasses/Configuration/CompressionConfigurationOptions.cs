namespace Kompression.DataClasses.Configuration
{
    internal class CompressionConfigurationOptions
    {
        public EncoderConfigurationOptions EncoderOptions { get; set; } = new();
        public DecoderConfigurationOptions DecoderOptions { get; set; } = new();
        public LempelZivOptions LempelZiv { get; set; } = new();
        public HuffmanOptions Huffman { get; set; } = new();
    }
}
