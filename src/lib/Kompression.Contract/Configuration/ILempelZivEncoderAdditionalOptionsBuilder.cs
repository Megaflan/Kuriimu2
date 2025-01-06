namespace Kompression.Contract.Configuration
{
    public interface ILempelZivEncoderAdditionalOptionsBuilder : ILempelZivEncoderOptionsBuilder
    {
        ILempelZivEncoderLimitationsOptionsBuilder AndFindWith(CreateMatchFinderDelegate finderDelegate);
        ILempelZivEncoderLimitationsOptionsBuilder AndFindPatternMatches();
        ILempelZivEncoderLimitationsOptionsBuilder AndFindRunLength();
        ILempelZivEncoderLimitationsOptionsBuilder AndFindConstantRunLength(int value);
    }
}
