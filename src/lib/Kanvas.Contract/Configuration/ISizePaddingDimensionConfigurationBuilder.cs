namespace Kanvas.Contract.Configuration
{
    public delegate int CreatePaddedSizeDimensionDelegate(int dimension);

    public interface ISizePaddingDimensionConfigurationBuilder
    {
        ISizePaddingConfigurationBuilder To(int dimension);
        ISizePaddingConfigurationBuilder To(CreatePaddedSizeDimensionDelegate dimensionDelegateDelegate);

        ISizePaddingConfigurationBuilder ToPowerOfTwo(int steps = 1);

        ISizePaddingConfigurationBuilder ToMultiple(int multiple);
    }
}
