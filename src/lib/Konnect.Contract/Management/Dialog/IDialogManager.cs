using Konnect.Contract.DataClasses.Management.Dialog;

namespace Konnect.Contract.Management.Dialog
{
    /// <summary>
    /// An interface defining methods to communicate with the User Interface.
    /// </summary>
    public interface IDialogManager
    {
        /// <summary>
        /// Previously selected options, if any
        /// </summary>
        IList<string> DialogOptions { get; }

        /// <summary>
        /// Shows a dialog on which the user can interact with the plugin.
        /// </summary>
        /// <param name="fields">The fields to show on the dialog.</param>
        /// <returns>The selected options from the dialog.</returns>
        void ShowDialog(DialogField[] fields);
    }
}
