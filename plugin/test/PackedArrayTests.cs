using GPoseStudio;
using Xunit;

public class PackedArrayTests
{
    [Fact]
    public void EachBlockLandsAtItsNewStride()
    {
        var src = new float[] { 1, 2, 3, 4, 5, 6 };
        var dst = PackedArray.Widen(src, length: 10, blocks: 2,
                                    oldStride: 3, newStride: 5, copyPerBlock: 3);

        Assert.Equal(new float[] { 1, 2, 3, 0, 0, 4, 5, 6, 0, 0 }, dst);
    }

    [Fact]
    public void TheRealForegroundMigrationPlacesTheSecondFieldCorrectly()
    {
        const int oldStride = 89, newStride = 111;
        var src = new float[2 * oldStride + 2];
        src[0] = 11f;
        src[oldStride] = 22f;
        src[oldStride + 88] = 33f;

        var dst = PackedArray.Widen(src, length: 2 * newStride + 2, blocks: 2,
                                    oldStride: oldStride, newStride: newStride, copyPerBlock: 89);

        Assert.Equal(11f, dst[0]);
        Assert.Equal(22f, dst[newStride]);
        Assert.Equal(33f, dst[newStride + 88]);
        Assert.Equal(0f, dst[newStride - 1]);
    }

    [Fact]
    public void TheRealElementMigrationPlacesEveryLayer()
    {
        const int oldStride = 16, newStride = 20;
        var src = new float[8 * oldStride];
        for (int layer = 0; layer < 8; layer++) src[layer * oldStride] = layer + 1;

        var dst = PackedArray.Widen(src, length: 8 * newStride, blocks: 8,
                                    oldStride: oldStride, newStride: newStride, copyPerBlock: 16);

        for (int layer = 0; layer < 8; layer++)
            Assert.Equal(layer + 1, dst[layer * newStride]);
    }

    [Fact]
    public void NewSlotsAreZeroBecauseZeroMeansOff()
    {
        var dst = PackedArray.Widen(new float[] { 7, 7 }, length: 8, blocks: 2,
                                    oldStride: 1, newStride: 4, copyPerBlock: 1);
        Assert.Equal(new float[] { 7, 0, 0, 0, 7, 0, 0, 0 }, dst);
    }

    [Fact]
    public void ANullOrShortSourceYieldsZeroesRatherThanThrowing()
    {
        Assert.Equal(new float[6], PackedArray.Widen(null, 6, 2, 3, 3, 3));

        var dst = PackedArray.Widen(new float[] { 1, 2, 3, 4 }, length: 10, blocks: 2,
                                    oldStride: 3, newStride: 5, copyPerBlock: 3);
        Assert.Equal(1f, dst[0]);
        Assert.Equal(3f, dst[2]);
        Assert.Equal(0f, dst[5]);
    }

    [Fact]
    public void ImpossibleStridesAreRefusedRatherThanGuessed()
    {
        Assert.Equal(new float[8], PackedArray.Widen(new float[] { 1, 2, 3, 4 }, 8, 2, 2, 4, 3));
        Assert.Equal(new float[8], PackedArray.Widen(new float[] { 1, 2, 3, 4 }, 8, 2, 4, 2, 3));
    }
}
