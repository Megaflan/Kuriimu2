using Konnect.Contract.DataClasses.FileSystem;
using Konnect.Contract.DataClasses.Plugin.File;
using Konnect.Contract.FileSystem;
using Konnect.Contract.Plugin.File;
using Konnect.Contract.Plugin.File.Image;
using Konnect.Plugin.File.Image;

namespace plugin_level5.Switch.Image
{
    internal class NxtchState : IImageFilePluginState, ILoadFiles, ISaveFiles
    {
        private readonly Nxtch _nxtch = new();

        private List<IImageFile> _images;

        public IReadOnlyList<IImageFile> Images => _images;
        public bool ContentChanged => _images.Any(x => x.ImageInfo.ContentChanged);

        public async Task Load(IFileSystem fileSystem, UPath filePath, LoadContext loadContext)
        {
            Stream fileStream = await fileSystem.OpenFileAsync(filePath);
            EncodingDefinition encodingDefinition = NxtchSupport.GetEncodingDefinition();

            _images = new List<IImageFile> { new ImageFile(_nxtch.Load(fileStream), encodingDefinition) };
        }

        public Task Save(IFileSystem fileSystem, UPath savePath, SaveContext saveContext)
        {
            var fileStream = fileSystem.OpenFile(savePath, FileMode.Create, FileAccess.Write);
            _nxtch.Save(fileStream, _images[0].ImageInfo);

            return Task.CompletedTask;
        }
    }
}
