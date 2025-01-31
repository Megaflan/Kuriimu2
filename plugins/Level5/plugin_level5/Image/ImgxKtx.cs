using Komponent.IO;
using Komponent.Streams;
using Konnect.Contract.DataClasses.FileSystem;
using Konnect.Contract.Enums.Management.Files;
using Konnect.Contract.Management.Files;
using Konnect.Contract.Plugin.File.Image;
using Konnect.Extensions;
using plugin_level5.Compression;

namespace plugin_level5.Image
{
    class ImgxKtx
    {
        private static readonly Guid KtxPluginId = Guid.Parse("d25919cc-ac22-4f4a-94b2-b0f42d1123d4");

        private ImgxHeader _header;
        private Level5CompressionMethod _dataCompressionFormat;

        private IFileState _ktxState;
        private IImageFilePluginState _imageState;

        public async Task<IImageFile> Load(Stream input, IPluginFileManager fileManager)
        {
            var typeReader = new BinaryTypeReader();
            using var br = new BinaryReaderX(input);

            // Read header
            _header = typeReader.Read<ImgxHeader>(br);

            // Load KTX
            _imageState = await LoadKtx(input, "file.ktx", fileManager);

            return _imageState.Images[0];
        }

        public async Task Save(Stream output, IPluginFileManager fileManager)
        {
            var typeWriter = new BinaryTypeWriter();
            await using var bw = new BinaryWriterX(output);

            // Save Ktx
            var saveResult = await fileManager.SaveStream(_ktxState);
            if (!saveResult.IsSuccessful)
                throw new InvalidOperationException($"Could not save KTX: {saveResult.Reason}");

            // Compress saved Ktx to output
            output.Position = _header.tableDataOffset;
            Level5Compressor.Compress(saveResult.SavedStreams[0].Stream, output, _dataCompressionFormat);
            var compressedLength = output.Length - _header.tableDataOffset;

            // Align file to 16 bytes
            bw.WriteAlignment(16);

            // Update header
            _header.imageCount = (byte)((_imageState.Images[0].ImageInfo.MipMapData?.Count ?? 0) + 1);
            _header.width = (short)_imageState.Images[0].ImageInfo.ImageSize.Width;
            _header.height = (short)_imageState.Images[0].ImageInfo.ImageSize.Height;
            _header.imgDataSize = (int)compressedLength;

            // Write header
            output.Position = 0;
            typeWriter.Write(_header, bw);

            // Close Ktx
            fileManager.Close(_ktxState);
        }

        private async Task<IImageFilePluginState> LoadKtx(Stream fileStream, UPath filePath, IPluginFileManager fileManager)
        {
            var imgData = new SubStream(fileStream, _header.tableDataOffset, _header.imgDataSize);
            _dataCompressionFormat = Level5Compressor.PeekCompressionMethod(imgData);

            var ktxFile = new MemoryStream();
            Level5Compressor.Decompress(imgData, ktxFile);
            ktxFile.Position = 0;

            var file = new StreamFile
            {
                Stream = ktxFile,
                Path = filePath.GetNameWithoutExtension() + ".ktx"
            };

            var loadResult = await fileManager.LoadFile(file, KtxPluginId);
            if (loadResult.Status != LoadStatus.Successful)
                throw new InvalidOperationException(loadResult.Reason.ToString());
            if (loadResult.LoadedFileState.PluginState is not IImageFilePluginState)
                throw new InvalidOperationException("The embedded KTX version is not supported.");

            _ktxState = loadResult.LoadedFileState;
            return (IImageFilePluginState)_ktxState.PluginState;
        }
    }
}
