using Kompression.Contract.Decoder;
using Kompression.Contract.Enums.Encoder.Huffman;
using Kompression.Decoder.Headerless;
using Kompression.Exceptions;

namespace Kompression.Decoder.Nintendo
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
            if (buffer[0] != 0x20 + _bitDepth)
                throw new InvalidCompressionException($"Nintendo Huffman{_bitDepth}");

            int decompressedLength = buffer[1] | buffer[2] << 8 | buffer[3] << 16;

            _decoder.Decode(input, output, decompressedLength);
        }

        public void Dispose()
        {
            // nothing to dispose
        }
    }
}
