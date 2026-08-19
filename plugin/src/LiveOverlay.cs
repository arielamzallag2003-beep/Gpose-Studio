using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Plugin.Services;
using Dalamud.Bindings.ImGui;

namespace GPoseStudio;

public sealed class LiveOverlay : IDisposable
{
    private readonly GposeGate _gate;
    private GpuRenderer? _gpu;
    private bool _gpuFailed;
    private IDalamudTextureWrap? _capture;
    private IDalamudTextureWrap? _incoming;
    private bool _capturing;
    private long _nextCaptureTick;

    private long _captureStartedAt;
    private const long CaptureStuckMs = 1500;
    private long _lastStuckLog;

    private int _framesSinceRender;
    private const int RevalidateEveryFrames = 30;
    private bool _disposed;
    private readonly System.Diagnostics.Stopwatch _animClock = System.Diagnostics.Stopwatch.StartNew();
    private readonly nint[] _lastMemeSrvs = new nint[8];
    private readonly IDalamudTextureWrap?[] _memeWraps = new IDalamudTextureWrap?[8];
    private bool _loggedDepth;
    private GpuRenderer.Params _lastParams;
    private nint _lastOutSrv;
    private nint _lastDepthSrv;

    private nint _depthSeenSrv;
    private int _depthSettled;
    private const int DepthSettleFrames = 3;
    private bool _loggedDepthSettle;
    private bool _haveRender;
    private bool _captureChanged;
    private GposePanel.Rect[] _gposeRects = Array.Empty<GposePanel.Rect>();

    private volatile bool _exportPending;
    private string _exportDir = "";
    private Action<string>? _exportDone;

    private const long CaptureIntervalMs = 100;

    public bool Enabled { get; set; }

    public bool DepthAvailable { get; private set; }

    public LiveOverlay(GposeGate gate)
    {
        _gate = gate;
        Services.PluginInterface.UiBuilder.Draw += OnDraw;
        Services.Framework.Update += OnFrameworkUpdate;
    }

    private void OnFrameworkUpdate(IFramework _)
    {
        if (_disposed) return;
        _gposeRects = (Enabled && _gate.IsActive)
            ? GposePanel.GetRects()
            : Array.Empty<GposePanel.Rect>();
    }

    public void RequestExport(string dir, Action<string> onDone)
    {
        _exportDir = dir;
        _exportDone = onDone;
        _exportPending = true;
    }

