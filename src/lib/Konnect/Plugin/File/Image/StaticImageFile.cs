using Kanvas;
using Konnect.Contract.DataClasses.Plugin.File.Image;
using Konnect.Contract.Plugin.File.Image;
using Konnect.Contract.Progress;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Konnect.Plugin.File.Image
{
    public class StaticImageFile : IImageFile
    {
        private Image<Rgba32> _image;

        public IEncodingDefinition EncodingDefinition { get; }
        public ImageFileInfo ImageInfo { get; }
        public int BitDepth => 32;
        public bool IsIndexed => false;
        public bool IsImageLocked => true;

        public StaticImageFile(Image<Rgba32> image)
        {
            var encodingDefinition = new EncodingDefinition();
            encodingDefinition.AddColorEncoding(0, ImageFormats.Rgba8888());

            _image = image;

            EncodingDefinition = encodingDefinition;
            ImageInfo = new ImageFileInfo
            {
                ImageSize = image.Size,
                ImageData = Array.Empty<byte>(),
                ImageFormat = -1
            };
        }

        public StaticImageFile(Image<Rgba32> image, string name) : this(image)
        {
            var encodingDefinition = new EncodingDefinition();
            encodingDefinition.AddColorEncoding(0, ImageFormats.Rgba8888());

            _image = image;

            EncodingDefinition = encodingDefinition;
            ImageInfo = new ImageFileInfo
            {
                Name = name,
                ImageSize = image.Size,
                ImageData = Array.Empty<byte>(),
                ImageFormat = -1
            };
        }

        public Image<Rgba32> GetImage(IProgressContext progress = null)
        {
            return _image;
        }

        public void SetImage(Image<Rgba32> image, IProgressContext progress = null)
        {
            ImageInfo.ContentChanged = true;
            _image = image;
        }

        public void TranscodeImage(int imageFormat, IProgressContext progress = null)
        {
            if (IsImageLocked)
                throw new InvalidOperationException("Image cannot be transcoded to another format.");

            throw new InvalidOperationException("Transcoding image is not supported for static images.");
        }

        public IList<Rgba32> GetPalette(IProgressContext progress = null)
        {
            if (!IsIndexed)
                throw new InvalidOperationException("Image is not indexed.");

            throw new InvalidOperationException("Getting palette is not supported for static images.");
        }

        public void SetPalette(IList<Rgba32> palette, IProgressContext progress = null)
        {
            if (!IsIndexed)
                throw new InvalidOperationException("Image is not indexed.");

            throw new InvalidOperationException("Setting palette is not supported for static images.");
        }

        public void TranscodePalette(int paletteFormat, IProgressContext progress = null)
        {
            if (!IsIndexed)
                throw new InvalidOperationException("Image is not indexed.");

            throw new InvalidOperationException("Transcoding palette is not supported for static images.");
        }

        public void SetColorInPalette(int paletteIndex, Rgba32 color)
        {
            if (!IsIndexed)
                throw new InvalidOperationException("Image is not indexed.");

            throw new InvalidOperationException("Setting color in palette is not supported for static images.");
        }

        public void SetIndexInImage(Point point, int paletteIndex)
        {
            if (!IsIndexed)
                throw new InvalidOperationException("Image is not indexed.");

            throw new InvalidOperationException("Setting index in image is not supported for static images.");
        }

        public void Dispose()
        {
            _image = null;
        }
    }
}
