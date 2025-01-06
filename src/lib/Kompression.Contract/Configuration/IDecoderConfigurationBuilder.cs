using Kompression.Contract.Decoder;

namespace Kompression.Contract.Configuration
{
    public delegate IDecoder CreateDecoderDelegate();

    public interface IDecoderConfigurationBuilder
    {
        /// <summary>
        /// Sets the factory to create an <see cref="IDecoder"/>.
        /// </summary>
        /// <param name="decoderDelegate">The factory to create an <see cref="IDecoder"/>.</param>
        /// <returns>The configuration object.</returns>
        ICompressionConfigurationBuilder With(CreateDecoderDelegate decoderDelegate);
    }
}
