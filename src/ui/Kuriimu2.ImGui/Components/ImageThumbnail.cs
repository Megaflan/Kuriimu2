using System.Numerics;
using ImGui.Forms;
using ImGui.Forms.Controls.Base;
using ImGui.Forms.Extensions;
using ImGui.Forms.Models;
using ImGui.Forms.Resources;
using ImGuiNET;
using Konnect.Contract.Plugin.File.Image;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Rectangle = Veldrid.Rectangle;
using Size = ImGui.Forms.Models.Size;

namespace Kuriimu2.ImGui.Components
{
    internal class ImageThumbnail : Component
    {
        private readonly int _index;

        private ImageResource _thumbnail;

        public IImageFile ImageFile { get; }

        public Vector2 ThumbnailSize { get; } = new(90, 60);
        public bool ShowThumbnailBorder { get; set; } = true;

        public string Name => ImageFile.ImageInfo.Name ?? $"{_index:00}";

        public ImageThumbnail(IImageFile imageFile, int index, Image<Rgba32> image)
        {
            _index = index;

            ImageFile = imageFile;

            Image<Rgba32> thumbnail = image.Clone();
            thumbnail.Mutate(context => context.Resize(new SixLabors.ImageSharp.Size((int)ThumbnailSize.X, (int)ThumbnailSize.Y)));

            _thumbnail = ImageResource.FromImage(thumbnail);
        }

        public override Size GetSize() => new(SizeValue.Parent, SizeValue.Absolute((int)ThumbnailSize.Y));

        public void SetThumbnail(Image<Rgba32> image)
        {
            Image<Rgba32> thumbnail = image.Clone();
            thumbnail.Mutate(context => context.Resize(new SixLabors.ImageSharp.Size((int)ThumbnailSize.X, (int)ThumbnailSize.Y)));

            _thumbnail = ImageResource.FromImage(thumbnail);
        }

        protected override void UpdateInternal(Rectangle contentRect)
        {
            // Add thumbnail
            ImGuiNET.ImGui.GetWindowDrawList().AddImage((nint)_thumbnail, contentRect.Position, ThumbnailSize);

            // Add name
            if (Name != null)
            {
                var textPosition = contentRect.Position + ThumbnailSize with { Y = 0 };
                var textColor = Style.GetColor(ImGuiCol.Text).ToUInt32();
                ImGuiNET.ImGui.GetWindowDrawList().AddText(textPosition, textColor, Name);
            }
        }
    }
}
