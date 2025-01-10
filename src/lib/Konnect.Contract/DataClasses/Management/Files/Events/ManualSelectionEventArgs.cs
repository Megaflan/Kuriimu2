using Konnect.Contract.Plugin.File;
using Konnect.Contract.Enums.Management.Files;

namespace Konnect.Contract.DataClasses.Management.Files.Events
{
    public class ManualSelectionEventArgs : EventArgs
    {
        public IEnumerable<IFilePlugin> FilePlugins { get; }
        public IEnumerable<IFilePlugin> FilteredFilePlugins { get; }
        public SelectionStatus SelectionStatus { get; }

        public IFilePlugin Result { get; set; }

        public ManualSelectionEventArgs(IEnumerable<IFilePlugin> allFilePlugins, IEnumerable<IFilePlugin> filteredFilePlugins, SelectionStatus status)
        {
            FilePlugins = allFilePlugins;
            FilteredFilePlugins = filteredFilePlugins;
            SelectionStatus = status;
        }
    }
}
