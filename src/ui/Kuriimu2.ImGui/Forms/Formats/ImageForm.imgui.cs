﻿using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ImGui.Forms.Controls;
using ImGui.Forms.Controls.Layouts;
using ImGui.Forms.Models;
using ImGui.Forms.Resources;
using Konnect.Contract.Plugin.File.Image;
using Konnect.Contract.Progress;
using Kuriimu2.ImGui.Components;
using Kuriimu2.ImGui.Resources;
using ImageResources = Kuriimu2.ImGui.Resources.ImageResources;
using Size = ImGui.Forms.Models.Size;

namespace Kuriimu2.ImGui.Forms.Formats
{
    partial class ImageForm
    {
        private StackLayout _mainLayout;

        private ImageButton _saveBtn;
        private ImageButton _saveAsBtn;
        private ImageButton _imgExportBtn;
        private ImageButton _imgImportBtn;
        private ImageButton _batchImgExportBtn;
        private ImageButton _batchImgImportBtn;

        private Label _widthTextLbl;
        private Label _heightTextLbl;
        private Label _widthContentLbl;
        private Label _heightContentLbl;

        private Label _formatTextLbl;
        private Label _paletteTextLbl;
        private ComboBox<int> _formatBox;
        private ComboBox<int> _paletteBox;

        private ZoomablePictureBox _imageBox;

        private global::ImGui.Forms.Controls.Lists.List<ImageThumbnail> _imgList;

