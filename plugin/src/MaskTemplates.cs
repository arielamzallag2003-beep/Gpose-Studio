using System;
using System.Collections.Generic;
using System.Text.Json;

namespace GPoseStudio;

public enum MaskTemplate
{
    TwoDiamonds, ComicTriptych, Triangles, HexBadge, NeonCross, TornPaper, Polaroid, CinemaBands,
    BurntEdges, LightCone, WindowBlinds, StageSpot, Starburst, DappledLight, SplitHalves,
    MirrorTwins, PortraitFocus, HaloAndRim, ColourPop, SoftVignette,
}

public static class MaskTemplates
{
    public readonly record struct Info(MaskTemplate Kind, string Name, string Group, string Tip);

    public static readonly Info[] All =
    {
        new(MaskTemplate.TwoDiamonds, "Two diamonds", "Panels",
            "Two overlapping diamond frames on paper, the figure breaking out over both.\nFrames add together: a pixel in either panel is visible."),
        new(MaskTemplate.ComicTriptych, "Comic triptych", "Panels",
            "Three tilted panels lying on each other like cards, with ink outlines and drop\nshadows. The middle one is on top; reorder them under Frames."),
        new(MaskTemplate.Triangles, "Triangles", "Panels",
            "A triangle and an upturned one, interlocking. Polygons stand on a flat base;\na rotation of 3.14 turns one over."),
        new(MaskTemplate.HexBadge, "Hex badge", "Panels",
            "A hexagon window with a glowing gold edge on a dark page, the figure breaking\nout. Glow draws a neon edge with no element layer."),
        new(MaskTemplate.NeonCross, "Neon cross", "Panels",
            "A cross-shaped window with a red neon edge. Inward feather keeps a panel from\nleaking past its line."),
        new(MaskTemplate.TornPaper, "Torn paper", "Panels",
            "A rough-edged card with a drop shadow on warm paper, a little warmer inside.\nRough edge tears any shape; the shadow makes it lie on the page."),
        new(MaskTemplate.Polaroid, "Instant photo", "Panels",
            "A rounded window on a white card with a soft shadow. Corner round keeps the\nsides straight."),
        new(MaskTemplate.CinemaBands, "Cinema bands", "Panels",
            "A 2.39:1 window with black above and below. A frame is also the simplest\nletterbox there is."),
        new(MaskTemplate.BurntEdges, "Burnt edges", "Panels",
            "A frame whose edge is torn with coarse noise into black: an old print, a\nburnt page."),
        new(MaskTemplate.LightCone, "Light cones", "Light",
            "Two beams from above the top-left corner, brighter and warmer inside.\nWedges reach off the frame; own settings are the light."),
        new(MaskTemplate.WindowBlinds, "Window blinds", "Light",
            "Slanted bands of warm light and cool shade from one stripe shape and its\ninverse. Linked, so they move as one."),
        new(MaskTemplate.StageSpot, "Stage spotlight", "Light",
            "A beam from above onto a pool on the floor, and the room behind the figure\nfalling into darkness. Combine lets the beam and pool add up."),
        new(MaskTemplate.Starburst, "Starburst", "Light",
            "Rays of light behind the head from a twelve-point star, only behind the figure.\nOnly on: behind the subject keeps a shape off the character."),
        new(MaskTemplate.DappledLight, "Dappled light", "Light",
            "Soft patches of sun and shade, as through leaves: noise and its inverse."),
        new(MaskTemplate.SplitHalves, "Split halves", "Grade",
            "Black and white on the left, warm colour on the right. Two linear divides\nfacing apart, each with settings of its own."),
        new(MaskTemplate.MirrorTwins, "Mirror twins", "Grade",
            "One brightened oval mirrored to both sides of the frame. Mirror makes a\nsymmetrical layout from one mask."),
        new(MaskTemplate.PortraitFocus, "Portrait focus", "Grade",
            "The face lifted and sharpened, the background around it darker and muted.\nPlace the small ellipse on the face."),
        new(MaskTemplate.HaloAndRim, "Halo and rim", "Grade",
            "A cool halo just outside the silhouette and a warm rim just inside it,\nfollowing the pose. Subject edge needs no placing."),
        new(MaskTemplate.ColourPop, "Colour pop", "Grade",
            "Everything goes grey except strongly coloured things. A saturation range,\ninverted, with the colour taken out."),
        new(MaskTemplate.SoftVignette, "Soft vignette", "Grade",
            "Darkened corners from an inverted ellipse at partial strength. Strength\nweakens a mask without touching its settings."),
    };

