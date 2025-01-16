using Kompression.Contract.Configuration;
using Kompression.Encoder.Huffman;

namespace Kompression.DataClasses.Configuration
{
    internal class HuffmanOptions
    {
        public CreateHuffmanTreeBuilder? TreeBuilderDelegate { get; set; } = () => new HuffmanTreeBuilder();
    }
}
