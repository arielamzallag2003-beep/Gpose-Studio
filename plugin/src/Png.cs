using System;
using System.IO;
using System.IO.Compression;

namespace GPoseStudio;

public static class Png
{
    private const int Bpp = 4;

    public static byte[] Encode(int width, int height, byte[] rgba)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("width/height must be positive");
        if (rgba.Length < (long)width * height * 4)
            throw new ArgumentException("rgba too small for dimensions");

        using var ms = new MemoryStream();
        Span<byte> sig = stackalloc byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        ms.Write(sig);

        var ihdr = new byte[13];
        WriteBE(ihdr, 0, (uint)width);
        WriteBE(ihdr, 4, (uint)height);
        ihdr[8] = 8;
        ihdr[9] = 6;
        ihdr[10] = 0;
        ihdr[11] = 0;
        ihdr[12] = 0;
        WriteChunk(ms, "IHDR", ihdr);

        int rowBytes = width * Bpp;
        using (var comp = new MemoryStream())
        {
            using (var z = new ZLibStream(comp, CompressionLevel.Optimal, leaveOpen: true))
            {
                byte[] prev = new byte[rowBytes];
                byte[] cur = new byte[rowBytes];
                byte[] best = new byte[rowBytes];
                byte[] scratch = new byte[rowBytes];
                for (int y = 0; y < height; y++)
                {
                    Buffer.BlockCopy(rgba, y * rowBytes, cur, 0, rowBytes);
                    int bestFilter = 0;
                    long bestScore = long.MaxValue;
                    for (int f = 0; f <= 4; f++)
                    {
                        FilterRow(f, cur, prev, rowBytes, scratch);
                        long score = 0;
                        for (int i = 0; i < rowBytes; i++) { int v = scratch[i]; score += v < 128 ? v : 256 - v; }
                        if (score < bestScore) { bestScore = score; bestFilter = f; (best, scratch) = (scratch, best); }
                    }
                    z.WriteByte((byte)bestFilter);
                    z.Write(best, 0, rowBytes);
                    (prev, cur) = (cur, prev);
                }
            }
            WriteChunk(ms, "IDAT", comp.ToArray());
        }

        WriteChunk(ms, "IEND", Array.Empty<byte>());
        return ms.ToArray();
    }

    private static void FilterRow(int f, byte[] cur, byte[] prev, int n, byte[] outp)
    {
        for (int i = 0; i < n; i++)
        {
            int raw = cur[i];
            int a = i >= Bpp ? cur[i - Bpp] : 0;
            int b = prev[i];
            int c = i >= Bpp ? prev[i - Bpp] : 0;
            int pred = f switch
            {
                1 => a,
                2 => b,
                3 => (a + b) >> 1,
                4 => Paeth(a, b, c),
                _ => 0,
            };
            outp[i] = (byte)((raw - pred) & 0xFF);
        }
    }

    private static int Paeth(int a, int b, int c)
    {
        int p = a + b - c;
        int pa = Math.Abs(p - a), pb = Math.Abs(p - b), pc = Math.Abs(p - c);
        return pa <= pb && pa <= pc ? a : pb <= pc ? b : c;
    }

    private static void WriteBE(byte[] b, int o, uint v)
    {
        b[o] = (byte)(v >> 24); b[o + 1] = (byte)(v >> 16);
        b[o + 2] = (byte)(v >> 8); b[o + 3] = (byte)v;
    }

    private static void WriteChunk(Stream s, string type, byte[] data)
    {
        Span<byte> len = stackalloc byte[4];
        len[0] = (byte)(data.Length >> 24); len[1] = (byte)(data.Length >> 16);
        len[2] = (byte)(data.Length >> 8); len[3] = (byte)data.Length;
        s.Write(len);

        var typeBytes = System.Text.Encoding.ASCII.GetBytes(type);
        s.Write(typeBytes);
        s.Write(data);

        uint crc = Crc32(typeBytes, data);
        Span<byte> c = stackalloc byte[4];
        c[0] = (byte)(crc >> 24); c[1] = (byte)(crc >> 16);
        c[2] = (byte)(crc >> 8); c[3] = (byte)crc;
        s.Write(c);
    }

    private static readonly uint[] CrcTable = BuildCrcTable();
    private static uint[] BuildCrcTable()
    {
        var t = new uint[256];
        for (uint n = 0; n < 256; n++)
        {
            uint c = n;
            for (int k = 0; k < 8; k++)
                c = (c & 1) != 0 ? 0xEDB88320 ^ (c >> 1) : c >> 1;
            t[n] = c;
        }
        return t;
    }

    private static uint Crc32(byte[] a, byte[] b)
    {
        uint c = 0xFFFFFFFF;
        foreach (var x in a) c = CrcTable[(c ^ x) & 0xFF] ^ (c >> 8);
        foreach (var x in b) c = CrcTable[(c ^ x) & 0xFF] ^ (c >> 8);
        return c ^ 0xFFFFFFFF;
    }
}
