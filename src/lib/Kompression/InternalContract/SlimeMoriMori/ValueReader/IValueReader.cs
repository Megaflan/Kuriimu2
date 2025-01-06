using Komponent.IO;

namespace Kompression.InternalContract.SlimeMoriMori.ValueReader
{
    interface IValueReader : IDisposable
    {
        void BuildTree(BinaryBitReader br);

        byte ReadValue(BinaryBitReader br);
    }
}
