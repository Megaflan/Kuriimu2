using Komponent.IO;
using Kompression.Contract.Configuration;
using Kompression.Contract.DataClasses.Encoder.LempelZiv;
using Kompression.Contract.Encoder;
using Kompression.Contract.Enums.Encoder.LempelZiv;
using Kompression.Encoder.LempelZiv.PriceCalculators;

namespace Kompression.Encoder
{
    // TODO: Refactor block class
    class Wp16Encoder : ILempelZivEncoder
    {
        private const int PreBufferSize_ = 0xFFE;

        class Block
        {
            public long flagBuffer;
            public int flagPosition;

            // at max 32 matches, one match is 2 bytes
            public byte[] buffer = new byte[32 * 2];
            public int bufferLength;
        }

        public void Configure(ILempelZivEncoderOptionsBuilder matchOptions)
        {
            matchOptions.CalculatePricesWith(() => new Wp16PriceCalculator())
                .FindPatternMatches().WithinLimitations(4, 0x42, 2, 0xFFE)
                .AdjustInput(input => input.Prepend(PreBufferSize_))
                .HasUnitSize(UnitSize.Short);
        }

        public void Encode(Stream input, Stream output, IEnumerable<LempelZivMatch> matches)
        {
            var block = new Block();

            using var bw = new BinaryWriterX(output, true);

            bw.WriteString("Wp16", writeNullTerminator: false);
            bw.Write((int)input.Length);

            foreach (var match in matches)
            {
                // Compress raw data
                if (input.Position < match.Position)
                    CompressRawData(input, output, block, (int)(match.Position - input.Position));

                // Compress match
                CompressMatchData(input, output, block, match);
            }

            // Compress raw data
            if (input.Position < input.Length)
                CompressRawData(input, output, block, (int)(input.Length - input.Position));

            if (block.flagPosition > 0)
                WriteAndResetBuffer(output, block);
        }

        private void CompressRawData(Stream input, Stream output, Block block, int rawLength)
        {
            while (rawLength > 0)
            {
                if (block.flagPosition == 32)
                    WriteAndResetBuffer(output, block);

                rawLength -= 2;
                block.flagBuffer |= 1L << block.flagPosition++;

                block.buffer[block.bufferLength++] = (byte)input.ReadByte();
                block.buffer[block.bufferLength++] = (byte)input.ReadByte();
            }

            if (block.flagPosition == 32)
                WriteAndResetBuffer(output, block);
        }

        private void CompressMatchData(Stream input, Stream output, Block block, LempelZivMatch lempelZivMatch)
        {
            if (block.flagPosition == 32)
                WriteAndResetBuffer(output, block);

            block.flagPosition++;

            var byte1 = (byte)(lempelZivMatch.Length / 2 - 2 & 0x1F);
            byte1 |= (byte)((lempelZivMatch.Displacement / 2 & 0x7) << 5);
            var byte2 = (byte)(lempelZivMatch.Displacement / 2 >> 3);

            block.buffer[block.bufferLength++] = byte1;
            block.buffer[block.bufferLength++] = byte2;

            if (block.flagPosition == 32)
                WriteAndResetBuffer(output, block);

            input.Position += lempelZivMatch.Length;
        }

        private void WriteAndResetBuffer(Stream output, Block block)
        {
            using var bw = new BinaryWriterX(output, true);

            // Write data to output
            bw.Write((int)block.flagBuffer);
            output.Write(block.buffer, 0, block.bufferLength);

            // Reset codeBlock and buffer
            block.flagBuffer = 0;
            block.flagPosition = 0;
            Array.Clear(block.buffer, 0, block.bufferLength);
            block.bufferLength = 0;
        }

        public void Dispose()
        {
        }
    }
}
