using Kompression.Contract.DataClasses.Encoder.LempelZiv;
using Kompression.Contract.Encoder.LempelZiv.InputManipulation;
using Kompression.DataClasses.Configuration;

namespace Kompression.Encoder.LempelZiv.InputManipulation
{
    internal class InputManipulator : IInputManipulator
    {
        private readonly LempelZivInputAdjustmentOptions _options;

        public InputManipulator(LempelZivInputAdjustmentOptions options)
        {
            _options = options;
        }

        public Stream Manipulate(Stream input)
        {
            foreach (IInputManipulation manipulation in _options.InputManipulations)
                input = manipulation.Manipulate(input);

            return input;
        }

        public void AdjustMatch(LempelZivMatch match)
        {
            foreach (IInputManipulation manipulation in _options.InputManipulations.Reverse())
                manipulation.AdjustMatch(match);
        }
    }
}
