using Kompression.Contract.Configuration;
using Kompression.Contract.DataClasses.Encoder.LempelZiv;

namespace Kompression.Contract.Encoder
{
    /// <summary>
    /// Provides functionality to encode data.
    /// </summary>
    public interface ILempelZivEncoder
    {
        /// <summary>
        /// Configures the match options for this specification.
        /// </summary>
        /// <param name="lempelZivOptions">The options to configure.</param>
        void Configure(ILempelZivEncoderOptionsBuilder lempelZivOptions);

        /// <summary>
        /// Encodes a stream of data.
        /// </summary>
        /// <param name="input">The input data to encode.</param>
        /// <param name="output">The output to encode to.</param>
        /// <param name="matches">The matches for the Lempel-Ziv compression.</param>
        void Encode(Stream input, Stream output, IEnumerable<LempelZivMatch> matches);
    }
}
