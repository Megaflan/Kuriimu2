using Kompression.Contract.Configuration;
using Kompression.DataClasses.Configuration;

namespace Kompression.Configuration
{
    internal class LempelZivOptionsBuilder : ILempelZivOptionsBuilder
    {
        private readonly LempelZivOptions _options;

        public LempelZivOptionsBuilder(LempelZivOptions options)
        {
            _options = options;
        }

        public ILempelZivOptionsBuilder WithDegreeOfParallelism(int taskCount)
        {
            _options.TaskCount = taskCount;
            return this;
        }

        public ILempelZivOptionsBuilder ParseMatchesWith(CreateMatchParserDelegate parserDelegateDelegate)
        {
            _options.CreateMatchParserDelegate = parserDelegateDelegate;
            return this;
        }
    }
}
