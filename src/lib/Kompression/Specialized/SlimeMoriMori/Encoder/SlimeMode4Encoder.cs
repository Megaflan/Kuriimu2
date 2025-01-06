using Komponent.IO;
using Kompression.Contract.DataClasses.Encoder.LempelZiv;
using Kompression.InternalContract.SlimeMoriMori.Encoder;
using Kompression.InternalContract.SlimeMoriMori.ValueWriter;

namespace Kompression.Specialized.SlimeMoriMori.Encoder
{
    class SlimeMode4Encoder : ISlimeEncoder
    {
        private IValueWriter _valueWriter;

        public SlimeMode4Encoder(IValueWriter valueWriter)
        {
            _valueWriter = valueWriter;
        }

        public void Encode(Stream input, BinaryBitWriter bw, LempelZivMatch[] matches)
        {
            while (input.Position < input.Length)
            {
                _valueWriter.WriteValue(bw, (byte)input.ReadByte());
            }
        }
    }
}
