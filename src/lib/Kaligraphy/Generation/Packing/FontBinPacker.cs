using Kaligraphy.Contract.DataClasses;
using Kaligraphy.Contract.DataClasses.Generation.Packing;
using SixLabors.ImageSharp;

namespace Kaligraphy.Generation.Packing
{
    public class FontBinPacker : BinPacker<GlyphData, PackedGylphData>
    {
        public FontBinPacker(Size canvasSize, int margin) : base(canvasSize, new Size(margin))
        {
        }

        protected override int CalculateVolume(GlyphData element)
        {
            return (element.Description.Size.Width + Margin.Width) *
                   (element.Description.Size.Height + Margin.Height);
        }

        protected override Size CalculateSize(GlyphData element)
        {
            return new Size(element.Description.Size.Width + Margin.Width,
                element.Description.Size.Height + Margin.Height);
        }

        protected override PackedGylphData CreatePackedElement(GlyphData element, Point position)
        {
            return new PackedGylphData
            {
                Element = element,
                Position = position + Margin
            };
        }
    }
}
