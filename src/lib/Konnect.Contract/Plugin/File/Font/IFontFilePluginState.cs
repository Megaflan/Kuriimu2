using Konnect.Contract.DataClasses.Plugin.File.Font;

namespace Konnect.Contract.Plugin.File.Font
{
    public interface IFontFilePluginState : IFilePluginState
    {
        /// <summary>
        /// The list of characters provided by the state.
        /// </summary>
        IReadOnlyList<CharacterInfo> Characters { get; }

        /// <summary>
        /// Character baseline.
        /// </summary>
        float Baseline { get; set; }

        /// <summary>
        /// Character descent line.
        /// </summary>
        float DescentLine { get; set; }

        #region Optional feature support checks

        public bool CanAddCharacter => this is IAddCharacters;
        public bool CanRemoveCharacter => this is IRemoveCharacters;

        #endregion
    }
}
