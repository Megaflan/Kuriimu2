using ImGui.Forms;
using ImGui.Forms.Models;
using Kuriimu2.ImGui.Forms;
using Kuriimu2.ImGui.Resources;
using ImGui.Forms.Factories;

var app = new Application(LocalizationResources.Instance);
var form = new MainForm();

FontFactory.RegisterFromResource("Roboto", "roboto.ttf", FontGlyphRange.Latin | FontGlyphRange.Cyrillic | FontGlyphRange.Greek);
FontFactory.RegisterFromResource("NotoJp", "notojp.ttf", FontGlyphRange.ChineseJapanese);
FontFactory.RegisterFromResource("NotoKr", "notokr.ttf", FontGlyphRange.Korean);
FontFactory.RegisterFromResource("NotoZhTc", "notozhtc.ttf", FontGlyphRange.ChineseJapanese);

form.DefaultFont = FontFactory.Get("Roboto", 15, FontFactory.Get("NotoJp", 15, FontFactory.Get("NotoKr", 15, FontFactory.Get("NotoZhTc", 15))));

app.Execute(form);