using Komponent.Contract.Aspects;
using Konnect.Contract.DataClasses.Plugin.File.Archive;
using Konnect.Contract.Progress;
using Konnect.Plugin.File.Archive;

namespace plugin_level5.N3DS.Archive
{
    class XpckHeader
    {
        [FixedLength(4)]
        public string magic = "XPCK";

        public byte fc1;
        public byte fc2;

        public ushort infoOffsetUnshifted;
        public ushort nameTableOffsetUnshifted;
        public ushort dataOffsetUnshifted;
        public ushort infoSizeUnshifted;
        public ushort nameTableSizeUnshifted;
        public uint dataSizeUnshifted;

        public ushort FileCount
        {
            get => (ushort)((fc2 & 0xf) << 8 | fc1);
            set
            {
                fc2 = (byte)((fc2 & 0xF0) | ((value >> 8) & 0x0F));
                fc1 = (byte)value;
            }
        }

        public ushort FileInfoOffset
        {
            get => (ushort)(infoOffsetUnshifted << 2);
            set => infoOffsetUnshifted = (ushort)(value >> 2);
        }

        public ushort FilenameTableOffset
        {
            get => (ushort)(nameTableOffsetUnshifted << 2);
            set => nameTableOffsetUnshifted = (ushort)(value >> 2);
        }

        public ushort DataOffset
        {
            get => (ushort)(dataOffsetUnshifted << 2);
            set => dataOffsetUnshifted = (ushort)(value >> 2);
        }

        public ushort FileInfoSize
        {
            get => (ushort)(infoSizeUnshifted << 2);
            set => infoSizeUnshifted = (ushort)(value >> 2);
        }

        public ushort FilenameTableSize
        {
            get => (ushort)(nameTableSizeUnshifted << 2);
            set => nameTableSizeUnshifted = (ushort)(value >> 2);
        }

        public uint DataSize
        {
            get => dataSizeUnshifted << 2;
            set => dataSizeUnshifted = value >> 2;
        }
    }

    public class XpckFileInfo
    {
        public uint hash;
        public ushort nameOffset;

        public ushort tmp;
        public ushort tmp2;
        public byte tmpZ;
        public byte tmp2Z;

        public int FileOffset
        {
            get => ((tmpZ << 16) | tmp) << 2;
            set
            {
                tmpZ = (byte)(value >> 18);
                tmp = (ushort)(value >> 2);
            }
        }

        public int FileSize
        {
            get => (tmp2Z << 16) | tmp2;
            set
            {
                tmp2Z = (byte)(value >> 16);
                tmp2 = (ushort)value;
            }
        }
    }

    public class XpckArchiveFile : ArchiveFile
    {
        public XpckFileInfo Entry { get; }

        public XpckArchiveFile(ArchiveFileInfo fileInfo, XpckFileInfo entry) : base(fileInfo)
        {
            Entry = entry;
        }
    }
}
