using Komponent.IO;
using Kompression.Contract.Configuration;
using Kompression.Contract.DataClasses.Encoder.LempelZiv;
using Kompression.Contract.Encoder;
using Kompression.Encoder.Headerless;

namespace Kompression.Encoder
{
    public class TalesOf01Encoder : ILempelZivEncoder
    {
        private Lzss01HeaderlessEncoder _encoder;

        public TalesOf01Encoder()
        {
            _encoder = new Lzss01HeaderlessEncoder();
        }

        public void Configure(ILempelZivEncoderOptionsBuilder matchOptions)
        {
            _encoder.Configure(matchOptions);
        }

        public void Encode(Stream input, Stream output, IEnumerable<LempelZivMatch> matches)
        {
            output.Position += 9;

            _encoder.Encode(input, output, matches);

            WriteHeaderData(output, (int)input.Length);
        }

        private void WriteHeaderData(Stream output, int decompressedLength)
        {
            var endPosition = output.Position;
            output.Position = 0;

            using var bw = new BinaryWriterX(output, true);

            bw.Write((byte)1);
            bw.Write((int)output.Length);
            bw.Write(decompressedLength);

            output.Position = endPosition;
        }

        public void Dispose()
        {
        }
    }
}
