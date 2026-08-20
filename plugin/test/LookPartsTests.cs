using GPoseStudio;
using Xunit;

public class LookPartsTests
{
    [Theory]
    [InlineData("Exposure", LookStore.Part.Grade)]
    [InlineData("Contrast", LookStore.Part.Grade)]
    [InlineData("StAmount", LookStore.Part.Grade)]
    [InlineData("CbShadowR", LookStore.Part.Grade)]
    [InlineData("FinalExposure", LookStore.Part.Grade)]
    [InlineData("GmHighR", LookStore.Part.Grade)]
    [InlineData("FrameCorner", LookStore.Part.Overlays)]
    [InlineData("Elem", LookStore.Part.Overlays)]
    [InlineData("Texts", LookStore.Part.Overlays)]
    [InlineData("BokehAmount", LookStore.Part.Fx)]
    [InlineData("ParticleType", LookStore.Part.Fx)]
    [InlineData("HudRadar", LookStore.Part.Fx)]
    [InlineData("VhsStatic", LookStore.Part.Fx)]
    [InlineData("UwCaustic", LookStore.Part.Fx)]
    [InlineData("FisheyeAmt", LookStore.Part.Camera)]
    [InlineData("SoftBlurRadius", LookStore.Part.Background)]
    [InlineData("BgBTopR", LookStore.Part.Background)]
    [InlineData("BgRecolorStart", LookStore.Part.Background)]
    [InlineData("UnivPattern", LookStore.Part.Background)]
    [InlineData("PatColMode", LookStore.Part.Background)]
    [InlineData("BlendFeather", LookStore.Part.Background)]
    [InlineData("FgOpacity", LookStore.Part.Background)]
    [InlineData("FogStrength", LookStore.Part.Background)]
    [InlineData("GroundCastLen", LookStore.Part.Light)]
    [InlineData("ShadowAmount", LookStore.Part.Light)]
    [InlineData("GoboPattern", LookStore.Part.Light)]
    [InlineData("BloomAmount", LookStore.Part.Light)]
    [InlineData("BackdropLightAmt", LookStore.Part.Light)]
    [InlineData("SkinWarmth", LookStore.Part.Subject)]
    [InlineData("WetAmount", LookStore.Part.Subject)]
    [InlineData("RimStrength", LookStore.Part.Subject)]
    [InlineData("SubjectPop", LookStore.Part.Subject)]
    [InlineData("DofFocus", LookStore.Part.Camera)]
    [InlineData("FilmToe", LookStore.Part.Camera)]
    [InlineData("Vignette", LookStore.Part.Camera)]
    [InlineData("Grain", LookStore.Part.Camera)]
    public void PropertiesLandInTheirTab(string name, LookStore.Part expected)
        => Assert.Equal(expected, LookStore.PartOf(name));

    [Theory]
    [InlineData("EdgeErode", LookStore.Part.Subject)]
    [InlineData("EdgeDespill", LookStore.Part.Subject)]
    [InlineData("EdgeWrap", LookStore.Part.Subject)]
    [InlineData("EdgeWrapWidth", LookStore.Part.Subject)]
    [InlineData("EdgeAura", LookStore.Part.Fx)]
    [InlineData("EdgeWidth", LookStore.Part.Fx)]
    [InlineData("EdgeThreshold", LookStore.Part.Fx)]
    [InlineData("EdgeR", LookStore.Part.Fx)]
    public void EdgeIsSplitBetweenSubjectAndFx(string name, LookStore.Part expected)
        => Assert.Equal(expected, LookStore.PartOf(name));

    [Theory]
    [InlineData("AnimSpeed")]
    [InlineData("ZoneNear")]
    [InlineData("ZoneNearSoft")]
    public void GenuinelyCrossCuttingValuesAreOther(string name)
        => Assert.Equal(LookStore.Part.Other, LookStore.PartOf(name));

    [Fact]
    public void AnUnknownPropertyIsOtherRatherThanGrade()
    {
        Assert.Equal(LookStore.Part.Other, LookStore.PartOf("SomethingAddedLater"));
        Assert.Equal(LookStore.Part.Other, LookStore.PartOf(""));
    }

    [Fact]
    public void EveryTabPartIsReachable()
    {
        foreach (var (probe, part) in new (string, LookStore.Part)[]
        {
            ("Exposure", LookStore.Part.Grade),
            ("BgTopR", LookStore.Part.Background),
            ("BloomAmount", LookStore.Part.Light),
            ("SkinWarmth", LookStore.Part.Subject),
            ("DofFocus", LookStore.Part.Camera),
            ("VhsStatic", LookStore.Part.Fx),
            ("Elem", LookStore.Part.Overlays),
        })
            Assert.Equal(part, LookStore.PartOf(probe));
    }

    [Theory]
    [InlineData("MaskAMode")]
    [InlineData("MaskBCx")]
    [InlineData("MaskCFeather")]
    [InlineData("MaskAInvert")]
    public void MasksAreOtherSoPartialLoadsLeaveThemAlone(string name)
    {
        Assert.Equal(LookStore.Part.Other, LookStore.PartOf(name));
    }

    [Theory]
    [InlineData("ZoneBgFill", LookStore.Part.Background)]
    [InlineData("ZoneBackdrop", LookStore.Part.Background)]
    [InlineData("ZoneFog", LookStore.Part.Background)]
    [InlineData("ZoneGlow", LookStore.Part.Light)]
    [InlineData("ZoneFinal", LookStore.Part.Grade)]
    [InlineData("ZoneGrade", LookStore.Part.Grade)]
    public void EveryRoutingIntTravelsWithTheTabItBelongsTo(string name, LookStore.Part want)
    {
        Assert.Equal(want, LookStore.PartOf(name));
    }

    [Fact]
    public void NoRoutingIntIsLeftUnclassified()
    {
        foreach (var p in typeof(PluginConfig).GetProperties())
        {
            if (p.PropertyType != typeof(int) || !p.Name.StartsWith("Zone", System.StringComparison.Ordinal)) continue;
            Assert.NotEqual(LookStore.Part.Other, LookStore.PartOf(p.Name));
        }
    }
}
