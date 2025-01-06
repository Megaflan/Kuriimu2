using Kompression.Contract.DataClasses.Encoder.LempelZiv;
using Kompression.Contract.DataClasses.Encoder.LempelZiv.MatchFinder;

namespace Kompression.Contract.Encoder.LempelZiv.MatchFinder
{
    public interface ILempelZivMatchFinder
    {
        LempelZivMatchFinderOptions Options { get; }

        void PreProcess(byte[] input);
        LempelZivAggregateMatch? FindMatchesAtPosition(byte[] input, int position);
    }
}
