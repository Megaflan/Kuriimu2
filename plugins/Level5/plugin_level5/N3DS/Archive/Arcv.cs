using Komponent.IO;
using Komponent.Streams;
using Konnect.Contract.DataClasses.Plugin.File.Archive;

namespace plugin_level5.N3DS.Archive
{
    // Game: Inazuma 3 Ogre Team
    public class Arcv
    {
        private const int HeaderSize_ = 12;
        private const int EntrySize_ = 12;

        public List<ArcvArchiveFile> Load(Stream input)
        {
            var typeReader = new BinaryTypeReader();
            using var br = new BinaryReaderX(input, true);

            // Read header
            var header = typeReader.Read<ArcvHeader>(br);
            if (header == null)
                return new List<ArcvArchiveFile>();

            // Read entries
            IList<ArcvFileInfo?> entries = typeReader.ReadMany<ArcvFileInfo>(br, header.fileCount);

            var files = new List<ArcvArchiveFile>();
            foreach (ArcvFileInfo? entry in entries)
            {
                if (entry == null)
                    continue;

                var fileStream = new SubStream(input, entry.offset, entry.size);
                var fileInfo = new ArchiveFileInfo
                {
                    FileData = fileStream,
                    FilePath = $"{files.Count:00000000}.bin"
                };

                files.Add(new ArcvArchiveFile(fileInfo, entry));
            }

            return files;
        }

        public void Save(Stream output, List<ArcvArchiveFile> files)
        {
            var typeWriter = new BinaryTypeWriter();
            using var bw = new BinaryWriterX(output);

            bw.BaseStream.Position = (HeaderSize_ + files.Count * EntrySize_ + 0x7F) & ~0x7F;

            // Write files
            foreach (ArcvArchiveFile file in files)
            {
                file.Entry.offset = (int)bw.BaseStream.Position;
                file.Entry.size = (int)file.FileSize;

                file.WriteFileData(bw.BaseStream, false);

                bw.WriteAlignment(0x80, 0xAC);
            }

            // Write header
            bw.BaseStream.Position = 0;
            typeWriter.Write(new ArcvHeader
            {
                fileSize = (int)output.Length,
                fileCount = files.Count
            }, bw);

            // Write file entries
            foreach (ArcvArchiveFile file in files)
                typeWriter.Write(file.Entry, bw);

            // Pad with 0xAC to first file
            bw.WriteAlignment(0x80, 0xAC);
        }
    }
}
