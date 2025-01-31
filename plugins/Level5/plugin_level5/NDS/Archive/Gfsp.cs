using System.Text;
using Komponent.IO;
using Komponent.Streams;
using Konnect.Contract.DataClasses.Plugin.File.Archive;
using Konnect.Extensions;
using Konnect.Plugin.File.Archive;
using Kryptography.Checksum.Crc;

namespace plugin_level5.NDS.Archive
{
    public class Gfsp
    {
        private const int HeaderSize_ = 20;

        public List<ArchiveFile> Load(Stream input)
        {
            var typeReader = new BinaryTypeReader();
            using var br = new BinaryReaderX(input, true);

            // Read header
            var header = typeReader.Read<GfspHeader>(br);

            // Read entries
            input.Position = header.FileInfoOffset;
            var entries = typeReader.ReadMany<GfspFileInfo>(br, header.FileCount);

            // Get name stream
            var nameStream = new SubStream(input, header.FilenameTableOffset, header.FilenameTableSize);
            using var nameBr = new BinaryReaderX(nameStream);

            // Add files
            var result = new List<ArchiveFile>();
            foreach (var entry in entries)
            {
                var fileStream = new SubStream(input, header.DataOffset + entry.FileOffset, entry.size);

                nameBr.BaseStream.Position = entry.NameOffset;
                var fileName = nameBr.ReadNullTerminatedString();

                var fileInfo = new ArchiveFileInfo
                {
                    FileData = fileStream,
                    FilePath = fileName
                };

                result.Add(new ArchiveFile(fileInfo));
            }

            return result;
        }

        public void Save(Stream output, List<ArchiveFile> files)
        {
            var crc16 = Crc16.X25;
            var typeWriter = new BinaryTypeWriter();
            using var bw = new BinaryWriterX(output);

            // Calculate offsets
            var fileInfoOffset = HeaderSize_;
            var nameOffset = fileInfoOffset + files.Count * HeaderSize_;
            var dataOffset = (nameOffset + files.Sum(x => Encoding.ASCII.GetByteCount(x.FilePath.GetName()) + 1) + 3) & ~3;

            // Write files
            var fileInfos = new List<GfspFileInfo>();

            var fileOffset = 0;
            var stringOffset = 0;
            foreach (var file in files)
            {
                output.Position = dataOffset + fileOffset;

                var writtenSize = file.WriteFileData(output, false);
                bw.WriteAlignment(4);

                fileInfos.Add(new GfspFileInfo
                {
                    hash = crc16.ComputeValue(file.FilePath.GetName()),
                    FileOffset = fileOffset,
                    NameOffset = stringOffset,
                    size = (ushort)writtenSize
                });

                fileOffset += (int)file.FileSize;
                stringOffset += Encoding.ASCII.GetByteCount(file.FilePath.GetName()) + 1;
            }

            // Write names
            output.Position = nameOffset;
            foreach (var name in files.Select(x => x.FilePath.GetName()))
                bw.WriteString(name);

            // Write entries
            output.Position = fileInfoOffset;
            typeWriter.WriteMany(fileInfos, bw);

            // Write header
            var header = new GfspHeader
            {
                FileCount = (ushort)files.Count,

                FileInfoOffset = (ushort)fileInfoOffset,
                FilenameTableOffset = (ushort)nameOffset,
                DataOffset = (ushort)dataOffset,

                FileInfoSize = (ushort)(nameOffset - fileInfoOffset),
                FilenameTableSize = (ushort)(dataOffset - nameOffset),
                DataSize = (uint)(output.Length - dataOffset)
            };

            output.Position = 0;
            typeWriter.Write(header, bw);
        }
    }
}
