using SixLabors.ImageSharp;

namespace Kanvas.Contract.Configuration
{
    public delegate void CreatePaddedSizeDelegate(ISizePaddingConfigurationBuilder builder);

    public interface ISizePaddingConfigurationBuilder
    {
        ISizePaddingDimensionConfigurationBuilder Width { get; }
        ISizePaddingDimensionConfigurationBuilder Height { get; }

        IImageConfigurationBuilder To(Size size);
        IImageConfigurationBuilder To(CreatePaddedSizeDelegate sizeDelegate);

        IImageConfigurationBuilder ToPowerOfTwo(int steps = 1);

        IImageConfigurationBuilder ToMultiple(int multiple);
    }
}
