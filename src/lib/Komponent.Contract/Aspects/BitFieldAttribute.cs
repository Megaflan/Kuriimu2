namespace Komponent.Contract.Aspects
{
    [AttributeUsage(AttributeTargets.Field)]
    public class BitFieldAttribute : Attribute
    {
        public int BitLength { get; }

        public BitFieldAttribute(int bitLength)
        {
            BitLength = bitLength;
        }
    }
}
