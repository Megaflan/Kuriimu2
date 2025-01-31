using Komponent.IO;
using Komponent.Streams;
using Konnect.Contract.DataClasses.Plugin.File.Archive;
using Konnect.Extensions;
using Konnect.Plugin.File.Archive;
using Kryptography.Checksum.Crc;

namespace plugin_level5.Switch.Archive
{
    // Game: Yo-kai Watch 4
    public class G4pk
    {
        private const int HeaderSize_ = 54;
        private const int OffsetSize_ = 4;
        private const int LengthSize_ = 4;
        private const int HashSize_ = 4;
        private const int UnkIdsSize_ = 2;
        private const int StringOffsetSize_ = 2;

        private G4pkHeader _header;
        private IList<short> _unkIds;

        public List<ArchiveFile> Load(Stream input)
        {
            var typeReader = new BinaryTypeReader();
            using var br = new BinaryReaderX(input, true);

            // Header
            var header = typeReader.Read<G4pkHeader>(br);
            if (header == null)
                return new List<ArchiveFile>();

            _header = header;

            // Entry information
            br.BaseStream.Position = _header.headerSize;
            var fileOffsets = typeReader.ReadMany<int>(br, _header.fileCount);
            var fileSizes = typeReader.ReadMany<int>(br, _header.fileCount);
            var hashes = typeReader.ReadMany<uint>(br, _header.table2EntryCount);

            // Unknown information
            _unkIds = typeReader.ReadMany<short>(br, _header.table3EntryCount / 2);

            // Strings
            br.BaseStream.Position = (br.BaseStream.Position + 3) & ~3;
            var stringOffset = br.BaseStream.Position;
            var stringOffsets = typeReader.ReadMany<short>(br, _header.table3EntryCount / 2);

            //Files
            var result = new List<ArchiveFile>();
            for (var i = 0; i < _header.fileCount; i++)
            {
                br.BaseStream.Position = stringOffset + stringOffsets[i];
                var name = br.ReadNullTerminatedString();

                var fileStream = new SubStream(input, _header.headerSize + (fileOffsets[i] << 2), fileSizes[i]);
                var fileInfo = new ArchiveFileInfo
                {
                    FileData = fileStream,
                    FilePath = name
                };

                result.Add(new ArchiveFile(fileInfo));
            }

            return result;
        }

        public void Save(Stream output, List<ArchiveFile> files)
        {
            var typeWriter = new BinaryTypeWriter();
            using var bw = new BinaryWriterX(output);

            var fileOffsetsPosition = HeaderSize_;
            var fileHashesPosition = fileOffsetsPosition + files.Count * (OffsetSize_ + LengthSize_);
            var unkIdsPosition = fileHashesPosition + files.Count * HashSize_;
            var stringOffsetPosition = (unkIdsPosition + UnkIdsSize_ + 3) & ~3;
            var stringPosition = (stringOffsetPosition + StringOffsetSize_ + 3) & ~3;

            // Write strings
            var crc32 = Crc32.Crc32B;

            bw.BaseStream.Position = stringOffsetPosition;
            var fileHashes = new List<uint>();
            var relativeStringOffset = stringPosition - stringOffsetPosition;
            for (var i = 0; i < files.Count; i++)
            {
                // Write string offset
                bw.BaseStream.Position = stringOffsetPosition + i * StringOffsetSize_;
                bw.Write((short)relativeStringOffset);

                // Add hash
                fileHashes.Add(crc32.ComputeValue(files[i].FilePath.ToRelative().FullName));

                // Write string
                bw.BaseStream.Position = stringOffsetPosition + relativeStringOffset;
                bw.WriteString(files[i].FilePath.ToRelative().FullName);

                relativeStringOffset = (int)(bw.BaseStream.Position - stringOffsetPosition);
            }

            var fileDataPosition = (bw.BaseStream.Position + 3) & ~3;

            // Write file data
            bw.BaseStream.Position = fileDataPosition;
            var fileOffset = new List<int>();
            var fileSizes = new List<int>();
            foreach (var file in files)
            {
                fileOffset.Add((int)((bw.BaseStream.Position - HeaderSize_) >> 2));

                var writtenSize = file.WriteFileData(bw.BaseStream, false);
                bw.WriteAlignment(0x20);

                fileSizes.Add((int)writtenSize);
            }

            // Write file information
            bw.BaseStream.Position = fileOffsetsPosition;
            typeWriter.WriteMany(fileOffset, bw);
            typeWriter.WriteMany(fileSizes, bw);
            typeWriter.WriteMany(fileHashes, bw);

            // Write unknown information
            typeWriter.WriteMany(_unkIds, bw);

            // Write header
            bw.BaseStream.Position = 0;

            _header.fileCount = files.Count;
            _header.contentSize = (int)(bw.BaseStream.Length - HeaderSize_);
            _header.table2EntryCount = (short)fileHashes.Count;

            typeWriter.Write(_header, bw);
        }
    }
}
