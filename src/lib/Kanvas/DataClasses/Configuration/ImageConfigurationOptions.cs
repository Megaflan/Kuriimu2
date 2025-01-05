using Kanvas.Contract.Enums;

namespace Kanvas.DataClasses.Configuration
{
    public class ImageConfigurationOptions
    {
        public ImageAnchor Anchor { get; set; } = ImageAnchor.TopLeft;
        public int TaskCount { get; set; } = Environment.ProcessorCount;
    }
}
