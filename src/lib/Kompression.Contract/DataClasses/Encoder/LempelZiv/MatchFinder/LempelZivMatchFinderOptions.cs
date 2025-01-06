using Kompression.Contract.Enums.Encoder.LempelZiv;

namespace Kompression.Contract.DataClasses.Encoder.LempelZiv.MatchFinder
{
    public class LempelZivMatchFinderOptions
    {
        public required LempelZivMatchLimitations Limitations { get; init; }
        public required UnitSize UnitSize { get; init; }
    }
}
