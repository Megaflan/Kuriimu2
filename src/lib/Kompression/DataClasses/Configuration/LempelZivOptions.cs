using Kompression.Contract.Configuration;
using Kompression.Encoder.LempelZiv.MatchParser;

namespace Kompression.DataClasses.Configuration
{
    internal class LempelZivOptions
    {
        public int TaskCount { get; set; } = Environment.ProcessorCount;

        public CreateMatchParserDelegate CreateMatchParserDelegate { get; set; } =
            options => new OptimalLempelZivMatchParser(options);
    }
}