    private void OnDraw()
    {
        if (_disposed) return;

        DrawGuides(Plugin.Config);
        DrawTexts(Plugin.Config);

        bool active = _gate.IsActive && !_gpuFailed;
        bool showLive = Enabled;
        bool want = active && (showLive || _exportPending);
        if (!want)
        {
            if (_exportPending) { _exportPending = false; _exportDone?.Invoke("error: enter GPose to export"); }
            Teardown();
            return;
        }

        try
        {
            _gpu ??= new GpuRenderer(Services.PluginInterface.UiBuilder.DeviceHandle);

            var incoming = Interlocked.Exchange(ref _incoming, null);
            if (incoming != null)
            {
                _capture?.Dispose();
                _capture = incoming;
                _captureChanged = true;
            }

            bool captureStuck = _capturing && Environment.TickCount64 - _captureStartedAt > CaptureStuckMs;
            if ((!_capturing || captureStuck)
                && (Environment.TickCount64 >= _nextCaptureTick || (_exportPending && _capture is null)))
            {
                if (captureStuck && Environment.TickCount64 - _lastStuckLog > 5000)
                {
                    _lastStuckLog = Environment.TickCount64;
                    Services.Log.Warning(
                        "capture did not return within {0} ms; starting another so the frame keeps refreshing.",
                        CaptureStuckMs);
                }
                _capturing = true;
                _captureStartedAt = Environment.TickCount64;
                StartCapture(ImGui.GetMainViewport().ID);
            }

            if (_capture is null) return;

            var srcPtr = (nint)_capture.Handle.Handle;
            int w = _capture.Width, h = _capture.Height;
            if (srcPtr == 0 || w <= 0 || h <= 0) return;

            var depth = DepthBuffer.TryGet();
            DepthAvailable = depth.Srv != 0;

            if (depth.Srv != _depthSeenSrv)
            {
                _depthSeenSrv = depth.Srv;
                _depthSettled = 0;
            }
            else if (_depthSettled < DepthSettleFrames)
            {
                _depthSettled++;
            }
            bool depthTrusted = depth.Srv != 0 && _depthSettled >= DepthSettleFrames;

            nint depthSrv = depthTrusted ? depth.Srv : 0;
            if (!depthTrusted && depth.Srv != 0 && !_loggedDepthSettle)
            {
                _loggedDepthSettle = true;
                Services.Log.Debug("depth view changed; skipping depth effects until it settles.");
            }
            if (!_loggedDepth)
            {
                _loggedDepth = true;
                Services.Log.Info(depth.Srv != 0
                    ? $"Scene depth SRV acquired: 0x{depth.Srv:X}, uvScale=({depth.ScaleX:0.###},{depth.ScaleY:0.###})"
                    : "Scene depth SRV not available (game exposes no depth SRV) — fog disabled.");
            }

            var cfg = Plugin.Config;

            var memeSrvs = new nint[8];
            bool memeChanged = false;
            var images = cfg.ElemImages;
            for (int L = 0; L < 8; L++)
            {
                nint mh = 0;
                IDalamudTextureWrap? wrap = null;
                string path = images != null && L < images.Length ? (images[L] ?? "") : "";
                if (path.Length > 0)
                {
                    try
                    {
                        wrap = Services.TextureProvider.GetFromFile(path).GetWrapOrDefault();
                        if (wrap != null) { nint hh = (nint)wrap.Handle.Handle; if (hh != 0) mh = hh; else wrap = null; }
                    }
                    catch { wrap = null; mh = 0; }
                }
                _memeWraps[L] = wrap;
                memeSrvs[L] = mh;
                if (mh != _lastMemeSrvs[L]) memeChanged = true;
            }

            var p = new GpuRenderer.Params
            {
                Exposure = cfg.Exposure,
                Contrast = cfg.Contrast,
                Saturation = cfg.Saturation,
                Temperature = cfg.Temperature,
                Tint = cfg.Tint,
                Lift = cfg.Lift,
                Gamma = cfg.Gamma,
                Gain = cfg.Gain,
                Vibrance = cfg.Vibrance,
                Vignette = cfg.Vignette,
                Sharpen = cfg.Sharpen,
                Chroma = cfg.Chroma,
                Grain = cfg.Grain,
                Letterbox = cfg.Letterbox,
                SwapRedBlue = cfg.SwapRedBlue ? 1 : 0,
                Flip = cfg.FlipVertical ? 1 : 0,
                FogStart = cfg.FogStart,
                FogStrength = cfg.FogStrength,
                FogColorR = cfg.FogColorR,
                FogColorG = cfg.FogColorG,
                FogColorB = cfg.FogColorB,
                BgPushStart = cfg.BgPushStart,
                BgPushStrength = cfg.BgPushStrength,
                DofFocus = cfg.DofFocus,
                DofRange = cfg.DofRange,
                DofStrength = cfg.DofStrength,
                DepthUvScaleX = depth.ScaleX,
                DepthUvScaleY = depth.ScaleY,
                TexelX = 1f / w,
                TexelY = 1f / h,
                HasDepth = depthTrusted ? 1 : 0,
                DebugView = cfg.DebugShowGate ? 2 : (cfg.DebugShowDepth ? 1 : (cfg.DebugShowClipping ? 3 : (cfg.DebugShowMatte && cfg.ExportTransparent ? 4 : 0))),
                BlackPoint = cfg.BlackPoint,
                WhitePoint = cfg.WhitePoint,
                HueShift = cfg.HueShift,
                Bleach = cfg.Bleach,
                BleachContrast = cfg.BleachContrast,
                TealOrange = cfg.TealOrange,
                TealOrangePunch = cfg.TealOrangePunch,
                ToShadowR = cfg.ToShadowR, ToShadowG = cfg.ToShadowG, ToShadowB = cfg.ToShadowB,
                ToHighR = cfg.ToHighR, ToHighG = cfg.ToHighG, ToHighB = cfg.ToHighB,
                ColorBalance = cfg.ColorBalance,
                CbShadowR = cfg.CbShadowR, CbShadowG = cfg.CbShadowG, CbShadowB = cfg.CbShadowB,
                CbMidR = cfg.CbMidR, CbMidG = cfg.CbMidG, CbMidB = cfg.CbMidB,
                CbHighR = cfg.CbHighR, CbHighG = cfg.CbHighG, CbHighB = cfg.CbHighB,
                FisheyeAmt = cfg.FisheyeAmt,
                FisheyeZoom = cfg.FisheyeZoom,
                SwirlAmt = cfg.SwirlAmt,
                SwirlRadius = cfg.SwirlRadius,
                MosaicSize = cfg.MosaicSize,
                KaleidoSegs = cfg.KaleidoSegs,
                KaleidoRot = cfg.KaleidoRot,
                BloomAmount = cfg.BloomAmount,
                BloomThreshold = cfg.BloomThreshold,
                BloomRadius = cfg.BloomRadius,
                Halation = cfg.Halation,
                HalationR = cfg.HalationR, HalationG = cfg.HalationG, HalationB = cfg.HalationB,
                GodrayAmount = cfg.GodrayAmount,
                GodrayLightX = cfg.GodrayLightX,
                GodrayLightY = cfg.GodrayLightY,
                GodrayDecay = cfg.GodrayDecay,
                GodrayThreshold = cfg.GodrayThreshold,
                GodrayR = cfg.GodrayR, GodrayG = cfg.GodrayG, GodrayB = cfg.GodrayB,
                RimStrength = cfg.RimStrength, RimThreshold = cfg.RimThreshold, RimWidth = cfg.RimWidth,
                RimR = cfg.RimR, RimG = cfg.RimG, RimB = cfg.RimB,
                BgRecolor = cfg.BgRecolor, BgRecolorStart = cfg.BgRecolorStart, BgRecolorFeather = cfg.BgRecolorFeather,
                BgTopR = cfg.BgTopR, BgTopG = cfg.BgTopG, BgTopB = cfg.BgTopB,
                BgBotR = cfg.BgBotR, BgBotG = cfg.BgBotG, BgBotB = cfg.BgBotB,
                BgBlur = cfg.BgBlur, BgBlurStart = cfg.BgBlurStart,
                Orton = cfg.Orton, Glamour = cfg.Glamour, GlamourMist = cfg.GlamourMist,
                SoftBlurRadius = cfg.SoftBlurRadius,
                GradMap = cfg.GradMap,
                GmShadowR = cfg.GmShadowR, GmShadowG = cfg.GmShadowG, GmShadowB = cfg.GmShadowB,
                GmMidR = cfg.GmMidR, GmMidG = cfg.GmMidG, GmMidB = cfg.GmMidB,
                GmHighR = cfg.GmHighR, GmHighG = cfg.GmHighG, GmHighB = cfg.GmHighB,
                Dehaze = cfg.Dehaze,
                WaveAmt = cfg.WaveAmt, WaveFreq = cfg.WaveFreq, WavePhase = cfg.WavePhase,
                GlitchAmt = cfg.GlitchAmt, GlitchBlocks = cfg.GlitchBlocks,
                StShadowR = cfg.StShadowR, StShadowG = cfg.StShadowG, StShadowB = cfg.StShadowB,
                StHighR = cfg.StHighR, StHighG = cfg.StHighG, StHighB = cfg.StHighB,
                StBalance = cfg.StBalance, StAmount = cfg.StAmount,
                Clarity = cfg.Clarity,
                TiltAmt = cfg.TiltAmt, TiltFocus = cfg.TiltFocus, TiltRange = cfg.TiltRange,
                FlowAmt = cfg.FlowAmt, FlowScale = cfg.FlowScale, FlowSeed = cfg.FlowSeed,
                ScopeMode = cfg.ScopeMode, ScopeSplit = cfg.ScopeSplit, ScopeSoft = cfg.ScopeSoft,
                EdgeAura = cfg.EdgeAura, EdgeWidth = cfg.EdgeWidth, EdgeThreshold = cfg.EdgeThreshold,
                EdgeR = cfg.EdgeR, EdgeG = cfg.EdgeG, EdgeB = cfg.EdgeB,
                Iridescent = cfg.Iridescent, IridFreq = cfg.IridFreq, IridShift = cfg.IridShift,
                Prism = cfg.Prism,
                LeakAmt = cfg.LeakAmt, LeakAngle = cfg.LeakAngle,
                LeakR = cfg.LeakR, LeakG = cfg.LeakG, LeakB = cfg.LeakB,
                AnamAmount = cfg.AnamAmount, AnamThreshold = cfg.AnamThreshold, AnamLength = cfg.AnamLength,
                AnamR = cfg.AnamR, AnamG = cfg.AnamG, AnamB = cfg.AnamB,
                HlRecovery = cfg.HlRecovery, SubjectPop = cfg.SubjectPop,
                HaloAmount = cfg.HaloAmount, HaloSplit = cfg.HaloSplit,
                HaloR = cfg.HaloR, HaloG = cfg.HaloG, HaloB = cfg.HaloB,
                FrostAmount = cfg.FrostAmount, FrostCoverage = cfg.FrostCoverage, FrostFeather = cfg.FrostFeather,
                WashAmount = cfg.WashAmount, WashX = cfg.WashX, WashY = cfg.WashY,
                WashR = cfg.WashR, WashG = cfg.WashG, WashB = cfg.WashB,
                CausticsAmt = cfg.CausticsAmt, CausticsScale = cfg.CausticsScale,
                CausticsR = cfg.CausticsR, CausticsG = cfg.CausticsG, CausticsB = cfg.CausticsB,
                ChromaClean = cfg.ChromaClean, Denoise = cfg.Denoise, DenoiseEdge = cfg.DenoiseEdge,
                KuwaharaAmt = cfg.KuwaharaAmt, KuwaharaRadius = cfg.KuwaharaRadius,
                BgFill = cfg.BgFill, BgFillStart = cfg.BgFillStart, BgFillFeather = cfg.BgFillFeather,
                BgFillR = cfg.BgFillR, BgFillG = cfg.BgFillG, BgFillB = cfg.BgFillB,
                ShadowAmount = cfg.ShadowAmount, ShadowSpread = cfg.ShadowSpread, ShadowOffsetX = cfg.ShadowOffsetX, ShadowOffsetY = cfg.ShadowOffsetY,
                ShadowSoftness = cfg.ShadowSoftness, ShadowR = cfg.ShadowR, ShadowG = cfg.ShadowG, ShadowB = cfg.ShadowB,
                ShadowContact = cfg.ShadowContact, ShadowDepth = cfg.ShadowDepth,
                EdgeErode = cfg.EdgeErode, EdgeDespill = cfg.EdgeDespill, EdgeWrap = cfg.EdgeWrap, EdgeWrapWidth = cfg.EdgeWrapWidth,
                FilmRolloff = cfg.FilmRolloff, FilmToe = cfg.FilmToe, FilmSat = cfg.FilmSat,
                LensVig = cfg.LensVig, LensCornerSoft = cfg.LensCornerSoft, ChromaRadial = cfg.ChromaRadial,
                BackdropLightAmt = cfg.BackdropLightAmt, BackdropLightX = cfg.BackdropLightX,
                BackdropLightY = cfg.BackdropLightY, BackdropLightSize = cfg.BackdropLightSize,
                ZoneNear = cfg.ZoneNear, ZoneNearSoft = cfg.ZoneNearSoft, ZoneWet = cfg.ZoneWet, ZoneBeauty = cfg.ZoneBeauty,
                ZoneSkin = cfg.ZoneSkin, ZoneBacklight = cfg.ZoneBacklight, ZoneShadow = cfg.ZoneShadow, ZoneBokeh = cfg.ZoneBokeh,
                ZoneBgPush = cfg.ZoneBgPush, ZoneBgBlur = cfg.ZoneBgBlur,
                ZoneGobo = cfg.ZoneGobo, ZoneSpot = cfg.ZoneSpot, ZoneFrost = cfg.ZoneFrost, ZoneStylize = cfg.ZoneStylize,
                ZoneUnderwater = cfg.ZoneUnderwater, ZoneVhs = cfg.ZoneVhs, ZoneRim = cfg.ZoneRim, ZoneGround = cfg.ZoneGround,
                ZoneHalo = cfg.ZoneHalo, ZoneCb = cfg.ZoneCb, ZoneTeal = cfg.ZoneTeal, ZoneSplitTone = cfg.ZoneSplitTone,
                ZoneBleach = cfg.ZoneBleach, ZoneGradMap = cfg.ZoneGradMap,
                RimSplit = cfg.RimSplit, RimSplitAngle = cfg.RimSplitAngle,
                RimSplitOffset = cfg.RimSplitOffset, RimSplitSoft = cfg.RimSplitSoft,
                Rim2R = cfg.Rim2R, Rim2G = cfg.Rim2G, Rim2B = cfg.Rim2B,
                Backlight2R = cfg.Backlight2R, Backlight2G = cfg.Backlight2G, Backlight2B = cfg.Backlight2B,
                PatMat = cfg.PatMat, PatMatR = cfg.PatMatR, PatMatG = cfg.PatMatG, PatMatB = cfg.PatMatB,
                PatMatRough = cfg.PatMatRough, PatMatSheen = cfg.PatMatSheen,
                PatMatPos = cfg.PatMatPos, PatMatRange = cfg.PatMatRange,
                PatColOverride = cfg.PatColOverride ? 1 : 0,
                PatColR = cfg.PatColR, PatColG = cfg.PatColG, PatColB = cfg.PatColB,
                PatColMode = cfg.PatColMode, PatCol2R = cfg.PatCol2R, PatCol2G = cfg.PatCol2G, PatCol2B = cfg.PatCol2B,
                PatMatTint = cfg.PatMatTint,
                Cutout = 0,
                CutoutFeather = cfg.CutoutFeather, CutoutShrink = cfg.CutoutShrink,
                EnFinal = cfg.EnFinalGrade ? 1 : 0, FinalExposure = cfg.FinalExposure,
                FinalContrast = cfg.FinalContrast, FinalSat = cfg.FinalSat,
                FinalTemp = cfg.FinalTemp, FinalLift = cfg.FinalLift,
                FinalGamma = cfg.FinalGamma, FinalGain = cfg.FinalGain,
                GroundMode = cfg.GroundMode, GroundCastAngle = cfg.GroundCastAngle,
                GroundCastLen = cfg.GroundCastLen,
                BgBPatColOverride = cfg.BgBPatColOverride ? 1 : 0, BgBPatColMode = cfg.BgBPatColMode, BgBPatColR = cfg.BgBPatColR, BgBPatColG = cfg.BgBPatColG,
                BgBPatColB = cfg.BgBPatColB, BgBPatCol2R = cfg.BgBPatCol2R, BgBPatCol2G = cfg.BgBPatCol2G, BgBPatCol2B = cfg.BgBPatCol2B,
                BgBPatCol3R = cfg.BgBPatCol3R, BgBPatCol3G = cfg.BgBPatCol3G, BgBPatCol3B = cfg.BgBPatCol3B, BgBPatCol4R = cfg.BgBPatCol4R,
                BgBPatCol4G = cfg.BgBPatCol4G, BgBPatCol4B = cfg.BgBPatCol4B, BgBPatCol5R = cfg.BgBPatCol5R, BgBPatCol5G = cfg.BgBPatCol5G,
                BgBPatCol5B = cfg.BgBPatCol5B, BgBPatMat = cfg.BgBPatMat, BgBPatMatR = cfg.BgBPatMatR, BgBPatMatG = cfg.BgBPatMatG,
                BgBPatMatB = cfg.BgBPatMatB, BgBPatMatTint = cfg.BgBPatMatTint,
                PatCol3R = cfg.PatCol3R, PatCol3G = cfg.PatCol3G, PatCol3B = cfg.PatCol3B,
                PatCol4R = cfg.PatCol4R, PatCol4G = cfg.PatCol4G, PatCol4B = cfg.PatCol4B,
                PatCol5R = cfg.PatCol5R, PatCol5G = cfg.PatCol5G, PatCol5B = cfg.PatCol5B,
                BgStyle = cfg.BgStyle, BgScale = cfg.BgScale, BgAngle = cfg.BgAngle, BgGrain = cfg.BgGrain,
                BgWarp = cfg.BgWarp, BgWarpAmt = cfg.BgWarpAmt, BgWarpScale = cfg.BgWarpScale,
                BgWarpAmt2 = cfg.BgWarpAmt2, BgWarpScale2 = cfg.BgWarpScale2,
                BgWarpX = cfg.BgWarpX, BgWarpY = cfg.BgWarpY,
                BgOffX = cfg.BgOffX, BgOffY = cfg.BgOffY, BgScaleY = cfg.BgScaleY, BgSharp = cfg.BgSharp,
                BgMidR = cfg.BgMidR, BgMidG = cfg.BgMidG, BgMidB = cfg.BgMidB,
                BgMetallic = cfg.BgMetallic, BgRoughness = cfg.BgRoughness, BgSpecular = cfg.BgSpecular,
                BgNormal = cfg.BgNormal, BgFresnel = cfg.BgFresnel,
                BgLightX = cfg.BgLightX, BgLightY = cfg.BgLightY, BgLightZ = cfg.BgLightZ, BgLightInt = cfg.BgLightInt,
                BgCol4R = cfg.BgCol4R, BgCol4G = cfg.BgCol4G, BgCol4B = cfg.BgCol4B, BgFbm = cfg.BgFbm,
                BgStars = cfg.BgStars, BgStarDensity = cfg.BgStarDensity, BgStarSize = cfg.BgStarSize, BgGlow = cfg.BgGlow,
                BgVignette = cfg.BgVignette, BgVignetteSize = cfg.BgVignetteSize, BgHueVar = cfg.BgHueVar, BgBright = cfg.BgBright,
                BgNebWarp = cfg.BgNebWarp, BgNebContrast = cfg.BgNebContrast, BgVoidCore = cfg.BgVoidCore, BgVoidRing = cfg.BgVoidRing,
                BgTwist = cfg.BgTwist, BgHaze = cfg.BgHaze, BgSparkle = cfg.BgSparkle, BgDisperse = cfg.BgDisperse,
                BgRingWidth = cfg.BgRingWidth, BgRing2 = cfg.BgRing2, BgEmbers = cfg.BgEmbers, BgFlow = cfg.BgFlow,
                BgCol5R = cfg.BgCol5R, BgCol5G = cfg.BgCol5G, BgCol5B = cfg.BgCol5B,
                BgCol6R = cfg.BgCol6R, BgCol6G = cfg.BgCol6G, BgCol6B = cfg.BgCol6B, BgEmberSize = cfg.BgEmberSize,
                BgPad0 = cfg.BgKeepVfx,
                VhsStatic = cfg.VhsStatic, VhsScan = cfg.VhsScan, VhsScanCount = cfg.VhsScanCount, VhsDropout = cfg.VhsDropout,
                VhsRoll = cfg.VhsRoll, VhsRollPos = cfg.VhsRollPos, VhsDesat = cfg.VhsDesat, VhsVignette = cfg.VhsVignette,
                BgReflect = cfg.BgReflect, BgMatDisp = cfg.BgMatDisp, BgAniso = cfg.BgAniso, BgEnvSharp = cfg.BgEnvSharp,
                BgEnvR = cfg.BgEnvR, BgEnvG = cfg.BgEnvG, BgEnvB = cfg.BgEnvB, BgClearcoat = cfg.BgClearcoat,
                BgCausticAmt = cfg.BgCausticAmt, BgShafts = cfg.BgShafts, BgBubbles = cfg.BgBubbles,
                UwTint = cfg.UwTint, UwTintR = cfg.UwTintR, UwTintG = cfg.UwTintG, UwTintB = cfg.UwTintB,
                UwCaustic = cfg.UwCaustic, UwMotes = cfg.UwMotes, UwShafts = cfg.UwShafts, UwFog = cfg.UwFog,
                GroundLevel = cfg.GroundLevel, GroundShadow = cfg.GroundShadow, GroundRipple = cfg.GroundRipple,
                GroundTintR = cfg.GroundTintR, GroundTintG = cfg.GroundTintG, GroundTintB = cfg.GroundTintB,
                GroundShadowX = cfg.GroundShadowX, GroundShadowY = cfg.GroundShadowY, GroundShadowW = cfg.GroundShadowW, GroundShadowH = cfg.GroundShadowH,
                BgGradType = cfg.BgGradType, BgPatMode = cfg.BgPatMode, BgPatStrength = cfg.BgPatStrength, BgPatAngle = cfg.BgPatAngle,
                UnivBase = cfg.UnivBase, UnivNoise = cfg.UnivNoise, UnivPattern = cfg.UnivPattern, UnivBlend = cfg.UnivBlend,
                UnivNoiseAmt = cfg.UnivNoiseAmt, UnivNoiseScale = cfg.UnivNoiseScale, UnivWarp = cfg.UnivWarp, UnivDetail = cfg.UnivDetail,
                AnimSpeed = cfg.AnimSpeed,
                HudIntensity = cfg.HudIntensity, HudR = cfg.HudR, HudG = cfg.HudG, HudB = cfg.HudB,
                HudReticle = cfg.HudReticle, HudRadar = cfg.HudRadar, HudScanline = cfg.HudScanline, HudHex = cfg.HudHex,
                HudChroma = cfg.HudChroma, HudFlicker = cfg.HudFlicker, HudScale = cfg.HudScale, HudFrame = cfg.HudFrame,
                BgBTopR = cfg.BgBTopR, BgBTopG = cfg.BgBTopG, BgBTopB = cfg.BgBTopB, BgBBotR = cfg.BgBBotR,
                BgBBotG = cfg.BgBBotG, BgBBotB = cfg.BgBBotB, BgBStyle = cfg.BgBStyle, BgBScale = cfg.BgBScale,
                BgBAngle = cfg.BgBAngle, BgBGrain = cfg.BgBGrain, BgBWarp = cfg.BgBWarp, BgBWarpAmt = cfg.BgBWarpAmt,
                BgBWarpScale = cfg.BgBWarpScale, BgBOffX = cfg.BgBOffX, BgBOffY = cfg.BgBOffY, BgBScaleY = cfg.BgBScaleY,
                BgBSharp = cfg.BgBSharp, BgBWarpX = cfg.BgBWarpX, BgBWarpY = cfg.BgBWarpY, BgBWarpAmt2 = cfg.BgBWarpAmt2,
                BgBWarpScale2 = cfg.BgBWarpScale2, BgBMidR = cfg.BgBMidR, BgBMidG = cfg.BgBMidG, BgBMidB = cfg.BgBMidB,
                BgBMetallic = cfg.BgBMetallic, BgBRoughness = cfg.BgBRoughness, BgBSpecular = cfg.BgBSpecular, BgBNormal = cfg.BgBNormal,
                BgBFresnel = cfg.BgBFresnel, BgBLightX = cfg.BgBLightX, BgBLightY = cfg.BgBLightY, BgBLightZ = cfg.BgBLightZ,
                BgBLightInt = cfg.BgBLightInt, BgBCol4R = cfg.BgBCol4R, BgBCol4G = cfg.BgBCol4G, BgBCol4B = cfg.BgBCol4B,
                BgBFbm = cfg.BgBFbm, BgBStars = cfg.BgBStars, BgBStarDensity = cfg.BgBStarDensity, BgBStarSize = cfg.BgBStarSize,
                BgBGlow = cfg.BgBGlow, BgBHueVar = cfg.BgBHueVar, BgBNebWarp = cfg.BgBNebWarp, BgBNebContrast = cfg.BgBNebContrast,
                BgBTwist = cfg.BgBTwist, BgBHaze = cfg.BgBHaze, BgBSparkle = cfg.BgBSparkle, BgBDisperse = cfg.BgBDisperse,
                BgBEmbers = cfg.BgBEmbers, BgBFlow = cfg.BgBFlow, BgBCol5R = cfg.BgBCol5R, BgBCol5G = cfg.BgBCol5G,
                BgBCol5B = cfg.BgBCol5B, BgBCol6R = cfg.BgBCol6R, BgBCol6G = cfg.BgBCol6G, BgBCol6B = cfg.BgBCol6B,
                BgBEmberSize = cfg.BgBEmberSize, BgBReflect = cfg.BgBReflect, BgBMatDisp = cfg.BgBMatDisp, BgBAniso = cfg.BgBAniso,
                BgBEnvSharp = cfg.BgBEnvSharp, BgBEnvR = cfg.BgBEnvR, BgBEnvG = cfg.BgBEnvG, BgBEnvB = cfg.BgBEnvB,
                BgBClearcoat = cfg.BgBClearcoat, BgBGradType = cfg.BgBGradType, BgBPatMode = cfg.BgBPatMode, BgBPatStrength = cfg.BgBPatStrength,
                BgBPatAngle = cfg.BgBPatAngle, BgBUnivBase = cfg.BgBUnivBase, BgBUnivNoise = cfg.BgBUnivNoise, BgBUnivPattern = cfg.BgBUnivPattern,
                BgBUnivBlend = cfg.BgBUnivBlend, BgBUnivNoiseAmt = cfg.BgBUnivNoiseAmt, BgBUnivNoiseScale = cfg.BgBUnivNoiseScale, BgBUnivWarp = cfg.BgBUnivWarp,
                BgBUnivDetail = cfg.BgBUnivDetail, BgBPad0 = cfg.BgBPad0, BgBPad1 = cfg.BgBPad1, BgBPad2 = cfg.BgBPad2,
                BlendMode = cfg.BlendMode, BlendAngle = cfg.BlendAngle, BlendOffset = cfg.BlendOffset, BlendCx = cfg.BlendCx,
                BlendCy = cfg.BlendCy, BlendRadius = cfg.BlendRadius, BlendEllipse = cfg.BlendEllipse, BlendDepthSplit = cfg.BlendDepthSplit,
                BlendDepthRef = cfg.BlendDepthRef, BlendDepthBend = cfg.BlendDepthBend, BlendFeather = cfg.BlendFeather, BlendNoiseAmt = cfg.BlendNoiseAmt,
                BlendNoiseScale = cfg.BlendNoiseScale, BlendMatch = cfg.BlendMatch, BlendMix = cfg.BlendMix, BlendMixLevel = cfg.BlendMixLevel,
                UnivHorizon = cfg.UnivHorizon, UnivGround = cfg.UnivGround, UnivOrb = cfg.UnivOrb, UnivOrbX = cfg.UnivOrbX, UnivOrbY = cfg.UnivOrbY, UnivOrbSize = cfg.UnivOrbSize, UnivRidges = cfg.UnivRidges, UnivParticle = cfg.UnivParticle,
                BgBUnivHorizon = cfg.BgBUnivHorizon, BgBUnivGround = cfg.BgBUnivGround, BgBUnivOrb = cfg.BgBUnivOrb, BgBUnivOrbX = cfg.BgBUnivOrbX, BgBUnivOrbY = cfg.BgBUnivOrbY, BgBUnivOrbSize = cfg.BgBUnivOrbSize, BgBUnivRidges = cfg.BgBUnivRidges, BgBUnivParticle = cfg.BgBUnivParticle,
                UnivCaustic = cfg.UnivCaustic, UnivShafts = cfg.UnivShafts,
                BgBUnivCaustic = cfg.BgBUnivCaustic, BgBUnivShafts = cfg.BgBUnivShafts,
                UnivPatBlend = cfg.UnivPatBlend, UnivPatStrength = cfg.UnivPatStrength,
                BgBUnivPatBlend = cfg.BgBUnivPatBlend, BgBUnivPatStrength = cfg.BgBUnivPatStrength,
                WetAmount = cfg.WetAmount, WetShine = cfg.WetShine, WetRough = cfg.WetRough, WetDeepen = cfg.WetDeepen, WetDroplets = cfg.WetDroplets, WetLightX = cfg.WetLightX, WetLightY = cfg.WetLightY, WetDepth = cfg.WetDepth,
                WetHighlight = cfg.WetHighlight, WetFresnel = cfg.WetFresnel, WetDropSize = cfg.WetDropSize, WetDropDensity = cfg.WetDropDensity, WetDropTrail = cfg.WetDropTrail,
                EnForeground = cfg.EnForegroundOn ? 1 : 0, FgPlaceMode = cfg.FgPlaceMode, FgPlaceSoft = cfg.FgPlaceSoft, FgPlaceSize = cfg.FgPlaceSize,
                FgPlaceAngle = cfg.FgPlaceAngle, FgOpacity = cfg.FgOpacity, FgBlendMode = cfg.FgBlendMode, FgDepthGate = cfg.FgDepthGate,
                FgSeamMode = cfg.FgSeamMode, FgSeamAngle = cfg.FgSeamAngle, FgSeamOffset = cfg.FgSeamOffset, FgSeamCx = cfg.FgSeamCx,
                FgSeamCy = cfg.FgSeamCy, FgSeamRadius = cfg.FgSeamRadius, FgSeamEllipse = cfg.FgSeamEllipse, FgSeamDepthSplit = cfg.FgSeamDepthSplit,
                FgSeamDepthRef = cfg.FgSeamDepthRef, FgSeamDepthBend = cfg.FgSeamDepthBend, FgSeamFeather = cfg.FgSeamFeather, FgSeamNoiseAmt = cfg.FgSeamNoiseAmt,
                FgSeamNoiseScale = cfg.FgSeamNoiseScale, FgSeamMix = cfg.FgSeamMix, FgSeamMixLevel = cfg.FgSeamMixLevel, FgSeamMatch = cfg.FgSeamMatch,
                GoboPattern = cfg.GoboPattern, GoboAmount = cfg.GoboAmount, GoboScale = cfg.GoboScale, GoboAngle = cfg.GoboAngle,
                GoboSoft = cfg.GoboSoft, BeautyAmount = cfg.BeautyAmount, BeautyRadius = cfg.BeautyRadius, BeautyGlow = cfg.BeautyGlow,
                SkinWarmth = cfg.SkinWarmth, SkinFlush = cfg.SkinFlush, SkinTintR = cfg.SkinTintR, SkinTintG = cfg.SkinTintG,
                SkinTintB = cfg.SkinTintB, BacklightAmount = cfg.BacklightAmount, BacklightWidth = cfg.BacklightWidth, BacklightR = cfg.BacklightR,
                BacklightG = cfg.BacklightG, BacklightB = cfg.BacklightB, SpotAmount = cfg.SpotAmount, SpotX = cfg.SpotX,
                SpotY = cfg.SpotY, SpotRadius = cfg.SpotRadius, SpotEllipse = cfg.SpotEllipse, SpotSoft = cfg.SpotSoft,
                SpotAngle = cfg.SpotAngle, SpotWarm = cfg.SpotWarm, ParticleType = cfg.ParticleType, ParticleAmount = cfg.ParticleAmount,
                ParticleSize = cfg.ParticleSize, ParticleFall = cfg.ParticleFall, ParticleR = cfg.ParticleR, ParticleG = cfg.ParticleG,
                ParticleB = cfg.ParticleB, BokehShape = cfg.BokehShape, BokehAmount = cfg.BokehAmount,
                Time = (((cfg.EnBackdrop || cfg.BgBStyle > 0 || cfg.EnForegroundOn) && cfg.AnimSpeed > 0f)
                        || (cfg.EnHud && cfg.HudIntensity > 0f && (cfg.HudRadar > 0f || cfg.HudFlicker > 0f))
                        || (cfg.EnParticles && cfg.ParticleAmount > 0f)
                        || (cfg.EnElements && cfg.AnyElementAnimated()))
                    ? (float)_animClock.Elapsed.TotalSeconds : 0f,
                Bypass = cfg.Bypass ? 1 : 0,
            };
            if (cfg.EnElements)
                unsafe { for (int k = 0; k < 160; k++) p.Elem[k] = cfg.Elem[k]; }
            if (cfg.EnForegroundOn && cfg.FgField != null)
                unsafe { int n = Math.Min(224, cfg.FgField.Length); for (int k = 0; k < n; k++) p.FgField[k] = cfg.FgField[k]; }
            GateGroups(ref p, cfg);

            _framesSinceRender++;
            bool needRender = _exportPending || !_haveRender || _captureChanged || memeChanged
                              || depthSrv != _lastDepthSrv
                              || _framesSinceRender >= RevalidateEveryFrames
                              || !ParamsEqual(in p, in _lastParams);
            nint outSrv;
            if (needRender)
            {
                outSrv = _gpu.Render(srcPtr, depthSrv, w, h, p, memeSrvs);
                if (outSrv == 0) return;
                _lastParams = p; _lastOutSrv = outSrv; _haveRender = true; _captureChanged = false;
                _lastDepthSrv = depthSrv;
                _framesSinceRender = 0;
                Array.Copy(memeSrvs, _lastMemeSrvs, 8);
            }
            else
            {
                outSrv = _lastOutSrv;
            }

            if (showLive)
            {
                var vp = ImGui.GetMainViewport();
                BlitOverGame(new ImTextureID(outSrv), vp.Pos, vp.Size, _gposeRects);
            }

            if (_exportPending)
            {
                _exportPending = false;
                bool jpegOut = Plugin.Config.ExportFormat == 1;
                var dir = _exportDir;
                var done = _exportDone;

                bool debugWasOn = p.DebugView != 0;
                if (debugWasOn) p.DebugView = 0;

                bool cutout = Plugin.Config.ExportTransparent && !jpegOut;
                if (cutout)
                {
                    p.Cutout = 1;
                    p.BgRecolor = 0f;
                    p.BgFill = 0f;
                }

                int scale = Math.Clamp(Plugin.Config.ExportScale, 1, 4);
                while (scale > 1 && ((long)w * scale > 16384 || (long)h * scale > 16384)) scale >>= 1;

                if (scale > 1)
                {
                    _gpu.Render(srcPtr, depthSrv, w * scale, h * scale, p, memeSrvs, scale);
                }
                else if (debugWasOn || cutout)
                {
                    _gpu.Render(srcPtr, depthSrv, w, h, p, memeSrvs, 1);
                }
                if (debugWasOn || cutout) _haveRender = false;

                var rb = _gpu.ReadbackLastOutput();
                if (rb is { } img)
                {
                    if (!cutout && IsEffectivelyBlack(img.Rgba))
                        done?.Invoke("warning: the exported image is black. Change any control to force a re-render, then export again.");

                    var (cw, ch, crgba) = CropForExport(img.Width, img.Height, img.Rgba, Plugin.Config.ExportAspect);
                    if (Plugin.Config.ShowGuides)
                        BurnGuidesInto(crgba, cw, ch, Plugin.Config, scale);
                    SaveImageAsync(cw, ch, crgba, dir, done);
                }
                else done?.Invoke("error: GPU readback failed");

                _haveRender = false; _lastOutSrv = 0; _lastDepthSrv = 0;
                if (scale > 1) _captureChanged = true;
            }
        }
        catch (Exception ex)
        {
            Services.Log.Error(ex, "LiveOverlay GPU render failed — disabling live preview");
            _gpuFailed = true;
            Enabled = false;
            Plugin.Config.LivePreview = false;
            if (_exportPending) { _exportPending = false; _exportDone?.Invoke($"error: {ex.Message}"); }
            Teardown();
        }
    }

