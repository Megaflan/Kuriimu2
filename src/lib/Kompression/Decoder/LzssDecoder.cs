using System.Buffers.Binary;
using Kompression.Contract.Decoder;
using Kompression.Decoder.Headerless;
using Kompression.Exceptions;

namespace Kompression.Decoder
{
    public class LzssDecoder : IDecoder
    {
        private readonly Lz10HeaderlessDecoder _decoder;

        public LzssDecoder()
        {
            _decoder = new Lz10HeaderlessDecoder();
        }

        public void Decode(Stream input, Stream output)
        {
            var buffer = new byte[4];

            _ = input.Read(buffer);
            if (buffer[0] is not 0x53 || buffer[1] is not 0x53 || buffer[2] is not 0x5A || buffer[3] is not 0x4C)   // "SSZL"
                throw new InvalidCompressionException("Lzss");

            input.Position = 0xC;
            _ = input.Read(buffer);
            int decompressedSize = BinaryPrimitives.ReadInt32LittleEndian(buffer);

            _decoder.Decode(input, output, decompressedSize);
        }

        public void Dispose()
        {
        }
    }
}
