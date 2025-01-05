using Kanvas.Contract.Configuration;

namespace Kanvas.DataClasses.Configuration
{
    internal class SizePaddingConfigurationOptions
    {
        public CreatePaddedSizeDimensionDelegate? WidthDelegate { get; set; }
        public CreatePaddedSizeDimensionDelegate? HeightDelegate { get; set; }
    }
}
