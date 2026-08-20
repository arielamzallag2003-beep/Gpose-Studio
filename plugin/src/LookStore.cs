using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace GPoseStudio;

public static partial class LookStore
{
    internal static readonly HashSet<string> Exclude = new()
    {
        "Version", "OutputDirectory", "LivePreview", "DebugShowDepth",
        "DebugShowGate", "DebugShowClipping", "Bypass", "SwapRedBlue", "FlipVertical",
        "ShowGuides", "GuideThirds", "GuideGolden", "GuideCenter", "GuideHorizon", "GuideHorizonY", "GuideOpacity",
        "ExportAspect", "ShowExportFrame", "ExportScale", "ExportFormat", "ExportJpegQuality",
        "EmbedLookInPng", "ExportTransparent", "DebugShowMatte",
        "DebugShowMask", "MaskShowWhich", "PlacingMask", "PlacingText", "LoadFromBase",
        "FreezeAnimation",
        "Pinned", "PinnedLooks",
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

    public static bool IsNameUsable(string? raw, out string error) => LookName.IsUsable(raw, out error);

    private static bool TryResolve(string? name, out string path, out string error)
    {
        path = "";
        if (!IsNameUsable(name, out error)) return false;

        try
        {
            var dir = Path.GetFullPath(FolderPath);
            var full = Path.GetFullPath(Path.Combine(dir, LookName.Clean(name) + ".json"));
            var root = dir.EndsWith(Path.DirectorySeparatorChar) ? dir : dir + Path.DirectorySeparatorChar;

            if (!full.StartsWith(root, StringComparison.OrdinalIgnoreCase))
            {
                error = "That name does not resolve inside the looks folder.";
                return false;
            }
            if (full.Length > LookName.MaxPathLength)
            {
                error = "That name makes the file path too long. Use a shorter one.";
                return false;
            }

            path = full;
            return true;
        }
        catch (Exception ex)
        {
            error = "That name cannot be used as a file.";
            Services.Log.Warning($"LookStore.TryResolve('{name}') failed: {ex.Message}");
            return false;
        }
    }

    private const int MaxLookBytes = 4 * 1024 * 1024;
    private static readonly JsonSerializerOptions ReadOptions = new() { MaxDepth = 32 };

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

    private static readonly HashSet<string> NotShareable = new() { "ElemImages" };

    private static readonly PropertyInfo[] Props = typeof(PluginConfig).GetProperties();

    public static string Capture(PluginConfig cfg, bool forSharing = false, Part part = Part.All)
    {
        var dict = new Dictionary<string, object?>();
        foreach (var p in Props)
        {
            if (!p.CanRead || !p.CanWrite || Exclude.Contains(p.Name)) continue;
            if (forSharing && NotShareable.Contains(p.Name)) continue;
            if (part != Part.All && PartOf(p.Name) != part && !IsAlwaysCarried(p.Name)) continue;
            dict[p.Name] = p.GetValue(cfg);
        }
        return JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true });
    }

    public static bool SaveToFile(string path, PluginConfig cfg, out string error)
    {
        error = "";
        try
        {
            var tmp = path + ".tmp";
            File.WriteAllText(tmp, Capture(cfg, forSharing: true));
            File.Move(tmp, path, overwrite: true);
            return true;
        }
        catch (Exception ex)
        {
            error = $"Could not write that file: {ex.Message}";
            Services.Log.Warning($"LookStore.SaveToFile('{path}') failed: {ex.Message}");
            return false;
        }
    }

    public static bool LoadFromFile(string path, PluginConfig cfg, Part part, out string error)
    {
        error = "";
        try
        {
            var info = new FileInfo(path);
            if (!info.Exists) { error = "That file does not exist."; return false; }
            if (info.Length > MaxImportBytes)
            {
                error = "That file is too large to be a look or an exported image.";
                return false;
            }

            var bytes = File.ReadAllBytes(path);

            if (Png.TryReadEmbeddedText(bytes, out var embedded))
            {
                if (!Apply(embedded, cfg, part)) { error = "That image carries a look this build cannot read."; return false; }
                return true;
            }

            if (LooksBinary(bytes))
            {
                error = "That image has no look embedded in it.";
                return false;
            }

            var json = System.Text.Encoding.UTF8.GetString(bytes);
            if (!Apply(json, cfg, part)) { error = "That file is not a look."; return false; }
            return true;
        }
        catch (Exception ex)
        {
            error = $"Could not read that file: {ex.Message}";
            Services.Log.Warning($"LookStore.LoadFromFile('{path}') failed: {ex.Message}");
            return false;
        }
    }

    private const int MaxImportBytes = 64 * 1024 * 1024;

    private static bool LooksBinary(byte[] bytes)
    {
        int n = Math.Min(bytes.Length, 512);
        for (int i = 0; i < n; i++)
        {
            byte b = bytes[i];
            if (b == 0) return true;
            if (b == (byte)'{') return false;
            if (b is (byte)' ' or (byte)'\r' or (byte)'\n' or (byte)'\t' or 0xEF or 0xBB or 0xBF) continue;
            return true;
        }
        return true;
    }

    public static bool Save(string name, PluginConfig cfg, out string error, Part part = Part.All)
    {
        if (!TryResolve(name, out var path, out error)) return false;

        var tmp = path + ".tmp";
        try
        {
            File.WriteAllText(tmp, Capture(cfg, forSharing: false, part: part));
            File.Move(tmp, path, overwrite: true);
            return true;
        }
        catch (Exception ex)
        {
            try { if (File.Exists(tmp)) File.Delete(tmp); } catch {  }
            error = $"Could not save the look: {ex.Message}";
            Services.Log.Warning($"LookStore.Save('{name}') failed: {ex}");
            return false;
        }
    }

    public static bool Load(string name, PluginConfig cfg) => Load(name, cfg, Part.All);

    public static bool Load(string name, PluginConfig cfg, Part part) => Load(name, cfg, part, out _);

    public static bool Load(string name, PluginConfig cfg, Part part, out int applied)
    {
        applied = 0;
        if (!TryResolve(name, out var path, out _)) return false;
        try
        {
            var info = new FileInfo(path);
            if (!info.Exists) return false;
            if (info.Length > MaxLookBytes)
            {
                Services.Log.Warning($"LookStore.Load('{name}'): {info.Length} bytes exceeds the cap; refusing.");
                return false;
            }
            return Apply(File.ReadAllText(path), cfg, part, out applied);
        }
        catch (Exception ex)
        {
            Services.Log.Warning($"LookStore.Load('{name}') failed: {ex.Message}");
            return false;
        }
    }

    public static bool Apply(string json, PluginConfig cfg, Part part) => Apply(json, cfg, part, out _);

    public static bool Apply(string json, PluginConfig cfg, Part part, out int applied)
    {
        applied = 0;
        Dictionary<string, JsonElement>? dict;
        try { dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json, ReadOptions); }
        catch { return false; }
        if (dict == null) return false;

        foreach (var p in Props)
        {
            if (!p.CanWrite || Exclude.Contains(p.Name)) continue;
            if (part != Part.All && PartOf(p.Name) != part && !IsAlwaysCarried(p.Name)) continue;
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

                if (part == Part.All || !IsAlwaysCarried(p.Name)) applied++;
            }
            catch {  }
        }

        if (part == Part.All
            && dict.ContainsKey("PatColOverride")
            && !dict.ContainsKey("BgBPatColOverride"))
            cfg.CarryPatternIdentity();
        return true;
    }

    public static bool Delete(string name, out string error)
    {
        if (!TryResolve(name, out var path, out error)) return false;
        try
        {
            File.Delete(path);
            return true;
        }
        catch (Exception ex)
        {
            error = $"Could not delete the look: {ex.Message}";
            Services.Log.Warning($"LookStore.Delete('{name}') failed: {ex.Message}");
            return false;
        }
    }

    public static readonly (string Name, string Cat, Action<PluginConfig> Apply)[] Builtins =
    {
        ("Studio Grey", "Portrait", c => c.ApplyStudioGreyPreset()),
        ("Window Light", "Portrait", c => c.ApplyWindowLightPreset()),
        ("Low Key", "Portrait", c => c.ApplyLowKeyPreset()),
        ("High Key", "Portrait", c => c.ApplyHighKeyPreset()),

        ("Anamorphic", "Cinematic", c => c.ApplyAnamorphicPreset()),
        ("Night Noir", "Cinematic", c => c.ApplyNightNoirPreset()),
        ("Golden Hour", "Cinematic", c => c.ApplyGoldenHourPreset()),

        ("Deep Field", "Backdrop", c => c.ApplyDeepFieldPreset()),
        ("The Void", "Backdrop", c => c.ApplyTheVoidPreset()),
        ("Sumi", "Backdrop", c => c.ApplySumiPreset()),

        ("Snowfall", "Weather", c => c.ApplySnowfallPreset()),

        ("Neon Drive", "Stylised", c => c.ApplyNeonDrivePreset()),
        ("Lost Signal", "Stylised", c => c.ApplyLostSignalPreset()),
    };

    public static readonly (string Name, string Cat, Action<PluginConfig> Apply)[] Legacy =
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

    public static bool InstallLegacy(string name, out string error)
    {
        error = "";
        foreach (var (n, _, apply) in Legacy)
        {
            if (n != name) continue;
            if (Exists(name)) { error = $"‘{name}’ is already in your looks."; return false; }
            var tmp = new PluginConfig();
            apply(tmp);
            tmp.CarryPatternIdentity();
            return SaveContent(name, Capture(tmp), out error);
        }
        error = $"No legacy look called ‘{name}’.";
        return false;
    }

    public static bool Exists(string name) =>
        TryResolve(name, out var path, out _) && File.Exists(path);

    private const int BuiltinsVersion = 67;
    private static string MarkerPath => Path.Combine(FolderPath, ".builtins");

    private static void RecordBuiltinHash(string name, string content)
    {
        try
        {
            var state = BuiltinGuard.Parse(File.Exists(MarkerPath) ? File.ReadAllText(MarkerPath) : null);
            state.Hashes[name] = BuiltinGuard.Hash(content);
            File.WriteAllText(MarkerPath, BuiltinGuard.Write(state));
        }
        catch {  }
    }

    public static void SeedBuiltins()
    {
        BuiltinGuard.State state;
        try { state = BuiltinGuard.Parse(File.Exists(MarkerPath) ? File.ReadAllText(MarkerPath) : null); }
        catch { state = new BuiltinGuard.State(); }

        bool refresh = state.Version < BuiltinsVersion;
        int kept = 0;

        foreach (var (name, _, apply) in Builtins)
        {
            bool present = Exists(name);
            if (!refresh && present) continue;

            string? current = null;
            if (present)
            {
                try { if (TryResolve(name, out var p, out _)) current = File.ReadAllText(p); }
                catch { current = null; }
            }
            state.Hashes.TryGetValue(name, out var recorded);
            if (!BuiltinGuard.MayOverwrite(current, recorded))
            {
                kept++;
                continue;
            }

            var tmp = new PluginConfig();
            apply(tmp);
            tmp.CarryPatternIdentity();
            var content = Capture(tmp);
            if (SaveContent(name, content, out var err)) state.Hashes[name] = BuiltinGuard.Hash(content);
            else Services.Log.Warning($"could not seed built-in look '{name}': {err}");
        }

        foreach (var (name, _, _) in Builtins)
        {
            if (state.Hashes.ContainsKey(name)) continue;
            try
            {
                if (TryResolve(name, out var p, out _) && File.Exists(p))
                    state.Hashes[name] = BuiltinGuard.Hash(File.ReadAllText(p));
            }
            catch {  }
        }

        if (refresh)
        {
            if (kept > 0)
                Services.Log.Info($"kept {kept} built-in look(s) that had been edited; the rest were refreshed.");
            state.Version = BuiltinsVersion;
            try { File.WriteAllText(MarkerPath, BuiltinGuard.Write(state)); } catch { }
        }
    }

    private static bool SaveContent(string name, string content, out string error)
    {
        if (!TryResolve(name, out var path, out error)) return false;
        var tmp = path + ".tmp";
        try
        {
            File.WriteAllText(tmp, content);
            File.Move(tmp, path, overwrite: true);
            return true;
        }
        catch (Exception ex)
        {
            try { if (File.Exists(tmp)) File.Delete(tmp); } catch { }
            error = $"Could not save the look: {ex.Message}";
            return false;
        }
    }

    public static void RegenerateBuiltin(string name)
    {
        foreach (var (n, _, apply) in Builtins)
            if (n == name)
            {
                var tmp = new PluginConfig();
                apply(tmp);
                tmp.CarryPatternIdentity();
                var content = Capture(tmp);
                if (SaveContent(name, content, out var err)) RecordBuiltinHash(name, content);
                else Services.Log.Warning($"could not regenerate built-in look '{name}': {err}");
                return;
            }
    }

}
