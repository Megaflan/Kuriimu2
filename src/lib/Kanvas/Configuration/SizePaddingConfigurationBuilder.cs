using Kanvas.Contract.Configuration;
using Kanvas.DataClasses.Configuration;

namespace Kanvas.Configuration
{
    internal class SizePaddingConfigurationBuilder : ISizePaddingConfigurationBuilder
    {
        private readonly IImageConfigurationBuilder _parent;
        private readonly ISizePaddingDimensionConfigurationBuilder _widthBuilder;
        private readonly ISizePaddingDimensionConfigurationBuilder _heightBuilder;

        public ISizePaddingDimensionConfigurationBuilder Width => _widthBuilder;
        public ISizePaddingDimensionConfigurationBuilder Height => _heightBuilder;

        public SizePaddingConfigurationBuilder(IImageConfigurationBuilder parent, SizePaddingConfigurationOptions options)
        {
            _parent = parent;
            _widthBuilder = new SizePaddingDimensionConfigurationBuilder(this, widthDelegate => options.WidthDelegate = widthDelegate);
            _heightBuilder = new SizePaddingDimensionConfigurationBuilder(this, heightDelegate => options.HeightDelegate = heightDelegate);
        }

        public IImageConfigurationBuilder ToPowerOfTwo(int steps = 1)
        {
            _widthBuilder.ToPowerOfTwo(steps);
            _heightBuilder.ToPowerOfTwo(steps);

            return _parent;
        }

        public IImageConfigurationBuilder ToMultiple(int multiple)
        {
            _widthBuilder.ToMultiple(multiple);
            _heightBuilder.ToMultiple(multiple);

            return _parent;
        }
    }
}
