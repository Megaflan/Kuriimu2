using Kanvas.Contract.Configuration;
using Kanvas.Contract.Encoding;
using SixLabors.ImageSharp;

namespace Kanvas.Contract.DataClasses
{
    public class SwizzleOptions
    {
        /// <summary>
        /// The meta information of the encoding used.
        /// </summary>
        public required IEncodingInfo EncodingInfo { get; init; }

        /// <summary>
        /// The size of the image.<para></para>
        /// If <see cref="IImageConfigurationBuilder.PadSize"/> is specified, this is the padded size.
        /// </summary>
        public required Size Size { get; init; }
    }
}
