using System;
using System.Collections.Generic;
using System.IO;
using GPoseStudio;
using Xunit;

public static class MaskPreview
{
    public const int W = 480, H = 270;
    private static float Asp => (float)W / H;
    private const float Start = 0.03f, Soft = 0.012f;

    private static float Frac(float x) => x - MathF.Floor(x);
    private static float S(float e0, float e1, float x)
    {
        float t = Math.Clamp((x - e0) / (e1 - e0), 0f, 1f);
        return t * t * (3f - 2f * t);
    }

    private static float Hash(float x, float y) => Frac(MathF.Sin(x * 127.1f + y * 311.7f) * 43758.5453f);
    private static float VNoise(float x, float y)
    {
        float ix = MathF.Floor(x), iy = MathF.Floor(y), fx = x - ix, fy = y - iy;
        float ux = fx * fx * (3f - 2f * fx), uy = fy * fy * (3f - 2f * fy);
        float a = Hash(ix, iy), b = Hash(ix + 1, iy), c = Hash(ix, iy + 1), d = Hash(ix + 1, iy + 1);
        return (a + (b - a) * ux) + ((c + (d - c) * ux) - (a + (b - a) * ux)) * uy;
    }
    private static float Fbm(float x, float y)
    {
        float v = 0f, a = 0.5f;
        for (int o = 0; o < 4; o++) { v += a * VNoise(x, y); x = x * 2.03f + 1.7f; y = y * 2.03f + 1.7f; a *= 0.5f; }
        return v / 0.9375f;
    }

    private static (float r, float g, float b) Scene(float u, float v)
    {
        float r = 0.10f + 0.35f * v, g = 0.18f + 0.22f * v, b = 0.42f - 0.15f * v;
        float lamp = MathF.Exp(-((u - 0.82f) * (u - 0.82f) * Asp * Asp + (v - 0.25f) * (v - 0.25f)) / 0.01f);
        r += lamp * 0.9f; g += lamp * 0.7f; b += lamp * 0.3f;
        if (u > 0.08f && u < 0.2f && v > 0.15f && v < 0.55f) { r = 0.85f; g = 0.12f; b = 0.25f; }
        float stripe = Frac((u * Asp + v) * 6f) < 0.5f ? 0.03f : 0f;
        r += stripe; g += stripe; b += stripe;
        if (Depth(u, v) < Start)
        {
            float shade = 0.55f + 0.4f * (1f - v);
            r = 0.78f * shade; g = 0.62f * shade; b = 0.55f * shade;
            if (MathF.Abs((u - 0.3f) * Asp * MathF.Sin(-0.2f) + (v - 0.62f) * MathF.Cos(-0.2f)) < 0.012f && u < 0.5f) { r = 0.8f; g = 0.82f; b = 0.9f; }
        }
        return (Math.Clamp(r, 0f, 1f), Math.Clamp(g, 0f, 1f), Math.Clamp(b, 0f, 1f));
    }

    private static float Depth(float u, float v)
    {
        float x = u * Asp, y = v;
        float hx = x - 0.58f * Asp, hy = y - 0.34f;
        if (hx * hx + hy * hy < 0.11f * 0.11f) return 0.01f;
        float bx = MathF.Abs(x - 0.58f * Asp) - 0.12f, by = MathF.Abs(y - 0.80f) - 0.36f;
        if (MathF.Max(bx, by) < 0f) return 0.012f;
        float ca = MathF.Cos(-0.2f), sa = MathF.Sin(-0.2f), dx = x - 0.30f * Asp, dy = y - 0.62f;
        float lx = dx * ca + dy * sa, ly = -dx * sa + dy * ca;
        if (MathF.Abs(lx) < 0.5f && MathF.Abs(ly) < 0.012f && x < 0.58f * Asp) return 0.011f;
        return 0.6f;
    }

    private static float Luma((float r, float g, float b) c) => c.r * 0.299f + c.g * 0.587f + c.b * 0.114f;
    private static float Sat((float r, float g, float b) c)
    {
        float mx = MathF.Max(c.r, MathF.Max(c.g, c.b)), mn = MathF.Min(c.r, MathF.Min(c.g, c.b));
        return mx <= 1e-5f ? 0f : (mx - mn) / mx;
    }

