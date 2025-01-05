using Kanvas.Contract.Encoding;

namespace Kanvas.DataClasses.Configuration
{
    internal class EncodingConfigurationOptions
    {
        public IColorEncoding? ColorEncoding { get; set; }
        public IIndexEncoding? IndexEncoding { get; set; }
        public IColorEncoding? PaletteEncoding { get; set; }
    }
}
