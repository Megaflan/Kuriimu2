namespace Kanvas.Contract.Configuration
{
    public interface IIndexedImageConfigurationBuilder : IImageConfigurationBuilder
    {
        public IPaletteEncodingConfigurationBuilder TranscodePalette { get; }
    }
}
