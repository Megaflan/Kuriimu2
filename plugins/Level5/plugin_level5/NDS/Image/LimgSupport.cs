﻿using Kanvas.Contract.Encoding;
using Kanvas.Encoding;
using Komponent.Contract.Aspects;
using Komponent.Contract.Enums;
using Konnect.Plugin.File.Image;

#pragma warning disable 649

namespace plugin_level5.NDS.Image
{
    class LimgHeader
    {
        [FixedLength(4)]
        public string magic;

        public uint paletteOffset;

        public short unkOffset1;
        public short unkCount1;     // Size 0x8
        public short unkOffset2;
        public short unkCount2;     // Size 0xc

        public short tileDataOffset;
        public short tileEntryCount;    // Size 0x2
        public short imageDataOffset;
        public short imageTileCount;    // Size 0x40

        public short imgFormat;
        public short colorCount;
        public short width;
        public short height;

        public short paddedWidth;
        public short paddedHeight;
    }

    class LimgSupport
    {
        public static IDictionary<int, (IIndexEncoding, int[])> LimgFormats = new Dictionary<int, (IIndexEncoding, int[])>
        {
            [0] = (new Kanvas.Encoding.Index(4, ByteOrder.LittleEndian, BitOrder.LeastSignificantBitFirst), new[] { 0 }),
            [1] = (new Kanvas.Encoding.Index(8), new[] { 0 }),
            [2] = (new Kanvas.Encoding.Index(5, 3), new[] { 0 }),
        };

        public static IDictionary<int, IColorEncoding> LimgPaletteFormats = new Dictionary<int, IColorEncoding>
        {
            [0] = new Rgba(5, 5, 5, "BGR")
        };

        public static EncodingDefinition GetEncodingDefinition()
        {
            var encodingDefinition = new EncodingDefinition();

            foreach (int format in LimgFormats.Keys)
                encodingDefinition.AddIndexEncoding(format, LimgFormats[format].Item1, LimgFormats[format].Item2);

            foreach (int format in LimgPaletteFormats.Keys)
                encodingDefinition.AddPaletteEncoding(format, LimgPaletteFormats[format]);

            return encodingDefinition;
        }
    }
}
