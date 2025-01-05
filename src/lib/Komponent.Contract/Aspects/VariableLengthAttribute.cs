using Komponent.Contract.Enums;

namespace Komponent.Contract.Aspects
{
    [AttributeUsage(AttributeTargets.Field)]
    public class VariableLengthAttribute : Attribute
    {
        public string FieldName { get; }
        public StringEncoding StringEncoding { get; set; } = StringEncoding.Ascii;
        public int Offset { get; set; }

        public VariableLengthAttribute(string fieldName)
        {
            FieldName = fieldName;
        }
    }
}
