using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.ImGuiFileDialog;
using Dalamud.Bindings.ImGui;

namespace GPoseStudio.Ui;

public sealed class MainWindow : Window, IDisposable
{
    private readonly GposeGate _gate;
    private readonly LiveOverlay _live;
    private readonly FileDialogManager _dialogs = new();
    private static readonly PluginConfig Defaults = new();
    private static readonly Vector4 AccentCol = new(0.62f, 0.74f, 0.95f, 1f);
    private string _lookFilter = "";
    private string _fontFilter = "";
    private string _pendingLoad = "";
    private string _lookSel = "";
    private string _confirmDelete = "";
    private readonly List<string> _undo = new();
    private readonly List<string> _redo = new();
    private string _baseline = "";
    private const int UndoDepth = 60;
    private int _applyPart;
    private int _savePart;

    private string _peekBackup = "";

    private string _loadedSnapshot = "";
    private bool _editedSinceLoad;

    private static readonly string[] UiScopeModes = { "Both", "Foreground only", "Background only" };
    private static readonly string[] UiBases = { "Linear", "Radial", "Diamond", "Conic", "Mirror", "Spiral" };
    private static readonly string[] UiNoises = { "None", "Fractal (fbm)", "Ridged veins", "Voronoi cells", "Turbulence", "Warped fractal", "Billow (puffy)", "Marble veins", "Rings / wood", "Cracks", "Dots / cells", "Weave" };
    private static readonly string[] UiBlends = { "Warp gradient (soft)", "Add (light)", "Multiply", "Overlay", "Mix to accent", "Shade (show field)" };
    private static readonly string[] UiFgPlace = { "Edges (vignette-in)", "Corners", "Top", "Bottom", "Left", "Right", "Radial (centre)", "Directional", "Full frame" };
    private static readonly string[] UiFgBlend = { "Over", "Add (glow)", "Screen", "Multiply (void)" };
    private static readonly string[] UiFgDepth = { "Everything", "Near / subject", "Far / background" };
    private static readonly string[] UiApplyPart =
        { "Everything", "Grade", "Background", "Light", "Subject", "Camera", "FX", "Overlays" };
    private static readonly string[] UiGroundMode = { "Placed blob", "Cast from silhouette" };
    private static readonly string[] UiGobo = { "Venetian blinds", "Window frame", "Lace / web", "Foliage dapple" };
    private static readonly string[] UiUpats = { "None", "Stripes", "Checker", "Dots", "Grid", "Rings", "Sunburst", "Hexagons", "Waves",
        "Triangles", "Diamonds", "Brick", "Fish scales", "Chevron", "Truchet weave", "Cells", "Quatrefoil", "Basket weave", "Spiral",
        "Data stream", "Circuit", "Seigaiha waves", "Girih star", "Gothic tracery", "Constellation",
        "Frost crystals", "Flames", "Wind", "Lightning" };
    private static readonly string[] UiPatBlend = { "Ink (replace)", "Glow (add)", "Shade (darken)" };
    private static readonly string[] UiPatMat = { "Flat (ink)", "Metal leaf", "Enamel", "Foil (iridescent)" };
    private static readonly string[] UiPatColMode = { "Solid", "Gradient (two colors)", "Palette (5 colors)", "Field palette", "Palette by intensity", "Field palette by intensity" };
    private static readonly string[] UiGrounds = { "Plain", "Mirror (reflects the sky)", "Perspective grid", "Water (rippled)", "Ice / cracked plane", "Molten (lava veins)" };
    private static readonly string[] UiOrbs = { "Off", "Disc (sun / moon)", "Ring (halo / eclipse)", "Glow only" };
    private static readonly string[] UiParts = { "Off", "Stars", "Snow", "Sparks / embers", "Bokeh (multi-colour)", "Bubbles", "Petals" };
    private static readonly string[] UiModes = { "Linear (angled split)", "Radial (portal)", "Depth (near / far)", "Whole frame (no split)" };
    private static readonly string[] UiStyles = { "Off", "Solid", "Vertical gradient", "Radial", "Horizontal", "Stripes", "Checker", "Dots", "Diagonal", "Spiral", "Grid lines", "Sunburst", "Concentric waves", "Noise clouds", "Nebula", "Starfield", "Void dimension", "Aquarium (underwater)", "Aurora borealis", "Synthwave", "Blood moon", "Tempe's red moon", "Forge", "Artisan's rest (8 crafts)", "Sunset", "Sin Eater (Light corruption)", "Custom (gradient + pattern + noise)", "Universal (procedural engine)", "Evercold (frost)", "Chinese ink (sumi-e)" };
    private static readonly int[] UiShown = { 0, 27, 26, 29, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 28 };
    private static readonly string[] UiGrad = { "Linear", "Radial", "Diamond", "Conic" };
    private static readonly string[] UiPats = { "None", "Stripes", "Checker", "Dots", "Grid", "Rings", "Sunburst", "Spiral" };
    private static readonly string[] UiTypes = { "— off —", "Ring", "Disc", "Polygon", "Star", "Cross", "Rectangle", "Arc", "Line", "Corner brackets", "Reticle", "Radar", "Rangefinder", "Telemetry", "AoE circle", "AoE donut", "AoE cone", "AoE line", "Image" };
    private static readonly string[] UiTypeTag = { "", "O", "\u25cf", "P", "\u2605", "+", "\u25ad", "(", "\u2014", "[", "\u2295", "R", "|", "\u2261", "A", "D", "C", "L", "IMG" };
    private static readonly string[] UiElemBlend = { "Auto", "Normal (over)", "Add (light)", "Screen", "Multiply", "Overlay", "Soft light" };
    private static readonly string[] UiElemFit = { "Fit inside", "Fill (crop)", "Stretch", "Tile" };
    private static readonly string[] UiMixes = { "Spatial seam (split the frame)", "Where B is bright (flames / energy)",
                           "Where B is dark (smoke / shadow / ink)", "Screen (both glow)",
                           "Max (brightest wins)", "Multiply (B darkens A)", "Marbled interleave" };
    private static readonly (string label, int bit)[] UiWarps = {
                    ("Swirl", 1), ("Bulge / pinch", 2), ("Kaleidoscope", 4), ("Wave", 8), ("Ripple", 16),
                };
    private static readonly string[] UiAligns = { "Left", "Center", "Right" };
    private readonly PluginConfig _scratch = new();
    private string _statusText = "";
    private DateTime _statusAt = DateTime.MinValue;
    private string _status
    {
        get => (DateTime.UtcNow - _statusAt).TotalSeconds > 8 ? "" : _statusText;
        set { _statusText = value; _statusAt = DateTime.UtcNow; }
    }
    private bool _dirty;
    private bool _savePending;
    private int _elemSlot;
    private int _textSel;
    private string _lookName = "";
    private List<string> _lookList = new();

