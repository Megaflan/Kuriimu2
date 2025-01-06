using Kompression.Contract.Encoder.Huffman;

namespace Kompression.Contract.Configuration
{
    public delegate IHuffmanTreeBuilder CreateHuffmanTreeBuilder();

    public interface IHuffmanOptionsBuilder
    {
        IHuffmanOptionsBuilder BuildTreeWith(CreateHuffmanTreeBuilder treeDelegate);
    }
}
