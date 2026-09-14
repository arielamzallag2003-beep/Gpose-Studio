using System.Linq;
using GPoseStudio;
using Xunit;

public class EightMaskTests
{
    [Fact]
    public void EveryIndexedAccessorReachesItsOwnMask()
    {
        var c = new PluginConfig();
        for (int i = 0; i < PluginConfig.MaskCount; i++)
        {
            c.SetMaskMode(i, 1 + i % 8); c.SetMaskCx(i, 0.1f * i); c.SetMaskCy(i, 0.05f * i);
            c.SetMaskSize(i, 0.02f * i); c.SetMaskEllipse(i, 1f + i); c.SetMaskAngle(i, -0.1f * i);
            c.SetMaskFeather(i, 0.01f * i); c.SetMaskInvert(i, i % 2 == 1); c.SetMaskFrame(i, i % 3 == 0);
            c.SetMaskOutline(i, 0.001f * i); c.SetMaskOutColor(i, 0.1f * i, 0.2f, 0.3f);
            c.SetMaskOverrides(i, "{\"Exposure\":" + (0.1 * i).ToString(System.Globalization.CultureInfo.InvariantCulture) + "}");
            c.SetMaskRegionOn(i, i % 2 == 0); c.SetMaskRegionMix(i, 0.1f * i); c.SetMaskRegionRank(i, 7 - i);
        }
        for (int i = 0; i < PluginConfig.MaskCount; i++)
        {
            char letter = PluginConfig.MaskLetter(i);
            object Prop(string field) => typeof(PluginConfig).GetProperty("Mask" + letter + field)!.GetValue(c)!;
            Assert.Equal(1 + i % 8, (int)Prop("Mode"));
            Assert.Equal(0.1f * i, (float)Prop("Cx"));
            Assert.Equal(0.05f * i, (float)Prop("Cy"));
            Assert.Equal(0.02f * i, (float)Prop("Size"));
            Assert.Equal(1f + i, (float)Prop("Ellipse"));
            Assert.Equal(-0.1f * i, (float)Prop("Angle"));
            Assert.Equal(0.01f * i, (float)Prop("Feather"));
            Assert.Equal(i % 2 == 1, (bool)Prop("Invert"));
            Assert.Equal(i % 3 == 0, (bool)Prop("Frame"));
            Assert.Equal(0.001f * i, (float)Prop("Outline"));
            Assert.Equal(0.1f * i, (float)Prop("OutR"));
            Assert.Equal(c.MaskOverrides(i), (string)Prop("Overrides"));
            Assert.Equal(i % 2 == 0, (bool)Prop("RegionOn"));
            Assert.Equal(0.1f * i, (float)Prop("RegionMix"));
            Assert.Equal(7 - i, (int)Prop("RegionRank"));
            Assert.Equal(c.MaskMode(i), (int)Prop("Mode"));
        }
    }

    [Fact]
    public void TheBitsForZonesMasksAndModeNeverOverlap()
    {
        Assert.Equal(0, ZoneBits.Zones & ZoneBits.Masks);
        Assert.Equal(0, ZoneBits.Masks & ZoneBits.MaskModeBits);
        for (int i = 0; i < PluginConfig.MaskCount; i++)
            Assert.Equal(ZoneBits.MaskBit(i), ZoneBits.MaskBit(i) & ZoneBits.Masks);
        Assert.Equal(new[] { 8, 16, 32 }, Enumerable.Range(0, 3).Select(ZoneBits.MaskBit));
    }

    [Fact]
    public void AnElementLayerCanUseMaskHAndACombineMode()
    {
        var c = new PluginConfig();
        c.SetElemFit(5, 2);
        int wanted = ZoneBits.WithMaskMode(ZoneBits.MaskBit(7) | ZoneBits.MaskBit(0), ZoneBits.MaskMinus);
        c.SetElemMasks(5, wanted);
        Assert.Equal(wanted, c.ElemMasks(5));
        Assert.Equal(2, c.ElemFit(5));
    }

    [Fact]
    public void DeletingAMaskLetsGoOfEverythingAimedAtIt()
    {
        var c = new PluginConfig { MaskAMode = 1, MaskCMode = 4, MaskCFrame = true, MaskCOverrides = "{\"Exposure\":0.5}" };
        c.ZoneGlow = 7 | ZoneBits.MaskBit(0) | ZoneBits.MaskBit(2);
        c.Elem[1 * PluginConfig.ElemStride] = 1f;
        c.SetElemMasks(1, ZoneBits.MaskBit(2));
        c.ExportCutoutMasks = ZoneBits.MaskBit(2) | ZoneBits.MaskBit(0);

        c.ClearMask(2);

        Assert.Equal(0, c.MaskCMode);
        Assert.False(c.MaskCFrame);
        Assert.Equal("", c.MaskCOverrides);
        Assert.Equal(7 | ZoneBits.MaskBit(0), c.ZoneGlow);
        Assert.Equal(0, c.ElemMasks(1));
        Assert.Equal(ZoneBits.MaskBit(0), c.ExportCutoutMasks);
        Assert.Equal(1, c.MaskAMode);
        Assert.Empty(c.MaskUsers(2));
    }

