using Kanvas;
using Kanvas.Contract;
using Kanvas.Contract.DataClasses;
using Kanvas.Encoding;
using Kanvas.Swizzle;
using Komponent.Contract.Aspects;
using Komponent.Contract.Enums;
using Konnect.Contract.DataClasses.Management.Dialog;
using Konnect.Contract.Enums.Management.Dialog;
using Konnect.Contract.Management.Dialog;
using Konnect.Plugin.File.Image;
using SixLabors.ImageSharp;
using ByteOrder = Komponent.Contract.Enums.ByteOrder;

#pragma warning disable 649

namespace plugin_level5.Image
{
    class ImgxHeader
    {
        [FixedLength(6)]
        public string magic; // IMGx00
        public short zero0;
        public short const1; // 30 00
        public byte imageFormat;
        public byte const2; // 01
        public byte imageCount;
        public byte bitDepth;
        public short bytesPerTile;
        public short width;
        public short height;
        public ushort paletteInfoOffset;
        public ushort paletteInfoCount;
        public ushort imageInfoOffset;
        public ushort imageInfoCount;
        public int dataOffset;
        public int const3;
    }

    class ImgxPaletteInfo
    {
        public int offset;
        public int size;
        public short colorCount;
        public byte const0;
        public byte format;
        public int zero0;
    }

    class ImgxImageInfo
    {
        public int tileOffset;
        public int tileSize;
        public int dataOffset;
        public int dataSize;
        public int zero0;
        public int zero1;
    }

    class ImgxSwizzle : IImageSwizzle
    {
        private readonly MasterSwizzle _swizzle;

        public int Width { get; }
        public int Height { get; }

        public ImgxSwizzle(SwizzleOptions options, string magic)
        {
            Width = (options.Size.Width + 7) & ~7;
            Height = (options.Size.Height + 7) & ~7;

            switch (magic)
            {
                case "IMGC00":
                    _swizzle = new MasterSwizzle(Width, Point.Empty, new[] { (0, 1), (1, 0), (0, 2), (2, 0), (0, 4), (4, 0) });
                    break;

                case "IMGP00":
                    switch (options.EncodingInfo.BitsPerValue)
                    {
                        case 4:
                            _swizzle = new MasterSwizzle(Width, Point.Empty, new[] { (1, 0), (2, 0), (4, 0), (8, 0), (16, 0), (0, 1), (0, 2), (0, 4) });
                            break;

                        case 8:
                            _swizzle = new MasterSwizzle(Width, Point.Empty, new[] { (1, 0), (2, 0), (4, 0), (8, 0), (0, 1), (0, 2), (0, 4) });
                            break;

                        case 16:
                            _swizzle = new MasterSwizzle(Width, Point.Empty, new[] { (1, 0), (2, 0), (4, 0), (0, 1), (0, 2), (0, 4) });
                            break;

                        case 32:
                            _swizzle = new MasterSwizzle(Width, Point.Empty, new[] { (1, 0), (2, 0), (0, 1), (0, 2), (0, 4) });
                            break;
                    }
                    break;

                default:
                    _swizzle = options.EncodingInfo.ColorsPerValue > 1 ?
                        new MasterSwizzle(Width, Point.Empty, new[] { (1, 0), (2, 0), (0, 1), (0, 2), (4, 0), (8, 0) }) :
                        new MasterSwizzle(Width, Point.Empty, new[] { (1, 0), (2, 0), (4, 0), (0, 1), (0, 2), (0, 4) });
                    break;
            }
        }

        public Point Transform(Point point) => _swizzle.Get(point.Y * Width + point.X);
    }

    class ImgxSupport
    {
        public static async Task<EncodingDefinition> GetEncodingDefinition(string magic, int format, int bitDepth, IDialogManager? dialogManager)
        {
            switch (magic)
            {
                case "IMGC00":
                    return await GetN3dsFormats(format, bitDepth, dialogManager);

                case "IMGV00":
                    return GetVitaFormats();

                case "IMGA00":
                    return GetMobileFormats();

                case "IMGN00":
                    return GetSwitchFormats();

                case "IMGP00":
                    return GetPspFormats();

                default:
                    throw new InvalidOperationException($"Invalid IMGx magic {magic}.");
            }
        }

