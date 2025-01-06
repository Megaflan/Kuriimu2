using Kompression.Contract.Encoder.LempelZiv.InputManipulation;
using Kompression.Contract.Encoder.LempelZiv.MatchFinder;
using Kompression.Contract.Encoder.LempelZiv.PriceCalculator;
using Kompression.Contract.Enums.Encoder.LempelZiv;

namespace Kompression.Contract.DataClasses.Encoder.LempelZiv.MatchParser
{
    public class LempelZivMatchParserOptions
    {
        public required ILempelZivMatchFinder[] MatchFinders { get; init; }
        public required ILempelZivPriceCalculator PriceCalculator { get; init; }
        public required IInputManipulator InputManipulation { get; init; }
        public required UnitSize UnitSize { get; init; }
        public required int TaskCount { get; init; }
    }
}
