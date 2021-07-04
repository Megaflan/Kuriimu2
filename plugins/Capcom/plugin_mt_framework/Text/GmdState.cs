﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kontract.Interfaces.FileSystem;
using Kontract.Interfaces.Plugins.State;
using Kontract.Models.Context;
using Kontract.Models.IO;
using Kontract.Models.Text;

namespace plugin_mt_framework.Text
{
    class GmdState : ITextState, ILoadFiles
    {
        private Gmd _txt;

        public IList<TextEntry> Texts { get; private set; }
        public bool ContentChanged { get; }

        public GmdState()
        {
            _txt = new Gmd();
        }

        public async Task Load(IFileSystem fileSystem, UPath filePath, LoadContext loadContext)
        {
            var fileStream = await fileSystem.OpenFileAsync(filePath);
            Texts = _txt.Load(fileStream);
        }

        public Task Save(IFileSystem fileSystem, UPath savePath, SaveContext saveContext)
        {
            throw new NotImplementedException();
        }
    }
}
