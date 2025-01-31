﻿using System;
using System.IO;
using System.Threading.Tasks;
using ImGui.Forms.Controls;
using ImGui.Forms.Controls.Base;
using ImGui.Forms.Controls.Lists;
using ImGui.Forms.Modals;
using ImGui.Forms.Modals.IO;
using Konnect.Contract.Plugin.File.Image;
using Konnect.Extensions;
using Kuriimu2.ImGui.Components;
using Kuriimu2.ImGui.Interfaces;
using Kuriimu2.ImGui.Models;
using Kuriimu2.ImGui.Resources;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Rectangle = Veldrid.Rectangle;
using Size = ImGui.Forms.Models.Size;

namespace Kuriimu2.ImGui.Forms.Formats
{
    partial class ImageForm : Component, IKuriimuForm
    {
        private readonly FormInfo<IImageFilePluginState> _state;

        private int _selectedImgIndex;
        private readonly AsyncOperation _asyncOperation;

        public ImageForm(FormInfo<IImageFilePluginState> state)
        {
            _state = state;
            _asyncOperation = new AsyncOperation();

            InitializeComponent();

            _formatBox.SelectedItemChanged += _formatBox_SelectedItemChanged;
            _paletteBox.SelectedItemChanged += _paletteBox_SelectedItemChanged;
            _imgList.SelectedItemChanged += _imgList_SelectedItemChanged;
            _saveBtn.Clicked += _saveBtn_Clicked;
            _saveAsBtn.Clicked += _saveAsBtn_Clicked;
            _imgExportBtn.Clicked += _imgExportBtn_Clicked;
            _imgImportBtn.Clicked += _imgImportBtn_Clicked;

            UpdateState();
            UpdateFormInternal();
        }

        #region Events

        private void _formatBox_SelectedItemChanged(object sender, EventArgs e)
        {
            var selectedFormat = ((ComboBox<int>)sender).SelectedItem.Content;

            var selectedImg = GetSelectedImage();
            if (selectedImg?.ImageInfo.ImageFormat == selectedFormat)
                return;

            selectedImg?.TranscodeImage(selectedFormat, _state.Progress);
            SetImage(selectedImg, _state.Progress);

            _state.FormCommunicator.Update(true, false);
            UpdateFormInternal();
        }

        private void _paletteBox_SelectedItemChanged(object sender, EventArgs e)
        {
            var selectedFormat = ((ComboBox<int>)sender).SelectedItem.Content;

            var selectedImg = GetSelectedImage();
            if (selectedImg?.ImageInfo.PaletteFormat == selectedFormat)
                return;

            selectedImg?.TranscodePalette(selectedFormat, _state.Progress);
            SetImage(selectedImg, _state.Progress);

            _state.FormCommunicator.Update(true, false);
            UpdateFormInternal();
        }

        private void _imgList_SelectedItemChanged(object sender, EventArgs e)
        {
            var imgList = (List<ImageThumbnail>)sender;
            var selectedItem = imgList.SelectedItem;

            _selectedImgIndex = imgList.Items.IndexOf(selectedItem);

            SetSelectedImage(selectedItem.ImageFile, _state.Progress);
        }

        private async void _saveBtn_Clicked(object sender, EventArgs e)
        {
            await Save(true);
        }

        private async void _saveAsBtn_Clicked(object sender, EventArgs e)
        {
            await Save(true);
        }

        private async void _imgExportBtn_Clicked(object sender, EventArgs e)
        {
            await Export();
        }

        private async void _imgImportBtn_Clicked(object sender, EventArgs e)
        {
            await Import();
        }

        private async Task Save(bool saveAs)
        {
            await _state.FormCommunicator.Save(saveAs);

            UpdateState();
            UpdateFormInternal();
        }

        private async Task Export()
        {
            DisableForm();

            var selectedItem = GetSelectedImageItem();

            _state.FormCommunicator.ReportStatus(StatusKind.Info, LocalizationResources.ImageStatusExportStart(selectedItem.Name));

            // Select file to save at
            var initialPath = Path.Combine(GetLastDirectory(), GetImageName(selectedItem) + ".png");
            var sfd = new SaveFileDialog(initialPath)
            {
                Caption = LocalizationResources.ImageMenuExportPng
            };

            if (await sfd.ShowAsync() != DialogResult.Ok)
            {
                _state.FormCommunicator.ReportStatus(StatusKind.Failure, LocalizationResources.ImageStatusExportCancel);

                UpdateFormInternal();
                return;
            }

            // Save selected path
            SettingsResources.LastDirectory = Path.GetDirectoryName(sfd.SelectedPath);

            // Export image
            try
            {
                await _asyncOperation.StartAsync(cts =>
                    Task.Run(() => selectedItem.ImageFile.GetImage(_state.Progress).SaveAsPng(sfd.SelectedPath), cts.Token));
            }
            catch (Exception ex)
            {
                _state.Logger.Fatal(ex, string.Empty);
                _state.FormCommunicator.ReportStatus(StatusKind.Failure, LocalizationResources.ImageStatusExportFailure);

                UpdateFormInternal();
                return;
            }

            UpdateFormInternal();

            _state.FormCommunicator.ReportStatus(StatusKind.Success, LocalizationResources.ImageStatusExportSuccess);
        }

