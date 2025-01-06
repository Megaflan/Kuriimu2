using Kompression.Contract.Configuration;
using Kompression.Contract.Encoder.Huffman;

namespace Kompression.Contract.Encoder
{
    /// <summary>
    /// Provides functionality to encode data.
    /// </summary>
    public interface IHuffmanEncoder
    {
        /// <summary>
        /// Configures the huffman options for this specification.
        /// </summary>
        /// <param name="huffmanOptions">The options to configure.</param>
        void Configure(IHuffmanEncoderOptionsBuilder huffmanOptions);

        /// <summary>
        /// Encodes a stream of data.
        /// </summary>
        /// <param name="input">The input data to encode.</param>
        /// <param name="output">The output to encode to.</param>
        /// <param name="treeBuilder">The tree builder for this huffman compression.</param>
        void Encode(Stream input, Stream output, IHuffmanTreeBuilder treeBuilder);
    }
}
