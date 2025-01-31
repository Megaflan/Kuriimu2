using Konnect.Contract.DataClasses.Plugin.File.Archive;
using Konnect.Plugin.File.Archive;

namespace plugin_level5.N3DS.Archive
{
    public class PckFileInfo
    {
        public uint hash;
        public int fileOffset;
        public int fileLength;
    }

    public class PckArchiveFile : ArchiveFile
    {
        public PckFileInfo Entry { get; }
        public IList<uint>? Hashes { get; }

        public PckArchiveFile(ArchiveFileInfo fileInfo, PckFileInfo entry, IList<uint>? hashBlock) : base(fileInfo)
        {
            Entry = entry;
            Hashes = hashBlock;
        }
    }
}
