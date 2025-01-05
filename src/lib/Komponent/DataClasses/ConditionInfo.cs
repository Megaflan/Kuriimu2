using Komponent.Contract.Enums;

namespace Komponent.DataClasses
{
    public class ConditionInfo
    {
        public string FieldName { get; }
        public ConditionComparer Comparer { get; }
        public ulong Value { get; }

        public ConditionInfo(string fieldName, ConditionComparer comp, ulong value)
        {
            FieldName = fieldName;
            Comparer = comp;
            Value = value;
        }
    }
}
