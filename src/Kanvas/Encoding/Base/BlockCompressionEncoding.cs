﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kanvas.MoreEnumerable;
using Komponent.IO;
using Kontract.Kanvas;
using Kontract.Kanvas.Model;
using Kontract.Models.IO;
using SixLabors.ImageSharp.PixelFormats;

namespace Kanvas.Encoding.Base
{
    public abstract class BlockCompressionEncoding<TBlock> : IColorEncoding
    {
        private readonly ByteOrder _byteOrder;

        /// <inheritdoc cref="BitDepth"/>
        public abstract int BitDepth { get; }

        /// <inheritdoc cref="BitsPerValue"/>
        public abstract int BitsPerValue { get; protected set; }

        /// <inheritdoc cref="ColorsPerValue"/>
        public abstract int ColorsPerValue { get; }

        /// <inheritdoc cref="FormatName"/>
        public abstract string FormatName { get; }

        protected BlockCompressionEncoding(ByteOrder byteOrder)
        {
            _byteOrder = byteOrder;
        }

        /// <inheritdoc cref="Load"/>
        public IEnumerable<Rgba32> Load(byte[] input, EncodingLoadContext loadContext)
        {
            var br = new BinaryReaderX(new MemoryStream(input), _byteOrder);

            return ReadBlocks(br).AsParallel().AsOrdered()
                .WithDegreeOfParallelism(loadContext.TaskCount)
                .SelectMany(DecodeNextBlock);
        }

        /// <inheritdoc cref="Save"/>
        public byte[] Save(IEnumerable<Rgba32> colors, EncodingSaveContext saveContext)
        {
            var ms = new MemoryStream();
            using var bw = new BinaryWriterX(ms, _byteOrder);

            var blocks = colors.Batch(ColorsPerValue)
                .AsParallel().AsOrdered()
                .WithDegreeOfParallelism(saveContext.TaskCount)
                .Select(c => EncodeNextBlock(c.ToArray()));

            foreach (var block in blocks)
                WriteNextBlock(bw, block);

            return ms.ToArray();
        }

        protected abstract TBlock ReadNextBlock(BinaryReaderX br);

        protected abstract void WriteNextBlock(BinaryWriterX bw, TBlock block);

        protected abstract IList<Rgba32> DecodeNextBlock(TBlock block);

        protected abstract TBlock EncodeNextBlock(IList<Rgba32> colors);

        private IEnumerable<TBlock> ReadBlocks(BinaryReaderX br)
        {
            while (br.BaseStream.Position < br.BaseStream.Length)
                yield return ReadNextBlock(br);
        }
    }
}
