using System.Numerics;
using Dalamud.Bindings.ImGui;

namespace GPoseStudio.Ui;

internal static class Logo
{
    private static readonly Vector2[] Outer =
    {
        new(54.00f, 0.00f), new(27.00f, 46.77f), new(-27.00f, 46.77f),
        new(-54.00f, 0.00f), new(-27.00f, -46.77f), new(27.00f, -46.77f),
    };

    private static readonly Vector2[] Inner =
    {
        new(18.12f, 14.16f), new(-3.20f, 22.78f), new(-21.33f, 8.62f),
        new(-18.12f, -14.16f), new(3.20f, -22.78f), new(21.33f, -8.62f),
    };

    private static readonly Vector4[] Colors =
    {
        new(1.00f, 0.48f, 0.24f, 0.94f),
        new(1.00f, 0.30f, 0.55f, 0.94f),
        new(0.69f, 0.30f, 1.00f, 0.94f),
        new(0.30f, 0.48f, 1.00f, 0.94f),
        new(0.17f, 0.85f, 0.77f, 0.94f),
        new(0.55f, 0.90f, 0.36f, 0.94f),
    };

    public static void Draw(ImDrawListPtr dl, Vector2 center, float radius)
    {
        float s = radius / 54f;
        uint dark = ImGui.GetColorU32(new Vector4(0.055f, 0.075f, 0.125f, 1f));
        uint edge = ImGui.GetColorU32(new Vector4(0.04f, 0.05f, 0.09f, 0.9f));

        dl.AddCircleFilled(center, radius * 1.06f, dark, 48);

        for (int k = 0; k < 6; k++)
        {
            int n = (k + 1) % 6;
            var v0 = center + Outer[k] * s;
            var v1 = center + Outer[n] * s;
            var v2 = center + Inner[n] * s;
            var v3 = center + Inner[k] * s;
            dl.AddQuadFilled(v0, v1, v2, v3, ImGui.GetColorU32(Colors[k]));
            dl.AddQuad(v0, v1, v2, v3, edge, 1f);
        }

        dl.AddCircleFilled(center, radius * 0.46f, ImGui.GetColorU32(new Vector4(1f, 1f, 1f, 0.10f)), 28);
        dl.AddCircleFilled(center, radius * 0.30f, ImGui.GetColorU32(new Vector4(1f, 0.99f, 0.95f, 0.92f)), 28);

        uint rim = ImGui.GetColorU32(new Vector4(0.80f, 0.86f, 1.0f, 0.55f));
        dl.AddCircle(center, radius * 1.06f, rim, 48, System.MathF.Max(1.5f, radius * 0.06f));
    }
}
