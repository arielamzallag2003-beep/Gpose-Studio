using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.Versioning;

namespace GPoseStudio;

[SupportedOSPlatform("windows")]
internal static class TextRender
{
    internal readonly struct Raster
    {
        public readonly int W, H;
        public readonly byte[] Rgba;
        public readonly int OffX, OffY;
        public Raster(int w, int h, byte[] rgba, int offX, int offY)
        { W = w; H = h; Rgba = rgba; OffX = offX; OffY = offY; }
        public bool IsEmpty => W <= 0 || H <= 0 || Rgba is null;
    }

    public static float PixelSize(TextMarker t, float frameH)
        => Math.Clamp(t.Size, 0.002f, 0.5f) * Math.Max(frameH, 1f);

    private static string[]? _families;
    public static string[] Families
    {
        get
        {
            if (_families != null) return _families;
            try
            {
                var fams = FontFamily.Families;
                var names = new List<string>(fams.Length);
                foreach (var f in fams) { names.Add(f.Name); f.Dispose(); }
                names.Sort(StringComparer.OrdinalIgnoreCase);
                _families = names.ToArray();
            }
            catch { _families = Array.Empty<string>(); }
            return _families;
        }
    }

    private static FontFamily Family(string? name)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            try { return new FontFamily(name); }
            catch {  }
        }
        return FontFamily.GenericSansSerif;
    }

    private static GraphicsPath BuildText(TextMarker t, FontFamily fam, FontStyle style, float em, Graphics g)
    {
        var path = new GraphicsPath();
        var fmt = StringFormat.GenericTypographic;
        fmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

        var lines = (t.Text ?? "").Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        float lineH = em * Math.Clamp(t.LineHeight, 0.5f, 3f);
        float track = em * Math.Clamp(t.Tracking, -0.2f, 1f);

        var widths = new float[lines.Length];
        float widest = 0f;
        for (int i = 0; i < lines.Length; i++)
        {
            widths[i] = LineWidth(lines[i], fam, style, em, track, g, fmt);
            widest = Math.Max(widest, widths[i]);
        }

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (line.Length == 0) continue;
            float x = t.Align switch { 1 => (widest - widths[i]) * 0.5f, 2 => widest - widths[i], _ => 0f };
            float y = i * lineH;

            if (track == 0f)
            {
                path.AddString(line, fam, (int)style, em, new PointF(x, y), fmt);
            }
            else
            {
                foreach (var ch in line)
                {
                    var s = ch.ToString();
                    if (ch != ' ') path.AddString(s, fam, (int)style, em, new PointF(x, y), fmt);
                    x += Advance(s, fam, style, em, g, fmt) + track;
                }
            }
        }
        return path;
    }

    private static float LineWidth(string line, FontFamily fam, FontStyle style, float em,
                                   float track, Graphics g, StringFormat fmt)
    {
        if (line.Length == 0) return 0f;
        if (track == 0f) return Advance(line, fam, style, em, g, fmt);
        float w = 0f;
        foreach (var ch in line) w += Advance(ch.ToString(), fam, style, em, g, fmt) + track;
        return Math.Max(w - track, 0f);
    }

    private static float Advance(string s, FontFamily fam, FontStyle style, float em, Graphics g, StringFormat fmt)
    {
        try
        {
            using var font = new Font(fam, em, style, GraphicsUnit.Pixel);
            return g.MeasureString(s, font, PointF.Empty, fmt).Width;
        }
        catch { return em * 0.5f * s.Length; }
    }

    public static Raster Rasterise(TextMarker t, float pxSize)
    {
        if (t is null || string.IsNullOrEmpty(t.Text)) return default;
        float em = Math.Clamp(pxSize, 4f, 4000f);

        FontFamily? fam = null;
        try
        {
            fam = Family(t.Font);
            var style = FontStyle.Regular;
            if (t.Bold) style |= FontStyle.Bold;
            if (t.Italic) style |= FontStyle.Italic;
            if (!fam.IsStyleAvailable(style)) style = FontStyle.Regular;

            using var probe = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
            using var pg = Graphics.FromImage(probe);
            using var text = BuildText(t, fam, style, em, pg);
            var tb = text.GetBounds();
            if (tb.Width <= 0.01f || tb.Height <= 0.01f) return default;

            float platePad = em * Math.Clamp(t.PlatePad, 0f, 2f);
            bool hasPlate = t.Plate > 0.002f;
            using var plate = new GraphicsPath();
            if (hasPlate)
            {
                var r = RectangleF.Inflate(tb, platePad, platePad * 0.7f);
                float round = Math.Min(r.Width, r.Height) * 0.5f * Math.Clamp(t.PlateRound, 0f, 1f);
                AddRoundedRect(plate, r, round);
            }

            float rot = t.Rotation;
            if (Math.Abs(rot) > 0.0005f)
            {
                using var m = new Matrix();
                var pivot = new PointF(tb.X + tb.Width * 0.5f, tb.Y + tb.Height * 0.5f);
                m.RotateAt(rot * 180f / (float)Math.PI, pivot);
                text.Transform(m);
                if (hasPlate) plate.Transform(m);
            }

            float outline = t.Outline ? Math.Max(1f, em * Math.Clamp(t.OutlineWidth, 0.01f, 0.30f)) : 0f;
            float shDist = t.ShadowAmount > 0.002f ? em * Math.Clamp(t.ShadowDist, 0f, 0.6f) : 0f;
            float shSoft = t.ShadowAmount > 0.002f ? em * Math.Clamp(t.ShadowSoft, 0f, 0.5f) : 0f;

            float grow = outline * 0.5f + shDist + shSoft * 2f + (hasPlate ? platePad : 0f) + 2f;
            var all = text.GetBounds();
            if (hasPlate) all = RectangleF.Union(all, plate.GetBounds());
            all = RectangleF.Inflate(all, grow, grow);

            int w = (int)Math.Ceiling(all.Width), h = (int)Math.Ceiling(all.Height);
            if (w <= 0 || h <= 0 || (long)w * h > 64L * 1024 * 1024 / 4) return default;

            using (var m = new Matrix())
            {
                m.Translate(-all.X, -all.Y);
                text.Transform(m);
                if (hasPlate) plate.Transform(m);
            }

            byte A(float v) => (byte)Math.Clamp((int)(v * 255f + 0.5f), 0, 255);
            float alpha = Math.Clamp(t.A, 0f, 1f);
            var outp = new byte[w * h * 4];

            if (hasPlate)
                Over(outp, Fill(w, h, g => { using var b = new SolidBrush(Color.FromArgb(
                        A(alpha * Math.Clamp(t.Plate, 0f, 1f)), A(t.PlateR), A(t.PlateG), A(t.PlateB)));
                    g.FillPath(b, plate); }), w, h);

            if (shDist > 0f || shSoft > 0f || t.ShadowAmount > 0.002f)
            {
                float ang = t.ShadowAngle;
                float dx = (float)Math.Cos(ang) * shDist, dy = (float)Math.Sin(ang) * shDist;
                var sh = Fill(w, h, g =>
                {
                    using var m2 = new Matrix();
                    m2.Translate(dx, dy);
                    using var moved = (GraphicsPath)text.Clone();
                    moved.Transform(m2);
                    using var b = new SolidBrush(Color.FromArgb(
                        A(alpha * Math.Clamp(t.ShadowAmount, 0f, 1f)), A(t.ShadowR), A(t.ShadowG), A(t.ShadowB)));
                    if (outline > 0f)
                    {
                        using var pen = new Pen(b.Color, outline) { LineJoin = LineJoin.Round, MiterLimit = 2f };
                        g.DrawPath(pen, moved);
                    }
                    g.FillPath(b, moved);
                });
                if (shSoft >= 1f) BlurAlpha(sh, w, h, (int)Math.Round(shSoft));
                Over(outp, sh, w, h);
            }

            Over(outp, Fill(w, h, g =>
            {
                if (outline > 0f)
                {
                    using var pen = new Pen(Color.FromArgb(A(alpha * 0.92f),
                            A(t.OutlineR), A(t.OutlineG), A(t.OutlineB)), outline)
                    { LineJoin = LineJoin.Round, MiterLimit = 2f };
                    g.DrawPath(pen, text);
                }
                using var brush = new SolidBrush(Color.FromArgb(A(alpha), A(t.R), A(t.G), A(t.B)));
                g.FillPath(brush, text);
            }), w, h);

            return new Raster(w, h, outp, -w / 2, -h / 2);
        }
        catch (Exception ex)
        {
            Services.Log.Warning($"could not render caption text: {ex.Message}");
            return default;
        }
        finally { fam?.Dispose(); }
    }

    private static void AddRoundedRect(GraphicsPath p, RectangleF r, float rad)
    {
        rad = Math.Max(rad, 0f);
        if (rad <= 0.5f) { p.AddRectangle(r); return; }
        float d = rad * 2f;
        p.AddArc(r.X, r.Y, d, d, 180, 90);
        p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        p.CloseFigure();
    }

    private static byte[] Fill(int w, int h, Action<Graphics> draw)
    {
        using var bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.Clear(Color.Transparent);
            draw(g);
        }
        var data = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        var rgba = new byte[w * h * 4];
        try
        {
            unsafe
            {
                byte* src = (byte*)data.Scan0;
                for (int y = 0; y < h; y++)
                {
                    byte* row = src + (long)y * data.Stride;
                    int d = y * w * 4;
                    for (int x = 0; x < w; x++)
                    {
                        rgba[d + x * 4]     = row[x * 4 + 2];
                        rgba[d + x * 4 + 1] = row[x * 4 + 1];
                        rgba[d + x * 4 + 2] = row[x * 4];
                        rgba[d + x * 4 + 3] = row[x * 4 + 3];
                    }
                }
            }
        }
        finally { bmp.UnlockBits(data); }
        return rgba;
    }

    private static void BlurAlpha(byte[] rgba, int w, int h, int radius)
    {
        radius = Math.Clamp(radius, 1, 64);
        var a = new byte[w * h];
        for (int i = 0; i < w * h; i++) a[i] = rgba[i * 4 + 3];
        var tmp = new byte[w * h];
        for (int pass = 0; pass < 2; pass++)
        {
            BoxH(a, tmp, w, h, radius);
            BoxV(tmp, a, w, h, radius);
        }
        for (int i = 0; i < w * h; i++) rgba[i * 4 + 3] = a[i];
    }

    private static void BoxH(byte[] src, byte[] dst, int w, int h, int r)
    {
        int n = r * 2 + 1;
        for (int y = 0; y < h; y++)
        {
            int row = y * w, sum = 0;
            for (int x = -r; x <= r; x++) sum += src[row + Math.Clamp(x, 0, w - 1)];
            for (int x = 0; x < w; x++)
            {
                dst[row + x] = (byte)(sum / n);
                sum += src[row + Math.Clamp(x + r + 1, 0, w - 1)] - src[row + Math.Clamp(x - r, 0, w - 1)];
            }
        }
    }

    private static void BoxV(byte[] src, byte[] dst, int w, int h, int r)
    {
        int n = r * 2 + 1;
        for (int x = 0; x < w; x++)
        {
            int sum = 0;
            for (int y = -r; y <= r; y++) sum += src[Math.Clamp(y, 0, h - 1) * w + x];
            for (int y = 0; y < h; y++)
            {
                dst[y * w + x] = (byte)(sum / n);
                sum += src[Math.Clamp(y + r + 1, 0, h - 1) * w + x] - src[Math.Clamp(y - r, 0, h - 1) * w + x];
            }
        }
    }

    private static void Over(byte[] dst, byte[] src, int w, int h)
    {
        for (int i = 0; i < w * h; i++)
        {
            int s = i * 4, a = src[s + 3];
            if (a == 0) continue;
            int da = dst[s + 3];
            if (da == 0 || a == 255)
            {
                dst[s] = src[s]; dst[s + 1] = src[s + 1]; dst[s + 2] = src[s + 2];
                dst[s + 3] = (byte)Math.Max(a, da);
                continue;
            }
            int outA = a + da * (255 - a) / 255;
            if (outA == 0) continue;
            for (int k = 0; k < 3; k++)
                dst[s + k] = (byte)((src[s + k] * a * 255 + dst[s + k] * da * (255 - a)) / (outA * 255));
            dst[s + 3] = (byte)outA;
        }
    }

    public static void Compose(byte[] rgba, int w, int h, IReadOnlyList<TextMarker> texts,
                               float cropX0, float cropY0, float cropX1, float cropY1)
    {
        if (rgba is null || texts is null || w <= 0 || h <= 0) return;
        float cw = Math.Max(cropX1 - cropX0, 1e-4f), ch = Math.Max(cropY1 - cropY0, 1e-4f);
        float frameH = h / ch;

        foreach (var t in texts)
        {
            if (t is null || string.IsNullOrEmpty(t.Text) || t.A <= 0.002f) continue;
            var r = Rasterise(t, PixelSize(t, frameH));
            if (r.IsEmpty) continue;

            int ax = (int)Math.Round((t.X - cropX0) / cw * w);
            int ay = (int)Math.Round((t.Y - cropY0) / ch * h);
            Blit(rgba, w, h, r, ax + r.OffX, ay + r.OffY);
        }
    }

    internal static void Blit(byte[] dst, int dw, int dh, in Raster r, int x0, int y0)
    {
        for (int y = 0; y < r.H; y++)
        {
            int ty = y0 + y;
            if (ty < 0 || ty >= dh) continue;
            for (int x = 0; x < r.W; x++)
            {
                int tx = x0 + x;
                if (tx < 0 || tx >= dw) continue;
                int s = (y * r.W + x) * 4;
                int a = r.Rgba[s + 3];
                if (a == 0) continue;
                int d = (ty * dw + tx) * 4;
                if (a == 255)
                {
                    dst[d] = r.Rgba[s]; dst[d + 1] = r.Rgba[s + 1]; dst[d + 2] = r.Rgba[s + 2];
                }
                else
                {
                    int ia = 255 - a;
                    dst[d]     = (byte)((r.Rgba[s]     * a + dst[d]     * ia) / 255);
                    dst[d + 1] = (byte)((r.Rgba[s + 1] * a + dst[d + 1] * ia) / 255);
                    dst[d + 2] = (byte)((r.Rgba[s + 2] * a + dst[d + 2] * ia) / 255);
                }
                if (a > dst[d + 3]) dst[d + 3] = (byte)a;
            }
        }
    }
}
