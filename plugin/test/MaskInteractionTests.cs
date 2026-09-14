using System.Linq;
using GPoseStudio;
using Xunit;

public class MaskInteractionTests
{
    [Fact]
    public void TheCombineModeLivesBesideTheZonesAndMasksWithoutDisturbingThem()
    {
        int bits = 0b101 | ZoneBits.MaskBit(0) | ZoneBits.MaskBit(2);
        int moded = ZoneBits.WithMaskMode(bits, ZoneBits.MaskMinus);

        Assert.Equal(ZoneBits.MaskMinus, ZoneBits.MaskMode(moded));
        Assert.Equal(0b101, ZoneBits.ZonePart(moded));
        Assert.Equal(ZoneBits.MaskBit(0) | ZoneBits.MaskBit(2), ZoneBits.MaskPart(moded));
    }

    [Fact]
    public void AnOldLookReadsAsBoth()
        => Assert.Equal(ZoneBits.MaskBoth, ZoneBits.MaskMode(0b111 | ZoneBits.MaskBit(1)));

    [Fact]
    public void TogglingAZoneOrAMaskKeepsTheMode()
    {
        int bits = ZoneBits.WithMaskMode(0b010 | ZoneBits.MaskBit(0), ZoneBits.MaskEither);
        Assert.Equal(ZoneBits.MaskEither, ZoneBits.MaskMode(ZoneBits.ToggleZone(bits, 4)));
        Assert.Equal(ZoneBits.MaskEither, ZoneBits.MaskMode(ZoneBits.ToggleMask(bits, ZoneBits.MaskBit(1))));
    }

    [Fact]
    public void AnElementLayerKeepsItsCombineModeAndNothingElseMoves()
    {
        var c = new PluginConfig();
        c.SetElemFlag(2, PluginConfig.ElemFlagFlipH, true);
        c.SetElemFit(2, 3);
        int wanted = ZoneBits.WithMaskMode(ZoneBits.MaskBit(0) | ZoneBits.MaskBit(1), ZoneBits.MaskOnlyOne);
        c.SetElemMasks(2, wanted | 0b111);

        Assert.Equal(wanted, c.ElemMasks(2));
        Assert.True(c.ElemFlag(2, PluginConfig.ElemFlagFlipH));
        Assert.Equal(3, c.ElemFit(2));
    }

    [Fact]
    public void TheDefaultStackIsTheOrderPassesWereAlwaysDrawnIn()
        => Assert.Equal(Enumerable.Range(0, PluginConfig.MaskCount), new PluginConfig().MaskRegionStack());

    [Fact]
    public void TiesFallBackToLetterOrder()
    {
        var c = new PluginConfig { MaskARegionRank = 1, MaskBRegionRank = 1, MaskCRegionRank = 1 };
        Assert.Equal(new[] { 0, 1, 2 }, c.MaskRegionStack().Take(3));
    }

    private static PluginConfig WithRegions(params int[] masks)
    {
        var c = new PluginConfig();
        foreach (var i in masks)
        {
            c.SetMaskMode(i, 1);
            c.SetMaskOverrides(i, "{\"Exposure\":0.5}");
        }
        return c;
    }

    [Fact]
    public void MovingUpSwapsWithTheNextMaskAbove()
    {
        var c = WithRegions(0, 1, 2);
        c.MoveMaskRegion(0, up: true);
        Assert.Equal(new[] { 1, 0, 2 }, c.MaskRegionStack().Take(3));
        c.MoveMaskRegion(2, up: false);
        Assert.Equal(new[] { 1, 2, 0 }, c.MaskRegionStack().Take(3));
    }

    [Fact]
    public void AMoveSkipsMasksWithoutOwnSettings()
    {
        var c = WithRegions(0, 2);
        c.MoveMaskRegion(0, up: true);
        var stack = c.MaskRegionStack();
        Assert.True(System.Array.IndexOf(stack, 0) > System.Array.IndexOf(stack, 2), "A should now be above C");
    }

