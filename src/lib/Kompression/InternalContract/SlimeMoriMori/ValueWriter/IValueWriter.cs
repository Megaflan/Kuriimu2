using Komponent.IO;

namespace Kompression.InternalContract.SlimeMoriMori.ValueWriter
{
    interface IValueWriter
    {
        void WriteValue(BinaryBitWriter bw, byte value);
    }
}
