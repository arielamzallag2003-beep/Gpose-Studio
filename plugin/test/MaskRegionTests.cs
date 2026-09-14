using System.Linq;
using GPoseStudio;
using Xunit;

public class MaskRegionTests
{
    [Fact]
    public void TheMasksAndTheOutputCannotDifferInsideAMask()
    {
        var names = MaskRegions.Overridable.Select(p => p.Name).ToHashSet();
        Assert.Contains("Exposure", names);
        Assert.Contains("EnGlow", names);
        Assert.DoesNotContain("MaskAMode", names);
        Assert.DoesNotContain("MaskFillR", names);
        Assert.DoesNotContain("Bypass", names);
        Assert.DoesNotContain("FrameMat", names);
        Assert.DoesNotContain("CutoutFeather", names);
        Assert.DoesNotContain("ExportTransparent", names);
        Assert.DoesNotContain("SwapRedBlue", names);
    }

    [Fact]
    public void AnEditInsideBecomesTheDifferenceAndTheWholeImageIsUntouched()
    {
        var b = new PluginConfig { Exposure = 0.1f, Contrast = 0.2f };
        var s = new MaskRegions.EditSession();
        s.Begin(b, "");
        s.Variant.Exposure = 0.9f;
        var next = s.End(b, "");

        Assert.Equal(new[] { "Exposure" }, MaskRegions.Names(next));
        Assert.Equal(0.1f, b.Exposure);
        var v = new PluginConfig();
        MaskRegions.BuildVariant(b, next, v);
        Assert.Equal(0.9f, v.Exposure);
    }

    [Fact]
    public void AnIdleFrameChangesNothing()
    {
        var b = new PluginConfig();
        var s = new MaskRegions.EditSession();
        s.Begin(b, "{\"Exposure\":0.5}");
        Assert.Null(s.End(b, "{\"Exposure\":0.5}"));
    }

    [Fact]
    public void TheWholeImageStillReachesInsideForEverythingNotOverridden()
    {
        var b = new PluginConfig { Contrast = 0.1f };
        var v = new PluginConfig();
        MaskRegions.BuildVariant(b, "{\"Exposure\":0.7}", v);
        Assert.Equal(0.1f, v.Contrast);

        b.Contrast = 0.4f;
        MaskRegions.BuildVariant(b, "{\"Exposure\":0.7}", v);
        Assert.Equal(0.4f, v.Contrast);
        Assert.Equal(0.7f, v.Exposure);
    }

    [Fact]
    public void PuttingAValueBackStopsItDiffering()
    {
        var b = new PluginConfig { Exposure = 0.1f };
        const string ov = "{\"Contrast\":0.5,\"Exposure\":0.9}";
        var s = new MaskRegions.EditSession();
        s.Begin(b, ov);
        s.Variant.Exposure = 0.1f;
        Assert.Equal(new[] { "Contrast" }, MaskRegions.Names(s.End(b, ov)));
    }

    [Fact]
    public void AChangeToTheWholeImageDuringTheFrameIsNotWrittenOver()
    {
        var b = new PluginConfig();
        var s = new MaskRegions.EditSession();
        s.Begin(b, "");
        b.Contrast = 0.6f;
        b.DebugShowMask = true;
        Assert.Null(s.End(b, ""));
        Assert.Equal(0.6f, b.Contrast);
        Assert.True(b.DebugShowMask);
    }

    [Fact]
    public void ASharedSettingChangedInsideGoesToTheWholeImage()
    {
        var b = new PluginConfig { SwapRedBlue = false };
        var s = new MaskRegions.EditSession();
        s.Begin(b, "");
        s.Variant.SwapRedBlue = true;
        Assert.Null(s.End(b, ""));
        Assert.True(b.SwapRedBlue);
    }

    [Fact]
    public void AFileCannotReachPastTheValuesAMaskMayChange()
    {
        var v = new PluginConfig { MaskAMode = 1 };
        int n = MaskRegions.ApplyOverrides(
            "{\"MaskAMode\":5,\"OutputDirectory\":\"C:/x\",\"MaskAOverrides\":\"{}\",\"Exposure\":0.5,\"Bypass\":true}", v);
        Assert.Equal(1, n);
        Assert.Equal(1, v.MaskAMode);
        Assert.False(v.Bypass);
        Assert.Equal(0.5f, v.Exposure);
    }

    [Theory]
    [InlineData("not json")]
    [InlineData("[1,2]")]
    [InlineData("{\"Exposure\":\"bright\"}")]
    [InlineData(null)]
    public void RubbishIsIgnoredRatherThanThrown(string? s)
    {
        var v = new PluginConfig { Exposure = 0.25f };
        Assert.Equal(0, MaskRegions.ApplyOverrides(s, v));
        Assert.Equal(0.25f, v.Exposure);
        Assert.Empty(MaskRegions.Names(s));
    }

    [Fact]
    public void AResetInsideLeavesTheMasksAlone()
    {
        var c = new PluginConfig { MaskAMode = 4, MaskAFrame = true, Exposure = 1f };
        MaskRegions.ResetOverridable(c);
        Assert.Equal(4, c.MaskAMode);
        Assert.True(c.MaskAFrame);
        Assert.Equal(new PluginConfig().Exposure, c.Exposure);
    }

    [Fact]
    public void ARegionNeedsAShapeSomethingToChangeAndStrength()
    {
        var c = new PluginConfig();
        c.MaskAOverrides = "{\"Exposure\":0.5}";
        Assert.False(c.MaskRegionActive(0));
        c.MaskAMode = 1;
        Assert.True(c.MaskRegionActive(0));
        c.MaskARegionMix = 0f;
        Assert.False(c.MaskRegionActive(0));
        c.MaskARegionMix = 1f; c.MaskARegionOn = false;
        Assert.False(c.MaskRegionActive(0));
        c.MaskARegionOn = true; c.MaskAOverrides = "";
        Assert.False(c.MaskRegionActive(0));
    }

    [Fact]
    public void OwnSettingsTravelWithTheLook()
    {
        var src = new PluginConfig { MaskBMode = 4, MaskBOverrides = "{\"Saturation\":-1}", MaskBRegionMix = 0.5f };
        var dst = new PluginConfig();
        LookStore.Apply(LookStore.Capture(src), dst, LookStore.Part.All);
        Assert.Equal("{\"Saturation\":-1}", dst.MaskBOverrides);
        Assert.Equal(0.5f, dst.MaskBRegionMix);
    }

    [Theory]
    [InlineData("MaskAOverrides")] [InlineData("MaskBRegionOn")] [InlineData("MaskCRegionMix")]
    public void OwnSettingsStayWithTheMasksInAPartialLoad(string name)
        => Assert.Equal(LookStore.Part.Other, LookStore.PartOf(name));

    [Fact]
    public void RemovingTheLastOneLeavesNothing()
    {
        Assert.Equal("", MaskRegions.Remove("{\"Exposure\":0.5}", "Exposure"));
        Assert.Equal(new[] { "Contrast" }, MaskRegions.Names(MaskRegions.Remove("{\"Contrast\":1,\"Exposure\":0.5}", "Exposure")));
    }

    [Fact]
    public void NamesReadAsWords()
    {
        Assert.Equal("Bloom Amount", MaskRegions.Label("BloomAmount"));
        Assert.Equal("Glow (on / off)", MaskRegions.Label("EnGlow"));
    }
}
