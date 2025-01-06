﻿using Kompression.Contract.Configuration;
using Kompression.Contract.DataClasses.Encoder.LempelZiv;
using Kompression.Contract.Encoder;
using Kompression.Encoder.LempelZiv.PriceCalculators;

namespace Kompression.Encoder.Headerless
{
    class ShadeLzHeaderlessEncoder : ILempelZivEncoder
    {
        public void Configure(ILempelZivEncoderOptionsBuilder matchOptions)
        {
            matchOptions.CalculatePricesWith(() => new SpikeChunsoftPriceCalculator())
                .FindPatternMatches().WithinLimitations(4, -1, 1, 0x1FFF)
                .AndFindRunLength().WithinLimitations(4, 0x1003);
        }

        public void Encode(Stream input, Stream output, IEnumerable<LempelZivMatch> matches)
        {
            foreach (var match in matches)
            {
                if (input.Position < match.Position)
                    WriteRawData(input, output, match.Position - input.Position);

                WriteMatchData(input, output, match);
            }

            if (input.Position < input.Length)
                WriteRawData(input, output, input.Length - input.Position);
        }

        private void WriteRawData(Stream input, Stream output, long length)
        {
            while (length > 0)
            {
                var cappedLength = Math.Min(length, 0x1FFF);
                if (cappedLength <= 0x1F)
                    output.WriteByte((byte)cappedLength);
                else
                {
                    output.WriteByte((byte)(0x20 | cappedLength >> 8));
                    output.WriteByte((byte)cappedLength);
                }

                for (var i = 0; i < cappedLength; i++)
                    output.WriteByte((byte)input.ReadByte());

                length -= cappedLength;
            }
        }

        private void WriteMatchData(Stream input, Stream output, LempelZivMatch lempelZivMatch)
        {
            var length = lempelZivMatch.Length - 4;
            if (lempelZivMatch.Displacement == 0)
            {
                // Rle
                if (length <= 0xF)
                    output.WriteByte((byte)(0x40 | length));
                else
                {
                    output.WriteByte((byte)(0x50 | length >> 8));
                    output.WriteByte((byte)length);
                }

                output.WriteByte((byte)input.ReadByte());
                input.Position--;
            }
            else
            {
                // Lz

                // Write displacement part first
                var cappedLength = Math.Min(length, 3);

                output.WriteByte((byte)(0x80 | cappedLength << 5 | lempelZivMatch.Displacement >> 8));
                output.WriteByte((byte)lempelZivMatch.Displacement);

                length -= cappedLength;
                while (length > 0)
                {
                    cappedLength = Math.Min(length, 0x1F);

                    output.WriteByte((byte)(0x60 | cappedLength));

                    length -= cappedLength;
                }
            }

            input.Position += lempelZivMatch.Length;
        }

        public void Dispose()
        {
        }
    }
}