    private static float BaseSd(PluginConfig c, int k, int mode, float u, float v, float lin, (float r, float g, float b) src)
    {
        float dx = (u - c.MaskCx(k)) * Asp, dy = v - c.MaskCy(k);
        float ang = c.MaskAngle(k), ca = MathF.Cos(ang), sa = MathF.Sin(ang);
        float rx = dx * ca + dy * sa, ry = -dx * sa + dy * ca;
        float size = c.MaskSize(k), ell = c.MaskEllipse(k), sz = MathF.Max(size, 1e-4f);
        switch (mode)
        {
            case 2: return -(dx * ca + dy * sa);
            case 3: return sz - MathF.Abs(lin - c.MaskCy(k));
            case 6: return sz - MathF.Abs(Luma(src) - c.MaskCy(k));
            case 8: return Start - lin;
            case 15: return sz - MathF.Abs(Sat(src) - c.MaskCy(k));
            case 16:
            {
                float ex = rx / sz, ey = ry / MathF.Max(ell, 0.05f) / sz;
                return (Fbm(ex + 17.3f, ey + 5.1f) - (1f - Math.Clamp(MaskGeometry.Detail(c.MaskDetail(k)), 0f, 1f))) * sz;
            }
            default:
            {
                float s = MaskGeometry.Inside(mode, rx, ry, size, ell, c.MaskSides(k), c.MaskDetail(k), c.MaskRound(k));
                return float.IsNaN(s) ? 1f : s;
            }
        }
    }

    public static float Sd(PluginConfig c, int k, float u, float v, float lin, (float r, float g, float b) src)
    {
        int mode = c.MaskMode(k);
        float best = BaseSd(c, k, mode, u, v, lin, src);
        int mir = PluginConfig.MaskPlaceable(mode) ? c.MaskMirror(k) : 0;
        for (int cp = 1; cp < 4; cp++)
        {
            if ((cp & mir) != cp) continue;
            float s2 = BaseSd(c, k, mode, (cp & 1) != 0 ? 1f - u : u, (cp & 2) != 0 ? 1f - v : v, lin, src);
            best = MathF.Max(best, s2);
        }
        if (c.MaskRough(k) > 0f && PluginConfig.MaskPlaceable(mode))
        {
            float sc = c.MaskRoughScale(k) > 0.01f ? c.MaskRoughScale(k) : 8f;
            best += (Fbm(u * Asp * sc + k * 17.3f, v * sc + k * 5.7f) - 0.5f) * c.MaskRough(k);
        }
        return best;
    }

    public static float Coverage(PluginConfig c, int k, float u, float v)
    {
        int mode = c.MaskMode(k);
        if (mode == 0) return 1f;
        float lin = Depth(u, v);
        var src = Scene(u, v);
        float m;
        if (mode == 14)
        {
            float r = MathF.Max(c.MaskSize(k), 0.002f);
            float here = 1f - S(Start, Start + Soft, lin);
            bool inSubject = here > 0.5f;
            float f = Math.Clamp(c.MaskFeather(k), 0.0015f, r * 0.5f), reach = r + f, dNear = reach;
            for (int a = 0; a < 16; a++)
            {
                float th = a * MathF.PI / 8f, dx = MathF.Cos(th) / Asp, dy = MathF.Sin(th);
                for (int st = 1; st <= 8; st++)
                {
                    float rr = reach * st / 8f;
                    if (rr >= dNear) break;
                    float sv = 1f - S(Start, Start + Soft, Depth(u + dx * rr, v + dy * rr));
                    if ((sv > 0.5f) != inSubject) { dNear = rr; break; }
                }
            }
            float cov = 1f - S(r - f, r + f, dNear), w = Math.Clamp(MaskGeometry.Detail(c.MaskDetail(k)), 0f, 1f);
            m = cov * (inSubject ? here * Math.Clamp(2f * w, 0f, 1f) : (1f - here) * Math.Clamp(2f - 2f * w, 0f, 1f));
        }
        else if (mode == 7) m = 1f;
        else
        {
            float sd = Sd(c, k, u, v, lin, src), f = MathF.Max(c.MaskFeather(k), 1e-4f);
            m = c.MaskEdge(k) switch { 1 => S(0f, 2f * f, sd), 2 => S(-2f * f, 0f, sd), _ => S(-f, f, sd) };
        }
        if (c.MaskInvert(k)) m = 1f - m;
        m *= Math.Clamp(c.MaskStrength(k), 0f, 1f);
        int dl = c.MaskDepthLimit(k);
        if (dl != 0)
        {
            float subj = 1f - S(Start, Start + Soft, lin);
            m *= dl == 1 ? subj : 1f - subj;
        }
        return m;
    }

    private static (float r, float g, float b) Grade(PluginConfig v, (float r, float g, float b) c, float subj)
    {
        float e = MathF.Pow(2f, v.Exposure);
        float r = c.r * e, g = c.g * e, b = c.b * e;
        r += v.Temperature * 0.12f; b -= v.Temperature * 0.12f; g += v.Tint * 0.06f;
        float k = 1f + v.Contrast;
        r = (r - 0.5f) * k + 0.5f; g = (g - 0.5f) * k + 0.5f; b = (b - 0.5f) * k + 0.5f;
        float l = r * 0.299f + g * 0.587f + b * 0.114f, s = 1f + v.Saturation;
        r = l + (r - l) * s; g = l + (g - l) * s; b = l + (b - l) * s;
        if (v.EnBgFill && v.BgFill > 0f)
        {
            float amt = v.BgFill * (1f - subj);
            r += (v.BgFillR - r) * amt; g += (v.BgFillG - g) * amt; b += (v.BgFillB - b) * amt;
        }
        return (Math.Clamp(r, 0f, 1f), Math.Clamp(g, 0f, 1f), Math.Clamp(b, 0f, 1f));
    }

