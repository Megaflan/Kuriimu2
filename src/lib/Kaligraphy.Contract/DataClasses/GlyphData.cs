using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

namespace Kaligraphy.Contract.DataClasses
{
    public class GlyphData
    {
        /// <summary>
        /// The glyph.
        /// </summary>
        public required Image<Rgba32> Glyph { get; init; }

        /// <summary>
        /// Gets a description of the glyph, including position and size of the glyph after additional adjustments.
        /// </summary>
        public required GlyphDescriptionData Description { get; init; }
    }
}
