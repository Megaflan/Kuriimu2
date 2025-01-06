using Kompression.Contract.Encoder.LempelZiv.PriceCalculator;

namespace Kompression.Encoder.LempelZiv.PriceCalculators
{
    public class LzePriceCalculator : ILempelZivPriceCalculator
    {
        public int CalculateLiteralPrice(int value, int literalRunLength, bool firstLiteralRun)
        {
            if (literalRunLength % 3 == 3)
                return 6;

            return 10;
        }

        public int CalculateMatchPrice(int displacement, int length, int matchRunLength, int firstValue)
        {
            if (displacement > 4 && length > 0x12)
                throw new InvalidOperationException("Invalid match for Lze.");

            if (displacement <= 4)
                return 10;

            return 18;
        }
    }
}