    public static byte[] Render(PluginConfig c)
    {
        var rgba = new byte[W * H * 4];
        var variants = new List<(int k, PluginConfig v)>();
        foreach (var k in c.MaskRegionStack())
        {
            if (!c.MaskRegionActive(k)) continue;
            var v = new PluginConfig();
            MaskRegions.BuildVariant(c, c.MaskOverrides(k), v);
            variants.Add((k, v));
        }
        var cov = new float[PluginConfig.MaskCount];
        for (int py = 0; py < H; py++)
        for (int px = 0; px < W; px++)
        {
            float u = (px + 0.5f) / W, v = (py + 0.5f) / H;
            float lin = Depth(u, v);
            var src = Scene(u, v);
            float subj = 1f - S(Start, Start + Soft, lin);
            for (int k = 0; k < PluginConfig.MaskCount; k++) cov[k] = Coverage(c, k, u, v);

            var col = src;
            foreach (var (k, vc) in variants)
            {
                var g = Grade(vc, src, subj);
                float a = cov[k] * Math.Clamp(c.MaskRegionMix(k), 0f, 1f);
                col = (col.r + (g.r - col.r) * a, col.g + (g.g - col.g) * a, col.b + (g.b - col.b) * a);
            }

            bool anyFrame = false; float keep = 0f;
            for (int k = 0; k < PluginConfig.MaskCount; k++)
                if (c.MaskFrame(k) && c.MaskMode(k) != 0) { anyFrame = true; keep = MathF.Max(keep, cov[k]); }
            if (anyFrame)
            {
                float vis = MathF.Max(keep, c.MaskBreakOut ? subj : 0f);
                float fa = (1f - vis) * Math.Clamp(c.MaskFillA, 0f, 1f);
                col = (col.r + (c.MaskFillR - col.r) * fa, col.g + (c.MaskFillG - col.g) * fa, col.b + (c.MaskFillB - col.b) * fa);
                for (int k = 0; k < PluginConfig.MaskCount; k++)
                {
                    if (!c.MaskFrame(k) || c.MaskShadow(k) <= 0f || !PluginConfig.MaskHasEdge(c.MaskMode(k))) continue;
                    float sdS = Sd(c, k, u - 0.010f / Asp, v - 0.014f, lin, src);
                    float dark = 1f - Math.Clamp(c.MaskShadow(k), 0f, 1f) * 0.75f * S(-0.018f, 0.012f, sdS) * (1f - vis);
                    col = (col.r * dark, col.g * dark, col.b * dark);
                }
                float behind = 1f - (c.MaskBreakOut ? subj : (c.MaskOutlineBehind ? subj * keep : 0f));
                float aa = 1.2f / H;
                for (int k = 0; k < PluginConfig.MaskCount; k++)
                {
                    int md = c.MaskMode(k);
                    float glow = Math.Clamp(c.MaskOutGlow(k), 0f, 1f), ow = c.MaskOutline(k);
                    if (!c.MaskFrame(k) || !PluginConfig.MaskHasEdge(md) || (ow <= 0f && glow <= 0f)) continue;
                    float sd = Sd(c, k, u, v, lin, src);
                    float ln = ow > 0f ? 1f - S(ow * 0.5f, ow * 0.5f + aa, MathF.Abs(sd)) : 0f;
                    if (glow > 0f) ln = MathF.Max(ln, glow * MathF.Exp(-MathF.Max(MathF.Abs(sd) - ow * 0.5f, 0f) / MathF.Max(ow * 3f, 0.006f)));
                    if (c.MaskFramesStacked)
                        for (int j = 0; j < PluginConfig.MaskCount; j++)
                            if (j != k && c.MaskFrame(j) && c.MaskMode(j) != 0 && c.MaskFrameRank(j) > c.MaskFrameRank(k)) ln *= 1f - cov[j];
                    var (orr, og, ob) = c.MaskOutColor(k);
                    float a = ln * behind;
                    col = (col.r + (orr - col.r) * a, col.g + (og - col.g) * a, col.b + (ob - col.b) * a);
                }
            }

            int o = (py * W + px) * 4;
            rgba[o] = (byte)(Math.Clamp(col.r, 0f, 1f) * 255f + 0.5f);
            rgba[o + 1] = (byte)(Math.Clamp(col.g, 0f, 1f) * 255f + 0.5f);
            rgba[o + 2] = (byte)(Math.Clamp(col.b, 0f, 1f) * 255f + 0.5f);
            rgba[o + 3] = 255;
        }
        return rgba;
    }

    public static void Write(string dir, string name, PluginConfig c)
        => File.WriteAllBytes(Path.Combine(dir, name + ".png"), Png.Encode(W, H, Render(c)));
}
