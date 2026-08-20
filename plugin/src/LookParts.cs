using System;
using System.Collections.Generic;

namespace GPoseStudio;

public static partial class LookStore
{
    public enum Part { All, Grade, Background, Light, Subject, Camera, Fx, Overlays, Other }

    private static readonly Dictionary<string, Part> Exact = new()
    {
        ["Exposure"] = Part.Grade, ["Contrast"] = Part.Grade, ["Saturation"] = Part.Grade,
        ["Vibrance"] = Part.Grade, ["Temperature"] = Part.Grade, ["Tint"] = Part.Grade,
        ["Lift"] = Part.Grade, ["Gamma"] = Part.Grade, ["Gain"] = Part.Grade,
        ["BlackPoint"] = Part.Grade, ["WhitePoint"] = Part.Grade, ["HueShift"] = Part.Grade,
        ["Clarity"] = Part.Grade, ["Dehaze"] = Part.Grade, ["Sharpen"] = Part.Grade,
        ["HlRecovery"] = Part.Grade, ["ColorBalance"] = Part.Grade, ["GradMap"] = Part.Grade,
        ["ScopeMode"] = Part.Grade, ["ScopeSplit"] = Part.Grade, ["ScopeSoft"] = Part.Grade,
        ["ToShadowR"] = Part.Grade, ["ToShadowG"] = Part.Grade, ["ToShadowB"] = Part.Grade,
        ["ToHighR"] = Part.Grade, ["ToHighG"] = Part.Grade, ["ToHighB"] = Part.Grade,
        ["EnColorBalance"] = Part.Grade, ["EnTealOrange"] = Part.Grade, ["EnSplitTone"] = Part.Grade,
        ["EnBleach"] = Part.Grade, ["EnGradMap"] = Part.Grade, ["EnFinalGrade"] = Part.Grade,
        ["ZoneCb"] = Part.Grade, ["ZoneTeal"] = Part.Grade, ["ZoneSplitTone"] = Part.Grade,
        ["ZoneBleach"] = Part.Grade, ["ZoneGradMap"] = Part.Grade, ["ZoneGrade"] = Part.Grade,
        ["ZoneFinal"] = Part.Grade,

        ["Vignette"] = Part.Camera, ["Grain"] = Part.Camera, ["Chroma"] = Part.Camera,
        ["ChromaClean"] = Part.Camera, ["ChromaRadial"] = Part.Camera, ["Letterbox"] = Part.Camera,
        ["Prism"] = Part.Camera, ["EnLens"] = Part.Camera, ["EnDof"] = Part.Camera,
        ["EnTiltShift"] = Part.Camera, ["EnWarp"] = Part.Camera,

        ["Orton"] = Part.Light, ["Glamour"] = Part.Light, ["GlamourMist"] = Part.Light,
        ["EnGlow"] = Part.Light, ["EnSpot"] = Part.Light, ["EnBacklight"] = Part.Light,
        ["EnGobo"] = Part.Light, ["EnHalo"] = Part.Light, ["EnShadow"] = Part.Light,
        ["EnGround"] = Part.Light, ["ZoneSpot"] = Part.Light, ["ZoneGobo"] = Part.Light,
        ["ZoneHalo"] = Part.Light, ["ZoneGround"] = Part.Light, ["ZoneShadow"] = Part.Light,
        ["ZoneGlow"] = Part.Light,
        ["ZoneBacklight"] = Part.Light,

        ["SubjectPop"] = Part.Subject, ["EnRim"] = Part.Subject, ["EnSkin"] = Part.Subject,
        ["EnBeauty"] = Part.Subject, ["EnWet"] = Part.Subject, ["EnEdge"] = Part.Subject,
        ["EdgeErode"] = Part.Subject, ["EdgeDespill"] = Part.Subject,
        ["EdgeWrap"] = Part.Subject, ["EdgeWrapWidth"] = Part.Subject,
        ["ZoneRim"] = Part.Subject, ["ZoneWet"] = Part.Subject, ["ZoneSkin"] = Part.Subject,
        ["ZoneBeauty"] = Part.Subject,

        ["SoftBlurRadius"] = Part.Background, ["EnBackdrop"] = Part.Background,
        ["EnBgFill"] = Part.Background, ["EnBgBlur"] = Part.Background,
        ["EnForegroundOn"] = Part.Background, ["EnFog"] = Part.Background,
        ["EnFrost"] = Part.Background, ["EnSubjectIso"] = Part.Background,
        ["ZoneBgPush"] = Part.Background, ["ZoneBgBlur"] = Part.Background,
        ["ZoneBgFill"] = Part.Background, ["ZoneBackdrop"] = Part.Background,
        ["ZoneFog"] = Part.Background,
        ["ZoneFrost"] = Part.Background,

        ["EnVhs"] = Part.Fx, ["EnHud"] = Part.Fx, ["EnUnderwater"] = Part.Fx,
        ["EnParticles"] = Part.Fx, ["EnStylize"] = Part.Fx, ["Iridescent"] = Part.Fx,
        ["EdgeAura"] = Part.Fx, ["EdgeWidth"] = Part.Fx, ["EdgeThreshold"] = Part.Fx,
        ["EdgeR"] = Part.Fx, ["EdgeG"] = Part.Fx, ["EdgeB"] = Part.Fx,
        ["ZoneVhs"] = Part.Fx, ["ZoneUnderwater"] = Part.Fx, ["ZoneStylize"] = Part.Fx,
        ["ZoneBokeh"] = Part.Fx,

        ["EnElements"] = Part.Overlays, ["EnText"] = Part.Overlays,
        ["EnFrame"] = Part.Overlays, ["Texts"] = Part.Overlays,
    };

