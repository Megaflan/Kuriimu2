namespace Kompression.DataClasses.Decoder.TaikoLz81
{
    internal class TaikoLz81Node
    {
        public TaikoLz81Node[] Children { get; } = new TaikoLz81Node[2];
        public int Value { get; set; } = -1;
        public bool IsLeaf => Value != -1;
    }
}
