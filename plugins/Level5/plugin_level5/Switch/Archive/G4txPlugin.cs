using Komponent.IO;
using Konnect.Contract.DataClasses.FileSystem;
using Konnect.Contract.DataClasses.Plugin;
using Konnect.Contract.DataClasses.Plugin.File;
using Konnect.Contract.Enums.Plugin.File;
using Konnect.Contract.FileSystem;
using Konnect.Contract.Management.Files;
using Konnect.Contract.Plugin.File;

namespace plugin_level5.Switch.Archive
{
    public class G4txPlugin : IIdentifyFiles
    {
        public Guid PluginId => Guid.Parse("ae6bc510-096b-4dcd-ba9c-b67985d2bed2");

        public PluginType PluginType => PluginType.Archive;
        public string[] FileExtensions => new[] { "*.g4tx" };

        public PluginMetadata Metadata { get; } = new()
        {
            Name = "G4TX",
            Author = "onepiecefreak",
            LongDescription = "The main image resource container in some Level5 Switch games."
        };

        public async Task<bool> IdentifyAsync(IFileSystem fileSystem, UPath filePath, IdentifyContext identifyContext)
        {
            Stream fileStream = await fileSystem.OpenFileAsync(filePath);

            using var br = new BinaryReaderX(fileStream);
            return br.ReadString(4) == "G4TX";
        }

        public IFilePluginState CreatePluginState(IPluginFileManager fileManager)
        {
            return new G4txState();
        }
    }
}
