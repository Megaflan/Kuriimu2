using Kanvas.Swizzle;
using Komponent.IO;
using Konnect.Contract.DataClasses.Plugin.File.Image;
using SixLabors.ImageSharp;

namespace plugin_level5.Switch.Image
{
    public class Nxtch
    {
        private const int HeaderSize_ = 44;

        private NxtchHeader _header;
        private byte[] _unkData;

        public ImageFileInfo Load(Stream input)
        {
            var typeReader = new BinaryTypeReader();
            using var br = new BinaryReaderX(input);

            // Read header
            _header = typeReader.Read<NxtchHeader>(br);

            // Read mip offsets
            var mipOffsets = typeReader.ReadMany<int>(br, _header.mipMapCount);

            // Read unknown data
            _unkData = br.ReadBytes(0x100 - (int)input.Position);

            input.Position = 0x74;
            var swizzleMode = br.ReadInt32();

            // Read image data
            var baseOffset = 0x100;

            input.Position = baseOffset + mipOffsets[0];
            var dataSize = mipOffsets.Count > 1 ? mipOffsets[1] - mipOffsets[0] : input.Length - baseOffset;
            var imageData = br.ReadBytes((int)dataSize);

            // Read mip data
            var mipData = new List<byte[]>();
            for (var i = 1; i < _header.mipMapCount; i++)
            {
                input.Position = mipOffsets[i];
                var mipSize = i + 1 >= _header.mipMapCount ? input.Length - baseOffset : mipOffsets[i + 1] - mipOffsets[i];
                mipData.Add(br.ReadBytes((int)mipSize));
            }

            // Create image info
            var imageInfo = new ImageFileInfo
            {
                BitDepth = imageData.Length * 8 / (_header.width * _header.height),
                ImageData = imageData,
                ImageFormat = _header.format,
                ImageSize = new Size(_header.width, _header.height),
                MipMapData = mipData,
                RemapPixels = context => new NxSwizzle(context, swizzleMode)
            };

            return imageInfo;
        }

        public void Save(Stream output, ImageFileInfo imageInfo)
        {
            var typeWriter = new BinaryTypeWriter();
            using var bw = new BinaryWriterX(output);

            var mipOffset = HeaderSize_;
            var dataOffset = 0x100;

            // Write image and mip data
            var mipOffsets = new List<int> { dataOffset };

            var dataPosition = dataOffset;
            output.Position = dataPosition;
            bw.Write(imageInfo.ImageData);
            dataPosition += imageInfo.ImageData.Length;

            if ((imageInfo.MipMapData?.Count ?? 0) > 0)
            {
                foreach (byte[] mipData in imageInfo.MipMapData)
                {
                    mipOffsets.Add(dataPosition);
                    bw.Write(mipData);
                    dataPosition += mipData.Length;
                }
            }

            // Write mip offsets
            output.Position = mipOffset;
            typeWriter.WriteMany(mipOffsets.Select(x => x - dataOffset), bw);

            // Write unknown data
            bw.Write(_unkData);

            // Write header
            _header.mipMapCount = imageInfo.MipMapData?.Count ?? 0;
            _header.format = imageInfo.ImageFormat;
            _header.width = imageInfo.ImageSize.Width;
            _header.height = imageInfo.ImageSize.Height;
            _header.textureDataSize = (int)(output.Length - dataOffset);
            _header.textureDataSize2 = (int)(output.Length - dataOffset);

            output.Position = 0;
            typeWriter.Write(_header, bw);
        }
    }
}
