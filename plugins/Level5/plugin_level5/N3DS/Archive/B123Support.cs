﻿using Komponent.Contract.Aspects;
using Komponent.IO;
using Konnect.Contract.DataClasses.Plugin.File.Archive;
using Konnect.Contract.Progress;
using Konnect.Plugin.File.Archive;

#pragma warning disable 649

namespace plugin_level5.N3DS.Archive
{
    class B123Header
    {
        [FixedLength(4)]
        public string magic = "B123";
        public int directoryEntriesOffset;
        public int directoryHashOffset;
        public int fileEntriesOffset;
        public int nameOffset;
        public int dataOffset;
        public short directoryEntriesCount;
        public short directoryHashCount;
        public int fileEntriesCount;
        public int tableChunkSize;
        public int zero1;

        //Hashes?
        public uint unk2;
        public uint unk3;
        public uint unk4;
        public uint unk5;

        public int directoryCount;
        public int fileCount;
        public uint unk7;
        public int zero2;
    }

    public class B123FileEntry
    {
        // TODO: Hashes of files to lower?
        public uint crc32;  // only filename.ToLower()
        public uint nameOffsetInFolder;
        public uint fileOffset;
        public uint fileSize;
    }

    class B123DirectoryEntry
    {
        // TODO: Hashes of files to lower?
        public uint crc32;  // directoryName.ToLower()
        public short fileCount;
        public short directoryCount;
        public int fileNameStartOffset;
        public int firstFileIndex;
        public int firstDirectoryIndex;
        public int directoryNameStartOffset;
    }

    public class B123ArchiveFile : ArchiveFile
    {
        public B123FileEntry Entry { get; }

        public B123ArchiveFile(ArchiveFileInfo fileInfo, B123FileEntry entry) : base(fileInfo)
        {
            Entry = entry;
        }
    }

    static class B123Support
    {
        public static Guid[] RetrievePluginMapping(Stream fileStream, string fileName)
        {
            var extension = Path.GetExtension(fileName);
            using var br = new BinaryReaderX(fileStream, true);

            var magic = br.ReadString(4);

            switch (extension)
            {
                case ".xi":
                    return new[] { Guid.Parse("898c9151-71bd-4638-8f90-6d34f0a8600c") };

                case ".xf":
                    return new[] { Guid.Parse("b1b397c4-9a02-4828-b568-39cad733fa3a") };

                case ".xr":
                case ".xc":
                case ".xa":
                case ".xk":
                    if (magic == "XPCK")
                        return new[] { Guid.Parse("de276e88-fb2b-48a6-a55f-d6c14ec60d4f") };

                    return null;

                case ".arc":
                    return new[] { Guid.Parse("db8c2deb-f11d-43c8-bb9e-e271408fd896") };

                // TODO: add t2b cfg.bin
                //case ".bin":
                //    if (!fileName.EndsWith(".cfg.bin"))
                //        return null;

                //    fileStream.Position = fileStream.Length - 0xF;
                //    if (br.ReadString(3) == "t2b")
                //        return null;

                //    return null;

                default:
                    return null;
            }
        }
    }
}
