using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace GPoseStudio;

public static class MaskRegions
{
    public const int MaxOverrideChars = 256 * 1024;

    private static readonly JsonSerializerOptions ReadOptions = new() { MaxDepth = 4 };

    private static readonly PropertyInfo[] All =
        typeof(PluginConfig).GetProperties()
            .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0)
            .ToArray();

    private static readonly bool[] AllOverridable = All.Select(IsOverridable).ToArray();

    public static readonly PropertyInfo[] Overridable = All.Where(IsOverridable).ToArray();

    private static readonly Dictionary<string, PropertyInfo> ByName =
        Overridable.ToDictionary(p => p.Name, StringComparer.Ordinal);

    internal static bool IsOverridable(PropertyInfo p)
    {
        if (!p.CanRead || !p.CanWrite || p.GetIndexParameters().Length != 0) return false;
        var t = p.PropertyType;
        if (t != typeof(float) && t != typeof(int) && t != typeof(bool)) return false;
        if (LookStore.Exclude.Contains(p.Name)) return false;
        string n = p.Name;
        if (n.StartsWith("Mask", StringComparison.Ordinal)) return false;
        if (n.StartsWith("Frame", StringComparison.Ordinal) || n == "EnFrame" || n == "EnText") return false;
        if (n.StartsWith("Cutout", StringComparison.Ordinal)) return false;
        if (n.StartsWith("Debug", StringComparison.Ordinal) || n.StartsWith("Guide", StringComparison.Ordinal)
            || n.StartsWith("Export", StringComparison.Ordinal) || n == "Version") return false;
        return true;
    }

    public static void BuildVariant(PluginConfig baseCfg, string? overrides, PluginConfig into)
    {
        foreach (var p in All) p.SetValue(into, p.GetValue(baseCfg));
        ApplyOverrides(overrides, into);
    }

    public static int ApplyOverrides(string? overrides, PluginConfig into)
    {
        int n = 0;
        foreach (var (name, value) in ToObjects(overrides))
        {
            ByName[name].SetValue(into, value);
            n++;
        }
        return n;
    }

    public static void ResetOverridable(PluginConfig c)
    {
        var def = new PluginConfig();
        foreach (var p in Overridable) p.SetValue(c, p.GetValue(def));
    }

    private static readonly ConditionalWeakTable<string, string[]> NamesCache = new();

    public static string[] Names(string? overrides)
    {
        if (string.IsNullOrEmpty(overrides)) return Array.Empty<string>();
        return NamesCache.GetValue(overrides, s => ToObjects(s).Keys.ToArray());
    }

    public static int Count(string? overrides) => Names(overrides).Length;

    public static string Remove(string? overrides, string name)
    {
        var d = ToObjects(overrides);
        d.Remove(name);
        return Serialize(d);
    }

    public static string Merge(IEnumerable<string?> bottomToTop)
    {
        var d = new SortedDictionary<string, object>(StringComparer.Ordinal);
        foreach (var src in bottomToTop)
            foreach (var (name, value) in ToObjects(src)) d[name] = value;
        return Serialize(d);
    }

    public readonly record struct Piece(int Inside, int Outside, int[] Masks);

    public const int MaxCombined = 3;

    public static List<Piece> PlanPieces(IReadOnlyList<int> activeBottomToTop, int overlap)
    {
        var list = new List<Piece>();
        int n = activeBottomToTop.Count, all = 0;
        foreach (var i in activeBottomToTop) all |= 8 << i;
        if (overlap == 1 && n > MaxCombined) overlap = 0;
        if (overlap == 1 && n > 1)
        {
            for (int subset = 1; subset < (1 << n); subset++)
            {
                var masks = new List<int>(n);
                int inside = 0;
                for (int k = 0; k < n; k++)
                    if ((subset & (1 << k)) != 0) { masks.Add(activeBottomToTop[k]); inside |= 8 << activeBottomToTop[k]; }
                list.Add(new Piece(inside, all & ~inside, masks.ToArray()));
            }
            return list.OrderBy(p => p.Masks.Length).ToList();
        }
        foreach (var i in activeBottomToTop)
            list.Add(new Piece(8 << i, overlap == 2 ? all & ~(8 << i) : 0, new[] { i }));
        return list;
    }

    public static string Label(string name)
    {
        bool toggle = name.Length > 2 && name.StartsWith("En", StringComparison.Ordinal) && char.IsUpper(name[2]);
        string s = toggle ? name.Substring(2) : name;
        var sb = new System.Text.StringBuilder(s.Length + 12);
        for (int i = 0; i < s.Length; i++)
        {
            if (i > 0 && char.IsUpper(s[i]) && !char.IsUpper(s[i - 1])) sb.Append(' ');
            sb.Append(s[i]);
        }
        if (toggle) sb.Append(" (on / off)");
        return sb.ToString();
    }

    private static SortedDictionary<string, object> ToObjects(string? overrides)
    {
        var d = new SortedDictionary<string, object>(StringComparer.Ordinal);
        if (string.IsNullOrWhiteSpace(overrides) || overrides.Length > MaxOverrideChars) return d;
        Dictionary<string, JsonElement>? parsed;
        try { parsed = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(overrides, ReadOptions); }
        catch { return d; }
        if (parsed == null) return d;
        foreach (var (name, el) in parsed)
        {
            if (!ByName.TryGetValue(name, out var p)) continue;
            try
            {
                if (p.PropertyType == typeof(float)) d[name] = el.GetSingle();
                else if (p.PropertyType == typeof(int)) d[name] = el.GetInt32();
                else d[name] = el.GetBoolean();
            }
            catch {  }
        }
        return d;
    }

    private static string Serialize(SortedDictionary<string, object> d)
        => d.Count == 0 ? "" : JsonSerializer.Serialize(d);

    public sealed class EditSession
    {
        private readonly object?[] _atBegin = new object?[All.Length];
        public PluginConfig Variant { get; } = new();

        public void Begin(PluginConfig baseCfg, string? overrides)
        {
            BuildVariant(baseCfg, overrides, Variant);
            for (int k = 0; k < All.Length; k++) _atBegin[k] = All[k].GetValue(Variant);
        }

        public string? End(PluginConfig baseCfg, string? currentOverrides)
        {
            SortedDictionary<string, object>? next = null;
            for (int k = 0; k < All.Length; k++)
            {
                var p = All[k];
                var v = p.GetValue(Variant);
                if (Equals(v, _atBegin[k])) continue;
                if (!AllOverridable[k]) { p.SetValue(baseCfg, v); continue; }
                next ??= ToObjects(currentOverrides);
                if (Equals(p.GetValue(baseCfg), v)) next.Remove(p.Name);
                else next[p.Name] = v!;
            }
            return next == null ? null : Serialize(next);
        }
    }
}
