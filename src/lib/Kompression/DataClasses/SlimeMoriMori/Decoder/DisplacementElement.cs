namespace Kompression.DataClasses.SlimeMoriMori.Decoder
{
    internal class DisplacementElement
    {
        public int ReadBits { get; }
        public int DisplacementStart { get; }

        public DisplacementElement(int readBits, int displacementStart)
        {
            ReadBits = readBits;
            DisplacementStart = displacementStart;
        }
    }
}
