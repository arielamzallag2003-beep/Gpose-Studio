using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace GPoseStudio;

public static class GposePanel
{
    public readonly record struct Rect(float X, float Y, float W, float H);

    private static readonly string[] Addons =
    {
        "GroupPoseGuide",
        "CameraSetting",
        "GroupPoseStamp",
        "SelectYesno",
    };

    public static unsafe Rect[] GetRects()
    {
        List<Rect>? rects = null;
        foreach (var name in Addons)
        {
            nint p;
            try { p = Services.GameGui.GetAddonByName(name, 1); }
            catch { continue; }
            if (p == 0) continue;

            var addon = (AtkUnitBase*)p;
            if (addon == null || !addon->IsVisible || addon->RootNode == null) continue;

            float scale = addon->Scale;
            float w = addon->RootNode->Width * scale;
            float h = addon->RootNode->Height * scale;
            if (w <= 1f || h <= 1f) continue;

            (rects ??= new List<Rect>()).Add(new Rect(addon->X, addon->Y, w, h));
        }
        return rects?.ToArray() ?? Array.Empty<Rect>();
    }
}
