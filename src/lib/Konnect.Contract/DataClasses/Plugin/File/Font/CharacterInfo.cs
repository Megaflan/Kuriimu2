﻿using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

namespace Konnect.Contract.DataClasses.Plugin.File.Font
{
    /// <summary>
    /// A class representing one character of a font.
    /// </summary>
    public class CharacterInfo
    {
        /// <summary>
        /// The code point of this character.
        /// </summary>
        public required char CodePoint { get; init; }

        /// <summary>
        /// The size of the character.
        /// </summary>
        public required Size CharacterSize { get; set; }

        /// <summary>
        /// The glyph of the character.
        /// </summary>
        public required Image<Rgba32> Glyph { get; set; }

        /// <summary>
        /// Determines if the content of this character was changed.
        /// </summary>
        /// <remarks>Should only be set by this class or the responsible plugin.</remarks>
        public bool ContentChanged { get; set; }
    }
}
