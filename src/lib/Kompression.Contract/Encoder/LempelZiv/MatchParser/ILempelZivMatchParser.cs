using Kompression.Contract.DataClasses.Encoder.LempelZiv;
using Kompression.Contract.DataClasses.Encoder.LempelZiv.MatchParser;

namespace Kompression.Contract.Encoder.LempelZiv.MatchParser
{
    public interface ILempelZivMatchParser
    {
        LempelZivMatchParserOptions Options { get; }

        IEnumerable<LempelZivMatch> ParseMatches(Stream input);
    }
}
