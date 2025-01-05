using Kanvas.Contract.DataClasses;
using SixLabors.ImageSharp.PixelFormats;

namespace Kanvas.Contract.Encoding
{
    /// <summary>
    /// An interface for defining a color encoding to use in the Kanvas image library.
    /// </summary>
    public interface IColorEncoding : IEncodingInfo
    {
        /// <summary>
        /// Decodes image data to a list of colors.
        /// </summary>
        /// <param name="input">Image data to decode.</param>
        /// <param name="options">The context for the load operation.</param>
        /// <returns>Decoded list of colors.</returns>
        IEnumerable<Rgba32> Load(byte[] input, EncodingOptions options);

        /// <summary>
        /// Encodes a list of colors.
        /// </summary>
        /// <param name="colors">List of colors to encode.</param>
        /// <param name="options">The context for the save operation.</param>
        /// <returns>Encoded data.</returns>
        byte[] Save(IEnumerable<Rgba32> colors, EncodingOptions options);
    }
}
