using Konnect.Contract.DataClasses.Plugin.File.Text;

namespace Konnect.Contract.Plugin.File.Text
{
    /// <summary>
    /// This interface allows the text adapter to delete entries through the UI.
    /// </summary>
    public interface IDeleteEntries : ITextFilePluginState
    {
        /// <summary>
        /// Deletes an entry and allows the plugin to perform any required deletion steps.
        /// </summary>
        /// <param name="entry">The entry to be deleted.</param>
        /// <returns>True if the entry was successfully deleted, False otherwise.</returns>
        bool DeleteEntry(TextEntry entry);
    }
}