    private static float AspectRatio(int a) => a switch
    {
        1 => 16f / 9f, 2 => 3f / 2f, 3 => 4f / 3f, 4 => 1f,
        5 => 4f / 5f, 6 => 9f / 16f, 7 => 21f / 9f, _ => -1f,
    };
    public static readonly string[] AspectNames = { "Full (native)", "16:9", "3:2", "4:3", "1:1 square", "4:5 portrait", "9:16 tall", "21:9 cinema" };

    private static (float x0, float y0, float x1, float y1) CropFrac(int aspect, float vpAspect)
    {
        float target = AspectRatio(aspect);
        if (target <= 0f) return (0f, 0f, 1f, 1f);
        if (target > vpAspect) { float h = vpAspect / target, y = (1f - h) * 0.5f; return (0f, y, 1f, y + h); }
        float w = target / vpAspect, x = (1f - w) * 0.5f; return (x, 0f, x + w, 1f);
    }

    private void DrawGuides(PluginConfig cfg)
    {
        if (!_gate.IsActive) return;
        bool wantFrame = cfg.ExportAspect != 0 && cfg.ShowExportFrame;
        bool wantBorder = cfg.EnFrame;
        if (!cfg.ShowGuides && !wantFrame && !wantBorder) return;

        var vp = ImGui.GetMainViewport();
        var dl = ImGui.GetForegroundDrawList(vp);
        Vector2 o = vp.Pos, s = vp.Size;
        var (fx0, fy0, fx1, fy1) = CropFrac(cfg.ExportAspect, s.X / s.Y);
        float rx0 = o.X + s.X * fx0, ry0 = o.Y + s.Y * fy0, rx1 = o.X + s.X * fx1, ry1 = o.Y + s.Y * fy1;

        if (wantFrame)
        {
            uint dark = ImGui.GetColorU32(new Vector4(0f, 0f, 0f, 0.5f));
            if (fy0 > 0f) dl.AddRectFilled(new Vector2(o.X, o.Y), new Vector2(o.X + s.X, ry0), dark);
            if (fy1 < 1f) dl.AddRectFilled(new Vector2(o.X, ry1), new Vector2(o.X + s.X, o.Y + s.Y), dark);
            if (fx0 > 0f) dl.AddRectFilled(new Vector2(o.X, ry0), new Vector2(rx0, ry1), dark);
            if (fx1 < 1f) dl.AddRectFilled(new Vector2(rx1, ry0), new Vector2(o.X + s.X, ry1), dark);
            dl.AddRect(new Vector2(rx0, ry0), new Vector2(rx1, ry1), ImGui.GetColorU32(new Vector4(1f, 1f, 1f, 0.6f)), 0f, ImDrawFlags.None, 1.5f);
        }

        if (wantBorder)
        {
            float shortPx = Math.Min(rx1 - rx0, ry1 - ry0);
            float pr = Math.Clamp(cfg.FrameCorner, 0f, 0.5f) * shortPx;
            uint matC = ImGui.ColorConvertFloat4ToU32(new Vector4(cfg.FrameMatR, cfg.FrameMatG, cfg.FrameMatB, 0.92f));
            if (cfg.FrameMat > 0f)
            {
                float m = cfg.FrameMat * shortPx;
                float bw = m * Math.Clamp(cfg.FrameBottom, 0f, 1f);
                float ox0 = rx0 - m, oy0 = ry0 - m, ox1 = rx1 + m, oy1 = ry1 + m + bw;
                dl.AddRectFilled(new Vector2(ox0, oy0), new Vector2(ox1, ry0), matC);
                dl.AddRectFilled(new Vector2(ox0, ry1), new Vector2(ox1, oy1), matC);
                dl.AddRectFilled(new Vector2(ox0, ry0), new Vector2(rx0, ry1), matC);
                dl.AddRectFilled(new Vector2(rx1, ry0), new Vector2(ox1, ry1), matC);
                dl.AddRect(new Vector2(ox0, oy0), new Vector2(ox1, oy1),
                           ImGui.GetColorU32(new Vector4(1f, 1f, 1f, 0.35f)),
                           Math.Clamp(cfg.FrameOuterCorner, 0f, 0.5f) * shortPx, ImDrawFlags.None, 1.5f);
            }
            if (pr > 0.5f)
                dl.AddRect(new Vector2(rx0, ry0), new Vector2(rx1, ry1),
                           ImGui.GetColorU32(new Vector4(1f, 1f, 1f, 0.5f)), pr, ImDrawFlags.None, 2f);
        }

        if (cfg.ShowGuides)
        {
            float fw = rx1 - rx0, fh = ry1 - ry0;
            float a = Math.Clamp(cfg.GuideOpacity, 0f, 1f);
            uint col = ImGui.GetColorU32(new Vector4(1f, 1f, 1f, 0.28f * a));
            void V(float f, uint c, float th) => dl.AddLine(new Vector2(rx0 + fw * f, ry0), new Vector2(rx0 + fw * f, ry1), c, th);
            void H(float f, uint c, float th) => dl.AddLine(new Vector2(rx0, ry0 + fh * f), new Vector2(rx1, ry0 + fh * f), c, th);
            if (cfg.GuideThirds) { V(1f / 3f, col, 1.2f); V(2f / 3f, col, 1.2f); H(1f / 3f, col, 1.2f); H(2f / 3f, col, 1.2f); }
            if (cfg.GuideGolden) { V(0.382f, col, 1.2f); V(0.618f, col, 1.2f); H(0.382f, col, 1.2f); H(0.618f, col, 1.2f); }
            if (cfg.GuideCenter) { V(0.5f, col, 1.2f); H(0.5f, col, 1.2f); }
            if (cfg.GuideHorizon) H(Math.Clamp(cfg.GuideHorizonY, 0f, 1f), ImGui.GetColorU32(new Vector4(1f, 0.78f, 0.24f, 0.55f * a)), 1.6f);
        }
    }

