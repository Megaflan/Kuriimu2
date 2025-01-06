namespace Kompression.Contract.Configuration
{
    public interface ILempelZivHuffmanConfigurationBuilder : ICompressionConfigurationBuilder
    {
        /// <summary>
        /// Sets and modifies the configuration to search and find pattern matches.
        /// </summary>
        /// <param name="configure">The action to configure pattern match operations.</param>
        /// <returns>The configuration object.</returns>
        IHuffmanConfigurationBuilder ConfigureLempelZiv(ConfigureLempelZivOptions configure);

        /// <summary>
        /// Sets and modifies the configuration for huffman encodings.
        /// </summary>
        /// <param name="configure">The action to configure huffman encoding operations.</param>
        /// <returns>The configuration object.</returns>
        ILempelZivConfigurationBuilder ConfigureHuffman(ConfigureHuffmanOptions configure);
    }
}
