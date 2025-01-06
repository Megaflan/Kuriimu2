using Kompression.Contract.Configuration;

namespace Kompression.DataClasses.Configuration
{
    internal class HuffmanOptions
    {
        public CreateHuffmanTreeBuilder? TreeBuilderDelegate { get; set; }
    }
}
