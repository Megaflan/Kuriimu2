﻿using Kompression.Contract.DataClasses.Encoder.LempelZiv;
using Kompression.Contract.DataClasses.Encoder.LempelZiv.MatchFinder;
using Kompression.Contract.Encoder.LempelZiv.MatchFinder;
using Kompression.Contract.Enums.Encoder.LempelZiv;

namespace Kompression.Encoder.LempelZiv.MatchFinder
{
    /// <summary>
    /// Find sequences of the same value.
    /// </summary>
    public class RleMatchFinder : ILempelZivMatchFinder
    {
        /// <inheritdoc />
        public LempelZivMatchFinderOptions Options { get; }

        /// <summary>
        /// Creates a new instance of <see cref="RleMatchFinder"/>.
        /// </summary>
        /// <param name="options">The options to search sequences with.</param>
        public RleMatchFinder(LempelZivMatchFinderOptions options)
        {
            Options = options;
        }

        /// <inheritdoc />
        public void PreProcess(byte[] input)
        {
        }

        /// <inheritdoc />
        public LempelZivAggregateMatch? FindMatchesAtPosition(byte[] input, int position)
        {
            if (input.Length - position < Options.Limitations.MinLength)
                return null;

            int maxLength = Options.Limitations.MaxLength <= 0 ? input.Length : Options.Limitations.MaxLength;
            var unitSize = (int)Options.UnitSize;

            int cappedLength = Math.Min(maxLength, input.Length - unitSize - position);
            for (var repetitions = 0; repetitions < cappedLength; repetitions += unitSize)
            {
                switch (Options.UnitSize)
                {
                    case UnitSize.Byte:
                        if (input[position + 1 + repetitions] != input[position])
                        {
                            if (repetitions > 0 && repetitions >= Options.Limitations.MinLength)
                                return new LempelZivAggregateMatch(0, repetitions);

                            return null;
                        }
                        break;

                    default:
                        throw new InvalidOperationException($"Unsupported unit size {Options.UnitSize}.");
                }
            }

            return new LempelZivAggregateMatch(0, cappedLength - cappedLength % (int)Options.UnitSize);
        }
        
        public void Dispose()
        {
        }
    }
}
