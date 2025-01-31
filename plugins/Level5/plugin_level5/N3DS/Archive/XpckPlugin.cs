using Komponent.IO;
using Konnect.Contract.DataClasses.FileSystem;
using Konnect.Contract.DataClasses.Plugin;
using Konnect.Contract.DataClasses.Plugin.File;
using Konnect.Contract.Enums.Plugin.File;
using Konnect.Contract.FileSystem;
using Konnect.Contract.Management.Files;
using Konnect.Contract.Plugin.File;

namespace plugin_level5.N3DS.Archive
{
    public class XpckPlugin : IIdentifyFiles
    {
        public Guid PluginId => Guid.Parse("de276e88-fb2b-48a6-a55f-d6c14ec60d4f");

        public PluginType PluginType => PluginType.Archive;
        public string[] FileExtensions => new[] { "*.xr", "*.xc", "*.xa", "*.xk" };

        public PluginMetadata Metadata { get; } = new()
        {
            Name = "XPCK",
            Author = "onepiecefreak",
            LongDescription = "Main archive for 3DS Level-5 games"
        };

        public async Task<bool> IdentifyAsync(IFileSystem fileSystem, UPath filePath, IdentifyContext identifyContext)
        {
            Stream fileStream = await fileSystem.OpenFileAsync(filePath);
            using var br = new BinaryReaderX(fileStream);

            return br.ReadString(4) == "XPCK";
        }

        public IFilePluginState CreatePluginState(IPluginFileManager fileManager)
        {
            return new XpckState();
        }
    }
}
