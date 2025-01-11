using System.IO;
using System.Threading.Tasks;
using Konnect.Contract.DataClasses.FileSystem;
using Konnect.Contract.DataClasses.Plugin.File;
using Konnect.Contract.FileSystem;
using Konnect.Contract.Plugin.File;
using Konnect.Contract.Plugin.File.Hex;

namespace Kuriimu2.ImGui.Models
{
    public class HexState : IHexFilePluginState, ILoadFiles
    {
        public Stream FileStream { get; private set; }

        public async Task Load(IFileSystem fileSystem, UPath filePath, LoadContext loadContext)
        {
            FileStream = await fileSystem.OpenFileAsync(filePath);
        }
    }
}
