using Komponent.IO;
using Kompression.Contract.Configuration;
using Kompression.Contract.DataClasses.Encoder.LempelZiv;
using Kompression.Contract.Encoder;
using Kompression.Encoder.LempelZiv.PriceCalculators;

namespace Kompression.Encoder
{
    // TODO: Refactor block class
    public class TalesOf03Encoder : ILempelZivEncoder
    {
        private const int WindowBufferLength_ = 0x1000;
        private const int PreBufferSize_ = 0xFEF;

        class Block
        {
            public byte[] buffer = new byte[1 + 8 * 3];
            public int bufferLength = 1;
            public int flagCount;
        }

        public void Configure(ILempelZivEncoderOptionsBuilder matchOptions)
        {
            matchOptions.CalculatePricesWith(() => new TalesOf03PriceCalculator())
                .FindPatternMatches().WithinLimitations(3, 0x11, 1, 0x1000)
                .AndFindRunLength().WithinLimitations(4, 0x112)
                .AdjustInput(input => input.Prepend(PreBufferSize_));
        }

        public void Encode(Stream input, Stream output, IEnumerable<LempelZivMatch> matches)
        {
            var block = new Block();

            output.Position += 9;

            foreach (var match in matches)
            {
                if (input.Position < match.Position)
                    WriteRawData(input, output, block, match.Position - input.Position);

                WriteMatchData(input, output, block, match);
            }

            if (input.Position < input.Length)
                WriteRawData(input, output, block, input.Length - input.Position);

            if (block.flagCount > 0)
                WriteAndResetBuffer(output, block);

            WriteHeaderData(output, (int)input.Length);
        }

        private void WriteRawData(Stream input, Stream output, Block block, long rawLength)
        {
            for (var i = 0; i < rawLength; i++)
            {
                if (block.flagCount == 8)
                    WriteAndResetBuffer(output, block);

                block.buffer[0] |= (byte)(1 << block.flagCount++);
                block.buffer[block.bufferLength++] = (byte)input.ReadByte();
            }
        }

        private void WriteMatchData(Stream input, Stream output, Block block, LempelZivMatch lempelZivMatch)
        {
            if (block.flagCount == 8)
                WriteAndResetBuffer(output, block);

            if (lempelZivMatch.Displacement == 0)
            {
                // Encode RLE
                if (lempelZivMatch.Length >= 0x13)
                {
                    var byte2 = (byte)0x0F;
                    var byte1 = (byte)(lempelZivMatch.Length - 0x13);

                    block.buffer[block.bufferLength++] = byte1;
                    block.buffer[block.bufferLength++] = byte2;
                    block.buffer[block.bufferLength++] = (byte)input.ReadByte();
                    input.Position += lempelZivMatch.Length - 1;
                }
                else
                {
                    var byte2 = (byte)((lempelZivMatch.Length - 3 & 0xF) << 4);
                    byte2 |= 0xF;
                    var byte1 = (byte)input.ReadByte();

                    block.buffer[block.bufferLength++] = byte1;
                    block.buffer[block.bufferLength++] = byte2;
                    input.Position += lempelZivMatch.Length - 1;
                }
            }
            else
            {
                // Encode LZ
                var bufferPosition = (lempelZivMatch.Position - lempelZivMatch.Displacement + PreBufferSize_) % WindowBufferLength_;

                var byte1 = (byte)bufferPosition;
                var byte2 = (byte)(lempelZivMatch.Length - 3 & 0xF);
                byte2 |= (byte)(bufferPosition >> 4 & 0xF0);

                block.buffer[block.bufferLength++] = byte1;
                block.buffer[block.bufferLength++] = byte2;
                input.Position += lempelZivMatch.Length;
            }

            block.flagCount++;
        }

        private void WriteHeaderData(Stream output, int decompressedLength)
        {
            var endPosition = output.Position;
            output.Position = 0;

            using var bw = new BinaryWriterX(output, true);

            bw.Write((byte)3);
            bw.Write((int)output.Length);
            bw.Write(decompressedLength);

            output.Position = endPosition;
        }

        private void WriteAndResetBuffer(Stream output, Block block)
        {
            output.Write(block.buffer, 0, block.bufferLength);

            Array.Clear(block.buffer, 0, block.bufferLength);
            block.bufferLength = 1;
            block.flagCount = 0;
        }

        public void Dispose()
        {
        }
    }
}
