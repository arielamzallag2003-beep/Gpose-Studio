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
