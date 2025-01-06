﻿using System.Text;
using Komponent.Contract.Enums;
using Komponent.IO;
using Kompression.Contract.Configuration;
using Kompression.Contract.DataClasses.Encoder.LempelZiv;
using Kompression.Contract.Encoder;
using Kompression.Encoder.LempelZiv.PriceCalculators;

namespace Kompression.Encoder.Nintendo
{
    public class Yay0Encoder : ILempelZivEncoder
    {
        private readonly ByteOrder _byteOrder;

        public Yay0Encoder(ByteOrder byteOrder)
        {
            _byteOrder = byteOrder;
        }

        public void Configure(ILempelZivEncoderOptionsBuilder matchOptions)
        {
            matchOptions.CalculatePricesWith(() => new Yay0PriceCalculator())
                .FindPatternMatches().WithinLimitations(3, 0x111, 1, 0x1000);
        }

        public void Encode(Stream input, Stream output, IEnumerable<LempelZivMatch> matches)
        {
            var bitLayoutStream = new MemoryStream();
            var compressedTableStream = new MemoryStream();
            var uncompressedTableStream = new MemoryStream();

            using var bitLayoutWriter = new BinaryBitWriter(bitLayoutStream, BitOrder.MostSignificantBitFirst, 1, ByteOrder.BigEndian);
            using var bwCompressed = new BinaryWriter(compressedTableStream, Encoding.ASCII, true);
            using var bwUncompressed = new BinaryWriter(uncompressedTableStream, Encoding.ASCII, true);

            foreach (var match in matches)
            {
                // Write any data before the match, to the uncompressed table
                while (input.Position < match.Position)
                {
                    bitLayoutWriter.WriteBit(1);
                    bwUncompressed.Write((byte)input.ReadByte());
                }

                // Write match data to the compressed table
                var firstByte = (byte)(match.Displacement - 1 >> 8);
                var secondByte = (byte)(match.Displacement - 1);

                if (match.Length < 0x12)
                    // Since minimum _length should be 3 for Yay0, we get a minimum matchLength of 1 in this case
                    firstByte |= (byte)(match.Length - 2 << 4);
                else
                    // Yes, we do write the _length for a match into the uncompressed data stream, if it's >=0x12
                    bwUncompressed.Write((byte)(match.Length - 0x12));

                bitLayoutWriter.WriteBit(0);
                bwCompressed.Write(firstByte);
                bwCompressed.Write(secondByte);

                input.Position += match.Length;
            }

            // Write any data after last match, to the uncompressed table
            while (input.Position < input.Length)
            {
                bitLayoutWriter.WriteBit(1);
                bwUncompressed.Write((byte)input.ReadByte());
            }

            bitLayoutWriter.Flush();

            WriteCompressedData(input, output, bitLayoutStream, compressedTableStream, uncompressedTableStream);
        }

        private void WriteCompressedData(Stream input, Stream output, Stream bitLayoutStream, Stream compressedTableStream, Stream uncompressedTableStream)
        {
            // Create header values
            var compressedTableOffsetInt = (int)(0x10 + (bitLayoutStream.Length + 3 & ~3));

            // Write header
            using var bw = new BinaryWriterX(output, true, _byteOrder);

            bw.WriteString("Yay0", writeNullTerminator: false);
            bw.Write((int)input.Length);
            bw.Write(compressedTableOffsetInt);
            bw.Write((int)(compressedTableOffsetInt + compressedTableStream.Length));

            // Write data streams
            bitLayoutStream.Position = 0;
            bitLayoutStream.CopyTo(output);
            output.Position = output.Position + 3 & ~3;

            compressedTableStream.Position = 0;
            compressedTableStream.CopyTo(output);

            uncompressedTableStream.Position = 0;
            uncompressedTableStream.CopyTo(output);
        }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}
