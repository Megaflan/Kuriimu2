using Komponent.Contract.Aspects;
using Konnect.Contract.DataClasses.Plugin.File.Archive;
using Konnect.Plugin.File.Archive;

namespace plugin_level5.N3DS.Archive
{
    class ArcvHeader
    {
        [FixedLength(4)]
        public string magic = "ARCV";
        public int fileCount;
        public int fileSize;
    }

    public class ArcvFileInfo
    {
        public int offset;
        public int size;
        public uint hash;
    }

    public class ArcvArchiveFile : ArchiveFile
    {
        public ArcvFileInfo Entry { get; }

        public ArcvArchiveFile(ArchiveFileInfo fileInfo, ArcvFileInfo entry) : base(fileInfo)
        {
            Entry = entry;
        }
    }
}
