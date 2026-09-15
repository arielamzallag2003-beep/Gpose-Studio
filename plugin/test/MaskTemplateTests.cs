using System;
using System.IO;
using System.Linq;
using GPoseStudio;
using Xunit;

public class MaskTemplateTests
{
    public static TheoryData<MaskTemplate> Every()
    {
        var d = new TheoryData<MaskTemplate>();
        foreach (MaskTemplate t in Enum.GetValues(typeof(MaskTemplate))) d.Add(t);
        return d;
    }

    [Fact]
    public void EveryTemplateIsListedOnce()
        => Assert.Equal(Enum.GetValues(typeof(MaskTemplate)).Length, MaskTemplates.All.Select(i => i.Kind).Distinct().Count());

    [Theory]
    [MemberData(nameof(Every))]
    public void ATemplateLeavesTheWholeImageAlone(MaskTemplate t)
    {
        var c = new PluginConfig { Exposure = 0.3f, Saturation = -0.2f, ZoneGlow = 7 | ZoneBits.MaskBit(3) };
        MaskTemplates.Apply(c, t);
        Assert.Equal(0.3f, c.Exposure);
        Assert.Equal(-0.2f, c.Saturation);
        Assert.Equal(7, c.ZoneGlow);
        Assert.True(c.AnyMaskSetUp());
    }

    [Theory]
    [MemberData(nameof(Every))]
    public void OwnSettingsNameOnlyWhatAMaskMayOwn(MaskTemplate t)
    {
        var c = new PluginConfig();
        MaskTemplates.Apply(c, t);
        var owned = MaskRegions.Overridable.Select(p => p.Name).ToHashSet();
        for (int i = 0; i < PluginConfig.MaskCount; i++)
        {
            var text = c.MaskOverrides(i);
            if (text.Length == 0) continue;
            var d = System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<string, System.Text.Json.JsonElement>>(text)!;
            foreach (var k in d.Keys) Assert.Contains(k, owned);
        }
    }

    [Theory]
    [MemberData(nameof(Every))]
    public void ATemplateSurvivesTheLookRoundTrip(MaskTemplate t)
    {
        var src = new PluginConfig();
        MaskTemplates.Apply(src, t);
        var dst = new PluginConfig();
        LookStore.Apply(LookStore.Capture(src), dst, LookStore.Part.All);
        Assert.Equal(LookStore.Capture(src), LookStore.Capture(dst));
    }

    [Fact]
    public void RenderEveryTemplate()
    {
        var dir = Environment.GetEnvironmentVariable("GPS_MASK_PREVIEW");
        if (string.IsNullOrEmpty(dir)) return;
        Directory.CreateDirectory(dir);
        foreach (MaskTemplate t in Enum.GetValues(typeof(MaskTemplate)))
        {
            var c = new PluginConfig();
            MaskTemplates.Apply(c, t);
            MaskPreview.Write(dir, t.ToString(), c);
        }
        MaskPreview.Write(dir, "_scene", new PluginConfig());
    }
}
