using System;
using GPoseStudio;
using Xunit;

public class LinkedMaskTests
{
    private const float Eps = 1e-4f;

    [Fact]
    public void AGroupMovesTogether()
    {
        var c = new PluginConfig { MaskAMode = 4, MaskACx = 0.4f, MaskACy = 0.5f, MaskBMode = 4, MaskBCx = 0.6f, MaskBCy = 0.3f };
        var group = new[] { 0, 1 };
        var start = new[] { c.PoseOf(0), c.PoseOf(1) };
        c.MoveMaskGroup(group, start, 0.5f, 0.4f, 0.1f, -0.05f, 0f, 1f, 16f / 9f);

        Assert.Equal(0.5f, c.MaskACx, 4);
        Assert.Equal(0.45f, c.MaskACy, 4);
        Assert.Equal(0.7f, c.MaskBCx, 4);
        Assert.Equal(0.25f, c.MaskBCy, 4);
    }

    [Theory]
    [InlineData(1f)]
    [InlineData(2f)]
    public void AQuarterTurnTurnsPositionsAndShapesAlikeOnAnyAspect(float asp)
    {
        var c = new PluginConfig { MaskBMode = 4, MaskBCx = 0.5f + 0.1f / asp, MaskBCy = 0.5f, MaskBAngle = 0.2f, MaskBSize = 0.3f };
        c.MoveMaskGroup(new[] { 1 }, new[] { c.PoseOf(1) }, 0.5f, 0.5f, 0f, 0f, MathF.PI / 2f, 1f, asp);

        Assert.Equal(0.5f, c.MaskBCx, 4);
        Assert.Equal(0.6f, c.MaskBCy, 4);
        Assert.Equal(0.2f + MathF.PI / 2f, c.MaskBAngle, 4);
        Assert.Equal(0.3f, c.MaskBSize, 4);
    }

    [Fact]
    public void ResizingScalesSizesAndDistancesFromTheCentre()
    {
        var c = new PluginConfig { MaskAMode = 1, MaskACx = 0.6f, MaskACy = 0.5f, MaskASize = 0.2f };
        c.MoveMaskGroup(new[] { 0 }, new[] { c.PoseOf(0) }, 0.5f, 0.5f, 0f, 0f, 0f, 2f, 1f);
        Assert.Equal(0.7f, c.MaskACx, 4);
        Assert.Equal(0.4f, c.MaskASize, 4);
    }

    [Fact]
    public void AnglesStayInTheSlidersRange()
    {
        var c = new PluginConfig { MaskAMode = 4, MaskAAngle = 3f };
        c.MoveMaskGroup(new[] { 0 }, new[] { c.PoseOf(0) }, c.MaskACx, c.MaskACy, 0f, 0f, 1f, 1f, 1f);
        Assert.InRange(c.MaskAAngle, -MathF.PI - Eps, MathF.PI + Eps);
        Assert.Equal(4f - MathF.Tau, c.MaskAAngle, 4);
    }

    [Fact]
    public void OnlyLinkedMasksWithAPositionJoinTheGroup()
    {
        var c = new PluginConfig
        {
            MaskAMode = 4, MaskALinked = true,
            MaskBMode = 4, MaskBLinked = true,
            MaskCMode = 6, MaskCLinked = true,
            MaskDMode = 1,
        };
        Assert.Equal(new[] { 0, 1 }, c.LinkedPlaceable(0));
        Assert.Equal(new[] { 3 }, c.LinkedPlaceable(3));
    }

    [Fact]
    public void AnExportCutKeepsOnlyMasksThatExist()
    {
        var c = new PluginConfig { MaskAMode = 4 };
        int either = ZoneBits.WithMaskMode(ZoneBits.MaskBit(0) | ZoneBits.MaskBit(2), ZoneBits.MaskEither);
        Assert.Equal(ZoneBits.WithMaskMode(ZoneBits.MaskBit(0), ZoneBits.MaskEither), c.LiveMaskBits(either));
        Assert.Equal(0, c.LiveMaskBits(ZoneBits.MaskBit(2)));
    }

    [Fact]
    public void LinksTravelWithTheLook()
    {
        var src = new PluginConfig { MaskCMode = 4, MaskCLinked = true };
        var dst = new PluginConfig();
        LookStore.Apply(LookStore.Capture(src), dst, LookStore.Part.All);
        Assert.True(dst.MaskCLinked);
        Assert.Equal(LookStore.Part.Other, LookStore.PartOf("MaskCLinked"));
    }
}
