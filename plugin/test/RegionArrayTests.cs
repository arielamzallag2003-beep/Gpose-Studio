using System.Linq;
using GPoseStudio;
using Xunit;

public class RegionArrayTests
{
    private static PropertyChange Edit(PluginConfig baseCfg, string overrides, System.Action<PluginConfig> edit)
    {
        var s = new MaskRegions.EditSession();
        s.Begin(baseCfg, overrides);
        edit(s.Variant);
        return new PropertyChange(s.End(baseCfg, overrides));
    }

    private readonly record struct PropertyChange(string? Overrides);

    [Fact]
    public void AVariantDoesNotShareItsArraysWithTheWholeImage()
    {
        var baseCfg = new PluginConfig();
        var variant = new PluginConfig();
        MaskRegions.BuildVariant(baseCfg, "", variant);

        Assert.False(ReferenceEquals(baseCfg.FgField, variant.FgField));
        Assert.False(ReferenceEquals(baseCfg.Elem, variant.Elem));
        Assert.False(ReferenceEquals(baseCfg.ElemImages, variant.ElemImages));

        variant.FgField[0] = 0.5f;
        variant.Elem[0] = 9f;
        variant.ElemImages[0] = "not-mine.png";
        Assert.NotEqual(0.5f, baseCfg.FgField[0]);
        Assert.NotEqual(9f, baseCfg.Elem[0]);
        Assert.Equal("", baseCfg.ElemImages[0]);
    }

    [Fact]
    public void TheVariantStartsAsACopyOfTheWholeImage()
    {
        var baseCfg = new PluginConfig();
        baseCfg.FgField[7] = 0.375f;
        baseCfg.Elem[3] = 0.44f;
        var variant = new PluginConfig();
        MaskRegions.BuildVariant(baseCfg, "", variant);

        Assert.Equal(0.375f, variant.FgField[7]);
        Assert.Equal(0.44f, variant.Elem[3]);
    }

    [Fact]
    public void TheForegroundFieldIsOwnable()
    {
        Assert.Contains(MaskRegions.Overridable, p => p.Name == nameof(PluginConfig.FgField));
    }

    [Fact]
    public void EditingItInsideAMaskBecomesThatMasksOwnSetting()
    {
        var baseCfg = new PluginConfig();
        float was = baseCfg.FgField[9];

        var change = Edit(baseCfg, "", v => v.FgField[9] = was + 0.25f);

        Assert.NotNull(change.Overrides);
        Assert.Contains(nameof(PluginConfig.FgField), MaskRegions.Names(change.Overrides));
        Assert.Equal(was, baseCfg.FgField[9]);

        var variant = new PluginConfig();
        MaskRegions.BuildVariant(baseCfg, change.Overrides, variant);
        Assert.Equal(was + 0.25f, variant.FgField[9]);
    }

    [Fact]
    public void PuttingItBackStopsBeingADifference()
    {
        var baseCfg = new PluginConfig();
        float was = baseCfg.FgField[9];
        var first = Edit(baseCfg, "", v => v.FgField[9] = was + 0.25f).Overrides;

        var second = Edit(baseCfg, first!, v => v.FgField[9] = was).Overrides;

        Assert.DoesNotContain(nameof(PluginConfig.FgField), MaskRegions.Names(second));
    }

    [Fact]
    public void TwoVariantsFromOneOverrideDoNotShareItsArray()
    {
        var baseCfg = new PluginConfig();
        var ov = Edit(baseCfg, "", v => v.FgField[2] = 0.8f).Overrides;

        var a = new PluginConfig();
        var b = new PluginConfig();
        MaskRegions.BuildVariant(baseCfg, ov, a);
        MaskRegions.BuildVariant(baseCfg, ov, b);
        a.FgField[2] = 0.1f;

        Assert.Equal(0.8f, b.FgField[2]);
    }

    [Theory]
    [InlineData("{\"FgField\":[1,2,3]}")]
    [InlineData("{\"FgField\":\"nope\"}")]
    [InlineData("{\"FgField\":null}")]
    public void APackOfTheWrongShapeIsIgnored(string overrides)
    {
        var into = new PluginConfig();
        float was = into.FgField[0];
        MaskRegions.ApplyOverrides(overrides, into);
        Assert.Equal(was, into.FgField[0]);
        Assert.Empty(MaskRegions.Names(overrides));
    }

    [Fact]
    public void APackOfTheRightLengthIsTaken()
    {
        var full = new float[new PluginConfig().FgField.Length];
        full[5] = 0.6f;
        string ov = "{\"FgField\":[" + string.Join(",", full.Select(f => f.ToString(System.Globalization.CultureInfo.InvariantCulture))) + "]}";

        var into = new PluginConfig();
        Assert.Equal(1, MaskRegions.ApplyOverrides(ov, into));
        Assert.Equal(0.6f, into.FgField[5]);
    }

    [Fact]
    public void ElementLayersAreStillNotOwnable()
    {
        Assert.DoesNotContain(MaskRegions.Overridable, p => p.Name == nameof(PluginConfig.Elem));

        var baseCfg = new PluginConfig();
        var change = Edit(baseCfg, "", v => v.Elem[1] = 0.33f);

        Assert.Null(change.Overrides);
        Assert.Equal(0.33f, baseCfg.Elem[1]);
    }

    [Fact]
    public void WhatTheDefaultsHoldSurvivesTheScratchRoundTrip()
    {
        var c = new PluginConfig();
        var before = (float[])c.FgField.Clone();
        var scratch = new PluginConfig();
        for (int field = 0; field < 2; field++)
        {
            c.CopyFgToScratch(scratch, field);
            c.CopyFgFromScratch(scratch, field);
        }
        Assert.Equal(before, c.FgField);
    }

    [Fact]
    public void TheScratchRoundTripSettlesAfterOnePass()
    {
        var c = new PluginConfig();
        for (int i = 0; i < c.FgField.Length; i++) c.FgField[i] = (i * 7 % 23) * 0.041f;
        foreach (int slot in new[] { 6, 10, 55, 60, 65, 70, 71, 72, 83, 84, 89, 90, 95, 100 })
            for (int field = 0; field < 2; field++)
                if (field * 111 + slot < c.FgField.Length) c.FgField[field * 111 + slot] = slot % 5;

        var scratch = new PluginConfig();
        void Trip()
        {
            for (int field = 0; field < 2; field++)
            {
                c.CopyFgToScratch(scratch, field);
                c.CopyFgFromScratch(scratch, field);
            }
        }

        Trip();
        var settled = (float[])c.FgField.Clone();
        Trip();

        Assert.Equal(settled, c.FgField);
    }
}
