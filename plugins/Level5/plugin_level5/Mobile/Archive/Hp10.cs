using Komponent.IO;
using Komponent.Streams;
using Konnect.Contract.DataClasses.Plugin.File.Archive;
using Konnect.Extensions;
using Kryptography.Checksum.Crc;

namespace plugin_level5.Mobile.Archive
{
    public class Hp10
    {
        private const int HeaderSize_ = 32;
        private const int EntrySize_ = 32;

        private Hp10Header _header;

        public List<Hp10ArchiveFile> Load(Stream input)
        {
            var typeReader = new BinaryTypeReader();
            using var br = new BinaryReaderX(input, true);

            // Read header
            var header = typeReader.Read<Hp10Header>(br);
            if (header == null)
                return new List<Hp10ArchiveFile>();

            _header = header;

            // Read entries
            var entries = typeReader.ReadMany<Hp10FileEntry>(br, _header.fileCount);

            // Add files
            var result = new List<Hp10ArchiveFile>();
            foreach (var entry in entries)
            {
                input.Position = _header.stringOffset + entry.nameOffset;
                var name = br.ReadNullTerminatedString();
                var fileStream = new SubStream(input, _header.dataOffset + entry.fileOffset, entry.fileSize);

                var fileInfo = new ArchiveFileInfo
                {
                    FileData = fileStream,
                    FilePath = name
                };
                result.Add(new Hp10ArchiveFile(fileInfo, entry));
            }

            return result;
        }

        public void Save(Stream output, List<Hp10ArchiveFile> files)
        {
            var crc32b = Crc32.Crc32B;
            var crc32c = Crc32.Crc32C;

            var typeWriter = new BinaryTypeWriter();
            using var bw = new BinaryWriterX(output);

            // Calculate offsets
            var entryOffset = HeaderSize_;
            var stringOffset = entryOffset + files.Count * EntrySize_;
            var dataOffset = (stringOffset + files.Sum(x => x.FilePath.ToRelative().FullName.Length + 1) + 0x7FF) & ~0x7FF;

            // Write files
            var dataPosition = (long)dataOffset;
            var namePosition = 0;

            var entries = new List<Hp10FileEntry>();
            var strings = new List<string>();
            foreach (Hp10ArchiveFile file in files)
            {
                // Write file data
                output.Position = dataPosition;
                var writtenSize = file.WriteFileData(output, false);
                bw.WriteAlignment(0x800);

                // Update entry
                file.Entry.crc32bFileNameHash = crc32b.ComputeValue(file.FilePath.GetName());
                file.Entry.crc32cFileNameHash = crc32c.ComputeValue(file.FilePath.GetName());
                file.Entry.crc32bFilePathHash = crc32b.ComputeValue(file.FilePath.ToRelative().FullName);
                file.Entry.crc32cFilePathHash = crc32c.ComputeValue(file.FilePath.ToRelative().FullName);
                file.Entry.nameOffset = namePosition;
                file.Entry.fileOffset = (uint)(dataPosition - dataOffset);
                file.Entry.fileSize = (int)writtenSize;
                entries.Add(file.Entry);
                strings.Add(file.FilePath.ToRelative().FullName);

                // Update positions
                namePosition += file.FilePath.ToRelative().FullName.Length + 1;
                dataPosition = (dataPosition + writtenSize + 0x7FF) & ~0x7FF;
            }

            // Write strings
            output.Position = stringOffset;
            foreach (var name in strings)
                bw.WriteString(name);
            var stringEndOffset = output.Position;

            // Write entries
            output.Position = entryOffset;
            typeWriter.WriteMany(entries, bw);

            // Write header
            _header.dataOffset = dataOffset;
            _header.stringOffset = stringOffset;
            _header.fileCount = files.Count;
            _header.fileSize = (uint)output.Length;
            _header.stringEnd = (int)stringEndOffset;

            output.Position = 0;
            typeWriter.Write(_header, bw);
        }
    }
}
