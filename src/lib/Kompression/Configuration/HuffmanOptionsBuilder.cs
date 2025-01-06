using Kompression.Contract.Configuration;
using Kompression.DataClasses.Configuration;

namespace Kompression.Configuration
{
    internal class HuffmanOptionsBuilder : IHuffmanOptionsBuilder
    {
        private readonly HuffmanOptions _options;

        public HuffmanOptionsBuilder(HuffmanOptions options)
        {
            _options = options;
        }

        public IHuffmanOptionsBuilder BuildTreeWith(CreateHuffmanTreeBuilder treeDelegate)
        {
            _options.TreeBuilderDelegate = treeDelegate;
            return this;
        }
    }
}
