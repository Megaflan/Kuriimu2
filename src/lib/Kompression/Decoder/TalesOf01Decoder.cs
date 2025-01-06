using System.Buffers.Binary;
using Kompression.Contract.Decoder;
using Kompression.Decoder.Headerless;

namespace Kompression.Decoder
{
    public class TalesOf01Decoder : IDecoder
    {
        private Lzss01HeaderlessDecoder _decoder;

        public TalesOf01Decoder()
        {
            _decoder = new Lzss01HeaderlessDecoder();
        }

        public void Decode(Stream input, Stream output)
        {
            if (input.ReadByte() != 0x01)
                throw new InvalidOperationException("This is not a tales of compression with version 1.");

            var buffer = new byte[8];

            _ = input.Read(buffer);
            int compressedDataSize = BinaryPrimitives.ReadInt32LittleEndian(buffer);
            int decompressedSize = BinaryPrimitives.ReadInt32LittleEndian(buffer[4..]);

            _decoder.Decode(input, output, decompressedSize);
        }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}