    private void DrawTexts(PluginConfig cfg)
    {
        if (!_gate.IsActive || !cfg.EnText || cfg.Texts == null || cfg.Texts.Count == 0) return;

        var vp = ImGui.GetMainViewport();
        var dl = ImGui.GetForegroundDrawList(vp);
        var font = ImGui.GetFont();
        float baseSize = ImGui.GetFontSize();

        foreach (var t in cfg.Texts)
        {
            if (t == null || string.IsNullOrEmpty(t.Text)) continue;
            float size = Math.Clamp(t.Size, 6f, 400f);
            Vector2 measured = ImGui.CalcTextSize(t.Text) * (size / Math.Max(baseSize, 1f));
            float ox = t.Align == 1 ? measured.X * 0.5f : (t.Align == 2 ? measured.X : 0f);
            var pos = new Vector2(vp.Pos.X + t.X * vp.Size.X - ox, vp.Pos.Y + t.Y * vp.Size.Y - measured.Y * 0.5f);

            uint col = ImGui.ColorConvertFloat4ToU32(new Vector4(t.R, t.G, t.B, Math.Clamp(t.A, 0f, 1f)));
            if (t.Outline)
            {
                uint oc = ImGui.ColorConvertFloat4ToU32(new Vector4(0f, 0f, 0f, Math.Clamp(t.A, 0f, 1f) * 0.85f));
                float d = Math.Max(1f, size * 0.045f);
                dl.AddText(font, size, pos + new Vector2(-d, 0f), oc, t.Text);
                dl.AddText(font, size, pos + new Vector2(d, 0f), oc, t.Text);
                dl.AddText(font, size, pos + new Vector2(0f, -d), oc, t.Text);
                dl.AddText(font, size, pos + new Vector2(0f, d), oc, t.Text);
            }
            dl.AddText(font, size, pos, col, t.Text);
        }
    }

