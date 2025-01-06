using Komponent.Streams;
using Kompression.Contract.DataClasses.Encoder.LempelZiv;
using Kompression.Contract.Encoder.LempelZiv.InputManipulation;

namespace Kompression.Encoder.LempelZiv.InputManipulation
{
    class SkipInput : IInputManipulation
    {
        private readonly int _skip;

        public SkipInput(int skip)
        {
            _skip = skip;
        }

        public Stream Manipulate(Stream input)
        {
            return new SubStream(input, _skip, input.Length - _skip)
            {
                Position = Math.Max(0, input.Position - _skip)
            };
        }

        public void AdjustMatch(LempelZivMatch lempelZivMatch)
        {
            lempelZivMatch.SetPosition(lempelZivMatch.Position + _skip);
        }
    }
}
