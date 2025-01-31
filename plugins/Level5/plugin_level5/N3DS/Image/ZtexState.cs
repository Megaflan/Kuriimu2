﻿using Konnect.Contract.DataClasses.FileSystem;
using Konnect.Contract.DataClasses.Plugin.File;
using Konnect.Contract.FileSystem;
using Konnect.Contract.Plugin.File;
using Konnect.Contract.Plugin.File.Image;
using Konnect.Plugin.File.Image;

namespace plugin_level5.N3DS.Image
{
    internal class ZtexState : IImageFilePluginState, ILoadFiles, ISaveFiles
    {
        private readonly Ztex _img = new();

        private List<ImageFile> _images;

        public IReadOnlyList<IImageFile> Images => _images;
        public bool ContentChanged => _images.Any(x => x.ImageInfo.ContentChanged);

        public async Task Load(IFileSystem fileSystem, UPath filePath, LoadContext loadContext)
        {
            Stream fileStream = await fileSystem.OpenFileAsync(filePath);
            EncodingDefinition encodingDefinition = ZtexSupport.GetEncodingDefinition();

            _images = _img.Load(fileStream).Select(x => new ImageFile(x, encodingDefinition)).ToList();
        }

        public async Task Save(IFileSystem fileSystem, UPath savePath, SaveContext saveContext)
        {
            Stream fileStream = await fileSystem.OpenFileAsync(savePath, FileMode.Create, FileAccess.Write);
            _img.Save(fileStream, _images.Select(x => x.ImageInfo).ToArray());
        }
    }
}
