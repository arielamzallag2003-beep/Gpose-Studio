using System;
using System.Collections.Generic;
using System.IO;

namespace Dalamud.Configuration
{
    public interface IPluginConfiguration
    {
        int Version { get; set; }
    }
}

namespace GPoseStudio
{

internal static class Services
{
    public static readonly FakePluginInterface PluginInterface = new();
    public static readonly FakeLog Log = new();
}

internal sealed class FakePluginInterface
{
    private readonly string _dir = Path.Combine(
        Path.GetTempPath(), "gposestudio-tests", Guid.NewGuid().ToString("N"));

    public object? StoredConfig;

    public string GetPluginConfigDirectory()
    {
        Directory.CreateDirectory(_dir);
        return _dir;
    }

    public void SavePluginConfig(object cfg) => StoredConfig = cfg;

    public object? GetPluginConfig() => StoredConfig;
}

internal sealed class FakeLog
{
    public readonly List<string> Lines = new();

    public void Info(string m) => Lines.Add("INF " + m);
    public void Warning(string m) => Lines.Add("WRN " + m);
    public void Warning(string m, params object[] a) => Lines.Add("WRN " + string.Format(m, a));
    public void Error(string m) => Lines.Add("ERR " + m);
    public void Error(Exception e, string m) => Lines.Add("ERR " + m + " " + e.Message);
}
}
