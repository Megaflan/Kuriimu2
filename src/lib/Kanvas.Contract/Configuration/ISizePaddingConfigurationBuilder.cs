namespace Kanvas.Contract.Configuration
{
    public interface ISizePaddingConfigurationBuilder
    {
        ISizePaddingDimensionConfigurationBuilder Width { get; }
        ISizePaddingDimensionConfigurationBuilder Height { get; }
        
        IImageConfigurationBuilder ToPowerOfTwo(int steps = 1);

        IImageConfigurationBuilder ToMultiple(int multiple);
    }
}
