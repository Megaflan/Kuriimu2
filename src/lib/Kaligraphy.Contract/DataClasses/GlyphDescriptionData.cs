using SixLabors.ImageSharp;

namespace Kaligraphy.Contract.DataClasses
{
    public class GlyphDescriptionData
    {
        /// <summary>
        /// The position into the glyph, where the non-white space starts.
        /// </summary>
        public required Point Position { get; init; }

        /// <summary>
        /// The size of the non-white space glyph.
        /// </summary>
        public required Size Size { get; init; }
    }
}
