using Komponent.Contract.Enums;

namespace Komponent.DataClasses
{
    public class TypeChoice
    {
        public string FieldName { get; }
        public TypeChoiceComparer Comparer { get; }
        public ulong Value { get; }
        public Type InjectionType { get; }

        public TypeChoice(string fieldName, TypeChoiceComparer comp, ulong value, Type injectionType)
        {
            FieldName = fieldName;
            Comparer = comp;
            Value = value;
            InjectionType = injectionType;
        }
    }
}
