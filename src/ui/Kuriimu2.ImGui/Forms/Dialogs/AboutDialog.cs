﻿using System;
using System.Text.Json;
using ImGui.Forms;
using ImGui.Forms.Controls;
using ImGui.Forms.Controls.Layouts;
using ImGui.Forms.Localization;
using ImGui.Forms.Models;
using ImGui.Forms.Modals;
using Kuriimu2.ImGui.Models;
using Kuriimu2.ImGui.Resources;

namespace Kuriimu2.ImGui.Forms.Dialogs
{
    public class AboutDialog : Modal
    {
        private Label _titleLabel;
        private Label _versionLabel;
        private Label _descriptionLabel;

        public AboutDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            var width = (int)Math.Ceiling(Application.Instance.MainForm.Width * .3f);
            var height = (int)Math.Ceiling(Application.Instance.MainForm.Height * .3f);
            Size = new Size(width, height);

            _titleLabel = new Label { Text = "Kuriimu2" };
            _versionLabel = new Label { Text = GetVersionText() };
            _descriptionLabel = new Label { Text = LocalizationResources.MenuAboutDescription };
            var mainLayout = new StackLayout
            {
                Size = new Size(width, height),
                Alignment = Alignment.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                ItemSpacing = 10,
                Items =
                {
                    new StackItem(_titleLabel) {VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center },
                    new StackItem(_versionLabel) {VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center },
                    new StackItem(_descriptionLabel) {VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center },
                },
            };

            Caption = LocalizationResources.MenuAboutTitle;
            Content = mainLayout;
        }

        private LocalizedString GetVersionText()
        {
            string manifest = BinaryResources.VersionManifest;
            var manifestObject = JsonSerializer.Deserialize<Manifest>(manifest);

            return LocalizationResources.MenuAboutVersion(manifestObject?.Version);
        }
    }
}