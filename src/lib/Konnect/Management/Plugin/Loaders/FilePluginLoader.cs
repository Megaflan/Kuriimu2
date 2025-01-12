using Konnect.Contract.DataClasses.Management.Plugin.Loaders;
using Konnect.Contract.Management.Plugin.Loaders;
using Konnect.Contract.Plugin.File;
using System.Reflection;

namespace Konnect.Management.Plugin.Loaders
{
    public class FilePluginLoader : PluginLoader, IPluginLoader<IFilePlugin>
    {
        /// <inheritdoc />
        public override IReadOnlyList<PluginLoadError> LoadErrors { get; }

        /// <inheritdoc />
        public IReadOnlyList<IFilePlugin> Plugins { get; }

        public FilePluginLoader(params string[] pluginPaths)
        {
            if (!TryLoadPlugins<IFilePlugin>(pluginPaths, out var plugins, out var errors))
            {
                errors = new List<PluginLoadError>();
                plugins = new List<IFilePlugin>();
            }

            LoadErrors = errors;
            Plugins = plugins;
        }

        public FilePluginLoader(params Assembly[] pluginAssemblies)
        {
            if (!TryLoadPlugins<IFilePlugin>(pluginAssemblies, out var plugins, out var errors))
            {
                errors = new List<PluginLoadError>();
                plugins = new List<IFilePlugin>();
            }

            LoadErrors = errors;
            Plugins = plugins;
        }

        /// <inheritdoc />
        public override bool Exists(Guid pluginId)
        {
            return Plugins.Any(p => p.PluginId == pluginId);
        }

        /// <inheritdoc />
        public IFilePlugin? GetPlugin(Guid pluginId)
        {
            return Plugins.FirstOrDefault(ep => ep.PluginId == pluginId);
        }
    }
}
