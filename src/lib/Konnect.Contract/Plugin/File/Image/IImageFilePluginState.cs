using Konnect.Contract.DataClasses.Plugin.File.Image;

namespace Konnect.Contract.Plugin.File.Image
{
    public interface IImageFilePluginState : IFilePluginState
    {
        IEncodingDefinition EncodingDefinition { get; }

        IReadOnlyList<ImageInfo> Images { get; }
    }
}
