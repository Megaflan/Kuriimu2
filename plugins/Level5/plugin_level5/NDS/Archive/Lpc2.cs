using System.Text;
using Komponent.IO;
using Komponent.Streams;
using Konnect.Contract.DataClasses.Plugin.File.Archive;
using Konnect.Plugin.File.Archive;

namespace plugin_level5.NDS.Archive
{
    // TODO: Test plugin
    // Game: Professor Layton 3 on DS
    public class Lpc2
    {
        private const int HeaderSize_ = 28;
        private const int FileEntrySize_ = 12;

        public List<ArchiveFile> Load(Stream input)
        {
            var typeReader = new BinaryTypeReader();
            using var br = new BinaryReaderX(input, true);

            // Read header
            var header = typeReader.Read<Lpc2Header>(br);

            // Read file entries
            br.BaseStream.Position = header.fileEntryOffset;
            var entries = typeReader.ReadMany<Lpc2FileEntry>(br, header.fileCount);

            // Add files
            var result = new List<ArchiveFile>();
            foreach (var entry in entries)
            {
                br.BaseStream.Position = header.nameOffset + entry.nameOffset;
                var name = br.ReadNullTerminatedString();

                var fileStream = new SubStream(input, header.dataOffset + entry.fileOffset, entry.fileSize);

                var fileInfo = new ArchiveFileInfo
                {
                    FileData = fileStream,
                    FilePath = name,
                    PluginIds = Lpc2Support.RetrievePluginMapping(name)
                };
                result.Add(new ArchiveFile(fileInfo));
            }

            return result;
        }

        public void Save(Stream output, IList<ArchiveFile> files)
        {
            var typeWriter = new BinaryTypeWriter();
            using var bw = new BinaryWriterX(output);

            var fileEntryStartOffset = HeaderSize_;
            var nameStartOffset = HeaderSize_ + files.Count * FileEntrySize_;

            // Write names
            var fileOffset = 0;
            var nameOffset = 0;
            var fileEntries = new List<Lpc2FileEntry>();
            foreach (var file in files)
            {
                fileEntries.Add(new Lpc2FileEntry
                {
                    fileOffset = fileOffset,
                    fileSize = (int)file.FileSize,
                    nameOffset = nameOffset
                });

                bw.BaseStream.Position = nameStartOffset + nameOffset;
                bw.WriteString(file.FilePath.FullName, Encoding.ASCII, false);
                nameOffset = (int)bw.BaseStream.Position - nameStartOffset;

                fileOffset += (int)file.FileSize;
            }

            // Write file data
            var dataOffset = (int)bw.BaseStream.Position;
            foreach (var file in files)
                file.WriteFileData(bw.BaseStream, false);

            // Write file entries
            bw.BaseStream.Position = fileEntryStartOffset;
            foreach (var fileEntry in fileEntries)
                typeWriter.Write(fileEntry, bw);

            // Write header
            bw.BaseStream.Position = 0;
            typeWriter.Write(new Lpc2Header
            {
                fileEntryOffset = fileEntryStartOffset,
                nameOffset = nameStartOffset,
                dataOffset = dataOffset,

                fileCount = files.Count,

                headerSize = HeaderSize_,
                fileSize = (int)bw.BaseStream.Length
            }, bw);
        }
    }
}
