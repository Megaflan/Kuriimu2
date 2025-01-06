namespace Kompression.InternalContract.SlimeMoriMori.Decoder
{
    interface ISlimeDecoder
    {
        void Decode(Stream input, Stream output);
    }
}