    [Fact]
    public void AMoveAtTheTopChangesNothing()
    {
        var c = WithRegions(0, 1, 2);
        c.MoveMaskRegion(2, up: true);
        Assert.Equal(new[] { 0, 1, 2 }, c.MaskRegionStack().Take(3));
    }

    [Fact]
    public void TopWinsDrawsEachMaskOnceBottomFirst()
    {
        var plan = MaskRegions.PlanPieces(new[] { 2, 0 }, overlap: 0);
        Assert.Equal(new[] { ZoneBits.MaskBit(2), ZoneBits.MaskBit(0) }, plan.Select(p => p.Inside));
        Assert.All(plan, p => Assert.Equal(0, p.Outside));
    }

    [Fact]
    public void CancelKeepsEachMaskClearOfTheOthers()
    {
        var plan = MaskRegions.PlanPieces(new[] { 0, 1 }, overlap: 2);
        Assert.Equal(2, plan.Count);
        Assert.Equal(ZoneBits.MaskBit(1), plan[0].Outside);
        Assert.Equal(ZoneBits.MaskBit(0), plan[1].Outside);
    }

    [Fact]
    public void CombineTilesTheFrameWithAPassPerCombination()
    {
        var active = new[] { 1, 0, 2 };
        int all = ZoneBits.MaskBit(0) | ZoneBits.MaskBit(1) | ZoneBits.MaskBit(2);
        var plan = MaskRegions.PlanPieces(active, overlap: 1);

        Assert.Equal(7, plan.Count);
        Assert.Equal(7, plan.Select(p => p.Inside).Distinct().Count());
        Assert.All(plan, p => Assert.Equal(all, p.Inside | p.Outside));
        Assert.All(plan, p => Assert.Equal(0, p.Inside & p.Outside));
        var pair = plan.Single(p => p.Inside == (ZoneBits.MaskBit(0) | ZoneBits.MaskBit(1)));
        Assert.Equal(new[] { 1, 0 }, pair.Masks);
        Assert.Equal(3, plan.Last().Masks.Length);
    }

    [Fact]
    public void CombineWithOneMaskIsJustThatMask()
    {
        var plan = MaskRegions.PlanPieces(new[] { 1 }, overlap: 1);
        var only = Assert.Single(plan);
        Assert.Equal(ZoneBits.MaskBit(1), only.Inside);
        Assert.Equal(0, only.Outside);
    }

    [Fact]
    public void MergedSettingsLetTheHigherMaskDecideAConflict()
    {
        var merged = MaskRegions.Merge(new[] { "{\"Exposure\":0.5,\"Contrast\":0.2}", "{\"Exposure\":-0.5,\"Saturation\":-1}" });
        var v = new PluginConfig();
        MaskRegions.ApplyOverrides(merged, v);
        Assert.Equal(-0.5f, v.Exposure);
        Assert.Equal(0.2f, v.Contrast);
        Assert.Equal(-1f, v.Saturation);
    }

    [Theory]
    [InlineData("MaskRegionOverlap")] [InlineData("MaskARegionRank")] [InlineData("MaskCRegionRank")]
    public void OverlapSettingsStayWithTheMasks(string name)
        => Assert.Equal(LookStore.Part.Other, LookStore.PartOf(name));

    [Fact]
    public void OverlapSettingsTravelWithTheLook()
    {
        var src = new PluginConfig { MaskRegionOverlap = 1, MaskARegionRank = 2, MaskCRegionRank = 0 };
        var dst = new PluginConfig();
        LookStore.Apply(LookStore.Capture(src), dst, LookStore.Part.All);
        Assert.Equal(1, dst.MaskRegionOverlap);
        Assert.Equal(new[] { 2, 1, 0 }, dst.MaskRegionStack().Take(3));
    }
}
