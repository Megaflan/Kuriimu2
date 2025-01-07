using Kanvas.DataClasses.Configuration;

namespace Kanvas.DataClasses
{
    internal class ImageTranscoderOptions
    {
        public ImageConfigurationOptions ImageOptions { get; } = new();
        public EncodingConfigurationOptions EncodingOptions { get; } = new();
        public ColorShaderConfigurationOptions ColorShaderOptions { get; } = new();
        public SizePaddingConfigurationOptions SizePaddingOptions { get; } = new();
        public PixelRemappingConfigurationOptions PixelRemappingOptions { get; } = new();
        public QuantizationConfigurationOptions? QuantizationOptions { get; set; }
    }
}