    public MainWindow(GposeGate gate, LiveOverlay live) : base("GPoseStudio###gposestudio_main")
    {
        _gate = gate;
        _live = live;
        _lookList = LookStore.List();
        Size = new Vector2(500, 600);
        SizeCondition = ImGuiCond.FirstUseEver;
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(440, 380),
            MaximumSize = new Vector2(1000, 1400),
        };
    }

    private const int SaveQuietMs = 500;
    private const int SaveMaxWaitMs = 4000;
    private long _saveQuietAt, _saveDeadline;

    private void FlushSave(PluginConfig cfg)
    {
        if (!_savePending) return;
        PushUndo(cfg);
        cfg.Save();
        _savePending = false;
    }

    public override void OnClose() => FlushSave(Plugin.Config);

    public override void Draw()
    {
        var cfg = Plugin.Config;
        _dirty = false;

        DrawHeader(cfg);
        ImGui.Separator();
        DrawPresets(cfg);
        ImGui.Spacing();

        DrawFinder(cfg);

        if (_filter != FilterMode.None)
        {
            DrawAllBodies(cfg);
        }
        else
        using (var bar = ImRaii.TabBar("##gps_tabs"))
        {
            if (bar)
            {
                DrawLooksTab(cfg);
                DrawLookTab(cfg);
                DrawCameraTab(cfg);
                DrawLightTab(cfg);
                DrawSubjectTab(cfg);
                DrawBackgroundTab(cfg);
                DrawFxTab(cfg);
                DrawOverlaysTab(cfg);
                DrawExportTab(cfg);
            }
        }

        if (_status.Length > 0)
        {
            ImGui.Separator();
            ImGui.TextWrapped(_status);
        }

        long nowMs = Environment.TickCount64;
        if (_dirty)
        {
            if (!_savePending) _saveDeadline = nowMs + SaveMaxWaitMs;
            _savePending = true;
            _saveQuietAt = nowMs + SaveQuietMs;
        }
        if (_savePending && !ImGui.IsAnyItemActive()
            && (nowMs >= _saveQuietAt || nowMs >= _saveDeadline))
            FlushSave(cfg);
        _dialogs.Draw();
    }

    private void DrawHeader(PluginConfig cfg)
    {
        var dl = ImGui.GetWindowDrawList();
        var p = ImGui.GetCursorScreenPos();
        const float r = 20f;
        Logo.Draw(dl, new Vector2(p.X + r, p.Y + r + 2f), r);
        ImGui.Dummy(new Vector2(r * 2 + 8f, r * 2 + 4f));
        ImGui.SameLine();

        ImGui.BeginGroup();
        ImGui.SetWindowFontScale(1.35f);
        ImGui.TextUnformatted("GPoseStudio");
        ImGui.SetWindowFontScale(1.0f);
        ImGui.TextDisabled("Live gpose grading");
        ImGui.EndGroup();

        string pill = _gate.IsActive ? "● GPOSE" : "○ idle";
        var col = _gate.IsActive ? new Vector4(0.40f, 0.85f, 0.45f, 1f) : new Vector4(0.6f, 0.6f, 0.6f, 1f);
        ImGui.SameLine();
        float rightX = ImGui.GetContentRegionMax().X - ImGui.CalcTextSize(pill).X;
        if (rightX > ImGui.GetCursorPosX()) ImGui.SetCursorPosX(rightX);
        ImGui.TextColored(col, pill);

        ImGui.Spacing();
        var live = cfg.LivePreview;
        if (ImGui.Checkbox("Live preview (apply over the game)", ref live))
        {
            cfg.LivePreview = live; _live.Enabled = live; _dirty = true;
        }
        ImGui.SameLine();
        float btnX = ImGui.GetContentRegionMax().X - 268f;
        if (btnX > ImGui.GetCursorPosX()) ImGui.SetCursorPosX(btnX);
        bool canUndo = _undo.Count > 0, canRedo = _redo.Count > 0;
        using (ImRaii.Disabled(!canUndo))
            if (ImGui.Button(canUndo ? $"Undo {_undo.Count}" : "Undo", new Vector2(76f, 0)) && canUndo) Undo(cfg);
        if (ImGui.IsItemHovered())
            ImGui.SetTooltip(canUndo
                ? $"Step back one edit. {_undo.Count} available.\nA whole slider drag counts as one step."
                : "Nothing to undo yet.");
        ImGui.SameLine();
        using (ImRaii.Disabled(!canRedo))
            if (ImGui.Button(canRedo ? $"Redo {_redo.Count}" : "Redo", new Vector2(76f, 0)) && canRedo) Redo(cfg);
        if (ImGui.IsItemHovered())
            ImGui.SetTooltip(canRedo ? $"Step forward again. {_redo.Count} available." : "Nothing to redo.");
        ImGui.SameLine();
        if (ImGui.Button("Reset look", new Vector2(96f, 0))) { cfg.ResetLook(); _dirty = true; }

        var bypass = cfg.Bypass;
        if (ImGui.Checkbox("Bypass — show the original (A/B compare)", ref bypass)) { cfg.Bypass = bypass; _dirty = true; }

        ImGui.SameLine();
        var frz = cfg.FreezeAnimation;
        if (ImGui.Checkbox("Freeze", ref frz)) { cfg.FreezeAnimation = frz; _dirty = true; }
        if (ImGui.IsItemHovered()) ImGui.SetTooltip(
            "Stop the clock. Everything procedural runs off one time value, so this holds\n" +
            "the whole picture still — the snow where it is, flames mid-lick, an element\n" +
            "mid-spin.\n\n" +
            "A gpose is a photograph of one moment, and until now which moment was not\nsomething you got to choose. Unfreezing carries on from where it was held\nrather than jumping forward by however long you took.");

        ImGui.SameLine();

        var clip = cfg.DebugShowClipping;

        if (ImGui.Checkbox("Clipping", ref clip))
        {
            cfg.DebugShowClipping = clip;
            if (clip) { cfg.DebugShowGate = false; cfg.DebugShowDepth = false; cfg.DebugShowMatte = false; cfg.DebugShowMask = false; }
            _dirty = true;
        }

        if (ImGui.IsItemHovered()) ImGui.SetTooltip(

            "Stripes where detail is being lost.\n" +

            "  red   = highlights blown to white\n" +

            "  blue  = shadows crushed to black\n\n" +

            "A blown highlight and a merely bright one look the same on screen, so grade with\nthis on for a moment before you commit.");
    }

    private void DrawPresets(PluginConfig cfg)
    {
        ImGui.TextDisabled("Presets");
        ImGui.SameLine();
        void Preset(string name, Action<PluginConfig> apply)
        {
            if (ImGui.SmallButton(name)) { cfg.ResetLook(); apply(cfg); _dirty = true; }
            ImGui.SameLine();
        }
        Preset("Neutral", _ => { });
        Preset("Cinematic", c => { c.Contrast = 0.15f; c.Saturation = -0.08f; c.Lift = 0.04f; c.Gain = 0.05f; c.Temperature = 0.05f; c.Tint = -0.03f; c.Vignette = 0.32f; c.Letterbox = 0.5f; });
        Preset("Warm Film", c => { c.Temperature = 0.12f; c.Exposure = 0.08f; c.Contrast = 0.10f; c.Saturation = 0.10f; c.Lift = 0.05f; c.Grain = 0.30f; c.Vignette = 0.25f; });
        Preset("Noir", c => { c.Saturation = -1f; c.Contrast = 0.28f; c.Gamma = -0.05f; c.Sharpen = 0.20f; c.Grain = 0.35f; c.Vignette = 0.42f; });
        Preset("Dreamy", c => { c.Exposure = 0.18f; c.Contrast = -0.08f; c.Lift = 0.12f; c.Gain = 0.08f; c.Vibrance = 0.25f; c.Temperature = 0.04f; c.Vignette = 0.20f; });
        ImGui.NewLine();
        ImGui.TextDisabled("Tip: right-click any slider or swatch to reset just that one.");
    }

    private void DrawLookTab(PluginConfig cfg)
    {
        using var tab = ImRaii.TabItem("Color");
        if (!tab) return;
        LookBody(cfg);
    }

    private void LookBody(PluginConfig cfg)
    {

        if (!Filtering)
        using (var tbl = ImRaii.Table("##image", 2, ImGuiTableFlags.BordersInnerV))
        {
            if (tbl)
            {

            ImGui.TableNextColumn();
            ImGui.TextDisabled("TONE");
            cfg.Exposure = Knob("Exposure", cfg.Exposure, -2f, 2f, Defaults.Exposure, "Overall brightness, in stops (2^x).");
            cfg.Contrast = Knob("Contrast", cfg.Contrast, -0.5f, 0.5f, Defaults.Contrast);
            cfg.Lift = Knob("Lift (shadows)", cfg.Lift, -0.5f, 0.5f, Defaults.Lift, "Raise/lower the darkest tones.");
            cfg.Gamma = Knob("Gamma (mids)", cfg.Gamma, -0.5f, 0.5f, Defaults.Gamma, "Midtone brightness.");
            cfg.Gain = Knob("Gain (highlights)", cfg.Gain, -0.5f, 0.5f, Defaults.Gain, "Lift/lower the brightest tones.");
            cfg.BlackPoint = Knob("Black point", cfg.BlackPoint, 0f, 0.3f, Defaults.BlackPoint, "Crush or raise the blacks.");
            cfg.WhitePoint = Knob("White point", cfg.WhitePoint, 0.6f, 1.2f, Defaults.WhitePoint, "Set the white level.");
            cfg.Clarity = Knob("Clarity", cfg.Clarity, 0f, 1f, Defaults.Clarity, "Local-contrast / detail punch.");
            cfg.HlRecovery = Knob("Highlight recovery", cfg.HlRecovery, 0f, 1f, Defaults.HlRecovery, "Soft shoulder to tame blown highlights.");
            cfg.Denoise = Knob("Denoise", cfg.Denoise, 0f, 1f, Defaults.Denoise, "Edge-preserving smoothing (removes noise/banding).");
            if (cfg.Denoise > 0f) cfg.DenoiseEdge = Knob("Denoise edge", cfg.DenoiseEdge, 0f, 1f, Defaults.DenoiseEdge, "How strongly edges are protected from the smoothing (0 = blur everything, 1 = keep detail).");

            ImGui.TableNextColumn();
            ImGui.TextDisabled("COLOR");
            cfg.Saturation = Knob("Saturation", cfg.Saturation, -1f, 1f, Defaults.Saturation, "-1 = grayscale.");
            cfg.Vibrance = Knob("Vibrance", cfg.Vibrance, -1f, 1f, Defaults.Vibrance, "Saturation that protects vivid tones / skin.");
            cfg.Temperature = Knob("Temperature", cfg.Temperature, -0.3f, 0.3f, Defaults.Temperature, "Cool <-> warm.");
            cfg.Tint = Knob("Tint", cfg.Tint, -0.3f, 0.3f, Defaults.Tint, "Green <-> magenta.");
            cfg.HueShift = Knob("Hue shift", cfg.HueShift, -0.25f, 0.25f, Defaults.HueShift, "Rotate every hue.");
            }
        }

        DrawGradeGroups(cfg);

        using (var fg = GroupEn("Final grade (after everything)", cfg.EnFinalGrade && FinalGradeActive(cfg),
                                cfg.EnFinalGrade, v => cfg.EnFinalGrade = v,
                                zoneGet: () => cfg.ZoneFinal, zoneSet: v => cfg.ZoneFinal = v, maskOnly: true))
        if (fg.Show)
        {
            ImGui.TextDisabled(
                "The controls above grade the CAPTURED frame, before the backdrop, shadows and\n" +
                "glow are composited over it \u2014 so pulling Exposure up there moves your character\n" +
                "and leaves the backdrop where it was. These grade the finished picture instead.\n" +
                "Reach for them last, to push both halves into one image.");
            cfg.FinalExposure = Knob("Final exposure", cfg.FinalExposure, -2f, 2f, Defaults.FinalExposure, "Stops, over the whole composite.");
            cfg.FinalContrast = Knob("Final contrast", cfg.FinalContrast, -1f, 1f, Defaults.FinalContrast, "Around mid grey.");
            cfg.FinalSat = Knob("Final saturation", cfg.FinalSat, -1f, 1f, Defaults.FinalSat, "-1 = grayscale. Useful for pulling a loud backdrop back toward the subject.");
            cfg.FinalTemp = Knob("Final temperature", cfg.FinalTemp, -0.3f, 0.3f, Defaults.FinalTemp, "Cool <-> warm. The one control that can warm subject and backdrop together.");
            cfg.FinalLift = Knob("Final lift", cfg.FinalLift, -0.1f, 0.2f, Defaults.FinalLift, "Raises the blacks of the whole image \u2014 the usual way to seat a composite in one atmosphere.");
            cfg.FinalGamma = Knob("Final gamma", cfg.FinalGamma, -0.5f, 0.5f, Defaults.FinalGamma, "Midtones.");
            cfg.FinalGain = Knob("Final gain", cfg.FinalGain, -0.5f, 0.5f, Defaults.FinalGain, "Highlights.");
            ImGui.TextDisabled("Runs before film response and lens, so the film curve still answers\nthe graded image rather than grading its own output.");
        }
    }

    private void DrawGradeGroups(PluginConfig cfg)
    {
        if (ImGui.Button("Suggest grade from character"))
            _live.SuggestGrade(cfg, r => _status = r);
        if (ImGui.IsItemHovered())
            ImGui.SetTooltip("Samples your character's dominant color and fills the grade-color\neffects with a matching dominant/complement palette (enables Color balance).");

        ImGui.Spacing();
        cfg.Dehaze = Knob("Dehaze", cfg.Dehaze, 0f, 1f, Defaults.Dehaze, "Punch up contrast + color (cuts through haze).");
        ImGui.Spacing();

        var scopeModes = UiScopeModes;
        int scope = cfg.ScopeMode < 0 || cfg.ScopeMode > 2 ? 0 : cfg.ScopeMode;
        ImGui.TextUnformatted("Apply grade to");
        ImGui.SameLine();
        ImGui.PushItemWidth(150f);
        if (ImGui.BeginCombo("##scope", scopeModes[scope]))
        {
            for (int m = 0; m < 3; m++)
                if (ImGui.Selectable(scopeModes[m], scope == m)) { cfg.ScopeMode = m; _dirty = true; }
            ImGui.EndCombo();
        }
        ImGui.PopItemWidth();
        if (scope != 0)
        {
            if (!_live.DepthAvailable)
                ImGui.TextDisabled("Needs depth — enable live preview in gpose.");
            cfg.ScopeSplit = Knob("Scope split", cfg.ScopeSplit, 0.01f, 0.4f, Defaults.ScopeSplit, "Depth that divides subject from background.", "%.3f");
            cfg.ScopeSoft = Knob("Scope softness", cfg.ScopeSoft, 0.005f, 0.2f, Defaults.ScopeSoft, "Transition width.", "%.3f");
        }
        if (cfg.MaskAMode != 0 || cfg.MaskBMode != 0 || cfg.MaskCMode != 0)
        {
            ImGui.TextUnformatted("Limit the grade to");
            ImGui.SameLine();
            MaskToggles("Grade", () => cfg.ZoneGrade, v => cfg.ZoneGrade = v);
            ImGui.NewLine();
        }
        ImGui.Spacing();
        ImGui.TextDisabled("Scene zones — every effect's F / C / B buttons route against these.");
        cfg.ZoneNear = Knob("Foreground split", cfg.ZoneNear, 0f, 0.3f, Defaults.ZoneNear,
            "Depth in front of which the scene counts as FOREGROUND rather than your character.\n0 = no foreground zone at all (the usual portrait case: the character IS the nearest thing),\nwhich is why an effect's F button does nothing until you raise this.\nUse it when a prop, a hand or another character sits closer to camera than the subject.", "%.3f");
        if (cfg.ZoneNear > 0f)
            cfg.ZoneNearSoft = Knob("  Zone softness", cfg.ZoneNearSoft, 0.005f, 0.15f, Defaults.ZoneNearSoft, "Transition width of the foreground boundary.", "%.3f");
        ImGui.Spacing();
        DrawMasksGroup(cfg);
        ImGui.Separator();

        using (var grp = GroupEn("Color balance (3-way)", cfg.ColorBalance > 0f, cfg.EnColorBalance, v => cfg.EnColorBalance = v, true, zoneGet: () => cfg.ZoneCb, zoneSet: v => cfg.ZoneCb = v))
        if (grp.Show)
        {
            cfg.ColorBalance = Knob("Color balance", cfg.ColorBalance, 0f, 1f, Defaults.ColorBalance, "Strength of the per-range tinting.");
            var sh = ColorPick("Shadows", new Vector3(cfg.CbShadowR, cfg.CbShadowG, cfg.CbShadowB), new Vector3(Defaults.CbShadowR, Defaults.CbShadowG, Defaults.CbShadowB));
            cfg.CbShadowR = sh.X; cfg.CbShadowG = sh.Y; cfg.CbShadowB = sh.Z;
            var mi = ColorPick("Midtones", new Vector3(cfg.CbMidR, cfg.CbMidG, cfg.CbMidB), new Vector3(Defaults.CbMidR, Defaults.CbMidG, Defaults.CbMidB));
            cfg.CbMidR = mi.X; cfg.CbMidG = mi.Y; cfg.CbMidB = mi.Z;
            var hg = ColorPick("Highlights", new Vector3(cfg.CbHighR, cfg.CbHighG, cfg.CbHighB), new Vector3(Defaults.CbHighR, Defaults.CbHighG, Defaults.CbHighB));
            cfg.CbHighR = hg.X; cfg.CbHighG = hg.Y; cfg.CbHighB = hg.Z;
        }

        using (var grp = GroupEn("Teal & orange", cfg.TealOrange > 0f, cfg.EnTealOrange, v => cfg.EnTealOrange = v, zoneGet: () => cfg.ZoneTeal, zoneSet: v => cfg.ZoneTeal = v))
        if (grp.Show)
        {
            cfg.TealOrange = Knob("Teal & orange", cfg.TealOrange, 0f, 1f, Defaults.TealOrange, "Cinematic shadow/highlight split.");
            cfg.TealOrangePunch = Knob("Punch", cfg.TealOrangePunch, 1f, 1.6f, Defaults.TealOrangePunch, "Saturation boost.");
            var tos = ColorPick("Shadow hue", new Vector3(cfg.ToShadowR, cfg.ToShadowG, cfg.ToShadowB), new Vector3(Defaults.ToShadowR, Defaults.ToShadowG, Defaults.ToShadowB));
            cfg.ToShadowR = tos.X; cfg.ToShadowG = tos.Y; cfg.ToShadowB = tos.Z;
            var toh = ColorPick("Light hue", new Vector3(cfg.ToHighR, cfg.ToHighG, cfg.ToHighB), new Vector3(Defaults.ToHighR, Defaults.ToHighG, Defaults.ToHighB));
            cfg.ToHighR = toh.X; cfg.ToHighG = toh.Y; cfg.ToHighB = toh.Z;
        }

        using (var grp = GroupEn("Split tone", cfg.StAmount > 0f, cfg.EnSplitTone, v => cfg.EnSplitTone = v, zoneGet: () => cfg.ZoneSplitTone, zoneSet: v => cfg.ZoneSplitTone = v))
        if (grp.Show)
        {
            cfg.StAmount = Knob("Split tone", cfg.StAmount, 0f, 1f, Defaults.StAmount, "Tint shadows and highlights separately.");
            cfg.StBalance = Knob("Balance", cfg.StBalance, 0.2f, 0.8f, Defaults.StBalance, "Pivot between shadows and highlights.");
            var ss = ColorPick("Shadow tint", new Vector3(cfg.StShadowR, cfg.StShadowG, cfg.StShadowB), new Vector3(Defaults.StShadowR, Defaults.StShadowG, Defaults.StShadowB));
            cfg.StShadowR = ss.X; cfg.StShadowG = ss.Y; cfg.StShadowB = ss.Z;
            var sht = ColorPick("Highlight tint", new Vector3(cfg.StHighR, cfg.StHighG, cfg.StHighB), new Vector3(Defaults.StHighR, Defaults.StHighG, Defaults.StHighB));
            cfg.StHighR = sht.X; cfg.StHighG = sht.Y; cfg.StHighB = sht.Z;
        }

        using (var grp = GroupEn("Bleach bypass", cfg.Bleach > 0f, cfg.EnBleach, v => cfg.EnBleach = v, zoneGet: () => cfg.ZoneBleach, zoneSet: v => cfg.ZoneBleach = v))
        if (grp.Show)
        {
            cfg.Bleach = Knob("Bleach", cfg.Bleach, 0f, 1f, Defaults.Bleach, "Desaturated, high-contrast 'silver' film look.");
            cfg.BleachContrast = Knob("Bleach contrast", cfg.BleachContrast, 1f, 1.6f, Defaults.BleachContrast);
        }

        using (var grp = GroupEn("Gradient map (duotone)", cfg.GradMap > 0f, cfg.EnGradMap, v => cfg.EnGradMap = v, zoneGet: () => cfg.ZoneGradMap, zoneSet: v => cfg.ZoneGradMap = v))
        if (grp.Show)
        {
            cfg.GradMap = Knob("Gradient map", cfg.GradMap, 0f, 1f, Defaults.GradMap, "Remaps brightness to a 3-color ramp.");
            var gs = ColorPick("Dark tone", new Vector3(cfg.GmShadowR, cfg.GmShadowG, cfg.GmShadowB), new Vector3(Defaults.GmShadowR, Defaults.GmShadowG, Defaults.GmShadowB));
            cfg.GmShadowR = gs.X; cfg.GmShadowG = gs.Y; cfg.GmShadowB = gs.Z;
            var gm = ColorPick("Mid tone", new Vector3(cfg.GmMidR, cfg.GmMidG, cfg.GmMidB), new Vector3(Defaults.GmMidR, Defaults.GmMidG, Defaults.GmMidB));
            cfg.GmMidR = gm.X; cfg.GmMidG = gm.Y; cfg.GmMidB = gm.Z;
            var gh = ColorPick("Light tone", new Vector3(cfg.GmHighR, cfg.GmHighG, cfg.GmHighB), new Vector3(Defaults.GmHighR, Defaults.GmHighG, Defaults.GmHighB));
            cfg.GmHighR = gh.X; cfg.GmHighG = gh.Y; cfg.GmHighB = gh.Z;
        }
    }

    private void DrawFxTab(PluginConfig cfg)
    {
        using var tab = ImRaii.TabItem("FX");
        if (!tab) return;
        FxBody(cfg);
    }

    private void FxBody(PluginConfig cfg)
    {

        using (var grp = GroupEn("Stylize", cfg.EdgeAura > 0f || cfg.Iridescent > 0f, cfg.EnStylize, v => cfg.EnStylize = v, zoneGet: () => cfg.ZoneStylize, zoneSet: v => cfg.ZoneStylize = v))
        if (grp.Show)
        {
            cfg.EdgeAura = Knob("Edge aura", cfg.EdgeAura, 0f, 2f, Defaults.EdgeAura, "Glowing outline on edges.");
            cfg.EdgeWidth = Knob("Edge width", cfg.EdgeWidth, 1f, 4f, Defaults.EdgeWidth);
            cfg.EdgeThreshold = Knob("Edge sensitivity", cfg.EdgeThreshold, 0.01f, 0.3f, Defaults.EdgeThreshold, null, "%.3f");
            var ec = ColorPick("Aura color", new Vector3(cfg.EdgeR, cfg.EdgeG, cfg.EdgeB), new Vector3(Defaults.EdgeR, Defaults.EdgeG, Defaults.EdgeB));
            cfg.EdgeR = ec.X; cfg.EdgeG = ec.Y; cfg.EdgeB = ec.Z;
            ImGui.Spacing();
            cfg.Iridescent = Knob("Iridescent sheen", cfg.Iridescent, 0f, 1f, Defaults.Iridescent, "Oil-slick rainbow sheen.");
            cfg.IridFreq = Knob("Sheen bands", cfg.IridFreq, 1f, 8f, Defaults.IridFreq);
            cfg.IridShift = Knob("Sheen hue", cfg.IridShift, 0f, 6.28f, Defaults.IridShift);
            ImGui.Spacing();
            cfg.CausticsAmt = Knob("Aether caustics", cfg.CausticsAmt, 0f, 1f, Defaults.CausticsAmt, "Rippling underwater-light pattern.");
            cfg.CausticsScale = Knob("Caustics scale", cfg.CausticsScale, 2f, 24f, Defaults.CausticsScale);
            var cc = ColorPick("Caustics color", new Vector3(cfg.CausticsR, cfg.CausticsG, cfg.CausticsB), new Vector3(Defaults.CausticsR, Defaults.CausticsG, Defaults.CausticsB));
            cfg.CausticsR = cc.X; cfg.CausticsG = cc.Y; cfg.CausticsB = cc.Z;
            ImGui.Spacing();
            cfg.KuwaharaAmt = Knob("Oil paint (Kuwahara)", cfg.KuwaharaAmt, 0f, 1f, Defaults.KuwaharaAmt, "Painterly oil-paint look. Heavy — use sparingly.");
            cfg.KuwaharaRadius = Knob("Brush size", cfg.KuwaharaRadius, 1f, 5f, Defaults.KuwaharaRadius, null, "%.0f");
        }

        using (var grp = GroupEn("Analog / CRT horror", cfg.VhsStatic > 0f || cfg.VhsScan > 0f || cfg.VhsDropout > 0f || cfg.VhsRoll > 0f || cfg.VhsDesat > 0f || cfg.VhsVignette > 0f, cfg.EnVhs, v => cfg.EnVhs = v, zoneGet: () => cfg.ZoneVhs, zoneSet: v => cfg.ZoneVhs = v))
        if (grp.Show)
        {
            ImGui.TextDisabled("Tip: the 'Dead Channel' preset (Background tab) sets this up.");
            cfg.VhsStatic = Knob("Static / snow", cfg.VhsStatic, 0f, 1f, Defaults.VhsStatic, "TV snow, in patches where the signal drops out.");
            cfg.VhsScan = Knob("Scanlines", cfg.VhsScan, 0f, 1f, Defaults.VhsScan, "Dark CRT scanlines.");
            if (cfg.VhsScan > 0f)
                cfg.VhsScanCount = Knob("Scanline density", cfg.VhsScanCount, 60f, 600f, Defaults.VhsScanCount, "Number of scanlines.", "%.0f");
            cfg.VhsDropout = Knob("Dropout tears", cfg.VhsDropout, 0f, 1f, Defaults.VhsDropout, "Horizontal tracking tears / signal dropout on scattered rows.");
            cfg.VhsRoll = Knob("Rolling bar", cfg.VhsRoll, 0f, 1f, Defaults.VhsRoll, "A bright vertical-hold band across the picture.");
            if (cfg.VhsRoll > 0f)
                cfg.VhsRollPos = Knob("Bar position", cfg.VhsRollPos, 0f, 1f, Defaults.VhsRollPos, "Vertical position of the rolling bar.");
            cfg.VhsDesat = Knob("Signal wash", cfg.VhsDesat, 0f, 1f, Defaults.VhsDesat, "Desaturate toward a sickly, washed-out broadcast.");
            cfg.VhsVignette = Knob("CRT vignette", cfg.VhsVignette, 0f, 1f, Defaults.VhsVignette, "Heavy darkening toward the corners.");
        }

        using (var grp = GroupEn("Magitek HUD", cfg.HudIntensity > 0f, cfg.EnHud, v => cfg.EnHud = v))
        if (grp.Show)
        {
            ImGui.TextDisabled("A targeting-visor overlay over the whole frame (like the 'Magitek HUD' preset).");
            ImGui.TextDisabled("The preset's reticle/scanner are movable layers — see 'Elements' (Background tab).");
            cfg.HudIntensity = Knob("Intensity", cfg.HudIntensity, 0f, 2f, Defaults.HudIntensity, "Master brightness of the HUD (0 = off).");
            var hc = ColorPick("Color", new Vector3(cfg.HudR, cfg.HudG, cfg.HudB), new Vector3(Defaults.HudR, Defaults.HudG, Defaults.HudB));
            cfg.HudR = hc.X; cfg.HudG = hc.Y; cfg.HudB = hc.Z;
            cfg.HudScale = Knob("Element scale", cfg.HudScale, 0f, 1.5f, Defaults.HudScale, "Size of the HUD elements.");
            cfg.HudFrame = Knob("Frame & scales", cfg.HudFrame, 0f, 1f, Defaults.HudFrame, "Corner brackets, rangefinder scale and telemetry bars.");
            cfg.HudReticle = Knob("Reticle", cfg.HudReticle, 0f, 1f, Defaults.HudReticle, "Centre targeting reticle (ring, ticks, lock-box).");
            cfg.HudRadar = Knob("Radar", cfg.HudRadar, 0f, 1f, Defaults.HudRadar, "Corner radar scope with a sweeping line (animates).");
            cfg.HudHex = Knob("Tech mesh", cfg.HudHex, 0f, 1f, Defaults.HudHex, "Faint hex-like grid over the frame.");
            cfg.HudScanline = Knob("Scanlines", cfg.HudScanline, 0f, 1f, Defaults.HudScanline, "Visor scanlines.");
            cfg.HudFlicker = Knob("Flicker", cfg.HudFlicker, 0f, 1f, Defaults.HudFlicker, "Subtle signal flicker (animates).");
            cfg.HudChroma = Knob("Visor edge", cfg.HudChroma, 0f, 1f, Defaults.HudChroma, "Darken + tint toward the HUD colour at the frame edges.");
        }

        using (var grp = GroupEn("Underwater", cfg.UwTint > 0f || cfg.UwCaustic > 0f || cfg.UwMotes > 0f || cfg.UwShafts > 0f || cfg.UwFog > 0f, cfg.EnUnderwater, v => cfg.EnUnderwater = v, zoneGet: () => cfg.ZoneUnderwater, zoneSet: v => cfg.ZoneUnderwater = v))
        if (grp.Show)
        {
            ImGui.TextDisabled("Submerges the whole frame — including the character.");
            cfg.UwTint = Knob("Water tint", cfg.UwTint, 0f, 1f, Defaults.UwTint, "Blue-green colour cast over everything.");
            if (cfg.UwTint > 0f)
            {
                var wc = ColorPick("Water color", new Vector3(cfg.UwTintR, cfg.UwTintG, cfg.UwTintB), new Vector3(Defaults.UwTintR, Defaults.UwTintG, Defaults.UwTintB));
                cfg.UwTintR = wc.X; cfg.UwTintG = wc.Y; cfg.UwTintB = wc.Z;
            }
            cfg.UwFog = Knob("Depth fog", cfg.UwFog, 0f, 1f, Defaults.UwFog, "Distant geometry dissolves into the water (needs depth).");
            cfg.UwCaustic = Knob("Caustics", cfg.UwCaustic, 0f, 1f, Defaults.UwCaustic, "Rippling refracted light dancing over the whole scene.");
            cfg.UwShafts = Knob("Light shafts", cfg.UwShafts, 0f, 1f, Defaults.UwShafts, "God-rays from the surface, over everything.");
            cfg.UwMotes = Knob("Marine snow", cfg.UwMotes, 0f, 1f, Defaults.UwMotes, "Faint floating particles drifting in the water.");
        }

#if PUBLIC_BUILD
        ImGui.TextDisabled("Particles & orbs — in progress, not in this build yet.");
#else
        using (var grp = GroupEn("Particles & bokeh", cfg.ParticleAmount > 0f || cfg.BokehAmount > 0f, cfg.EnParticles, v => cfg.EnParticles = v, zoneGet: () => cfg.ZoneBokeh, zoneSet: v => cfg.ZoneBokeh = v))
        if (grp.Show)
        {
            ImGui.TextDisabled("Falling petals / hearts / bubbles in front, and shaped bokeh on background highlights.");
            Combo("Particle", "##ptype", UiParticle, cfg.ParticleType, v => cfg.ParticleType = v);
            cfg.ParticleAmount = Knob("Particles", cfg.ParticleAmount, 0f, 1f, Defaults.ParticleAmount, "How many / how strong (0 = off).");
            if (cfg.ParticleAmount > 0f)
            {
                cfg.ParticleSize = Knob("  Size", cfg.ParticleSize, 0f, 1f, Defaults.ParticleSize, "Particle size.");
                cfg.ParticleFall = Knob("  Fall speed", cfg.ParticleFall, 0f, 1f, Defaults.ParticleFall, "How fast they drift down.\nFreeze, at the top of the window, stops them wherever they are.");
                cfg.ParticleSoft = Knob("  Near blur", cfg.ParticleSoft, 0f, 1f, Defaults.ParticleSoft,
                    "How out of focus the nearest ones are. This is the control that puts them IN\nthe air rather than on the glass: nothing in a real lens is sharp at two\ndistances at once, so uniformly crisp flakes read as a decal over the shot.");
                cfg.ParticleTumble = Knob("  Tumble", cfg.ParticleTumble, 0f, 1f, Defaults.ParticleTumble,
                    "A flat thing turning over as it falls — wide, then edge-on, then wide again.\nAt zero they are stamps that never turn, which is the other giveaway.");
                var pc = ColorPick("  Color", new Vector3(cfg.ParticleR, cfg.ParticleG, cfg.ParticleB), new Vector3(Defaults.ParticleR, Defaults.ParticleG, Defaults.ParticleB));
                cfg.ParticleR = pc.X; cfg.ParticleG = pc.Y; cfg.ParticleB = pc.Z;
                bool psolid = cfg.ParticleSolid;
                if (ImGui.Checkbox("  Solid (not glowing)##psolid", ref psolid)) { cfg.ParticleSolid = psolid; _dirty = true; }
                if (ImGui.IsItemHovered()) ImGui.SetTooltip(
                    "Off, particles ADD light, which is right for snow catching a lamp.\n" +
                    "On, they sit in front of what is behind them.\n\n" +
                    "Anything darker than the sky needs this: adding a dark colour still\nbrightens, which is why ash and petals came out white however they were tinted.");
            }
            ImGui.Spacing();
            Combo("Orb shape", "##bshape", UiBokeh, cfg.BokehShape, v => cfg.BokehShape = v);
            if (cfg.BokehShape == 2 || cfg.BokehShape == 4)
            {
                cfg.BokehBlades = Knob("  Blades", cfg.BokehBlades, 3f, 12f, Defaults.BokehBlades,
                    "How many leaves the diaphragm has. Five gives pentagons, six hexagons \u2014\nthis is the same shape at different stops rather than a list of them.", "%.0f");
            }
            cfg.BokehRotate = Knob("  Rotation", cfg.BokehRotate, -3.15f, 3.15f, Defaults.BokehRotate,
                "Turns every orb. Each one also gets a little of its own on top, because a\nfield of polygons all facing one way is a pattern rather than a lens.");
            var bc = ColorPick("Colour", new Vector3(cfg.BokehR, cfg.BokehG, cfg.BokehB),
                               new Vector3(Defaults.BokehR, Defaults.BokehG, Defaults.BokehB));
            cfg.BokehR = bc.X; cfg.BokehG = bc.Y; cfg.BokehB = bc.Z;
            cfg.BokehHueVar = Knob("Colour spread", cfg.BokehHueVar, 0f, 0.5f, Defaults.BokehHueVar,
                "How far each orb wanders from that colour. Real defocused highlights are never\none colour \u2014 they carry whatever lit them, and the glass adds a fringe.", "%.3f");

            Combo("Source", "##bsrc", UiBokehSource, cfg.BokehSource, v => cfg.BokehSource = v);
            if (cfg.BokehSource == 0)
                ImGui.TextDisabled("Needs something bright to defocus, which an empty backdrop has none of.\nSwitch to Everywhere to use these over one.");
            else
                ImGui.TextDisabled("The orbs are the light rather than a report of one, so they need no\nhighlight and no depth. This is the mode for an empty backdrop.");

            if (cfg.BokehSource == 0)
            cfg.BokehThreshold = Knob("Highlight threshold", cfg.BokehThreshold, 0f, 1f, Defaults.BokehThreshold,
                "How bright the background has to be before it blooms one. Bokeh is what a\nHIGHLIGHT looks like out of focus \u2014 a defocused grey sky is still a grey sky,\nand discs spread evenly over one is the giveaway. Lower it to catch more.");
            cfg.BokehDensity = Knob("Density", cfg.BokehDensity, 0f, 1f, Defaults.BokehDensity, "How closely packed they are.");
            cfg.BokehRim = Knob("Rim", cfg.BokehRim, 0f, 1f, Defaults.BokehRim,
                "Brighter at the edge than through the middle \u2014 the soap-bubble look of a\ncheaper lens. At zero they are flat discs, which no lens produces.");
            cfg.BokehCatEye = Knob("Cat's eye", cfg.BokehCatEye, 0f, 1f, Defaults.BokehCatEye,
                "A lens clips its own aperture toward the frame edge, so a disc in the corner\nis a lens shape pointing at the middle rather than a circle. Its absence is\nthe clearest sign these were pasted on.");
            cfg.BokehAmount = Knob("Bokeh", cfg.BokehAmount, 0f, 1f, Defaults.BokehAmount, "Glowing shaped discs over bright background highlights (needs depth).");
        }
#endif

    }

    private static (bool c2, bool c3, string c3role, bool scaleX, bool scaleY, bool angle, bool hardness, bool offset, bool ramp) BgCaps(int s)
    {
        switch (s)
        {
            case 1:  return (false, false, "", false, false, false, false, false, false);
            case 2: case 4: case 8: return (true, true, "middle stop", false, false, false, true, true, true);
            case 3:  return (true, true, "middle stop", false, false, false, true, true, true);
            case 12: return (true, true, "middle ring", true, false, true, true, true, true);
            case 13: return (true, true, "mid tone", true, true, false, true, true, true);
            case 5:  return (true, true, "middle band", true, false, true, false, true, false);
            case 9:  return (true, true, "middle arm", true, false, true, false, true, false);
            case 11: return (true, true, "middle ray", true, false, true, false, true, false);
            case 6:  return (true, true, "grout lines", true, true, false, false, true, false);
            case 7:  return (true, true, "dot ring", true, true, false, false, true, false);
            case 10: return (true, true, "intersections", true, true, false, false, true, false);
            case 14: return (true, true, "mid cloud", true, false, false, false, true, true);
            case 15: return (false, false, "", false, false, false, false, false, false);
            case 16: return (true, true, "vein tone", true, false, false, false, true, true);
            case 17: return (true, true, "water mid", true, false, false, false, true, true);
            case 18: return (true, true, "aurora mid", true, false, false, false, true, true);
            case 19: return (true, true, "sky mid", true, true, false, false, true, true);
            case 20: return (true, true, "sky mid", true, true, false, false, true, true);
            case 21: return (true, true, "sky mid", true, true, false, false, true, true);
            case 22: return (true, true, "ember mid", true, true, false, false, true, true);
            case 23: return (true, true, "dusk mid", true, false, false, false, true, true);
            case 24: return (true, true, "sky mid", true, true, false, false, true, true);
            case 25: return (true, true, "pale mid", true, true, false, false, true, true);
            case 26: return (true, true, "gradient mid", true, true, true, true, true, true);
            case 27: return (true, true, "gradient mid", true, true, true, true, true, true);
            case 28: return (true, true, "sky mid", true, true, false, false, true, true);
            case 29: return (true, false, "", true, true, false, false, true, false);
            default: return (true, true, "third colour", true, true, true, true, true, false);
        }
    }

    private void DrawOverlaysTab(PluginConfig cfg)
    {
        using var tab = ImRaii.TabItem("Overlays");
        if (!tab) return;
        OverlaysBody(cfg);
    }

    private void OverlaysBody(PluginConfig cfg)
    {

        if (Group("Composition guides", cfg.ShowGuides))
        {
            ImGui.TextDisabled("Overlay lines drawn straight on the game (in gpose) — visible\nwhile you pose, even with live preview off.");
            var sg = cfg.ShowGuides;
            if (ImGui.Checkbox("Show guides", ref sg)) { cfg.ShowGuides = sg; _dirty = true; }
            if (cfg.ShowGuides)
            {
                var t3 = cfg.GuideThirds; if (ImGui.Checkbox("Rule of thirds", ref t3)) { cfg.GuideThirds = t3; _dirty = true; }
                ImGui.SameLine(); var gr = cfg.GuideGolden; if (ImGui.Checkbox("Golden ratio", ref gr)) { cfg.GuideGolden = gr; _dirty = true; }
                var cc = cfg.GuideCenter; if (ImGui.Checkbox("Center cross", ref cc)) { cfg.GuideCenter = cc; _dirty = true; }
                ImGui.SameLine(); var hz = cfg.GuideHorizon; if (ImGui.Checkbox("Level / horizon", ref hz)) { cfg.GuideHorizon = hz; _dirty = true; }
                if (cfg.GuideHorizon)
                    cfg.GuideHorizonY = Knob("Horizon height", cfg.GuideHorizonY, 0f, 1f, 0.5f, "Vertical position of the level line.");
                cfg.GuideOpacity = Knob("Guide opacity", cfg.GuideOpacity, 0.1f, 1f, 1f, "Opacity of the guide lines.");
            }
            ImGui.Spacing();
        }

        DrawTextGroup(cfg);

        DrawElementsGroup(cfg);
    }

    private void DrawBackgroundTab(PluginConfig cfg)
    {
        using var tab = ImRaii.TabItem("Background");
        if (!tab) return;
        BackgroundBody(cfg);
    }

    private void BackgroundBody(PluginConfig cfg)
    {

        if (!_live.DepthAvailable)
            ImGui.TextDisabled(_live.Enabled ? "Depth not available yet…" : "Enable live preview to use background effects.");

        using (var grp = GroupEn("Background style", cfg.BgRecolor > 0f && cfg.BgStyle > 0, cfg.EnBackdrop, v => cfg.EnBackdrop = v, true,
                              zoneGet: () => cfg.ZoneBackdrop, zoneSet: v => cfg.ZoneBackdrop = v, maskOnly: true))
        if (grp.Show) DrawBgStyleGroup(cfg);

        using (var grp = GroupEn("Edge integration", cfg.EdgeErode > 0f || cfg.EdgeDespill > 0f || cfg.EdgeWrap > 0f, cfg.EnEdge, v => cfg.EnEdge = v))
        if (grp.Show)
        {
            if (!_live.DepthAvailable) ImGui.TextDisabled("Needs depth — enable live preview in gpose.");
            ImGui.TextDisabled("Blends the character INTO the background instead of pasting them on.\nThe bright outline you get around a cutout is the old background still\nstuck to the anti-aliased edge pixels — this removes it and lets the new\nbackdrop's light spill onto the rim. Needs a background style active.");
            cfg.EdgeErode = Knob("Trim fringe", cfg.EdgeErode, 0f, 1f, Defaults.EdgeErode, "Eats the contaminated rim so the backdrop replaces it outright. The direct fix for a bright outline tracing the hair and limbs.");
            cfg.EdgeDespill = Knob("Despill", cfg.EdgeDespill, 0f, 1f, Defaults.EdgeDespill, "Repaints whatever fringe is left with clean colour from just inside the subject, instead of the old background's colour.");
            cfg.EdgeWrap = Knob("Light wrap", cfg.EdgeWrap, 0f, 1f, Defaults.EdgeWrap, "Lets the backdrop's light spill onto the subject's edge — the compositing trick that makes subject and background look lit by the same room.");
            if (cfg.EdgeWrap > 0f || cfg.EdgeDespill > 0f || cfg.EdgeErode > 0f)
                cfg.EdgeWrapWidth = Knob("Reach", cfg.EdgeWrapWidth, 0f, 1f, Defaults.EdgeWrapWidth, "How far in from the silhouette the trim / despill / wrap reach.");
        }

        using (var grp = GroupEn("Background push", cfg.BgPushStrength > 0f, cfg.EnSubjectIso, v => cfg.EnSubjectIso = v, true, zoneGet: () => cfg.ZoneBgPush, zoneSet: v => cfg.ZoneBgPush = v))
        if (grp.Show)
        {
            cfg.BgPushStrength = Knob("Background push", cfg.BgPushStrength, 0f, 1f, Defaults.BgPushStrength, "Desaturate + darken the background so your subject pops.");
            cfg.BgPushStart = Knob("Push start", cfg.BgPushStart, 0f, 1f, Defaults.BgPushStart, "How far out the push begins.");
        }

        using (var grp = GroupEn("Background blur", cfg.BgBlur > 0f, cfg.EnBgBlur, v => cfg.EnBgBlur = v, zoneGet: () => cfg.ZoneBgBlur, zoneSet: v => cfg.ZoneBgBlur = v))
        if (grp.Show)
        {
            cfg.BgBlur = Knob("Background blur", cfg.BgBlur, 0f, 1f, Defaults.BgBlur, "Blurs the far background (portrait bokeh); the subject stays sharp.");
            cfg.BgBlurStart = Knob("Blur start", cfg.BgBlurStart, 0f, 0.5f, Defaults.BgBlurStart, "How far out the blur begins.");
            ImGui.TextDisabled("Blur spread is the shared \"Soft blur radius\" in Effects ▸ Glow.");
        }

        using (var grp = GroupEn("Frost overlay", cfg.FrostAmount > 0f, cfg.EnFrost, v => cfg.EnFrost = v, zoneGet: () => cfg.ZoneFrost, zoneSet: v => cfg.ZoneFrost = v))
        if (grp.Show)
        {
            ImGui.TextDisabled("Ice crystals creeping in from the screen edges, over everything.");
            cfg.FrostAmount = Knob("Frost amount", cfg.FrostAmount, 0f, 1f, Defaults.FrostAmount, "How thick the frost is (0 = off).");
            cfg.FrostCoverage = Knob("Coverage", cfg.FrostCoverage, 0f, 1f, Defaults.FrostCoverage, "How far the frost reaches inward from the edges.");
            cfg.FrostFeather = Knob("Crystal detail", cfg.FrostFeather, 0f, 1f, Defaults.FrostFeather, "Finer (1) vs broader (0) ice crystals.");
        }

        using (var grp = GroupEn("Distance fog", cfg.FogStrength > 0f, cfg.EnFog, v => cfg.EnFog = v,
                              zoneGet: () => cfg.ZoneFog, zoneSet: v => cfg.ZoneFog = v, maskOnly: true))
        if (grp.Show)
        {
            cfg.FogStrength = Knob("Distance fog", cfg.FogStrength, 0f, 1f, Defaults.FogStrength, "Fades distant geometry into atmosphere.");
            cfg.FogStart = Knob("Fog start", cfg.FogStart, 0f, 0.5f, Defaults.FogStart, "How far out the fog begins.");
            var fog = ColorPick("Fog color", new Vector3(cfg.FogColorR, cfg.FogColorG, cfg.FogColorB), new Vector3(Defaults.FogColorR, Defaults.FogColorG, Defaults.FogColorB));
            cfg.FogColorR = fog.X; cfg.FogColorG = fog.Y; cfg.FogColorB = fog.Z;
        }

        using (var grp = GroupEn("Solid backdrop (fill)", cfg.BgFill > 0f, cfg.EnBgFill, v => cfg.EnBgFill = v,
                              zoneGet: () => cfg.ZoneBgFill, zoneSet: v => cfg.ZoneBgFill = v, maskOnly: true))
        if (grp.Show)
        {
            ImGui.TextDisabled("Replaces the background with one flat colour — a clean empty backdrop.\nUse alone, or as a base a procedural background sits on so walls don't show through.");

            if (ImGui.Button("Black backdrop", new Vector2(140f, 0)))
            {
                PushUndo(cfg);
                bool hadBackdrop = cfg.EnBackdrop && cfg.BgRecolor > 0f && cfg.BgStyle > 0;
                cfg.EnBgFill = true;
                cfg.BgFill = 1f;
                cfg.BgFillR = cfg.BgFillG = cfg.BgFillB = 0f;
                cfg.BgFillStart = 0.05f;
                cfg.BgFillFeather = 0.014f;
                if (hadBackdrop) cfg.EnBackdrop = false;
                _status = hadBackdrop
                    ? "Background set to black. The procedural backdrop was switched off — it would have painted over it."
                    : "Background set to black. If a wall still shows, lower Start (depth) below.";
                _dirty = true;
            }
            if (ImGui.IsItemHovered()) ImGui.SetTooltip(
                "Subject on black, in one click.\n\n" +
                "Sets this fill to opaque black and cuts it in close behind the subject.\n" +
                "Switches off the procedural backdrop if one is on, since that would\npaint over the black.\n\n" +
                "Needs depth: without it there is no way to tell background from subject.");
            ImGui.SameLine();
            if (ImGui.Button("White", new Vector2(80f, 0)))
            {
                PushUndo(cfg);
                bool hadBackdrop = cfg.EnBackdrop && cfg.BgRecolor > 0f && cfg.BgStyle > 0;
                cfg.EnBgFill = true;
                cfg.BgFill = 1f;
                cfg.BgFillR = cfg.BgFillG = cfg.BgFillB = 1f;
                cfg.BgFillStart = 0.05f;
                cfg.BgFillFeather = 0.014f;
                if (hadBackdrop) cfg.EnBackdrop = false;
                _status = "Background set to white.";
                _dirty = true;
            }
            if (ImGui.IsItemHovered()) ImGui.SetTooltip("The same thing the other way up — a high-key studio backdrop.");

            if (!_live.DepthAvailable)
                ImGui.TextColored(new Vector4(1f, 0.62f, 0.25f, 1f),
                    "No depth yet — enter gpose with live preview on, or this fills the whole frame.");
            else if (cfg.EnBgFill && cfg.BgFill > 0f)
                ImGui.TextDisabled("If a wall behind you is still showing, lower Start (depth).\nTick Show what this covers to see exactly where it reaches.");

            ImGui.Spacing();
            cfg.BgFill = Knob("Opacity", cfg.BgFill, 0f, 1f, Defaults.BgFill, "How opaque the fill is (1 = fully replaces the background with the colour).");
            var bf = ColorPick("Colour", new Vector3(cfg.BgFillR, cfg.BgFillG, cfg.BgFillB), new Vector3(Defaults.BgFillR, Defaults.BgFillG, Defaults.BgFillB));
            cfg.BgFillR = bf.X; cfg.BgFillG = bf.Y; cfg.BgFillB = bf.Z;
            cfg.BgFillStart = Knob("Start (depth)", cfg.BgFillStart, 0f, 0.5f, Defaults.BgFillStart, "How far out the fill begins. Lower it to swallow a wall/floor just behind the subject.");
            GateToggle(cfg, "fill");
            cfg.BgFillFeather = Knob("Cutoff softness", cfg.BgFillFeather, 0.003f, 0.3f, Defaults.BgFillFeather, "Depth transition width. LOW = a hard cut right behind the subject (covers a close wall cleanly); HIGH = a soft fade.", "%.3f");
        }

        using (var grp = GroupEn("Foreground layer", cfg.EnForegroundOn, cfg.EnForegroundOn, v => cfg.EnForegroundOn = v))
        if (grp.Show) DrawForegroundLayer(cfg);
    }

    private void DrawForegroundLayer(PluginConfig cfg)
    {
        ImGui.TextDisabled("A Universal field over everything (the subject too), masked to a\nplacement shape. Great for fog, mist or a creeping void.");

        Combo("Placement", "##fgplace", UiFgPlace, cfg.FgPlaceMode, v => cfg.FgPlaceMode = v);
        cfg.FgPlaceSize = Knob("Reach", cfg.FgPlaceSize, 0f, 0.98f, Defaults.FgPlaceSize, "How far the layer reaches in from its edge/shape.");
        cfg.FgPlaceSoft = Knob("Feather", cfg.FgPlaceSoft, 0.02f, 0.6f, Defaults.FgPlaceSoft, "Softness of the placement edge.");
        if (cfg.FgPlaceMode == 7)
            cfg.FgPlaceAngle = Knob("Direction", cfg.FgPlaceAngle, -3.14f, 3.14f, Defaults.FgPlaceAngle, "Angle of the directional wash.");
        cfg.FgOpacity = Knob("Opacity", cfg.FgOpacity, 0f, 1f, Defaults.FgOpacity, "Overall strength of the foreground layer.");
        Combo("Blend", "##fgblend", UiFgBlend, cfg.FgBlendMode, v => cfg.FgBlendMode = v);
        Combo("Over depth", "##fgdepth", UiFgDepth, cfg.FgDepthGate, v => cfg.FgDepthGate = v);

        ImGui.Spacing();
        ImGui.Separator();
        using var id = ImRaii.PushId("fgfield");
        cfg.CopyFgToScratch(_scratch, 0);
        _scratch.BgStyle = 27;

        ImGui.TextDisabled("The foreground field — its own colours, engine and placement.");
        var s1 = ColorPick("Color 1", new Vector3(_scratch.BgTopR, _scratch.BgTopG, _scratch.BgTopB), new Vector3(Defaults.BgTopR, Defaults.BgTopG, Defaults.BgTopB));
        _scratch.BgTopR = s1.X; _scratch.BgTopG = s1.Y; _scratch.BgTopB = s1.Z;
        var s2 = ColorPick("Color 2", new Vector3(_scratch.BgCol5R, _scratch.BgCol5G, _scratch.BgCol5B), new Vector3(Defaults.BgCol5R, Defaults.BgCol5G, Defaults.BgCol5B));
        _scratch.BgCol5R = s2.X; _scratch.BgCol5G = s2.Y; _scratch.BgCol5B = s2.Z;
        var s3 = ColorPick("Color 3", new Vector3(_scratch.BgMidR, _scratch.BgMidG, _scratch.BgMidB), new Vector3(Defaults.BgMidR, Defaults.BgMidG, Defaults.BgMidB));
        _scratch.BgMidR = s3.X; _scratch.BgMidG = s3.Y; _scratch.BgMidB = s3.Z;
        var s4 = ColorPick("Color 4", new Vector3(_scratch.BgCol6R, _scratch.BgCol6G, _scratch.BgCol6B), new Vector3(Defaults.BgCol6R, Defaults.BgCol6G, Defaults.BgCol6B));
        _scratch.BgCol6R = s4.X; _scratch.BgCol6G = s4.Y; _scratch.BgCol6B = s4.Z;
        var s5 = ColorPick("Color 5", new Vector3(_scratch.BgBotR, _scratch.BgBotG, _scratch.BgBotB), new Vector3(Defaults.BgBotR, Defaults.BgBotG, Defaults.BgBotB));
        _scratch.BgBotR = s5.X; _scratch.BgBotG = s5.Y; _scratch.BgBotB = s5.Z;

        DrawUniversalControls(_scratch);

        _scratch.BgScale = Knob("Scale", _scratch.BgScale, 1f, 40f, Defaults.BgScale, "Pattern density of the foreground field.", "%.0f");
        _scratch.BgScaleY = Knob("Scale Y", _scratch.BgScaleY, 1f, 40f, Defaults.BgScaleY, "Pattern size of the foreground field.", "%.0f");
        _scratch.BgAngle = Knob("Angle", _scratch.BgAngle, 0f, 3.14f, Defaults.BgAngle, "Rotation of the foreground field.");
        _scratch.BgSharp = Knob("Edge hardness", _scratch.BgSharp, 0f, 1f, Defaults.BgSharp, "Soft gradient (0) -> hard band (1).");
        _scratch.BgFbm = Knob("Detail", _scratch.BgFbm, 1f, 6f, Defaults.BgFbm, "Fractal octaves of the foreground field.", "%.0f");

        ImGui.Spacing();
        ImGui.TextDisabled("Atmosphere (extras layered on this field):");
        var fa4 = ColorPick("Star / glow color", new Vector3(_scratch.BgCol4R, _scratch.BgCol4G, _scratch.BgCol4B), new Vector3(Defaults.BgCol4R, Defaults.BgCol4G, Defaults.BgCol4B));
        _scratch.BgCol4R = fa4.X; _scratch.BgCol4G = fa4.Y; _scratch.BgCol4B = fa4.Z;
        _scratch.BgHaze = Knob("Haze", _scratch.BgHaze, 0f, 1f, Defaults.BgHaze, "Soft low-frequency glow — atmospheric depth in front.");
        _scratch.BgGlow = Knob("Glow", _scratch.BgGlow, 0f, 1f, Defaults.BgGlow, "Fake bloom on the field's bright wisps.");
        _scratch.BgStars = Knob("Stars / bokeh", _scratch.BgStars, 0f, 1f, Defaults.BgStars, "Sparkle points floating in front — dust or bokeh.");
        if (_scratch.BgStars > 0f)
        {
            _scratch.BgStarDensity = Knob("Star density", _scratch.BgStarDensity, 4f, 120f, Defaults.BgStarDensity, "How many points.", "%.0f");
            _scratch.BgStarSize = Knob("Star size", _scratch.BgStarSize, 0f, 1f, Defaults.BgStarSize, "How large each point is.");
        }
        _scratch.BgHueVar = Knob("Hue variation", _scratch.BgHueVar, 0f, 1f, Defaults.BgHueVar, "Vary the hue across the field.");
        _scratch.BgEmbers = Knob("Dust motes", _scratch.BgEmbers, 0f, 1f, Defaults.BgEmbers, "Drifting motes catching light.");
        if (_scratch.BgEmbers > 0f)
            _scratch.BgEmberSize = Knob("Mote size", _scratch.BgEmberSize, 0f, 1f, Defaults.BgEmberSize, "Size of each mote.");
        _scratch.BgGrain = Knob("Grain", _scratch.BgGrain, 0f, 1f, Defaults.BgGrain, "Fine noise over the field.");

        cfg.CopyFgFromScratch(_scratch, 0);

        ImGui.Spacing();
        ImGui.Separator();
        bool fgb = cfg.FgBActive;
        if (ImGui.Checkbox("Second foreground field (combine)", ref fgb))
        {
            cfg.SetFgBActive(fgb);
            if (fgb && cfg.FgSeamMix == 0) { cfg.FgSeamMix = 1; cfg.FgSeamMode = 3; }
            _dirty = true;
        }
        if (fgb)
        {
            using var id2 = ImRaii.PushId("fgfield2");
            cfg.CopyFgToScratch(_scratch, 1);
            _scratch.BgStyle = 27;
            ImGui.TextDisabled("The second foreground field — its own colours and engine.");
            var t1 = ColorPick("Color 1", new Vector3(_scratch.BgTopR, _scratch.BgTopG, _scratch.BgTopB), new Vector3(Defaults.BgTopR, Defaults.BgTopG, Defaults.BgTopB));
            _scratch.BgTopR = t1.X; _scratch.BgTopG = t1.Y; _scratch.BgTopB = t1.Z;
            var t2 = ColorPick("Color 2", new Vector3(_scratch.BgCol5R, _scratch.BgCol5G, _scratch.BgCol5B), new Vector3(Defaults.BgCol5R, Defaults.BgCol5G, Defaults.BgCol5B));
            _scratch.BgCol5R = t2.X; _scratch.BgCol5G = t2.Y; _scratch.BgCol5B = t2.Z;
            var t3 = ColorPick("Color 3", new Vector3(_scratch.BgMidR, _scratch.BgMidG, _scratch.BgMidB), new Vector3(Defaults.BgMidR, Defaults.BgMidG, Defaults.BgMidB));
            _scratch.BgMidR = t3.X; _scratch.BgMidG = t3.Y; _scratch.BgMidB = t3.Z;
            var t4 = ColorPick("Color 4", new Vector3(_scratch.BgCol6R, _scratch.BgCol6G, _scratch.BgCol6B), new Vector3(Defaults.BgCol6R, Defaults.BgCol6G, Defaults.BgCol6B));
            _scratch.BgCol6R = t4.X; _scratch.BgCol6G = t4.Y; _scratch.BgCol6B = t4.Z;
            var t5 = ColorPick("Color 5", new Vector3(_scratch.BgBotR, _scratch.BgBotG, _scratch.BgBotB), new Vector3(Defaults.BgBotR, Defaults.BgBotG, Defaults.BgBotB));
            _scratch.BgBotR = t5.X; _scratch.BgBotG = t5.Y; _scratch.BgBotB = t5.Z;

            DrawUniversalControls(_scratch);

            _scratch.BgScale = Knob("Scale", _scratch.BgScale, 1f, 40f, Defaults.BgScale, "Pattern density of the second foreground field.", "%.0f");
            _scratch.BgScaleY = Knob("Scale Y", _scratch.BgScaleY, 1f, 40f, Defaults.BgScaleY, "Pattern size of the second foreground field.", "%.0f");
            _scratch.BgAngle = Knob("Angle", _scratch.BgAngle, 0f, 3.14f, Defaults.BgAngle, "Rotation of the second foreground field.");
            _scratch.BgSharp = Knob("Edge hardness", _scratch.BgSharp, 0f, 1f, Defaults.BgSharp, "Soft gradient (0) -> hard band (1).");
            _scratch.BgFbm = Knob("Detail", _scratch.BgFbm, 1f, 6f, Defaults.BgFbm, "Fractal octaves of the second foreground field.", "%.0f");

            cfg.CopyFgFromScratch(_scratch, 1);

            ImGui.Spacing();
            using (ImRaii.PushId("fgseam")) DrawFgCombine(cfg);
        }
    }

    private void DrawFgCombine(PluginConfig cfg)
    {
        var mixes = UiMixes;
        int mx = Math.Clamp(cfg.FgSeamMix, 0, mixes.Length - 1);
        ImGui.TextUnformatted("Combine"); ImGui.SameLine(110f); ImGui.PushItemWidth(-1f);
        if (ImGui.BeginCombo("##fgmix", mixes[mx]))
        {
            for (int i = 0; i < mixes.Length; i++)
                if (ImGui.Selectable(mixes[i], mx == i))
                {
                    cfg.FgSeamMix = i;
                    if (i > 0 && cfg.FgSeamMode != 3) cfg.FgSeamMode = 3;
                    else if (i == 0 && cfg.FgSeamMode == 3) cfg.FgSeamMode = 0;
                    _dirty = true;
                }
            ImGui.EndCombo();
        }
        ImGui.PopItemWidth();
        if (cfg.FgSeamMix == 1 || cfg.FgSeamMix == 2 || cfg.FgSeamMix == 5 || cfg.FgSeamMix == 6)
            cfg.FgSeamMixLevel = Knob("Level", cfg.FgSeamMixLevel, 0f, 1f, Defaults.FgSeamMixLevel, "Threshold the combine keys off (how bright/dark before B shows). Feather softens it.");
        ImGui.TextDisabled(cfg.FgSeamMix == 0 ? "Splits the layer along the seam below." : "Blends in one shared space; the seam below still confines it to a region.");
        ImGui.Separator();

        var modes = UiModes;
        int md = Math.Clamp(cfg.FgSeamMode, 0, modes.Length - 1);
        ImGui.TextUnformatted("Seam"); ImGui.SameLine(110f); ImGui.PushItemWidth(-1f);
        if (ImGui.BeginCombo("##fgmode", modes[md]))
        {
            for (int i = 0; i < modes.Length; i++)
                if (ImGui.Selectable(modes[i], md == i)) { cfg.FgSeamMode = i; _dirty = true; }
            ImGui.EndCombo();
        }
        ImGui.PopItemWidth();

        if (cfg.FgSeamMode == 0)
        {
            cfg.FgSeamAngle = Knob("Angle", cfg.FgSeamAngle, 0f, 3.14f, Defaults.FgSeamAngle, "Orientation of the split. 0 = left/right; ~1.57 = top/bottom.");
            cfg.FgSeamOffset = Knob("Position", cfg.FgSeamOffset, -0.7f, 0.7f, Defaults.FgSeamOffset, "Slide the seam across (0 = centred).");
        }
        else if (cfg.FgSeamMode == 1)
        {
            cfg.FgSeamCx = Knob("Center X", cfg.FgSeamCx, 0f, 1f, Defaults.FgSeamCx, "Centre of B's region.");
            cfg.FgSeamCy = Knob("Center Y", cfg.FgSeamCy, 0f, 1f, Defaults.FgSeamCy, "Centre of B's region.");
            cfg.FgSeamRadius = Knob("Radius", cfg.FgSeamRadius, 0f, 1f, Defaults.FgSeamRadius, "Size of B's region.");
            cfg.FgSeamEllipse = Knob("Oval", cfg.FgSeamEllipse, 0.2f, 3f, Defaults.FgSeamEllipse, "Vertical squash (1 = a circle).");
        }
        else
        {
            cfg.FgSeamDepthSplit = Knob("Depth split", cfg.FgSeamDepthSplit, 0f, 1f, Defaults.FgSeamDepthSplit, "B fills past this depth; A fills what is nearer. Needs depth.");
        }

        cfg.FgSeamFeather = Knob("Feather", cfg.FgSeamFeather, 0.001f, 0.3f, Defaults.FgSeamFeather, "Half-width of the soft transition band (0 = a hard cut).");
        cfg.FgSeamNoiseAmt = Knob("Ragged edge", cfg.FgSeamNoiseAmt, 0f, 1f, Defaults.FgSeamNoiseAmt, "Perturb the seam with noise: low Scale = wandering edge, high Scale = granular dissolve.");
        if (cfg.FgSeamNoiseAmt > 0f)
            cfg.FgSeamNoiseScale = Knob("Ragged scale", cfg.FgSeamNoiseScale, 0.5f, 24f, Defaults.FgSeamNoiseScale, "Frequency of the seam noise.", "%.1f");
        cfg.FgSeamMatch = Knob("Brightness match", cfg.FgSeamMatch, 0f, 1f, Defaults.FgSeamMatch, "Ease a brightness step at the seam by nudging both sides together.");
        cfg.FgSeamDepthBend = Knob("Depth bend", cfg.FgSeamDepthBend, -1f, 1f, Defaults.FgSeamDepthBend, "Let scene depth push the seam so it reads as in-world. Needs depth.");
        if (cfg.FgSeamDepthBend != 0f) cfg.FgSeamDepthRef = Knob("Depth neutral", cfg.FgSeamDepthRef, 0f, 1f, Defaults.FgSeamDepthRef, "The depth the bend treats as flat.");
    }

    private void DrawCameraTab(PluginConfig cfg)
    {
        using var tab = ImRaii.TabItem("Camera");
        if (!tab) return;
        CameraBody(cfg);
    }

    private void CameraBody(PluginConfig cfg)
    {

        using (var grp = GroupEn("Depth of field", cfg.DofStrength > 0f, cfg.EnDof, v => cfg.EnDof = v))
        if (grp.Show)
        {
            cfg.DofStrength = Knob("Focus blur", cfg.DofStrength, 0f, 1f, Defaults.DofStrength, "Blurs away from the focus plane.");
            cfg.DofFocus = Knob("Focus distance", cfg.DofFocus, 0f, 1f, Defaults.DofFocus, "Where the sharp plane sits (near = 0).");
            cfg.DofRange = Knob("Focus range", cfg.DofRange, 0.02f, 0.5f, Defaults.DofRange, "How deep the in-focus band is.");
        }

        using (var grp = GroupEn("Tilt-shift", cfg.TiltAmt > 0f, cfg.EnTiltShift, v => cfg.EnTiltShift = v))
        if (grp.Show)
        {
            cfg.TiltAmt = Knob("Tilt-shift", cfg.TiltAmt, 0f, 1f, Defaults.TiltAmt, "Blurs above and below a horizontal band (miniature look). No depth needed.");
            cfg.TiltFocus = Knob("Focus line", cfg.TiltFocus, 0f, 1f, Defaults.TiltFocus, "Vertical position of the sharp band.");
            cfg.TiltRange = Knob("Focus band", cfg.TiltRange, 0.05f, 0.5f, Defaults.TiltRange, "Height of the sharp band.");
        }

        using (var grp = GroupEn("Lens & film", cfg.Vignette > 0f || cfg.Sharpen > 0f || cfg.Chroma > 0f || cfg.Grain > 0f || cfg.Letterbox > 0f || cfg.Prism > 0f || cfg.LeakAmt > 0f, cfg.EnLens, v => cfg.EnLens = v, true))
        if (grp.Show)
        {
            cfg.Vignette = Knob("Vignette", cfg.Vignette, 0f, 1f, Defaults.Vignette, "Darkens the corners.");
            cfg.Sharpen = Knob("Sharpen", cfg.Sharpen, 0f, 1f, Defaults.Sharpen, "Crisp up detail (unsharp mask).");
            cfg.Chroma = Knob("Chromatic aberration", cfg.Chroma, 0f, 1f, Defaults.Chroma, "Color fringing toward the edges.");
            if (cfg.Chroma > 0f)
                cfg.ChromaRadial = Knob("  Corner-weighted", cfg.ChromaRadial, 0f, 1f, Defaults.ChromaRadial, "Push the fringing out to the corners and keep the centre clean — how a real lens behaves. 0 = the uniform ramp.");
            cfg.ChromaClean = Knob("Chromatic cleanup", cfg.ChromaClean, 0f, 1f, Defaults.ChromaClean, "Reduce color fringing/noise (keeps luma sharp).");
            cfg.Prism = Knob("Prism dispersion", cfg.Prism, 0f, 1f, Defaults.Prism, "Rainbow lens dispersion (stronger than CA).");
            cfg.Grain = Knob("Film grain", cfg.Grain, 0f, 1f, Defaults.Grain, "Adds static film grain.");
            cfg.Letterbox = Knob("Letterbox", cfg.Letterbox, 0f, 1f, Defaults.Letterbox, "Cinematic black bars.");
            ImGui.Spacing();
            cfg.LeakAmt = Knob("Light leak", cfg.LeakAmt, 0f, 1f, Defaults.LeakAmt, "Directional colored light leak.");
            cfg.LeakAngle = Knob("Leak angle", cfg.LeakAngle, 0f, 6.28f, Defaults.LeakAngle);
            var lk = ColorPick("Leak color", new Vector3(cfg.LeakR, cfg.LeakG, cfg.LeakB), new Vector3(Defaults.LeakR, Defaults.LeakG, Defaults.LeakB));
            cfg.LeakR = lk.X; cfg.LeakG = lk.Y; cfg.LeakB = lk.Z;
            ImGui.Spacing();
            cfg.WashAmount = Knob("Atmosphere wash", cfg.WashAmount, 0f, 1f, Defaults.WashAmount, "Soft colored light glow from a point.");
            cfg.WashX = Knob("Wash X", cfg.WashX, 0f, 1f, Defaults.WashX);
            cfg.WashY = Knob("Wash Y", cfg.WashY, 0f, 1f, Defaults.WashY, "Height of the wash. 0 = bottom of frame, 1 = top.");
            var wc = ColorPick("Wash color", new Vector3(cfg.WashR, cfg.WashG, cfg.WashB), new Vector3(Defaults.WashR, Defaults.WashG, Defaults.WashB));
            cfg.WashR = wc.X; cfg.WashG = wc.Y; cfg.WashB = wc.Z;

            ImGui.Spacing();
            ImGui.TextDisabled("Film response — how a negative answers light, instead of a\nrenderer's hard clip. The quickest win against the 'CG' look.");
            cfg.FilmRolloff = Knob("Highlight rolloff", cfg.FilmRolloff, 0f, 1f, Defaults.FilmRolloff, "Soft shoulder: bright areas ease into white instead of clipping flat. Recovers detail in skin hotspots and bright backdrops.");
            cfg.FilmSat = Knob("Highlight desaturation", cfg.FilmSat, 0f, 1f, Defaults.FilmSat, "Saturated highlights wash toward white as they climb, the way film and sensors do. Stops neon/blown colour looking like paint.");
            cfg.FilmToe = Knob("Shadow toe", cfg.FilmToe, 0f, 1f, Defaults.FilmToe, "Lifts only the deepest shadows — film never reaches pure black. Subtle, but it kills the 'digital void' in dark areas.");

            ImGui.Spacing();
            ImGui.TextDisabled("Lens character — real glass isn't perfect edge to edge.");
            cfg.LensVig = Knob("Optical falloff", cfg.LensVig, 0f, 1f, Defaults.LensVig, "Natural corner light falloff with a touch of corner desaturation — the physical version of the artistic Vignette above.");
            cfg.LensCornerSoft = Knob("Corner softness", cfg.LensCornerSoft, 0f, 1f, Defaults.LensCornerSoft, "Field curvature: detail softens toward the corners while the centre stays sharp.");
        }

        using (var grp = GroupEn("Warp", cfg.FisheyeAmt != 0f || cfg.FisheyeZoom != 1f || cfg.SwirlAmt != 0f || cfg.MosaicSize >= 2f || cfg.KaleidoSegs >= 2f || cfg.WaveAmt > 0f || cfg.GlitchAmt > 0f || cfg.FlowAmt > 0f, cfg.EnWarp, v => cfg.EnWarp = v))
        if (grp.Show)
        {
            cfg.FisheyeAmt = Knob("Fisheye", cfg.FisheyeAmt, -0.8f, 0.8f, Defaults.FisheyeAmt, "Barrel (+) / pincushion (-) curvature.");
            cfg.FisheyeZoom = Knob("Zoom", cfg.FisheyeZoom, 0.7f, 1.4f, Defaults.FisheyeZoom);
            cfg.SwirlAmt = Knob("Swirl", cfg.SwirlAmt, -3.14f, 3.14f, Defaults.SwirlAmt, "Twist around the center.");
            cfg.SwirlRadius = Knob("Swirl radius", cfg.SwirlRadius, 0.05f, 1.2f, Defaults.SwirlRadius);
            cfg.MosaicSize = Knob("Pixelate", cfg.MosaicSize, 0f, 64f, Defaults.MosaicSize, "Cell size in pixels (0 = off).", "%.0f");
            cfg.KaleidoSegs = Knob("Kaleidoscope", cfg.KaleidoSegs, 0f, 16f, Defaults.KaleidoSegs, "Mirror segments (0 = off).", "%.0f");
            cfg.KaleidoRot = Knob("Kaleido rotate", cfg.KaleidoRot, 0f, 6.28f, Defaults.KaleidoRot);
            cfg.WaveAmt = Knob("Wave ripple", cfg.WaveAmt, 0f, 0.05f, Defaults.WaveAmt, "Wavy distortion.", "%.3f");
            cfg.WaveFreq = Knob("Wave frequency", cfg.WaveFreq, 2f, 60f, Defaults.WaveFreq, null, "%.0f");
            if (cfg.WaveAmt > 0f) cfg.WavePhase = Knob("Wave phase", cfg.WavePhase, 0f, 6.28f, Defaults.WavePhase, "Shifts the ripple along — animate by hand for a moving wave.");
            cfg.GlitchAmt = Knob("Glitch", cfg.GlitchAmt, 0f, 0.1f, Defaults.GlitchAmt, "Digital slice tearing.", "%.3f");
            cfg.GlitchBlocks = Knob("Glitch slices", cfg.GlitchBlocks, 4f, 80f, Defaults.GlitchBlocks, null, "%.0f");
            cfg.FlowAmt = Knob("Flow warp", cfg.FlowAmt, 0f, 0.08f, Defaults.FlowAmt, "Organic flowing distortion.", "%.3f");
            cfg.FlowScale = Knob("Flow scale", cfg.FlowScale, 1f, 20f, Defaults.FlowScale, null, "%.1f");
            if (cfg.FlowAmt > 0f) cfg.FlowSeed = Knob("Flow seed", cfg.FlowSeed, 0f, 20f, Defaults.FlowSeed, "Pick a different random flow pattern.", "%.1f");
        }

    }
    private void DrawLightTab(PluginConfig cfg)
    {
        using var tab = ImRaii.TabItem("Light");
        if (!tab) return;
        LightBody(cfg);
    }

    private void LightBody(PluginConfig cfg)
    {

        if (!_live.DepthAvailable)
            ImGui.TextDisabled(_live.Enabled ? "Depth not available yet…" : "Enable live preview for these.");

        using (var grp = GroupEn("Glow", cfg.BloomAmount > 0f || cfg.Halation > 0f || cfg.GodrayAmount > 0f || cfg.Orton > 0f || cfg.Glamour > 0f, cfg.EnGlow, v => cfg.EnGlow = v, true,
                              zoneGet: () => cfg.ZoneGlow, zoneSet: v => cfg.ZoneGlow = v, maskOnly: true))
        if (grp.Show)
        {
            cfg.BloomAmount = Knob("Bloom", cfg.BloomAmount, 0f, 1.5f, Defaults.BloomAmount, "Soft glow from bright areas.");
            cfg.BloomThreshold = Knob("Bloom threshold", cfg.BloomThreshold, 0.3f, 0.95f, Defaults.BloomThreshold, "How bright a pixel must be to bloom.");
            cfg.BloomRadius = Knob("Bloom radius", cfg.BloomRadius, 1f, 6f, Defaults.BloomRadius, "Spread of the glow.");
            ImGui.Spacing();
            cfg.Halation = Knob("Halation", cfg.Halation, 0f, 1f, Defaults.Halation, "Warm bleed around highlights (shares the bloom spread).");
            var hc = ColorPick("Halation tint", new Vector3(cfg.HalationR, cfg.HalationG, cfg.HalationB), new Vector3(Defaults.HalationR, Defaults.HalationG, Defaults.HalationB));
            cfg.HalationR = hc.X; cfg.HalationG = hc.Y; cfg.HalationB = hc.Z;
            ImGui.Spacing();
            cfg.GodrayAmount = Knob("God rays", cfg.GodrayAmount, 0f, 2f, Defaults.GodrayAmount, "Light shafts streaming from a point.");
            cfg.GodrayLightX = Knob("Light X", cfg.GodrayLightX, 0f, 1f, Defaults.GodrayLightX, "Light source horizontal position.");
            cfg.GodrayLightY = Knob("Light Y", cfg.GodrayLightY, 0f, 1f, Defaults.GodrayLightY, "Light source vertical position (0 = top).");
            cfg.GodrayThreshold = Knob("Ray threshold", cfg.GodrayThreshold, 0.3f, 0.95f, Defaults.GodrayThreshold);
            cfg.GodrayDecay = Knob("Ray length", cfg.GodrayDecay, 0.85f, 0.99f, Defaults.GodrayDecay);
            var gc = ColorPick("Ray color", new Vector3(cfg.GodrayR, cfg.GodrayG, cfg.GodrayB), new Vector3(Defaults.GodrayR, Defaults.GodrayG, Defaults.GodrayB));
            cfg.GodrayR = gc.X; cfg.GodrayG = gc.Y; cfg.GodrayB = gc.Z;
            ImGui.Spacing();
            cfg.Orton = Knob("Orton glow", cfg.Orton, 0f, 1f, Defaults.Orton, "Dreamy soft glow over the whole image.");
            cfg.Glamour = Knob("Glamour (pro-mist)", cfg.Glamour, 0f, 1f, Defaults.Glamour, "Cinematic soft diffusion.");
            cfg.GlamourMist = Knob("Glamour mist", cfg.GlamourMist, 0f, 1f, Defaults.GlamourMist, "Misty lifted blacks (with Glamour).");
            cfg.SoftBlurRadius = Knob("Soft blur radius", cfg.SoftBlurRadius, 1f, 6f, Defaults.SoftBlurRadius, "Spread for Orton / Glamour / background blur.");
            ImGui.Spacing();
            cfg.AnamAmount = Knob("Anamorphic flare", cfg.AnamAmount, 0f, 2f, Defaults.AnamAmount, "Wide horizontal light streak from bright spots.");
            cfg.AnamThreshold = Knob("Flare threshold", cfg.AnamThreshold, 0.3f, 0.95f, Defaults.AnamThreshold);
            cfg.AnamLength = Knob("Flare length", cfg.AnamLength, 4f, 30f, Defaults.AnamLength, null, "%.0f");
            var ac = ColorPick("Flare color", new Vector3(cfg.AnamR, cfg.AnamG, cfg.AnamB), new Vector3(Defaults.AnamR, Defaults.AnamG, Defaults.AnamB));
            cfg.AnamR = ac.X; cfg.AnamG = ac.Y; cfg.AnamB = ac.Z;
        }

        using (var grp = GroupEn("Spotlight pool", cfg.SpotAmount > 0f, cfg.EnSpot, v => cfg.EnSpot = v, zoneGet: () => cfg.ZoneSpot, zoneSet: v => cfg.ZoneSpot = v))
        if (grp.Show)
        {
            ImGui.TextDisabled("A pool of light isolating the subject in darkness. Drag Position to place it.");
            cfg.SpotAmount = Knob("Darken outside", cfg.SpotAmount, 0f, 1f, Defaults.SpotAmount, "How dark it gets outside the pool (0 = off).");
            cfg.SpotX = Knob("Position X", cfg.SpotX, 0f, 1f, Defaults.SpotX, "Pool centre X.");
            cfg.SpotY = Knob("Position Y", cfg.SpotY, 0f, 1f, Defaults.SpotY, "Pool centre height. 0 = bottom of frame, 1 = top.");
            cfg.SpotRadius = Knob("Size", cfg.SpotRadius, 0.05f, 1.2f, Defaults.SpotRadius, "Pool radius.");
            cfg.SpotEllipse = Knob("Oval", cfg.SpotEllipse, 0.3f, 3f, Defaults.SpotEllipse, "Vertical squash (1 = a circle).");
            cfg.SpotSoft = Knob("Softness", cfg.SpotSoft, 0.02f, 1f, Defaults.SpotSoft, "Edge falloff of the pool.");
            cfg.SpotAngle = Knob("Angle", cfg.SpotAngle, -1.57f, 1.57f, Defaults.SpotAngle, "Rotation of the oval.");
            cfg.SpotWarm = Knob("Warmth", cfg.SpotWarm, 0f, 1f, Defaults.SpotWarm, "Warm tint inside the pool.");
        }

        using (var grp = GroupEn("Body backlight", cfg.BacklightAmount > 0f, cfg.EnBacklight, v => cfg.EnBacklight = v, zoneGet: () => cfg.ZoneBacklight, zoneSet: v => cfg.ZoneBacklight = v))
        if (grp.Show)
        {
            if (!_live.DepthAvailable) ImGui.TextDisabled("Needs depth — enable live preview in gpose.");
            ImGui.TextDisabled("A warm glow hugging the SUBJECT's silhouette — a backlit rim.");
            cfg.BacklightAmount = Knob("Amount", cfg.BacklightAmount, 0f, 2f, Defaults.BacklightAmount, "Glow strength (0 = off).");
            cfg.BacklightWidth = Knob("Width", cfg.BacklightWidth, 0f, 1f, Defaults.BacklightWidth, "How far the glow wraps in from the silhouette.");
            var bl = ColorPick("Backlight color", new Vector3(cfg.BacklightR, cfg.BacklightG, cfg.BacklightB), new Vector3(Defaults.BacklightR, Defaults.BacklightG, Defaults.BacklightB));
            cfg.BacklightR = bl.X; cfg.BacklightG = bl.Y; cfg.BacklightB = bl.Z;
        }

        using (var grp = GroupEn("Gobo (light patterns)", cfg.GoboAmount > 0f, cfg.EnGobo, v => cfg.EnGobo = v, zoneGet: () => cfg.ZoneGobo, zoneSet: v => cfg.ZoneGobo = v))
        if (grp.Show)
        {
            ImGui.TextDisabled("Projected light/shadow across the scene — blinds, a window, lace, dappled leaves.");
            Combo("Pattern", "##gobopat", UiGobo, cfg.GoboPattern, v => cfg.GoboPattern = v);
            cfg.GoboAmount = Knob("Amount", cfg.GoboAmount, 0f, 1f, Defaults.GoboAmount, "Shadow strength (0 = off).");
            cfg.GoboScale = Knob("Scale", cfg.GoboScale, 1f, 30f, Defaults.GoboScale, "Pattern size / repeats.", "%.0f");
            cfg.GoboAngle = Knob("Angle", cfg.GoboAngle, 0f, 3.14f, Defaults.GoboAngle, "Rotation of the pattern.");
            cfg.GoboSoft = Knob("Softness", cfg.GoboSoft, 0.02f, 0.5f, Defaults.GoboSoft, "Edge softness of the shadows.");
        }

        using (var grp = GroupEn("Backdrop halo", cfg.HaloAmount > 0f, cfg.EnHalo, v => cfg.EnHalo = v, zoneGet: () => cfg.ZoneHalo, zoneSet: v => cfg.ZoneHalo = v))
        if (grp.Show)
        {
            cfg.HaloAmount = Knob("Backdrop halo", cfg.HaloAmount, 0f, 2f, Defaults.HaloAmount, "Soft glow in the background around your subject's silhouette.");
            cfg.HaloSplit = Knob("Halo split", cfg.HaloSplit, 0.02f, 0.4f, Defaults.HaloSplit, "Depth dividing subject from background.", "%.3f");
            var ha = ColorPick("Halo color", new Vector3(cfg.HaloR, cfg.HaloG, cfg.HaloB), new Vector3(Defaults.HaloR, Defaults.HaloG, Defaults.HaloB));
            cfg.HaloR = ha.X; cfg.HaloG = ha.Y; cfg.HaloB = ha.Z;
        }

        using (var grp = GroupEn("Contact shadow", cfg.ShadowAmount > 0f, cfg.EnShadow, v => cfg.EnShadow = v, zoneGet: () => cfg.ZoneShadow, zoneSet: v => cfg.ZoneShadow = v))
        if (grp.Show)
        {
            if (!_live.DepthAvailable) ImGui.TextDisabled("Needs depth — enable live preview in gpose.");
            ImGui.TextDisabled("The shadow your character casts onto whatever is behind them.\nFollows the actual silhouette — this is what stops a character on a\nprocedural background from looking pasted on.");
            cfg.ShadowAmount = Knob("Amount", cfg.ShadowAmount, 0f, 1f, Defaults.ShadowAmount, "How dark the cast shadow is (0 = off).");
            cfg.ShadowSpread = Knob("Spread", cfg.ShadowSpread, 0f, 1f, Defaults.ShadowSpread, "How far the shadow reaches from the body. Small = a tight contact shadow, large = a soft distant one.");
            cfg.ShadowOffsetX = Knob("Offset X", cfg.ShadowOffsetX, -1f, 1f, Defaults.ShadowOffsetX, "Push the shadow sideways — away from your light source.");
            cfg.ShadowOffsetY = Knob("Offset Y", cfg.ShadowOffsetY, -1f, 1f, Defaults.ShadowOffsetY, "Push the shadow up/down. Negative drops it below (light from above).");
            cfg.ShadowSoftness = Knob("Light size", cfg.ShadowSoftness, 0f, 1f, Defaults.ShadowSoftness, "Penumbra falloff: 0 = a defined edge (hard key light), 1 = a soft diffuse falloff (overcast / softbox).");
            cfg.ShadowContact = Knob("Contact darkness", cfg.ShadowContact, 0f, 1f, Defaults.ShadowContact, "An extra tight dark core right where the body meets the background — the occlusion that really sells the contact.");
            var sc = ColorPick("Shadow color", new Vector3(cfg.ShadowR, cfg.ShadowG, cfg.ShadowB), new Vector3(Defaults.ShadowR, Defaults.ShadowG, Defaults.ShadowB));
            cfg.ShadowR = sc.X; cfg.ShadowG = sc.Y; cfg.ShadowB = sc.Z;
            cfg.ShadowDepth = Knob("Subject depth", cfg.ShadowDepth, 0.02f, 0.4f, Defaults.ShadowDepth, "How near a pixel must be to count as the caster (the subject) rather than the surface being shadowed.", "%.3f");
        }

        using (var grp = GroupEn("Ground shadow", cfg.GroundShadow > 0f, cfg.EnGround, v => cfg.EnGround = v, zoneGet: () => cfg.ZoneGround, zoneSet: v => cfg.ZoneGround = v))
        if (grp.Show)
        {
            ImGui.TextDisabled("A soft shadow decal you place under the subject to ground them.\nDrag Position/Size so it sits at the feet. Works on any backdrop.");
            cfg.GroundShadow = Knob("Opacity", cfg.GroundShadow, 0f, 1f, Defaults.GroundShadow, "How dark the shadow is (0 = off).");
            Combo("Mode", "##groundmode", UiGroundMode, cfg.GroundMode, v => cfg.GroundMode = v);
            if (cfg.GroundMode == 1)
            {
                cfg.GroundCastAngle = Knob("Light bearing", cfg.GroundCastAngle, 0f, 3.14f, Defaults.GroundCastAngle, "Which way the light comes from, so which way the shadow runs.");
                cfg.GroundCastLen = Knob("Length", cfg.GroundCastLen, 0f, 1f, Defaults.GroundCastLen, "How far the shadow reaches. A low light throws a long one.");
                ImGui.TextDisabled("  Cast from the actual silhouette, so it follows the pose.\n  Its edge widens as it runs; Softness below sets how much.");
            }
            else
            {
                cfg.GroundShadowX = Knob("Position X", cfg.GroundShadowX, -1.5f, 1.5f, Defaults.GroundShadowX, "Horizontal position (0 = centre). Reaches past both edges, for a subject standing off to the side.");
                cfg.GroundShadowY = Knob("Position Y", cfg.GroundShadowY, -0.5f, 2f, Defaults.GroundShadowY, "Vertical position, set to the subject's feet. 0 = bottom of frame, 1 = top.\nRuns past both, for a low camera or feet below the crop.");
                cfg.GroundShadowW = Knob("Width", cfg.GroundShadowW, 0.02f, 2f, Defaults.GroundShadowW, "Half-width of the shadow ellipse. Wide enough to take in the whole floor under a raking light.");
                cfg.GroundShadowH = Knob("Height", cfg.GroundShadowH, 0.005f, 1.2f, Defaults.GroundShadowH, "Half-height. Low for a flat ground-hugging shadow; raise it for a long one thrown toward the camera.");
            }
            cfg.GroundRipple = Knob("Softness", cfg.GroundRipple, 0f, 1f, Defaults.GroundRipple, "How soft the shadow's edge is.");
            var gt = ColorPick("Shadow tint", new Vector3(cfg.GroundTintR, cfg.GroundTintG, cfg.GroundTintB), new Vector3(Defaults.GroundTintR, Defaults.GroundTintG, Defaults.GroundTintB));
            cfg.GroundTintR = gt.X; cfg.GroundTintG = gt.Y; cfg.GroundTintB = gt.Z;
            ImGui.Separator();
            cfg.GroundLevel = Knob("Theme floor line", cfg.GroundLevel, 0.3f, 0.95f, Defaults.GroundLevel, "Only for themes that draw their own floor (Tempe, Forge, Synthwave, Sunset): the horizon / waterline height.");
        }

    }
    private void DrawSubjectTab(PluginConfig cfg)
    {
        using var tab = ImRaii.TabItem("Subject");
        if (!tab) return;
        SubjectBody(cfg);
    }

    private void SubjectBody(PluginConfig cfg)
    {

        if (!_live.DepthAvailable)
            ImGui.TextDisabled(_live.Enabled ? "Depth not available yet…" : "Enable live preview for these.");

        using (var grp = GroupEn("Rim & separation", cfg.RimStrength > 0f || cfg.SubjectPop > 0f, cfg.EnRim, v => cfg.EnRim = v, true, zoneGet: () => cfg.ZoneRim, zoneSet: v => cfg.ZoneRim = v))
        if (grp.Show)
        {
            cfg.RimStrength = Knob("Separation rim", cfg.RimStrength, 0f, 2f, Defaults.RimStrength, "Lights the subject's silhouette where it meets the background.");
            cfg.RimThreshold = Knob("Edge sensitivity", cfg.RimThreshold, 0.002f, 0.06f, Defaults.RimThreshold, "Lower = catches finer edges.", "%.3f");
            cfg.RimWidth = Knob("Rim width", cfg.RimWidth, 1f, 5f, Defaults.RimWidth, null, "%.0f");
            var rc = ColorPick("Rim color", new Vector3(cfg.RimR, cfg.RimG, cfg.RimB), new Vector3(Defaults.RimR, Defaults.RimG, Defaults.RimB));
            cfg.RimR = rc.X; cfg.RimG = rc.Y; cfg.RimB = rc.Z;

            ImGui.Spacing();
            ImGui.TextDisabled("Two outline colours — for two characters in one shot.");
            cfg.RimSplit = Knob("Split outline", cfg.RimSplit, 0f, 1f, Defaults.RimSplit,
                "Give each side of the frame its own outline colour. Depth can tell a subject from the\nbackground, but not one character from another at the same distance — so the divide is\nspatial: whoever stands left gets the first colour, whoever stands right gets the second.\nApplies to the rim AND the Body backlight, so both outlines agree. 0 = one colour.");
            if (cfg.RimSplit > 0f)
            {
                var r2 = ColorPick("  Second rim color", new Vector3(cfg.Rim2R, cfg.Rim2G, cfg.Rim2B), new Vector3(Defaults.Rim2R, Defaults.Rim2G, Defaults.Rim2B));
                cfg.Rim2R = r2.X; cfg.Rim2G = r2.Y; cfg.Rim2B = r2.Z;
                var b2 = ColorPick("  Second backlight", new Vector3(cfg.Backlight2R, cfg.Backlight2G, cfg.Backlight2B), new Vector3(Defaults.Backlight2R, Defaults.Backlight2G, Defaults.Backlight2B));
                cfg.Backlight2R = b2.X; cfg.Backlight2G = b2.Y; cfg.Backlight2B = b2.Z;
                cfg.RimSplitOffset = Knob("  Divide position", cfg.RimSplitOffset, -0.7f, 0.7f, Defaults.RimSplitOffset, "Slide the divide across the frame — line it up with your background seam.");
                cfg.RimSplitAngle = Knob("  Divide angle", cfg.RimSplitAngle, 0f, 3.14f, Defaults.RimSplitAngle, "0 = a left/right divide; ~1.57 = top/bottom.");
                cfg.RimSplitSoft = Knob("  Divide softness", cfg.RimSplitSoft, 0.005f, 0.4f, Defaults.RimSplitSoft, "How gradually one colour becomes the other.", "%.3f");
            }
            ImGui.Spacing();
            cfg.SubjectPop = Knob("Subject pop", cfg.SubjectPop, 0f, 1f, Defaults.SubjectPop, "Boost contrast + saturation on the near subject.");
        }

        using (var grp = GroupEn("Skin warmth & flush", cfg.SkinWarmth > 0f || cfg.SkinFlush > 0f, cfg.EnSkin, v => cfg.EnSkin = v, zoneGet: () => cfg.ZoneSkin, zoneSet: v => cfg.ZoneSkin = v))
        if (grp.Show)
        {
            if (!_live.DepthAvailable) ImGui.TextDisabled("Needs depth — enable live preview in gpose.");
            ImGui.TextDisabled("Warms the SUBJECT so skin reads alive, not flat.");
            cfg.SkinWarmth = Knob("Subsurface warmth", cfg.SkinWarmth, 0f, 1f, Defaults.SkinWarmth, "Warm glow in the midtone (fleshy) areas.");
            cfg.SkinFlush = Knob("Flush", cfg.SkinFlush, 0f, 1f, Defaults.SkinFlush, "Overall rosy tint on the subject.");
            var sk = ColorPick("Warmth / flush color", new Vector3(cfg.SkinTintR, cfg.SkinTintG, cfg.SkinTintB), new Vector3(Defaults.SkinTintR, Defaults.SkinTintG, Defaults.SkinTintB));
            cfg.SkinTintR = sk.X; cfg.SkinTintG = sk.Y; cfg.SkinTintB = sk.Z;
        }

        using (var grp = GroupEn("Beauty softening", cfg.BeautyAmount > 0f, cfg.EnBeauty, v => cfg.EnBeauty = v, zoneGet: () => cfg.ZoneBeauty, zoneSet: v => cfg.ZoneBeauty = v))
        if (grp.Show)
        {
            if (!_live.DepthAvailable) ImGui.TextDisabled("Needs depth — enable live preview in gpose.");
            ImGui.TextDisabled("A dreamy soft-focus bloom on the SUBJECT only (background stays crisp).\nSubject-masked version of Effects ▸ Glow ▸ Glamour, which is full-frame.");
            cfg.BeautyAmount = Knob("Amount", cfg.BeautyAmount, 0f, 1f, Defaults.BeautyAmount, "Softening strength (0 = off).");
            cfg.BeautyRadius = Knob("Softness", cfg.BeautyRadius, 0f, 2f, Defaults.BeautyRadius, "How wide the diffusion spreads.");
            cfg.BeautyGlow = Knob("Highlight bloom", cfg.BeautyGlow, 0f, 1.5f, Defaults.BeautyGlow, "Extra glow lifted from the bright areas.");
        }

        using (var grp = GroupEn("Wet skin / sheen", cfg.WetAmount > 0f, cfg.EnWet, v => cfg.EnWet = v, zoneGet: () => cfg.ZoneWet, zoneSet: v => cfg.ZoneWet = v))
        if (grp.Show)
        {
            if (!_live.DepthAvailable)
                ImGui.TextDisabled("Needs depth — enable live preview in gpose.");
            ImGui.TextDisabled("A glossy wet-look on the SUBJECT (FFXIV's wet flag skips skin).");
            cfg.WetAmount = Knob("Amount", cfg.WetAmount, 0f, 1f, Defaults.WetAmount, "Master strength (0 = off).");
            cfg.WetDeepen = Knob("Soak (darken)", cfg.WetDeepen, 0f, 1f, Defaults.WetDeepen, "The main wet cue: a water film darkens the whole surface (darks more than brights) and enriches saturation. Brightness comes back through the sheen. Wet hair/skin/fabric all read mostly through this.");
            ImGui.Spacing(); ImGui.TextDisabled("Sheen — how it catches light");
            cfg.WetHighlight = Knob("Highlight follow", cfg.WetHighlight, 0f, 2f, Defaults.WetHighlight, "Amplifies the character's EXISTING highlights, so the sheen tracks the real game / plugin key light wherever it falls. The most natural control.");
            cfg.WetFresnel = Knob("Edge sheen", cfg.WetFresnel, 0f, 2f, Defaults.WetFresnel, "Wet glow at grazing angles / relief edges. View-based, so it reads wet under any lighting.");
            cfg.WetShine = Knob("Directional glint", cfg.WetShine, 0f, 2f, Defaults.WetShine, "An added specular hotspot from the Light X/Y direction below (artistic control on top of the light-following sheen).");
            cfg.WetRough = Knob("Roughness", cfg.WetRough, 0f, 1f, Defaults.WetRough, "Tight glossy highlight (0) -> broad satin sheen (1).");
            if (cfg.WetShine > 0f)
            {
                cfg.WetLightX = Knob("Light X", cfg.WetLightX, -1f, 1f, Defaults.WetLightX, "Direction the directional glint catches from (left/right).");
                cfg.WetLightY = Knob("Light Y", cfg.WetLightY, -1f, 1f, Defaults.WetLightY, "Directional glint light (up/down).");
            }
            ImGui.Spacing(); ImGui.TextDisabled("Droplets");
            cfg.WetDroplets = Knob("Droplets", cfg.WetDroplets, 0f, 1f, Defaults.WetDroplets, "Beaded water on the skin (0 = smooth wet). Amount = coverage.");
            if (cfg.WetDroplets > 0f)
            {
                cfg.WetDropSize = Knob("  Bead size", cfg.WetDropSize, 0f, 1f, Defaults.WetDropSize, "Size of each droplet.");
                cfg.WetDropDensity = Knob("  Density", cfg.WetDropDensity, 0f, 1f, Defaults.WetDropDensity, "How many droplets (finer = more, smaller cells).");
                cfg.WetDropTrail = Knob("  Runs", cfg.WetDropTrail, 0f, 1f, Defaults.WetDropTrail, "Elongate droplets into downward runs / trickles.");
            }
            ImGui.Spacing();
            cfg.WetDepth = Knob("Subject depth", cfg.WetDepth, 0.02f, 0.4f, Defaults.WetDepth, "How near a pixel must be to count as the subject.", "%.3f");
        }

    }

    private void DrawExportTab(PluginConfig cfg)
    {
        using var tab = ImRaii.TabItem("Export");
        if (!tab) return;
        ExportBody(cfg);
    }

    private void ExportBody(PluginConfig cfg)
    {

        ImGui.TextDisabled("Aspect / crop");
        string[] an = LiveOverlay.AspectNames;
        int asp = cfg.ExportAspect < 0 || cfg.ExportAspect >= an.Length ? 0 : cfg.ExportAspect;
        ImGui.PushItemWidth(180f);
        if (ImGui.BeginCombo("##aspect", an[asp]))
        {
            for (int i = 0; i < an.Length; i++)
                if (ImGui.Selectable(an[i], asp == i)) { cfg.ExportAspect = i; _dirty = true; }
            ImGui.EndCombo();
        }
        ImGui.PopItemWidth();
        if (cfg.ExportAspect != 0)
        {
            ImGui.SameLine();
            var sf = cfg.ShowExportFrame;
            if (ImGui.Checkbox("Show frame", ref sf)) { cfg.ShowExportFrame = sf; _dirty = true; }
            ImGui.TextDisabled("Export is cropped to this aspect (centered). The frame overlay in\ngpose shows exactly what will be saved.");
        }

        ImGui.Spacing();
        ImGui.TextDisabled("Quality");
        int[] scales = { 1, 2, 4 };
        int scIdx = Math.Max(0, Array.IndexOf(scales, Math.Clamp(cfg.ExportScale, 1, 4)));
        ImGui.PushItemWidth(180f);
        if (ImGui.BeginCombo("##exportscale", scales[scIdx] + "x  (supersampled)"))
        {
            for (int i = 0; i < scales.Length; i++)
                if (ImGui.Selectable(scales[i] + "x", scIdx == i)) { cfg.ExportScale = scales[i]; _dirty = true; }
            ImGui.EndCombo();
        }
        ImGui.PopItemWidth();
        ImGui.TextDisabled(cfg.ExportScale > 1
            ? $"Renders the whole look at {cfg.ExportScale}x — backgrounds, patterns and\neffects gain real detail; the character is upscaled. Brief hitch on save."
            : "1x = the on-screen resolution. 2x / 4x render larger for a crisper image.");

        string[] fmts = { "PNG (lossless)", "JPEG (smaller)" };
        int fmt = Math.Clamp(cfg.ExportFormat, 0, 1);
        ImGui.TextUnformatted("Format"); ImGui.SameLine(70f); ImGui.PushItemWidth(180f);
        if (ImGui.BeginCombo("##exportfmt", fmts[fmt]))
        {
            for (int i = 0; i < fmts.Length; i++)
                if (ImGui.Selectable(fmts[i], fmt == i)) { cfg.ExportFormat = i; _dirty = true; }
            ImGui.EndCombo();
        }
        ImGui.PopItemWidth();
        if (cfg.ExportFormat == 1)
        {
            cfg.ExportJpegQuality = (int)Knob("JPEG quality", cfg.ExportJpegQuality, 60f, 100f, Defaults.ExportJpegQuality,
                "Higher = better looking + bigger. 90-95 is visually near-lossless at a fraction of PNG's size; below ~80 artifacts start to show.", "%.0f");
            ImGui.TextDisabled("JPEG has no transparency and is lossy — use PNG if you need either.");
        }
        ImGui.Separator();

        using (var grp = GroupEn("Frame & corners", cfg.EnFrame, cfg.EnFrame, v => cfg.EnFrame = v))
        if (grp.Show)
        {
            ImGui.TextDisabled("A print-style mat and rounded corners on the SAVED image.\nEvery size is a share of the short side, so one setting looks right\non any layout — 16:9, 1:1 or 9:16 alike.");
            cfg.FrameMat = Knob("Mat width", cfg.FrameMat, 0f, 0.2f, Defaults.FrameMat, "The border band around the photo. 0 = no border.", "%.3f");
            if (cfg.FrameMat > 0f)
            {
                var mi = cfg.FrameMatInset;
                if (ImGui.Checkbox("Keep export size (inset mat)", ref mi)) { cfg.FrameMatInset = mi; _dirty = true; }
                if (ImGui.IsItemHovered()) ImGui.SetTooltip(
                    "On: the mat eats inward, so the saved file keeps exactly the size and\naspect your layout asked for — the picture is scaled down to fit inside\nthe border (area-averaged, so it stays clean).\nOff: the canvas grows outward by the mat, like a matted print, and the\noutput is a little larger than the chosen aspect.");
                var mc = ColorPick("Mat color", new Vector3(cfg.FrameMatR, cfg.FrameMatG, cfg.FrameMatB), new Vector3(Defaults.FrameMatR, Defaults.FrameMatG, Defaults.FrameMatB));
                cfg.FrameMatR = mc.X; cfg.FrameMatG = mc.Y; cfg.FrameMatB = mc.Z;
                cfg.FrameBottom = Knob("Bottom weight", cfg.FrameBottom, 0f, 1f, Defaults.FrameBottom, "Extra depth on the bottom band only — the classic gallery mat, which looks better balanced than an even border.");
                cfg.FrameShadow = Knob("Photo shadow", cfg.FrameShadow, 0f, 1f, Defaults.FrameShadow, "A soft shadow of the photo falling onto the mat, so the print sits above it instead of being flush.");
                cfg.FrameKeyline = Knob("Keyline", cfg.FrameKeyline, 0f, 0.01f, Defaults.FrameKeyline, "A hairline drawn where the photo meets the mat — the detail that makes a border look deliberate. 0 = none.", "%.4f");
                if (cfg.FrameKeyline > 0f)
                {
                    var kc = ColorPick("Keyline color", new Vector3(cfg.FrameKeyR, cfg.FrameKeyG, cfg.FrameKeyB), new Vector3(Defaults.FrameKeyR, Defaults.FrameKeyG, Defaults.FrameKeyB));
                    cfg.FrameKeyR = kc.X; cfg.FrameKeyG = kc.Y; cfg.FrameKeyB = kc.Z;
                }
            }
            ImGui.Spacing();
            cfg.FrameCorner = Knob("Photo corners", cfg.FrameCorner, 0f, 0.2f, Defaults.FrameCorner, "Corner radius of the photo itself. True anti-aliased arcs at any size or export scale.", "%.3f");
            if (cfg.FrameCorner > 0f || cfg.FrameOuterCorner > 0f)
                cfg.FrameSmooth = Knob("Corner smoothing", cfg.FrameSmooth, 0f, 1f, Defaults.FrameSmooth,
                    "Shapes the corner curve. 0 is a plain circular arc, which meets the straight edge with\nan abrupt change in curvature — the thing that makes a rounded rectangle look like a default.\nHigher eases it into a superellipse (a squircle): the continuous-curvature corner used by\nApple icons and Figma's corner smoothing. Around 0.6 reads deliberate without looking soft.");
            if (cfg.FrameMat > 0f)
                cfg.FrameOuterCorner = Knob("Outer corners", cfg.FrameOuterCorner, 0f, 0.2f, Defaults.FrameOuterCorner, "Corner radius of the outside of the mat.", "%.3f");
            var fa = cfg.FrameAlpha;
            if (ImGui.Checkbox("Transparent outside corners", ref fa)) { cfg.FrameAlpha = fa; _dirty = true; }
            if (ImGui.IsItemHovered()) ImGui.SetTooltip("Rounded corners cut to real transparency instead of being filled\nwith the mat colour. PNG only — JPEG has no alpha and falls back\nto the mat colour automatically.");
            if (cfg.FrameAlpha && cfg.ExportFormat == 1)
                ImGui.TextDisabled("JPEG can't store transparency — corners will use the mat color.");
            ImGui.TextDisabled("Shown as an outline in the export frame overlay; the real\nframe is composed when you save.");
        }
        ImGui.Separator();

        var dir = cfg.OutputDirectory;
        ImGui.TextDisabled("Export folder");
        ImGui.PushItemWidth(-1f);
        if (ImGui.InputText("##dir", ref dir, 1024)) cfg.OutputDirectory = dir;
        if (ImGui.IsItemDeactivatedAfterEdit()) _dirty = true;
        ImGui.PopItemWidth();
        if (ImGui.Button("Browse…"))
        {
            _dialogs.OpenFolderDialog("Choose export folder", (ok, path) =>
            {
                if (ok && !string.IsNullOrWhiteSpace(path)) { cfg.OutputDirectory = path; cfg.Save(); }
            });
        }
        ImGui.SameLine();
        if (ImGui.Button("Open folder")) OpenOutputFolder();

        ImGui.Spacing();
        ImGui.Separator();
        using (ImRaii.Disabled(!_gate.IsActive))
        {
            if (ImGui.Button("Capture GPose → Save PNG", new Vector2(-1f, 0))) RequestSave();
        }
        ImGui.TextDisabled(_gate.IsActive
            ? "Saves exactly what the preview shows (all effects baked in)."
            : "Enter GPose to capture.");

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.TextDisabled("Capture fix-ups — only if the image looks wrong");
        var cut = cfg.ExportTransparent;
        if (ImGui.Checkbox("Transparent background (PNG only)", ref cut))
        {
            cfg.ExportTransparent = cut;
            if (!cut) cfg.DebugShowMatte = false;
            _dirty = true;
        }
        if (ImGui.IsItemHovered()) ImGui.SetTooltip(
            "Export the subject on nothing, for compositing elsewhere.\n\n" +
            "The background style and the solid fill are switched off for the\n" +
            "export only \u2014 a backdrop and a transparent background cannot both\n" +
            "be true, and the preview keeps showing the look as you composed it.\n\n" +
            "JPEG has no transparency, so this does nothing in that format.");

        if (cfg.ExportTransparent)
        {
            ImGui.Indent(10f);
            if (cfg.ExportFormat == 1)
                ImGui.TextColored(new Vector4(1f, 0.62f, 0.25f, 1f),
                    "Format is JPEG, which has no transparency. Switch to PNG below.");

            cfg.CutoutShrink = Knob("  Edge shrink", cfg.CutoutShrink, 0f, 1f, Defaults.CutoutShrink,
                "Pulls the matte inward. The depth silhouette is a little wider than the character, because the\nanti-aliased pixels along the edge are a blend of subject and whatever was behind it \u2014 keeping those\nfully opaque is what leaves a halo of old background around a cutout. Raise this until the halo goes.");
            cfg.CutoutFeather = Knob("  Edge softness", cfg.CutoutFeather, 0f, 1f, Defaults.CutoutFeather,
                "How sharply alpha falls off. A little softness hides the depth buffer's stair-stepping;\ntoo much makes the subject look like it is dissolving.");

            var mt = cfg.DebugShowMatte;
            if (ImGui.Checkbox("  Preview the matte", ref mt))
            {
                cfg.DebugShowMatte = mt;
                if (mt) { cfg.DebugShowGate = false; cfg.DebugShowDepth = false; cfg.DebugShowClipping = false; cfg.DebugShowMask = false; }
                _dirty = true;
            }
            if (ImGui.IsItemHovered()) ImGui.SetTooltip(
                "Shows what will be transparent, over a checkerboard, so the edge can be\njudged here rather than after opening the file somewhere else.");
            if (cfg.DebugShowMatte)
                ImGui.TextColored(new Vector4(1f, 0.62f, 0.25f, 1f), "  Matte preview is on \u2014 untick to see the shot.");

            ImGui.TextDisabled("  Hair and effect particles do not write depth, so they are kept whole\n  rather than cut cleanly. That is a limit of depth, not of the setting.");
            ImGui.Unindent(10f);
            ImGui.Spacing();
        }

        var emb = cfg.EmbedLookInPng;
        if (ImGui.Checkbox("Store the look inside exported PNGs", ref emb)) { cfg.EmbedLookInPng = emb; _dirty = true; }
        if (ImGui.IsItemHovered()) ImGui.SetTooltip(
            "Writes the settings into the file, so the image can be imported later\n" +
            "on the Looks tab to get its look back. Costs a few kilobytes and does\n" +
            "not touch a single pixel.\n\n" +
            "Only numbers travel. Element-layer image paths are left out, so nothing\n" +
            "about your folders goes into a picture you share. JPEG has nowhere to\n" +
            "put it, so it carries nothing either way.");
        ImGui.Spacing();

        var swap = cfg.SwapRedBlue;
        if (ImGui.Checkbox("Swap red/blue (fix blue tint)", ref swap)) { cfg.SwapRedBlue = swap; _dirty = true; }
        var flip = cfg.FlipVertical;
        if (ImGui.Checkbox("Flip vertically (fix upside-down)", ref flip)) { cfg.FlipVertical = flip; _dirty = true; }
    }

    private void DrawUniversalControls(PluginConfig cfg)
    {
        if (ImGui.Button("Randomize")) { Roll(cfg); _dirty = true; }
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Roll whatever is not locked below — a coherent archetype + harmonious palette.\nKeep rolling until something catches your eye, then tweak.");
        ImGui.SameLine();
        using (ImRaii.Disabled(_rollUndo == null))
        {
            if (ImGui.Button("Undo")) { RestoreRoll(cfg); _dirty = true; }
        }
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Restore the state from just before your last roll.");
        ImGui.SameLine();
        if (ImGui.SmallButton("?")) { }
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Build any background from layers: a FIELD (gradient + noise + pattern),\na SCENE (horizon / ground / sun / mountains), ATMOSPHERE (light) and\nPARTICLES. Colours 1-5 + Accent are the shared palette above.");
        ImGui.TextDisabled("Lock:"); ImGui.SameLine();
        ImGui.Checkbox("Palette##lockpal", ref _lockPalette); ImGui.SameLine();
        ImGui.Checkbox("Structure##lockstruct", ref _lockStructure);

        if (ImGui.CollapsingHeader("Field##ufield", ImGuiTreeNodeFlags.DefaultOpen))
        {
            Combo("Base", "##univbase", UiBases, cfg.UnivBase, v => cfg.UnivBase = v);
            Combo("Noise", "##univnoise", UiNoises, cfg.UnivNoise, v => cfg.UnivNoise = v);
            if (cfg.UnivNoise > 0)
            {
                Combo("Combine", "##univblend", UiBlends, cfg.UnivBlend, v => cfg.UnivBlend = v);
                cfg.UnivNoiseAmt = Knob("Noise amount", cfg.UnivNoiseAmt, 0f, 1.5f, Defaults.UnivNoiseAmt, "How strongly the noise field expresses.");
                cfg.UnivNoiseScale = Knob("Noise scale", cfg.UnivNoiseScale, 0f, 3f, Defaults.UnivNoiseScale, "Noise frequency (higher = finer detail).");
            }
            cfg.UnivWarp = Knob("Domain warp", cfg.UnivWarp, 0f, 1.5f, Defaults.UnivWarp, "Distorts the whole field with a low-frequency swirl (organic look).");
            if (cfg.UnivWarp > 0f) cfg.UnivDetail = Knob("Warp detail", cfg.UnivDetail, 0f, 1f, Defaults.UnivDetail, "Finer (1) vs broader (0) warp.");
            Combo("Pattern", "##univpat", UiUpats, cfg.UnivPattern, v => cfg.UnivPattern = v);
            if (cfg.UnivPattern > 0)
            {
                Combo("  Blend", "##univpatblend", UiPatBlend, cfg.UnivPatBlend, v => cfg.UnivPatBlend = v);
                var pco = cfg.PatColOverride;
                if (ImGui.Checkbox("  Own pattern color", ref pco))
                {
                    cfg.PatColOverride = pco; _dirty = true;
                    if (pco && cfg.PatColR == 0f && cfg.PatColG == 0f && cfg.PatColB == 0f
                            && cfg.PatCol5R == 0f && cfg.PatCol5G == 0f && cfg.PatCol5B == 0f)
                    {
                        cfg.PatColR = Defaults.PatColR; cfg.PatColG = Defaults.PatColG; cfg.PatColB = Defaults.PatColB;
                        cfg.PatCol2R = Defaults.PatCol2R; cfg.PatCol2G = Defaults.PatCol2G; cfg.PatCol2B = Defaults.PatCol2B;
                        cfg.PatCol3R = Defaults.PatCol3R; cfg.PatCol3G = Defaults.PatCol3G; cfg.PatCol3B = Defaults.PatCol3B;
                        cfg.PatCol4R = Defaults.PatCol4R; cfg.PatCol4G = Defaults.PatCol4G; cfg.PatCol4B = Defaults.PatCol4B;
                        cfg.PatCol5R = Defaults.PatCol5R; cfg.PatCol5G = Defaults.PatCol5G; cfg.PatCol5B = Defaults.PatCol5B;
                        cfg.PatMatR = Defaults.PatMatR; cfg.PatMatG = Defaults.PatMatG; cfg.PatMatB = Defaults.PatMatB;
                        cfg.PatMatTint = Defaults.PatMatTint;
                    }
                }
                if (ImGui.IsItemHovered()) ImGui.SetTooltip(
                    "Patterns borrow the accent colour, which also drives stars, haze and glow —\nso a pattern could never be coloured on its own. Tick this to give it one.\nA Material below overrides it with its own colour.");
                if (cfg.PatColOverride)
                {
                    Combo("  Color mode", "##patcolmode", UiPatColMode, cfg.PatColMode, v => cfg.PatColMode = v);
                    if (cfg.PatColMode != 3 && cfg.PatColMode != 5)
                    {
                        var pc2 = ColorPick(cfg.PatColMode == 1 ? "  Color A" : ((cfg.PatColMode == 2 || cfg.PatColMode == 4) ? "  Stop 1" : "  Pattern color"), new Vector3(cfg.PatColR, cfg.PatColG, cfg.PatColB), new Vector3(Defaults.PatColR, Defaults.PatColG, Defaults.PatColB));
                        cfg.PatColR = pc2.X; cfg.PatColG = pc2.Y; cfg.PatColB = pc2.Z;
                    }
                    if (cfg.PatColMode == 1)
                    {
                        var pc3 = ColorPick("  Color B", new Vector3(cfg.PatCol2R, cfg.PatCol2G, cfg.PatCol2B), new Vector3(Defaults.PatCol2R, Defaults.PatCol2G, Defaults.PatCol2B));
                        cfg.PatCol2R = pc3.X; cfg.PatCol2G = pc3.Y; cfg.PatCol2B = pc3.Z;
                    }
                    if (cfg.PatColMode == 2 || cfg.PatColMode == 4)
                    {
                        var q2 = ColorPick("  Stop 2", new Vector3(cfg.PatCol2R, cfg.PatCol2G, cfg.PatCol2B), new Vector3(Defaults.PatCol2R, Defaults.PatCol2G, Defaults.PatCol2B));
                        cfg.PatCol2R = q2.X; cfg.PatCol2G = q2.Y; cfg.PatCol2B = q2.Z;
                        var q3 = ColorPick("  Stop 3", new Vector3(cfg.PatCol3R, cfg.PatCol3G, cfg.PatCol3B), new Vector3(Defaults.PatCol3R, Defaults.PatCol3G, Defaults.PatCol3B));
                        cfg.PatCol3R = q3.X; cfg.PatCol3G = q3.Y; cfg.PatCol3B = q3.Z;
                        var q4 = ColorPick("  Stop 4", new Vector3(cfg.PatCol4R, cfg.PatCol4G, cfg.PatCol4B), new Vector3(Defaults.PatCol4R, Defaults.PatCol4G, Defaults.PatCol4B));
                        cfg.PatCol4R = q4.X; cfg.PatCol4G = q4.Y; cfg.PatCol4B = q4.Z;
                        var q5 = ColorPick("  Stop 5", new Vector3(cfg.PatCol5R, cfg.PatCol5G, cfg.PatCol5B), new Vector3(Defaults.PatCol5R, Defaults.PatCol5G, Defaults.PatCol5B));
                        cfg.PatCol5R = q5.X; cfg.PatCol5G = q5.Y; cfg.PatCol5B = q5.Z;
                        if (ImGui.SmallButton("Rainbow##patrainbow"))
                        {
                            cfg.PatColR = 0.95f; cfg.PatColG = 0.25f; cfg.PatColB = 0.25f;
                            cfg.PatCol2R = 0.98f; cfg.PatCol2G = 0.72f; cfg.PatCol2B = 0.20f;
                            cfg.PatCol3R = 0.35f; cfg.PatCol3G = 0.85f; cfg.PatCol3B = 0.40f;
                            cfg.PatCol4R = 0.25f; cfg.PatCol4G = 0.60f; cfg.PatCol4B = 0.95f;
                            cfg.PatCol5R = 0.70f; cfg.PatCol5G = 0.35f; cfg.PatCol5B = 0.90f;
                            _dirty = true;
                        }
                        ImGui.SameLine();
                        if (ImGui.SmallButton("Sunset##patsunset"))
                        {
                            cfg.PatColR = 0.16f; cfg.PatColG = 0.10f; cfg.PatColB = 0.28f;
                            cfg.PatCol2R = 0.55f; cfg.PatCol2G = 0.18f; cfg.PatCol2B = 0.38f;
                            cfg.PatCol3R = 0.90f; cfg.PatCol3G = 0.36f; cfg.PatCol3B = 0.30f;
                            cfg.PatCol4R = 0.98f; cfg.PatCol4G = 0.68f; cfg.PatCol4B = 0.32f;
                            cfg.PatCol5R = 1.00f; cfg.PatCol5G = 0.92f; cfg.PatCol5B = 0.70f;
                            _dirty = true;
                        }
                        ImGui.SameLine();
                        if (ImGui.SmallButton("Fire##patfire"))
                        {
                            cfg.PatColR = 0.14f; cfg.PatColG = 0.02f; cfg.PatColB = 0.02f;
                            cfg.PatCol2R = 0.74f; cfg.PatCol2G = 0.11f; cfg.PatCol2B = 0.02f;
                            cfg.PatCol3R = 0.98f; cfg.PatCol3G = 0.42f; cfg.PatCol3B = 0.05f;
                            cfg.PatCol4R = 1.00f; cfg.PatCol4G = 0.80f; cfg.PatCol4B = 0.24f;
                            cfg.PatCol5R = 1.00f; cfg.PatCol5G = 0.97f; cfg.PatCol5B = 0.88f;
                            _dirty = true;
                        }
                        ImGui.SameLine(); ImGui.TextDisabled("presets");
                        if (cfg.PatColMode == 4)
                            ImGui.TextDisabled("  Stop 1 is the faintest part of the pattern, stop 5 the strongest.\n  With Flames + Fire that is ember red at the edges to white at the core.");
                    }
                    if (cfg.PatColMode == 5)
                        ImGui.TextDisabled("  Each field colours its own pattern, from its own 5 colors,\n  mapped to the pattern\u2019s density. Use this when two fields need\n  different pattern colors \u2014 green wind in front, purple bolts behind.");
                    if (cfg.PatColMode == 3)
                        ImGui.TextDisabled("  Uses the field's own 5 colors. Cohesive, but it can read as\n  camouflage against its own background \u2014 Palette gives you the\n  same arrangement in colors of your choosing.");
                }
                Combo("  Material", "##patmat", UiPatMat, cfg.PatMat, v => cfg.PatMat = v);
                if (cfg.PatMat > 0)
                {
                    var pmc = ColorPick("  Material color", new Vector3(cfg.PatMatR, cfg.PatMatG, cfg.PatMatB), new Vector3(Defaults.PatMatR, Defaults.PatMatG, Defaults.PatMatB));
                    cfg.PatMatR = pmc.X; cfg.PatMatG = pmc.Y; cfg.PatMatB = pmc.Z;
                    cfg.PatMatTint = Knob("  Material tint", cfg.PatMatTint, 0f, 1f, Defaults.PatMatTint,
                        "How far the material colour takes over the pattern colour. 1 = pure material (the old\nbehaviour). Lower it and your pattern colour shows through the body while the metal keeps\nits travel and its catch — that is how a coloured metal or a patina reads.");
                    ImGui.TextDisabled("  Roughness / sheen / highlight are shared by all fields.");
                    cfg.PatMatRough = Knob("  Roughness", cfg.PatMatRough, 0f, 1f, Defaults.PatMatRough, "Tight mirror highlight (0) -> broad satin (1).");
                    cfg.PatMatSheen = Knob("  Sheen", cfg.PatMatSheen, 0f, 2f, Defaults.PatMatSheen, "How strongly the catch reads. It can no longer burn to white — the top end rolls off — so this is safe to push.");
                    cfg.PatMatPos = Knob("  Highlight position", cfg.PatMatPos, 0f, 1f, Defaults.PatMatPos, "Slides the catch across the frame. Put it where you want the eye to go — usually just off your subject, not on them.");
                    cfg.PatMatRange = Knob("  Sweep range", cfg.PatMatRange, 0f, 1f, Defaults.PatMatRange, "How far the metal travels from shadow to catch across the frame. Low = an even sheet; high = a dramatic raking light.");
                    ImGui.TextDisabled("  Lit by the field's own light direction (Material group),\n  so the pattern agrees with whatever lights the backdrop.\n  Needs the Glow blend to show its highlights fully.");
                }
                cfg.UnivPatStrength = Knob("  Strength", cfg.UnivPatStrength, 0f, 1f, Defaults.UnivPatStrength,
                    "How strongly the pattern shows. Glow/Shade blend keep the field underneath (they compose cleanly); Ink replaces toward the accent colour. Scale = density, Scale Y = feature size.");
            }
            ImGui.TextDisabled("Shaping (shared knobs, in the groups below): Current flow = stretch/rise,\nFilament contrast = crush to veins, Vortex twist = swirl.");
        }

        if (ImGui.CollapsingHeader("Scene##uscene"))
        {
            cfg.UnivHorizon = Knob("Horizon", cfg.UnivHorizon, 0f, 0.95f, Defaults.UnivHorizon,
                "Splits the frame into sky and ground at this height. 0 = no horizon (a pure field). This one control turns the engine into a scene.");
            if (cfg.UnivHorizon > 0f)
                Combo("Ground", "##univground", UiGrounds, cfg.UnivGround, v => cfg.UnivGround = v);
            Combo("Orb (sun/moon)", "##univorb", UiOrbs, cfg.UnivOrb, v => cfg.UnivOrb = v);
            if (cfg.UnivOrb > 0)
            {
                cfg.UnivOrbX = Knob("Orb X", cfg.UnivOrbX, 0f, 1f, Defaults.UnivOrbX, "Horizontal position of the sun / moon.");
                cfg.UnivOrbY = Knob("Orb Y", cfg.UnivOrbY, 0f, 1f, Defaults.UnivOrbY, "Vertical position — just above the horizon for a sunset.");
                cfg.UnivOrbSize = Knob("Orb size", cfg.UnivOrbSize, 0.01f, 0.6f, Defaults.UnivOrbSize, "Radius. Core glow rides on the Glow knob.");
            }
            cfg.UnivRidges = Knob("Ridges", cfg.UnivRidges, 0f, 1f, Defaults.UnivRidges,
                "Layered mountain silhouettes receding into the distance (0 = off). Offset X/Y slides them.");
        }

        if (ImGui.CollapsingHeader("Atmosphere##uatmo"))
        {
            cfg.UnivCaustic = Knob("Caustics", cfg.UnivCaustic, 0f, 1.5f, Defaults.UnivCaustic, "Rippling refracted-light web — water surface, ice, aether. Drifts with Current flow + Animation speed, tinted by Accent.");
            cfg.UnivShafts = Knob("Light shafts", cfg.UnivShafts, 0f, 1.5f, Defaults.UnivShafts, "Volumetric god-ray beams fanning from the orb (or the top when no orb). Accent-tinted.");
            ImGui.TextDisabled("Haze, core glow, hue variation and stars live in the Glow group below.");
        }

        if (ImGui.CollapsingHeader("Particles##upart"))
        {
            Combo("Type", "##univpart", UiParts, cfg.UnivParticle, v => cfg.UnivParticle = v);
            if (cfg.UnivParticle > 0)
                ImGui.TextDisabled("Count / size come from Star count + Star size in the Glow group.");
        }

        var uac = ColorPick("Accent color", new Vector3(cfg.BgCol4R, cfg.BgCol4G, cfg.BgCol4B), new Vector3(Defaults.BgCol4R, Defaults.BgCol4G, Defaults.BgCol4B));
        cfg.BgCol4R = uac.X; cfg.BgCol4G = uac.Y; cfg.BgCol4B = uac.Z;
    }

    private void Combo(string label, string id, string[] items, int cur, Action<int> set)
    {
        int v = Math.Clamp(cur, 0, items.Length - 1);
        ImGui.TextUnformatted(label); ImGui.SameLine(120f); ImGui.PushItemWidth(-1f);
        if (ImGui.BeginCombo(id, items[v]))
        {
            for (int i = 0; i < items.Length; i++)
                if (ImGui.Selectable(items[i], v == i)) { set(i); _dirty = true; }
            ImGui.EndCombo();
        }
        ImGui.PopItemWidth();
    }

    private readonly Random _rng = new();
    private bool _lockPalette, _lockStructure;
    private Dictionary<string, object>? _rollUndo;

    private static bool IsRollField(System.Reflection.PropertyInfo p) =>
        p.CanRead && p.CanWrite && (p.PropertyType == typeof(float) || p.PropertyType == typeof(int))
        && (p.Name.StartsWith("Univ") || p.Name.StartsWith("Bg"));

    private void SnapshotRoll(PluginConfig cfg)
    {
        _rollUndo = new Dictionary<string, object>();
        foreach (var p in typeof(PluginConfig).GetProperties())
            if (IsRollField(p)) _rollUndo[p.Name] = p.GetValue(cfg)!;
    }

    private void RestoreRoll(PluginConfig cfg)
    {
        if (_rollUndo == null) return;
        foreach (var p in typeof(PluginConfig).GetProperties())
            if (IsRollField(p) && _rollUndo.TryGetValue(p.Name, out var v)) p.SetValue(cfg, v);
        _rollUndo = null;
    }

    private void Roll(PluginConfig cfg)
    {
        if (_lockPalette && _lockStructure) return;
        SnapshotRoll(cfg);
        if (!_lockStructure) RandomizeStructure(cfg);
        if (!_lockPalette) RandomizePalette(cfg);
    }

    private void RandomizePalette(PluginConfig cfg)
    {
        float baseHue = (float)_rng.NextDouble();
        float spread = 0.03f + (float)_rng.NextDouble() * 0.14f;
        float sat = 0.45f + (float)_rng.NextDouble() * 0.4f;
        Vector3 Ramp(int i)
        {
            float h = baseHue + (i - 2) * spread;
            float s = sat * (1f - i * 0.10f);
            float v = 0.06f + i / 4f * 0.9f;
            return Hsv(h, Math.Clamp(s, 0f, 1f), v);
        }
        var c1 = Ramp(0); cfg.BgTopR = c1.X; cfg.BgTopG = c1.Y; cfg.BgTopB = c1.Z;
        var c2 = Ramp(1); cfg.BgCol5R = c2.X; cfg.BgCol5G = c2.Y; cfg.BgCol5B = c2.Z;
        var c3 = Ramp(2); cfg.BgMidR = c3.X; cfg.BgMidG = c3.Y; cfg.BgMidB = c3.Z;
        var c4 = Ramp(3); cfg.BgCol6R = c4.X; cfg.BgCol6G = c4.Y; cfg.BgCol6B = c4.Z;
        var c5 = Ramp(4); cfg.BgBotR = c5.X; cfg.BgBotG = c5.Y; cfg.BgBotB = c5.Z;
        bool comp = _rng.NextDouble() < 0.4;
        var acc = Hsv(baseHue + (comp ? 0.5f : 0.12f), 0.7f, 0.95f);
        cfg.BgCol4R = acc.X; cfg.BgCol4G = acc.Y; cfg.BgCol4B = acc.Z;
    }

    private void RandomizeStructure(PluginConfig cfg)
    {
        float R(float a, float b) => a + (float)_rng.NextDouble() * (b - a);
        bool Chance(double p) => _rng.NextDouble() < p;

        cfg.UnivHorizon = 0f; cfg.UnivGround = 0; cfg.UnivOrb = 0; cfg.UnivRidges = 0f;
        cfg.UnivCaustic = 0f; cfg.UnivShafts = 0f; cfg.UnivParticle = 0; cfg.UnivPattern = 0;
        cfg.BgTwist = 0f; cfg.BgNebContrast = 0f; cfg.BgHaze = 0f; cfg.BgHueVar = 0f;
        cfg.BgScale = R(4f, 12f); cfg.BgScaleY = R(4f, 12f); cfg.BgAngle = R(0f, 3.14f);
        cfg.BgGlow = R(0.2f, 0.8f); cfg.BgStars = 0f; cfg.BgFlow = 0f;

        int arch = _rng.Next(6);
        switch (arch)
        {
            case 0:
                cfg.UnivBase = _rng.Next(2, 6); cfg.UnivNoise = Chance(0.5) ? 5 : 2; cfg.UnivBlend = 0;
                cfg.UnivNoiseAmt = R(0.5f, 1.3f); cfg.UnivNoiseScale = R(0.4f, 1.4f);
                cfg.UnivWarp = R(0.4f, 1.2f); cfg.UnivDetail = R(0.2f, 0.7f);
                cfg.BgNebContrast = R(0.2f, 0.6f); cfg.BgHueVar = R(0f, 0.4f);
                cfg.BgStars = Chance(0.6) ? R(0.3f, 0.8f) : 0f; cfg.BgFlow = R(0f, 0.4f);
                if (Chance(0.4)) { cfg.UnivOrb = 3; cfg.UnivOrbX = R(0.3f, 0.7f); cfg.UnivOrbY = R(0.2f, 0.5f); cfg.UnivOrbSize = R(0.08f, 0.2f); }
                break;
            case 1:
                cfg.UnivBase = 0; cfg.UnivNoise = 1; cfg.UnivBlend = 0; cfg.UnivNoiseAmt = R(0.1f, 0.4f); cfg.UnivNoiseScale = R(0.3f, 0.8f);
                cfg.UnivHorizon = R(0.45f, 0.7f); cfg.UnivGround = _rng.Next(1, 6);
                cfg.UnivOrb = Chance(0.7) ? 1 : 3; cfg.UnivOrbX = R(0.25f, 0.75f); cfg.UnivOrbY = cfg.UnivHorizon - R(0.05f, 0.2f); cfg.UnivOrbSize = R(0.06f, 0.22f);
                cfg.UnivRidges = Chance(0.6) ? R(0.4f, 1f) : 0f;
                cfg.BgNebContrast = R(0.2f, 0.5f);
                break;
            case 2:
                cfg.UnivBase = 1; cfg.UnivNoise = Chance(0.5) ? 1 : 0; cfg.UnivNoiseAmt = R(0.2f, 0.6f);
                cfg.UnivCaustic = R(0.5f, 1.3f); cfg.UnivShafts = R(0.4f, 1.1f);
                cfg.UnivOrb = 3; cfg.UnivOrbX = R(0.3f, 0.7f); cfg.UnivOrbY = R(0f, 0.25f); cfg.UnivOrbSize = R(0.1f, 0.25f);
                cfg.UnivParticle = Chance(0.5) ? 5 : 0; cfg.BgHaze = R(0.2f, 0.5f); cfg.BgFlow = R(0.1f, 0.4f);
                break;
            case 3:
                cfg.UnivBase = _rng.Next(0, 6); cfg.UnivNoise = Chance(0.5) ? _rng.Next(1, 12) : 0; cfg.UnivBlend = _rng.Next(0, 6);
                cfg.UnivNoiseAmt = R(0.2f, 1f); cfg.UnivPattern = _rng.Next(1, 29);
                cfg.UnivPatBlend = _rng.Next(0, 3); cfg.UnivPatStrength = R(0.3f, 0.9f);
                cfg.UnivWarp = Chance(0.5) ? R(0.2f, 1f) : 0f; cfg.BgSharp = R(0f, 0.6f);
                cfg.BgTwist = Chance(0.4) ? R(-0.6f, 0.6f) : 0f;
                break;
            case 4:
                cfg.UnivBase = Chance(0.5) ? 5 : 1; cfg.UnivNoise = 2; cfg.UnivBlend = 0;
                cfg.UnivNoiseAmt = R(0.4f, 1f); cfg.UnivNoiseScale = R(0.5f, 1.5f);
                cfg.BgNebContrast = R(0.4f, 0.8f); cfg.BgTwist = R(-0.8f, 0.8f);
                cfg.BgStars = R(0.4f, 0.9f); cfg.BgHueVar = R(0f, 0.3f);
                cfg.UnivOrb = Chance(0.5) ? 2 : 0; cfg.UnivOrbX = 0.5f; cfg.UnivOrbY = R(0.3f, 0.5f); cfg.UnivOrbSize = R(0.1f, 0.3f);
                break;
            default:
                cfg.UnivBase = _rng.Next(0, 3); cfg.UnivNoise = Chance(0.5) ? 1 : 0; cfg.UnivNoiseAmt = R(0.1f, 0.4f);
                cfg.UnivParticle = _rng.Next(1, 7); cfg.BgStars = Chance(0.4) ? R(0.2f, 0.5f) : 0f;
                cfg.BgHaze = R(0.1f, 0.4f); cfg.BgGlow = R(0.4f, 1f);
                break;
        }
    }

    private static Vector3 Hsv(float h, float s, float v)
    {
        h = h - (float)Math.Floor(h);
        float i = h * 6f, f = i - (float)Math.Floor(i);
        float p = v * (1f - s), q = v * (1f - s * f), t = v * (1f - s * (1f - f));
        switch ((int)i % 6)
        {
            case 0: return new Vector3(v, t, p);
            case 1: return new Vector3(q, v, p);
            case 2: return new Vector3(p, v, t);
            case 3: return new Vector3(p, q, v);
            case 4: return new Vector3(t, p, v);
            default: return new Vector3(v, p, q);
        }
    }

    private void DrawSecondLayer(PluginConfig cfg)
    {
        ImGui.Spacing();
        ImGui.Separator();
        bool on = cfg.BgBStyle > 0;
        if (ImGui.Checkbox("Second field (combine)", ref on))
        {
            cfg.BgBStyle = on ? 27 : 0;
            if (on && cfg.BlendMix == 0) { cfg.BlendMix = 1; cfg.BlendMode = 3; }
            _dirty = true;
        }
        if (!on)
        {
            ImGui.TextDisabled("Adds a second procedural field mixed into this one background —\nsame engine, same controls, no themes.");
            return;
        }

        using var id = ImRaii.PushId("uni2");

        if (ImGui.Button("Copy first field \u2192 here"))
        {
            cfg.SaveBgBFrom(cfg);
            _dirty = true;
            _status = "Copied the first field into the second.";
        }
        if (ImGui.IsItemHovered()) ImGui.SetTooltip(
            "Overwrite this field with the first one, then adjust from there.\nUndo puts it back.");
        ImGui.SameLine();
        if (ImGui.Button("Copy here \u2192 first field"))
        {
            cfg.LoadBgBInto(cfg);
            _dirty = true;
            _status = "Copied the second field into the first.";
        }
        if (ImGui.IsItemHovered()) ImGui.SetTooltip(
            "Overwrite the FIRST field with this one. Undo puts it back.");
        ImGui.Spacing();

        cfg.LoadBgBInto(_scratch);
        _scratch.BgStyle = 27;

        ImGui.TextDisabled("The second field — its own colours, engine and placement.");
        var s1 = ColorPick("Color 1", new Vector3(_scratch.BgTopR, _scratch.BgTopG, _scratch.BgTopB), new Vector3(Defaults.BgTopR, Defaults.BgTopG, Defaults.BgTopB));
        _scratch.BgTopR = s1.X; _scratch.BgTopG = s1.Y; _scratch.BgTopB = s1.Z;
        var s2 = ColorPick("Color 2", new Vector3(_scratch.BgCol5R, _scratch.BgCol5G, _scratch.BgCol5B), new Vector3(Defaults.BgCol5R, Defaults.BgCol5G, Defaults.BgCol5B));
        _scratch.BgCol5R = s2.X; _scratch.BgCol5G = s2.Y; _scratch.BgCol5B = s2.Z;
        var s3 = ColorPick("Color 3", new Vector3(_scratch.BgMidR, _scratch.BgMidG, _scratch.BgMidB), new Vector3(Defaults.BgMidR, Defaults.BgMidG, Defaults.BgMidB));
        _scratch.BgMidR = s3.X; _scratch.BgMidG = s3.Y; _scratch.BgMidB = s3.Z;
        var s4 = ColorPick("Color 4", new Vector3(_scratch.BgCol6R, _scratch.BgCol6G, _scratch.BgCol6B), new Vector3(Defaults.BgCol6R, Defaults.BgCol6G, Defaults.BgCol6B));
        _scratch.BgCol6R = s4.X; _scratch.BgCol6G = s4.Y; _scratch.BgCol6B = s4.Z;
        var s5 = ColorPick("Color 5", new Vector3(_scratch.BgBotR, _scratch.BgBotG, _scratch.BgBotB), new Vector3(Defaults.BgBotR, Defaults.BgBotG, Defaults.BgBotB));
        _scratch.BgBotR = s5.X; _scratch.BgBotG = s5.Y; _scratch.BgBotB = s5.Z;

        DrawUniversalControls(_scratch);

        _scratch.BgScale = Knob("Scale", _scratch.BgScale, 1f, 40f, Defaults.BgScale, "Pattern density of the second field.", "%.0f");
        _scratch.BgScaleY = Knob("Scale Y", _scratch.BgScaleY, 1f, 40f, Defaults.BgScaleY, "Pattern size of the second field.", "%.0f");
        _scratch.BgAngle = Knob("Angle", _scratch.BgAngle, 0f, 3.14f, Defaults.BgAngle, "Rotation of the second field.");
        _scratch.BgSharp = Knob("Edge hardness", _scratch.BgSharp, 0f, 1f, Defaults.BgSharp, "Soft gradient (0) -> hard band (1).");
        _scratch.BgFbm = Knob("Detail", _scratch.BgFbm, 1f, 6f, Defaults.BgFbm, "Fractal octaves of the second field.", "%.0f");

        cfg.SaveBgBFrom(_scratch);

        ImGui.Spacing();
        using (ImRaii.PushId("seam")) DrawBlendControls(cfg);
    }

    private void DrawBlendControls(PluginConfig cfg)
    {
        var mixes = UiMixes;
        int mx = Math.Clamp(cfg.BlendMix, 0, mixes.Length - 1);
        ImGui.TextUnformatted("Combine"); ImGui.SameLine(110f); ImGui.PushItemWidth(-1f);
        if (ImGui.BeginCombo("##blendmix", mixes[mx]))
        {
            for (int i = 0; i < mixes.Length; i++)
                if (ImGui.Selectable(mixes[i], mx == i))
                {
                    cfg.BlendMix = i;
                    if (i > 0 && cfg.BlendMode != 3) cfg.BlendMode = 3;
                    else if (i == 0 && cfg.BlendMode == 3) cfg.BlendMode = 0;
                    _dirty = true;
                }
            ImGui.EndCombo();
        }
        ImGui.PopItemWidth();
        if (cfg.BlendMix == 1 || cfg.BlendMix == 2 || cfg.BlendMix == 5 || cfg.BlendMix == 6)
            cfg.BlendMixLevel = Knob("Level", cfg.BlendMixLevel, 0f, 1f, Defaults.BlendMixLevel,
                "Threshold the combine keys off — for 'where B is bright', how bright a pixel must be before B shows. Feather controls how soft that cutoff is.");
        ImGui.TextDisabled(cfg.BlendMix == 0
            ? "Splits the frame along the seam below."
            : "Blends in one shared space; the seam below still confines it to a region.");
        ImGui.Separator();

        var modes = UiModes;
        int md = Math.Clamp(cfg.BlendMode, 0, modes.Length - 1);
        ImGui.TextUnformatted("Seam"); ImGui.SameLine(110f); ImGui.PushItemWidth(-1f);
        if (ImGui.BeginCombo("##blendmode", modes[md]))
        {
            for (int i = 0; i < modes.Length; i++)
                if (ImGui.Selectable(modes[i], md == i)) { cfg.BlendMode = i; _dirty = true; }
            ImGui.EndCombo();
        }
        ImGui.PopItemWidth();

        if (cfg.BlendMode == 0)
        {
            cfg.BlendAngle = Knob("Angle", cfg.BlendAngle, 0f, 3.14f, Defaults.BlendAngle, "Orientation of the split. 0 = a left/right seam; ~1.57 = top/bottom.");
            cfg.BlendOffset = Knob("Position", cfg.BlendOffset, -0.7f, 0.7f, Defaults.BlendOffset, "Slide the seam across the frame (0 = centred).");
        }
        else if (cfg.BlendMode == 1)
        {
            cfg.BlendCx = Knob("Center X", cfg.BlendCx, 0f, 1f, Defaults.BlendCx, "Centre of B's region.");
            cfg.BlendCy = Knob("Center Y", cfg.BlendCy, 0f, 1f, Defaults.BlendCy, "Centre of B's region. 0 = bottom of frame, 1 = top.");
            cfg.BlendRadius = Knob("Radius", cfg.BlendRadius, 0f, 1f, Defaults.BlendRadius, "Size of B's region.");
            cfg.BlendEllipse = Knob("Oval", cfg.BlendEllipse, 0.2f, 3f, Defaults.BlendEllipse, "Vertical squash (1 = a circle).");
        }
        else
        {
            cfg.BlendDepthSplit = Knob("Depth split", cfg.BlendDepthSplit, 0f, 1f, Defaults.BlendDepthSplit, "B fills the scene past this depth; A fills what is nearer. Needs depth.");
        }

        cfg.BlendFeather = Knob("Feather", cfg.BlendFeather, 0.001f, 0.3f, Defaults.BlendFeather, "Half-width of the soft transition band (0 = a hard cut).");
        cfg.BlendNoiseAmt = Knob("Ragged edge", cfg.BlendNoiseAmt, 0f, 1f, Defaults.BlendNoiseAmt, "Perturb the seam with noise: a low Scale gives a wandering organic edge, a high Scale a granular dissolve.");
        if (cfg.BlendNoiseAmt > 0f)
            cfg.BlendNoiseScale = Knob("Ragged scale", cfg.BlendNoiseScale, 0.5f, 24f, Defaults.BlendNoiseScale, "Frequency of the seam noise: low = organic waves, high = fine dissolve.", "%.1f");
        cfg.BlendMatch = Knob("Brightness match", cfg.BlendMatch, 0f, 1f, Defaults.BlendMatch, "Ease a brightness step at the seam by nudging both sides toward each other — useful when the two styles have very different exposure.");
        cfg.BlendDepthBend = Knob("Depth bend", cfg.BlendDepthBend, -1f, 1f, Defaults.BlendDepthBend, "Let scene depth push the seam, so it reads as being in the world rather than on the lens. Needs depth.");
        if (cfg.BlendDepthBend != 0f) cfg.BlendDepthRef = Knob("Depth neutral", cfg.BlendDepthRef, 0f, 1f, Defaults.BlendDepthRef, "The depth the bend treats as flat — nearer bends one way, farther the other.");
    }

    private void DrawBgStyleGroup(PluginConfig cfg)
    {
            var styles = UiStyles;
            int st = cfg.BgStyle < 0 || cfg.BgStyle >= styles.Length ? 0 : cfg.BgStyle;
            var shown = UiShown;
            ImGui.TextUnformatted("Style"); ImGui.SameLine(110f);
            ImGui.PushItemWidth(-1f);
            if (ImGui.BeginCombo("##bgstyle", styles[st]))
            {
                bool inShown = Array.IndexOf(shown, st) >= 0;
                if (!inShown) { ImGui.Selectable(styles[st] + "  (legacy — use Custom)", true); ImGui.Separator(); }
                foreach (int s in shown)
                    if (ImGui.Selectable(styles[s], st == s)) { cfg.BgStyle = s; _dirty = true; }
                ImGui.EndCombo();
            }
            ImGui.PopItemWidth();
            ImGui.TextUnformatted("Preset"); ImGui.SameLine(110f);
            ImGui.PushItemWidth(-1f);
            if (ImGui.BeginCombo("##preset", "Load a theme…"))
            {
                foreach (var (name, _cat, _) in LookStore.Builtins)
                {
                    if (ImGui.Selectable(name))
                    {
                        LookStore.SeedBuiltins();
                        var keepB = new PluginConfig();
                        cfg.CopyBTo(keepB);
                        LookStore.Load(name, cfg);
                        cfg.CopyBFrom(keepB);
                        st = cfg.BgStyle; _dirty = true;
                    }
                }
                ImGui.EndCombo();
            }
            ImGui.PopItemWidth();
            var caps = BgCaps(st);
            {
                if (ReferenceEquals(cfg, Plugin.Config))
                {
                cfg.BgRecolor = Knob("Strength", cfg.BgRecolor, 0f, 1f, Defaults.BgRecolor, "How strongly the background style is applied (1 = fully replaces the background).");
                cfg.BgRecolorStart = Knob("Start (depth)", cfg.BgRecolorStart, 0f, 0.5f, Defaults.BgRecolorStart, "How far out the background begins. Lower it to catch a wall or object just behind the subject.");
                GateToggle(cfg, "backdrop");
                cfg.BgRecolorFeather = Knob("Cutoff softness", cfg.BgRecolorFeather, 0.003f, 0.3f, Defaults.BgRecolorFeather, "Width of the depth transition. LOW = a hard cut right behind the subject, so a wall/object just behind it still gets replaced without bleeding onto the subject. HIGH = a soft fade. To cover walls: lower Start and lower this together.", "%.3f");
                cfg.BgKeepVfx = Knob("Keep VFX", cfg.BgKeepVfx, 0f, 1f, Defaults.BgKeepVfx, "Keeps glowing effect particles that float AROUND the subject (they don't write depth, so the background would otherwise paint over them). 0 = off.");
                }
                else ImGui.TextDisabled("Strength, depth gate and Keep VFX are set once for the\nwhole backdrop, on the first field.");
                ImGui.Spacing();
                ImGui.TextDisabled("Key light on the backdrop — a real studio backdrop is never flat.");
                cfg.BackdropLightAmt = Knob("Backdrop light", cfg.BackdropLightAmt, -1f, 1f, Defaults.BackdropLightAmt, "Lights the backdrop with a hotspot that falls off outward, so it looks lit by the same lamp as your subject. Negative flips it (bright edges, dark centre).");
                if (cfg.BackdropLightAmt != 0f)
                {
                    cfg.BackdropLightX = Knob("  Light X", cfg.BackdropLightX, 0f, 1f, Defaults.BackdropLightX, "Hotspot position — put it where your key light is.");
                    cfg.BackdropLightY = Knob("  Light Y", cfg.BackdropLightY, 0f, 1f, Defaults.BackdropLightY, "Hotspot height. 0 = bottom of frame, 1 = top.");
                    cfg.BackdropLightSize = Knob("  Spread", cfg.BackdropLightSize, 0.1f, 1.5f, Defaults.BackdropLightSize, "How broad the pool of light on the backdrop is.");
                }
                cfg.AnimSpeed = Knob("Animation speed", cfg.AnimSpeed, 0f, 1f, Defaults.AnimSpeed, "Drifts motes / clouds / water / sparks over time. 0 = a static still (no cost). Turn up for video / GIF capture.");
            }
            var b1 = ColorPick("Color 1", new Vector3(cfg.BgTopR, cfg.BgTopG, cfg.BgTopB), new Vector3(Defaults.BgTopR, Defaults.BgTopG, Defaults.BgTopB));
            cfg.BgTopR = b1.X; cfg.BgTopG = b1.Y; cfg.BgTopB = b1.Z;
            if (caps.ramp)
            {
                var r2 = ColorPick("Color 2", new Vector3(cfg.BgCol5R, cfg.BgCol5G, cfg.BgCol5B), new Vector3(Defaults.BgCol5R, Defaults.BgCol5G, Defaults.BgCol5B));
                cfg.BgCol5R = r2.X; cfg.BgCol5G = r2.Y; cfg.BgCol5B = r2.Z;
                var r3 = ColorPick("Color 3", new Vector3(cfg.BgMidR, cfg.BgMidG, cfg.BgMidB), new Vector3(Defaults.BgMidR, Defaults.BgMidG, Defaults.BgMidB));
                cfg.BgMidR = r3.X; cfg.BgMidG = r3.Y; cfg.BgMidB = r3.Z;
                var r4 = ColorPick("Color 4", new Vector3(cfg.BgCol6R, cfg.BgCol6G, cfg.BgCol6B), new Vector3(Defaults.BgCol6R, Defaults.BgCol6G, Defaults.BgCol6B));
                cfg.BgCol6R = r4.X; cfg.BgCol6G = r4.Y; cfg.BgCol6B = r4.Z;
                var r5 = ColorPick("Color 5", new Vector3(cfg.BgBotR, cfg.BgBotG, cfg.BgBotB), new Vector3(Defaults.BgBotR, Defaults.BgBotG, Defaults.BgBotB));
                cfg.BgBotR = r5.X; cfg.BgBotG = r5.Y; cfg.BgBotB = r5.Z;
                ImGui.TextDisabled(st == 18
                    ? "Color 1 = night sky; Colors 2-5 = aurora light (green -> pink)."
                    : st == 19
                    ? "Colors 1-5 = sky (top -> horizon). Neon grid uses the glow color; Scale = grid density, Scale Y = sun size."
                    : st == 20
                    ? "Colors 1-5 = sky (top -> horizon); glow color = the moon. Scale = clouds, Scale Y = moon size, Offset = moon position."
                    : st == 21
                    ? "Tempe's variant: Colors 1-5 = the crimson abyss; glow color = the moon's red iris (the relic). Her teal aether is baked in. Scale = clouds, Scale Y = moon size, Offset = moon position. Floor + reflection are in the Ground group."
                    : st == 22
                    ? "Forge: Colors 1-5 = sooty ambient (top -> ember low); glow color = the sparks. Scale = spark density, Scale Y = furnace size, Offset = furnace position. Molten trough sits at the Ground level."
                    : st == 23
                    ? "Artisan's Rest: Colors 1-5 = the dusk sky; glow color = the hearth warmth. Bokeh lights are the 8 craft-class colors. Scale = bokeh density, Offset = glow position."
                    : st == 24
                    ? "Sunset: Colors 1-5 = the sky (top -> horizon); glow color = the sun. Scale = cloud scale, Scale Y = horizon height, Offset X = sun position, Core glow = bloom."
                    : st == 25
                    ? "Sin Eater: Colors 1-5 = the cold void; glow color = the Light. Background only (VFX-safe). Scale = crystal density, Scale Y = halo size, Offset = the Light's position."
                    : st == 28
                    ? "Evercold: Colors 1-5 = the cold sky (top -> icy horizon); glow color = the cold light / ice glow. Scale = frost density, Scale Y = snow horizon height, Offset X = cold sun position. Snow drifts with Animation speed."
                    : "5-colour gradient: Color 1 (dark) -> Color 5 (bright).");
            }
            else
            {
                if (caps.c2)
                {
                    var b2 = ColorPick("Color 2", new Vector3(cfg.BgBotR, cfg.BgBotG, cfg.BgBotB), new Vector3(Defaults.BgBotR, Defaults.BgBotG, Defaults.BgBotB));
                    cfg.BgBotR = b2.X; cfg.BgBotG = b2.Y; cfg.BgBotB = b2.Z;
                }
                if (caps.c3)
                {
                    var b3 = ColorPick("Color 3", new Vector3(cfg.BgMidR, cfg.BgMidG, cfg.BgMidB), new Vector3(Defaults.BgMidR, Defaults.BgMidG, Defaults.BgMidB));
                    cfg.BgMidR = b3.X; cfg.BgMidG = b3.Y; cfg.BgMidB = b3.Z;
                    ImGui.TextDisabled($"Color 3 = {caps.c3role} for this style.");
                }
            }
            if (caps.scaleX)
                cfg.BgScale = Knob(caps.scaleY ? "Scale X" : "Scale", cfg.BgScale, 1f, 40f, Defaults.BgScale, "Pattern density (higher = finer / more repeats).", "%.0f");
            if (caps.scaleY)
                cfg.BgScaleY = Knob("Scale Y", cfg.BgScaleY, 1f, 40f, Defaults.BgScaleY, "Vertical density — set different from Scale X for stretched checker / oval dots / a rectangular grid.", "%.0f");
            if (caps.angle)
                cfg.BgAngle = Knob("Pattern angle", cfg.BgAngle, 0f, 3.14f, Defaults.BgAngle, "Rotation / phase of the pattern.");
            if (caps.hardness)
                cfg.BgSharp = Knob("Edge hardness", cfg.BgSharp, 0f, 1f, Defaults.BgSharp, "Soft gradient (0) -> hard band (1).");
            if (caps.offset)
            {
                cfg.BgOffX = Knob("Offset X", cfg.BgOffX, -1f, 1f, Defaults.BgOffX, "Pan the whole pattern horizontally (moves gradient / radial / spiral centre).");
                cfg.BgOffY = Knob("Offset Y", cfg.BgOffY, -1f, 1f, Defaults.BgOffY, "Pan the whole pattern vertically.");
            }
            cfg.BgGrain = Knob("Background grain", cfg.BgGrain, 0f, 1f, Defaults.BgGrain, "Film grain on the background only.");

            if (st == 26)
            {
                var grad = UiGrad;
                int gt = cfg.BgGradType < 0 || cfg.BgGradType >= grad.Length ? 0 : cfg.BgGradType;
                ImGui.TextUnformatted("Gradient"); ImGui.SameLine(110f); ImGui.PushItemWidth(-1f);
                if (ImGui.BeginCombo("##gradtype", grad[gt])) { for (int i = 0; i < grad.Length; i++) if (ImGui.Selectable(grad[i], gt == i)) { cfg.BgGradType = i; _dirty = true; } ImGui.EndCombo(); }
                ImGui.PopItemWidth();
                var pats = UiPats;
                int pm = cfg.BgPatMode < 0 || cfg.BgPatMode >= pats.Length ? 0 : cfg.BgPatMode;
                ImGui.TextUnformatted("Pattern"); ImGui.SameLine(110f); ImGui.PushItemWidth(-1f);
                if (ImGui.BeginCombo("##patmode", pats[pm])) { for (int i = 0; i < pats.Length; i++) if (ImGui.Selectable(pats[i], pm == i)) { cfg.BgPatMode = i; _dirty = true; } ImGui.EndCombo(); }
                ImGui.PopItemWidth();
                if (cfg.BgPatMode > 0)
                {
                    cfg.BgPatStrength = Knob("Pattern strength", cfg.BgPatStrength, 0f, 1f, 0.5f, "How strongly the pattern shows over the gradient.");
                    cfg.BgPatAngle = Knob("Pattern rotation", cfg.BgPatAngle, -3.14f, 3.14f, 0f, "Rotate the pattern.");
                    var pc = ColorPick("Pattern color", new Vector3(cfg.BgCol4R, cfg.BgCol4G, cfg.BgCol4B), new Vector3(1f, 1f, 1f));
                    cfg.BgCol4R = pc.X; cfg.BgCol4G = pc.Y; cfg.BgCol4B = pc.Z;
                }
                cfg.BgNebContrast = Knob("Noise clouds", cfg.BgNebContrast, 0f, 1f, Defaults.BgNebContrast, "Fractal-noise overlay over the gradient.");
                if (cfg.BgNebContrast > 0f) cfg.BgNebWarp = Knob("Noise warp", cfg.BgNebWarp, 0f, 1f, Defaults.BgNebWarp, "Swirl the noise.");
                ImGui.TextDisabled("Scale = pattern density, Scale Y = pattern size, Angle = gradient direction,\nHardness = gradient edge. Stars/vignette/grain (glow group) apply too.");
            }

            if (st == 27)
            {
                DrawUniversalControls(cfg);
                DrawSecondLayer(cfg);
            }

            bool isNebulaOrNoise = st == 13 || st == 14 || st == 16;
            bool isNebulaFamily = st == 14 || st == 16;

            ImGui.Spacing();
            if (ImGui.CollapsingHeader("Glow & atmosphere"))
            {
                var b4 = ColorPick("Star / glow color", new Vector3(cfg.BgCol4R, cfg.BgCol4G, cfg.BgCol4B), new Vector3(Defaults.BgCol4R, Defaults.BgCol4G, Defaults.BgCol4B));
                cfg.BgCol4R = b4.X; cfg.BgCol4G = b4.Y; cfg.BgCol4B = b4.Z;
                if (isNebulaFamily)
                {
                    cfg.BgFbm = Knob("Detail", cfg.BgFbm, 1f, 6f, Defaults.BgFbm, "Fractal octaves — more = finer, wispier structure.", "%.0f");
                    cfg.BgNebWarp = Knob("Aether tendrils", cfg.BgNebWarp, 0f, 1f, Defaults.BgNebWarp, "Domain-warps the clouds/arms into flowing, marbled turbulence.");
                    cfg.BgNebContrast = Knob("Filament contrast", cfg.BgNebContrast, 0f, 1f, Defaults.BgNebContrast, "Crushes toward black, leaving thin glowing filaments — the void look.");
                    cfg.BgFlow = Knob("Current flow", cfg.BgFlow, 0f, 1f, Defaults.BgFlow, "Stretches the structure into streaming directional currents.");
                    cfg.BgTwist = Knob(st == 16 ? "Churn" : "Vortex twist", cfg.BgTwist, -1f, 1f, Defaults.BgTwist, st == 16 ? "Swirls the aether around the subject (no hole/portal)." : "Spirals the whole field into the centre — a portal vortex.");
                    cfg.BgHaze = Knob("Depth haze", cfg.BgHaze, 0f, 1f, Defaults.BgHaze, "Soft low-frequency glow behind the structure for atmospheric depth.");
                }
                if (st == 18)
                {
                    cfg.BgFbm = Knob("Detail", cfg.BgFbm, 1f, 6f, Defaults.BgFbm, "Fractal detail of the curtains' motion.", "%.0f");
                    cfg.BgNebWarp = Knob("Curtain wave", cfg.BgNebWarp, 0f, 1f, Defaults.BgNebWarp, "How much the light curtains ripple and braid.");
                    cfg.BgTwist = Knob("Drift", cfg.BgTwist, -1f, 1f, Defaults.BgTwist, "Sideways shear/drift of the curtains.");
                    cfg.BgHueVar = Knob("Hue drift", cfg.BgHueVar, 0f, 1f, Defaults.BgHueVar, "Shimmering hue variation across the light.");
                }
                if (st == 20 || st == 21)
                {
                    cfg.BgFbm = Knob("Cloud detail", cfg.BgFbm, 1f, 6f, Defaults.BgFbm, "Fractal detail of the drifting clouds.", "%.0f");
                    cfg.BgNebWarp = Knob("Cloud wisps", cfg.BgNebWarp, 0f, 1f, Defaults.BgNebWarp, "How torn / wispy the clouds are.");
                    cfg.BgNebContrast = Knob("Cloud density", cfg.BgNebContrast, 0f, 1f, Defaults.BgNebContrast, "How much of the sky the clouds cover.");
                    cfg.BgTwist = Knob("Cloud drift", cfg.BgTwist, -1f, 1f, Defaults.BgTwist, "Sideways drift of the clouds.");
                }
                if (st == 25)
                {
                    ImGui.TextDisabled("Background only — leaves the subject & its VFX untouched.");
                    cfg.BgNebContrast = Knob("Colour drain", cfg.BgNebContrast, 0f, 1f, Defaults.BgNebContrast, "Drains the colour into cold shadow — higher = colder / greyer.");
                    cfg.BgHaze = Knob("Grief mist", cfg.BgHaze, 0f, 1f, Defaults.BgHaze, "Cold soul-wisps drifting through the dark.");
                    cfg.BgNebWarp = Knob("Crystal jaggedness", cfg.BgNebWarp, 0f, 1f, Defaults.BgNebWarp, "How jagged / warped the crystalline fractures & light-dendrites are.");
                    cfg.BgDisperse = Knob("Halo prism", cfg.BgDisperse, 0f, 1f, Defaults.BgDisperse, "Chromatic split of the eclipse ring (cold prismatic sheen).");
                    cfg.BgSparkle = Knob("Light feathers", cfg.BgSparkle, 0f, 1f, Defaults.BgSparkle, "Drifting soft light-feathers that catch the god-rays.");
                    cfg.BgEmbers = Knob("Grief motes", cfg.BgEmbers, 0f, 1f, Defaults.BgEmbers, "Drifting soul-blue flecks (glint in the light).");
                    cfg.BgEmberSize = Knob("Mote size", cfg.BgEmberSize, 0f, 1f, Defaults.BgEmberSize, "Size of the drifting flecks.");
                    cfg.BgFlow = Knob("Fall speed", cfg.BgFlow, -1f, 1f, Defaults.BgFlow, "How fast the motes & feathers fall / drift.");
                    cfg.BgGrain = Knob("Cold grain", cfg.BgGrain, 0f, 1f, Defaults.BgGrain, "Cold film grain over the void.");
                    ImGui.TextDisabled("Scale = crystal density, Scale Y = halo size, Offset = the Light.");
                }
                if (st == 24)
                {
                    cfg.BgNebContrast = Knob("Cloud density", cfg.BgNebContrast, 0f, 1f, Defaults.BgNebContrast, "How much cloud streaks the sky.");
                    cfg.BgNebWarp = Knob("Cloud wisps", cfg.BgNebWarp, 0f, 1f, Defaults.BgNebWarp, "How torn / wispy the clouds are.");
                    cfg.BgTwist = Knob("Cloud drift", cfg.BgTwist, -1f, 1f, Defaults.BgTwist, "Sideways drift of the clouds.");
                    cfg.BgFlow = Knob("Water shimmer", cfg.BgFlow, -1f, 1f, Defaults.BgFlow, "Animates the sun-glitter on the water.");
                    ImGui.TextDisabled("Scale Y = horizon height, Offset X = sun position.");
                }
                if (st == 23)
                {
                    ImGui.TextDisabled("Soft bokeh in the 8 crafting-class colors, drifting over a calm dusk.");
                    cfg.BgTwist = Knob("Drift X", cfg.BgTwist, -1f, 1f, Defaults.BgTwist, "Sideways drift of the bokeh lights.");
                    cfg.BgFlow = Knob("Drift Y", cfg.BgFlow, -1f, 1f, Defaults.BgFlow, "Vertical drift of the bokeh lights.");
                }
                if (st == 22)
                {
                    cfg.BgEmbers = Knob("Sparks", cfg.BgEmbers, 0f, 1f, Defaults.BgEmbers, "Rising, twinkling forge sparks (size = Ember size below).");
                    cfg.BgEmberSize = Knob("Spark size", cfg.BgEmberSize, 0f, 1f, Defaults.BgEmberSize, "Size of the sparks.");
                    cfg.BgNebWarp = Knob("Flame turbulence", cfg.BgNebWarp, 0f, 1f, Defaults.BgNebWarp, "How much the furnace flames writhe.");
                    cfg.BgFlow = Knob("Flow", cfg.BgFlow, -1f, 1f, Defaults.BgFlow, "Drift of the flames, sparks and molten metal.");
                    cfg.BgTwist = Knob("Spark drift", cfg.BgTwist, -1f, 1f, Defaults.BgTwist, "Sideways lean of the rising sparks.");
                    cfg.BgHaze = Knob("Heat shimmer", cfg.BgHaze, 0f, 1f, Defaults.BgHaze, "Rippling heat-haze distortion above the forge.");
                    ImGui.TextDisabled("Molten trough height = 'Ground level' (Ground group below).");
                }
                if (st == 21)
                    ImGui.TextDisabled("Floor height + her reflection live in the 'Ground (fake floor)' group below.");
                if (isNebulaOrNoise)
                    cfg.BgHueVar = Knob("Hue variation", cfg.BgHueVar, 0f, 1f, Defaults.BgHueVar, "Drifts the hue across the clouds for richer, multi-toned colour.");
                cfg.BgStars = Knob("Stars", cfg.BgStars, 0f, 1f, Defaults.BgStars, "Procedural starfield over the background (works on any style).");
                if (cfg.BgStars > 0f || st == 15)
                {
                    cfg.BgStarDensity = Knob("Star count", cfg.BgStarDensity, 8f, 120f, Defaults.BgStarDensity, "How many stars.", "%.0f");
                    cfg.BgStarSize = Knob("Star size", cfg.BgStarSize, 0f, 1f, Defaults.BgStarSize, "Star / sparkle size.");
                    if (st != 25) cfg.BgSparkle = Knob("Sparkle spikes", cfg.BgSparkle, 0f, 1f, Defaults.BgSparkle, "Diffraction cross-spikes on the brightest stars.");
                }
            if (st != 22 && st != 25)
            {
            cfg.BgEmbers = Knob("Embers / motes", cfg.BgEmbers, 0f, 1f, Defaults.BgEmbers, "Drifting dust motes catching the light — atmosphere & life.");
                if (cfg.BgEmbers > 0f)
                    cfg.BgEmberSize = Knob("Ember size", cfg.BgEmberSize, 0f, 1f, Defaults.BgEmberSize, "Size of the soft motes.");
                }
            cfg.BgGlow = Knob("Core glow", cfg.BgGlow, 0f, 2f, Defaults.BgGlow, "Makes the bright wisps and stars self-illuminate (fake bloom).");
                cfg.BgVignette = Knob("Center glow / falloff", cfg.BgVignette, 0f, 1f, Defaults.BgVignette, "Bright centre fading to dark edges, like a spotlight behind the subject.");
                cfg.BgVoidCore = Knob("Void core (dark)", cfg.BgVoidCore, 0f, 1f, Defaults.BgVoidCore, "A dark absorbing centre that swallows light — the opposite of the centre glow.");
                cfg.BgVoidRing = Knob("Void ring", cfg.BgVoidRing, 0f, 1f, Defaults.BgVoidRing, "A glowing accretion ring around the void core (uses the star / glow colour).");
                if (cfg.BgVoidCore > 0f || cfg.BgVoidRing > 0f || cfg.BgRing2 > 0f)
                {
                    cfg.BgRingWidth = Knob("Ring width", cfg.BgRingWidth, 0.2f, 3f, Defaults.BgRingWidth, "Thickness of the accretion ring.");
                    cfg.BgDisperse = Knob("Ring dispersion", cfg.BgDisperse, 0f, 1f, Defaults.BgDisperse, "Chromatic rainbow fringing across the ring — cinematic.");
                    cfg.BgRing2 = Knob("Outer halo ring", cfg.BgRing2, 0f, 1f, Defaults.BgRing2, "A faint second ring further out, for depth.");
                }
                if (cfg.BgVignette > 0f || cfg.BgVoidCore > 0f || cfg.BgVoidRing > 0f || cfg.BgRing2 > 0f)
                    cfg.BgVignetteSize = Knob("Center / core radius", cfg.BgVignetteSize, 0.15f, 1.5f, Defaults.BgVignetteSize, "Radius of the centre glow, void core and rings.");
                cfg.BgBright = Knob("Brightness", cfg.BgBright, -0.9f, 2f, Defaults.BgBright, "Overall background exposure.");

                bool aquatic = st == 17 || cfg.BgCausticAmt > 0f || cfg.BgShafts > 0f || cfg.BgBubbles > 0f;
                if (aquatic)
                {
                    ImGui.Spacing();
                    ImGui.TextUnformatted("Underwater");
                    cfg.BgCausticAmt = Knob("Caustics", cfg.BgCausticAmt, 0f, 2f, Defaults.BgCausticAmt, "Rippling refracted-light web (scale = Scale, ripple = Aether tendrils).");
                    cfg.BgShafts = Knob("Light shafts", cfg.BgShafts, 0f, 2f, Defaults.BgShafts, "God-ray beams descending from the surface.");
                    cfg.BgBubbles = Knob("Bubbles", cfg.BgBubbles, 0f, 1f, Defaults.BgBubbles, "Rising bubbles (size = Ember size).");
                }
            }

            ImGui.Spacing();
            if (ImGui.CollapsingHeader("Warps (stack any combination)"))
            {
                var warps = UiWarps;
                foreach (var (label, bit) in warps)
                {
                    bool on = (cfg.BgWarp & bit) != 0;
                    if (ImGui.Checkbox(label, ref on))
                    {
                        cfg.BgWarp = on ? (cfg.BgWarp | bit) : (cfg.BgWarp & ~bit);
                        _dirty = true;
                    }
                }
                if ((cfg.BgWarp & (1 | 2 | 4)) != 0)
                {
                    cfg.BgWarpAmt = Knob("Swirl/bulge amount", cfg.BgWarpAmt, 0f, 2f, Defaults.BgWarpAmt, "Strength of the swirl and bulge/pinch warps.");
                    cfg.BgWarpScale = Knob("Kaleido segments", cfg.BgWarpScale, 2f, 24f, Defaults.BgWarpScale, "Number of mirrored kaleidoscope segments.", "%.0f");
                }
                if ((cfg.BgWarp & (8 | 16)) != 0)
                {
                    cfg.BgWarpAmt2 = Knob("Wave/ripple amount", cfg.BgWarpAmt2, 0f, 2f, Defaults.BgWarpAmt2, "Strength of the wave and ripple warps.");
                    cfg.BgWarpScale2 = Knob("Wave/ripple frequency", cfg.BgWarpScale2, 1f, 24f, Defaults.BgWarpScale2, "How many wave / ripple cycles across the screen.", "%.0f");
                }
                if (cfg.BgWarp != 0)
                {
                    cfg.BgWarpX = Knob("Warp centre X", cfg.BgWarpX, 0f, 1f, Defaults.BgWarpX, "Pivot of swirl / bulge / kaleido / ripple.");
                    cfg.BgWarpY = Knob("Warp centre Y", cfg.BgWarpY, 0f, 1f, Defaults.BgWarpY, "Pivot of swirl / bulge / kaleido / ripple.");
                }
            }

            ImGui.Spacing();
            if (ImGui.CollapsingHeader("Material (light the pattern like a surface)"))
            {
                cfg.BgNormal = Knob("Normal / bump", cfg.BgNormal, 0f, 1f, Defaults.BgNormal, "Embosses the pattern edges into 3D relief that catches the light.");
                cfg.BgSpecular = Knob("Specular", cfg.BgSpecular, 0f, 2f, Defaults.BgSpecular, "Glossy highlight intensity.");
                cfg.BgRoughness = Knob("Roughness", cfg.BgRoughness, 0f, 1f, Defaults.BgRoughness, "Tight sharp highlight (0) -> broad soft sheen (1).");
                cfg.BgMetallic = Knob("Metallic", cfg.BgMetallic, 0f, 1f, Defaults.BgMetallic, "0 = dielectric/glass (white highlight, faint reflection), 1 = metal (coloured reflection, dark base).");
                cfg.BgReflect = Knob("Reflection", cfg.BgReflect, 0f, 2f, Defaults.BgReflect, "Environment reflection — the surface mirrors a faux sky along its relief. This is what sells metal & glass.");
                if (cfg.BgReflect > 0f)
                {
                    cfg.BgEnvSharp = Knob("Reflection sharpness", cfg.BgEnvSharp, 0f, 1f, Defaults.BgEnvSharp, "Blurry sheen (0) -> tight mirror hotspot (1).");
                    var env = ColorPick("Reflected sky", new Vector3(cfg.BgEnvR, cfg.BgEnvG, cfg.BgEnvB), new Vector3(Defaults.BgEnvR, Defaults.BgEnvG, Defaults.BgEnvB));
                    cfg.BgEnvR = env.X; cfg.BgEnvG = env.Y; cfg.BgEnvB = env.Z;
                }
                cfg.BgClearcoat = Knob("Clearcoat", cfg.BgClearcoat, 0f, 2f, Defaults.BgClearcoat, "A second razor-sharp gloss highlight — wet glass / lacquer.");
                cfg.BgAniso = Knob("Brushed (anisotropy)", cfg.BgAniso, 0f, 1f, Defaults.BgAniso, "Stretches the highlight into a brushed-metal streak.");
                cfg.BgFresnel = Knob("Fresnel edge", cfg.BgFresnel, 0f, 2f, Defaults.BgFresnel, "Rim sheen on grazing angles / relief edges (glass rim).");
                if (cfg.BgFresnel > 0f)
                    cfg.BgMatDisp = Knob("Edge dispersion", cfg.BgMatDisp, 0f, 1f, Defaults.BgMatDisp, "Splits the fresnel rim into RGB — a prismatic glass edge.");
                cfg.BgLightInt = Knob("Diffuse shading", cfg.BgLightInt, 0f, 1f, Defaults.BgLightInt, "How much the light direction shades the base (0 = flat/evenly lit).");
                if (cfg.BgNormal > 0f || cfg.BgSpecular > 0f || cfg.BgMetallic > 0f || cfg.BgFresnel > 0f || cfg.BgLightInt > 0f || cfg.BgReflect > 0f || cfg.BgClearcoat > 0f)
                {
                    cfg.BgLightX = Knob("Light X", cfg.BgLightX, -1f, 1f, Defaults.BgLightX, "Light direction, left/right.");
                    cfg.BgLightY = Knob("Light Y", cfg.BgLightY, -1f, 1f, Defaults.BgLightY, "Light direction, up/down.");
                    cfg.BgLightZ = Knob("Light height", cfg.BgLightZ, 0.05f, 1f, Defaults.BgLightZ, "Light height above the surface (low = long raking highlights).");
                }
            }
    }

    private void DrawElementsGroup(PluginConfig cfg)
    {
        int S = PluginConfig.ElemStride;
        bool anyActive = false;
        for (int L = 0; L < 8; L++) if (cfg.Elem[L * S] >= 0.5f && Math.Abs(cfg.Elem[L * S + 11]) > 0f) { anyActive = true; break; }
        using var grp = GroupEn("Elements (layers)", anyActive, cfg.EnElements, v => cfg.EnElements = v);
        if (!grp.Show) return;

        ImGui.TextDisabled("8 stackable layers — a shape, a HUD part, or an image composited over the shot.");

        for (int L = 0; L < 8; L++)
        {
            if (L > 0) ImGui.SameLine(0f, 3f);
            int lt = (int)(cfg.Elem[L * S] + 0.5f);
            if (lt < 0 || lt >= UiTypes.Length) lt = 0;
            bool act = lt > 0 && Math.Abs(cfg.Elem[L * S + 11]) > 0f;
            bool sel = _elemSlot == L;
            ImGui.PushStyleColor(ImGuiCol.Button, sel ? new Vector4(0.35f, 0.5f, 0.75f, 1f)
                                : act ? new Vector4(0.26f, 0.30f, 0.34f, 1f)
                                      : new Vector4(0.20f, 0.20f, 0.22f, 1f));
            string tag = lt > 0 ? UiTypeTag[lt] : "\u00b7";
            if (ImGui.Button($"{L + 1} {tag}##eslot" + L, new Vector2(48f, 0))) _elemSlot = L;
            ImGui.PopStyleColor();
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(lt == 0 ? "Empty slot." : UiTypes[lt] + (act ? "" : "  (intensity is 0, so it draws nothing)"));
        }

        int b = _elemSlot * S;
        var types = UiTypes;
        int ty = (int)(cfg.Elem[b] + 0.5f); if (ty < 0 || ty >= types.Length) ty = 0;
        ImGui.TextUnformatted("Type"); ImGui.SameLine(90f);
        ImGui.PushItemWidth(-1f);
        if (ImGui.BeginCombo("##etype", types[ty]))
        {
            for (int t = 0; t < types.Length; t++)
                if (ImGui.Selectable(types[t], ty == t)) { cfg.Elem[b] = t; _dirty = true; }
            ImGui.EndCombo();
        }
        ImGui.PopItemWidth();
        if (ty == 0) return;

        if (ty == 18) DrawElementImage(cfg, _elemSlot);

        bool telOrImg = ty >= 14 && ty <= 18;
        cfg.Elem[b + 11] = Knob(telOrImg ? "Opacity" : "Intensity", cfg.Elem[b + 11], ty == 18 ? 0f : -2f, 2f, 1f, ty == 18 ? "Opacity." : "Brightness — negative subtracts (cut).");
        var ec = ColorPick(ty == 18 ? "Tint" : "Color", new Vector3(cfg.Elem[b + 8], cfg.Elem[b + 9], cfg.Elem[b + 10]), new Vector3(0.9f, 0.3f, 0.2f));
        cfg.Elem[b + 8] = ec.X; cfg.Elem[b + 9] = ec.Y; cfg.Elem[b + 10] = ec.Z;
        cfg.Elem[b + 1] = Knob("Position X", cfg.Elem[b + 1], -0.6f, 0.6f, 0f, "Horizontal position.");
        cfg.Elem[b + 2] = Knob("Position Y", cfg.Elem[b + 2], -0.6f, 0.6f, 0f, "Vertical position.");
        cfg.Elem[b + 3] = Knob(ty == 12 ? "Half-width" : (ty == 9 ? "Corner X" : "Size"), cfg.Elem[b + 3], 0.02f, 0.9f, 0.22f, ty == 18 ? "Image width." : "Radius / scale (X).");
        if (ty == 9) cfg.Elem[b + 4] = Knob("Corner Y", cfg.Elem[b + 4], 0.01f, 0.9f, 0.44f, "How far the brackets sit toward the top/bottom edge.");
        else if (ty == 12) cfg.Elem[b + 4] = Knob("Tick spacing", cfg.Elem[b + 4], 0.02f, 0.12f, 0.05f, "Distance between rangefinder ticks.");
        else if (ty >= 14 && ty <= 17) cfg.Elem[b + 4] = Knob("Flatten", cfg.Elem[b + 4], 0f, 1f, 0.5f, "Flatten vertically for ground perspective.");
        else if (ty == 1 || ty == 2 || ty == 3 || ty == 4 || ty == 5 || ty == 6 || ty == 7 || ty == 10 || ty == 11 || ty == 18)
            cfg.Elem[b + 15] = Knob(ty == 6 ? "Height" : "Size Y", cfg.Elem[b + 15], 0f, 0.9f, 0f, "Independent vertical size (0 = same as Size X).");
        if (ty != 18) cfg.Elem[b + 7] = Knob("Thickness", cfg.Elem[b + 7], 0.002f, 0.1f, 0.006f, "Line thickness (outlines).");
        cfg.Elem[b + 5] = Knob("Rotation", cfg.Elem[b + 5], -3.15f, 3.15f, 0f, "Rotation.");
        cfg.Elem[b + 6] = Knob("Spin", cfg.Elem[b + 6], -2f, 2f, 0f, "Rotation speed over time (animates).");
        if (ty == 3 || ty == 4) cfg.Elem[b + 14] = Knob(ty == 4 ? "Points" : "Sides", cfg.Elem[b + 14], 3f, 12f, 6f, "Polygon sides / star points.", "%.0f");
        else if (ty == 7) cfg.Elem[b + 14] = Knob("Arc span", cfg.Elem[b + 14], 1f, 12f, 6f, "How much of the ring the arc covers.");
        else if (ty == 16) cfg.Elem[b + 14] = Knob("Cone angle", cfg.Elem[b + 14], 1f, 12f, 4f, "Width of the cone.");

        if (ty >= 1 && ty <= 8)
        {
            cfg.Elem[b + 16] = Knob("Glow", cfg.Elem[b + 16], 0f, 2f, 0f, "Soft neon halo bleeding out from the edge (0 = off).");
            if (cfg.Elem[b + 16] > 0.001f)
                cfg.Elem[b + 17] = Knob("Glow width", cfg.Elem[b + 17] > 0.0005f ? cfg.Elem[b + 17] : 0.045f, 0.01f, 0.2f, 0.045f, "How far the halo spreads.");
        }

        Combo("Blend", "##eblend", UiElemBlend, (int)(cfg.Elem[b + PluginConfig.ElemBlend] + 0.5f),
              v => cfg.Elem[b + PluginConfig.ElemBlend] = v);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip(
            "Add is a light source: it can only brighten, whatever colour you give it.\n" +
            "Normal paints over. Multiply darkens. Screen brightens without blowing out.\n" +
            "Overlay and Soft light keep the shot's own light and shade showing through,\nwhich is what makes an overlay look part of the photograph.");

        cfg.Elem[b + PluginConfig.ElemFeather] = Knob(ty == 18 ? "Edge softness" : "Softness",
            cfg.Elem[b + PluginConfig.ElemFeather], 0f, ty == 18 ? 0.5f : 0.08f, 0f,
            ty == 18 ? "Fades the image's own rectangular edge. A hard rectangle across a\nphotograph is the one thing that always reads as pasted on."
                     : "Widens the edge of the shape itself, so it can sit on a photograph\nwithout announcing that it is vector art.",
            ty == 18 ? "%.3f" : "%.4f");

        if (cfg.MaskAMode != 0 || cfg.MaskBMode != 0 || cfg.MaskCMode != 0)
        {
            int slot = _elemSlot;
            ImGui.TextUnformatted("Limit to");
            ImGui.SameLine();
            MaskToggles("Elem" + slot, () => cfg.ElemMasks(slot), v => cfg.SetElemMasks(slot, v));
            ImGui.NewLine();
        }

        bool front = cfg.Elem[b + 13] > 0.5f;
        if (ImGui.Checkbox("In front of subject##e", ref front)) { cfg.Elem[b + 13] = front ? 1f : 0f; _dirty = true; }
        if (ty == 3 || ty == 4 || ty == 5 || ty == 6)
        {
            ImGui.SameLine();
            bool fl = cfg.Elem[b + 12] > 0.5f;
            if (ImGui.Checkbox("Filled##e", ref fl)) { cfg.Elem[b + 12] = fl ? 1f : 0f; _dirty = true; }
        }

        ImGui.Spacing();
        if (ImGui.SmallButton("Clear layer")) { cfg.ClearElemSlot(_elemSlot); _dirty = true; }
        ImGui.SameLine();
        if (ImGui.SmallButton("Duplicate to\u2026")) ImGui.OpenPopup("##edup");
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Copy this whole layer, image and all, into another slot.\nBuilding a second one by hand is a dozen numbers and one of them wrong.");
        if (ImGui.BeginPopup("##edup"))
        {
            for (int L = 0; L < 8; L++)
            {
                if (L == _elemSlot) continue;
                int lt = (int)(cfg.Elem[L * PluginConfig.ElemStride] + 0.5f);
                if (ImGui.Selectable($"Slot {L + 1}" + (lt > 0 ? "  (overwrites " + UiTypes[Math.Clamp(lt, 0, UiTypes.Length - 1)] + ")" : "  (empty)")))
                {
                    PushUndo(cfg);
                    cfg.CopyElemSlot(_elemSlot, L);
                    _elemSlot = L;
                    _dirty = true;
                }
            }
            ImGui.EndPopup();
        }
    }

    private void DrawElementImage(PluginConfig cfg, int slot)
    {
        string cur = cfg.ElemImages != null && slot < cfg.ElemImages.Length ? (cfg.ElemImages[slot] ?? "") : "";
        bool portable = ElementImages.IsStoredName(cur);

        if (cur.Length == 0) ImGui.TextDisabled("No image on this layer yet.");
        else
        {
            ImGui.TextUnformatted(System.IO.Path.GetFileName(cur));
            if (!portable)
                ImGui.TextColored(new Vector4(1f, 0.62f, 0.25f, 1f),
                    "Linked from elsewhere on this PC — it breaks if that file moves.");
        }

        if (ImGui.Button("Import image\u2026", new Vector2(140f, 0)))
            _dialogs.OpenFileDialog("Choose an image", "Images{.png,.jpg,.jpeg,.bmp,.gif,.tga,.dds}", (ok, path) =>
            {
                if (!ok || string.IsNullOrWhiteSpace(path)) return;
                if (cfg.ElemImages == null || slot >= cfg.ElemImages.Length) return;
                if (ElementImages.Import(path, out var name, out var err))
                {
                    cfg.ElemImages[slot] = name;
                    _status = $"Imported \u2018{name}\u2019 into the plugin's own folder.";
                    _dirty = true;
                }
                else _status = err;
            });
        if (ImGui.IsItemHovered()) ImGui.SetTooltip(
            "Copies the file into the plugin's folder and uses that copy from then on.\n" +
            "The original can be moved or deleted afterwards without breaking the layer.");

        if (cur.Length > 0)
        {
            ImGui.SameLine();
            if (ImGui.Button("Remove", new Vector2(80f, 0))) { cfg.ElemImages![slot] = ""; _dirty = true; }
            if (!portable)
            {
                ImGui.SameLine();
                if (ImGui.Button("Make portable", new Vector2(120f, 0)))
                {
                    if (ElementImages.Import(cur, out var name, out var err))
                    { cfg.ElemImages![slot] = name; _status = $"Copied in as \u2018{name}\u2019."; _dirty = true; }
                    else _status = err;
                }
                if (ImGui.IsItemHovered()) ImGui.SetTooltip("Copy this file into the plugin's folder, so the layer stops\ndepending on where it currently sits.");
            }
        }
        ImGui.SameLine();
        if (ImGui.Button("Folder", new Vector2(80f, 0))) OpenFolder(ElementImages.FolderPath);

        int slotSel = slot;
        Combo("Fit", "##efit", UiElemFit, cfg.ElemFit(slotSel), v => cfg.SetElemFit(slotSel, v));
        if (ImGui.IsItemHovered()) ImGui.SetTooltip(
            "Fit inside keeps the whole image and leaves gaps.\nFill crops it to cover the box.\n" +
            "Stretch ignores the image's shape.\nTile repeats it across the frame — for textures, dust and grain plates.");

        bool fh = cfg.ElemFlag(slotSel, PluginConfig.ElemFlagFlipH);
        if (ImGui.Checkbox("Flip horizontally##eimg", ref fh)) { cfg.SetElemFlag(slotSel, PluginConfig.ElemFlagFlipH, fh); _dirty = true; }
        ImGui.SameLine();
        bool fv = cfg.ElemFlag(slotSel, PluginConfig.ElemFlagFlipV);
        if (ImGui.Checkbox("Flip vertically##eimg", ref fv)) { cfg.SetElemFlag(slotSel, PluginConfig.ElemFlagFlipV, fv); _dirty = true; }
    }

    private float Knob(string label, float v, float min, float max, float def, string? tip = null, string fmt = "%.2f")
    {
        ImGui.TextUnformatted(label);
        var t = v;
        ImGui.PushItemWidth(-1f);
        if (ImGui.SliderFloat("##" + label, ref t, min, max, fmt)) _dirty = true;
        if (ImGui.IsItemClicked(ImGuiMouseButton.Right)) { t = def; _dirty = true; }
        bool hovered = ImGui.IsItemHovered();
        ImGui.PopItemWidth();
        if (hovered)
            ImGui.SetTooltip((tip != null ? tip + "\n" : "") + "Right-click: reset to default");
        return t;
    }

    private void DrawTextGroup(PluginConfig cfg)
    {
        cfg.Texts ??= new List<TextMarker>();
        using var grp = GroupEn("Text markers", cfg.Texts.Count > 0, cfg.EnText, v => cfg.EnText = v);
        if (!grp.Show) return;

        ImGui.TextDisabled("Captions over the shot, and in the file. They are rasterised once and\nshown as that same image, so the preview is what gets saved.");
        if (ImGui.SmallButton("+ Add text")) { cfg.Texts.Add(new TextMarker()); _textSel = cfg.Texts.Count - 1; _dirty = true; }
        if (cfg.Texts.Count == 0) return;
        if (_textSel < 0 || _textSel >= cfg.Texts.Count) _textSel = 0;

        for (int i = 0; i < cfg.Texts.Count; i++)
        {
            if (i > 0) ImGui.SameLine(0f, 3f);
            var ti = cfg.Texts[i];
            var raw = (ti?.Text ?? "").Replace("\n", " ").Trim();
            var label = raw.Length == 0 ? "(empty)" : (raw.Length > 12 ? raw.Substring(0, 12) + "\u2026" : raw);
            bool sel = _textSel == i;
            ImGui.PushStyleColor(ImGuiCol.Button, sel ? new Vector4(0.35f, 0.5f, 0.75f, 1f)
                                                      : new Vector4(0.22f, 0.22f, 0.24f, 1f));
            if (ImGui.Button(label + "##tsel" + i)) _textSel = i;
            ImGui.PopStyleColor();
            if (ImGui.IsItemHovered() && raw.Length > 12) ImGui.SetTooltip(raw);
        }

        var t = cfg.Texts[_textSel];
        string txt = t.Text ?? "";
        ImGui.TextUnformatted("Content");
        if (ImGui.InputTextMultiline("##ttext", ref txt, 512,
                                     new Vector2(-1f, ImGui.GetTextLineHeight() * 3.4f)))
        { t.Text = txt; _dirty = true; }

        bool placing = cfg.PlacingText == _textSel + 1;
        if (placing) ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.30f, 0.62f, 0.34f, 1f));
        if (ImGui.Button(placing ? "Placing\u2026 (Esc to finish)##tplace" : "Place on screen##tplace", new Vector2(220f, 0)))
        { cfg.PlacingText = placing ? 0 : _textSel + 1; _dirty = true; }
        if (placing) ImGui.PopStyleColor();
        if (ImGui.IsItemHovered()) ImGui.SetTooltip(
            "Drag the caption onto the shot. Two sliders can put it anywhere, and\nneither of them can tell you whether it is sitting on somebody's face.\n\n" +
            "While this is on, the mouse belongs to the caption rather than to the\ngpose camera. Escape gives it back.");

        t.X = Knob("Position X", t.X, 0f, 1f, 0.5f, "Horizontal position (0 = left, 1 = right).\nThe caption is anchored at its middle, so editing the wording does not move it.", "%.3f");
        t.Y = Knob("Position Y", t.Y, 0f, 1f, 0.5f, "Vertical position (0 = top, 1 = bottom).", "%.3f");
        t.Rotation = Knob("Rotation", t.Rotation, -3.15f, 3.15f, 0f, "Turns the caption. The plate and the shadow turn with it.");
        t.Size = Knob("Size", t.Size, 0.005f, 0.30f, 0.06f,
            "Text height as a fraction of the frame height, not pixels \u2014 so a caption is\nthe same size relative to the picture at 1x and at 4x.", "%.3f");

        var tc = ColorPick("Colour", new Vector3(t.R, t.G, t.B), new Vector3(1f, 1f, 1f));
        t.R = tc.X; t.G = tc.Y; t.B = tc.Z;
        t.A = Knob("Opacity", t.A, 0f, 1f, 1f, "Text opacity.");

        var aligns = UiAligns;
        int al = Math.Clamp(t.Align, 0, 2);
        ImGui.TextUnformatted("Align"); ImGui.SameLine(90f); ImGui.PushItemWidth(150f);
        if (ImGui.BeginCombo("##talign", aligns[al]))
        {
            for (int a = 0; a < 3; a++) if (ImGui.Selectable(aligns[a], al == a)) { t.Align = a; _dirty = true; }
            ImGui.EndCombo();
        }
        ImGui.PopItemWidth();
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("How the lines line up against each other. The caption as a whole\nstays centred on its position.");

        ImGui.TextUnformatted("Font"); ImGui.SameLine(90f); ImGui.PushItemWidth(-1f);
        var famNow = string.IsNullOrWhiteSpace(t.Font) ? "Default sans" : t.Font;
        if (ImGui.BeginCombo("##tfont", famNow))
        {
            var ff = _fontFilter;
            ImGui.PushItemWidth(-1f);
            if (ImGui.InputTextWithHint("##tfontfilter", "Search fonts\u2026", ref ff, 48)) _fontFilter = ff;
            ImGui.PopItemWidth();
            if (ImGui.Selectable("Default sans", string.IsNullOrWhiteSpace(t.Font))) { t.Font = ""; _dirty = true; }
            foreach (var name in TextRender.Families)
            {
                if (_fontFilter.Length > 0 && !name.Contains(_fontFilter, StringComparison.OrdinalIgnoreCase)) continue;
                if (ImGui.Selectable(name, name == t.Font)) { t.Font = name; _dirty = true; }
            }
            ImGui.EndCombo();
        }
        ImGui.PopItemWidth();
        if (ImGui.IsItemHovered()) ImGui.SetTooltip(
            "Every font installed on this PC. A look from somebody else naming a font\nyou do not have falls back to the default rather than refusing to draw.");

        bool bold = t.Bold;
        if (ImGui.Checkbox("Bold##t", ref bold)) { t.Bold = bold; _dirty = true; }
        ImGui.SameLine();
        bool ital = t.Italic;
        if (ImGui.Checkbox("Italic##t", ref ital)) { t.Italic = ital; _dirty = true; }

        bool ol = t.Outline;
        if (ImGui.Checkbox("Outline##t", ref ol)) { t.Outline = ol; _dirty = true; }
        if (ImGui.IsItemHovered()) ImGui.SetTooltip(
            "A real outline around the letterforms, drawn under them.\nThis is what keeps a caption readable over a busy or bright shot.");
        if (t.Outline)
        {
            ImGui.Indent(10f);
            t.OutlineWidth = Knob("  Outline width", t.OutlineWidth, 0.01f, 0.30f, 0.10f,
                "As a fraction of the text height, so it holds up at any size.", "%.3f");
            var oc = ColorPick("  Outline colour", new Vector3(t.OutlineR, t.OutlineG, t.OutlineB), new Vector3(0f, 0f, 0f));
            t.OutlineR = oc.X; t.OutlineG = oc.Y; t.OutlineB = oc.Z;
            ImGui.Unindent(10f);
        }

        if (ImGui.CollapsingHeader("Typography##t"))
        {
            ImGui.Indent(10f);
            t.LineHeight = Knob("  Line height", t.LineHeight, 0.6f, 2.5f, 1.15f,
                "Distance between lines, as a multiple of the text height.\nOnly does anything once a caption has more than one line.");
            t.Tracking = Knob("  Letter spacing", t.Tracking, -0.1f, 0.6f, 0f,
                "Space added between letters, as a fraction of the text height.\nAt zero the line keeps the font's own kerning, which is what you\nwant for a sentence; open it up for a title.", "%.3f");
            ImGui.Unindent(10f);
        }

        if (ImGui.CollapsingHeader("Shadow##t"))
        {
            ImGui.Indent(10f);
            t.ShadowAmount = Knob("  Shadow", t.ShadowAmount, 0f, 1f, 0f, "Opacity of the drop shadow (0 = off).");
            if (t.ShadowAmount > 0.002f)
            {
                t.ShadowDist = Knob("  Distance", t.ShadowDist, 0f, 0.5f, 0.06f, "How far it falls, as a fraction of the text height.", "%.3f");
                t.ShadowAngle = Knob("  Direction", t.ShadowAngle, -3.15f, 3.15f, 1.05f, "Which way it falls.");
                t.ShadowSoft = Knob("  Softness", t.ShadowSoft, 0f, 0.4f, 0.06f,
                    "A real blur. This is the control that separates a shadow from a\nsecond copy of the text sitting behind the first.", "%.3f");
                var sc = ColorPick("  Shadow colour", new Vector3(t.ShadowR, t.ShadowG, t.ShadowB), new Vector3(0f, 0f, 0f));
                t.ShadowR = sc.X; t.ShadowG = sc.Y; t.ShadowB = sc.Z;
            }
            ImGui.Unindent(10f);
        }

        if (ImGui.CollapsingHeader("Plate##t"))
        {
            ImGui.Indent(10f);
            ImGui.TextDisabled("A panel behind the words. The one thing that holds up over a shot that\nis busy AND bright, where an outline only adds more edges.");
            t.Plate = Knob("  Plate", t.Plate, 0f, 1f, 0f, "Opacity of the panel (0 = off).");
            if (t.Plate > 0.002f)
            {
                var pc = ColorPick("  Plate colour", new Vector3(t.PlateR, t.PlateG, t.PlateB), new Vector3(0f, 0f, 0f));
                t.PlateR = pc.X; t.PlateG = pc.Y; t.PlateB = pc.Z;
                t.PlatePad = Knob("  Padding", t.PlatePad, 0f, 1.5f, 0.35f, "Space around the text, as a fraction of the text height.");
                t.PlateRound = Knob("  Corner", t.PlateRound, 0f, 1f, 0.30f, "0 = square corners, 1 = fully rounded ends.");
            }
            ImGui.Unindent(10f);
        }

        ImGui.Spacing();
        if (ImGui.SmallButton("Duplicate##t"))
        {
            cfg.Texts.Insert(_textSel + 1, new TextMarker
            {
                Text = t.Text, X = Math.Clamp(t.X + 0.03f, 0f, 1f), Y = Math.Clamp(t.Y + 0.05f, 0f, 1f),
                Size = t.Size, R = t.R, G = t.G, B = t.B, A = t.A, Align = t.Align,
                Outline = t.Outline, Font = t.Font, Bold = t.Bold, Italic = t.Italic,
                OutlineWidth = t.OutlineWidth, OutlineR = t.OutlineR, OutlineG = t.OutlineG, OutlineB = t.OutlineB,
                LineHeight = t.LineHeight, Tracking = t.Tracking, Rotation = t.Rotation,
                ShadowAmount = t.ShadowAmount, ShadowDist = t.ShadowDist, ShadowAngle = t.ShadowAngle,
                ShadowSoft = t.ShadowSoft, ShadowR = t.ShadowR, ShadowG = t.ShadowG, ShadowB = t.ShadowB,
                Plate = t.Plate, PlateR = t.PlateR, PlateG = t.PlateG, PlateB = t.PlateB,
                PlatePad = t.PlatePad, PlateRound = t.PlateRound,
            });
            _textSel++;
            _dirty = true;
        }
        ImGui.SameLine();
        if (ImGui.SmallButton("Delete this text"))
        { cfg.Texts.RemoveAt(_textSel); _textSel = Math.Max(0, _textSel - 1); cfg.PlacingText = 0; _dirty = true; }

        if (cfg.ExportAspect != 0)
            ImGui.TextDisabled("Position is against the whole screen; the export is cropped to the\nchosen aspect. Tick Show export frame to see what is kept.");
        ImGui.Spacing();
    }

    private bool Group(string title, bool active, bool open = false)
    {
        if (_filter != FilterMode.None && !MatchesFilter(title)) return false;
        var flags = (open || _filter != FilterMode.None) ? ImGuiTreeNodeFlags.DefaultOpen : ImGuiTreeNodeFlags.None;
        return ImGui.CollapsingHeader((active ? title + "   ●" : title) + "###" + title, flags);
    }

    private bool Filtering => _filter != FilterMode.None;

    private void ZoneToggles(string title, Func<int> get, Action<int> set)
    {
        int bits = get();
        using var pad = ImRaii.PushStyle(ImGuiStyleVar.FramePadding, new Vector2(7f, 3f));
        ReadOnlySpan<string> letters = new[] { "F", "C", "B" };
        for (int k = 0; k < 3; k++)
        {
            int bit = 1 << k;
            bool on = (bits & bit) != 0;
            if (on) ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.26f, 0.52f, 0.85f, 1f));
            else ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.22f, 0.22f, 0.24f, 1f));
            if (ImGui.SmallButton(letters[k] + "##z" + title + k))
            {
                set(ZoneBits.ToggleZone(bits, bit));
                _dirty = true;
            }
            ImGui.PopStyleColor();
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(k == 0 ? "Foreground — anything nearer than your character.\nNeeds a Foreground split above 0 (Color tab)."
                               : k == 1 ? "Character — the subject."
                                        : "Background — everything behind the subject.");
            ImGui.SameLine(0f, 2f);
        }

        ImGui.Dummy(new Vector2(6f, 0f));
        ImGui.SameLine(0f, 2f);
        MaskToggles(title, get, set);
    }

    private void MaskToggles(string title, Func<int> get, Action<int> set)
    {
        int bits = get();
        var cfg = Plugin.Config;
        using var pad = ImRaii.PushStyle(ImGuiStyleVar.FramePadding, new Vector2(7f, 3f));
        for (int k = 0; k < 3; k++)
        {
            if (cfg.MaskMode(k) == 0) continue;
            int bit = ZoneBits.MaskBit(k);
            bool on = (bits & bit) != 0;
            ImGui.PushStyleColor(ImGuiCol.Button, on ? new Vector4(0.30f, 0.62f, 0.34f, 1f)
                                                     : new Vector4(0.22f, 0.22f, 0.24f, 1f));
            if (ImGui.SmallButton(((char)('A' + k)).ToString() + "##m" + title + k))
            {
                set(ZoneBits.ToggleMask(bits, bit));
                _dirty = true;
            }
            ImGui.PopStyleColor();
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip($"Limit this to mask {(char)('A' + k)}.\n" +
                                 "Masks combine by narrowing: two of them means where both overlap.\nSet them up under Masks, in the Color tab.");
            ImGui.SameLine(0f, 2f);
        }
    }

    private GroupScope GroupEn(string title, bool active, bool enabled, Action<bool> setEnabled, bool open = false,
                              Func<int>? zoneGet = null, Action<int>? zoneSet = null, bool maskOnly = false)
    {
        if (_filter != FilterMode.None && !MatchesFilter(title))
            return new GroupScope(false, null);

        var e = enabled;
        if (ImGui.Checkbox("##en" + title, ref e)) { setEnabled(e); _dirty = true; }
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Enable / bypass this group");
        ImGui.SameLine();
        if (zoneGet != null && zoneSet != null)
        {
            if (maskOnly) MaskToggles(title, zoneGet, zoneSet);
            else ZoneToggles(title, zoneGet, zoneSet);
        }
        bool pinned = Plugin.Config.Pinned.Contains(title);
        var flags = (open || _filter != FilterMode.None) ? ImGuiTreeNodeFlags.DefaultOpen : ImGuiTreeNodeFlags.None;
        string label = (pinned ? "★ " : "") + (active ? title + "   ●" : title);
        bool show = ImGui.CollapsingHeader(label + "###" + title, flags);
        if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
        {
            if (pinned) Plugin.Config.Pinned.Remove(title); else Plugin.Config.Pinned.Add(title);
            _dirty = true;
        }
        if (ImGui.IsItemHovered())
            ImGui.SetTooltip(pinned ? "Right-click: unpin from Favorites" : "Right-click: pin to Favorites");
        return new GroupScope(show, show ? (IDisposable?)ImRaii.PushId(title) : null);
    }

    private int _maskSel;

    private static readonly string[] UiMaskMode =
        { "Off", "Ellipse", "Linear gradient", "Depth band" };

    private void DrawMasksGroup(PluginConfig cfg)
    {
        if (_filter != FilterMode.None && !MatchesFilter("Masks")) return;
        if (!ImGui.CollapsingHeader("Masks###Masks")) return;

        ImGui.Indent(10f);
        ImGui.TextDisabled("A shaped region an effect can be limited to. Set one up here, then press\nits letter on any effect\u2019s header, beside F / C / B.");

        for (int k = 0; k < 3; k++)
        {
            bool sel = _maskSel == k;
            bool live = cfg.MaskMode(k) != 0;
            ImGui.PushStyleColor(ImGuiCol.Button, sel ? new Vector4(0.30f, 0.62f, 0.34f, 1f)
                                                      : new Vector4(0.22f, 0.22f, 0.24f, 1f));
            if (ImGui.Button(((char)('A' + k)).ToString() + (live ? " \u25cf" : "") + "##masksel" + k, new Vector2(52f, 0)))
            {
                _maskSel = k;
                if (cfg.DebugShowMask) { cfg.MaskShowWhich = k + 1; _dirty = true; }
            }
            ImGui.PopStyleColor();
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(live ? UiMaskMode[Math.Clamp(cfg.MaskMode(k), 0, 3)] : "Not set up yet.");
            ImGui.SameLine();
        }
        ImGui.NewLine();

        int m = Math.Clamp(_maskSel, 0, 2);
        char letter = (char)('A' + m);
        Combo("Shape", "##maskmode" + letter, UiMaskMode, cfg.MaskMode(m), v =>
        {
            cfg.SetMaskMode(m, v);
            if ((v == 0 || v == 3) && cfg.PlacingMask == m + 1) cfg.PlacingMask = 0;
        });

        int md = cfg.MaskMode(m);
        if (md == 0)
        {
            ImGui.TextDisabled("Ellipse \u2014 a face, a shoulder, one pool of interest.");
            ImGui.TextDisabled("Linear gradient \u2014 half the frame: the top, one side, a diagonal.");
            ImGui.TextDisabled("Depth band \u2014 a slice of distance. Zones say near or far; this is\nhow you say only the middle.");
            ImGui.Spacing();
            ImGui.TextDisabled("Off covers everything, so an effect subscribed to a mask that is not\nset up is left alone rather than switched off.");
            ImGui.Unindent(10f);
            return;
        }

        if (md != 3)
        {
            bool placing = cfg.PlacingMask == m + 1;
            if (placing) ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.30f, 0.62f, 0.34f, 1f));
            if (ImGui.Button(placing ? "Placing\u2026 (Esc to finish)##place" : "Place on screen##place", new Vector2(220f, 0)))
            {
                cfg.PlacingMask = placing ? 0 : m + 1;
                _dirty = true;
            }
            if (placing) ImGui.PopStyleColor();
            if (ImGui.IsItemHovered()) ImGui.SetTooltip(
                "Drag the mask onto the shot instead of nudging numbers.\n\n" +
                "While this is on, the mouse belongs to the mask rather than to the\ngpose camera \u2014 they are the same gesture, so only one can have it.\nEscape gives it back.");
            if (!_live.Enabled)
                ImGui.TextDisabled("Turn live preview on first, or there is nothing to place it against.");
        }

        if (md == 1)
        {
            cfg.SetMaskCx(m, Knob("Centre X##mk" + letter, cfg.MaskCx(m), 0f, 1f, 0.5f, "Across the frame. 0 = left edge, 1 = right.", "%.3f"));
            cfg.SetMaskCy(m, Knob("Centre Y##mk" + letter, cfg.MaskCy(m), 0f, 1f, 0.45f, "Down the frame. 0 = top, 1 = bottom.", "%.3f"));
            cfg.SetMaskSize(m, Knob("Radius##mk" + letter, cfg.MaskSize(m), 0.01f, 1.2f, 0.25f, "Measured in frame HEIGHTS, so the mask keeps its size and its shape\nwhen the export aspect changes.", "%.3f"));
            cfg.SetMaskEllipse(m, Knob("Squash##mk" + letter, cfg.MaskEllipse(m), 0.15f, 4f, 1.2f, "Height against width. Above 1 is taller than wide \u2014 a head.\nBelow 1 is wider than tall \u2014 a band across the body."));
            cfg.SetMaskAngle(m, Knob("Rotation##mk" + letter, cfg.MaskAngle(m), -3.15f, 3.15f, 0f, "Turns the ellipse. Does nothing while Squash is exactly 1, because\na circle has no orientation to turn."));
        }
        else if (md == 2)
        {
            cfg.SetMaskCx(m, Knob("Edge X##mk" + letter, cfg.MaskCx(m), 0f, 1f, 0.5f, "A point the dividing line passes through.", "%.3f"));
            cfg.SetMaskCy(m, Knob("Edge Y##mk" + letter, cfg.MaskCy(m), 0f, 1f, 0.5f, "A point the dividing line passes through.", "%.3f"));
            cfg.SetMaskAngle(m, Knob("Direction##mk" + letter, cfg.MaskAngle(m), -3.15f, 3.15f, 0f, "Turns the divide. The short arrow drawn on screen points INTO the covered\nside, so follow the arrow rather than guessing; 0 covers the left of a\nvertical divide."));
        }
        else
        {
            cfg.SetMaskCy(m, Knob("Centre depth##mk" + letter, cfg.MaskCy(m), 0f, 1f, 0.5f, "The distance the band sits at. Turn on the depth view (Export tab)\nto read off where things in this scene actually are.", "%.3f"));
            cfg.SetMaskSize(m, Knob("Band width##mk" + letter, cfg.MaskSize(m), 0.005f, 0.5f, 0.25f, "How much distance either side of the centre the band reaches.", "%.3f"));
            if (!_live.DepthAvailable)
                ImGui.TextColored(new Vector4(1f, 0.62f, 0.25f, 1f), "No depth yet \u2014 this mask covers everything until there is.");
        }

        cfg.SetMaskFeather(m, Knob("Feather##mk" + letter, cfg.MaskFeather(m), 0.001f, 0.5f, 0.08f, "How far the edge takes to fade out. A hard mask on a photograph reads as a\ncut-out, so this rarely wants to be near zero.", "%.3f"));

        bool inv = cfg.MaskInvert(m);
        if (ImGui.Checkbox("Invert##mk" + letter, ref inv)) { cfg.SetMaskInvert(m, inv); _dirty = true; }
        if (ImGui.IsItemHovered()) ImGui.SetTooltip(
            "Everything except the shape. Two masks narrow each other, so inverting is\nhow you reach the regions that narrowing alone cannot make.");

        int subs = cfg.MaskSubscribers(m);
        ImGui.TextDisabled(subs == 0
            ? "Nothing is using this mask yet — press " + letter + " on an effect’s header."
            : (subs == 1 ? "1 effect is limited to this mask." : subs + " effects are limited to this mask."));

        bool showing = cfg.DebugShowMask && cfg.MaskShowWhich == m + 1;
        if (ImGui.Checkbox("Show what this covers##mk" + letter, ref showing))
        {
            cfg.DebugShowMask = showing;
            cfg.MaskShowWhich = m + 1;
            if (showing) { cfg.DebugShowGate = false; cfg.DebugShowDepth = false; cfg.DebugShowClipping = false; cfg.DebugShowMatte = false; }
            _dirty = true;
        }
        if (ImGui.IsItemHovered()) ImGui.SetTooltip(
            "Paints the covered area green over a dimmed frame. A soft edge on a busy\nimage is not something the shot itself will ever show you.");
        if (cfg.DebugShowMask)
            ImGui.TextColored(new Vector4(1f, 0.62f, 0.25f, 1f),
                $"Mask view is on, showing {(char)('A' + Math.Clamp(cfg.MaskShowWhich, 1, 3) - 1)} \u2014 untick to see the shot.");

        ImGui.Unindent(10f);
    }

    private void DrawFinder(PluginConfig cfg)
    {
        ImGui.PushItemWidth(200f);
        var s = _search;
        if (ImGui.InputTextWithHint("##find", "Search controls…", ref s, 64)) _search = s;
        ImGui.PopItemWidth();
        ImGui.SameLine();
        if (!string.IsNullOrWhiteSpace(_search))
        {
            if (ImGui.SmallButton("Clear")) _search = string.Empty;
            ImGui.SameLine();
        }
        bool favView = _filter == FilterMode.Favorites;
        if (ImGui.Checkbox("★ Favorites", ref favView))
            _filter = favView ? FilterMode.Favorites : FilterMode.None;
        if (ImGui.IsItemHovered())
            ImGui.SetTooltip("Show only pinned groups.\nRight-click any group header to pin or unpin it.");

        if (!string.IsNullOrWhiteSpace(_search)) _filter = FilterMode.Search;
        else if (_filter == FilterMode.Search) _filter = FilterMode.None;

        var on = EnabledGroups(cfg);
        if (on.Count > 0 && _filter == FilterMode.None)
        {
            ImGui.TextDisabled($"On ({on.Count}):");
            float avail = ImGui.GetContentRegionAvail().X;
            for (int i = 0; i < on.Count; i++)
            {
                ImGui.SameLine();
                if (ImGui.GetCursorPosX() > avail - 90f) { ImGui.NewLine(); ImGui.TextDisabled("     "); ImGui.SameLine(); }
                if (ImGui.SmallButton(on[i].Label + "##on" + i)) _search = on[i].Label;
            }
            if (ImGui.IsItemHovered()) ImGui.SetTooltip("Click to find this group");
        }
        if (_filter == FilterMode.Favorites && cfg.Pinned.Count == 0)
            ImGui.TextDisabled("Nothing pinned yet — right-click a group header to pin it here.");
        ImGui.Spacing();
    }

    private static readonly (System.Reflection.PropertyInfo Prop, string Label)[] EnSwitches =
        Array.ConvertAll(
            Array.FindAll(typeof(PluginConfig).GetProperties(),
                p => p.PropertyType == typeof(bool) && p.Name.StartsWith("En", StringComparison.Ordinal) && p.Name.Length >= 3),
            p => (p, EnLabel(p.Name)));

    private static List<(string Prop, string Label)> EnabledGroups(PluginConfig cfg)
    {
        var outp = new List<(string, string)>();
        foreach (var (p, label) in EnSwitches)
            if (p.GetValue(cfg) is bool b && b) outp.Add((p.Name, label));
        return outp;
    }

    private static string EnLabel(string prop) => prop switch
    {
        "EnRim" => "Rim & separation",
        "EnBgFill" => "Solid backdrop",
        "EnBgBlur" => "Background blur",
        "EnSubjectIso" => "Background push",
        "EnVhs" => "Analog",
        "EnHud" => "Magitek HUD",
        "EnEdge" => "Edge integration",
        "EnBeauty" => "Beauty softening",
        "EnSkin" => "Skin warmth",
        "EnSpot" => "Spotlight",
        "EnGobo" => "Gobo",
        "EnWet" => "Wet skin",
        "EnDof" => "Depth of field",
        "EnForegroundOn" => "Foreground layer",
        "EnFrame" => "Frame & corners",
        "EnText" => "Text markers",
        "EnElements" => "Elements",
        _ => prop.Substring(2),
    };

    private enum FilterMode { None, Search, Favorites }
    private FilterMode _filter = FilterMode.None;
    private string _search = string.Empty;

    private bool MatchesFilter(string title) => _filter switch
    {
        FilterMode.Search => title.Contains(_search, StringComparison.OrdinalIgnoreCase),
        FilterMode.Favorites => Plugin.Config.Pinned.Contains(title),
        _ => true,
    };

    private void DrawAllBodies(PluginConfig cfg)
    {
        LookBody(cfg); CameraBody(cfg); LightBody(cfg); SubjectBody(cfg);
        BackgroundBody(cfg); FxBody(cfg); OverlaysBody(cfg);
    }

    private readonly struct GroupScope : IDisposable
    {
        public readonly bool Show;
        private readonly IDisposable? _id;
        public GroupScope(bool show, IDisposable? id) { Show = show; _id = id; }
        public void Dispose() => _id?.Dispose();
    }

    private Vector3 ColorPick(string label, Vector3 v, Vector3 def)
    {
        ImGui.TextUnformatted(label);
        ImGui.SameLine(130f);
        if (ImGui.ColorEdit3("##" + label, ref v, ImGuiColorEditFlags.NoInputs)) _dirty = true;
        if (ImGui.IsItemClicked(ImGuiMouseButton.Right)) { v = def; _dirty = true; }
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Right-click: reset to default");
        return v;
    }

    private void RequestSave()
    {
        _status = "Saving…";
        _live.RequestExport(Plugin.Config.OutputDirectory, result =>
            _status = result.StartsWith("error:") ? result : $"Saved: {result}");
    }

    private void DrawLooksTab(PluginConfig cfg)
    {
        using var tab = ImRaii.TabItem("Looks");
        if (!tab) return;
        LooksBody(cfg);
    }

    private void LooksBody(PluginConfig cfg)
    {
        var cat = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var (bn, bc, _) in LookStore.Builtins) cat[bn] = bc;
        var legacyCat = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var (ln, lc, _) in LookStore.Legacy)
            if (!cat.ContainsKey(ln)) legacyCat[ln] = lc;

        bool usable = LookStore.IsNameUsable(_lookName, out var nameError);
        string trimmed = usable ? LookName.Clean(_lookName) : _lookName.Trim();
        bool exists = trimmed.Length > 0 && _lookList.Contains(trimmed);
        bool has = _lookSel.Length > 0 && _lookList.Contains(_lookSel);
        bool isBuiltin = has && cat.ContainsKey(_lookSel);

        if (has)
        {
            ImGui.TextDisabled("Loaded:");
            ImGui.SameLine();
            ImGui.TextColored(AccentCol, _lookSel);
            if (_editedSinceLoad) { ImGui.SameLine(); ImGui.TextDisabled("(edited)"); }

            using (ImRaii.Disabled(_dirty || _savePending))
                ImGui.Button("Hold to preview", new Vector2(120f, 0));
            bool holding = ImGui.IsItemActive() && !_dirty && !_savePending;
            if (holding && _peekBackup.Length == 0)
            {
                _peekBackup = LookStore.Capture(cfg);
                if (cfg.LoadFromBase) cfg.ResetLook();
                LookStore.Load(_lookSel, cfg, (LookStore.Part)_applyPart);
            }
            else if (!holding && _peekBackup.Length > 0)
            {
                LookStore.Apply(_peekBackup, cfg, LookStore.Part.All);
                _peekBackup = "";
            }
            if (ImGui.IsItemHovered() && _peekBackup.Length == 0) ImGui.SetTooltip(
                "Hold to see this look on the shot in front of you.\nRelease and it goes back. Nothing is saved either way.");

            if (_editedSinceLoad)
            {
                ImGui.SameLine();
                if (ImGui.Button("Revert##lookrevert", new Vector2(78f, 0)))
                {
                    if (LookStore.Load(_lookSel, cfg, LookStore.Part.All))
                    {
                        _dirty = true;
                        _loadedSnapshot = LookStore.Capture(cfg);
                        _editedSinceLoad = false;
                        _status = $"Reverted to the saved \u2018{_lookSel}\u2019.";
                    }
                    else _status = $"Could not re-read \u2018{_lookSel}\u2019.";
                }
                if (ImGui.IsItemHovered()) ImGui.SetTooltip(
                    "Throw away every change since this look was loaded.\nUndo still steps back from here.");
            }

            ImGui.SameLine();
            if (isBuiltin)
            {
                if (ImGui.Button("Reset##looksel", new Vector2(70f, 0)))
                {
                    LookStore.RegenerateBuiltin(_lookSel);
                    LookStore.Load(_lookSel, cfg);
                    _dirty = true;
                    _lookList = LookStore.List();
                    _status = $"Reset \u2018{_lookSel}\u2019 to its original.";
                }
                if (ImGui.IsItemHovered()) ImGui.SetTooltip("Put this built-in look back the way it shipped.");
            }
            else if (_confirmDelete == _lookSel)
            {
                if (ImGui.Button("Confirm delete##lookdel", new Vector2(126f, 0)))
                {
                    if (LookStore.Delete(_lookSel, out var delError))
                    {
                        _status = $"Deleted \u2018{_lookSel}\u2019.";
                        cfg.PinnedLooks.Remove(_lookSel);
                        _lookSel = ""; _confirmDelete = "";
                    }
                    else _status = delError;
                    _lookList = LookStore.List();
                }
                ImGui.SameLine();
                if (ImGui.Button("Keep##lookkeep", new Vector2(60f, 0))) _confirmDelete = "";
            }
            else if (ImGui.Button("Delete##looksel", new Vector2(70f, 0))) _confirmDelete = _lookSel;
        }
        else
        {
            ImGui.TextDisabled("Nothing loaded.");
            ImGui.TextDisabled("Click a look below to load it.");
        }

        if (_applyPart != 0)
        {
            ImGui.TextColored(new Vector4(1f, 0.62f, 0.25f, 1f),
                $"Clicking a look loads only its {UiApplyPart[_applyPart].ToLowerInvariant()}.");
            ImGui.SameLine();
            if (ImGui.SmallButton("Load everything again##applyreset2")) _applyPart = 0;
        }
        if (cfg.LoadFromBase)
        {
            ImGui.TextColored(new Vector4(1f, 0.62f, 0.25f, 1f), "Clicking a look clears everything else first.");
            ImGui.SameLine();
            if (ImGui.SmallButton("Stop clearing##basereset")) { cfg.LoadFromBase = false; _dirty = true; }
        }

        ImGui.Separator();

        if (ImGui.CollapsingHeader("Save, import, export"))
        {
            ImGui.Indent(10f);
            ImGui.PushItemWidth(-96f);
            ImGui.InputTextWithHint("##lookname", "Save current settings as\u2026", ref _lookName, 64);
            ImGui.PopItemWidth();
            ImGui.SameLine();
            using (ImRaii.Disabled(!usable))
                if (ImGui.Button(exists ? "Overwrite" : "Save", new Vector2(84f, 0)))
                {
                    var part = (LookStore.Part)_savePart;
                    if (LookStore.Save(_lookName, cfg, out var saveError, part))
                    {
                        if (part == LookStore.Part.All)
                        {
                            _loadedSnapshot = LookStore.Capture(cfg);
                            _editedSinceLoad = false;
                        }
                        _status = part == LookStore.Part.All
                            ? $"Saved \u2018{trimmed}\u2019."
                            : $"Saved the {UiApplyPart[_savePart]} of \u2018{trimmed}\u2019 \u2014 loading it leaves everything else alone.";
                        _lookList = LookStore.List();
                        _lookSel = trimmed;
                    }
                    else _status = saveError;
                }
            if (_lookName.Trim().Length > 0 && !usable)
                ImGui.TextColored(new Vector4(1f, 0.62f, 0.25f, 1f), nameError);
            else if (exists)
                ImGui.TextDisabled($"\u2018{trimmed}\u2019 already exists \u2014 saving replaces it.");
            else if (usable && trimmed != _lookName.Trim())
                ImGui.TextDisabled($"Will be saved as \u2018{trimmed}\u2019.");

            if (ImGui.Button("Import\u2026", new Vector2(84f, 0)))
            {
                _dialogs.OpenFileDialog("Import a look, or an image with one embedded",
                    "Look or image{.json,.png}", (ok, path) =>
                    {
                        if (!ok || string.IsNullOrWhiteSpace(path)) return;
                        PushUndo(cfg);
                        if (cfg.LoadFromBase) cfg.ResetLook();
                        if (LookStore.LoadFromFile(path, cfg, (LookStore.Part)_applyPart, out var impErr))
                        {
                            _dirty = true;
                            _status = $"Imported from \u2018{Path.GetFileName(path)}\u2019. Save it to keep it.";
                        }
                        else _status = impErr;
                    });
            }
            if (ImGui.IsItemHovered()) ImGui.SetTooltip(
                "Load a .json look, or a .png exported with its look inside it.\n" +
                "Applies the same slice as clicking a look does, so set that first.\n" +
                "Nothing is saved until you name it and press Save.");

            ImGui.SameLine();
            using (ImRaii.Disabled(!has))
                if (ImGui.Button("Export\u2026", new Vector2(84f, 0)))
                {
                    var picked = _lookSel;
                    _dialogs.SaveFileDialog("Export the selected look", ".json", picked, ".json",
                        (ok, path) =>
                        {
                            if (!ok || string.IsNullOrWhiteSpace(path)) return;
                            if (LookStore.SaveToFile(path, cfg, out var expErr))
                                _status = $"Exported to \u2018{Path.GetFileName(path)}\u2019.";
                            else _status = expErr;
                        });
                }
            if (ImGui.IsItemHovered()) ImGui.SetTooltip(
                "Write the current settings to a file you can send to someone.\n" +
                "Element-layer image paths are left out: they would not resolve\non another machine, and they name folders on this one.");
            ImGui.SameLine();
            if (ImGui.Button("Folder", new Vector2(84f, 0))) OpenFolder(LookStore.FolderPath);

            ImGui.Spacing();
            ImGui.TextDisabled("Clicking a look loads");
            ImGui.SameLine();
            ImGui.PushItemWidth(170f);
            if (ImGui.BeginCombo("##applypart", UiApplyPart[Math.Clamp(_applyPart, 0, UiApplyPart.Length - 1)]))
            {
                for (int k = 0; k < UiApplyPart.Length; k++)
                    if (ImGui.Selectable(UiApplyPart[k], _applyPart == k)) _applyPart = k;
                ImGui.EndCombo();
            }
            ImGui.PopItemWidth();
            if (_applyPart != 0)
            {
                ImGui.SameLine();
                if (ImGui.SmallButton("All##applyreset")) _applyPart = 0;
            }

            bool fromBase = cfg.LoadFromBase;
            if (ImGui.Checkbox("Clear everything else first", ref fromBase)) { cfg.LoadFromBase = fromBase; _dirty = true; }
            if (ImGui.IsItemHovered()) ImGui.SetTooltip(
                "Put every setting back to its default before the look is applied, so what\n" +
                "you get is the look and nothing inherited from the last one.\n\n" +
                "Changes nothing for a whole look, which already overwrites everything it\n" +
                "mentions. It is for the ones that mention less: a saved part, or a file\n" +
                "written before half these settings existed \u2014 those leave the rest as they\n" +
                "found it, which is how a look ends up being the look plus a leftover.\n\n" +
                "Element images and text captions are content, not settings, and survive it.");

            ImGui.TextDisabled("Pressing Save writes");
            ImGui.SameLine();
            ImGui.PushItemWidth(170f);
            if (ImGui.BeginCombo("##savepart", UiApplyPart[Math.Clamp(_savePart, 0, UiApplyPart.Length - 1)]))
            {
                for (int k = 0; k < UiApplyPart.Length; k++)
                    if (ImGui.Selectable(UiApplyPart[k], _savePart == k)) _savePart = k;
                ImGui.EndCombo();
            }
            ImGui.PopItemWidth();
            if (_savePart != 0)
            {
                ImGui.SameLine();
                if (ImGui.SmallButton("All##savereset")) _savePart = 0;
            }
            ImGui.TextDisabled("Saving one part is how a grade or a gobo setup becomes reusable on its\nown: the file holds fewer keys, so loading it leaves the rest alone.");
            ImGui.Unindent(10f);
        }

        if (ImGui.CollapsingHeader("New here? Start with this"))
        {
            ImGui.Indent(10f);
            ImGui.TextWrapped(
                "1.  Click a look below. That loads it, and the other tabs are then just " +
                "its settings for you to nudge.");
            ImGui.TextWrapped(
                "2.  Tick Live preview at the top. It is a still frame, not live video, so " +
                "set your pose and camera first and then preview.");
            ImGui.TextWrapped(
                "3.  Purple, black, or upside down? That is a capture quirk, not you. " +
                "Export tab, Capture fix-ups: untick Swap red/blue and Flip vertically. " +
                "Set once, kept forever.");
            ImGui.TextWrapped(
                "4.  Bypass at the top shows the same frame without effects, for a quick " +
                "before and after.");
            ImGui.TextWrapped(
                "5.  The F / C / B buttons on an effect choose what it touches: Foreground, " +
                "Character, Background. The green letters beside them limit it to a mask.");
            ImGui.Spacing();
            ImGui.TextWrapped(
                "The one control worth knowing early is Start (depth) in the Background tab. " +
                "It decides how far away counts as background. Set too far it selects nothing " +
                "and the backdrop never appears, which looks broken but is not \u2014 tick " +
                "Show what this covers beside it and lower the value until the wall behind " +
                "your subject turns magenta.");
            ImGui.Spacing();
            ImGui.TextWrapped(
                "The search box at the very top of the window searches every control in " +
                "every tab at once.");
            ImGui.Unindent(10f);
        }

        ImGui.PushItemWidth(-150f);
        var f = _lookFilter;
        if (ImGui.InputTextWithHint("##lookfilter", "Search looks\u2026", ref f, 64)) _lookFilter = f;
        ImGui.PopItemWidth();
        ImGui.SameLine();
        using (ImRaii.Disabled(_lookFilter.Length == 0))
            if (ImGui.Button("Clear", new Vector2(60f, 0))) { _lookFilter = ""; _confirmDelete = ""; }
        ImGui.SameLine();
        ImGui.TextDisabled($"{_lookList.Count}");
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Looks saved on this machine.\nRight-click any of them to pin it to the top.");

        bool Match(string n)
        {
            if (_lookFilter.Length == 0) return true;
            if (n.Contains(_lookFilter, StringComparison.OrdinalIgnoreCase)) return true;
            if (cat.TryGetValue(n, out var c1) && c1.Contains(_lookFilter, StringComparison.OrdinalIgnoreCase)) return true;
            return legacyCat.TryGetValue(n, out var c2) && c2.Contains(_lookFilter, StringComparison.OrdinalIgnoreCase);
        }

        void Row(string n)
        {
            bool pinned = cfg.PinnedLooks.Contains(n);
            if (ImGui.Selectable((pinned ? "\u2605 " : "") + n, _lookSel == n))
            {
                _confirmDelete = "";
                _lookSel = n;
                _pendingLoad = n;
            }
            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
                if (pinned) cfg.PinnedLooks.Remove(n); else cfg.PinnedLooks.Add(n);
                _dirty = true;
            }
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(pinned ? "Right-click: unpin" : "Right-click: pin to the top");
        }

        void Group(string title, IEnumerable<string> names)
        {
            var list = names.ToList();
            if (list.Count == 0) return;
            ImGui.TextColored(AccentCol, title);
            ImGui.Indent(10f);
            foreach (var n in list) Row(n);
            ImGui.Unindent(10f);
            ImGui.Spacing();
        }

        var shown = _lookList.Where(Match).ToList();
        Group("\u2605 Pinned", shown.Where(n => cfg.PinnedLooks.Contains(n)));
        var rest = shown.Where(n => !cfg.PinnedLooks.Contains(n)).ToList();

        foreach (var g in rest.Where(n => cat.ContainsKey(n))
                              .GroupBy(n => cat[n]).OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase))
            Group(g.Key, g);

        Group("Yours", rest.Where(n => !cat.ContainsKey(n) && !legacyCat.ContainsKey(n)));

        var legacyHere = rest.Where(n => legacyCat.ContainsKey(n)).ToList();
        var legacyMissing = LookStore.Legacy.Where(l => !_lookList.Contains(l.Name)).ToList();
        if (legacyHere.Count > 0 || legacyMissing.Count > 0)
        {
            if (ImGui.CollapsingHeader($"Earlier looks ({legacyHere.Count + legacyMissing.Count})###earlier"))
            {
                ImGui.Indent(10f);
                ImGui.TextDisabled(
                    "The set this shipped with before it was rebuilt. The ones you have are\n" +
                    "ordinary looks of yours now: never refreshed, never overwritten again.");
                ImGui.Spacing();
                foreach (var g in legacyHere.GroupBy(n => legacyCat[n]).OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase))
                {
                    ImGui.TextDisabled(g.Key);
                    ImGui.Indent(10f);
                    foreach (var n in g) Row(n);
                    ImGui.Unindent(10f);
                }
                if (legacyMissing.Count > 0)
                {
                    ImGui.Spacing();
                    ImGui.TextDisabled("Not installed here");
                    ImGui.Indent(10f);
                    foreach (var (name, _, _) in legacyMissing)
                    {
                        ImGui.TextUnformatted(name);
                        ImGui.SameLine(240f);
                        if (ImGui.SmallButton("Install##lg" + name))
                        {
                            if (LookStore.InstallLegacy(name, out var lgErr))
                            {
                                _lookList = LookStore.List();
                                _status = $"Added \u2018{name}\u2019 to your looks.";
                            }
                            else _status = lgErr;
                        }
                    }
                    ImGui.Unindent(10f);
                }
                ImGui.Unindent(10f);
            }
        }

        if (shown.Count == 0)
            ImGui.TextDisabled(_lookList.Count == 0 ? "No looks yet." : "Nothing matches that search.");

        if (_pendingLoad.Length > 0)
        {
            var n = _pendingLoad;
            _pendingLoad = "";
            var part = (LookStore.Part)_applyPart;
            if (cfg.LoadFromBase) cfg.ResetLook();
            if (LookStore.Load(n, cfg, part, out int applied))
            {
                if (applied == 0)
                {
                    _status = $"\u2018{n}\u2019 has nothing in the {UiApplyPart[_applyPart]} part.";
                }
                else
                {
                    _dirty = true;
                    if (part == LookStore.Part.All)
                    {
                        _loadedSnapshot = LookStore.Capture(cfg);
                        _editedSinceLoad = false;
                    }
                    _status = part == LookStore.Part.All
                        ? $"Loaded \u2018{n}\u2019."
                        : $"Loaded the {UiApplyPart[_applyPart].ToLowerInvariant()} from \u2018{n}\u2019.";
                }
            }
            else _status = $"Could not load \u2018{n}\u2019.";
        }

    }

    private void GateToggle(PluginConfig cfg, string id)
    {
        if (!ReferenceEquals(cfg, Plugin.Config)) return;
        var g = Plugin.Config.DebugShowGate;
        if (ImGui.Checkbox("Show what this covers##" + id, ref g))
        {
            Plugin.Config.DebugShowGate = g;
            if (g) { Plugin.Config.DebugShowDepth = false; Plugin.Config.DebugShowClipping = false; Plugin.Config.DebugShowMatte = false; Plugin.Config.DebugShowMask = false; }
            _dirty = true;
        }
        if (ImGui.IsItemHovered()) ImGui.SetTooltip(
            "Paints the frame by what the depth gate selects:\n" +
            "  magenta = the backdrop reaches here\n" +
            "  cyan    = the solid fill reaches here\n\n" +
            "If nothing is tinted, Start (depth) is set beyond everything in the scene —\n" +
            "lower it until the wall behind your subject lights up.");
        ImGui.SameLine();
        var d = Plugin.Config.DebugShowDepth;
        if (ImGui.Checkbox("Raw depth##" + id, ref d))
        {
            Plugin.Config.DebugShowDepth = d;
            if (d) { Plugin.Config.DebugShowGate = false; Plugin.Config.DebugShowClipping = false; Plugin.Config.DebugShowMatte = false; Plugin.Config.DebugShowMask = false; }
            _dirty = true;
        }
        if (ImGui.IsItemHovered()) ImGui.SetTooltip(
            "The depth buffer itself, near black to far white.\n" +
            "Read a value off it to know what to set Start (depth) to.\n" +
            "Red means the game gave us no depth this frame.");
        if (Plugin.Config.DebugShowGate || Plugin.Config.DebugShowDepth)
            ImGui.TextColored(new Vector4(1f, 0.62f, 0.25f, 1f), "Depth preview is on — untick to see the shot.");
        if (Plugin.Config.DebugShowGate)
        {
            var pc = Plugin.Config;
            bool anyBackdrop = pc.EnBackdrop && pc.BgRecolor > 0f && pc.BgStyle > 0;
            bool anyFill = pc.EnBgFill && pc.BgFill > 0f;
            if (!anyBackdrop && !anyFill)
                ImGui.TextColored(new Vector4(1f, 0.62f, 0.25f, 1f),
                    "Nothing to tint: pick a Background style, or raise the fill's Opacity.");
        }
    }

    private void PushUndo(PluginConfig cfg)
    {
        var now = LookStore.Capture(cfg);
        _editedSinceLoad = _loadedSnapshot.Length > 0
                           && !string.Equals(now, _loadedSnapshot, StringComparison.Ordinal);
        if (_baseline.Length == 0) { _baseline = now; return; }
        if (string.Equals(now, _baseline, StringComparison.Ordinal)) return;
        _undo.Add(_baseline);
        if (_undo.Count > UndoDepth) _undo.RemoveAt(0);
        _redo.Clear();
        _baseline = now;
    }

    private void Undo(PluginConfig cfg)
    {
        if (_undo.Count == 0) { _status = "Nothing to undo."; return; }
        _redo.Add(LookStore.Capture(cfg));
        var j = _undo[_undo.Count - 1];
        _undo.RemoveAt(_undo.Count - 1);
        LookStore.Apply(j, cfg, LookStore.Part.All);
        _baseline = j;
        _savePending = true;
        _status = $"Undo ({_undo.Count} left).";
    }

    private void Redo(PluginConfig cfg)
    {
        if (_redo.Count == 0) { _status = "Nothing to redo."; return; }
        _undo.Add(LookStore.Capture(cfg));
        var j = _redo[_redo.Count - 1];
        _redo.RemoveAt(_redo.Count - 1);
        LookStore.Apply(j, cfg, LookStore.Part.All);
        _baseline = j;
        _savePending = true;
        _status = "Redo.";
    }

    private static bool FinalGradeActive(PluginConfig c) =>
        c.FinalExposure != 0f || c.FinalContrast != 0f || c.FinalSat != 0f || c.FinalTemp != 0f
        || c.FinalLift != 0f || c.FinalGamma != 0f || c.FinalGain != 0f;

    private void OpenOutputFolder() => OpenFolder(Plugin.Config.OutputDirectory);

    private void OpenFolder(string path)
    {
        try
        {
            Directory.CreateDirectory(path);
            Process.Start(new ProcessStartInfo { FileName = path, UseShellExecute = true });
        }
        catch (Exception ex)
        {
            Services.Log.Error(ex, "OpenFolder failed");
            _status = $"Could not open folder: {ex.Message}";
        }
    }

    public void Dispose()
    {
        if (_savePending) Plugin.Config.Save();
    }
}

