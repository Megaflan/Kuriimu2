using Komponent.Contract.Enums;

namespace Komponent.Contract.Aspects
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FixedLengthAttribute : Attribute
    {
        public int Length { get; }
        public StringEncoding StringEncoding { get; set; } = StringEncoding.Ascii;

        public FixedLengthAttribute(int length)
        {
            Length = length;
        }
    }
}
