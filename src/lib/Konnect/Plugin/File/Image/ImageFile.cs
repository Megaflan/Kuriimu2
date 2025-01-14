using Kanvas.Configuration;
using Kanvas.Contract;
using Kanvas.Contract.DataClasses;
using Kanvas.Contract.Encoding;
using Konnect.Contract.DataClasses.Plugin.File.Image;
using Konnect.Contract.Plugin.File.Image;
using Konnect.Contract.Progress;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Konnect.Plugin.File.Image
{
    public class ImageFile : IImageFile
    {
        private Image<Rgba32> _decodedImage;
        private Image<Rgba32> _bestImage;

        private IList<Rgba32> _decodedPalette;

        #region Properties

        private int TaskCount => Environment.ProcessorCount;

        /// <inheritdoc />
        public IEncodingDefinition EncodingDefinition { get; }

        /// <inheritdoc />
        public ImageFileInfo ImageInfo { get; }

        /// <inheritdoc />
        public bool IsIndexed => IsIndexEncoding(ImageInfo.ImageFormat);

        /// <inheritdoc />
        public bool IsImageLocked { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="ImageInfo"/>.
        /// </summary>
        /// <param name="imageInfo">The image info to represent.</param>
        /// <param name="encodingDefinition">The definition of encodings to use on the data.</param>
        public ImageFile(ImageFileInfo imageInfo, IEncodingDefinition encodingDefinition)
        {
            ImageInfo = imageInfo;
            EncodingDefinition = encodingDefinition;
        }

        /// <summary>
        /// Creates a new instance of <see cref="ImageInfo"/>.
        /// </summary>
        /// <param name="imageInfo">The image info to represent.</param>
        /// <param name="encodingDefinition">The definition of encodings to use on the data.</param>
        /// <param name="lockImage">Locks the image to its initial dimension and encodings. This will throw an exception in the methods that may try such changes.</param>
        public ImageFile(ImageFileInfo imageInfo, bool lockImage, IEncodingDefinition encodingDefinition)
            : this(imageInfo, encodingDefinition)
        {
            IsImageLocked = lockImage;
        }

        #endregion

        protected virtual Image<Rgba32> GetDecodedImage(IProgressContext? progress = null)
        {
            IImageTranscoder transcoder;

            if (IsIndexed)
            {
                transcoder = CreateImageConfiguration(ImageInfo.ImageFormat, ImageInfo.PaletteFormat)
                    .Transcode.With(EncodingDefinition.GetIndexEncoding(ImageInfo.ImageFormat).IndexEncoding)
                    .TranscodePalette.With(EncodingDefinition.GetPaletteEncoding(ImageInfo.PaletteFormat))
                    .Build();

                return transcoder.Decode(ImageInfo.ImageData, ImageInfo.PaletteData, ImageInfo.ImageSize);
            }

            transcoder = CreateImageConfiguration(ImageInfo.ImageFormat, ImageInfo.PaletteFormat)
                .Transcode.With(EncodingDefinition.GetColorEncoding(ImageInfo.ImageFormat))
                .Build();

            return transcoder.Decode(ImageInfo.ImageData, ImageInfo.ImageSize);
        }

        protected virtual (byte[], byte[]) GetEncodedImage(Image<Rgba32> image, int imageFormat, int paletteFormat, IProgressContext? progress = null)
        {
            IImageTranscoder transcoder;

            if (IsIndexed)
            {
                var indexEncoding = EncodingDefinition.GetIndexEncoding(imageFormat).IndexEncoding;
                var paletteEncoding = EncodingDefinition.GetPaletteEncoding(paletteFormat);

                ImageInfo.BitDepth = indexEncoding.BitDepth;

                transcoder = CreateImageConfiguration(imageFormat, paletteFormat)
                    .ConfigureQuantization(options => options.WithColorCount(indexEncoding.MaxColors))
                    .Transcode.With(indexEncoding)
                    .TranscodePalette.With(paletteEncoding)
                    .Build();
            }
            else
            {
                var encoding = EncodingDefinition.GetColorEncoding(imageFormat);

                ImageInfo.BitDepth = encoding.BitDepth;

                transcoder = CreateImageConfiguration(imageFormat, paletteFormat)
                    .Transcode.With(encoding)
                    .Build();
            }

            return transcoder.Encode(image);
        }

        protected virtual byte[] GetEncodedMipMap(Image<Rgba32> image, int imageFormat, IList<Rgba32> palette, IProgressContext? progress = null)
        {
            IImageTranscoder transcoder;

            if (IsIndexed)
            {
                var indexEncoding = EncodingDefinition.GetIndexEncoding(imageFormat).IndexEncoding;
                transcoder = CreateImageConfiguration(ImageInfo.ImageFormat, ImageInfo.PaletteFormat)
                    .ConfigureQuantization(options => options.WithColorCount(indexEncoding.MaxColors).WithPalette(() => palette))
                    .Transcode.With(indexEncoding)
                    .Build();
            }
            else
            {
                transcoder = CreateImageConfiguration(ImageInfo.ImageFormat, ImageInfo.PaletteFormat)
                    .Transcode.With(EncodingDefinition.GetColorEncoding(imageFormat))
                    .Build();
            }

            return transcoder.Encode(image).imageData;
        }

        private ImageConfigurationBuilder CreateImageConfiguration(int imageFormat, int paletteFormat)
        {
            var config = new ImageConfigurationBuilder();

            config.IsAnchoredAt(ImageInfo.IsAnchoredAt);

            if (ImageInfo.PadSize != null)
                config.PadSize.To(ImageInfo.PadSize);

            if (ImageInfo.RemapPixels != null)
                config.RemapPixels.With(ImageInfo.RemapPixels);

            if (IsIndexed)
            {
                if (EncodingDefinition.ContainsPaletteShader(paletteFormat))
                    config.ShadeColors.With(() => EncodingDefinition.GetPaletteShader(paletteFormat));

                var indexEncoding = EncodingDefinition.GetIndexEncoding(imageFormat).IndexEncoding;
                config.Transcode.With(indexEncoding);
            }
            else
            {
                if (EncodingDefinition.ContainsColorShader(imageFormat))
                    config.ShadeColors.With(() => EncodingDefinition.GetColorShader(imageFormat));

                var encoding = EncodingDefinition.GetColorEncoding(imageFormat);
                config.Transcode.With(encoding);
            }

            return config;
        }

        private IEncodingInfo GetEncodingInfo(int imageFormat)
        {
            if (IsIndexed)
                return EncodingDefinition.GetIndexEncoding(imageFormat).IndexEncoding;

            return EncodingDefinition.GetColorEncoding(imageFormat);
        }

        #region Image methods

        /// <inheritdoc />
        public Image<Rgba32> GetImage(IProgressContext? progress = null)
        {
            return DecodeImage(progress);
        }

        /// <inheritdoc />
        public void SetImage(Image<Rgba32> image, IProgressContext? progress = null)
        {
            // Check for locking
            if (IsImageLocked && (ImageInfo.ImageSize.Width != image.Width || ImageInfo.ImageSize.Height != image.Height))
                throw new InvalidOperationException("Only images with the same dimensions can be set.");

            _bestImage = image;

            _decodedImage = null;
            _decodedPalette = null;

            var (imageData, paletteData) = EncodeImage(image, ImageInfo.ImageFormat, ImageInfo.PaletteFormat, progress);

            ImageInfo.BitDepth = GetEncodingInfo(ImageInfo.ImageFormat).BitDepth;
            ImageInfo.ImageData = imageData.FirstOrDefault();
            ImageInfo.PaletteData = paletteData;
            ImageInfo.MipMapData = imageData.Skip(1).ToArray();
            ImageInfo.ImageSize = image.Size;

            ImageInfo.ContentChanged = true;
        }

        /// <inheritdoc />
        public void TranscodeImage(int imageFormat, IProgressContext? progress = null)
        {
            if (IsImageLocked)
                throw new InvalidOperationException("Image cannot be transcoded to another format.");

            var paletteFormat = ImageInfo.PaletteFormat;
            if (!IsIndexed && IsIndexEncoding(imageFormat))
                paletteFormat = EncodingDefinition.GetIndexEncoding(imageFormat).PaletteEncodingIndices.First();

            TranscodeInternal(imageFormat, paletteFormat, true, progress);
        }

        /// <inheritdoc />
        public void SetIndexInImage(Point point, int paletteIndex)
        {
            if (!IsIndexed)
                throw new InvalidOperationException("Image is not indexed.");

            var image = DecodeImage();
            if (!IsPointInRegion(point, image.Size))
                throw new InvalidOperationException($"Point {point} is not in image.");

            var palette = DecodePalette();
            if (paletteIndex >= palette.Count)
                throw new InvalidOperationException($"Palette index {paletteIndex} is out of range.");

            image[point.X, point.Y] = palette[paletteIndex];

            _decodedImage = image;
            var (imageData, paletteData) = EncodeImage(image, ImageInfo.ImageFormat, ImageInfo.PaletteFormat);

            ImageInfo.ImageData = imageData.FirstOrDefault();
            ImageInfo.MipMapData = imageData.Skip(1).ToArray();
            ImageInfo.PaletteData = paletteData;

            ImageInfo.ContentChanged = true;
        }

        #endregion

        #region Palette methods

        /// <inheritdoc />
        public IList<Rgba32> GetPalette(IProgressContext? progress = null)
        {
            if (!IsIndexed)
                throw new InvalidOperationException("Image is not indexed.");

            return DecodePalette(progress);
        }

        /// <inheritdoc />
        public void SetPalette(IList<Rgba32> palette, IProgressContext? progress = null)
        {
            if (!IsIndexed)
                throw new InvalidOperationException("Image is not indexed.");

            // Check for locking
            if (IsImageLocked && GetPalette(progress).Count != palette.Count)
                throw new InvalidOperationException("Only palettes with the same amount of colors can be set.");

            _decodedImage = _bestImage = null;
            _decodedPalette = palette;

            ImageInfo.PaletteData = EncodePalette(palette, ImageInfo.PaletteFormat);

            ImageInfo.ContentChanged = true;
        }

        /// <inheritdoc />
        public void TranscodePalette(int paletteFormat, IProgressContext? progress = null)
        {
            if (!IsIndexed)
                throw new InvalidOperationException("Image is not indexed.");

            if (IsImageLocked)
                throw new InvalidOperationException("Palette cannot be transcoded to another format.");

            TranscodeInternal(ImageInfo.ImageFormat, paletteFormat, true, progress);
        }

        /// <inheritdoc />
        public void SetColorInPalette(int paletteIndex, Rgba32 color)
        {
            if (!IsIndexed)
                throw new InvalidOperationException("Image is not indexed.");

            var palette = DecodePalette();
            if (paletteIndex >= palette.Count)
                throw new InvalidOperationException($"Palette index {paletteIndex} is out of range.");

            palette[paletteIndex] = color;
            SetPalette(palette);
        }

        #endregion

        #region Decode methods

        private Image<Rgba32> DecodeImage(IProgressContext? progress = null)
        {
            if (_decodedImage != null)
                return _decodedImage;

            ExecuteActionWithProgress(() => _decodedImage = GetDecodedImage(progress), progress);

            _bestImage ??= _decodedImage;

            return _decodedImage;
        }

        /// <summary>
        /// Decode current palette from <see cref="ImageData"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Either buffered palette or decoded palette.</returns>
        private IList<Rgba32> DecodePalette(IProgressContext? context = null)
        {
            if (_decodedPalette != null)
                return _decodedPalette;

            return _decodedPalette = DecodePalette(ImageInfo.PaletteData, context);
        }

        /// <summary>
        /// Decode given palette data without buffering.
        /// </summary>
        /// <param name="paletteData">Palette data to decode.</param>
        /// <param name="context"></param>
        /// <returns>Decoded palette.</returns>
        private IList<Rgba32> DecodePalette(byte[] paletteData, IProgressContext? context = null)
        {
            var paletteEncoding = EncodingDefinition.GetPaletteEncoding(ImageInfo.PaletteFormat);
            return paletteEncoding
                .Load(paletteData, new EncodingOptions
                {
                    Size = new Size(1, paletteData.Length * 8 / paletteEncoding.BitsPerValue),
                    TaskCount = TaskCount
                })
                .ToArray();
        }

        #endregion

        #region Encode methods

        private (IList<byte[]> imageData, byte[] paletteData) EncodeImage(Image<Rgba32> image, int imageFormat, int paletteFormat = -1, IProgressContext? progress = null)
        {
            // Transcode image
            byte[] mainImageData = null;
            byte[] mainPaletteData = null;
            ExecuteActionWithProgress(() => (mainImageData, mainPaletteData) = GetEncodedImage(image, imageFormat, paletteFormat, progress), progress);

            var mipCount = ImageInfo.MipMapData?.Count ?? 0;
            var imageData = new byte[mipCount + 1][];
            imageData[0] = mainImageData;

            // Decode palette if present, only when mip maps are needed
            IList<Rgba32> decodedPalette = null;
            if (mainPaletteData != null && mipCount > 0)
                decodedPalette = DecodePalette(mainPaletteData);

            // Encode mip maps
            var (width, height) = (image.Width / 2, image.Height / 2);
            for (var i = 0; i < mipCount; i++)
            {
                imageData[i + 1] = EncodeMipMap(ResizeImage(image, width, height), imageFormat, decodedPalette);

                width /= 2;
                height /= 2;
            }

            return (imageData, mainPaletteData);
        }

        // TODO: Use progress
        private byte[] EncodeMipMap(Image<Rgba32> mipMap, int imageFormat, IList<Rgba32> palette = null)
        {
            return GetEncodedMipMap(mipMap, imageFormat, palette, null);
        }

        private byte[] EncodePalette(IList<Rgba32> palette, int paletteFormat)
        {
            return EncodingDefinition.GetPaletteEncoding(paletteFormat)
                .Save(palette, new EncodingOptions
                {
                    Size = new Size(1, palette.Count),
                    TaskCount = TaskCount
                });
        }

        #endregion

        private void TranscodeInternal(int imageFormat, int paletteFormat, bool checkFormatEquality, IProgressContext? progress = null)
        {
            AssertImageFormatExists(imageFormat);
            if (IsIndexEncoding(imageFormat))
                AssertPaletteFormatExists(paletteFormat);

            if (checkFormatEquality)
                if (ImageInfo.ImageFormat == imageFormat &&
                    IsIndexEncoding(imageFormat) && ImageInfo.PaletteFormat == paletteFormat)
                    return;

            // Decode image
            var decodedImage = _bestImage ?? DecodeImage(progress);

            // Update format information
            ImageInfo.ImageFormat = imageFormat;
            ImageInfo.PaletteFormat = IsIndexEncoding(imageFormat) ? paletteFormat : -1;

            // Encode image
            var (imageData, paletteData) = EncodeImage(decodedImage, imageFormat, paletteFormat, progress);

            // Set remaining image info properties
            ImageInfo.BitDepth = GetEncodingInfo(ImageInfo.ImageFormat).BitDepth;
            ImageInfo.ImageData = imageData.FirstOrDefault();
            ImageInfo.MipMapData = imageData.Skip(1).ToArray();
            ImageInfo.PaletteData = IsIndexEncoding(imageFormat) ? paletteData : null;

            ImageInfo.ImageSize = decodedImage.Size;

            _decodedImage = null;
            _decodedPalette = null;

            ImageInfo.ContentChanged = true;
        }

        private Image<Rgba32> ResizeImage(Image<Rgba32> image, int width, int height)
        {
            Image<Rgba32> cloned = image.Clone();
            cloned.Mutate(context => context.Resize(width, height));

            return cloned;
        }

        private void AssertImageFormatExists(int imageFormat)
        {
            if (EncodingDefinition.GetColorEncoding(imageFormat) == null &&
                EncodingDefinition.GetIndexEncoding(imageFormat) == null)
                throw new InvalidOperationException($"The image format '{imageFormat}' is not supported by the plugin.");
        }

        private void AssertPaletteFormatExists(int paletteFormat)
        {
            if (EncodingDefinition.GetPaletteEncoding(paletteFormat) == null)
                throw new InvalidOperationException($"The palette format '{paletteFormat}' is not supported by the plugin.");
        }

        protected bool IsIndexEncoding(int imageFormat)
        {
            return EncodingDefinition.GetIndexEncoding(imageFormat) != null;
        }

        private bool IsPointInRegion(Point point, Size region)
        {
            var rectangle = new Rectangle(Point.Empty, region);
            return rectangle.Contains(point);
        }

        private void ExecuteActionWithProgress(Action action, IProgressContext? progress = null)
        {
            var isRunning = progress?.IsRunning();
            if (isRunning.HasValue && !isRunning.Value)
                progress?.StartProgress();

            action();

            if (isRunning.HasValue && !isRunning.Value)
                progress?.FinishProgress();
        }

        public void Dispose()
        {
            _decodedImage?.Dispose();
            _bestImage?.Dispose();
        }
    }
}
