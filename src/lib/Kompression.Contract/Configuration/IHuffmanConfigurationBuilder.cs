namespace Kompression.Contract.Configuration
{
    public delegate void ConfigureHuffmanOptions(IHuffmanOptionsBuilder options);

    public interface IHuffmanConfigurationBuilder : ICompressionConfigurationBuilder
    {
        /// <summary>
        /// Sets and modifies the configuration for huffman encodings.
        /// </summary>
        /// <param name="configure">The action to configure huffman encoding operations.</param>
        /// <returns>The configuration object.</returns>
        ICompressionConfigurationBuilder ConfigureHuffman(ConfigureHuffmanOptions configure);
    }
}
