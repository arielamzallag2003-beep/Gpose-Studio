using System;
using GPoseStudio;
using Xunit;

public class MaskShapeTests
{
    [Theory]
    [InlineData(PluginConfig.MaskPreset.Face, 1)]
    [InlineData(PluginConfig.MaskPreset.Diamond, 4)]
    [InlineData(PluginConfig.MaskPreset.LeftHalf, 2)]
    [InlineData(PluginConfig.MaskPreset.TopHalf, 2)]
    [InlineData(PluginConfig.MaskPreset.Subject, 8)]
    [InlineData(PluginConfig.MaskPreset.Highlights, 6)]
    [InlineData(PluginConfig.MaskPreset.Shadows, 6)]
    [InlineData(PluginConfig.MaskPreset.SkinTones, 7)]
    public void APresetWritesItsOwnSlotAndOnlyThat(PluginConfig.MaskPreset preset, int mode)
    {
        var c = new PluginConfig { MaskAMode = 5, MaskACx = 0.9f, MaskCMode = 3, MaskCCy = 0.7f };
        c.ApplyMaskPreset(1, preset);

        Assert.Equal(mode, c.MaskBMode);
        Assert.Equal(5, c.MaskAMode);
        Assert.Equal(0.9f, c.MaskACx);
        Assert.Equal(3, c.MaskCMode);
        Assert.Equal(0.7f, c.MaskCCy);
    }

    [Fact]
    public void APresetOverwritesEverythingInTheSlot()
    {
        var c = new PluginConfig { MaskAInvert = true, MaskAFeather = 0.45f, MaskAAngle = 2f };
        c.ApplyMaskPreset(0, PluginConfig.MaskPreset.Face);
        Assert.False(c.MaskAInvert);
        Assert.NotEqual(0.45f, c.MaskAFeather);
        Assert.Equal(0f, c.MaskAAngle);
    }

    [Fact]
    public void NoPresetAimsAnythingAtItsMask()
    {
        foreach (PluginConfig.MaskPreset p in Enum.GetValues(typeof(PluginConfig.MaskPreset)))
        {
            var c = new PluginConfig();
            c.ApplyMaskPreset(0, p);
            Assert.Equal(0, c.MaskSubscribers(0));
        }
    }

    [Theory]
    [InlineData(0, false)] [InlineData(1, true)] [InlineData(2, true)] [InlineData(3, false)]
    [InlineData(4, true)] [InlineData(5, true)] [InlineData(6, false)] [InlineData(7, false)] [InlineData(8, false)]
    public void OnlyShapesWithAPositionCanBeDragged(int mode, bool placeable)
        => Assert.Equal(placeable, PluginConfig.MaskPlaceable(mode));

    [Fact]
    public void TwoDiamondFramesSetsUpAAndBAndLeavesCFree()
    {
        var c = new PluginConfig { MaskCMode = 6, MaskCCy = 0.33f };
        c.SetUpTwoDiamondFrames();

        Assert.Equal(4, c.MaskAMode);
        Assert.Equal(4, c.MaskBMode);
        Assert.True(c.MaskAFrame && c.MaskBFrame);
        Assert.False(c.MaskCFrame);
        Assert.Equal((float)(Math.PI / 4), c.MaskAAngle, 4);
        Assert.True(c.MaskBreakOut);
        Assert.True(c.AnyMaskFrame());
        Assert.Equal(6, c.MaskCMode);
        Assert.Equal(0.33f, c.MaskCCy);
    }

    [Fact]
    public void AFrameOnAMaskThatIsOffIsNotAFrame()
    {
        var c = new PluginConfig { MaskAFrame = true, MaskAMode = 0 };
        Assert.False(c.AnyMaskFrame());
    }

    [Theory]
    [InlineData("MaskAFrame")] [InlineData("MaskBOutline")] [InlineData("MaskCOutR")]
    [InlineData("MaskFillR")] [InlineData("MaskFillA")] [InlineData("MaskBreakOut")]
    public void FramesTravelWithTheMasksNotWithTheExportMat(string name)
    {
        Assert.Equal(LookStore.Part.Other, LookStore.PartOf(name));
    }

    [Fact]
    public void AFramedLayoutSurvivesTheLookRoundTrip()
    {
        var src = new PluginConfig();
        src.SetUpTwoDiamondFrames();
        src.MaskFillR = 0.1f; src.MaskBOutline = 0.012f; src.MaskBreakOut = false;

        var dst = new PluginConfig();
        LookStore.Apply(LookStore.Capture(src), dst, LookStore.Part.All);

        Assert.True(dst.MaskAFrame && dst.MaskBFrame);
        Assert.Equal(4, dst.MaskAMode);
        Assert.Equal(0.1f, dst.MaskFillR);
        Assert.Equal(0.012f, dst.MaskBOutline);
        Assert.False(dst.MaskBreakOut);
    }
}
