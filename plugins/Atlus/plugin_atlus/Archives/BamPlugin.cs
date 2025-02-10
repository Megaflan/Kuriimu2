﻿using System;
using System.Threading.Tasks;
using Komponent.IO;
using Kontract.Interfaces.FileSystem;
using Kontract.Interfaces.Managers;
using Kontract.Interfaces.Plugins.Identifier;
using Kontract.Interfaces.Plugins.State;
using Kontract.Models;
using Kontract.Models.Context;
using Kontract.Models.IO;

namespace plugin_atlus.Archives
{
    public class BamPlugin : IFilePlugin, IIdentifyFiles
    {
        public Guid PluginId => Guid.Parse("f0fac450-6567-4bf6-a820-d9c32134062d");
        public PluginType PluginType => PluginType.Archive;
        public string[] FileExtensions => new[] { "*.bam" };
        public PluginMetadata Metadata { get; }

        public BamPlugin()
        {
            Metadata = new PluginMetadata("BAM", "onepiecefreak", "The BAM resource container for Atlus games on 3DS.");
        }

        public async Task<bool> IdentifyAsync(IFileSystem fileSystem, UPath filePath, IdentifyContext identifyContext)
        {
            var fileStream = await fileSystem.OpenFileAsync(filePath);
            using var br = new BinaryReaderX(fileStream);

            return br.ReadString(4) == "ATBC";
        }

        public IPluginState CreatePluginState(IBaseFileManager pluginManager)
        {
            return new BamState();
        }
    }
}
