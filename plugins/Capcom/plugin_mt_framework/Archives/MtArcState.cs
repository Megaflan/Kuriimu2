﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kontract.Interfaces.FileSystem;
using Kontract.Interfaces.Plugins.State;
using Kontract.Interfaces.Plugins.State.Archive;
using Kontract.Models.Archive;
using Kontract.Models.Context;
using Kontract.Models.IO;

namespace plugin_mt_framework.Archives
{
    class MtArcState : IArchiveState, ILoadFiles, ISaveFiles, IReplaceFiles, IAddFiles, IRenameFiles
    {
        private MtArc _arc;

        public IList<IArchiveFileInfo> Files { get; private set; }
        public bool ContentChanged => IsContentChanged();

        public MtArcState()
        {
            _arc = new MtArc();
        }

        public async Task Load(IFileSystem fileSystem, UPath filePath, LoadContext loadContext)
        {
            var fileStream = await fileSystem.OpenFileAsync(filePath);

            var platform = MtArcSupport.DeterminePlatform(fileStream);
            Files = _arc.Load(fileStream, platform);
        }

        public Task Save(IFileSystem fileSystem, UPath savePath, SaveContext saveContext)
        {
            var fileStream = fileSystem.OpenFile(savePath, FileMode.Create, FileAccess.Write);
            _arc.Save(fileStream, Files);

            return Task.CompletedTask;
        }

        public void ReplaceFile(IArchiveFileInfo afi, Stream fileData)
        {
            afi.SetFileData(fileData);
        }

        public IArchiveFileInfo AddFile(Stream fileData, UPath filePath)
        {
            var afi = _arc.Add(fileData, filePath);
            Files.Add(afi);

            return afi;
        }

        public void Rename(IArchiveFileInfo afi, UPath path)
        {
            afi.FilePath = path;
        }

        private bool IsContentChanged()
        {
            return Files.Any(x => x.ContentChanged);
        }
    }
}
