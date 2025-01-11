using ImGui.Forms.Localization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Kuriimu2.ImGui.Resources
{
    internal class Localizer : BaseLocalizer
    {
        private const string NameSpace_ = "Kuriimu2.ImGui.Resources.Localizations.";

        protected override string DefaultLocale => "en";
        protected override string UndefinedValue => "<undefined>";

        public Localizer()
        {
            Initialize();
        }

        protected override IList<LanguageInfo> InitializeLocalizations()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var localNames = assembly.GetManifestResourceNames().Where(n => n.StartsWith(NameSpace_));

            var jsonOptions = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };

            var result = new List<LanguageInfo>();
            foreach (string localName in localNames)
            {
                var locStream = assembly.GetManifestResourceStream(localName);
                if (locStream == null)
                    continue;

                // Read text from stream
                var reader = new StreamReader(locStream, Encoding.UTF8);
                var json = reader.ReadToEnd();

                // Deserialize JSON
                var translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json, jsonOptions);
                if (!translations.TryGetValue("Name", out string localeName))
                    continue;

                var languageInfo = new LanguageInfo(GetLocale(localName), localeName, translations);

                result.Add(languageInfo);
            }

            return result;
        }

        protected override string InitializeLocale()
        {
            return SettingsResources.Locale;
        }

        private string GetLocale(string resourceName)
        {
            return resourceName.Replace(NameSpace_, string.Empty).Replace(".json", string.Empty);
        }
    }
}
