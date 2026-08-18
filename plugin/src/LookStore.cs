using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace GPoseStudio;

public static class LookStore
{
    internal static readonly HashSet<string> Exclude = new()
    {
        "Version", "OutputDirectory", "LivePreview", "DebugShowDepth",
        "DebugShowGate", "Bypass", "SwapRedBlue", "FlipVertical",
        "ShowGuides", "GuideThirds", "GuideGolden", "GuideCenter", "GuideHorizon", "GuideHorizonY", "GuideOpacity",
        "ExportAspect", "ShowExportFrame", "ExportScale", "ExportFormat", "ExportJpegQuality",
        "Pinned",
    };

    public static string FolderPath
    {
        get
        {
            var d = Path.Combine(Services.PluginInterface.GetPluginConfigDirectory(), "looks");
            Directory.CreateDirectory(d);
            return d;
        }
    }

    public static List<string> List()
    {
        try
        {
            return Directory.GetFiles(FolderPath, "*.json")
                .Select(Path.GetFileNameWithoutExtension)
                .Where(n => !string.IsNullOrEmpty(n))
                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                .ToList()!;
        }
        catch { return new List<string>(); }
    }

    public static void Save(string name, PluginConfig cfg)
    {
        var dict = new Dictionary<string, object?>();
        foreach (var p in typeof(PluginConfig).GetProperties())
            if (p.CanRead && p.CanWrite && !Exclude.Contains(p.Name))
                dict[p.Name] = p.GetValue(cfg);

        var json = JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(FolderPath, Sanitize(name) + ".json"), json);
    }

    public static bool Load(string name, PluginConfig cfg)
    {
        var path = Path.Combine(FolderPath, Sanitize(name) + ".json");
        if (!File.Exists(path)) return false;

        Dictionary<string, JsonElement>? dict;
        try { dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(File.ReadAllText(path)); }
        catch { return false; }
        if (dict == null) return false;

        foreach (var p in typeof(PluginConfig).GetProperties())
        {
            if (!p.CanWrite || Exclude.Contains(p.Name)) continue;
            if (!dict.TryGetValue(p.Name, out var el)) continue;
            try
            {
                if (p.PropertyType == typeof(float)) p.SetValue(cfg, el.GetSingle());
                else if (p.PropertyType == typeof(int)) p.SetValue(cfg, el.GetInt32());
                else if (p.PropertyType == typeof(bool)) p.SetValue(cfg, el.GetBoolean());
                else if (p.PropertyType == typeof(float[]))
                {
                    var arr = el.Deserialize<float[]>();
                    if (arr != null)
                    {
                        int want = (p.GetValue(cfg) as float[])?.Length ?? arr.Length;
                        if (arr.Length == want) p.SetValue(cfg, arr);
                        else if (arr.Length == 180 && want == 224)
                        {
                            var up = new float[224];
                            for (int f = 0; f < 2; f++)
                                for (int k = 0; k < 89; k++) up[f * 111 + k] = arr[f * 89 + k];
                            p.SetValue(cfg, up);
                        }
                        else if (arr.Length == 128 && want == 160)
                        {
                            var up = new float[160];
                            for (int L = 0; L < 8; L++) for (int k = 0; k < 16; k++) up[L * 20 + k] = arr[L * 16 + k];
                            p.SetValue(cfg, up);
                        }
                    }
                }
                else if (p.PropertyType == typeof(string[]))
                {
                    var arr = el.Deserialize<string[]>();
                    if (arr != null && arr.Length == 8) p.SetValue(cfg, arr);
                }
                else if (p.PropertyType == typeof(List<TextMarker>))
                    p.SetValue(cfg, el.Deserialize<List<TextMarker>>() ?? new List<TextMarker>());
            }
            catch {  }
        }

        if (!dict.ContainsKey("BgBPatColOverride")) cfg.CarryPatternIdentity();
        return true;
    }

    public static void Delete(string name)
    {
        try { File.Delete(Path.Combine(FolderPath, Sanitize(name) + ".json")); }
        catch {  }
    }

    public static readonly (string Name, string Cat, Action<PluginConfig> Apply)[] Builtins =
    {
        ("Cosmic Nebula", "Space", c => c.ApplyCosmicPreset()),
        ("Astral Void", "Space", c => c.ApplyVoidPreset()),
        ("Dead Channel", "Stylised", c => c.ApplyHorrorPreset()),
        ("Hellfire", "Elements", c => c.ApplyHellfirePreset()),
        ("Aquarium", "Elements", c => c.ApplyAquariumPreset()),
        ("Aurora Borealis", "Sky", c => c.ApplyAuroraPreset()),
        ("Synthwave", "Stylised", c => c.ApplySynthwavePreset()),
        ("Blood Moon", "Sky", c => c.ApplyBloodMoonPreset()),
        ("Tempe — The Red That Follows", "Sky", c => c.ApplyTempeMoonPreset()),
        ("Forge", "Scene", c => c.ApplyForgePreset()),
        ("Artisan's Rest", "Scene", c => c.ApplyArtisanPreset()),
        ("Sunset", "Sky", c => c.ApplySunsetPreset()),
        ("Sin Eater", "Fantasy", c => c.ApplySinEaterPreset()),
        ("Magitek HUD", "Overlay", c => c.ApplyMagitekHudPreset()),
        ("Gpose Viewfinder", "Overlay", c => c.ApplyGposeViewfinderPreset()),
        ("AoE Telegraph", "Overlay", c => c.ApplyAoeTelegraphPreset()),
        ("Evercold", "Elements", c => c.ApplyEvercoldPreset()),
        ("Studio Portrait", "Portrait", c => c.ApplyStudioPortraitPreset()),
        ("On Location", "Portrait", c => c.ApplyOnLocationPreset()),
        ("Duality", "Portrait", c => c.ApplyDualityPreset()),
        ("Aetherbloom", "Fantasy", c => c.ApplyAetherbloomPreset()),
        ("Tempest", "Weather", c => c.ApplyTempestPreset()),
        ("Hoarfrost", "Weather", c => c.ApplyHoarfrostPreset()),
        ("Emberfall", "Weather", c => c.ApplyEmberfallPreset()),
        ("Starlit Vow", "Fantasy", c => c.ApplyStarlitVowPreset()),
        ("Chinese Ink", "Stylised", c => c.ApplyChineseInkPreset()),
    };

    public static bool Exists(string name) =>
        File.Exists(Path.Combine(FolderPath, Sanitize(name) + ".json"));

    private const int BuiltinsVersion = 59;
    private static string MarkerPath => Path.Combine(FolderPath, ".builtins");

    public static void SeedBuiltins()
    {
        int have = 0;
        try { if (File.Exists(MarkerPath)) int.TryParse(File.ReadAllText(MarkerPath).Trim(), out have); }
        catch {  }
        bool refresh = have < BuiltinsVersion;

        foreach (var (name, _, apply) in Builtins)
        {
            if (!refresh && Exists(name)) continue;
            var tmp = new PluginConfig();
            apply(tmp);
            tmp.CarryPatternIdentity();
            Save(name, tmp);
        }
        if (refresh) { try { File.WriteAllText(MarkerPath, BuiltinsVersion.ToString()); } catch { } }
    }

    public static void RegenerateBuiltin(string name)
    {
        foreach (var (n, _, apply) in Builtins)
            if (n == name) { var tmp = new PluginConfig(); apply(tmp); tmp.CarryPatternIdentity(); Save(name, tmp); return; }
    }

    private static string Sanitize(string n) =>
        string.Concat(n.Trim().Split(Path.GetInvalidFileNameChars()));
}
