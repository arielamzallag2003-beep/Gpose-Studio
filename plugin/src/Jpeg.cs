using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace GPoseStudio;

[SupportedOSPlatform("windows")]
public static class Jpeg
{
    public static byte[] Encode(int width, int height, byte[] rgba, int quality)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("width/height must be positive");
        if (rgba.Length < (long)width * height * 4)
            throw new ArgumentException("rgba too small for dimensions");
        quality = Math.Clamp(quality, 1, 100);

        using var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        var data = bmp.LockBits(new Rectangle(0, 0, width, height),
                                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
        try
        {
            var row = new byte[width * 4];
            for (int y = 0; y < height; y++)
            {
                int src = y * width * 4;
                for (int x = 0; x < width; x++)
                {
                    int s = src + x * 4, d = x * 4;
                    row[d]     = rgba[s + 2];
                    row[d + 1] = rgba[s + 1];
                    row[d + 2] = rgba[s];
                    row[d + 3] = rgba[s + 3];
                }
                Marshal.Copy(row, 0, data.Scan0 + y * data.Stride, width * 4);
            }
        }
        finally { bmp.UnlockBits(data); }

        var codec = GetJpegCodec() ?? throw new InvalidOperationException("no JPEG encoder available");
        using var ep = new EncoderParameters(1);
        ep.Param[0] = new EncoderParameter(Encoder.Quality, (long)quality);
        using var ms = new MemoryStream();
        bmp.Save(ms, codec, ep);
        return ms.ToArray();
    }

    private static ImageCodecInfo? GetJpegCodec()
    {
        foreach (var c in ImageCodecInfo.GetImageEncoders())
            if (c.FormatID == ImageFormat.Jpeg.Guid) return c;
        return null;
    }
}
