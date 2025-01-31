using Konnect.Contract.DataClasses.FileSystem;
using Konnect.Contract.DataClasses.Plugin.File;
using Konnect.Contract.FileSystem;
using Konnect.Contract.Management.Files;
using Konnect.Contract.Plugin.File;
using Konnect.Contract.Plugin.File.Image;
using Konnect.Plugin.File.Image;

namespace plugin_level5.Image
{
    class ImgxState : IImageFilePluginState, ILoadFiles, ISaveFiles
    {
        private readonly IPluginFileManager _fileManager;
        private readonly Imgx _img = new();
        private readonly ImgxKtx _ktx = new();

        private int _format;
        private List<IImageFile> _images;

        public IReadOnlyList<IImageFile> Images => _images;
        public bool ContentChanged => _images.Any(x => x.ImageInfo.ContentChanged);

        public ImgxState(IPluginFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public async Task Load(IFileSystem fileSystem, UPath filePath, LoadContext loadContext)
        {
            var fileStream = await fileSystem.OpenFileAsync(filePath);

            // Specially handle format 0x2B, which is KTX
            fileStream.Position = 0x0A;
            _format = fileStream.ReadByte();
            fileStream.Position = 0;

            switch (_format)
            {
                // Load KTX by plugin
                case 0x2B:
                    var imageInfo = await _ktx.Load(fileStream, _fileManager);

                    _images = new List<IImageFile> { imageInfo };
                    break;

                // Otherwise load normal IMGx
                default:
                    var data = _img.Load(fileStream);
                    var def = await ImgxSupport.GetEncodingDefinition(_img.Magic, _img.Format, _img.BitDepth, loadContext.DialogManager);

                    _images = new List<IImageFile> { new ImageFile(data, def) };
                    break;
            }
        }

        public async Task Save(IFileSystem fileSystem, UPath savePath, SaveContext saveContext)
        {
            var fileStream = await fileSystem.OpenFileAsync(savePath, FileMode.Create, FileAccess.Write);

            switch (_format)
            {
                // Save KTX by plugin
                case 0x2B:
                    await _ktx.Save(fileStream, _fileManager);
                    break;

                // Otherwise save normal IMGx
                default:
                    _img.Save(fileStream, _images[0].ImageInfo);
                    break;
            }
        }
    }
}
