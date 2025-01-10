using Konnect.Contract.Enums.Management.Files;
using Konnect.Contract.Management.Files;

namespace Konnect.Contract.DataClasses.Management.Files
{
    public class LoadResult
    {
        /// <summary>
        /// The status of the load operation
        /// </summary>
        public required LoadStatus Status { get; init; }

        /// <summary>
        /// Contains the result if the load process was successful.
        /// </summary>
        public IFileState? LoadedFileState { get; init; }

        /// <summary>
        /// Contains a human readable message about the state of the load process.
        /// </summary>
        /// <remarks>Can contain a message, even if the load process was successful. <see langword="null" /> otherwise.</remarks>
        public string? Message { get; init; }

        /// <summary>
        /// Contains an exception, if any subsequent process was unsuccessful and finished with an exception.
        /// </summary>
        public Exception? Exception { get; init; }
    }
}
