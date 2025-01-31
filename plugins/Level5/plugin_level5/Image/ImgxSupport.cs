using Kanvas;
using Kanvas.Contract;
using Kanvas.Contract.DataClasses;
using Kanvas.Encoding;
using Kanvas.Swizzle;
using Komponent.Contract.Aspects;
using Konnect.Contract.DataClasses.Management.Dialog;
using Konnect.Contract.Enums.Management.Dialog;
using Konnect.Contract.Management.Dialog;
using Konnect.Plugin.File.Image;
using SixLabors.ImageSharp;

#pragma warning disable 649

namespace plugin_level5.Image
{
    class ImgxHeader
    {
        [FixedLength(4)]
        public string magic; // IMGx
        public int const1; // 30 30 00 00
        public short const2; // 30 00
        public byte imageFormat;
        public byte const3; // 01
        public byte imageCount;
        public byte bitDepth;
        public short bytesPerTile;
        public short width;
        public short height;
        public int const4; // 30 00 00 00
        public int const5; // 30 00 01 00
        public int tableDataOffset; // always 0x48
        public int const6; // 03 00 00 00
        public int const7; // 00 00 00 00
        public int const8; // 00 00 00 00
        public int const9; // 00 00 00 00
        public int const10; // 00 00 00 00
        public int tileTableSize;
        public int tileTableSizePadded;
        public int imgDataSize;
        public int const11; // 00 00 00 00
        public int const12; // 00 00 00 00
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
                case "IMGC":
                    _swizzle = new MasterSwizzle(Width, Point.Empty, new[] { (0, 1), (1, 0), (0, 2), (2, 0), (0, 4), (4, 0) });
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
                case "IMGC":
                    return await GetN3dsFormats(format, bitDepth, dialogManager);

                case "IMGV":
                    return GetVitaFormats();

                case "IMGA":
                    return GetMobileFormats();

                case "IMGN":
                    return GetSwitchFormats();

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
