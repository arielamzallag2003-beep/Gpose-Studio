using System;
using System.IO;
using System.IO.Compression;
using GPoseStudio;
using Xunit;

public class PngTests
{
    [Fact]
    public void Encode_ValidSignatureDimensionsAndIdat()
    {
        int w = 5, h = 3;
        var rgba = new byte[w * h * 4];
        for (int i = 0; i < rgba.Length; i++) rgba[i] = (byte)(i * 3);

        var png = Png.Encode(w, h, rgba);

        byte[] sig = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        for (int i = 0; i < 8; i++) Assert.Equal(sig[i], png[i]);

        int pw = png[16] << 24 | png[17] << 16 | png[18] << 8 | png[19];
        int ph = png[20] << 24 | png[21] << 16 | png[22] << 8 | png[23];
        Assert.Equal(w, pw);
        Assert.Equal(h, ph);

        var idat = FindChunk(png, "IDAT");
        Assert.NotNull(idat);
        using var inflated = new MemoryStream();
        using (var z = new ZLibStream(new MemoryStream(idat!), CompressionMode.Decompress))
            z.CopyTo(inflated);
        var raw = inflated.ToArray();
        Assert.Equal(h * (1 + w * 4), raw.Length);

        int rb = w * 4;
        var recon = new byte[w * h * 4];
        for (int y = 0; y < h; y++)
        {
            int ft = raw[y * (1 + rb)];
            Assert.InRange(ft, 0, 4);
            for (int i = 0; i < rb; i++)
            {
                int val = raw[y * (1 + rb) + 1 + i];
                int a = i >= 4 ? recon[y * rb + i - 4] : 0;
                int b = y > 0 ? recon[(y - 1) * rb + i] : 0;
                int c = (i >= 4 && y > 0) ? recon[(y - 1) * rb + i - 4] : 0;
                int pred = ft switch
                {
                    1 => a, 2 => b, 3 => (a + b) >> 1,
                    4 => Paeth(a, b, c), _ => 0,
                };
                recon[y * rb + i] = (byte)((val + pred) & 0xFF);
            }
        }
        Assert.Equal(rgba, recon);

        Assert.NotNull(FindChunk(png, "IEND"));
    }

    private static int Paeth(int a, int b, int c)
    {
        int p = a + b - c, pa = Math.Abs(p - a), pb = Math.Abs(p - b), pc = Math.Abs(p - c);
        return pa <= pb && pa <= pc ? a : pb <= pc ? b : c;
    }

    [Fact]
    public void Encode_AdaptiveFilteringShrinksGradient()
    {
        int w = 256, h = 256;
        var rgba = new byte[w * h * 4];
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                int o = (y * w + x) * 4;
                rgba[o] = (byte)x; rgba[o + 1] = (byte)y; rgba[o + 2] = (byte)((x + y) / 2); rgba[o + 3] = 255;
            }
        var png = Png.Encode(w, h, rgba);
        Assert.True(png.Length < w * h * 4 / 3, $"expected strong compression, got {png.Length} bytes");
    }

    private static byte[]? FindChunk(byte[] png, string type)
    {
        int pos = 8;
        while (pos + 8 <= png.Length)
        {
            int len = png[pos] << 24 | png[pos + 1] << 16 | png[pos + 2] << 8 | png[pos + 3];
            var t = System.Text.Encoding.ASCII.GetString(png, pos + 4, 4);
            int dataStart = pos + 8;
            if (t == type)
                return png[dataStart..(dataStart + len)];
            pos = dataStart + len + 4;
        }
        return null;
    }
}
