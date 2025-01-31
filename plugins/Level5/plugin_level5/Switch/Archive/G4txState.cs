using Komponent.IO;
using Konnect.Contract.DataClasses.FileSystem;
using Konnect.Contract.DataClasses.Plugin.File;
using Konnect.Contract.FileSystem;
using Konnect.Contract.Plugin.File;
using Konnect.Contract.Plugin.File.Archive;

namespace plugin_level5.Switch.Archive
{
    internal class G4txState : ILoadFiles, ISaveFiles, IReplaceFiles
    {
        private G4tx _g4tx = new();

        private List<G4txArchiveFile> _files;

        public IReadOnlyList<IArchiveFile> Files => _files;
        public bool ContentChanged => _files.Any(x => x.ContentChanged);

        public async Task Load(IFileSystem fileSystem, UPath filePath, LoadContext loadContext)
        {
            Stream fileStream = await fileSystem.OpenFileAsync(filePath);
            _files= _g4tx.Load(fileStream);
        }

        public async Task Save(IFileSystem fileSystem, UPath savePath, SaveContext saveContext)
        {
            Stream fileStream = await fileSystem.OpenFileAsync(savePath, FileMode.Create, FileAccess.ReadWrite);
            _g4tx.Save(fileStream, _files);
        }

        public void ReplaceFile(IArchiveFile file, Stream fileData)
        {
            using var br = new BinaryReaderX(fileData, true);
            if (br.ReadString(5) != "NXTCH")
                throw new InvalidOperationException("File needs to be a valid NXTCH.");

            file.SetFileData(fileData);
        }
    }
}
