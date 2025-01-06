using Kompression.Contract.Decoder;
using Kompression.Decoder.Headerless;
using Kompression.Exceptions;

namespace Kompression.Decoder.Level5
{
    public class Lz10Decoder : IDecoder
    {
        private readonly Lz10HeaderlessDecoder _decoder;

        public Lz10Decoder()
        {
            _decoder = new Lz10HeaderlessDecoder();
        }

        public void Decode(Stream input, Stream output)
        {
            var buffer = new byte[4];

            _ = input.Read(buffer, 0, 4);
            if ((buffer[0] & 0x7) != 0x1)
                throw new InvalidCompressionException("Level5 Lz10");

            int decompressedSize = buffer[0] >> 3 | buffer[1] << 5 |
                                   buffer[2] << 13 | buffer[3] << 21;

            _decoder.Decode(input, output, decompressedSize);
        }

        public void Dispose()
        {
        }
    }
}
