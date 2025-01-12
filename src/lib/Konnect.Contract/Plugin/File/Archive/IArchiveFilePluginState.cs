using Konnect.Contract.DataClasses.FileSystem;

namespace Konnect.Contract.Plugin.File.Archive
{
    /// <summary>
    /// Marks the state to be an archive and exposes properties to retrieve and modify file data from the state.
    /// </summary>
    public interface IArchiveFilePluginState : IFilePluginState
    {
        /// <summary>
        /// The read-only collection of files the current archive contains.
        /// </summary>
        IReadOnlyList<IArchiveFile> Files { get; }

        #region Optional feature support checks

        public bool CanReplaceFiles => this is IReplaceFiles;
        public bool CanRenameFiles => this is IRenameFiles;
        public bool CanDeleteFiles => this is IRemoveFiles;
        public bool CanAddFiles => this is IAddFiles;

        #endregion

        #region Optional feature casting defaults

        void AttemptReplaceFile(IArchiveFile afi, Stream fileData) => ((IReplaceFiles)this).ReplaceFile(afi, fileData);
        void AttemptRenameFile(IArchiveFile afi, UPath path) => ((IRenameFiles)this).RenameFile(afi, path);
        void AttemptRemoveFile(IArchiveFile afi) => ((IRemoveFiles)this).RemoveFile(afi);
        void AttemptRemoveAll() => ((IRemoveFiles)this).RemoveAll();
        IArchiveFile AttemptAddFile(Stream fileData, UPath filePath) => ((IAddFiles)this).AddFile(fileData, filePath);

        #endregion
    }
}
