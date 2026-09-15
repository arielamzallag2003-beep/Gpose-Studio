using System;
using System.Collections.Generic;
using System.Numerics;

namespace GPoseStudio;

public static class MaskGeometry
{
    public static float Sides(float s) => s >= 3f ? MathF.Floor(s + 0.5f) : 5f;
    public static float Detail(float d) => d > 0.001f ? d : 0.5f;

    private static float Frac(float x) => x - MathF.Floor(x);

    private static float BoxOut(float x, float y, float hx, float hy)
    {
        float qx = MathF.Abs(x) - hx, qy = MathF.Abs(y) - hy;
        float ox = MathF.Max(qx, 0f), oy = MathF.Max(qy, 0f);
        return MathF.Sqrt(ox * ox + oy * oy) + MathF.Min(MathF.Max(qx, qy), 0f);
    }

    private static float PolyOut(float x, float y, float r, float n)
    {
        float seg = MathF.Tau / n;
        float a = MathF.Atan2(y, x + 1e-7f) - MathF.PI / 2f;
        a -= seg * MathF.Floor(a / seg + 0.5f);
        float l = MathF.Sqrt(x * x + y * y);
        float qx = l * MathF.Cos(a), qy = l * MathF.Abs(MathF.Sin(a));
        float he = r * MathF.Cos(seg * 0.5f), hw = r * MathF.Sin(seg * 0.5f);
        return qy > hw ? MathF.Sqrt((qx - he) * (qx - he) + (qy - hw) * (qy - hw)) : qx - he;
    }

    private static float StarOut(float x, float y, float r, float n, float k)
    {
        float seg = MathF.Tau / n;
        float a = MathF.Atan2(x + 1e-7f, -y);
        a = MathF.Abs(a - seg * MathF.Floor(a / seg + 0.5f));
        float l = MathF.Sqrt(x * x + y * y);
        float qx = l * MathF.Sin(a), qy = l * MathF.Cos(a);
        float ax = 0f, ay = r;
        float bx = r * k * MathF.Sin(seg * 0.5f), by = r * k * MathF.Cos(seg * 0.5f);
        float abx = bx - ax, aby = by - ay, aqx = qx - ax, aqy = qy - ay;
        float t = Math.Clamp((aqx * abx + aqy * aby) / MathF.Max(abx * abx + aby * aby, 1e-8f), 0f, 1f);
        float dx = aqx - abx * t, dy = aqy - aby * t;
        float dist = MathF.Sqrt(dx * dx + dy * dy);
        return abx * aqy - aby * aqx < 0f ? -dist : dist;
    }

    public static float Inside(int mode, float x, float y, float size, float ell, float sides, float detail, float round)
    {
        float sz = MathF.Max(size, 1e-4f), n = Sides(sides), dt = Detail(detail), rnd = Math.Clamp(round, 0f, 1f);
        switch (mode)
        {
            case 1:
            {
                float ey = y / MathF.Max(ell, 0.05f);
                return sz - MathF.Sqrt(x * x + ey * ey);
            }
            case 4:
            {
                float hx = sz, hy = MathF.Max(size * ell, 1e-4f);
                float rc = rnd * MathF.Min(hx, hy);
                return -(BoxOut(x, y, hx - rc, hy - rc) - rc);
            }
            case 5:
            {
                float rr = MathF.Sqrt(x * x + y * y), inner = sz * Math.Clamp(ell, 0f, 1f);
                return MathF.Min(sz - rr, rr - inner);
            }
            case 9:
            {
                float ey = y / MathF.Max(ell, 0.05f), cosn = MathF.Cos(MathF.PI / n);
                float rc = rnd * sz * cosn;
                return -(PolyOut(x, ey, MathF.Max(sz - rc / cosn, 1e-4f), n) - rc);
            }
            case 10:
            {
                float ey = y / MathF.Max(ell, 0.05f), rc = rnd * sz * 0.25f;
                return -(StarOut(x, ey, MathF.Max(sz - rc, 1e-4f), n, Math.Clamp(dt, 0.05f, 0.98f)) - rc);
            }
            case 11:
            {
                float la = MathF.Sqrt(x * x + y * y);
                float da = Math.Clamp(dt, 0f, 1f) * MathF.PI - MathF.Abs(MathF.Atan2(y, x + 1e-7f));
                return MathF.Min(la * MathF.Sin(Math.Clamp(da, -MathF.PI / 2f, MathF.PI / 2f)), sz - la);
            }
            case 12:
            {
                float u = Frac(x / sz + 0.5f) - 0.5f;
                return (Math.Clamp(dt, 0.02f, 0.98f) * 0.5f - MathF.Abs(u)) * sz;
            }
            case 13:
            {
                float ht = sz * Math.Clamp(dt, 0.02f, 1f) * 0.5f;
                float rc = rnd * ht;
                float arms = MathF.Min(BoxOut(x, y, sz - rc, ht - rc), BoxOut(x, y, ht - rc, MathF.Max(size * ell, ht) - rc));
                return -(arms - rc);
            }
            default:
                return float.NaN;
        }
    }

    public static List<Vector2> Contour(int mode, float level, float size, float ell, float sides, float detail, float round, int steps = 180)
    {
        var pts = new List<Vector2>(steps + 1);
        if (!(Inside(mode, 0f, 0f, size, ell, sides, detail, round) > level)) return pts;
        float reach = MathF.Max(size, 1e-3f) * MathF.Max(ell, 1f) * 2f + MathF.Abs(level) + 0.05f;
        for (int k = 0; k <= steps; k++)
        {
            float th = k * MathF.Tau / steps, dx = MathF.Cos(th), dy = MathF.Sin(th);
            float lo = 0f, hi = reach;
            for (int it = 0; it < 28; it++)
            {
                float mid = (lo + hi) * 0.5f;
                if (Inside(mode, dx * mid, dy * mid, size, ell, sides, detail, round) > level) lo = mid; else hi = mid;
            }
            pts.Add(new Vector2(dx * lo, dy * lo));
        }
        return pts;
    }

    public static List<Vector2> Wedge(float size, float detail, int steps = 48)
    {
        float half = Math.Clamp(Detail(detail), 0f, 1f) * MathF.PI, reach = MathF.Max(size, 1e-3f);
        var pts = new List<Vector2>(steps + 3) { Vector2.Zero };
        for (int k = 0; k <= steps; k++)
        {
            float a = -half + 2f * half * k / steps;
            pts.Add(new Vector2(MathF.Cos(a) * reach, MathF.Sin(a) * reach));
        }
        pts.Add(Vector2.Zero);
        return pts;
    }
}
