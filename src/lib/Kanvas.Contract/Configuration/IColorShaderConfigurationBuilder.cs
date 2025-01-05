namespace Kanvas.Contract.Configuration
{
    public delegate IColorShader CreateColorShaderDelegate();

    public interface IColorShaderConfigurationBuilder
    {
        IImageConfigurationBuilder With(CreateColorShaderDelegate shaderDelegate);
    }
}