    private static readonly (string Prefix, Part Part)[] Families =
    {
        ("Mask", Part.Other),

        ("St", Part.Grade), ("Cb", Part.Grade), ("Gm", Part.Grade), ("Final", Part.Grade),
        ("Bleach", Part.Grade), ("Teal", Part.Grade), ("Denoise", Part.Grade),

        ("Dof", Part.Camera), ("Tilt", Part.Camera), ("Film", Part.Camera), ("Lens", Part.Camera),
        ("Fisheye", Part.Camera), ("Swirl", Part.Camera), ("Kaleido", Part.Camera),
        ("Wave", Part.Camera), ("Flow", Part.Camera),

        ("Bloom", Part.Light), ("Halation", Part.Light), ("Godray", Part.Light),
        ("Anam", Part.Light), ("Spot", Part.Light), ("Backlight", Part.Light),
        ("BackdropLight", Part.Light), ("Gobo", Part.Light), ("Halo", Part.Light),
        ("Shadow", Part.Light), ("Ground", Part.Light), ("Wash", Part.Light),
        ("Leak", Part.Light), ("Caustics", Part.Light),

        ("Rim", Part.Subject), ("Skin", Part.Subject), ("Beauty", Part.Subject), ("Wet", Part.Subject),

        ("BgB", Part.Background), ("Bg", Part.Background), ("Fg", Part.Background),
        ("Univ", Part.Background), ("Pat", Part.Background), ("Blend", Part.Background),
        ("Fog", Part.Background), ("Frost", Part.Background),

        ("Vhs", Part.Fx), ("Hud", Part.Fx), ("Uw", Part.Fx), ("Particle", Part.Fx),
        ("Bokeh", Part.Fx), ("Kuwahara", Part.Fx), ("Mosaic", Part.Fx), ("Irid", Part.Fx),
        ("Glitch", Part.Fx),

        ("Elem", Part.Overlays), ("Frame", Part.Overlays),
    };

    private static readonly HashSet<string> AlwaysCarried = new(StringComparer.Ordinal)
    {
        "ZoneNear", "ZoneNearSoft",
    };

    internal static bool IsAlwaysCarried(string name) => AlwaysCarried.Contains(name);

    internal static Part PartOf(string name)
    {
        if (Exact.TryGetValue(name, out var p)) return p;
        foreach (var (prefix, part) in Families)
            if (name.StartsWith(prefix, StringComparison.Ordinal)) return part;
        return Part.Other;
    }
}
