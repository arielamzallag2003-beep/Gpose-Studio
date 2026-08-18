using System;
using Dalamud.Plugin.Services;

namespace GPoseStudio;

public sealed class GposeGate : IDisposable
{
    private bool _last;
    public bool IsActive => _last;
    public event Action<bool>? Changed;

    public GposeGate() => Services.Framework.Update += OnUpdate;

    private void OnUpdate(IFramework _)
    {
        var now = Services.ClientState.IsGPosing;
        if (now == _last) return;
        _last = now;
        try { Changed?.Invoke(now); }
        catch (Exception ex) { Services.Log.Error(ex, "GposeGate.Changed handler threw"); }
    }

    public void Dispose() => Services.Framework.Update -= OnUpdate;
}
