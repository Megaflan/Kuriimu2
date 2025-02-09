using Komponent.IO;
using Komponent.Streams;
using Konnect.Contract.DataClasses.Plugin.File.Image;
using Kryptography.Checksum.Crc;
using plugin_level5.Compression;
using SixLabors.ImageSharp;

namespace plugin_level5.Image
{
    public class Imgx
    {
        private ImgxHeader _header;

        private byte[] _tileLegacy;

        public string Magic => _header.magic;
        public int BitDepth => _header.bitDepth;
        public int Format => _header.imageFormat;

        public ImageFileInfo Load(Stream input)
        {
            var typeReader = new BinaryTypeReader();
            using var br = new BinaryReaderX(input);

            // Read header
            _header = typeReader.Read<ImgxHeader>(br);

            // Read info tables
            br.BaseStream.Position = _header.paletteInfoOffset;
            var paletteInfos = typeReader.ReadMany<ImgxPaletteInfo>(br, _header.paletteInfoCount).ToArray();

            br.BaseStream.Position = _header.imageInfoOffset;
            var imageInfos = typeReader.ReadMany<ImgxImageInfo>(br, _header.imageInfoCount).ToArray();

            // Get tile table
            var tileTableComp = new SubStream(input, _header.dataOffset + imageInfos[0].tileOffset, imageInfos[0].tileSize);
            var tileTable = new MemoryStream();
            Level5Compressor.Decompress(tileTableComp, tileTable);

            // Get image data
            var imageDataComp = new SubStream(input, _header.dataOffset + imageInfos[0].dataOffset, imageInfos[0].dataSize);
            var imageData = new MemoryStream();
            Level5Compressor.Decompress(imageDataComp, imageData);

            // Combine tiles to full image data
            tileTable.Position = imageData.Position = 0;
            var combinedImageStream = CombineTiles(tileTable, imageData, _header.bytesPerTile);

            // Split image data and mip map data
            var images = new byte[_header.imageCount][];
            var (width, height) = (_header.width + 7 & ~7, _header.height + 7 & ~7);
            for (var i = 0; i < _header.imageCount; i++)
            {
                images[i] = new byte[width * height * _header.bitDepth / 8];
                combinedImageStream.Read(images[i], 0, images[i].Length);

                (width, height) = (width >> 1, height >> 1);
            }

            var imageInfo = new ImageFileInfo
            {
                BitDepth = _header.bitDepth,
                ImageData = images.FirstOrDefault(),
                MipMapData = images.Skip(1).ToArray(),
                ImageFormat = _header.imageFormat,
                ImageSize = new Size(_header.width, _header.height),
                RemapPixels = context => new ImgxSwizzle(context, _header.magic)
            };

            if (paletteInfos.Length > 0)
            {
                // Get palette
                var paletteDataComp = new SubStream(input, _header.dataOffset + paletteInfos[0].offset, paletteInfos[0].size);
                var paletteData = new MemoryStream();
                Level5Compressor.Decompress(paletteDataComp, paletteData);

                imageInfo.PaletteBitDepth = (int)(paletteData.Length / paletteInfos[0].colorCount) * 8;
                imageInfo.PaletteData = paletteData.ToArray();
                imageInfo.PaletteFormat = paletteInfos[0].format;
            }

            return imageInfo;
        }

