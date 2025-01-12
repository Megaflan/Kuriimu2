namespace Konnect.Contract.Plugin.File
{
    /// <summary>
    /// This interface allows a plugin to create files.
    /// </summary>
    public interface ICreateFiles : IFilePluginState
    {
        /// <summary>
        /// Creates a new instance of the underlying format.
        /// </summary>
        void Create();
    }
}
