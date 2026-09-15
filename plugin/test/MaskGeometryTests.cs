using System;
using GPoseStudio;
using Xunit;

public class MaskGeometryTests
{
    private const float Eps = 2e-3f;

    private static float At(int mode, float x, float y, float size = 0.2f, float ell = 1f, float sides = 5f, float detail = 0.5f, float round = 0f)
        => MaskGeometry.Inside(mode, x, y, size, ell, sides, detail, round);

    [Fact]
    public void AHexagonReachesItsCornersAndItsFlatSides()
    {
        float apothem = 0.2f * MathF.Cos(MathF.PI / 6f);
        Assert.Equal(apothem, At(9, 0f, 0f, sides: 6f), 3);
        Assert.InRange(At(9, 0f, apothem, sides: 6f), -Eps, Eps);
        Assert.InRange(At(9, 0.2f, 0f, sides: 6f), -Eps, Eps);
    }

    [Fact]
    public void ATriangleStandsOnItsBaseWithItsPointUp()
    {
        Assert.InRange(At(9, 0f, 0.1f, sides: 3f), -Eps, Eps);
        Assert.InRange(At(9, 0f, -0.2f, sides: 3f), -Eps, Eps);
        Assert.True(At(9, 0f, -0.15f, sides: 3f) > 0f);
    }

    [Fact]
    public void AStarHasItsTipsAndNotchesWhereTheNumbersSay()
    {
        Assert.InRange(At(10, 0f, -0.2f, detail: 0.5f), -Eps, Eps);
        float a = MathF.PI / 5f;
        Assert.InRange(At(10, MathF.Sin(a) * 0.1f, -MathF.Cos(a) * 0.1f, detail: 0.5f), -Eps, Eps);
        Assert.True(At(10, 0f, 0f, detail: 0.5f) > 0f);
        Assert.True(At(10, MathF.Sin(a) * 0.15f, -MathF.Cos(a) * 0.15f, detail: 0.5f) < 0f);
    }

    [Fact]
    public void RoundingARectangleLeavesItsSidesAndCutsItsCorners()
    {
        Assert.InRange(At(4, 0.2f, 0f, round: 0.5f), -Eps, Eps);
        Assert.InRange(At(4, 0.2f, 0.2f, round: 0f), -Eps, Eps);
        Assert.Equal(-(MathF.Sqrt(0.02f) - 0.1f), At(4, 0.2f, 0.2f, round: 0.5f), 3);
    }

    [Fact]
    public void AWedgeOpensAlongItsDirectionAndEndsAtItsReach()
    {
        Assert.True(At(11, 0.5f, 0f, size: 1f, detail: 0.25f) > 0f);
        Assert.True(At(11, 0f, 0.5f, size: 1f, detail: 0.25f) < 0f);
        float c = MathF.Cos(MathF.PI / 4f) * 0.5f;
        Assert.InRange(At(11, c, c, size: 1f, detail: 0.25f), -Eps, Eps);
        Assert.True(At(11, 1.2f, 0f, size: 1f, detail: 0.25f) < 0f);
    }

    [Fact]
    public void StripesRepeatWithTheirBandWidth()
    {
        Assert.Equal(0.02f, At(12, 0f, 0f, size: 0.1f, detail: 0.4f), 3);
        Assert.Equal(0.02f, At(12, 0.3f, 0.7f, size: 0.1f, detail: 0.4f), 3);
        Assert.Equal(-0.03f, At(12, 0.05f, 0f, size: 0.1f, detail: 0.4f), 3);
    }

    [Fact]
    public void ACrossCoversItsArmsAndNotItsCorners()
    {
        Assert.True(At(13, 0.15f, 0f) > 0f);
        Assert.True(At(13, 0f, 0.15f) > 0f);
        Assert.True(At(13, 0.15f, 0.15f) < 0f);
        Assert.True(At(13, 0f, 0.35f, ell: 2f) > 0f);
    }

    [Fact]
    public void ZeroSidesAndZeroDetailMeanTheDefaults()
    {
        Assert.Equal(At(9, 0.05f, 0.07f, sides: 5f), At(9, 0.05f, 0.07f, sides: 0f));
        Assert.Equal(At(10, 0.05f, 0.07f, detail: 0.5f), At(10, 0.05f, 0.07f, detail: 0f));
    }

    [Theory]
    [InlineData(9)] [InlineData(10)] [InlineData(13)] [InlineData(4)] [InlineData(1)]
    public void TheDrawnOutlineLiesOnTheEdge(int mode)
    {
        var loop = MaskGeometry.Contour(mode, 0f, 0.2f, 1.3f, 7f, 0.4f, 0.3f);
        Assert.NotEmpty(loop);
        foreach (var p in loop)
            Assert.InRange(MaskGeometry.Inside(mode, p.X, p.Y, 0.2f, 1.3f, 7f, 0.4f, 0.3f), -Eps, Eps);
    }

    [Fact]
    public void ShapesWithoutAnOutlineHaveNoDistance()
    {
        foreach (int mode in new[] { 0, 3, 6, 7, 8, 14, 15, 16 })
            Assert.True(float.IsNaN(At(mode, 0f, 0f)));
    }
}
