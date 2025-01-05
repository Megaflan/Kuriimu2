using SixLabors.ImageSharp;

namespace Kanvas.Quantization.ColorDitherer.Ordered
{
    public class Bayer2Ditherer : OrderedDitherer
    {
        protected override byte[,] Matrix => new byte[,]
        {
            {1, 3},
            {4, 2}
        };

        public Bayer2Ditherer(Size imageSize, int taskCount) :
            base(imageSize, taskCount)
        {
        }
    }
}
