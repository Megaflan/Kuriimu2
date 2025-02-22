﻿using System.Collections.Generic;
using SixLabors.ImageSharp.PixelFormats;

namespace Kontract.Kanvas.Quantization
{
    /// <summary>
    /// Describes methods to quantize a collection of colors.
    /// </summary>
    public interface IColorQuantizer
    {
        /// <summary>
        /// Determines if the quantizer can only use a fixed color cache.
        /// </summary>
        bool IsColorCacheFixed { get; }

        /// <summary>
        /// Determines if the color count can be changed.
        /// </summary>
        bool UsesVariableColorCount { get; }

        /// <summary>
        /// Determines if alpha is supported for quantization.
        /// </summary>
        bool SupportsAlpha { get; }

        /// <summary>
        /// Creates a palette out of a collection of colors.
        /// </summary>
        /// <param name="colors"></param>
        /// <returns></returns>
        IList<Rgba32> CreatePalette(IEnumerable<Rgba32> colors);

        /// <summary>
        /// Gets the fixed color cache for this quantizer.
        /// </summary>
        /// <param name="palette">The palette to store in the fixed color cache.</param>
        /// <returns>The fixed color cache for this quantizer.</returns>
        IColorCache GetFixedColorCache(IList<Rgba32> palette);
    }
}
