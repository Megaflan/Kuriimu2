using Komponent.Contract.Enums;

namespace Komponent.DataClasses
{
    public class BitFieldInfo
    {
        public int BlockSize { get; set; } = 4;
        public BitOrder BitOrder { get; set; } = BitOrder.LeastSignificantBitFirst;
    }
}
