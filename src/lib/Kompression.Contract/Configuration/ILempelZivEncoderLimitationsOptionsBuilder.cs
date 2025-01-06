namespace Kompression.Contract.Configuration
{
    public interface ILempelZivEncoderLimitationsOptionsBuilder
    {
        ILempelZivEncoderAdditionalOptionsBuilder WithinLimitations(int minLength, int maxLength);
        ILempelZivEncoderAdditionalOptionsBuilder WithinLimitations(int minLength, int maxLength, int minDisplacement, int maxDisplacement);
    }
}
