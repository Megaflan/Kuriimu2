namespace Kompression.Contract.Configuration
{
    public delegate void ConfigureLempelZivOptions(ILempelZivOptionsBuilder options);

    public interface ILempelZivConfigurationBuilder : ICompressionConfigurationBuilder
    {
        /// <summary>
        /// Sets and modifies the configuration to search and find pattern matches.
        /// </summary>
        /// <param name="configure">The action to configure pattern match operations.</param>
        /// <returns>The configuration object.</returns>
        ICompressionConfigurationBuilder ConfigureLempelZiv(ConfigureLempelZivOptions configure);
    }
}
