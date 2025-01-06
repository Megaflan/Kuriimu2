using Kompression.Contract.Configuration;

namespace Kompression.DataClasses.Configuration
{
    internal class DecoderConfigurationOptions
    {
        public CreateDecoderDelegate? DecoderDelegate { get; set; }
    }
}