        public void Save(Stream output, ImageFileInfo imageInfo)
        {
            var typeWriter = new BinaryTypeWriter();
            using var bw = new BinaryWriterX(output);

            // Update header
            _header.width = (short)imageInfo.ImageSize.Width;
            _header.height = (short)imageInfo.ImageSize.Height;
            _header.imageFormat = (byte)imageInfo.ImageFormat;
            _header.bitDepth = (byte)imageInfo.BitDepth;
            _header.bytesPerTile = (short)(64 * imageInfo.BitDepth / 8);
            _header.imageCount = (byte)((imageInfo.MipMapData?.Count ?? 0) + 1);

            _header.paletteInfoOffset = 0x30;
            _header.paletteInfoCount = (ushort)(imageInfo.PaletteFormat >= 0 ? 1 : 0);

            _header.imageInfoOffset = _header.paletteInfoOffset;
            if (imageInfo.PaletteFormat >= 0)
                _header.imageInfoOffset += 0x10;
            _header.imageInfoCount = 1;

            _header.dataOffset = 0x48;
            if (imageInfo.PaletteFormat >= 0)
                _header.dataOffset += 0x10;

            // Write image data to stream
            var combinedImageStream = new MemoryStream();

            combinedImageStream.Write(imageInfo.ImageData);
            for (var i = 0; i < (imageInfo.MipMapData?.Count ?? 0); i++)
                combinedImageStream.Write(imageInfo.MipMapData[i]);

            // Create reduced tiles and indices
            var (imageData, tileTable) = SplitTiles(combinedImageStream, _header.bytesPerTile);

            // Write palette data
            int paletteOffset = _header.dataOffset;

            output.Position = paletteOffset;
            if (imageInfo.PaletteFormat >= 0)
            {
                Level5Compressor.Compress(new MemoryStream(imageInfo.PaletteData), output, Level5CompressionMethod.Huffman4Bit);
                bw.WriteAlignment(4);
            }

            int paletteSize = (int)output.Position - paletteOffset;

            // Write tile table
            var tileOffset = (int)output.Position;

            Level5Compressor.Compress(tileTable, output, Level5CompressionMethod.Huffman4Bit);
            bw.WriteAlignment(4);

            int tileSize = (int)output.Position - tileOffset;

            // Write image tiles
            var imageOffset = (int)output.Position;

            Level5Compressor.Compress(imageData, output, Level5CompressionMethod.Lz10);
            bw.WriteAlignment(4);

            int imageSize = (int)output.Position - imageOffset;

            // Write header
            output.Position = 0;
            typeWriter.Write(_header, bw);

            // Write palette entries
            if (imageInfo.PaletteFormat >= 0)
            {
                output.Position = _header.paletteInfoOffset;

                bw.Write(paletteOffset - _header.dataOffset);
                bw.Write(paletteSize);
                bw.Write((short)(imageInfo.PaletteData.Length * 8 / imageInfo.PaletteBitDepth));
                bw.Write((byte)1);
                bw.Write((byte)imageInfo.PaletteFormat);
                bw.Write(0);
            }

            // Write image entries
            output.Position = _header.imageInfoOffset;

            bw.Write(tileOffset - _header.dataOffset);
            bw.Write(tileSize);
            bw.Write(imageOffset - _header.dataOffset);
            bw.Write(imageSize);
            bw.Write(0L);
        }

        private Stream CombineTiles(Stream tileTableStream, Stream imageDataStream, int tileByteDepth)
        {
            using var tileTable = new BinaryReaderX(tileTableStream);

            var readEntry = new Func<BinaryReaderX, int>(br => br.ReadInt16());

            // Read legacy head
            var legacyIndicator = tileTable.ReadUInt16();
            tileTable.BaseStream.Position -= 2;

            if (legacyIndicator == 0x453)
            {
                _tileLegacy = tileTable.ReadBytes(8);
                readEntry = br => br.ReadInt32();
            }

            var tile = new byte[tileByteDepth];
            var result = new MemoryStream();
            while (tileTableStream.Position < tileTableStream.Length)
            {
                var entry = readEntry(tileTable);
                if (entry >= 0)
                {
                    imageDataStream.Position = entry * tileByteDepth;
                    imageDataStream.Read(tile, 0, tile.Length);
                }
                else
                    Array.Clear(tile, 0, tile.Length);

                result.Write(tile, 0, tile.Length);
            }

            result.Position = 0;
            return result;
        }

        private (Stream imageData, Stream tileData) SplitTiles(Stream image, int tileByteDepth)
        {
            var tileTable = new MemoryStream();
            var imageData = new MemoryStream();
            using var tileBw = new BinaryWriterX(tileTable, true);
            using var imageBw = new BinaryWriterX(imageData, true);

            if (_tileLegacy != null)
                tileTable.Write(_tileLegacy, 0, _tileLegacy.Length);

            var crc32 = Crc32.Crc32B;
            var tileDictionary = new Dictionary<uint, int>();

            // Add placeholder tile for all 0's
            var zeroTileHash = crc32.ComputeValue(new byte[tileByteDepth]);
            tileDictionary[zeroTileHash] = -1;

            var imageOffset = 0;
            var tileIndex = 0;
            while (imageOffset < image.Length)
            {
                var tile = new SubStream(image, imageOffset, tileByteDepth);
                var tileHash = crc32.ComputeValue(tile);
                if (!tileDictionary.ContainsKey(tileHash))
                {
                    tile.Position = 0;
                    tile.CopyTo(imageBw.BaseStream);

                    tileDictionary[tileHash] = tileIndex++;
                }

                if (_tileLegacy != null)
                    tileBw.Write(tileDictionary[tileHash]);
                else
                    tileBw.Write((short)tileDictionary[tileHash]);

                imageOffset += tileByteDepth;
            }

            imageData.Position = tileTable.Position = 0;
            return (imageData, tileTable);
        }
    }
}
