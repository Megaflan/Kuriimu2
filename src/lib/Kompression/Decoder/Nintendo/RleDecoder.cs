using Kompression.Contract.Decoder;
using Kompression.Decoder.Headerless;
using Kompression.Exceptions;

namespace Kompression.Decoder.Nintendo
{
    public class RleDecoder : IDecoder
    {
        private readonly RleHeaderlessDecoder _decoder;

        public RleDecoder()
        {
            _decoder = new RleHeaderlessDecoder();
        }

        public void Decode(Stream input, Stream output)
        {
            var buffer = new byte[4];

            _ = input.Read(buffer, 0, 4);
            if (buffer[0] != 0x30)
                throw new InvalidCompressionException("Nintendo Rle");

            int decompressedSize = buffer[1] | buffer[2] << 8 | buffer[3] << 16;

            _decoder.Decode(input, output, decompressedSize);
        }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}