    private static (int w, int h, byte[] rgba) CropForExport(int w, int h, byte[] rgba, int aspect)
    {
        if (AspectRatio(aspect) <= 0f) return (w, h, rgba);
        var (fx0, fy0, fx1, fy1) = CropFrac(aspect, (float)w / h);
        int cx0 = (int)Math.Round(fx0 * w), cy0 = (int)Math.Round(fy0 * h);
        int cw = Math.Clamp((int)Math.Round((fx1 - fx0) * w), 1, w - cx0);
        int ch = Math.Clamp((int)Math.Round((fy1 - fy0) * h), 1, h - cy0);
        var outp = new byte[cw * ch * 4];
        for (int y = 0; y < ch; y++)
            Array.Copy(rgba, ((cy0 + y) * w + cx0) * 4, outp, y * cw * 4, cw * 4);
        return (cw, ch, outp);
    }

    private static void BurnGuidesInto(byte[] rgba, int w, int h, PluginConfig cfg, int scale)
    {
        if (w <= 0 || h <= 0) return;
        float a = Math.Clamp(cfg.GuideOpacity, 0f, 1f);
        if (a <= 0f) return;
        int th = Math.Max(1, (int)Math.Round(1.2f * scale));
        int thH = Math.Max(1, (int)Math.Round(1.6f * scale));

        void Band(int x0, int y0, int x1, int y1, float r, float g, float b, float al)
        {
            if (al <= 0f) return;
            x0 = Math.Clamp(x0, 0, w); x1 = Math.Clamp(x1, 0, w);
            y0 = Math.Clamp(y0, 0, h); y1 = Math.Clamp(y1, 0, h);
            byte R = (byte)(r * 255f + 0.5f), G = (byte)(g * 255f + 0.5f), B = (byte)(b * 255f + 0.5f);
            for (int y = y0; y < y1; y++)
                for (int x = x0; x < x1; x++)
                {
                    int i = (y * w + x) * 4;
                    rgba[i]     = (byte)(rgba[i]     * (1f - al) + R * al);
                    rgba[i + 1] = (byte)(rgba[i + 1] * (1f - al) + G * al);
                    rgba[i + 2] = (byte)(rgba[i + 2] * (1f - al) + B * al);
                }
        }
        void V(float f, float r, float g, float b, float al, int t)
        { int x = (int)Math.Round(f * w); Band(x - t / 2, 0, x - t / 2 + t, h, r, g, b, al); }
        void H(float f, float r, float g, float b, float al, int t)
        { int y = (int)Math.Round(f * h); Band(0, y - t / 2, w, y - t / 2 + t, r, g, b, al); }

        float gc = 0.28f * a;
        if (cfg.GuideThirds) { V(1f / 3f, 1, 1, 1, gc, th); V(2f / 3f, 1, 1, 1, gc, th); H(1f / 3f, 1, 1, 1, gc, th); H(2f / 3f, 1, 1, 1, gc, th); }
        if (cfg.GuideGolden) { V(0.382f, 1, 1, 1, gc, th); V(0.618f, 1, 1, 1, gc, th); H(0.382f, 1, 1, 1, gc, th); H(0.618f, 1, 1, 1, gc, th); }
        if (cfg.GuideCenter) { V(0.5f, 1, 1, 1, gc, th); H(0.5f, 1, 1, 1, gc, th); }
        if (cfg.GuideHorizon) H(Math.Clamp(cfg.GuideHorizonY, 0f, 1f), 1f, 0.78f, 0.24f, 0.55f * a, thH);
    }

