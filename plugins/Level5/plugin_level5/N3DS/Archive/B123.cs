using System.Text;
using Komponent.IO;
using Komponent.Streams;
using Kompression;
using Konnect.Contract.DataClasses.Plugin.File.Archive;
using Konnect.Extensions;
using Kryptography.Checksum.Crc;

namespace plugin_level5.N3DS.Archive
{
    // Game: Inazuma 3 Ogre Team
    public class B123
    {
        private const int HeaderSize_ = 72;
        private const int DirectoryEntrySize_ = 24;
        private const int DirectoryHashSize_ = 4;
        private const int FileEntrySize_ = 16;

        private B123Header _header;

        public List<B123ArchiveFile> Load(Stream input)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var typeReader = new BinaryTypeReader();
            using var br = new BinaryReaderX(input, Encoding.GetEncoding("Shift-JIS"), true);

            // Read header
            var header = typeReader.Read<B123Header>(br);
            if (header == null)
                return new List<B123ArchiveFile>();

            _header = header;

            // Read directory entries
            br.BaseStream.Position = _header.directoryEntriesOffset;
            var directoryEntries = typeReader.ReadMany<B123DirectoryEntry>(br, _header.directoryEntriesCount);

            // Read file entry table
            br.BaseStream.Position = _header.fileEntriesOffset;
            var entries = typeReader.ReadMany<B123FileEntry>(br, _header.fileEntriesCount);

            // Add Files
            var result = new List<B123ArchiveFile>();
            foreach (var directory in directoryEntries)
            {
                var filesInDirectory = entries.Skip(directory.firstFileIndex).Take(directory.fileCount);
                foreach (var file in filesInDirectory)
                {
                    var fileStream = new SubStream(input, _header.dataOffset + file.fileOffset, file.fileSize);

                    br.BaseStream.Position = _header.nameOffset + directory.fileNameStartOffset + file.nameOffsetInFolder;
                    string fileName = br.ReadNullTerminatedString();

                    br.BaseStream.Position = _header.nameOffset + directory.directoryNameStartOffset;
                    string directoryName = br.ReadNullTerminatedString();

                    result.Add(CreateAfi(fileStream, directoryName + fileName, file));
                }
            }

            return result;
        }

        public void Save(Stream output, List<B123ArchiveFile> files)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Build directory, file, and name tables
            BuildTables(files, out var directoryEntries, out var directoryHashes, out var fileEntries, out var nameStream);

            // -- Write file --

            var typeWriter = new BinaryTypeWriter();
            using var bw = new BinaryWriterX(output);
            bw.BaseStream.Position = HeaderSize_;

            _header.dataOffset = (int)(HeaderSize_ +
                                       directoryEntries.Count * DirectoryEntrySize_ +
                                       directoryHashes.Count * DirectoryHashSize_ +
                                       fileEntries.Count * FileEntrySize_ +
                                       nameStream.Length + 3) & ~3;

            // Write file data
            var fileOffset = 0u;
            var fileIndex = 1;
            foreach (var fileEntry in fileEntries)
            {
                bw.BaseStream.Position = _header.dataOffset + fileOffset;
                var writtenSize = fileEntry.WriteFileData(bw.BaseStream, true);

                bw.WriteAlignment(4);
                
                fileEntry.Entry.fileOffset = fileOffset;
                fileEntry.Entry.fileSize = (uint)writtenSize;

                fileOffset = (uint)(bw.BaseStream.Position - _header.dataOffset);
            }

            bw.BaseStream.Position = HeaderSize_;

            // Write directory entries
            _header.directoryCount = directoryEntries.Count;
            _header.directoryEntriesCount = (short)directoryEntries.Count;
            _header.directoryEntriesOffset = HeaderSize_;

            typeWriter.WriteMany(directoryEntries, bw);

            // Write directory hashes
            _header.directoryHashCount = (short)directoryHashes.Count;
            _header.directoryHashOffset = (int)bw.BaseStream.Position;

            typeWriter.WriteMany(directoryHashes, bw);

            // Write file entry hashes
            _header.fileCount = fileEntries.Count;
            _header.fileEntriesCount = fileEntries.Count;
            _header.fileEntriesOffset = (int)bw.BaseStream.Position;

            typeWriter.WriteMany(fileEntries.Select(x => x.Entry), bw);

            // Write name table
            _header.nameOffset = (int)bw.BaseStream.Position;
            _header.tableChunkSize = (int)((_header.nameOffset + nameStream.Length + 3) & ~3) - HeaderSize_;
            
