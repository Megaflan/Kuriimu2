using System.Text;
using Komponent.IO;
using Komponent.Streams;
using Konnect.Contract.DataClasses.Plugin.File.Archive;
using Konnect.Extensions;
using Kryptography.Checksum.Crc;
using plugin_level5.Compression;

namespace plugin_level5.N3DS.Archive
{
    // Game: Yo-kai Watch, Time Travelers, more Level5 games in general on 3DS
    public class Xpck
    {
        private const int HeaderSize_ = 20;
        private const int EntrySize_ = 12;

        private XpckHeader _header;
        private bool _allowZlib;    // ZLib is only supported for Switch and newer; older platforms may not implement ZLib

        public List<XpckArchiveFile> Load(Stream input)
        {
            var typeReader = new BinaryTypeReader();
            using var br = new BinaryReaderX(input, true);

            // Header
            var header = typeReader.Read<XpckHeader>(br);
            if (header == null)
                return new List<XpckArchiveFile>();

            _header = header;

            // Entries
            br.BaseStream.Position = _header.FileInfoOffset;
            var entries = typeReader.ReadMany<XpckFileInfo>(br, _header.FileCount);

            // File names
            var compNameTable = new SubStream(input, _header.FilenameTableOffset, _header.FilenameTableSize);
            _allowZlib = Level5Compressor.PeekCompressionMethod(compNameTable) == Level5CompressionMethod.ZLib;
            var decNames = new MemoryStream();
            Level5Compressor.Decompress(compNameTable, decNames);

            // Files
            using var nameList = new BinaryReaderX(decNames);

            var files = new List<XpckArchiveFile>();
            foreach (var entry in entries)
            {
                nameList.BaseStream.Position = entry.nameOffset;
                var name = nameList.ReadNullTerminatedString();

                var fileData = new SubStream(input, _header.DataOffset + entry.FileOffset, entry.FileSize);
                if (name == "RES.bin")
                {
                    var method = Level5Compressor.PeekCompressionMethod(fileData);
                    var decompressedSize = Level5Compressor.PeekDecompressedSize(fileData);
                    var configuration = Level5Compressor.GetCompression(method);

                    var fileInfo = new CompressedArchiveFileInfo
                    {
                        FileData = fileData,
                        FilePath = name,
                        Compression = configuration,
                        DecompressedSize = decompressedSize
                    };

                    files.Add(new XpckArchiveFile(fileInfo, entry));
                }
                else
                {
                    var fileInfo = new ArchiveFileInfo
                    {
                        FileData = fileData,
                        FilePath = name
                    };

                    files.Add(new XpckArchiveFile(fileInfo, entry));
                }
            }

            return files;
        }

        public void Save(Stream output, List<XpckArchiveFile> files)
        {
            var typeWriter = new BinaryTypeWriter();
            using var bw = new BinaryWriterX(output);

            // Collect names
            var nameStream = new MemoryStream();
            using var nameBw = new BinaryWriterX(nameStream, true);
            foreach (XpckArchiveFile file in files)
            {
                file.Entry.nameOffset = (ushort)nameStream.Position;

                nameBw.WriteString(file.FilePath.ToRelative().FullName, Encoding.ASCII, false);
            }

            var nameStreamComp = new MemoryStream();
            Compress(nameStream, nameStreamComp, _allowZlib ? Level5CompressionMethod.ZLib : Level5CompressionMethod.Lz10);

            // Write files
            _header.DataOffset = (ushort)((HeaderSize_ + files.Count * EntrySize_ + nameStreamComp.Length + 3) & ~3);

            var crc32 = Crc32.Crc32B;

            var fileOffset = (int)_header.DataOffset;
            foreach (XpckArchiveFile file in files.OrderBy(x => x.Entry.FileOffset))
            {
                output.Position = fileOffset;
                var writtenSize = file.WriteFileData(output, true);

                if (output.Position % 4 > 0)
                {
                    bw.WriteAlignment(4);
                }
                else
                {
                    bw.Write(0);
                }

                file.Entry.FileOffset = fileOffset - _header.DataOffset;
                file.Entry.FileSize = (int)writtenSize;
                file.Entry.hash = crc32.ComputeValue(file.FilePath.ToRelative().FullName);

                fileOffset = (int)output.Length;
            }

            _header.DataSize = (uint)output.Length - _header.DataOffset;

            // Entries
            _header.FileCount = (ushort)files.Count;
            _header.FileInfoOffset = (ushort)HeaderSize_;
            _header.FileInfoSize = (ushort)(EntrySize_ * files.Count);

            bw.BaseStream.Position = HeaderSize_;
            foreach (XpckArchiveFile file in files)
                typeWriter.Write(file.Entry, bw);

            // File names
            _header.FilenameTableOffset = (ushort)bw.BaseStream.Position;
            _header.FilenameTableSize = (ushort)((nameStreamComp.Length + 3) & ~3);
            nameStreamComp.CopyTo(output);

            // Header
            bw.BaseStream.Position = 0;
            typeWriter.Write(_header, bw);
        }

        private void Compress(Stream input, Stream output, Level5CompressionMethod compressionMethod)
        {
            input.Position = 0;
            output.Position = 0;

            Level5Compressor.Compress(input, output, compressionMethod);

            output.Position = 0;
            input.Position = 0;
        }
    }
}
