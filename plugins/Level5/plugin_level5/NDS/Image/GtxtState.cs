using Konnect.Contract.DataClasses.FileSystem;
using Konnect.Contract.DataClasses.Plugin.File;
using Konnect.Contract.FileSystem;
using Konnect.Contract.Plugin.File;
using Konnect.Contract.Plugin.File.Image;
using Konnect.Extensions;
using Konnect.Plugin.File.Image;

namespace plugin_level5.NDS.Image
{
    internal class GtxtState : IImageFilePluginState, ILoadFiles, ISaveFiles
    {
        private readonly Gtxt _img = new();

        private List<IImageFile> _images;

        public IReadOnlyList<IImageFile> Images => _images;
        public bool ContentChanged => _images.Any(x => x.ImageInfo.ContentChanged);

        public async Task Load(IFileSystem fileSystem, UPath filePath, LoadContext loadContext)
        {
            Stream ltFileStream;
            Stream lpFileStream;

            if (filePath.GetExtensionWithDot() == ".lt")
            {
                ltFileStream = await fileSystem.OpenFileAsync(filePath);
                lpFileStream = await fileSystem.OpenFileAsync(filePath.ChangeExtension(".lp"));
            }
            else
            {
                ltFileStream = await fileSystem.OpenFileAsync(filePath.ChangeExtension(".lt"));
                lpFileStream = await fileSystem.OpenFileAsync(filePath);
            }

            var imageInfo = _img.Load(ltFileStream, lpFileStream);
            var encodingDefinition = GtxtSupport.GetEncodingDefinition();

            _images = new List<IImageFile> { new ImageFile(imageInfo, encodingDefinition) };
        }

        public async Task Save(IFileSystem fileSystem, UPath savePath, SaveContext saveContext)
        {
            Stream ltFileStream;
            Stream lpFileStream;

            if (savePath.GetExtensionWithDot() == ".lt")
            {
                ltFileStream = await fileSystem.OpenFileAsync(savePath, FileMode.Create, FileAccess.Write);
                lpFileStream = await fileSystem.OpenFileAsync(savePath.ChangeExtension(".lp"), FileMode.Create, FileAccess.Write);
            }
            else
            {
                ltFileStream = await fileSystem.OpenFileAsync(savePath.ChangeExtension(".lt"), FileMode.Create, FileAccess.Write);
                lpFileStream = await fileSystem.OpenFileAsync(savePath, FileMode.Create, FileAccess.Write);
            }

            _img.Save(ltFileStream, lpFileStream, _images[0].ImageInfo);
        }
    }
}
