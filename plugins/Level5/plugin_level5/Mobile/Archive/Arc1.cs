using Komponent.IO;
using Komponent.Streams;
using Konnect.Contract.DataClasses.Plugin.File.Archive;
using Konnect.Extensions;

namespace plugin_level5.Mobile.Archive
{
    public class Arc1
    {
        private const int HeaderSize_ = 20;
        private const int EntrySize_ = 12;

        private Arc1Header _header;

        public List<Arc1ArchiveFile> Load(Stream input)
        {
            var typeReader = new BinaryTypeReader();
            using var headerBr = new BinaryReaderX(new Arc1CryptoStream(input, 0), true);

            // Read header
            var header = typeReader.Read<Arc1Header>(headerBr);
            if (header == null)
                return new List<Arc1ArchiveFile>();

            _header = header;

            // Prepare file info stream
            var infoStream = new Arc1CryptoStream(new SubStream(input, _header.entryOffset, _header.entrySize), (uint)_header.entryOffset);
            using var infoBr = new BinaryReaderX(infoStream);

            // Read entries
            var entryCount = infoBr.ReadInt32();
            var entries = typeReader.ReadMany<Arc1FileEntry>(infoBr, entryCount);

            // Add files
            var result = new List<Arc1ArchiveFile>();
            foreach (var entry in entries)
            {
                infoStream.Position = entry.nameOffset;
                var name = infoBr.ReadNullTerminatedString();

                var fileStream = new SubStream(input, entry.offset, entry.size);
                var fileInfo = new ArchiveFileInfo
                {
                    FileData = Path.GetExtension(name) != ".mp4" ? new Arc1CryptoStream(fileStream, (uint)entry.offset) : fileStream,
                    FilePath = name
                };

                result.Add(new Arc1ArchiveFile(fileInfo));
            }

            return result;
        }

        public void Save(Stream output, List<Arc1ArchiveFile> files)
        {
            // Calculate offsets
            var dataOffset = HeaderSize_;
            var entryOffset = dataOffset + files.Sum(x => x.FileSize);
            var stringOffset = entryOffset + 4 + files.Count * EntrySize_;
            var totalSize = stringOffset + files.Sum(x => x.FilePath.ToRelative().FullName.Length + 1);

            // Prepare output stream
            output.SetLength(totalSize);

            // Write files
            var dataPosition = dataOffset;
            var stringPosition = stringOffset - entryOffset;

            var entries = new List<Arc1FileEntry>();
            foreach (Arc1ArchiveFile file in files)
            {
                // Write file data
                Stream outputRegion = new SubStream(output, dataPosition, file.FileSize);
                if (file.ContentChanged && file.FilePath.GetExtensionWithDot() != ".mp4")
                    outputRegion = new Arc1CryptoStream(outputRegion, (uint)dataPosition);

                file.GetFinalStream().CopyTo(outputRegion);

                // Add entry
                entries.Add(new Arc1FileEntry
                {
                    nameOffset = (int)stringPosition,
                    offset = dataPosition,
                    size = (int)file.FileSize
                });

                dataPosition += (int)file.FileSize;
                stringPosition += file.FilePath.ToRelative().FullName.Length + 1;
            }

            // Write entry information
            var infoStream = new Arc1CryptoStream(new SubStream(output, entryOffset, totalSize - entryOffset), (uint)entryOffset);

            var typeWriter = new BinaryTypeWriter();
            using var infoBw = new BinaryWriterX(infoStream);

            infoBw.Write(files.Count);
            typeWriter.WriteMany(entries, infoBw);

            foreach (var name in files.Select(x => x.FilePath.ToRelative().FullName))
                infoBw.WriteString(name);

            // Write header
            var headerStream = new Arc1CryptoStream(new SubStream(output, 0, HeaderSize_), 0);
            using var headerBw = new BinaryWriterX(headerStream);

            _header.entryOffset = (int)entryOffset;
            _header.entrySize = (int)(totalSize - entryOffset);
            _header.fileSize = (int)totalSize;

            typeWriter.Write(_header, headerBw);
        }
    }
}