    private static string Own(params (string Name, object Value)[] values)
    {
        var d = new Dictionary<string, object>(values.Length);
        foreach (var (n, v) in values) d[n] = v;
        return JsonSerializer.Serialize(d);
    }

    private static void Shape(PluginConfig c, int i, int mode, float cx, float cy, float size, float ell = 1f, float ang = 0f,
                              float feather = 0.004f, float sides = 5f, float detail = 0.5f, float round = 0f)
    {
        c.SetMaskMode(i, mode); c.SetMaskCx(i, cx); c.SetMaskCy(i, cy); c.SetMaskSize(i, size);
        c.SetMaskEllipse(i, ell); c.SetMaskAngle(i, ang); c.SetMaskFeather(i, feather);
        c.SetMaskSides(i, sides); c.SetMaskDetail(i, detail); c.SetMaskRound(i, round);
    }

    private static void Frame(PluginConfig c, int i, float outline, (float R, float G, float B) col, float glow = 0f, float shadow = 0f)
    {
        c.SetMaskFrame(i, true); c.SetMaskOutline(i, outline); c.SetMaskOutColor(i, col.R, col.G, col.B);
        c.SetMaskOutGlow(i, glow); c.SetMaskShadow(i, shadow);
    }

    private static void Fill(PluginConfig c, float r, float g, float b, bool breakOut = true)
    {
        c.MaskFillR = r; c.MaskFillG = g; c.MaskFillB = b; c.MaskFillA = 1f; c.MaskBreakOut = breakOut;
    }

