using System;

namespace GPoseStudio;

internal static class Frame
{
    internal readonly struct Opts
    {
        public readonly float Corner, Mat, OuterCorner, Keyline, Shadow, Bottom;
        public readonly float MatR, MatG, MatB, KeyR, KeyG, KeyB;
        public readonly float Smooth;
        public readonly bool Alpha, Inset;
        public Opts(bool inset, float smooth, float corner, float mat, float outerCorner,
                    float keyline, float shadow, float bottom,
                    float matR, float matG, float matB,
                    float keyR, float keyG, float keyB, bool alpha)
        {
            Inset = inset;
            Smooth = Math.Clamp(smooth, 0f, 1f);
            Corner = Math.Clamp(corner, 0f, 0.5f);
            Mat = Math.Clamp(mat, 0f, 0.4f);
            OuterCorner = Math.Clamp(outerCorner, 0f, 0.5f);
            Keyline = Math.Clamp(keyline, 0f, 0.05f);
            Shadow = Math.Clamp(shadow, 0f, 1f);
            Bottom = Math.Clamp(bottom, 0f, 1f);
            MatR = matR; MatG = matG; MatB = matB;
            KeyR = keyR; KeyG = keyG; KeyB = keyB;
            Alpha = alpha;
        }
        public bool IsNoOp => Mat <= 0f && Corner <= 0f && Keyline <= 0f && Shadow <= 0f;
    }

    private static float Sd(float px, float py, float hx, float hy, float r, float n)
    {
        r = MathF.Min(r, MathF.Min(hx, hy));
        float dx = MathF.Abs(px) - (hx - r), dy = MathF.Abs(py) - (hy - r);
        float ax = MathF.Max(dx, 0f), ay = MathF.Max(dy, 0f);
        float corner;
        if (ax <= 0f) corner = ay;
        else if (ay <= 0f) corner = ax;
        else if (n <= 2.001f) corner = MathF.Sqrt(ax * ax + ay * ay);
        else corner = MathF.Pow(MathF.Pow(ax, n) + MathF.Pow(ay, n), 1f / n);
        return corner + MathF.Min(MathF.Max(dx, dy), 0f) - r;
    }

    private static float Cov(float sd) => Math.Clamp(0.5f - sd, 0f, 1f);

