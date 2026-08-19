using GPoseStudio;
using Xunit;

public class FrameTests
{
    private static Frame.Opts Opts(float mat = 0f, float corner = 0f, float keyline = 0f,
                                   float shadow = 0f, bool alpha = false, bool inset = false,
                                   float bottom = 0f, float smooth = 0f)
        => new(inset, smooth, corner, mat, 0f, keyline, shadow, bottom,
               0.9f, 0.9f, 0.88f, 0.1f, 0.1f, 0.1f, alpha);

    private static byte[] Solid(int w, int h, byte r, byte g, byte b, byte a = 255)
    {
        var px = new byte[w * h * 4];
        for (int i = 0; i < w * h; i++)
        {
            px[i * 4] = r; px[i * 4 + 1] = g; px[i * 4 + 2] = b; px[i * 4 + 3] = a;
        }
        return px;
    }

    [Fact]
    public void NothingSetIsRecognisedAsNoWork()
    {
        Assert.True(Opts().IsNoOp);
        Assert.False(Opts(mat: 0.05f).IsNoOp);
        Assert.False(Opts(corner: 0.05f).IsNoOp);
        Assert.False(Opts(keyline: 0.01f).IsNoOp);
        Assert.False(Opts(shadow: 0.5f).IsNoOp);
    }

    [Fact]
    public void OutOfRangeValuesAreClampedOnTheWayIn()
    {
        var o = new Opts_Probe();
        Assert.Equal(0.4f, o.MatTooBig.Mat);
        Assert.Equal(0f, o.MatNegative.Mat);
        Assert.Equal(0.5f, o.CornerTooBig.Corner);
        Assert.Equal(1f, o.SmoothTooBig.Smooth);
    }

    private sealed class Opts_Probe
    {
        public readonly Frame.Opts MatTooBig = new(false, 0, 0, 9f, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, false);
        public readonly Frame.Opts MatNegative = new(false, 0, 0, -3f, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, false);
        public readonly Frame.Opts CornerTooBig = new(false, 0, 9f, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, false);
        public readonly Frame.Opts SmoothTooBig = new(false, 9f, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, false);
    }

    [Fact]
    public void AMatGrowsTheImageByTheSameMarginOnBothAxes()
    {
        const int w = 400, h = 200;
        var (ow, oh, _) = Frame.Compose(w, h, Solid(w, h, 20, 30, 40), Opts(mat: 0.1f), allowAlpha: true);

        int addedW = ow - w, addedH = oh - h;
        Assert.Equal(addedW, addedH);
        Assert.True(addedW > 0, "a mat should make the image bigger");
    }

    [Fact]
    public void AMatSurroundsThePhotoWithMatColour()
    {
        const int w = 64, h = 64;
        var (ow, oh, px) = Frame.Compose(w, h, Solid(w, h, 255, 0, 0), Opts(mat: 0.15f), allowAlpha: true);

        Assert.True(px[0] > 200 && px[1] > 200, "corner should be the light mat colour");
        int c = ((oh / 2) * ow + ow / 2) * 4;
        Assert.True(px[c] > 200 && px[c + 1] < 40, "centre should still be the red photo");
    }

    [Fact]
    public void InsetKeepsTheCanvasSizeInsteadOfGrowingIt()
    {
        const int w = 200, h = 120;
        var (ow, oh, _) = Frame.Compose(w, h, Solid(w, h, 10, 10, 10), Opts(mat: 0.1f, inset: true), allowAlpha: true);
        Assert.Equal(w, ow);
        Assert.Equal(h, oh);
    }

    [Fact]
    public void RoundedCornersAreTransparentOnlyWhenAlphaIsAllowed()
    {
        const int w = 80, h = 80;

        var (_, _, opaque) = Frame.Compose(w, h, Solid(w, h, 200, 200, 200),
                                           Opts(corner: 0.3f, alpha: true), allowAlpha: false);
        Assert.Equal(255, opaque[3]);

        var (ow, _, clear) = Frame.Compose(w, h, Solid(w, h, 200, 200, 200),
                                           Opts(corner: 0.3f, alpha: true), allowAlpha: true);
        Assert.True(clear[3] < 255, "a rounded corner should be transparent in a PNG");
        int mid = (ow / 2) * 4;
        Assert.Equal(255, clear[mid + 3]);
    }

    [Fact]
    public void ANoOpComposeReturnsTheImageUnchanged()
    {
        const int w = 32, h = 16;
        var src = Solid(w, h, 1, 2, 3);
        var (ow, oh, px) = Frame.Compose(w, h, src, Opts(), allowAlpha: true);
        Assert.Equal(w, ow);
        Assert.Equal(h, oh);
        Assert.Equal(src, px);
    }
}
