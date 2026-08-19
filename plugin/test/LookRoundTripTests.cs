using System;
using System.Linq;
using GPoseStudio;
using Xunit;

public class LookRoundTripTests
{
    private static PluginConfig Tweaked()
    {
        var c = new PluginConfig
        {
            Exposure = 0.375f,
            BgStyle = 27,
            EnBackdrop = false,
            SkinWarmth = 0.5f,
            GoboPattern = 2,
            FinalExposure = -0.25f,
            BgBPatColOverride = true,
        };
        c.Elem[0] = 1.5f;
        c.Elem[^1] = 2.5f;
        c.FgField[0] = 3.5f;
        c.FgField[^1] = 4.5f;
        c.Texts.Add(new TextMarker { Text = "caption", X = 0.25f, Size = 18f });
        return c;
    }

    [Fact]
    public void EveryLookValueSurvivesCaptureAndApply()
    {
        var src = Tweaked();
        var json = LookStore.Capture(src);

        var dst = new PluginConfig();
        Assert.True(LookStore.Apply(json, dst, LookStore.Part.All));

        Assert.Equal(src.Exposure, dst.Exposure);
        Assert.Equal(src.BgStyle, dst.BgStyle);
        Assert.Equal(src.EnBackdrop, dst.EnBackdrop);
        Assert.Equal(src.SkinWarmth, dst.SkinWarmth);
        Assert.Equal(src.GoboPattern, dst.GoboPattern);
        Assert.Equal(src.FinalExposure, dst.FinalExposure);
        Assert.Equal(src.BgBPatColOverride, dst.BgBPatColOverride);

        Assert.Equal(src.Elem, dst.Elem);
        Assert.Equal(src.FgField, dst.FgField);

        Assert.Single(dst.Texts);
        Assert.Equal("caption", dst.Texts[0].Text);
        Assert.Equal(0.25f, dst.Texts[0].X);
    }

    [Fact]
    public void ASecondRoundTripChangesNothing()
    {
        var once = LookStore.Capture(Tweaked());

        var mid = new PluginConfig();
        LookStore.Apply(once, mid, LookStore.Part.All);

        Assert.Equal(once, LookStore.Capture(mid));
    }

    [Fact]
    public void CaptureOmitsWhatIsNotPartOfALook()
    {
        var json = LookStore.Capture(new PluginConfig { OutputDirectory = @"C:\somewhere" });

        Assert.DoesNotContain("OutputDirectory", json);
        Assert.DoesNotContain("LivePreview", json);
        Assert.DoesNotContain("ExportScale", json);
        Assert.DoesNotContain("Bypass", json);
    }

    [Fact]
    public void SharingDropsThePathsAndOnlyThePaths()
    {
        var c = Tweaked();
        c.ElemImages[0] = @"C:\Users\someone\Pictures\overlay.png";

        Assert.Contains("ElemImages", LookStore.Capture(c));
        Assert.DoesNotContain("ElemImages", LookStore.Capture(c, forSharing: true));
        Assert.DoesNotContain("someone", LookStore.Capture(c, forSharing: true));
        Assert.Contains("Exposure", LookStore.Capture(c, forSharing: true));
    }

    [Fact]
    public void APartialLookCarriesOnlyItsPart()
    {
        var src = Tweaked();
        var json = LookStore.Capture(src, forSharing: false, part: LookStore.Part.Grade);

        Assert.Contains("Exposure", json);
        Assert.Contains("FinalExposure", json);
        Assert.DoesNotContain("SkinWarmth", json);
        Assert.DoesNotContain("GoboPattern", json);
    }

    [Fact]
    public void APartialLoadLeavesEverythingElseAlone()
    {
        var src = Tweaked();
        var grade = LookStore.Capture(src, forSharing: false, part: LookStore.Part.Grade);

        var dst = new PluginConfig { SkinWarmth = 0.125f, GoboPattern = 1 };
        Assert.True(LookStore.Apply(grade, dst, LookStore.Part.All, out int applied));

        Assert.True(applied > 0);
        Assert.Equal(src.Exposure, dst.Exposure);
        Assert.Equal(0.125f, dst.SkinWarmth);
        Assert.Equal(1, dst.GoboPattern);
    }

    [Fact]
    public void TheFrameOfReferenceTravelsWithEveryPart()
    {
        var src = new PluginConfig { ZoneNear = 0.321f, Exposure = 0.5f };
        var grade = LookStore.Capture(src, forSharing: false, part: LookStore.Part.Grade);

        Assert.Contains("ZoneNear", grade);

        var dst = new PluginConfig { ZoneNear = 0.05f };
        LookStore.Apply(grade, dst, LookStore.Part.Grade);
        Assert.Equal(0.321f, dst.ZoneNear);
    }

    [Fact]
    public void LoadingUnderANonIntersectingFilterReportsNothingApplied()
    {
        var light = LookStore.Capture(Tweaked(), forSharing: false, part: LookStore.Part.Light);

        var dst = new PluginConfig();
        LookStore.Apply(light, dst, LookStore.Part.Camera, out int applied);

        Assert.Equal(0, applied);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not json")]
    [InlineData("[1,2,3]")]
    [InlineData("{\"Exposure\":\"not a number\"}")]
    public void MalformedInputIsRefusedOrIgnoredWithoutThrowing(string json)
    {
        var dst = new PluginConfig();
        var before = LookStore.Capture(dst);

        LookStore.Apply(json, dst, LookStore.Part.All);

        Assert.Equal(before, LookStore.Capture(dst));
    }
}