        private async Task Import()
        {
            DisableForm();

            var selectedItem = GetSelectedImageItem();

            _state.FormCommunicator.ReportStatus(StatusKind.Info, LocalizationResources.ImageStatusImportStart(selectedItem.Name));

            // Select file to import from
            var ofd = new OpenFileDialog
            {
                Caption = LocalizationResources.ImageMenuImportPng,
                InitialDirectory = GetLastDirectory(),
                FileFilters = { new FileFilter(LocalizationResources.FilterPng, "*.png") }
            };

            if (await ofd.ShowAsync() != DialogResult.Ok)
            {
                _state.FormCommunicator.ReportStatus(StatusKind.Failure, LocalizationResources.ImageStatusImportCancel);

                UpdateFormInternal();
                return;
            }

            // Save selected path
            SettingsResources.LastDirectory = Path.GetDirectoryName(ofd.SelectedPath);

            // Import image
            try
            {
                var newImage = Image.Load<Rgba32>(ofd.SelectedPath);
                await _asyncOperation.StartAsync(cts => Task.Run(() => selectedItem.ImageFile.SetImage(newImage, _state.Progress), cts.Token));
            }
            catch (Exception ex)
            {
                _state.Logger.Fatal(ex, string.Empty);
                _state.FormCommunicator.ReportStatus(StatusKind.Failure, LocalizationResources.ImageStatusImportFailure);

                UpdateFormInternal();
                return;
            }

            // Update form
            UpdateFormInternal();

            SetImage(selectedItem.ImageFile, _state.Progress);

            _state.FormCommunicator.Update(true, false);
            _state.FormCommunicator.ReportStatus(StatusKind.Success, LocalizationResources.ImageStatusImportSuccess);
        }

        #region Support

        private string GetImageName(ImageThumbnail item)
        {
            return string.IsNullOrEmpty(item.ImageFile.ImageInfo.Name) ?
                _state.FileState.FilePath.GetNameWithoutExtension() + "." + item.Name :
                item.ImageFile.ImageInfo.Name;
        }

        private string GetLastDirectory()
        {
            var settingsDir = SettingsResources.LastDirectory;
            return string.IsNullOrEmpty(settingsDir) ? Path.GetFullPath(".") : settingsDir;
        }

        private IImageFile GetSelectedImage()
        {
            return GetSelectedImageItem().ImageFile;
        }

        private ImageThumbnail GetSelectedImageItem()
        {
            var clampedIndex = Math.Clamp(_selectedImgIndex, 0, _imgList.Items.Count - 1);
            return _imgList.Items[clampedIndex];
        }

        #endregion

        private void UpdateState()
        {
            SetImages(_state.PluginState.Images, _state.Progress);
            SetSelectedImage(GetSelectedImage(), _state.Progress);
        }

        #endregion

        #region Update methods

        private void DisableForm()
        {
            _imgExportBtn.Enabled = false;
            _imgImportBtn.Enabled = false;
            _batchImgExportBtn.Enabled = false;
            _batchImgImportBtn.Enabled = false;
        }

        private void UpdateFormInternal()
        {
            _imgExportBtn.Enabled = !_asyncOperation.IsRunning;
            _imgImportBtn.Enabled = !_asyncOperation.IsRunning;
            _batchImgExportBtn.Enabled = !_asyncOperation.IsRunning;
            _batchImgImportBtn.Enabled = !_asyncOperation.IsRunning;

            // Update save button enablement
            var canSave = _state.FileState.PluginState.CanSave;

            _saveBtn.Enabled = canSave && _state.FileState.StateChanged;
            _saveAsBtn.Enabled = canSave && _state.FileState.StateChanged && _state.FileState.ParentFileState == null;
        }

        #endregion

        #region Component implementation

        public override Size GetSize()
        {
            return Size.Parent;
        }

        protected override void UpdateInternal(Rectangle contentRect)
        {
            _mainLayout.Update(contentRect);
        }

        #endregion

        #region IKuriimuForm implementation

        public void UpdateForm()
        {
            UpdateFormInternal();
        }

        public bool HasRunningOperations()
        {
            return _asyncOperation.IsRunning;
        }

        public void CancelOperations()
        {
            _asyncOperation.Cancel();
        }

        #endregion
    }
}
