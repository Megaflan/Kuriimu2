using Kanvas.Contract.Enums;

namespace Kanvas.Contract.Configuration
{
    public interface IImageConfigurationBuilder
    {
        IEncodingConfigurationBuilder Transcode { get; }

        ISizePaddingConfigurationBuilder PadSize { get; }

        IRemapPixelsConfigurationBuilder RemapPixels { get; }

        IColorShaderConfigurationBuilder ColorShader { get; }

        IImageConfigurationBuilder IsAnchoredAt(ImageAnchor anchor);

        IImageConfigurationBuilder WithDegreeOfParallelism(int taskCount);

        IImageConfigurationBuilder ConfigureQuantization(Action<IQuantizationConfigurationBuilder> configure);
        IImageConfigurationBuilder WithoutQuantization();

        IImageTranscoder Build();

        IImageConfigurationBuilder Clone();
    }
}
