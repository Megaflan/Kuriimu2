namespace Kompression.Contract.Encoder.LempelZiv.PriceCalculator
{
    public interface ILempelZivPriceCalculator
    {
        int CalculateLiteralPrice(int value, int literalRunLength, bool firstLiteralRun);
        int CalculateMatchPrice(int displacement, int length, int matchRunLength, int firstValue);
    }
}
