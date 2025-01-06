using System.Buffers.Binary;
using Kompression.Contract.Configuration;
using Kompression.Contract.DataClasses.Encoder.LempelZiv;
using Kompression.Contract.Encoder;
using Kompression.Encoder.Headerless;

namespace Kompression.Encoder
{
    public class IecpEncoder : ILempelZivEncoder
    {
        private Lzss01HeaderlessEncoder _encoder;

        public IecpEncoder()
        {
            _encoder = new Lzss01HeaderlessEncoder();
        }

        public void Configure(ILempelZivEncoderOptionsBuilder matchOptions)
        {
            _encoder.Configure(matchOptions);
        }

        public void Encode(Stream input, Stream output, IEnumerable<LempelZivMatch> matches)
        {
            output.Position += 8;

            _encoder.Encode(input, output, matches);

            WriteHeaderData(output, (int)input.Length);
        }

        private void WriteHeaderData(Stream output, int decompressedLength)
        {
            var endPosition = output.Position;
            output.Position = 0;

            var buffer = new byte[] { 0x49, 0x45, 0x43, 0x50 };
            output.Write(buffer);

            BinaryPrimitives.WriteInt32LittleEndian(buffer, decompressedLength);
            output.Write(buffer);

            output.Position = endPosition;
        }

        public void Dispose()
        {
        }
    }
}