    public void SuggestGrade(PluginConfig cfg, Action<string> done)
    {
        var cap = _capture;
        if (cap is null) { done("No frame captured yet — enter gpose with live preview on."); return; }
        bool swap = cfg.SwapRedBlue;

        _ = Task.Run(async () =>
        {
            try
            {
                var (spec, bytes) = await Services.TextureReadback.GetRawImageAsync(cap).ConfigureAwait(false);
                var dom = DominantColor(spec.Width, spec.Height, bytes, swap);
                ApplyPalette(cfg, dom);
                cfg.Save();
                int R = (int)(dom.X * 255), G = (int)(dom.Y * 255), B = (int)(dom.Z * 255);
                done($"Grade suggested from character (#{R:X2}{G:X2}{B:X2}).");
            }
            catch (Exception ex)
            {
                Services.Log.Error(ex, "SuggestGrade failed");
                done($"error: {ex.Message}");
            }
        });
    }

    private static Vector3 DominantColor(int w, int h, byte[] px, bool swap)
    {
        int x0 = w * 20 / 100, x1 = w * 80 / 100;
        int y0 = h * 12 / 100, y1 = h * 92 / 100;
        int stride = w * 4;
        double sr = 0, sg = 0, sb = 0, sw = 0;
        for (int y = y0; y < y1; y += 2)
        {
            int row = y * stride;
            for (int x = x0; x < x1; x += 2)
            {
                int i = row + x * 4;
                float r = px[i] / 255f, g = px[i + 1] / 255f, b = px[i + 2] / 255f;
                if (swap) (r, b) = (b, r);
                float lum = 0.299f * r + 0.587f * g + 0.114f * b;
                if (lum < 0.06f || lum > 0.95f) continue;
                float sat = MathF.Max(r, MathF.Max(g, b)) - MathF.Min(r, MathF.Min(g, b));
                float wgt = sat * sat + 0.02f;
                sr += r * wgt; sg += g * wgt; sb += b * wgt; sw += wgt;
            }
        }
        if (sw < 1e-5) return new Vector3(0.5f);
        return new Vector3((float)(sr / sw), (float)(sg / sw), (float)(sb / sw));
    }