    public static (int w, int h, byte[] rgba) Compose(int w, int h, byte[] src, in Opts o, bool allowAlpha)
    {
        if (w <= 0 || h <= 0 || o.IsNoOp) return (w, h, src);

        float shortSide = Math.Min(w, h);
        int mat = (int)MathF.Round(o.Mat * shortSide);
        int bottom = (int)MathF.Round(mat * o.Bottom);

        int outW, outH, photoW, photoH, ox, oy;
        byte[] photo;
        if (o.Inset)
        {
            outW = w; outH = h;
            int innerW = Math.Max(1, w - 2 * mat), innerH = Math.Max(1, h - 2 * mat - bottom);
            float sc = MathF.Min((float)innerW / w, (float)innerH / h);
            photoW = Math.Clamp((int)MathF.Round(w * sc), 1, innerW);
            photoH = Math.Clamp((int)MathF.Round(h * sc), 1, innerH);
            ox = (outW - photoW) / 2;
            oy = mat + (innerH - photoH) / 2;
            photo = (photoW == w && photoH == h) ? src : BoxResample(src, w, h, photoW, photoH);
        }
        else
        {
            outW = w + 2 * mat; outH = h + 2 * mat + bottom;
            photoW = w; photoH = h; ox = mat; oy = mat;
            photo = src;
        }
        if ((long)outW * outH > 400_000_000L) return (w, h, src);

        float innerR = o.Corner * shortSide;
        float outerR = mat > 0 ? o.OuterCorner * shortSide : innerR;
        float key = o.Keyline * shortSide;
        float shRad = MathF.Max(mat * 0.62f, 1.5f);
        float cn = 2.0f + o.Smooth * 3.0f;
        float shDrop = MathF.Max(mat * 0.16f, 1f);

        var dst = new byte[(long)outW * outH * 4];

        float pcx = ox + photoW * 0.5f, pcy = oy + photoH * 0.5f;
        float phx = photoW * 0.5f, phy = photoH * 0.5f;
        float ocx = outW * 0.5f, ocy = outH * 0.5f;
        float ohx = outW * 0.5f, ohy = outH * 0.5f;

        bool opaqueOutside = !allowAlpha || !o.Alpha;

        for (int y = 0; y < outH; y++)
        {
            float py = y + 0.5f;
            int row = y * outW * 4;
            for (int x = 0; x < outW; x++)
            {
                float px = x + 0.5f;
                int di = row + x * 4;

                float sdP = Sd(px - pcx, py - pcy, phx, phy, innerR, cn);
                float covP = Cov(sdP);

                float bA = opaqueOutside ? 1f : Cov(Sd(px - ocx, py - ocy, ohx, ohy, outerR, cn));
                float bR = o.MatR, bG = o.MatG, bB = o.MatB;

                if (bA > 0f)
                {
                    if (o.Shadow > 0f)
                    {
                        float sdSh = Sd(px - pcx, py - pcy - shDrop, phx, phy, innerR, cn);
                        if (sdSh > 0f)
                        {
                            float s = MathF.Exp(-sdSh / shRad) * o.Shadow * 0.6f;
                            bR *= 1f - s; bG *= 1f - s; bB *= 1f - s;
                        }
                    }
                    if (key > 0f)
                    {
                        float ring = Math.Clamp(Cov(sdP - key) - covP, 0f, 1f);
                        bR += (o.KeyR - bR) * ring; bG += (o.KeyG - bG) * ring; bB += (o.KeyB - bB) * ring;
                    }
                }

                if (covP >= 1f)
                {
                    int si = ((y - oy) * photoW + (x - ox)) * 4;
                    dst[di] = photo[si]; dst[di + 1] = photo[si + 1]; dst[di + 2] = photo[si + 2]; dst[di + 3] = 255;
                    continue;
                }

                float pr = 0f, pg = 0f, pb = 0f;
                if (covP > 0f)
                {
                    int sx = Math.Clamp(x - ox, 0, photoW - 1), sy = Math.Clamp(y - oy, 0, photoH - 1);
                    int si = (sy * photoW + sx) * 4;
                    pr = photo[si] / 255f; pg = photo[si + 1] / 255f; pb = photo[si + 2] / 255f;
                }

                float outA = covP + bA * (1f - covP);
                float rr, gg, bb2;
                if (outA <= 1e-5f) { rr = bR; gg = bG; bb2 = bB; outA = 0f; }
                else
                {
                    float wb = bA * (1f - covP);
                    rr = (pr * covP + bR * wb) / outA;
                    gg = (pg * covP + bG * wb) / outA;
                    bb2 = (pb * covP + bB * wb) / outA;
                }

                dst[di] = To8(rr); dst[di + 1] = To8(gg); dst[di + 2] = To8(bb2);
                dst[di + 3] = (byte)Math.Clamp((int)MathF.Round(outA * 255f), 0, 255);
            }
        }
        return (outW, outH, dst);
    }

    private static byte To8(float v) => (byte)Math.Clamp((int)MathF.Round(v * 255f), 0, 255);

    private static byte[] BoxResample(byte[] src, int sw, int sh, int dw, int dh)
    {
        var dst = new byte[(long)dw * dh * 4];
        for (int y = 0; y < dh; y++)
        {
            int y0 = (int)((long)y * sh / dh), y1 = (int)(((long)y + 1) * sh / dh);
            if (y1 <= y0) y1 = y0 + 1;
            if (y1 > sh) y1 = sh;
            for (int x = 0; x < dw; x++)
            {
                int x0 = (int)((long)x * sw / dw), x1 = (int)(((long)x + 1) * sw / dw);
                if (x1 <= x0) x1 = x0 + 1;
                if (x1 > sw) x1 = sw;
                int r = 0, g = 0, b = 0, n = 0;
                for (int sy = y0; sy < y1; sy++)
                {
                    int row = sy * sw;
                    for (int sx = x0; sx < x1; sx++)
                    {
                        int i = (row + sx) * 4;
                        r += src[i]; g += src[i + 1]; b += src[i + 2]; n++;
                    }
                }
                int di = (y * dw + x) * 4;
                dst[di] = (byte)(r / n); dst[di + 1] = (byte)(g / n); dst[di + 2] = (byte)(b / n); dst[di + 3] = 255;
            }
        }
        return dst;
    }
}
