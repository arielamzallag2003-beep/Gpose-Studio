using GPoseStudio;
using Xunit;

public class FrameOrderTests
{
    private static PluginConfig Frames(params int[] masks)
    {
        var c = new PluginConfig();
        foreach (var i in masks) { c.SetMaskMode(i, 4); c.SetMaskFrame(i, true); }
        return c;
    }

    [Fact]
    public void OnlyFramesAreInTheStackAndLaterLettersStartOnTop()
    {
        var c = Frames(0, 1, 3);
        c.MaskCMode = 1;
        Assert.Equal(new[] { 0, 1, 3 }, c.MaskFrameStack());
    }

    [Fact]
    public void MovingAFrameUpPutsItOverTheOneAbove()
    {
        var c = Frames(0, 1);
        c.MoveMaskFrame(0, up: true);
        Assert.Equal(new[] { 1, 0 }, c.MaskFrameStack());
        Assert.True(c.MaskAFrameRank > c.MaskBFrameRank);
    }

    [Fact]
    public void AMovePastTheEndChangesNothing()
    {
        var c = Frames(0, 1);
        c.MoveMaskFrame(1, up: true);
        Assert.Equal(new[] { 0, 1 }, c.MaskFrameStack());
    }

    [Fact]
    public void TheOldBehaviourIsTheDefault()
    {
        var c = new PluginConfig();
        Assert.False(c.MaskFramesStacked);
        Assert.False(c.MaskOutlineBehind);
    }

    [Fact]
    public void OrderAndOptionsTravelWithTheLookAndStayWithTheMasks()
    {
        var src = Frames(0, 1);
        src.MaskFramesStacked = true; src.MaskOutlineBehind = true;
        src.MoveMaskFrame(0, up: true);
        var dst = new PluginConfig();
        LookStore.Apply(LookStore.Capture(src), dst, LookStore.Part.All);
        Assert.True(dst.MaskFramesStacked);
        Assert.True(dst.MaskOutlineBehind);
        Assert.Equal(new[] { 1, 0 }, dst.MaskFrameStack());
        Assert.Equal(LookStore.Part.Other, LookStore.PartOf("MaskAFrameRank"));
        Assert.Equal(LookStore.Part.Other, LookStore.PartOf("MaskFramesStacked"));
    }

    [Fact]
    public void DeletingAMaskResetsItsPlaceInTheStack()
    {
        var c = Frames(0, 1);
        c.MoveMaskFrame(0, up: true);
        c.ClearMask(0);
        Assert.Equal(0, c.MaskAFrameRank);
    }
}
