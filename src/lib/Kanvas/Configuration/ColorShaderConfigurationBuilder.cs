using Kanvas.Contract.Configuration;
using Kanvas.DataClasses.Configuration;

namespace Kanvas.Configuration
{
    internal class ColorShaderConfigurationBuilder : IColorShaderConfigurationBuilder
    {
        private readonly IImageConfigurationBuilder _parent;
        private readonly ColorShaderConfigurationOptions _options;

        public ColorShaderConfigurationBuilder(IImageConfigurationBuilder parent, ColorShaderConfigurationOptions options)
        {
            _parent = parent;
            _options = options;
        }

        public IImageConfigurationBuilder With(CreateColorShaderDelegate shaderDelegate)
        {
            _options.ColorShaderDelegate = shaderDelegate;
            return _parent;
        }
    }
}
