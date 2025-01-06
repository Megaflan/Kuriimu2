namespace Kompression.Contract.Configuration
{
    public interface ICompressionConfigurationBuilder
    {
        IEncoderConfigurationBuilder Encode { get; }

        IDecoderConfigurationBuilder Decode { get; }

        /// <summary>
        /// Builds the current configuration to an <see cref="ICompression"/>.
        /// </summary>
        /// <returns>The <see cref="ICompression"/> for this configuration.</returns>
        ICompression Build();

        ICompressionConfigurationBuilder Clone();
    }
}
