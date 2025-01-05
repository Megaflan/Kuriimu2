using Kanvas.Contract.Configuration;
using Kanvas.Quantization.ColorCache;
using Kanvas.Quantization.ColorQuantizer;

namespace Kanvas.DataClasses.Configuration
{
    internal class QuantizationConfigurationOptions
    {
        public int TaskCount { get; set; } = Environment.ProcessorCount;
        public int ColorCount { get; set; } = 256;
        public CreatePaletteDelegate? PaletteDelegate { get; set; }
        public CreateColorQuantizerDelegate ColorQuanitzerDelegate { get; set; } = (colorCount, _) => new WuColorQuantizer(6, 2, colorCount);
        public CreateColorCacheDelegate ColorCacheDelegate { get; set; } = palette => new EuclideanDistanceColorCache(palette);
        public CreateColorDithererDelegate? ColorDithererDelegate { get; set; }
    }
}
