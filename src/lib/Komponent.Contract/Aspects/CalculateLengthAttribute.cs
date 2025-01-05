using Komponent.Contract.Enums;

namespace Komponent.Contract.Aspects
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CalculateLengthAttribute : Attribute
    {
        public Type CalculationType { get; }
        public string CalculationMethodName { get; }

        public StringEncoding StringEncoding { get; set; } = StringEncoding.Ascii;

        public CalculateLengthAttribute(Type calculationType, string calculationMethod)
        {
            CalculationType = calculationType;
            CalculationMethodName = calculationMethod;
        }
    }
}
