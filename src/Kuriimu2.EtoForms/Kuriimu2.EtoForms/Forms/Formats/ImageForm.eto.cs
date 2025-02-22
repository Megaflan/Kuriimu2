﻿using Eto.Drawing;
using Eto.Forms;
using Kuriimu2.EtoForms.Controls;
using Kuriimu2.EtoForms.Controls.ImageView;
using Kuriimu2.EtoForms.Forms.Models;
using Kuriimu2.EtoForms.Resources;
using Kuriimu2.EtoForms.Support;

namespace Kuriimu2.EtoForms.Forms.Formats
{
    partial class ImageForm : Panel
    {
        #region Localization Keys

        private const string SaveKey_ = "Save";
        private const string SaveAsKey_ = "SaveAs";

        private const string ExportImageKey_ = "ExportImage";
        private const string ImportImageKey_ = "ImportImage";

        private const string WidthKey_ = "Width";
        private const string HeightKey_ = "Height";
        private const string FormatKey_ = "Format";
        private const string PaletteKey_ = "Palette";

        #endregion

        #region Commands

        private Command saveCommand;
        private Command saveAsCommand;
        private Command exportCommand;
        private Command importCommand;

        #endregion

        #region Controls

        private ButtonToolStripItem saveButton;
        private ButtonToolStripItem saveAsButton;
        private ButtonToolStripItem exportButton;
        private ButtonToolStripItem importButton;

        private ImageViewEx imageView;
        private PaletteView imagePalette;

        private Label width;
        private Label height;
        private ComboBox formats;
        private ComboBox palettes;
        private ListBox imageList;

        #endregion

        private void InitializeComponent()
        {
            #region Commands

            saveCommand = new Command { Image = ImageResources.Actions.Save };
            saveAsCommand = new Command { Image = ImageResources.Actions.SaveAs };
            exportCommand = new Command { Image = ImageResources.Actions.ImageExport };
            importCommand = new Command { Image = ImageResources.Actions.ImageImport };

            #endregion

            #region Controls

            #region Buttons

            saveButton = new ButtonToolStripItem
            {
                ToolTip = Localize(SaveKey_),
                Command = saveCommand,
            };

            saveAsButton = new ButtonToolStripItem
            {
                ToolTip = Localize(SaveAsKey_),
                Command = saveAsCommand,
            };

            exportButton = new ButtonToolStripItem
            {
                ToolTip = Localize(ExportImageKey_),
                Command = exportCommand,
            };

            importButton = new ButtonToolStripItem
            {
                ToolTip = Localize(ImportImageKey_),
                Command = importCommand,
            };

            #endregion

            #region Default

            imageList = new ListBox
            {
                ItemImageBinding = Binding.Property<Image>(nameof(ImageElement.Thumbnail)),
                ItemTextBinding = Binding.Property<string>(nameof(ImageElement.Text)),
            };

            imageView = new ImageViewEx { BackgroundColor = Themer.Instance.GetTheme().ImageViewBackColor };
            imagePalette = new PaletteView { Size = new Size(200, -1), };

            var widthLabel = new Label { Text = Localize(WidthKey_) };
            var heightLabel = new Label { Text = Localize(HeightKey_) };
            var formatLabel = new Label { Text = Localize(FormatKey_) };
            var paletteLabel = new Label { Text = Localize(PaletteKey_) };

            width = new Label();
            height = new Label();
            formats = new ComboBox { ItemKeyBinding = Binding.Property<string>("ImageIdent") };
            palettes = new ComboBox();

            #endregion

            #region Toolstrip

            var mainToolStrip = new ToolStrip
            {
                Padding = 3,
                Items =
                {
                    saveButton,
                    saveAsButton,
                    new SplitterToolStripItem(),
                    exportButton,
                    importButton
                }
            };

            #endregion

            #region Layouts

            var imageLayout = new StackLayout
            {
                Spacing = 3,
                Orientation = Orientation.Vertical,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Items =
                {
                    new StackLayoutItem(imageView) { Expand = true, HorizontalAlignment = HorizontalAlignment.Stretch },
                    new TableLayout
                    {
                        Spacing = new Size(3, 3),
                        Rows =
                        {
                            new TableRow
                            {
                                Cells =
                                {
                                    widthLabel,
                                    width,
                                    heightLabel,
                                    height,
                                    new TableCell { ScaleWidth = true }
                                }
                            },
                            new TableRow
                            {
                                Cells =
                                {
                                    formatLabel,
                                    formats,
                                    paletteLabel,
                                    palettes,
                                    new TableCell { ScaleWidth = true }
                                }
                            }
                        }
                    }
                }
            };

            var listLayout = new StackLayout
            {
                Spacing = 3,
                VerticalContentAlignment = VerticalAlignment.Stretch,
                Orientation = Orientation.Vertical,
                Items =
                {
                    new StackLayoutItem(imageList, true) { HorizontalAlignment = HorizontalAlignment.Stretch },
                    new StackLayoutItem(imagePalette, true)
                }
            };

            var mainLayout = new TableLayout
            {
                Spacing = new Size(3, 3),
                Rows =
                {
                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(imageLayout) { ScaleWidth = true },
                            listLayout
                        }
                    }
                }
            };

            #endregion

            #endregion

            Content = new TableLayout
            {
                Rows =
                {
                    new TableRow(mainToolStrip),
                    mainLayout
                }
            };
        }
    }
}
