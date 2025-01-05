using Komponent.Contract.Enums;

namespace Komponent.Contract.Aspects
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class BitFieldInfoAttribute : Attribute
    {
        public int BlockSize { get; set; } = 1;
        public BitOrder BitOrder { get; set; } = BitOrder.MostSignificantBitFirst;
    }
}
