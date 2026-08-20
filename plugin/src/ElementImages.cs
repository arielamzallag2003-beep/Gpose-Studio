using System;
using System.IO;

namespace GPoseStudio;

internal static class ElementImages
{
    public const string FolderName = "elements";

    public const long MaxBytes = 64L * 1024 * 1024;

    private static readonly string[] Allowed = { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tga", ".dds" };

    public static string FolderPath
    {
        get
        {
            var d = Path.Combine(Services.PluginInterface.GetPluginConfigDirectory(), FolderName);
            try { Directory.CreateDirectory(d); } catch {  }
            return d;
        }
    }

    public static bool IsStoredName(string? stored)
    {
        if (string.IsNullOrEmpty(stored)) return false;
        if (stored.IndexOf('/') >= 0 || stored.IndexOf('\\') >= 0) return false;
        if (stored.Contains("..", StringComparison.Ordinal)) return false;
        if (Path.IsPathRooted(stored)) return false;
        return stored == Path.GetFileName(stored);
    }

    public static string Resolve(string root, string? stored)
    {
        if (string.IsNullOrWhiteSpace(stored)) return "";
        if (IsStoredName(stored)) return Path.Combine(root, stored);
        return Path.IsPathRooted(stored) ? stored : "";
    }

    public static bool IsAllowedExtension(string path)
        => Array.IndexOf(Allowed, Path.GetExtension(path ?? "").ToLowerInvariant()) >= 0;

    public static string UniqueName(Func<string, bool> exists, string fileName)
    {
        if (exists == null) throw new ArgumentNullException(nameof(exists));
        var name = LookName.Clean(Path.GetFileNameWithoutExtension(fileName ?? ""));
        if (name.Length == 0) name = "image";
        if (LookName.IsReservedDeviceName(name)) name = "_" + name;
        var ext = Path.GetExtension(fileName ?? "").ToLowerInvariant();
        if (ext.Length == 0) ext = ".png";

        var candidate = name + ext;
        for (int n = 2; exists(candidate) && n < 1000; n++) candidate = $"{name} ({n}){ext}";
        return candidate;
    }

    public static bool Import(string source, out string storedName, out string error)
    {
        storedName = ""; error = "";
        try
        {
            if (string.IsNullOrWhiteSpace(source) || !File.Exists(source))
            { error = "That file is not there any more."; return false; }
            if (!IsAllowedExtension(source))
            { error = $"{Path.GetExtension(source)} is not an image this can read."; return false; }

            var info = new FileInfo(source);
            if (info.Length > MaxBytes)
            { error = $"That file is {info.Length / (1024 * 1024)} MB; the limit is {MaxBytes / (1024 * 1024)} MB."; return false; }
            if (info.Length == 0) { error = "That file is empty."; return false; }

            var root = FolderPath;
            var name = UniqueName(n => File.Exists(Path.Combine(root, n)), Path.GetFileName(source));
            var dest = Path.Combine(root, name);

            var tmp = dest + ".tmp";
            File.Copy(source, tmp, overwrite: true);
            File.Move(tmp, dest, overwrite: true);

            storedName = name;
            return true;
        }
        catch (Exception ex)
        {
            error = "Could not copy that image: " + ex.Message;
            return false;
        }
    }
}
