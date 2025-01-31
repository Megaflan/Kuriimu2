﻿using Konnect.Contract.DataClasses.FileSystem;
using Konnect.Contract.DataClasses.Plugin.File;
using Konnect.Contract.FileSystem;
using Konnect.Contract.Plugin.File;
using Konnect.Contract.Plugin.File.Archive;

namespace plugin_level5.Mobile.Archive
{
    internal class Hp10State : ILoadFiles, ISaveFiles, IReplaceFiles
    {
        private readonly Hp10 _arc = new();

        private List<Hp10ArchiveFile> _files;

        public IReadOnlyList<IArchiveFile> Files => _files;
        public bool ContentChanged => _files.Any(x => x.ContentChanged);

        public async Task Load(IFileSystem fileSystem, UPath filePath, LoadContext loadContext)
        {
            Stream fileStream = await fileSystem.OpenFileAsync(filePath);
            _files = _arc.Load(fileStream);
        }

        public async Task Save(IFileSystem fileSystem, UPath savePath, SaveContext saveContext)
        {
            Stream fileStream = await fileSystem.OpenFileAsync(savePath, FileMode.Create, FileAccess.Write);
            _arc.Save(fileStream, _files);
        }

        public void ReplaceFile(IArchiveFile file, Stream fileData)
        {
            file.SetFileData(fileData);
        }
    }
}
