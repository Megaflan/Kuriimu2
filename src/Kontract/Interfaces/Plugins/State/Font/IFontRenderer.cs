﻿using Kontract.Models.Font;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Kontract.Interfaces.Plugins.State.Font
{
    /// <inheritdoc />
    /// <summary>
    /// This is the font renderer interface for creating font rendering plugins.
    /// </summary>
    public interface IFontRenderer : IFontState
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        CharacterInfo GetCharWidthInfo(char c);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="stopChar"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        float MeasureString(string text, char stopChar, float scale = 1.0f);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        void SetColor(Color color);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="image"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        void Draw(char c, Image<Rgba32> image, float x, float y, float scaleX, float scaleY);
    }
}
