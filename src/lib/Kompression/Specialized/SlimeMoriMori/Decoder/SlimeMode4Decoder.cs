using Komponent.Contract.Enums;
using Komponent.IO;
using Kompression.InternalContract.SlimeMoriMori.ValueReader;

namespace Kompression.Specialized.SlimeMoriMori.Decoder
{
    class SlimeMode4Decoder : SlimeDecoder
    {
        public SlimeMode4Decoder(IValueReader huffmanReader) : base(huffmanReader)
        {
        }

        public override void Decode(Stream input, Stream output)
        {
            using var br = new BinaryBitReader(input, BitOrder.MostSignificantBitFirst, 4, ByteOrder.LittleEndian);

            var uncompressedSize = br.ReadInt32() >> 8;
            br.ReadByte();
            HuffmanReader.BuildTree(br);

            while (output.Length < uncompressedSize)
                ReadHuffmanValues(br, output, 1, 1);
        }
    }
}
