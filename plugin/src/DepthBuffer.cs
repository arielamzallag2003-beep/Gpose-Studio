using FFXIVClientStructs.FFXIV.Client.Graphics.Render;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;

namespace GPoseStudio;

public static class DepthBuffer
{
    public readonly record struct Handle(nint Srv, float ScaleX, float ScaleY);

    private static readonly Handle None = new(0, 1f, 1f);

    public static unsafe Handle TryGet()
    {
        try
        {
            var rtm = RenderTargetManager.Instance();
            if (rtm == null) return None;

            Texture* depth = rtm->DepthStencil;
            if (depth == null) return None;

            nint srv = (nint)depth->D3D11ShaderResourceView;
            if (srv == 0) return None;

            float sx = 1f, sy = 1f;
            uint aw = depth->ActualWidth, ah = depth->ActualHeight;
            uint allocW = depth->AllocatedWidth, allocH = depth->AllocatedHeight;
            if (aw > 0 && ah > 0 && allocW > 0 && allocH > 0)
            {
                sx = (float)aw / allocW;
                sy = (float)ah / allocH;
            }

            return new Handle(srv, sx, sy);
        }
        catch
        {
            return None;
        }
    }
}
