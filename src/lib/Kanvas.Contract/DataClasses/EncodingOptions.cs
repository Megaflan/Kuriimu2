using SixLabors.ImageSharp;

namespace Kanvas.Contract.DataClasses
{
    /// <summary>
    /// The options for encoding operations.
    /// </summary>
    public class EncodingOptions
    {
        /// <summary>
        /// The degree of parallelism in the load operation.
        /// </summary>
        public required int TaskCount { get; init; }

        /// <summary>
        /// The dimensions of the image to save.
        /// </summary>
        public required Size Size { get; init; }
    }
}