        private static EncodingDefinition GetVitaFormats()
        {
            var encodingDefinition = new EncodingDefinition();
            encodingDefinition.AddColorEncoding(0x03, ImageFormats.Rgb888());
            encodingDefinition.AddColorEncoding(0x1E, ImageFormats.Dxt1());

            return encodingDefinition;
        }

        private static EncodingDefinition GetMobileFormats()
        {
            var encodingDefinition = new EncodingDefinition();
            encodingDefinition.AddColorEncoding(0x03, ImageFormats.Rgb888());

            return encodingDefinition;
        }

        private static EncodingDefinition GetSwitchFormats()
        {
            var encodingDefinition = new EncodingDefinition();
            encodingDefinition.AddColorEncoding(0x00, new Rgba(8, 8, 8, 8, "ABGR"));

            return encodingDefinition;
        }

        // This mapping was determined through Inazuma Eleven GO Big Bang
        private static EncodingDefinition GetN3dsFormats1()
        {
            var encodingDefinition = new EncodingDefinition();

            encodingDefinition.AddColorEncoding(0x00, ImageFormats.Rgba8888());
            encodingDefinition.AddColorEncoding(0x01, ImageFormats.Rgba4444());
            encodingDefinition.AddColorEncoding(0x02, ImageFormats.Rgba5551());
            encodingDefinition.AddColorEncoding(0x03, new Rgba(8, 8, 8, "BGR"));
            encodingDefinition.AddColorEncoding(0x04, ImageFormats.Rgb565());
            encodingDefinition.AddColorEncoding(0x0A, ImageFormats.La88());
            encodingDefinition.AddColorEncoding(0x0B, ImageFormats.La44());
            encodingDefinition.AddColorEncoding(0x0C, ImageFormats.L8());
            encodingDefinition.AddColorEncoding(0x0D, ImageFormats.L4());
            encodingDefinition.AddColorEncoding(0x0E, ImageFormats.A8());
            encodingDefinition.AddColorEncoding(0x0F, ImageFormats.A4());
            encodingDefinition.AddColorEncoding(0x1B, ImageFormats.Etc1(true));
            encodingDefinition.AddColorEncoding(0x1C, ImageFormats.Etc1A4(true));

            return encodingDefinition;
        }

        // This mapping was determined through Time Travelers
        private static EncodingDefinition GetN3dsFormats2()
        {
            var encodingDefinition = new EncodingDefinition();

            encodingDefinition.AddColorEncoding(0x00, ImageFormats.Rgba8888());
            encodingDefinition.AddColorEncoding(0x01, ImageFormats.Rgba4444());
            encodingDefinition.AddColorEncoding(0x02, ImageFormats.Rgba5551());
            encodingDefinition.AddColorEncoding(0x03, new Rgba(8, 8, 8, "BGR"));
            encodingDefinition.AddColorEncoding(0x04, ImageFormats.Rgb565());
            encodingDefinition.AddColorEncoding(0x0B, ImageFormats.La88());
            encodingDefinition.AddColorEncoding(0x0C, ImageFormats.La44());
            encodingDefinition.AddColorEncoding(0x0D, ImageFormats.L8());
            encodingDefinition.AddColorEncoding(0x0E, ImageFormats.L4());
            encodingDefinition.AddColorEncoding(0x0F, ImageFormats.A8());
            encodingDefinition.AddColorEncoding(0x10, ImageFormats.A4());
            encodingDefinition.AddColorEncoding(0x1B, ImageFormats.Etc1(true));
            encodingDefinition.AddColorEncoding(0x1C, ImageFormats.Etc1(true));
            encodingDefinition.AddColorEncoding(0x1D, ImageFormats.Etc1A4(true));

            return encodingDefinition;
        }

