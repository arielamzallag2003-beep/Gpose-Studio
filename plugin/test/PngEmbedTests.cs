using System;
using System.Linq;
using System.Text;
using GPoseStudio;
using Xunit;

public class PngEmbedTests
{
    private static byte[] Pixels(int w, int h)
    {
        var px = new byte[w * h * 4];
        for (int i = 0; i < px.Length; i++) px[i] = (byte)(i * 7);
        return px;
    }

    [Fact]
    public void ALookSurvivesTheRoundTrip()
    {
        var look = "{\"Exposure\":0.25,\"Name\":\"Slow Burn\"}";
        var png = Png.Encode(8, 4, Pixels(8, 4), look);

        Assert.True(Png.TryReadEmbeddedText(png, out var back));
        Assert.Equal(look, back);
    }

    [Fact]
    public void NonAsciiSurvives()
    {
        var look = "{\"Name\":\"Tempe — The Red That Follows\",\"Note\":\"café ‘soft’ 日本\"}";
        var png = Png.Encode(8, 4, Pixels(8, 4), look);

        Assert.True(Png.TryReadEmbeddedText(png, out var back));
        Assert.Equal(look, back);
    }

    [Fact]
    public void ALargeLookStillFitsAndCompresses()
    {
        var look = string.Concat(Enumerable.Range(0, 2000).Select(i => $"\"Prop{i}\":0.5,"));
        var png = Png.Encode(16, 16, Pixels(16, 16), look);

        Assert.True(Png.TryReadEmbeddedText(png, out var back));
        Assert.Equal(look, back);
        Assert.True(png.Length < look.Length, "the embedded look should compress well below its raw size");
    }

    [Fact]
    public void PartialAlphaSurvivesTheEncoder()
    {
        const int w = 4, h = 1;
        var px = new byte[w * h * 4];
        byte[] alphas = { 0, 64, 191, 255 };
        for (int i = 0; i < w; i++)
        {
            px[i * 4] = 200; px[i * 4 + 1] = 100; px[i * 4 + 2] = 50;
            px[i * 4 + 3] = alphas[i];
        }

        var png = Png.Encode(w, h, px);
        var back = DecodeRgba(png, w, h);

        for (int i = 0; i < w; i++)
        {
            Assert.Equal(alphas[i], back[i * 4 + 3]);
            Assert.Equal(200, back[i * 4]);
        }
    }

    private static byte[] DecodeRgba(byte[] png, int w, int h)
    {
        var idat = new System.IO.MemoryStream();
        int i = 8;
        while (i + 12 <= png.Length)
        {
            int len = (png[i] << 24) | (png[i + 1] << 16) | (png[i + 2] << 8) | png[i + 3];
            var type = Encoding.ASCII.GetString(png, i + 4, 4);
            if (type == "IDAT") idat.Write(png, i + 8, len);
            if (type == "IEND") break;
            i += 12 + len;
        }
        idat.Position = 0;
        using var z = new System.IO.Compression.ZLibStream(idat, System.IO.Compression.CompressionMode.Decompress);
        using var raw = new System.IO.MemoryStream();
        z.CopyTo(raw);
        var data = raw.ToArray();

        var outp = new byte[w * h * 4];
        int stride = w * 4;
        for (int y = 0; y < h; y++)
        {
            int src = y * (stride + 1);
            byte filter = data[src];
            for (int x = 0; x < stride; x++)
            {
                int v = data[src + 1 + x];
                int a = x >= 4 ? outp[y * stride + x - 4] : 0;
                int b = y > 0 ? outp[(y - 1) * stride + x] : 0;
                int cc = (x >= 4 && y > 0) ? outp[(y - 1) * stride + x - 4] : 0;
                int res = filter switch
                {
                    0 => v,
                    1 => v + a,
                    2 => v + b,
                    3 => v + ((a + b) >> 1),
                    4 => v + Paeth(a, b, cc),
                    _ => v,
                };
                outp[y * stride + x] = (byte)res;
            }
        }
        return outp;
    }

    private static int Paeth(int a, int b, int c)
    {
        int p = a + b - c, pa = Math.Abs(p - a), pb = Math.Abs(p - b), pc = Math.Abs(p - c);
        return (pa <= pb && pa <= pc) ? a : (pb <= pc ? b : c);
    }

    [Fact]
    public void AnImageWithoutALookReportsNothingFound()
    {
        var png = Png.Encode(4, 4, Pixels(4, 4));
        Assert.False(Png.TryReadEmbeddedText(png, out var back));
        Assert.Equal("", back);
    }

    [Fact]
    public void EmbeddingDoesNotDisturbTheImageItself()
    {
        var px = Pixels(9, 5);
        var plain = Png.Encode(9, 5, px);
        var tagged = Png.Encode(9, 5, px, "{\"a\":1}");

        Assert.True(tagged.Length > plain.Length);
        Assert.Equal(FindIdat(plain), FindIdat(tagged));
    }

    private static string FindIdat(byte[] png)
    {
        var sb = new StringBuilder();
        int i = 8;
        while (i + 12 <= png.Length)
        {
            int len = (png[i] << 24) | (png[i + 1] << 16) | (png[i + 2] << 8) | png[i + 3];
            var type = Encoding.ASCII.GetString(png, i + 4, 4);
            if (type == "IDAT") sb.Append(Convert.ToHexString(png, i + 8, len));
            if (type == "IEND") break;
            i += 12 + len;
        }
        return sb.ToString();
    }

    [Theory]
    [InlineData(new byte[0])]
    [InlineData(new byte[] { 1, 2, 3 })]
    [InlineData(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A })]
    public void RubbishInputIsRefusedWithoutThrowing(byte[] bytes)
    {
        Assert.False(Png.TryReadEmbeddedText(bytes, out _));
    }

    [Fact]
    public void NullIsRefusedWithoutThrowing()
    {
        Assert.False(Png.TryReadEmbeddedText(null, out _));
    }

    [Fact]
    public void ALyingChunkLengthIsRefusedRatherThanRead()
    {
        var png = Png.Encode(8, 4, Pixels(8, 4), "{\"a\":1}");
        png[8] = 0x7F; png[9] = 0xFF; png[10] = 0xFF; png[11] = 0xFF;

        Assert.False(Png.TryReadEmbeddedText(png, out _));
    }

    [Fact]
    public void TruncationIsRefusedRatherThanRead()
    {
        var png = Png.Encode(8, 4, Pixels(8, 4), "{\"a\":1}");
        var cut = new byte[png.Length / 2];
        Array.Copy(png, cut, cut.Length);

        Assert.False(Png.TryReadEmbeddedText(cut, out _));
    }

    [Fact]
    public void CorruptCompressedPayloadIsRefusedRatherThanThrowing()
    {
        var png = Png.Encode(8, 4, Pixels(8, 4), "{\"Exposure\":0.25}");

        int i = 8;
        while (i + 12 <= png.Length)
        {
            int len = (png[i] << 24) | (png[i + 1] << 16) | (png[i + 2] << 8) | png[i + 3];
            if (Encoding.ASCII.GetString(png, i + 4, 4) == "iTXt")
            {
                for (int k = i + 8 + len / 2; k < i + 8 + len; k++) png[k] ^= 0xFF;
                break;
            }
            i += 12 + len;
        }

        Assert.False(Png.TryReadEmbeddedText(png, out _));
    }
}
