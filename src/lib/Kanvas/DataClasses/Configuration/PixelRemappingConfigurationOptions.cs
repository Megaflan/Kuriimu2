using Kanvas.Contract.Configuration;

namespace Kanvas.DataClasses.Configuration
{
    internal class PixelRemappingConfigurationOptions
    {
        public CreatePixelRemapperDelegate? PixelRemappingDelegate { get; set; }
    }
}
