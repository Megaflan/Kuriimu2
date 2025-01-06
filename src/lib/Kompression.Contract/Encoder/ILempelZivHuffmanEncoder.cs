using Kompression.Contract.Configuration;
using Kompression.Contract.DataClasses.Encoder.LempelZiv;
using Kompression.Contract.Encoder.Huffman;

namespace Kompression.Contract.Encoder
{
    /// <summary>
    /// Provides functionality to encode data.
    /// </summary>
    public interface ILempelZivHuffmanEncoder
    {
        /// <summary>
        /// Configures the options for this specification.
        /// </summary>
        /// <param name="lempelZivOptions">The match options to configure.</param>
        /// <param name="huffmanOptions">The huffman options to configure.</param>
        void Configure(ILempelZivEncoderOptionsBuilder lempelZivOptions, IHuffmanEncoderOptionsBuilder huffmanOptions);

        /// <summary>
        /// Encodes a stream of data.
        /// </summary>
        /// <param name="input">The input data to encode.</param>
        /// <param name="output">The output to encode to.</param>
        /// <param name="matches">The matches for the Lempel-Ziv compression.</param>
        /// <param name="treeBuilder">The tree builder for this huffman compression.</param>
        void Encode(Stream input, Stream output, IEnumerable<LempelZivMatch> matches, IHuffmanTreeBuilder treeBuilder);
    }
}
