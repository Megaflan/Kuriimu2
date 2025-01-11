namespace Konnect.Contract.Enums.Management.Files
{
    public enum SaveErrorReason
    {
        None,
        Closed,
        Saving,
        Closing,
        NotLoaded,
        NoChanges,
        StateSaveError,
        DestinationNotExist,
        FileReplaceError,
        FileCopyError,
        StateReloadError
    }
}
