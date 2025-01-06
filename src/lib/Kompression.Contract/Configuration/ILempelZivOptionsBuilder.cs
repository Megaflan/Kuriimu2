using Kompression.Contract.DataClasses.Encoder.LempelZiv.MatchParser;
using Kompression.Contract.Encoder.LempelZiv.MatchParser;

namespace Kompression.Contract.Configuration
{
    public delegate ILempelZivMatchParser CreateMatchParserDelegate(LempelZivMatchParserOptions options);

    public interface ILempelZivOptionsBuilder
    {
        ILempelZivOptionsBuilder WithDegreeOfParallelism(int taskCount);

        ILempelZivOptionsBuilder ParseMatchesWith(CreateMatchParserDelegate parserDelegateDelegate);
    }
}