            nameStream.Position = 0;
            nameStream.CopyTo(bw.BaseStream);
            bw.WriteAlignment(4);

            // Write header
            bw.BaseStream.Position = 0;
            typeWriter.Write(_header, bw);
        }

        private B123ArchiveFile CreateAfi(Stream input, string filePath, B123FileEntry entry)
        {
            ArchiveFileInfo fileInfo;

            input.Position = 0;
            using var br = new BinaryReaderX(input, true);

            if (br.ReadString(4) != "SSZL")
            {
                fileInfo = new ArchiveFileInfo
                {
                    FileData = input,
                    FilePath = filePath,
                    PluginIds = B123Support.RetrievePluginMapping(input, filePath)
                };
                return new B123ArchiveFile(fileInfo, entry);
            }

            br.BaseStream.Position = 0xC;
            int decompressedSize = br.ReadInt32();

            fileInfo = new CompressedArchiveFileInfo
            {
                FileData = input,
                FilePath = filePath,
                Compression = Compressions.Level5.Inazuma3Lzss.Build(),
                DecompressedSize = decompressedSize,
                PluginIds = B123Support.RetrievePluginMapping(input, filePath)
            };
            return new B123ArchiveFile(fileInfo, entry);
        }

        // TODO: Hashes of files to lower?
        private void BuildTables(IEnumerable<B123ArchiveFile> files,
            out IList<B123DirectoryEntry> directoryEntries, out IList<uint> directoryHashes,
            out IList<B123ArchiveFile> fileEntries, out Stream nameStream)
        {
            var groupedFiles = files.OrderBy(x => x.FilePath.GetDirectory())
                .GroupBy(x => x.FilePath.GetDirectory())
                .ToArray();

            var crc32 = Crc32.Crc32B;
            var sjis = Encoding.GetEncoding("SJIS");

            nameStream = new MemoryStream();
            using var nameBw = new BinaryWriterX(nameStream, sjis, true);

            var fileInfos = new List<B123ArchiveFile>();
            directoryEntries = new List<B123DirectoryEntry>();
            directoryHashes = new List<uint>();
            foreach (var fileGroup in groupedFiles)
            {
                var fileIndex = fileInfos.Count;
                var fileGroupEntries = fileGroup.ToArray();

                // Add directory entry first
                var directoryNameOffset = (int)nameBw.BaseStream.Position;
                var directoryName = fileGroup.Key.ToRelative().FullName;
                if (!string.IsNullOrEmpty(directoryName))
                    directoryName += "/";
                nameBw.WriteString(directoryName);

                var hash = crc32.ComputeValue(directoryName.ToLower(), sjis);
                var newDirectoryEntry = new B123DirectoryEntry
                {
                    crc32 = string.IsNullOrEmpty(fileGroup.Key.ToRelative().FullName) ? 0xFFFFFFFF : hash,

                    directoryCount = (short)groupedFiles.Count(gf => fileGroup.Key != gf.Key && gf.Key.IsInDirectory(fileGroup.Key, false)),

                    fileCount = (short)fileGroupEntries.Length,
                    firstFileIndex = (short)fileIndex,

                    directoryNameStartOffset = directoryNameOffset,
                    fileNameStartOffset = (int)nameBw.BaseStream.Position
                };
                if (newDirectoryEntry.crc32 != 0xFFFFFFFF)
                    directoryHashes.Add(newDirectoryEntry.crc32);
                directoryEntries.Add(newDirectoryEntry);

                // Write file names in alphabetic order
                foreach (B123ArchiveFile file in fileGroupEntries)
                {
                    file.Entry.nameOffsetInFolder = (uint)(nameBw.BaseStream.Position - newDirectoryEntry.fileNameStartOffset);
                    file.Entry.crc32 = crc32.ComputeValue(file.FilePath.GetName().ToLower(), sjis);

                    nameBw.WriteString(file.FilePath.GetName());
                }

                // Add file entries in order of ascending hash
                fileInfos.AddRange(fileGroupEntries.Cast<B123ArchiveFile>().OrderBy(x => x.Entry.crc32));
            }

            fileEntries = fileInfos;

            // Order directory entries by hash and set directoryIndex accordingly
            var directoryIndex = 0;
            directoryEntries = directoryEntries.OrderBy(x => x.crc32).Select(x =>
            {
                x.firstDirectoryIndex = directoryIndex;
                directoryIndex += x.directoryCount;
                return x;
            }).ToList();
        }
    }
}
