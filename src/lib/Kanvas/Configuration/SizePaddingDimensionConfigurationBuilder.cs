using Kanvas.Contract.Configuration;
using System;

namespace Kanvas.Configuration
{
    internal class SizePaddingDimensionConfigurationBuilder : ISizePaddingDimensionConfigurationBuilder
    {
        private readonly ISizePaddingConfigurationBuilder _parent;
        private readonly Action<CreatePaddedSizeDimensionDelegate> _setDelegate;

        public SizePaddingDimensionConfigurationBuilder(ISizePaddingConfigurationBuilder parent, Action<CreatePaddedSizeDimensionDelegate> dimensionSetDelegate)
        {
            _parent = parent;
            _setDelegate = dimensionSetDelegate;
        }

        public ISizePaddingConfigurationBuilder To(int dimension)
        {
            _setDelegate.Invoke(_ => dimension);
            return _parent;
        }

        public ISizePaddingConfigurationBuilder To(CreatePaddedSizeDimensionDelegate dimensionDelegateDelegate)
        {
            _setDelegate.Invoke(dimensionDelegateDelegate);
            return _parent;
        }

        public ISizePaddingConfigurationBuilder ToPowerOfTwo(int steps = 1)
        {
            _setDelegate.Invoke(value => SizePadding.PowerOfTwo(value, steps));
            return _parent;
        }

        public ISizePaddingConfigurationBuilder ToMultiple(int multiple)
        {
            _setDelegate.Invoke(value => SizePadding.Multiple(value, multiple));
            return _parent;
        }
    }
}