    private static float C01(float v) => v < 0f ? 0f : v > 1f ? 1f : v;

    private static void ApplyPalette(PluginConfig cfg, Vector3 dom)
    {
        float lum = 0.299f * dom.X + 0.587f * dom.Y + 0.114f * dom.Z;
        var chroma = dom - new Vector3(lum);
        var comp = new Vector3(lum) - chroma;
        var domTint = new Vector3(0.5f) + chroma * 0.8f;
        var compTint = new Vector3(0.5f) - chroma * 0.8f;

        cfg.CbHighR = C01(domTint.X); cfg.CbHighG = C01(domTint.Y); cfg.CbHighB = C01(domTint.Z);
        cfg.CbShadowR = C01(compTint.X); cfg.CbShadowG = C01(compTint.Y); cfg.CbShadowB = C01(compTint.Z);
        cfg.CbMidR = cfg.CbMidG = cfg.CbMidB = 0.5f;
        cfg.ColorBalance = 0.45f; cfg.EnColorBalance = true;

        cfg.ToHighR = C01(dom.X); cfg.ToHighG = C01(dom.Y); cfg.ToHighB = C01(dom.Z);
        cfg.ToShadowR = C01(comp.X); cfg.ToShadowG = C01(comp.Y); cfg.ToShadowB = C01(comp.Z);

        cfg.StHighR = C01(domTint.X); cfg.StHighG = C01(domTint.Y); cfg.StHighB = C01(domTint.Z);
        cfg.StShadowR = C01(compTint.X); cfg.StShadowG = C01(compTint.Y); cfg.StShadowB = C01(compTint.Z);

        var light = Vector3.Lerp(dom, new Vector3(1f), 0.5f);
        cfg.GmShadowR = C01(comp.X * 0.35f); cfg.GmShadowG = C01(comp.Y * 0.35f); cfg.GmShadowB = C01(comp.Z * 0.35f);
        cfg.GmMidR = C01(dom.X * 0.8f); cfg.GmMidG = C01(dom.Y * 0.8f); cfg.GmMidB = C01(dom.Z * 0.8f);
        cfg.GmHighR = C01(light.X); cfg.GmHighG = C01(light.Y); cfg.GmHighB = C01(light.Z);
    }

