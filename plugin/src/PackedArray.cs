using System;

namespace GPoseStudio;

internal static class PackedArray
{
    public static float[] Widen(float[]? src, int length, int blocks,
                                int oldStride, int newStride, int copyPerBlock)
    {
        if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));

        var dst = new float[length];
        if (src == null || blocks <= 0 || copyPerBlock <= 0) return dst;
        if (oldStride < copyPerBlock || newStride < copyPerBlock) return dst;

        for (int b = 0; b < blocks; b++)
        {
            int from = b * oldStride, to = b * newStride;
            if (from + copyPerBlock > src.Length) break;
            if (to + copyPerBlock > dst.Length) break;
            Array.Copy(src, from, dst, to, copyPerBlock);
        }
        return dst;
    }
}