        private static EncodingDefinition GetPspFormats()
        {
            var encodingDefinition = new EncodingDefinition();

            encodingDefinition.AddPaletteEncoding(0x00, ImageFormats.Rgba8888(ByteOrder.BigEndian));
            encodingDefinition.AddPaletteEncoding(0x01, new Rgba(4, 4, 4, 4, "ARGB"));
            encodingDefinition.AddPaletteEncoding(0x02, new Rgba(5, 5, 5, 1, "ABGR"));

            encodingDefinition.AddColorEncoding(0x00, ImageFormats.Rgba8888(ByteOrder.BigEndian));
            encodingDefinition.AddIndexEncoding(0x11, ImageFormats.I8(), new[] { 0, 1, 2 });
            encodingDefinition.AddIndexEncoding(0x13, ImageFormats.I8(), new[] { 0, 1, 2 });
            encodingDefinition.AddIndexEncoding(0x17, ImageFormats.I4(BitOrder.LeastSignificantBitFirst), new[] { 0, 1, 2 });

            return encodingDefinition;
        }

        private static async Task<EncodingDefinition> GetN3dsFormats(int format, int bitDepth, IDialogManager dialogManager)
        {
            var encodingDefinitions = new[]
            {
                GetN3dsFormats1(),
                GetN3dsFormats2()
            };

            // If format does not exist in any
            if (encodingDefinitions.All(x => !x.ContainsColorEncoding(format)))
                return EncodingDefinition.Empty;

            // If the format exists only in one of the mappings
            if (encodingDefinitions.Count(x => x.ContainsColorEncoding(format)) == 1)
                return encodingDefinitions.First(x => x.ContainsColorEncoding(format));

            // If format exists in more than one, compare bitDepth
            var viableMappings = encodingDefinitions.Where(x => x.ContainsColorEncoding(format)).ToArray();

            // If only one mapping matches the given bitDepth
            if (viableMappings.Count(x => x.GetColorEncoding(format).BitDepth == bitDepth) == 1)
                return viableMappings.First(x => x.GetColorEncoding(format).BitDepth == bitDepth);

            // Otherwise the heuristic could not determine a definite mapping
            // Show a dialog to the user, selecting the game
            return await RequestEncodingDefinition(dialogManager);
        }

        private static async Task<EncodingDefinition> RequestEncodingDefinition(IDialogManager dialogManager)
        {
            var availableGames = GameMapping.Keys.ToArray();
            var dialogField = new DialogField
            {
                Type = DialogFieldType.DropDown,
                Text = "Select the game:",
                DefaultValue = availableGames.First(),
                Options = availableGames
            };

            var result = await dialogManager.ShowDialog(new[] { dialogField });
            var definitionIndex = GameMapping[result ? dialogField.Result : dialogField.DefaultValue];

            switch (definitionIndex)
            {
                case 0:
                    return GetN3dsFormats1();

                case 1:
                    return GetN3dsFormats2();

                default:
                    throw new InvalidOperationException($"Invalid selected encoding definition index {definitionIndex}.");
            }
        }

        private static readonly IDictionary<string, int> GameMapping =
            new Dictionary<string, int>
            {
                ["Fantasy Life"] = 1, // TODO: Unconfirmed
                ["Inazuma Eleven GO"] = 1, // TODO: Unconfirmed
                ["Inazuma Eleven GO: Chrono Stones"] = 1, // TODO: Unconfirmed
                ["Inazuma Eleven GO: Galaxy"] = 0,
                ["Laytons Mystery Journey"] = 1, // TODO: Unconfirmed
                ["Professor Layton 5"] = 1, // TODO: Unconfirmed
                ["Professor Layton 6"] = 1, // TODO: Unconfirmed
                ["Professor Layton vs Phoenix Wright"] = 1, // TODO: Unconfirmed
                ["Time Travelers"] = 1,
                ["Yo-Kai Watch"] = 1, // TODO: Unconfirmed
                ["Yo-Kai Watch 2"] = 1, // TODO: Unconfirmed
                ["Yo-Kai Watch 3"] = 1, // TODO: Unconfirmed
                ["Yo-Kai Watch Blasters"] = 1, // TODO: Unconfirmed
                ["Yo-Kai Watch Blasters 2"] = 1, // TODO: Unconfirmed
                ["Yo-Kai Watch Sangokushi"] = 1, // TODO: Unconfirmed
            };
    }
}
