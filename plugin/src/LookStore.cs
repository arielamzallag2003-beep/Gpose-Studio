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

    public static readonly (string Name, string Desc, Action<PluginConfig> Apply)[] Builtins =
    {
        ("Cosmic Nebula", "Fractal nebula, starfield, core glow. Background only.", c => c.ApplyCosmicPreset()),
        ("Astral Void", "Haunting astral void — a full scene (also sets grade + rim).", c => c.ApplyVoidPreset()),
        ("Dead Channel", "Analog / VHS horror — the scene degraded into a haunted broadcast.", c => c.ApplyHorrorPreset()),
        ("Hellfire", "Fiery, hellish backdrop — billowing flames + embers. Background only.", c => c.ApplyHellfirePreset()),
        ("Aquarium", "Underwater — caustics, shafts, bubbles + submerged character.", c => c.ApplyAquariumPreset()),
        ("Aurora Borealis", "Night sky with northern-light curtains + stars. Background only.", c => c.ApplyAuroraPreset()),
        ("Synthwave", "Retro sunset — gradient sky, neon sun, grid floor. Background only.", c => c.ApplySynthwavePreset()),
        ("Blood Moon", "Huge dim-red moon in a crimson sky with torn clouds. Background only.", c => c.ApplyBloodMoonPreset()),
        ("Tempe — The Red That Follows", "Tempe Pelis variant: the relic as a red moon-eye with a teal limbal ring over a crimson abyss. Background only.", c => c.ApplyTempeMoonPreset()),
        ("Forge", "A smithy — molten furnace, rising sparks, a trough of glowing metal + crafting aether. Background only.", c => c.ApplyForgePreset()),
        ("Artisan's Rest", "A chill dusk with soft bokeh lights in the eight crafting-class colours. Background only.", c => c.ApplyArtisanPreset()),
        ("Sunset", "A warm sun setting over shimmering water with streaked clouds — golden-hour chill. Background only.", c => c.ApplySunsetPreset()),
        ("Sin Eater", "Cold Light-corruption in a grieving dark — distant light, halo, soul-mist, crystalline fractures. Background only, VFX-safe.", c => c.ApplySinEaterPreset()),
        ("Magitek HUD", "A Garlean targeting-visor over the scene — brackets, reticle, rangefinder, sweeping radar, tech mesh, scanlines.", c => c.ApplyMagitekHudPreset()),
        ("Gpose Viewfinder", "A clean in-camera frame — corner brackets, rule-of-thirds guides, REC dot + readout. Overlay, neutral grade.", c => c.ApplyGposeViewfinderPreset()),
        ("AoE Telegraph", "The FFXIV floor-marker look — a glowing orange ground telegraph under the subject. Add cone/donut/line via Elements.", c => c.ApplyAoeTelegraphPreset()),
        ("Evercold", "A frozen frost world — pale cold sky over a snow horizon, drifting snowfall and cold mist, with an icy rim, cold fog and frost creeping in on the lens.", c => c.ApplyEvercoldPreset()),
        ("Studio Portrait", "A photograph, not a render — a grey seamless sweep lit by its own key, thrown out of focus, with the subject casting a real shadow onto it. No outline: separation comes from the backdrop falloff, the defocus and the shadow, never from a light tracing the silhouette. Softened skin, restrained grade, clean glass.", c => c.ApplyStudioPortraitPreset()),
        ("On Location", "A photographic finish for gposes shot in the world — it resolves the scene rather than restyling it. Opens the shadows, rolls the highlights so a bright sky keeps its gradient, enriches colour without shifting any hue, then a little clarity, fine grain and gentle corner falloff. No backdrop, no fog, no colour scheme imposed.", c => c.ApplyOnLocationPreset()),
        ("Duality", "A two-hander with the frame split down the middle — one side lit green, the other charcoal, meeting at a soft seam rather than a hard line. Uses the background A/B combine, and leans on edge integration so neither character gets the bright cut-out outline this kind of shot usually has.", c => c.ApplyDualityPreset()),
        ("Aetherbloom", "An amplifier, not a generator \u2014 draws nothing procedural and lets the VFX you staged in Brio be the picture. Low-threshold bloom, halation and an anamorphic streak make the game\u2019s own light behave like light, with a film rolloff that HOLDS hue instead of washing highlights white. Background push, blur and DoF are deliberately off: the VFX live in the far region and all three would damage them.", c => c.ApplyAetherbloomPreset()),
        ("Tempest", "A true storm on all four fields \u2014 a rolling cloud cell, bolts punched in on brightness so only a strike clears the bar, and wind driven across the frame with rain multiplying it down. The wind is a divergence-free curl field drawn by line integral convolution; the bolts are fractal channels that flash and decay.", c => c.ApplyTempestPreset()),
        ("Hoarfrost", "A cold world built from all four fields \u2014 a pale sky with snowfall, a sheet of ice crystals mixed into it on brightness, and frost creeping across the lens in front with cold air beneath it. Skin kept warm so the face stays human against all that blue.", c => c.ApplyHoarfrostPreset()),
        ("Emberfall", "Fire on all four fields \u2014 a dark room lit by a wall of flame punched in on brightness, embers on the air, and flame licking up the bottom of frame with smoke multiplying over it so the top goes dark. Cool shadows hold the warm back from taking the whole frame.", c => c.ApplyEmberfallPreset()),
        ("Starlit Vow", "A night-sky studio portrait — cold blue-white stars over indigo, a warm pool of light where he stands, and light spilling off his silhouette into the dark. Cool shadows against gold highlights, so hope and its cost sit in the same frame. Still and quiet: no falling particles.", c => c.ApplyStarlitVowPreset()),
        ("Chinese Ink", "Sumi-e brush painting — black ink on warm rice paper, receding mountain ridges, bleeding washes with pigment pooling, dry-brush texture and flicked spatter. Soft monochrome grade.", c => c.ApplyChineseInkPreset()),
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
