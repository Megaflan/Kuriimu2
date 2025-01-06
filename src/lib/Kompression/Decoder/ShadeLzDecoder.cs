using System.Buffers.Binary;
using Kompression.Contract.Decoder;
using Kompression.Decoder.Headerless;
using Kompression.Exceptions;

namespace Kompression.Decoder
{
    public class ShadeLzDecoder : IDecoder
    {
        private readonly ShadeLzHeaderlessDecoder _decoder;

        public ShadeLzDecoder()
        {
            _decoder = new ShadeLzHeaderlessDecoder();
        }

        public void Decode(Stream input, Stream output)
        {
            var buffer = new byte[8];

            _ = input.Read(buffer[..4]);
            if (buffer[0] is not 0xFC || buffer[1] is not 0xAA || buffer[2] is not 0x55 || buffer[3] is not 0xA7)
                throw new InvalidCompressionException("Spike Chunsoft");

            _ = input.Read(buffer);
            int decompressedSize = BinaryPrimitives.ReadInt32LittleEndian(buffer);
            int compressedSize = BinaryPrimitives.ReadInt32LittleEndian(buffer[4..]);

            _decoder.Decode(input, output, decompressedSize);
        }

        public void Dispose()
        {
        }
    }
}
