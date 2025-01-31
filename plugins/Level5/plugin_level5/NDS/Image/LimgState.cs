using Konnect.Contract.DataClasses.FileSystem;
using Konnect.Contract.DataClasses.Plugin.File;
using Konnect.Contract.FileSystem;
using Konnect.Contract.Plugin.File;
using Konnect.Contract.Plugin.File.Image;
using Konnect.Plugin.File.Image;

namespace plugin_level5.NDS.Image
{
    internal class LimgState : IImageFilePluginState, ILoadFiles, ISaveFiles
    {
        private readonly Limg _limg = new();

        private List<IImageFile> _images;

        public IReadOnlyList<IImageFile> Images => _images;
        public bool ContentChanged => _images.Any(x => x.ImageInfo.ContentChanged);

        public async Task Load(IFileSystem fileSystem, UPath filePath, LoadContext loadContext)
        {
            Stream fileStream = await fileSystem.OpenFileAsync(filePath);

            var imageInfo = _limg.Load(fileStream);
            EncodingDefinition encodingDefinition = LimgSupport.GetEncodingDefinition();

            _images = new List<IImageFile> { new ImageFile(imageInfo, encodingDefinition) };
        }

        public async Task Save(IFileSystem fileSystem, UPath savePath, SaveContext saveContext)
        {
            Stream fileStream = await fileSystem.OpenFileAsync(savePath, FileMode.Create, FileAccess.Write);
            _limg.Save(fileStream, _images[0].ImageInfo);
        }
    }
}
