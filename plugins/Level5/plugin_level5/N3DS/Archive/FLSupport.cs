using Komponent.IO;
using Konnect.Contract.DataClasses.Plugin.File.Archive;
using Konnect.Plugin.File.Archive;

namespace plugin_level5.N3DS.Archive
{
    public class FLArchiveFile : ArchiveFile
    {
        public int Index { get; }

        public FLArchiveFile(ArchiveFileInfo fileInfo, int index) : base(fileInfo)
        {
            Index = index;
        }
    }

    class FLSupport
    {
        public static string DetermineExtension(Stream input)
        {
            var magicSamples = CollectMagicSamples(input);

            if (magicSamples.Any(x => x.Contains("ztex")))
                return ".ztex";

            if (magicSamples.Any(x => x.Contains("zmdl")))
                return ".zmdl";

            return ".bin";
        }

        private static IList<string> CollectMagicSamples(Stream input)
        {
            var bkPos = input.Position;

            using var br = new BinaryReaderX(input, true);

            // Get 3 samples to check magic with compression
            input.Position = bkPos + 5;
            var magic1 = br.ReadString(4);
            input.Position = bkPos + 6;
            var magic2 = br.ReadString(4);
            input.Position = bkPos + 7;
            var magic3 = br.ReadString(4);

            input.Position = bkPos;
            return new[] { magic1, magic2, magic3 };
        }
    }
}
