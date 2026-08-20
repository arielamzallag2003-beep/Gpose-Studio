using System;
using System.Collections.Generic;
using System.IO;

namespace GPoseStudio;

internal static class LookName
{
    public const int MaxLength = 64;

    public const int MaxPathLength = 250;

    private static readonly HashSet<string> Reserved = new(StringComparer.OrdinalIgnoreCase)
    {
        "CON", "PRN", "AUX", "NUL", "CLOCK$",
        "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
        "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9",
    };

    public static string Clean(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return "";

        var stripped = string.Concat(raw.Trim().Split(Path.GetInvalidFileNameChars()));

        stripped = stripped.TrimEnd('.', ' ');
        if (stripped.Length > MaxLength) stripped = stripped.Substring(0, MaxLength).TrimEnd('.', ' ');

        return stripped;
    }

    public static bool IsReservedDeviceName(string? name)
    {
        if (string.IsNullOrEmpty(name)) return false;
        int dot = name.IndexOf('.');
        var stem = dot >= 0 ? name.Substring(0, dot) : name;
        return Reserved.Contains(stem);
    }

    public static bool IsUsable(string? raw, out string error)
    {
        error = "";

        if (string.IsNullOrWhiteSpace(raw))
        {
            error = "A look needs a name.";
            return false;
        }

        var cleaned = Clean(raw);
        if (cleaned.Length == 0)
        {
            error = "That name has no characters a file can use.";
            return false;
        }

        int dot = cleaned.IndexOf('.');
        var stem = dot >= 0 ? cleaned.Substring(0, dot) : cleaned;
        if (Reserved.Contains(stem))
        {
            error = $"‘{stem}’ is a name Windows keeps for itself. Try another.";
            return false;
        }

        return true;
    }
}
