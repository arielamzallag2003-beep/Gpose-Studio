using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace GPoseStudio;

internal static class BuiltinGuard
{
    public sealed class State
    {
        public int Version { get; set; }
        public Dictionary<string, string> Hashes { get; set; } = new();
    }

    public static string Hash(string content)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(content ?? ""));
        return Convert.ToHexString(bytes);
    }

    public static bool MayOverwrite(string? current, string? recorded)
    {
        if (current == null) return true;
        if (string.IsNullOrEmpty(recorded)) return false;
        return string.Equals(Hash(current), recorded, StringComparison.OrdinalIgnoreCase);
    }

    public static State Parse(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return new State();

        var trimmed = text.Trim();
        if (int.TryParse(trimmed, out int bare)) return new State { Version = bare };

        try
        {
            return JsonSerializer.Deserialize<State>(trimmed) ?? new State();
        }
        catch
        {
            return new State();
        }
    }

    public static string Write(State state) =>
        JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
}
