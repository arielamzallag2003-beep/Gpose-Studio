using System.Collections.Generic;
using System.Text.Json;
using GPoseStudio;
using Xunit;

public class MaskOptionTests
{
    private static readonly string[] Names =
        { "Sides", "Detail", "Round", "Rough", "RoughScale", "Strength", "DepthLimit", "Mirror", "Edge", "OutGlow", "Shadow" };

    private static object Prop(PluginConfig c, char letter, string name)
        => typeof(PluginConfig).GetProperty("Mask" + letter + name)!.GetValue(c)!;

    private static void Scribble(PluginConfig c, int i)
    {
        c.SetMaskSides(i, 9f); c.SetMaskDetail(i, 0.2f); c.SetMaskRound(i, 0.7f); c.SetMaskRough(i, 0.05f);
        c.SetMaskRoughScale(i, 20f); c.SetMaskStrength(i, 0.3f); c.SetMaskDepthLimit(i, 2); c.SetMaskMirror(i, 3);
        c.SetMaskEdge(i, 1); c.SetMaskOutGlow(i, 0.6f); c.SetMaskShadow(i, 0.4f);
    }

    private static void AssertDefaults(PluginConfig c, int i)
    {
        var fresh = new PluginConfig();
        char letter = PluginConfig.MaskLetter(i);
        foreach (var n in Names) Assert.Equal(Prop(fresh, letter, n), Prop(c, letter, n));
    }

    [Fact]
    public void TheDefaultsAreTheMasksAsTheyWere()
    {
        var c = new PluginConfig();
        for (int i = 0; i < PluginConfig.MaskCount; i++)
        {
            Assert.Equal(5f, c.MaskSides(i)); Assert.Equal(0.5f, c.MaskDetail(i)); Assert.Equal(0f, c.MaskRound(i));
            Assert.Equal(0f, c.MaskRough(i)); Assert.Equal(8f, c.MaskRoughScale(i)); Assert.Equal(1f, c.MaskStrength(i));
            Assert.Equal(0, c.MaskDepthLimit(i)); Assert.Equal(0, c.MaskMirror(i)); Assert.Equal(0, c.MaskEdge(i));
            Assert.Equal(0f, c.MaskOutGlow(i)); Assert.Equal(0f, c.MaskShadow(i));
        }
    }

    [Fact]
    public void EveryAccessorReachesItsOwnMask()
    {
        var c = new PluginConfig();
        for (int i = 0; i < PluginConfig.MaskCount; i++)
        {
            c.SetMaskSides(i, 3 + i); c.SetMaskDetail(i, 0.1f * i); c.SetMaskRound(i, 0.05f * i); c.SetMaskRough(i, 0.01f * i);
            c.SetMaskRoughScale(i, 2 + i); c.SetMaskStrength(i, 0.1f * i); c.SetMaskDepthLimit(i, i % 3); c.SetMaskMirror(i, i % 4);
            c.SetMaskEdge(i, i % 3); c.SetMaskOutGlow(i, 0.02f * i); c.SetMaskShadow(i, 0.03f * i);
        }
        for (int i = 0; i < PluginConfig.MaskCount; i++)
        {
            char l = PluginConfig.MaskLetter(i);
            Assert.Equal(3f + i, (float)Prop(c, l, "Sides")); Assert.Equal(0.1f * i, (float)Prop(c, l, "Detail"));
            Assert.Equal(0.05f * i, (float)Prop(c, l, "Round")); Assert.Equal(0.01f * i, (float)Prop(c, l, "Rough"));
            Assert.Equal(2f + i, (float)Prop(c, l, "RoughScale")); Assert.Equal(0.1f * i, (float)Prop(c, l, "Strength"));
            Assert.Equal(i % 3, (int)Prop(c, l, "DepthLimit")); Assert.Equal(i % 4, (int)Prop(c, l, "Mirror"));
            Assert.Equal(i % 3, (int)Prop(c, l, "Edge")); Assert.Equal(0.02f * i, (float)Prop(c, l, "OutGlow"));
            Assert.Equal(0.03f * i, (float)Prop(c, l, "Shadow"));
        }
    }

    [Fact]
    public void DeletingAMaskClearsItsOptions()
    {
        var c = new PluginConfig { MaskCMode = 10 };
        Scribble(c, 2);
        c.ClearMask(2);
        AssertDefaults(c, 2);
    }

    [Fact]
    public void ResettingTheLookClearsThem()
    {
        var c = new PluginConfig { MaskAMode = 9 };
        Scribble(c, 0);
        c.ResetLook();
        AssertDefaults(c, 0);
    }

    [Fact]
    public void APresetWritesTheOptionsItDoesNotUseBackToDefault()
    {
        var c = new PluginConfig();
        Scribble(c, 1);
        c.ApplyMaskPreset(1, PluginConfig.MaskPreset.Face);
        Assert.Equal(0f, c.MaskBRough);
        Assert.Equal(1f, c.MaskBStrength);
        Assert.Equal(0, c.MaskBMirror);
        c.ApplyMaskPreset(1, PluginConfig.MaskPreset.Hexagon);
        Assert.Equal(6f, c.MaskBSides);
    }

    [Fact]
    public void ALookSavedBeforeTheOptionsLeavesNoneBehindOnTheMasksItNames()
    {
        var src = new PluginConfig { MaskAMode = 1 };
        var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(LookStore.Capture(src))!;
        foreach (var n in Names) dict.Remove("MaskA" + n);
        foreach (var key in new List<string>(dict.Keys))
            if (key.StartsWith("MaskC", System.StringComparison.Ordinal)) dict.Remove(key);

        var dst = new PluginConfig { MaskAMode = 10, MaskCMode = 9 };
        Scribble(dst, 0);
        Scribble(dst, 2);
        LookStore.Apply(JsonSerializer.Serialize(dict), dst, LookStore.Part.All);

        AssertDefaults(dst, 0);
        Assert.Equal(1, dst.MaskAMode);
        Assert.Equal(9f, dst.MaskCSides);
        Assert.Equal(9, dst.MaskCMode);
    }

    [Fact]
    public void APartialLoadLeavesTheMasksAlone()
    {
        var src = new PluginConfig { MaskAMode = 1, Exposure = 0.3f };
        var dst = new PluginConfig { MaskAMode = 10 };
        Scribble(dst, 0);
        LookStore.Apply(LookStore.Capture(src), dst, LookStore.Part.Grade);
        Assert.Equal(0.05f, dst.MaskARough);
        Assert.Equal(10, dst.MaskAMode);
    }

    [Fact]
    public void TheOptionsTravelWithTheLook()
    {
        var src = new PluginConfig { MaskEMode = 12 };
        Scribble(src, 4);
        var dst = new PluginConfig();
        LookStore.Apply(LookStore.Capture(src), dst, LookStore.Part.All);
        Assert.Equal(0.2f, dst.MaskEDetail);
        Assert.Equal(3, dst.MaskEMirror);
        Assert.Equal(0.4f, dst.MaskEShadow);
    }

    [Theory]
    [InlineData("MaskASides")] [InlineData("MaskHShadow")] [InlineData("MaskDDepthLimit")] [InlineData("MaskBOutGlow")]
    public void TheOptionsClassifyWithTheMasks(string name)
        => Assert.Equal(LookStore.Part.Other, LookStore.PartOf(name));

    [Fact]
    public void AMaskCannotOwnItsOwnOptions()
    {
        foreach (var p in MaskRegions.Overridable) Assert.False(p.Name.StartsWith("Mask", System.StringComparison.Ordinal));
    }
}
