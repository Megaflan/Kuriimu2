using System.Buffers.Binary;
using Komponent.IO;
using Kompression.Contract.Configuration;
using Kompression.Contract.DataClasses.Encoder.LempelZiv;
using Kompression.Contract.Encoder;
using Kompression.Encoder.Headerless;

namespace Kompression.Encoder
{
    public class LzssEncoder : ILempelZivEncoder
    {
        private Lz10HeaderlessEncoder _encoder;

        public LzssEncoder()
        {
            _encoder = new Lz10HeaderlessEncoder();
        }

        public void Configure(ILempelZivEncoderOptionsBuilder matchOptions)
        {
            _encoder.Configure(matchOptions);
        }

        public void Encode(Stream input, Stream output, IEnumerable<LempelZivMatch> matches)
        {
            var outputStartPos = output.Position;
            output.Position += 0x10;
            _encoder.Encode(input, output, matches);

            using var bw = new BinaryWriterX(output, true);

            var outputPos = output.Position;
            var buffer = new byte[] { 0x53, 0x53, 0x5A, 0x4C };

            output.Position = outputStartPos;
            bw.Write(buffer);

            output.Position += 8;
            bw.Write((int)input.Length);

            output.Position = outputPos;
        }

        public void Dispose()
        {
            _encoder = null;
        }
    }
}
