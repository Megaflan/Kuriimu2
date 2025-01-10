namespace Konnect.Contract.DataClasses.Management.Files
{
    public class SaveResult
    {
        /// <summary>
        /// Declares if the save process was successful.
        /// </summary>
        public required bool IsSuccessful { get; init; }

        /// <summary>
        /// Contains a human readable message about the state of the save process.
        /// </summary>
        /// <remarks>Can contain a message, even if the save process was successful. <see langword="null" /> otherwise.</remarks>
        public string? Message { get; init; }

        /// <summary>
        /// Contains an exception, if any subsequent process was unsuccessful and finished with an exception.
        /// </summary>
        public Exception? Exception { get; init; }
    }
}
