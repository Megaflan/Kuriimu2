﻿using Kompression.Contract.Decoder;
using Kompression.Decoder.Headerless;
using Kompression.Exceptions;

namespace Kompression.Decoder.Nintendo
{
    public class Lz10Decoder : IDecoder
    {
        private readonly Lz10HeaderlessDecoder _decoder;

        public Lz10Decoder()
        {
            _decoder = new Lz10HeaderlessDecoder();
        }

        public void Decode(Stream input, Stream output)
        {
            var buffer = new byte[4];

            _ = input.Read(buffer, 0, 4);
            if (buffer[0] != 0x10)
                throw new InvalidCompressionException("Nintendo Lz10");

            int decompressedSize = buffer[1] | buffer[2] << 8 | buffer[3] << 16;

            _decoder.Decode(input, output, decompressedSize);
        }

        public void Dispose()
        {
        }
    }
}
