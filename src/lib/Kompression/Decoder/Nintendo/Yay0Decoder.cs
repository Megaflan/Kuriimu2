﻿using Komponent.Contract.Enums;
using Komponent.IO;
using Komponent.Streams;
using Kompression.Contract.Decoder;
using Kompression.Exceptions;
using Kompression.IO;

namespace Kompression.Decoder.Nintendo
{
    public class Yay0Decoder : IDecoder
    {
        private readonly ByteOrder _byteOrder;

        public Yay0Decoder(ByteOrder byteOrder)
        {
            _byteOrder = byteOrder;
        }

        public void Decode(Stream input, Stream output)
        {
            long inputStartPosition = input.Position;

            var buffer = new byte[4];

            _ = input.Read(buffer);
            if (buffer[0] is not 0x59 || buffer[1] is not 0x61 || buffer[2] is not 0x79 || buffer[3] is not 0x30)
                throw new InvalidCompressionException("Yay0" + (_byteOrder == ByteOrder.LittleEndian ? "LE" : "BE"));

            using var br = new BinaryReaderX(input, true, _byteOrder);
            
            int uncompressedLength = br.ReadInt32();
            int compressedTableOffset = br.ReadInt32();
            int uncompressedTableOffset = br.ReadInt32();

            var circularBuffer = new CircularBuffer(0x1000);
            var compressedTablePosition = 0;
            var uncompressedTablePosition = 0;

            var bitStream = new SubStream(input, input.Position, compressedTableOffset - 0x10);
            using var bitReader = new BinaryBitReader(bitStream, BitOrder.MostSignificantBitFirst, 1, ByteOrder.BigEndian);

            while (output.Length < uncompressedLength)
            {
                if (bitReader.ReadBit() == 1)
                {
                    // Flag for uncompressed byte
                    input.Position = inputStartPosition + uncompressedTableOffset + uncompressedTablePosition++;
                    var value = (byte)input.ReadByte();

                    output.WriteByte(value);
                    circularBuffer.WriteByte(value);
                }
                else
                {
                    // Flag for compressed data
                    input.Position = inputStartPosition + compressedTableOffset + compressedTablePosition;
                    var firstByte = input.ReadByte();
                    var secondByte = input.ReadByte();
                    compressedTablePosition += 2;

                    var length = firstByte >> 4;
                    if (length > 0)
                        length += 2;
                    else
                    {
                        // Yes, we do read the length from the uncompressed data stream
                        input.Position = inputStartPosition + uncompressedTableOffset + uncompressedTablePosition++;
                        length = input.ReadByte() + 0x12;
                    }
                    var displacement = ((firstByte & 0xF) << 8 | secondByte) + 1;

                    circularBuffer.Copy(output, displacement, length);
                }
            }
        }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}
