using Kompression.Contract.Encoder;

namespace Kompression.Contract.Configuration
{
    public delegate IEncoder CreateEncoderDelegate();
    public delegate ILempelZivEncoder CreateLempelZivEncoderDelegate();
    public delegate IHuffmanEncoder CreateHuffmanEncoderDelegate();
    public delegate ILempelZivHuffmanEncoder CreateLempelZivHuffmanEncoderDelegate();

    public interface IEncoderConfigurationBuilder
    {
        /// <summary>
        /// Sets the factory to create an <see cref="IEncoder"/>.
        /// </summary>
        /// <param name="encoderDelegate">The factory to create an <see cref="IEncoder"/>.</param>
        /// <returns>The configuration object.</returns>
        ICompressionConfigurationBuilder With(CreateEncoderDelegate encoderDelegate);

        /// <summary>
        /// Sets the factory to create an <see cref="ILempelZivEncoder"/>.
        /// </summary>
        /// <param name="encoderDelegate">The factory to create an <see cref="ILempelZivEncoder"/>.</param>
        /// <returns>The configuration object.</returns>
        ILempelZivConfigurationBuilder With(CreateLempelZivEncoderDelegate encoderDelegate);

        /// <summary>
        /// Sets the factory to create an <see cref="IHuffmanEncoder"/>.
        /// </summary>
        /// <param name="encoderDelegate">The factory to create an <see cref="IHuffmanEncoder"/>.</param>
        /// <returns>The configuration object.</returns>
        IHuffmanConfigurationBuilder With(CreateHuffmanEncoderDelegate encoderDelegate);

        /// <summary>
        /// Sets the factory to create an <see cref="ILempelZivHuffmanEncoder"/>.
        /// </summary>
        /// <param name="encoderDelegate">The factory to create an <see cref="ILempelZivHuffmanEncoder"/>.</param>
        /// <returns>The configuration object.</returns>
        ILempelZivHuffmanConfigurationBuilder With(CreateLempelZivHuffmanEncoderDelegate encoderDelegate);
    }
}
