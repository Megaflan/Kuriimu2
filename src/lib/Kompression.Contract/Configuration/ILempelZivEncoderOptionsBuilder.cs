using Kompression.Contract.DataClasses.Encoder.LempelZiv.MatchFinder;
using Kompression.Contract.Encoder.LempelZiv.MatchFinder;
using Kompression.Contract.Encoder.LempelZiv.MatchParser;
using Kompression.Contract.Encoder.LempelZiv.PriceCalculator;
using Kompression.Contract.Enums.Encoder.LempelZiv;

namespace Kompression.Contract.Configuration
{
    public delegate ILempelZivMatchFinder CreateMatchFinderDelegate(LempelZivMatchFinderOptions options);
    public delegate ILempelZivPriceCalculator CreatePriceCalculatorDelegate();
    public delegate void AdjustInputDelegate(ILempelZivInputAdjustmentOptionsBuilder inputBuilder);

    public interface ILempelZivEncoderOptionsBuilder
    {
        ILempelZivEncoderLimitationsOptionsBuilder FindWith(CreateMatchFinderDelegate finderDelegate);
        ILempelZivEncoderLimitationsOptionsBuilder FindPatternMatches();
        ILempelZivEncoderLimitationsOptionsBuilder FindRunLength();
        ILempelZivEncoderLimitationsOptionsBuilder FindConstantRunLength(int value);

        ILempelZivEncoderOptionsBuilder CalculatePricesWith(CreatePriceCalculatorDelegate calculatorDelegate);

        ILempelZivEncoderOptionsBuilder SkipUnitsAfterMatch(int skipUnits);
        ILempelZivEncoderOptionsBuilder HasUnitSize(UnitSize unitSize);

        ILempelZivEncoderOptionsBuilder AdjustInput(AdjustInputDelegate inputDelegate);

        ILempelZivMatchParser Build();
    }
}
