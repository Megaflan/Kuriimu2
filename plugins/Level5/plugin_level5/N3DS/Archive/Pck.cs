using Komponent.IO;
using Komponent.Streams;
using Konnect.Contract.DataClasses.Plugin.File.Archive;

namespace plugin_level5.N3DS.Archive
{
    // TODO:
    // Game: Time Travelers
    public class Pck
    {
        private const int EntrySize_ = 12;

        public List<PckArchiveFile> Load(Stream input)
        {
            var typeReader = new BinaryTypeReader();
            using var br = new BinaryReaderX(input, true);

            // Read file infos
            var fileCount = br.ReadInt32();
            var entries = typeReader.ReadMany<PckFileInfo>(br, fileCount);

            // Add files
            var result = new List<PckArchiveFile>();
            foreach (var entry in entries)
            {
                br.BaseStream.Position = entry.fileOffset;

                // Read hash block before the file data
                var blockOffset = 0;
                var entryHashes = (IList<uint>)null;
                var hashIdent = br.ReadInt16();
                if (hashIdent == 0x64)
                {
                    var hashCount = br.ReadInt16();
                    entryHashes = typeReader.ReadMany<uint>(br, hashCount);

                    blockOffset = (hashCount + 1) * 4;
                }

                // Decide filename
                var fileName = $"{entry.hash:X8}.bin";

                // Add file
                var fileStream = new SubStream(input, entry.fileOffset + blockOffset, entry.fileLength - blockOffset);
                var fileInfo = new ArchiveFileInfo
                {
                    FileData = fileStream,
                    FilePath = fileName
                };
                result.Add(new PckArchiveFile(fileInfo, entry, entryHashes));
            }

            return result;
        }

        public void Save(Stream output, List<PckArchiveFile> files)
        {
            var typeWriter = new BinaryTypeWriter();
            using var bw = new BinaryWriterX(output);

            // Write file count
            bw.Write(files.Count);

            // Write file infos
            var dataOffset = 4 + files.Count * EntrySize_;
            foreach (PckArchiveFile file in files)
            {
                var fileSize = (int)file.FileSize;
                if (file.Hashes != null)
                    fileSize += (file.Hashes.Count + 1) * 4;

                typeWriter.Write(new PckFileInfo
                {
                    hash = file.Entry.hash,
                    fileOffset = dataOffset,
                    fileLength = fileSize
                }, bw);

                dataOffset += fileSize;
            }

            // Write file data
            foreach (PckArchiveFile file in files)
            {
                if (file.Hashes != null)
                {
                    bw.Write((short)0x64);
                    bw.Write((short)file.Hashes.Count);
                    typeWriter.WriteMany(file.Hashes, bw);
                }

                file.WriteFileData(bw.BaseStream, false);
            }
        }
    }
}
