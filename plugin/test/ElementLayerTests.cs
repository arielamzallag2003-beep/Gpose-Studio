using System;
using GPoseStudio;
using Xunit;

public class ElementLayerTests
{
    private const int S = PluginConfig.ElemStride;

    [Fact]
    public void TheStrideIsWhatTheShaderPacksAgainst()
    {
        Assert.Equal(24, S);
        Assert.Equal(0, S % 4);
        Assert.Equal(8 * S, new PluginConfig().Elem.Length);
    }

    [Fact]
    public void AConfigFromBeforeTheGlowRowIsWidenedTwice()
    {
        var old = new float[8 * 16];
        for (int L = 0; L < 8; L++) { old[L * 16] = L + 1; old[L * 16 + 11] = 0.5f; }

        var widened = PackedArray.Widen(old, 8 * S, 8, 16, S, 16);

        for (int L = 0; L < 8; L++)
        {
            Assert.Equal(L + 1, widened[L * S]);
            Assert.Equal(0.5f, widened[L * S + 11]);
            Assert.Equal(0f, widened[L * S + PluginConfig.ElemBlend]);
            Assert.Equal(0f, widened[L * S + PluginConfig.ElemFlags]);
            Assert.Equal(0f, widened[L * S + PluginConfig.ElemFeather]);
        }
    }

    [Fact]
    public void AConfigFromTheGlowEraIsWidenedOnce()
    {
        var old = new float[8 * 20];
        for (int L = 0; L < 8; L++) { old[L * 20] = 18f; old[L * 20 + 16] = 0.75f; }

        var widened = PackedArray.Widen(old, 8 * S, 8, 20, S, 20);

        for (int L = 0; L < 8; L++)
        {
            Assert.Equal(18f, widened[L * S]);
            Assert.Equal(0.75f, widened[L * S + 16]);
        }
    }

    [Fact]
    public void TheFourPackedThingsDoNotDisturbEachOther()
    {
        var c = new PluginConfig();

        c.SetElemFlag(3, PluginConfig.ElemFlagFlipH, true);
        c.SetElemFit(3, 2);
        c.SetElemMasks(3, ZoneBits.MaskBit(1));
        c.SetElemFlag(3, PluginConfig.ElemFlagFlipV, true);

        Assert.True(c.ElemFlag(3, PluginConfig.ElemFlagFlipH));
        Assert.True(c.ElemFlag(3, PluginConfig.ElemFlagFlipV));
        Assert.Equal(2, c.ElemFit(3));
        Assert.Equal(ZoneBits.MaskBit(1), c.ElemMasks(3));

        c.SetElemFlag(3, PluginConfig.ElemFlagFlipH, false);
        Assert.False(c.ElemFlag(3, PluginConfig.ElemFlagFlipH));
        Assert.True(c.ElemFlag(3, PluginConfig.ElemFlagFlipV));
        Assert.Equal(2, c.ElemFit(3));
        Assert.Equal(ZoneBits.MaskBit(1), c.ElemMasks(3));
    }

    [Fact]
    public void ALayersMaskBitsAreTheSameBitsEveryOtherEffectUses()
    {
        var c = new PluginConfig();
        c.SetElemMasks(0, ZoneBits.MaskBit(0) | ZoneBits.MaskBit(2) | 7);
        Assert.Equal(ZoneBits.MaskBit(0) | ZoneBits.MaskBit(2), c.ElemMasks(0));
        Assert.Equal(0, ZoneBits.ZonePart(c.ElemMasks(0)));
    }

    [Fact]
    public void FlagsOnOneLayerDoNotReachAnother()
    {
        var c = new PluginConfig();
        c.SetElemFit(0, 3);
        Assert.Equal(3, c.ElemFit(0));
        for (int L = 1; L < 8; L++) Assert.Equal(0, c.ElemFit(L));
    }

    [Fact]
    public void DuplicatingALayerCopiesTheWholeBlockAndItsImage()
    {
        var c = new PluginConfig();
        for (int k = 0; k < S; k++) c.Elem[2 * S + k] = k + 1;
        c.ElemImages[2] = "overlay.png";

        c.CopyElemSlot(2, 5);

        for (int k = 0; k < S; k++) Assert.Equal(k + 1, c.Elem[5 * S + k]);
        Assert.Equal("overlay.png", c.ElemImages[5]);
        Assert.Equal(1f, c.Elem[2 * S]);
        Assert.Equal("overlay.png", c.ElemImages[2]);
    }

    [Fact]
    public void ClearingALayerClearsExactlyThatLayer()
    {
        var c = new PluginConfig();
        for (int L = 0; L < 8; L++) { c.Elem[L * S] = 7f; c.ElemImages[L] = "x.png"; }

        c.ClearElemSlot(4);

        Assert.Equal(0f, c.Elem[4 * S]);
        Assert.Equal("", c.ElemImages[4]);
        for (int L = 0; L < 8; L++)
            if (L != 4) { Assert.Equal(7f, c.Elem[L * S]); Assert.Equal("x.png", c.ElemImages[L]); }
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, 9)]
    [InlineData(3, 3)]
    public void OutOfRangeCopiesDoNothingRatherThanThrow(int from, int to)
    {
        var c = new PluginConfig();
        c.Elem[0] = 5f;
        c.CopyElemSlot(from, to);
        Assert.Equal(5f, c.Elem[0]);
    }

    [Fact]
    public void ALayerSurvivesTheLookRoundTrip()
    {
        var src = new PluginConfig();
        src.Elem[S + PluginConfig.ElemBlend] = 4f;
        src.SetElemFit(1, 3);
        src.SetElemMasks(1, ZoneBits.MaskBit(2));
        src.Elem[S + PluginConfig.ElemFeather] = 0.125f;

        var dst = new PluginConfig();
        LookStore.Apply(LookStore.Capture(src), dst, LookStore.Part.All);

        Assert.Equal(4f, dst.Elem[S + PluginConfig.ElemBlend]);
        Assert.Equal(3, dst.ElemFit(1));
        Assert.Equal(ZoneBits.MaskBit(2), dst.ElemMasks(1));
        Assert.Equal(0.125f, dst.Elem[S + PluginConfig.ElemFeather]);
    }
}
