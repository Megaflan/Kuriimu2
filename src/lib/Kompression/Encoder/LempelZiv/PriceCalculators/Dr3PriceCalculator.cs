using Kompression.Contract.Encoder.LempelZiv.PriceCalculator;

namespace Kompression.Encoder.LempelZiv.PriceCalculators
{
    public class Dr3PriceCalculator : ILempelZivPriceCalculator
    {
        public int CalculateLiteralPrice(int value, int literalRunLength, bool firstLiteralRun)
        {
            return 9;
        }

        public int CalculateMatchPrice(int displacement, int length, int matchRunLength, int firstValue)
        {
            return 17;
        }
    }
}
