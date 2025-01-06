using Komponent.Contract.Enums;
using Komponent.IO;
using Kompression.Contract.Configuration;
using Kompression.Contract.DataClasses.Encoder.LempelZiv;
using Kompression.Contract.Encoder;
using Kompression.Encoder.LempelZiv.PriceCalculators;

namespace Kompression.Encoder
{
    // TODO: Refactor block class
    public class LzEcdEncoder : ILempelZivEncoder
    {
        class Block
        {
            public byte codeBlock;
            public int codeBlockPosition;

            // each buffer can be at max 8 pairs of compressed matches; a compressed match is 2 bytes
            public byte[] buffer = new byte[8 * 2];
            public int bufferLength;
        }

        private const int WindowBufferLength_ = 0x400;
        private const int PreBufferSize_ = 0x3BE;

        public void Configure(ILempelZivEncoderOptionsBuilder matchOptions)
        {
            matchOptions.CalculatePricesWith(() => new LzEcdPriceCalculator())
                .FindPatternMatches().WithinLimitations(3, 0x42, 1, 0x400)
                .AdjustInput(input => input.Prepend(PreBufferSize_));
        }

        public void Encode(Stream input, Stream output, IEnumerable<LempelZivMatch> matches)
        {
            var originalOutputPosition = output.Position;
            output.Position += 0x10;

            var block = new Block();

            foreach (var match in matches)
            {
                // Write any data before the match, to the uncompressed table
                while (input.Position < match.Position)
                {
                    if (block.codeBlockPosition == 8)
                        WriteAndResetBuffer(output, block);

                    block.codeBlock |= (byte)(1 << block.codeBlockPosition++);
                    block.buffer[block.bufferLength++] = (byte)input.ReadByte();
                }

                // Write match data to the buffer
                var bufferPosition = (PreBufferSize_ + match.Position - match.Displacement) % WindowBufferLength_;
                var firstByte = (byte)bufferPosition;
                var secondByte = (byte)(bufferPosition >> 2 & 0xC0 | (byte)(match.Length - 3));

                if (block.codeBlockPosition == 8)
                    WriteAndResetBuffer(output, block);

                block.codeBlockPosition++; // Since a match is flagged with a 0 bit, we don't need a bit shift and just increase the position
                block.buffer[block.bufferLength++] = firstByte;
                block.buffer[block.bufferLength++] = secondByte;

                input.Position += match.Length;
            }

            // Write any data after last match, to the buffer
            while (input.Position < input.Length)
            {
                if (block.codeBlockPosition == 8)
                    WriteAndResetBuffer(output, block);

                block.codeBlock |= (byte)(1 << block.codeBlockPosition++);
                block.buffer[block.bufferLength++] = (byte)input.ReadByte();
            }

            // Flush remaining buffer to stream
            if (block.codeBlockPosition > 0)
                WriteAndResetBuffer(output, block);

            // Write header information
            WriteHeaderData(input, output, originalOutputPosition);
        }

        private void WriteAndResetBuffer(Stream output, Block block)
        {
            // Write data to output
            output.WriteByte(block.codeBlock);
            output.Write(block.buffer, 0, block.bufferLength);

            // Reset codeBlock and buffer
            block.codeBlock = 0;
            block.codeBlockPosition = 0;
            Array.Clear(block.buffer, 0, block.bufferLength);
            block.bufferLength = 0;
        }

        private void WriteHeaderData(Stream input, Stream output, long originalOutputPosition)
        {
            var outputEndPosition = output.Position;

            // Write header
            using var bw = new BinaryWriterX(output, true, ByteOrder.BigEndian);
            output.Position = originalOutputPosition;

            bw.WriteString("ECD\x1", writeNullTerminator: false);
            bw.Write(0);
            bw.Write((int)(output.Length - 0x10));
            bw.Write((int)input.Length);

            output.Position = outputEndPosition;
        }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}