        private void InitializeComponent()
        {
            #region Controls

            _widthTextLbl = new Label(LocalizationResources.ImageLabelWidth);
            _heightTextLbl = new Label(LocalizationResources.ImageLabelHeight);
            _widthContentLbl = new Label();
            _heightContentLbl = new Label();

            _formatTextLbl = new Label(LocalizationResources.ImageLabelFormat);
            _paletteTextLbl = new Label(LocalizationResources.ImageLabelPalette);
            _formatBox = new ComboBox<int>();
            _paletteBox = new ComboBox<int>();

            _imageBox = new ZoomablePictureBox { ShowBorder = true };

            _imgList = new global::ImGui.Forms.Controls.Lists.List<ImageThumbnail>
            {
                ItemSpacing = 4
            };

            _saveBtn = new ImageButton { Image = ImageResources.Save, ImageSize = new Vector2(16, 16), Padding = new Vector2(5, 5), Enabled = false };
            _saveAsBtn = new ImageButton { Image = ImageResources.SaveAs, ImageSize = new Vector2(16, 16), Padding = new Vector2(5, 5), Enabled = false };
            _imgExportBtn = new ImageButton { Image = ImageResources.ImageExport, ImageSize = new Vector2(16, 16), Padding = new Vector2(5, 5) };
            _imgImportBtn = new ImageButton { Image = ImageResources.ImageImport, ImageSize = new Vector2(16, 16), Padding = new Vector2(5, 5) };
            _batchImgExportBtn = new ImageButton { Image = ImageResources.BatchImageExport, ImageSize = new Vector2(16, 16), Padding = new Vector2(5, 5) };
            _batchImgImportBtn = new ImageButton { Image = ImageResources.BatchImageImport, ImageSize = new Vector2(16, 16), Padding = new Vector2(5, 5) };

            #endregion

            _mainLayout = new StackLayout
            {
                Alignment = Alignment.Horizontal,
                ItemSpacing = 4,
                Items =
                {
                    new StackLayout
                    {
                        Alignment = Alignment.Vertical,
                        ItemSpacing = 4,
                        Size = Size.Parent,
                        Items =
                        {
                            new StackLayout
                            {
                                Alignment = Alignment.Horizontal,
                                ItemSpacing = 4,
                                Size = Size.WidthAlign,
                                Items =
                                {
                                    _saveBtn,
                                    _saveAsBtn,
                                    new Splitter{Length = 26},
                                    _imgExportBtn,
                                    _imgImportBtn,
                                    new Splitter{Length = 26},
                                    _batchImgExportBtn,
                                    _batchImgImportBtn
                                }
                            },
                            _imageBox,
                            new TableLayout
                            {
                                Spacing = new Vector2(4,4),
                                Size = Size.WidthAlign,
                                Rows =
                                {
                                    new TableRow
                                    {
                                        Cells =
                                        {
                                            _widthTextLbl,
                                            _widthContentLbl,
                                            _heightTextLbl,
                                            _heightContentLbl
                                        }
                                    },
                                    new TableRow
                                    {
                                        Cells =
                                        {
                                            _formatTextLbl,
                                            _formatBox,
                                            _paletteTextLbl,
                                            _paletteBox
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new StackLayout
                    {
                        Alignment = Alignment.Vertical,
                        ItemSpacing = 4,
                        Size = new Size(300,SizeValue.Parent),
                        Items =
                        {
                            _imgList
                        }
                    }
                }
            };
        }

        private void SetImages(IReadOnlyList<IImageFile> images, IProgressContext progress)
        {
            _imgList.Items.Clear();
            _imgList.SelectedItem = null;

            if (images == null || images.Count <= 0)
                return;

            var perPart = 100f / images.Count;
            var perStart = 0f;

            for (var i = 0; i < images.Count; i++)
            {
                var img = images[i];
                var scopeProgress = progress.CreateScope(LocalizationResources.ImageProgressDecode, perStart, perStart + perPart);

                _imgList.Items.Add(new ImageThumbnail(img, i, img.GetImage(scopeProgress)));

                perStart += perPart;
            }
        }

        private void SetSelectedImage(IImageFile img, IProgressContext progress)
        {
            SetFormats(img);
            SetPaletteFormats(img);

            _imgList.SelectedItem = _imgList.Items.FirstOrDefault(x => x.ImageFile == img);
            SetImage(img, progress);

            _widthContentLbl.Text = img.ImageInfo.ImageSize.Width.ToString();
            _heightContentLbl.Text = img.ImageInfo.ImageSize.Height.ToString();
        }

        private void SetImage(IImageFile img, IProgressContext progress)
        {
            var image = img.GetImage(progress);

            _imageBox.Image = ImageResource.FromImage(image);
            _imgList.SelectedItem.SetThumbnail(image);
        }

        private void SetFormats(IImageFile img)
        {
            _formatBox.Items.Clear();
            _formatBox.SelectedItem = null;

            if (img == null)
                return;

            var hasFormats = img.EncodingDefinition.ColorEncodings.Any() || img.EncodingDefinition.IndexEncodings.Any();
            _formatBox.Visible = _formatTextLbl.Visible = hasFormats;

            if (!hasFormats)
                return;

            if (img.EncodingDefinition.ColorEncodings.Any())
                foreach (var colorEnc in img.EncodingDefinition.ColorEncodings)
                    _formatBox.Items.Add(new DropDownItem<int>(colorEnc.Key, colorEnc.Value.FormatName));

            if (img.EncodingDefinition.IndexEncodings.Any())
                foreach (var indexEnc in img.EncodingDefinition.IndexEncodings)
                    _formatBox.Items.Add(new DropDownItem<int>(indexEnc.Key, indexEnc.Value.IndexEncoding.FormatName));

            _formatBox.SelectedItem = _formatBox.Items.FirstOrDefault(x => x.Content == img.ImageInfo.ImageFormat);
        }

        private void SetPaletteFormats(IImageFile img)
        {
            _paletteBox.Items.Clear();
            _paletteBox.SelectedItem = null;

            if (img == null)
                return;

            var hasPalettes = img.EncodingDefinition.PaletteEncodings.Any();
            _paletteBox.Visible = _paletteTextLbl.Visible = hasPalettes;

            if (!hasPalettes)
                return;

            if (img.EncodingDefinition.PaletteEncodings.Any())
                foreach (var paletteEnc in img.EncodingDefinition.PaletteEncodings)
                    _paletteBox.Items.Add(new DropDownItem<int>(paletteEnc.Key, paletteEnc.Value.FormatName));

            _paletteBox.SelectedItem = _paletteBox.Items.FirstOrDefault(x => x.Content == img.ImageInfo.PaletteFormat);
        }
    }
}
