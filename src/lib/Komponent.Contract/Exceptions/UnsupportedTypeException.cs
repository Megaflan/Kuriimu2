namespace Komponent.Contract.Exceptions
{
    public class UnsupportedTypeException : Exception
    {
        public UnsupportedTypeException(Type type)
            : base($"The given type {type.Name} is not supported.")
        {
        }
    }
}
