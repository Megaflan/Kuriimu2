using Konnect.Contract.Plugin.File.Archive;
using Konnect.Extensions;

namespace Kuriimu2.ImGui.Models
{
    class ArchiveFile
    {
        public IArchiveFile File { get; }

        public string Name { get; }

        public long Size { get; }

        public ArchiveFile(IArchiveFile afi)
        {
            File = afi;

            // Set them explicitly instead of a getter, to avoid potential race conditions from the rendering loop accessing this class
            Name = afi.FilePath.GetName();
            Size = afi.FileSize;
        }
    }
}
