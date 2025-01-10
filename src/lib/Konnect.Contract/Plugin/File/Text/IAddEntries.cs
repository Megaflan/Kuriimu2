﻿using Konnect.Contract.DataClasses.Plugin.File.Text;

namespace Konnect.Contract.Plugin.File.Text
{
    /// <summary>
    /// This interface allows the text adapter to add new entries through the UI.
    /// </summary>
    public interface IAddEntries : ITextFilePluginState
    {
        /// <summary>
        /// Creates a new entry and allows the plugin to provide its derived type.
        /// </summary>
        /// <returns>TextEntry or a derived type.</returns>
        TextEntry NewEntry();

        /// <summary>
        /// Adds a newly created entry to the file and allows the plugin to perform any required adding steps.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns>True if the entry was added, False otherwise.</returns>
        bool AddEntry(TextEntry entry);
    }
}
