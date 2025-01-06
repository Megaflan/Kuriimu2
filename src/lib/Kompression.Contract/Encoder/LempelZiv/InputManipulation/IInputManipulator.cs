using Kompression.Contract.DataClasses.Encoder.LempelZiv;

namespace Kompression.Contract.Encoder.LempelZiv.InputManipulation
{
    public interface IInputManipulator
    {
        Stream Manipulate(Stream input);
        void AdjustMatch(LempelZivMatch match);
    }
}
