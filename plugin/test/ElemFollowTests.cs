using System;
using GPoseStudio;
using Xunit;

public class ElemFollowTests
{
    private const int S = PluginConfig.ElemStride;

    private static PluginConfig Ring(float x, float y)
    {
        var c = new PluginConfig { MaskAMode = 1, MaskACx = 0.5f, MaskACy = 0.5f, MaskASize = 0.2f, MaskAAngle = 0f };
        c.Elem[0] = 1f; c.Elem[1] = x; c.Elem[2] = y; c.Elem[3] = 0.2f; c.Elem[5] = 0f;
        c.SetElemFollowMask(0, 0);
        c.SyncElemFollowers(1f);
        return c;
    }

    [Fact]
    public void NothingFollowsByDefault()
    {
        var c = new PluginConfig();
        for (int L = 0; L < 8; L++) Assert.Equal(-1, c.ElemFollowMask(L));
    }

    [Fact]
    public void TheFirstSyncMovesNothing()
    {
        var c = Ring(0.1f, 0.2f);
        Assert.False(c.SyncElemFollowers(1f));
        Assert.Equal(0.1f, c.Elem[1], 4);
        Assert.Equal(0.2f, c.Elem[2], 4);
    }

    [Fact]
    public void MovingTheMaskMovesTheLayer()
    {
        var c = Ring(0.1f, 0.2f);
        c.MaskACx = 0.6f; c.MaskACy = 0.4f;
        Assert.True(c.SyncElemFollowers(16f / 9f));
        Assert.Equal(0.2f, c.Elem[1], 4);
        Assert.Equal(0.3f, c.Elem[2], 4);
    }

    [Fact]
    public void TurningTheMaskSwingsTheLayerRoundItAndTurnsItAlike()
    {
        var c = Ring(0.1f, 0f);
        c.MaskAAngle = MathF.PI / 2f;
        c.SyncElemFollowers(1f);
        Assert.Equal(0f, c.Elem[1], 4);
        Assert.Equal(-0.1f, c.Elem[2], 4);
        Assert.Equal(MathF.PI / 2f, c.Elem[5], 4);
    }

    [Fact]
    public void ResizingScalesTheDistanceAndTheSize()
    {
        var c = Ring(0.1f, 0f);
        c.Elem[7] = 0.004f;
        c.MaskASize = 0.4f;
        c.SyncElemFollowers(1f);
        Assert.Equal(0.2f, c.Elem[1], 4);
        Assert.Equal(0.4f, c.Elem[3], 4);
        Assert.Equal(0.004f, c.Elem[7], 4);
    }

    [Fact]
    public void ALinkedGroupMoveCarriesTheLayer()
    {
        var c = Ring(0.1f, 0f);
        c.MoveMaskGroup(new[] { 0 }, new[] { c.PoseOf(0) }, 0.5f, 0.5f, 0.1f, 0f, 0f, 1f, 1f);
        c.SyncElemFollowers(1f);
        Assert.Equal(0.2f, c.Elem[1], 4);
    }

    [Fact]
    public void ApplyingALookIsNotTheMaskMoving()
    {
        var src = Ring(-0.2f, 0.1f);
        src.MaskACx = 0.3f;
        src.SyncElemFollowers(1f);
        float x = src.Elem[1];

        var dst = Ring(0.1f, 0f);
        LookStore.Apply(LookStore.Capture(src), dst, LookStore.Part.All);
        dst.SyncElemFollowers(1f);
        Assert.Equal(x, dst.Elem[1], 4);
        Assert.Equal(0, dst.ElemFollowMask(0));
    }

    [Fact]
    public void DeletingTheMaskLeavesTheLayerWhereItIs()
    {
        var c = Ring(0.1f, 0f);
        Assert.Contains("ElemFollow0", c.MaskUsers(0));
        c.ClearMask(0);
        Assert.Equal(-1, c.ElemFollowMask(0));
        Assert.Equal(0.1f, c.Elem[1], 4);
    }

    [Fact]
    public void RemovingItFromUsedByStopsTheFollowOnly()
    {
        var c = Ring(0.1f, 0f);
        c.RemoveMaskUser(0, "ElemFollow0");
        Assert.Equal(-1, c.ElemFollowMask(0));
        Assert.Equal(1f, c.Elem[0]);
    }

    [Fact]
    public void TheResetButtonClearsLayersButAPresetsResetKeepsThem()
    {
        var kept = Ring(0.1f, 0f);
        kept.ResetLook();
        Assert.Equal(1f, kept.Elem[0]);

        var wiped = Ring(0.1f, 0f);
        wiped.ElemImages[0] = "logo.png";
        wiped.ResetLook(keepPlaced: false);
        Assert.Equal(0f, wiped.Elem[0]);
        Assert.Equal(-1, wiped.ElemFollowMask(0));
        Assert.Equal("", wiped.ElemImages[0] ?? "");
    }

    [Fact]
    public void AMaskWithNothingOnScreenIsNotFollowed()
    {
        var c = Ring(0.1f, 0f);
        c.MaskAMode = 6;
        c.MaskACx = 0.9f;
        Assert.False(c.SyncElemFollowers(1f));
        Assert.Equal(0.1f, c.Elem[1], 4);
    }
}
