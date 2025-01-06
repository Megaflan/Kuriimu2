using Komponent.IO;
using Kompression.Contract.DataClasses.Encoder.LempelZiv;

namespace Kompression.InternalContract.SlimeMoriMori.Encoder
{
    interface ISlimeEncoder
    {
        void Encode(Stream input, BinaryBitWriter bw, LempelZivMatch[] matches);
    }
}