    private static bool ParamsEqual(in GpuRenderer.Params a, in GpuRenderer.Params b)
    {
        var sa = MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in a), 1));
        var sb = MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in b), 1));
        return sa.SequenceEqual(sb);
    }

    private static void GateGroups(ref GpuRenderer.Params p, PluginConfig c)
    {
        if (!c.EnColorBalance) p.ColorBalance = 0f;
        if (!c.EnTealOrange) p.TealOrange = 0f;
        if (!c.EnSplitTone) p.StAmount = 0f;
        if (!c.EnBleach) p.Bleach = 0f;
        if (!c.EnGradMap) p.GradMap = 0f;
        if (!c.EnGlow) { p.BloomAmount = 0f; p.Halation = 0f; p.GodrayAmount = 0f; p.Orton = 0f; p.Glamour = 0f; p.AnamAmount = 0f; }
        if (!c.EnLens) { p.Vignette = 0f; p.Sharpen = 0f; p.Chroma = 0f; p.Grain = 0f; p.Letterbox = 0f; p.Prism = 0f; p.LeakAmt = 0f; p.WashAmount = 0f; p.ChromaClean = 0f;
                         p.FilmRolloff = 0f; p.FilmToe = 0f; p.FilmSat = 0f; p.LensVig = 0f; p.LensCornerSoft = 0f; }
        if (!c.EnWarp) { p.FisheyeAmt = 0f; p.FisheyeZoom = 1f; p.SwirlAmt = 0f; p.MosaicSize = 0f; p.KaleidoSegs = 0f; p.WaveAmt = 0f; p.GlitchAmt = 0f; p.FlowAmt = 0f; }
        if (!c.EnStylize) { p.EdgeAura = 0f; p.Iridescent = 0f; p.CausticsAmt = 0f; p.KuwaharaAmt = 0f; }
        if (!c.EnFog) p.FogStrength = 0f;
        if (!c.EnSubjectIso) p.BgPushStrength = 0f;
        if (!c.EnRim) { p.RimStrength = 0f; p.SubjectPop = 0f; }
        if (!c.EnBackdrop) { p.BgRecolor = 0f; p.BgWarp = 0; }
        if (!c.EnHalo) p.HaloAmount = 0f;
        if (!c.EnFrost) p.FrostAmount = 0f;
        if (!c.EnBgFill) p.BgFill = 0f;
        if (!c.EnBgBlur) p.BgBlur = 0f;
        if (!c.EnTiltShift) p.TiltAmt = 0f;
        if (!c.EnDof) p.DofStrength = 0f;
        if (!c.EnVhs) { p.VhsStatic = 0f; p.VhsScan = 0f; p.VhsDropout = 0f; p.VhsRoll = 0f; p.VhsDesat = 0f; p.VhsVignette = 0f; }
        if (!c.EnUnderwater) { p.UwTint = 0f; p.UwCaustic = 0f; p.UwMotes = 0f; p.UwShafts = 0f; p.UwFog = 0f; }
        if (!c.EnGround) p.GroundShadow = 0f;
        if (!c.EnHud) p.HudIntensity = 0f;
        if (!c.EnWet) p.WetAmount = 0f;
        if (!c.EnShadow) p.ShadowAmount = 0f;
        if (!c.EnEdge) { p.EdgeErode = 0f; p.EdgeDespill = 0f; p.EdgeWrap = 0f; }
        if (!c.EnGobo) p.GoboAmount = 0f;
        if (!c.EnBeauty) p.BeautyAmount = 0f;
        if (!c.EnSkin) { p.SkinWarmth = 0f; p.SkinFlush = 0f; }
        if (!c.EnBacklight) p.BacklightAmount = 0f;
        if (!c.EnSpot) p.SpotAmount = 0f;
        if (!c.EnParticles) { p.ParticleAmount = 0f; p.BokehAmount = 0f; }
    }

    private void StartCapture(uint viewportId)
    {
        var args = new ImGuiViewportTextureArgs
        {
            ViewportId = viewportId,
            TakeBeforeImGuiRender = true,
            KeepTransparency = false,
        };

        _ = Task.Run(async () =>
        {
            try
            {
                var wrap = await Services.TextureProvider
                    .CreateFromImGuiViewportAsync(args, "gposestudio-live").ConfigureAwait(false);
                if (_disposed) { wrap.Dispose(); return; }
                var prev = Interlocked.Exchange(ref _incoming, wrap);
                prev?.Dispose();
            }
            catch (Exception ex)
            {
                Services.Log.Error(ex, "LiveOverlay capture failed");
            }
            finally
            {
                _nextCaptureTick = Environment.TickCount64 + CaptureIntervalMs;
                _capturing = false;
            }
        });
    }

    private static void BlitOverGame(ImTextureID tex, Vector2 pos, Vector2 size, GposePanel.Rect[] holes)
    {
        var dl = ImGui.GetBackgroundDrawList();
        var max = pos + size;

        var clamped = new List<(float x0, float y0, float x1, float y1)>(holes.Length);
        var ys = new SortedSet<float> { pos.Y, max.Y };
        foreach (var r in holes)
        {
            float x0 = Math.Max(r.X, pos.X), y0 = Math.Max(r.Y, pos.Y);
            float x1 = Math.Min(r.X + r.W, max.X), y1 = Math.Min(r.Y + r.H, max.Y);
            if (x1 <= x0 || y1 <= y0) continue;
            if ((x1 - x0) >= size.X * 0.7f && (y1 - y0) >= size.Y * 0.7f) continue;
            clamped.Add((x0, y0, x1, y1));
            ys.Add(y0); ys.Add(y1);
        }
        if (clamped.Count == 0) { dl.AddImage(tex, pos, max); return; }

        var yb = new List<float>(ys);
        var ex = new List<(float a, float b)>();
        for (int i = 0; i + 1 < yb.Count; i++)
        {
            float ya = yb[i], yc = yb[i + 1];
            if (yc <= ya) continue;
            float mid = (ya + yc) * 0.5f;

            ex.Clear();
            foreach (var c in clamped)
                if (c.y0 <= mid && c.y1 >= mid) ex.Add((c.x0, c.x1));
            ex.Sort((p1, p2) => p1.a.CompareTo(p2.a));

            float cursor = pos.X;
            foreach (var e in ex)
            {
                if (e.a > cursor) Region(dl, tex, pos, size, new Vector2(cursor, ya), new Vector2(e.a, yc));
                cursor = Math.Max(cursor, e.b);
            }
            if (cursor < max.X) Region(dl, tex, pos, size, new Vector2(cursor, ya), new Vector2(max.X, yc));
        }
    }

    private static void Region(ImDrawListPtr dl, ImTextureID tex, Vector2 fullPos, Vector2 fullSize, Vector2 a, Vector2 b)
    {
        if (b.X <= a.X || b.Y <= a.Y) return;
        dl.AddImage(tex, a, b, (a - fullPos) / fullSize, (b - fullPos) / fullSize);
    }

    private static bool IsEffectivelyBlack(byte[] rgba)
    {
        int px = rgba.Length / 4;
        if (px <= 0) return false;
        int step = Math.Max(1, px / 4096);
        for (int i = 0; i < px; i += step)
        {
            int o = i * 4;
            if (rgba[o] > 8 || rgba[o + 1] > 8 || rgba[o + 2] > 8) return false;
        }
        return true;
    }

    private static void SaveImageAsync(int w, int h, byte[] rgba, string dir, Action<string>? done)
    {
        bool jpeg = Plugin.Config.ExportFormat == 1;
        int quality = Plugin.Config.ExportJpegQuality;
        bool framed = Plugin.Config.EnFrame;
        var fopts = Plugin.Config.FrameOpts();
        string? embed = (!jpeg && Plugin.Config.EmbedLookInPng)
                        ? LookStore.Capture(Plugin.Config, forSharing: true) : null;
        _ = Task.Run(async () =>
        {
            try
            {
                if (framed)
                    (w, h, rgba) = Frame.Compose(w, h, rgba, in fopts, allowAlpha: !jpeg);
                Directory.CreateDirectory(dir);
                string ext = jpeg ? "jpg" : "png";
                var path = Path.Combine(dir, $"gpose_{DateTime.Now:yyyyMMdd_HHmmss_fff}.{ext}");
                byte[] bytes = jpeg ? Jpeg.Encode(w, h, rgba, quality) : Png.Encode(w, h, rgba, embed);
                var tmp = path + ".tmp";
                await File.WriteAllBytesAsync(tmp, bytes).ConfigureAwait(false);
                File.Move(tmp, path, overwrite: true);
                done?.Invoke(path);
            }
            catch (Exception ex)
            {
                Services.Log.Error(ex, "image save failed");
                done?.Invoke($"error: {ex.Message}");
            }
        });
    }

    private void Teardown()
    {
        DepthAvailable = false;
        _capture?.Dispose();
        _capture = null;
        var staged = Interlocked.Exchange(ref _incoming, null);
        staged?.Dispose();
        _haveRender = false;
        _lastOutSrv = 0;
        _lastDepthSrv = 0;
        _captureChanged = false;
        _framesSinceRender = 0;
        _capturing = false;
        _captureStartedAt = 0;
        _depthSeenSrv = 0;
        _depthSettled = 0;
    }

    public void Dispose()
    {
        _disposed = true;
        Services.PluginInterface.UiBuilder.Draw -= OnDraw;
        Services.Framework.Update -= OnFrameworkUpdate;
        Teardown();
        _gpu?.Dispose();
        _gpu = null;
    }
}
