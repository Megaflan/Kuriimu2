using Kompression.Contract.Decoder;
using Kompression.Contract.Enums.Encoder.Huffman;
using Kompression.Decoder.Headerless;
using Kompression.Exceptions;

namespace Kompression.Decoder.Level5
{
    public class HuffmanDecoder : IDecoder
    {
        private readonly int _bitDepth;
        private readonly HuffmanHeaderlessDecoder _decoder;

        public HuffmanDecoder(int bitDepth, NibbleOrder nibbleOrder)
        {
            _bitDepth = bitDepth;

            _decoder = new HuffmanHeaderlessDecoder(bitDepth, nibbleOrder);
        }

        public void Decode(Stream input, Stream output)
        {
            var buffer = new byte[4];

            _ = input.Read(buffer);

            int huffmanMode = _bitDepth == 4 ? 2 : 3;
            if ((buffer[0] & 0x7) != huffmanMode)
                throw new InvalidCompressionException($"Level5 Huffman{_bitDepth}");

            int decompressedSize = buffer[0] >> 3 | buffer[1] << 5 |
                                   buffer[2] << 13 | buffer[3] << 21;

            _decoder.Decode(input, output, decompressedSize);
        }

        public void Dispose()
        {
            // nothing to dispose
        }
    }
}
