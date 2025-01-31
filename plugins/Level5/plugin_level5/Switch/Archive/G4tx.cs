using System.Text;
using Komponent.IO;
using Komponent.Streams;
using Konnect.Contract.DataClasses.Plugin.File.Archive;
using Konnect.Extensions;
using Kryptography.Checksum.Crc;

namespace plugin_level5.Switch.Archive
{
    // Hash: Crc32.Default
    public class G4tx
    {
        private const int HeaderSize_ = 96;
        private const int EntrySize_ = 48;
        private const int SubEntrySize_ = 24;

        private G4txHeader _header;
        private IList<G4txEntry> _entries;
        private IList<G4txSubEntry> _subEntries;
        private IList<byte> _ids;

        public List<G4txArchiveFile> Load(Stream input)
        {
            var typeReader = new BinaryTypeReader();
            using var br = new BinaryReaderX(input, true);

            // Read header
            _header = typeReader.Read<G4txHeader>(br);

            // Read entries
            _entries = typeReader.ReadMany<G4txEntry>(br, _header.textureCount);
            _subEntries = typeReader.ReadMany<G4txSubEntry>(br, _header.subTextureCount);
            br.SeekAlignment();

            // Skip hashes
            typeReader.ReadMany<uint>(br, _header.totalCount);

            // Read ids
            _ids = typeReader.ReadMany<byte>(br, _header.totalCount);
            br.SeekAlignment(4);

            // Prepare string reader
            var nxtchBase = (_header.headerSize + _header.tableSize + 0xF) & ~0xF;
            var stringSize = nxtchBase - input.Position;
            var stringStream = new SubStream(input, input.Position, stringSize);
            using var stringBr = new BinaryReaderX(stringStream);

            // Read string offsets
            var stringOffsets = typeReader.ReadMany<short>(br, _header.totalCount);

            // Add files
            // TODO: Check if name is set by order of entries or ID
            var result = new List<G4txArchiveFile>();
            var subEntryId = _header.textureCount;
            for (var i = 0; i < _header.textureCount; i++)
            {
                var entry = _entries[i];

                // Prepare base information
                stringStream.Position = stringOffsets[i];
                var name = stringBr.ReadNullTerminatedString();

                var fileStream = new SubStream(input, nxtchBase + entry.nxtchOffset, entry.nxtchSize);

                // Prepare sub entries
                var subEntries = new List<G4txSubTextureEntry>();
                foreach (var unkEntry in _subEntries.Where(x => x.entryId == i))
                {
                    stringStream.Position = stringOffsets[subEntryId];
                    var subName = stringBr.ReadNullTerminatedString();

                    subEntries.Add(new G4txSubTextureEntry(_ids[subEntryId++], unkEntry, subName));
                }

                var fileInfo = new ArchiveFileInfo
                {
                    FileData = fileStream,
                    FilePath = name + ".nxtch",
                    PluginIds = new[] { Guid.Parse("89222f8f-a345-45ed-9b79-e9e873bda1e9") }
                };

                result.Add(new G4txArchiveFile(fileInfo, entry, _ids[i], subEntries));
            }

            return result;
        }

        public void Save(Stream output, List<G4txArchiveFile> files)
        {
            var crc = Crc32.Crc32B;

            var typeReader = new BinaryTypeReader();
            var typeWriter = new BinaryTypeWriter();
            using var bw = new BinaryWriterX(output);
            using var br = new BinaryReaderX(output);

            // Calculate offsets
            var subEntryCount = files.Sum(x => x.Entries.Count);

            var entryOffset = HeaderSize_;
            var subEntryOffset = entryOffset + files.Count * EntrySize_;
            var hashOffset = (subEntryOffset + subEntryCount * SubEntrySize_ + 0xF) & ~0xF;
            var idOffset = hashOffset + (files.Count + subEntryCount) * 4;
            var stringOffset = (idOffset + (files.Count + subEntryCount) + 0x3) & ~0x3;
            var stringContentOffset = (stringOffset + (files.Count + subEntryCount) * 2 + 0x7) & ~0x7;
            var dataOffset = (stringContentOffset + files
                .Sum(x => x.FilePath.GetNameWithoutExtension().Length + 1 + x.Entries.Sum(y => y.Name.Length + 1)) + 0xF) & ~0xF;

            // Write files
            var dataPosition = dataOffset;
            foreach (G4txArchiveFile file in files)
            {
                output.Position = dataPosition;
                var writtenSize = file.WriteFileData(output, false);

                // Update file entry
                output.Position = dataPosition;
                var nxtchHeader = typeReader.Read<NxtchHeader>(br);

                file.Entry.nxtchOffset = dataPosition - dataOffset;
                file.Entry.nxtchSize = (int)(output.Length - dataPosition);
                file.Entry.width = (short)nxtchHeader.width;
                file.Entry.height = (short)nxtchHeader.height;

                dataPosition = (int)((dataPosition + writtenSize + 0xF) & ~0xF);
            }

            // Write strings
            var stringContentPosition = stringContentOffset;

            var names = files.Select(x => x.FilePath.GetNameWithoutExtension())
                .Concat(files.SelectMany(x => x.Entries.Select(y => y.Name))).ToArray();
            var stringOffsets = new List<short>();
            foreach (var name in names)
            {
                stringOffsets.Add((short)(stringContentPosition - stringOffset));

                output.Position = stringContentPosition;
                bw.WriteString(name, Encoding.ASCII, false);

                stringContentPosition += name.Length + 1;
            }

            // Write string offsets
            output.Position = stringOffset;
            typeWriter.WriteMany(stringOffsets, bw);

            // Write ids
            var ids = files.Select(x => x.Id).Concat(files.SelectMany(x => x.Entries.Select(y => y.Id)));

            output.Position = idOffset;
            typeWriter.WriteMany(ids, bw);

            // Write hashes
            var hashes = names.Select(x => crc.ComputeValue(x));

            output.Position = hashOffset;
            typeWriter.WriteMany(hashes, bw);

            // Write sub entries
            var subEntries = files.SelectMany(x => x.Entries.Select(y => y.EntryEntry)).ToArray();

            output.Position = subEntryOffset;
            typeWriter.WriteMany(subEntries, bw);

            // Write entries
            var entries = files.Select(x => x.Entry);

            output.Position = entryOffset;
            typeWriter.WriteMany(entries, bw);

            // Write header
            _header.textureCount = (short)files.Count;
            _header.tableSize = stringContentPosition - HeaderSize_;
            _header.textureDataSize = (int)output.Length - dataOffset;
            _header.subTextureCount = (byte)subEntries.Length;
            _header.totalCount = (short)(_header.textureCount + _header.subTextureCount);

            output.Position = 0;
            typeWriter.Write(_header, bw);
        }
    }
}
