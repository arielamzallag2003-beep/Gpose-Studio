using GPoseStudio;
using Xunit;

public class ZoneBitsTests
{
    [Fact]
    public void TogglingAZoneLeavesTheMasksAlone()
    {
        int bits = 0b010 | ZoneBits.MaskBit(0) | ZoneBits.MaskBit(2);

        int after = ZoneBits.ToggleZone(bits, 4);

        Assert.Equal(0b110, ZoneBits.ZonePart(after));
        Assert.Equal(ZoneBits.MaskBit(0) | ZoneBits.MaskBit(2), ZoneBits.MaskPart(after));
    }

    [Fact]
    public void TurningOffTheLastZoneTurnsItBackOn()
    {
        int after = ZoneBits.ToggleZone(0b010, 2);
        Assert.Equal(0b010, ZoneBits.ZonePart(after));
    }

    [Fact]
    public void TheZoneFloorDoesNotDestroyMaskBitsOnTheWayThrough()
    {
        int bits = 0b010 | ZoneBits.MaskBit(1);

        int after = ZoneBits.ToggleZone(bits, 2);

        Assert.Equal(0b010, ZoneBits.ZonePart(after));
        Assert.Equal(ZoneBits.MaskBit(1), ZoneBits.MaskPart(after));
    }

    [Fact]
    public void TogglingAMaskLeavesTheZonesAlone()
    {
        int bits = 0b101;

        int on = ZoneBits.ToggleMask(bits, ZoneBits.MaskBit(1));
        Assert.Equal(0b101, ZoneBits.ZonePart(on));
        Assert.Equal(ZoneBits.MaskBit(1), ZoneBits.MaskPart(on));

        int off = ZoneBits.ToggleMask(on, ZoneBits.MaskBit(1));
        Assert.Equal(0b101, ZoneBits.ZonePart(off));
        Assert.Equal(0, ZoneBits.MaskPart(off));
    }

    [Fact]
    public void SubscribingToNothingIsAllowed()
    {
        Assert.Equal(0, ZoneBits.MaskPart(ZoneBits.ToggleMask(ZoneBits.MaskBit(0) | 7, ZoneBits.MaskBit(0))));
    }

    [Theory]
    [InlineData(0, 8)]
    [InlineData(1, 16)]
    [InlineData(2, 32)]
    public void MaskBitsSitAboveTheZones(int index, int expected)
    {
        Assert.Equal(expected, ZoneBits.MaskBit(index));
        Assert.Equal(0, ZoneBits.MaskBit(index) & ZoneBits.Zones);
    }

    [Fact]
    public void AnOldLookReadsAsNoMasks()
    {
        for (int bits = 0; bits <= 7; bits++)
        {
            Assert.Equal(0, ZoneBits.MaskPart(bits));
            Assert.Equal(bits, ZoneBits.ZonePart(bits));
        }
    }

    [Fact]
    public void SubscribersAreCountedAcrossEveryEffect()
    {
        var c = new PluginConfig
        {
            ZoneSkin = 2 | ZoneBits.MaskBit(0),
            ZoneGobo = 7 | ZoneBits.MaskBit(0),
            ZoneSpot = 7 | ZoneBits.MaskBit(1),
        };

        Assert.Equal(2, c.MaskSubscribers(0));
        Assert.Equal(1, c.MaskSubscribers(1));
        Assert.Equal(0, c.MaskSubscribers(2));
    }

    [Fact]
    public void ADefaultConfigSubscribesToNothing()
    {
        var c = new PluginConfig();
        for (int i = 0; i < 3; i++) Assert.Equal(0, c.MaskSubscribers(i));
    }
}