    public static void Apply(PluginConfig c, MaskTemplate t)
    {
        var def = new PluginConfig();
        for (int i = 0; i < PluginConfig.MaskCount; i++) c.ClearMask(i);
        c.MaskFillR = def.MaskFillR; c.MaskFillG = def.MaskFillG; c.MaskFillB = def.MaskFillB; c.MaskFillA = def.MaskFillA;
        c.MaskBreakOut = def.MaskBreakOut; c.MaskFramesStacked = false; c.MaskOutlineBehind = false;
        c.MaskRegionOverlap = 0;

        const float Pi = MathF.PI, Quarter = MathF.PI / 4f, HalfPi = MathF.PI / 2f;
        var ink = (0.08f, 0.08f, 0.10f);
        switch (t)
        {
            case MaskTemplate.TwoDiamonds:
                c.SetUpTwoDiamondFrames();
                Fill(c, 0.97f, 0.96f, 0.94f);
                break;

            case MaskTemplate.ComicTriptych:
                Shape(c, 0, 4, 0.20f, 0.50f, 0.150f, 2.85f, -0.10f, 0.003f);
                Shape(c, 1, 4, 0.52f, 0.49f, 0.175f, 2.60f, 0.05f, 0.003f);
                Shape(c, 2, 4, 0.83f, 0.51f, 0.150f, 2.85f, -0.07f, 0.003f);
                for (int i = 0; i < 3; i++) Frame(c, i, 0.006f, ink, shadow: 0.6f);
                c.MaskAFrameRank = 0; c.MaskBFrameRank = 2; c.MaskCFrameRank = 1;
                c.MaskFramesStacked = true;
                Fill(c, 0.96f, 0.95f, 0.91f);
                break;

            case MaskTemplate.Triangles:
                Shape(c, 0, 9, 0.40f, 0.60f, 0.40f, 1f, 0f, 0.003f, sides: 3f);
                Shape(c, 1, 9, 0.64f, 0.40f, 0.40f, 1f, Pi, 0.003f, sides: 3f);
                Frame(c, 0, 0.005f, (0.96f, 0.96f, 0.96f));
                Frame(c, 1, 0.005f, (0.96f, 0.96f, 0.96f));
                c.MaskFramesStacked = true;
                Fill(c, 0.05f, 0.05f, 0.06f);
                break;

            case MaskTemplate.HexBadge:
                Shape(c, 0, 9, 0.56f, 0.50f, 0.46f, 1f, 0f, 0.003f, sides: 6f);
                Frame(c, 0, 0.005f, (0.95f, 0.78f, 0.38f), glow: 0.45f);
                c.SetMaskEdge(0, 1);
                c.MaskAOverrides = Own(("Contrast", 0.12f), ("Clarity", 0.15f));
                Fill(c, 0.04f, 0.04f, 0.07f);
                break;

            case MaskTemplate.NeonCross:
                Shape(c, 0, 13, 0.56f, 0.50f, 0.42f, 1.15f, 0f, 0.002f, detail: 0.42f);
                Frame(c, 0, 0.004f, (1.00f, 0.25f, 0.32f), glow: 0.7f);
                c.SetMaskEdge(0, 1);
                Fill(c, 0.03f, 0.02f, 0.04f);
                break;

            case MaskTemplate.TornPaper:
                Shape(c, 0, 4, 0.50f, 0.50f, 0.78f, 0.52f, -0.02f, 0.004f, round: 0.04f);
                c.SetMaskRough(0, 0.014f); c.SetMaskRoughScale(0, 14f);
                Frame(c, 0, 0f, ink, shadow: 0.7f);
                c.MaskAOverrides = Own(("Temperature", 0.08f), ("Contrast", -0.04f));
                Fill(c, 0.92f, 0.88f, 0.79f);
                break;

            case MaskTemplate.Polaroid:
                Shape(c, 0, 4, 0.50f, 0.43f, 0.40f, 0.95f, -0.03f, 0.002f, round: 0.03f);
                Frame(c, 0, 0f, ink, shadow: 0.45f);
                Fill(c, 0.97f, 0.97f, 0.95f, breakOut: false);
                break;

            case MaskTemplate.CinemaBands:
                Shape(c, 0, 4, 0.50f, 0.50f, 1.0f, 0.372f, 0f, 0.002f);
                Frame(c, 0, 0f, ink);
                c.SetMaskEdge(0, 1);
                Fill(c, 0f, 0f, 0f, breakOut: false);
                break;

            case MaskTemplate.BurntEdges:
                Shape(c, 0, 4, 0.50f, 0.50f, 0.80f, 0.50f, 0f, 0.02f, round: 0.12f);
                c.SetMaskRough(0, 0.05f); c.SetMaskRoughScale(0, 5f);
                Frame(c, 0, 0f, ink);
                c.MaskAOverrides = Own(("Temperature", 0.10f), ("Saturation", -0.15f));
                Fill(c, 0.02f, 0.015f, 0.01f, breakOut: false);
                break;

            case MaskTemplate.LightCone:
                Shape(c, 0, 11, 0.10f, -0.08f, 1.6f, 1f, 0.95f, 0.05f, detail: 0.09f);
                Shape(c, 1, 11, 0.26f, -0.08f, 1.6f, 1f, 1.10f, 0.04f, detail: 0.05f);
                c.MaskAOverrides = Own(("Exposure", 0.70f), ("Temperature", 0.14f));
                c.MaskBOverrides = Own(("Exposure", 0.45f), ("Temperature", 0.12f));
                c.MaskALinked = c.MaskBLinked = true;
                c.MaskRegionOverlap = 1;
                break;

            case MaskTemplate.WindowBlinds:
                Shape(c, 0, 12, 0.50f, 0.50f, 0.17f, 1f, 0.55f, 0.03f, detail: 0.55f);
                Shape(c, 1, 12, 0.50f, 0.50f, 0.17f, 1f, 0.55f, 0.03f, detail: 0.55f);
                c.MaskBInvert = true;
                c.MaskAOverrides = Own(("Exposure", 0.30f), ("Temperature", 0.10f));
                c.MaskBOverrides = Own(("Exposure", -0.25f), ("Temperature", -0.05f));
                c.MaskALinked = c.MaskBLinked = true;
                break;

            case MaskTemplate.StageSpot:
                Shape(c, 0, 11, 0.58f, -0.12f, 1.3f, 1f, HalfPi, 0.05f, detail: 0.09f);
                Shape(c, 1, 1, 0.58f, 0.92f, 0.30f, 0.26f, 0f, 0.10f);
                Shape(c, 2, 1, 0.58f, 0.55f, 0.52f, 1.2f, 0f, 0.25f);
                c.MaskCInvert = true; c.SetMaskDepthLimit(2, 2);
                c.MaskAOverrides = Own(("Exposure", 0.50f), ("Temperature", 0.05f));
                c.MaskBOverrides = Own(("Exposure", 0.40f));
                c.MaskCOverrides = Own(("Exposure", -1.0f), ("Saturation", -0.3f));
                c.MaskCRegionRank = 0; c.MaskARegionRank = 1; c.MaskBRegionRank = 2;
                c.MaskRegionOverlap = 1;
                c.MaskALinked = c.MaskBLinked = true;
                break;

            case MaskTemplate.Starburst:
                Shape(c, 0, 10, 0.58f, 0.34f, 1.3f, 1f, 0f, 0.03f, sides: 12f, detail: 0.12f);
                Shape(c, 1, 1, 0.58f, 0.34f, 0.32f, 1f, 0f, 0.25f);
                c.SetMaskDepthLimit(0, 2); c.SetMaskDepthLimit(1, 2);
                c.MaskAOverrides = Own(("Exposure", 0.45f), ("Temperature", 0.10f));
                c.MaskBOverrides = Own(("Exposure", 0.40f));
                c.MaskALinked = c.MaskBLinked = true;
                c.MaskRegionOverlap = 1;
                break;

            case MaskTemplate.DappledLight:
                Shape(c, 0, 16, 0.50f, 0.50f, 0.30f, 0.8f, 0.4f, 0.10f, detail: 0.40f);
                Shape(c, 1, 16, 0.50f, 0.50f, 0.30f, 0.8f, 0.4f, 0.10f, detail: 0.40f);
                c.MaskBInvert = true; c.SetMaskStrength(1, 0.5f);
                c.MaskAOverrides = Own(("Exposure", 0.45f), ("Temperature", 0.12f));
                c.MaskBOverrides = Own(("Exposure", -0.35f), ("Temperature", -0.04f));
                c.MaskALinked = c.MaskBLinked = true;
                break;

            case MaskTemplate.SplitHalves:
                Shape(c, 0, 2, 0.50f, 0.50f, 0.25f, 1f, 0f, 0.01f);
                Shape(c, 1, 2, 0.50f, 0.50f, 0.25f, 1f, Pi, 0.01f);
                c.MaskAOverrides = Own(("Saturation", -1.0f), ("Contrast", 0.2f));
                c.MaskBOverrides = Own(("Temperature", 0.12f), ("Saturation", 0.15f));
                c.MaskALinked = c.MaskBLinked = true;
                break;

            case MaskTemplate.MirrorTwins:
                Shape(c, 0, 1, 0.20f, 0.45f, 0.22f, 1.4f, 0.25f, 0.12f);
                c.SetMaskMirror(0, 1);
                c.MaskAOverrides = Own(("Exposure", 0.60f), ("Contrast", 0.10f), ("Temperature", -0.10f));
                break;

            case MaskTemplate.PortraitFocus:
                Shape(c, 0, 1, 0.58f, 0.34f, 0.15f, 1.25f, 0f, 0.12f);
                Shape(c, 1, 1, 0.58f, 0.50f, 0.50f, 1.0f, 0f, 0.30f);
                c.MaskBInvert = true; c.SetMaskDepthLimit(1, 2);
                c.MaskAOverrides = Own(("Exposure", 0.15f), ("Sharpen", 0.4f), ("Clarity", 0.2f));
                c.MaskBOverrides = Own(("Exposure", -0.6f), ("Saturation", -0.4f));
                c.MaskBRegionRank = 0; c.MaskARegionRank = 1;
                c.MaskRegionOverlap = 1;
                break;

            case MaskTemplate.HaloAndRim:
                Shape(c, 0, 14, 0.5f, 0.5f, 0.030f, 1f, 0f, 0.02f, detail: 0.05f);
                Shape(c, 1, 14, 0.5f, 0.5f, 0.008f, 1f, 0f, 0.02f, detail: 0.95f);
                c.MaskAOverrides = Own(("Exposure", 0.7f), ("Temperature", -0.12f));
                c.MaskBOverrides = Own(("Exposure", 0.35f), ("Temperature", 0.08f));
                c.MaskRegionOverlap = 1;
                break;

            case MaskTemplate.ColourPop:
                Shape(c, 0, 15, 0.5f, 0.80f, 0.35f, 1f, 0f, 0.08f);
                c.MaskAInvert = true;
                c.MaskAOverrides = Own(("Saturation", -1.0f));
                break;

            case MaskTemplate.SoftVignette:
                Shape(c, 0, 1, 0.50f, 0.50f, 0.62f, 0.75f, 0f, 0.35f);
                c.MaskAInvert = true; c.SetMaskStrength(0, 0.8f);
                c.MaskAOverrides = Own(("Exposure", -0.8f));
                break;
        }
    }
}
