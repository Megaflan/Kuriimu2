using Komponent.IO;
using Kompression.Contract.DataClasses.Encoder.Huffman;
using Kompression.InternalContract.SlimeMoriMori.ValueWriter;

namespace Kompression.Specialized.SlimeMoriMori.ValueWriter
{
    class HuffmanWriter : IValueWriter
    {
        private IDictionary<int, string> _huffmanCodes;

        public HuffmanWriter(HuffmanTreeNode huffmanTree)
        {
            _huffmanCodes = huffmanTree.GetHuffCodes().ToDictionary(node => node.Item1, node => node.Item2);
        }

        public void WriteValue(BinaryBitWriter bw, byte value)
        {
            var code = _huffmanCodes[value].Select(x => x - '0').Aggregate((a, b) => (a << 1) | b);
            bw.WriteBits(code, _huffmanCodes[value].Length);
        }
    }
}
