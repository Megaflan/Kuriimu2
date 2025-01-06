namespace Kompression.Contract.DataClasses.Encoder.LempelZiv.MatchFinder
{
    public class LempelZivMatchLimitations
    {
        public required int MinLength { get; init; }
        public required int MaxLength { get; init; }
        public int MinDisplacement { get; init; }
        public int MaxDisplacement { get; init; }
    }
}