    [Fact]
    public void UsersAreListedByNameAndCanBeRemovedOneByOne()
    {
        var c = new PluginConfig { MaskDMode = 1 };
        c.ZoneFog = 7 | ZoneBits.MaskBit(3);
        c.ZoneGrade |= ZoneBits.MaskBit(3);
        c.Elem[4 * PluginConfig.ElemStride] = 2f;
        c.SetElemMasks(4, ZoneBits.MaskBit(3));

        var users = c.MaskUsers(3);
        Assert.Contains("ZoneFog", users);
        Assert.Contains("ZoneGrade", users);
        Assert.Contains("Elem4", users);
        Assert.Equal(users.Count, c.MaskSubscribers(3));

        c.RemoveMaskUser(3, "Elem4");
        c.RemoveMaskUser(3, "ZoneFog");
        Assert.Equal(new[] { "ZoneGrade" }, c.MaskUsers(3));
        Assert.Equal(7, c.ZoneFog);
    }

    [Fact]
    public void ANewMaskGoesInTheFirstFreeSlot()
    {
        var c = new PluginConfig { MaskAMode = 1, MaskBMode = 2 };
        Assert.Equal(2, c.FirstFreeMask());
        for (int i = 0; i < PluginConfig.MaskCount; i++) c.SetMaskMode(i, 1);
        Assert.Equal(-1, c.FirstFreeMask());
    }

    [Fact]
    public void TheExportKeepsTheFramesWhenThereAreAny()
    {
        var c = new PluginConfig { MaskAMode = 4, MaskAFrame = true, MaskBMode = 1, MaskEMode = 4, MaskEFrame = true };
        int cut = c.DefaultCutoutMasks();
        Assert.Equal(ZoneBits.MaskBit(0) | ZoneBits.MaskBit(4), ZoneBits.MaskPart(cut));
        Assert.Equal(ZoneBits.MaskEither, ZoneBits.MaskMode(cut));

        var plain = new PluginConfig { MaskBMode = 1, MaskCMode = 2 };
        Assert.Equal(ZoneBits.MaskBit(1) | ZoneBits.MaskBit(2), ZoneBits.MaskPart(plain.DefaultCutoutMasks()));
    }

    [Fact]
    public void TheExportCutTravelsWithTheLookWhoseMasksItNames()
    {
        var c = new PluginConfig { ExportCutoutMasks = ZoneBits.MaskBit(1), ExportCutoutSubject = false, ExportExcludeMasks = ZoneBits.MaskBit(2) };
        var dst = new PluginConfig();
        LookStore.Apply(LookStore.Capture(c), dst, LookStore.Part.All);
        Assert.Equal(ZoneBits.MaskBit(1), dst.ExportCutoutMasks);
        Assert.False(dst.ExportCutoutSubject);
        Assert.Equal(ZoneBits.MaskBit(2), dst.ExportExcludeMasks);
    }

    [Theory]
    [InlineData("MaskHOverrides")] [InlineData("MaskDFrame")] [InlineData("MaskGRegionRank")]
    public void TheNewMasksClassifyWithTheOthers(string name)
        => Assert.Equal(LookStore.Part.Other, LookStore.PartOf(name));

    [Fact]
    public void CombineFallsBackToTopWinsPastItsLimit()
    {
        var plan = MaskRegions.PlanPieces(new[] { 0, 1, 2, 3 }, overlap: 1);
        Assert.Equal(4, plan.Count);
        Assert.All(plan, p => Assert.Equal(0, p.Outside));
    }

    [Fact]
    public void MaskHRoundTripsThroughALook()
    {
        var src = new PluginConfig { MaskHMode = 5, MaskHCx = 0.3f, MaskHOverrides = "{\"Saturation\":-1}" };
        var dst = new PluginConfig();
        LookStore.Apply(LookStore.Capture(src), dst, LookStore.Part.All);
        Assert.Equal(5, dst.MaskHMode);
        Assert.Equal(0.3f, dst.MaskHCx);
        Assert.Equal("{\"Saturation\":-1}", dst.MaskHOverrides);
    }

    [Fact]
    public void RemovedMasksAreListedClearedAndSavedWithTheLook()
    {
        var c = new PluginConfig { MaskFMode = 1, ExportExcludeMasks = ZoneBits.MaskBit(5) };
        Assert.Contains(nameof(PluginConfig.ExportExcludeMasks), c.MaskUsers(5));
        Assert.Contains("ExportExcludeMasks", LookStore.Capture(c));

        c.RemoveMaskUser(5, nameof(PluginConfig.ExportExcludeMasks));
        Assert.Equal(0, c.ExportExcludeMasks);

        c.ExportExcludeMasks = ZoneBits.MaskBit(5);
        c.ClearMask(5);
        Assert.Equal(0, c.ExportExcludeMasks);
    }
}
