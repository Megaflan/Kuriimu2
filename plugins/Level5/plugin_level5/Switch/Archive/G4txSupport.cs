using Komponent.Contract.Aspects;
using Konnect.Contract.DataClasses.Plugin.File.Archive;
using Konnect.Plugin.File.Archive;

namespace plugin_level5.Switch.Archive
{
    class G4txHeader
    {
        [FixedLength(4)]
        public string magic = "G4TX";
        public short headerSize = 0x60;
        public short fileType = 0x65;
        public int unk1 = 0x00180000;
        public int tableSize;
        [FixedLength(0x10)]
        public byte[] zeroes;
        public short textureCount;
        public short totalCount;
        public byte unk2;
        public byte subTextureCount;
        public short unk3;
        public int unk4;
        public int textureDataSize;
        public long unk5;
        [FixedLength(0x28)]
        public byte[] unk6;
    }

    public class G4txEntry
    {
        public int unk1;
        public int nxtchOffset;
        public int nxtchSize;
        public int unk2;
        public int unk3;
        public int unk4;
        public short width;
        public short height;
        public int const2 = 1;
        [FixedLength(0x10)]
        public byte[] unk5;
    }

    public class G4txSubEntry
    {
        public short entryId;
        public short unk1;
        public short x;
        public short y;
        public short width;
        public short height;
        public int unk2;
        public int unk3;
        public int unk4;
    }

    class NxtchHeader
    {
        [FixedLength(8)]
        public string magic = "NXTCH000";
        public int textureDataSize;
        public int unk1;
        public int unk2;
        public int width;
        public int height;
        public int unk3;
        public int unk4;
        public int format;
        public int mipMapCount;
        public int textureDataSize2;
    }

    public class G4txSubTextureEntry
    {
        public byte Id { get; }

        public G4txSubEntry EntryEntry { get; }

        public string Name { get; }

        public G4txSubTextureEntry(byte id, G4txSubEntry entryEntry,string name)
        {
            Id = id;
            EntryEntry = entryEntry;
            Name = name;
        }
    }

    public class G4txArchiveFile : ArchiveFile
    {
        public G4txEntry Entry { get; }

        public byte Id { get; }

        public IList<G4txSubTextureEntry> Entries { get; }

        public G4txArchiveFile(ArchiveFileInfo fileInfo, G4txEntry entry, byte id, IList<G4txSubTextureEntry> entries) 
            : base(fileInfo)
        {
            Entry = entry;
            Id = id;
            Entries = entries;
        }
    }
}
