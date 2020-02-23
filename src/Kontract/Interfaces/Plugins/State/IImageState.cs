﻿using System.Collections.Generic;
using Kontract.Kanvas;
using Kontract.Models.Images;

namespace Kontract.Interfaces.Plugins.State
{
    public interface IImageState
    {
        IList<ImageInfo> Images { get; }

        IDictionary<int, IColorEncoding> SupportedEncodings { get; }

        IDictionary<int, IColorIndexEncoding> SupportedIndexEncodings { get; }

        IDictionary<int, IColorEncoding> SupportedPaletteEncodings { get; }
    }
}
