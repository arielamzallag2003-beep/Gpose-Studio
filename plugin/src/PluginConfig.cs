using System;
using System.Collections.Generic;
using System.IO;
using Dalamud.Configuration;

namespace GPoseStudio;

public sealed class PluginConfig : IPluginConfiguration
{
    public int Version { get; set; } = 1;

    public string OutputDirectory { get; set; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "GPoseStudio");

    public bool SwapRedBlue { get; set; } = true;
    public bool FlipVertical { get; set; } = false;

    public bool LivePreview { get; set; } = false;

    public float Exposure { get; set; } = 0f;
    public float Contrast { get; set; } = 0f;
    public float Saturation { get; set; } = 0f;
    public float Temperature { get; set; } = 0f;
    public float Tint { get; set; } = 0f;
    public float Lift { get; set; } = 0f;
    public float Gamma { get; set; } = 0f;
    public float Gain { get; set; } = 0f;
    public float Vibrance { get; set; } = 0f;
    public float Vignette { get; set; } = 0f;
    public float Sharpen { get; set; } = 0f;
    public float Chroma { get; set; } = 0f;
    public float Grain { get; set; } = 0f;
    public float Letterbox { get; set; } = 0f;

    public float BlackPoint { get; set; } = 0f;
    public float WhitePoint { get; set; } = 1f;
    public float HueShift { get; set; } = 0f;

    public float Bleach { get; set; } = 0f;
    public float BleachContrast { get; set; } = 1.25f;

    public float TealOrange { get; set; } = 0f;
    public float TealOrangePunch { get; set; } = 1.18f;
    public float ToShadowR { get; set; } = 0.0f; public float ToShadowG { get; set; } = 0.55f; public float ToShadowB { get; set; } = 0.62f;
    public float ToHighR { get; set; } = 1.0f; public float ToHighG { get; set; } = 0.62f; public float ToHighB { get; set; } = 0.22f;

    public float ColorBalance { get; set; } = 0f;
    public float CbShadowR { get; set; } = 0.5f; public float CbShadowG { get; set; } = 0.5f; public float CbShadowB { get; set; } = 0.5f;
    public float CbMidR { get; set; } = 0.5f; public float CbMidG { get; set; } = 0.5f; public float CbMidB { get; set; } = 0.5f;
    public float CbHighR { get; set; } = 0.5f; public float CbHighG { get; set; } = 0.5f; public float CbHighB { get; set; } = 0.5f;

    public float FisheyeAmt { get; set; } = 0f;
    public float FisheyeZoom { get; set; } = 1f;
    public float SwirlAmt { get; set; } = 0f;
    public float SwirlRadius { get; set; } = 0.45f;
    public float MosaicSize { get; set; } = 0f;
    public float KaleidoSegs { get; set; } = 0f;
    public float KaleidoRot { get; set; } = 0f;

    public float BloomAmount { get; set; } = 0f;
    public float BloomThreshold { get; set; } = 0.7f;
    public float BloomRadius { get; set; } = 2.5f;

    public float Halation { get; set; } = 0f;
    public float HalationR { get; set; } = 1.0f; public float HalationG { get; set; } = 0.45f; public float HalationB { get; set; } = 0.30f;

    public float GodrayAmount { get; set; } = 0f;
    public float GodrayLightX { get; set; } = 0.5f;
    public float GodrayLightY { get; set; } = 0.35f;
    public float GodrayDecay { get; set; } = 0.96f;
    public float GodrayThreshold { get; set; } = 0.6f;
    public float GodrayR { get; set; } = 1.0f; public float GodrayG { get; set; } = 0.95f; public float GodrayB { get; set; } = 0.85f;

    public float FogStart { get; set; } = 0f;
    public float FogStrength { get; set; } = 0f;
    public float FogColorR { get; set; } = 0.62f;
    public float FogColorG { get; set; } = 0.68f;
    public float FogColorB { get; set; } = 0.78f;
    public float BgPushStart { get; set; } = 0.05f;
    public float BgPushStrength { get; set; } = 0f;
    public float DofFocus { get; set; } = 0.03f;
    public float DofRange { get; set; } = 0.08f;
    public float DofStrength { get; set; } = 0f;

    public float RimStrength { get; set; } = 0f;
    public float RimThreshold { get; set; } = 0.01f;
    public float RimWidth { get; set; } = 2f;
    public float RimR { get; set; } = 1f; public float RimG { get; set; } = 1f; public float RimB { get; set; } = 1f;

    public float BgRecolor { get; set; } = 0f;
    public float BgRecolorStart { get; set; } = 0.15f;
    public float BgRecolorFeather { get; set; } = 0.08f;
    public float BgTopR { get; set; } = 0.30f; public float BgTopG { get; set; } = 0.40f; public float BgTopB { get; set; } = 0.62f;
    public float BgBotR { get; set; } = 0.10f; public float BgBotG { get; set; } = 0.12f; public float BgBotB { get; set; } = 0.20f;

    public float BgBlur { get; set; } = 0f;
    public float BgBlurStart { get; set; } = 0.06f;
    public float Orton { get; set; } = 0f;
    public float Glamour { get; set; } = 0f;
    public float GlamourMist { get; set; } = 0.3f;
    public float SoftBlurRadius { get; set; } = 3f;

    public float GradMap { get; set; } = 0f;
    public float GmShadowR { get; set; } = 0.10f; public float GmShadowG { get; set; } = 0.05f; public float GmShadowB { get; set; } = 0.22f;
    public float GmMidR { get; set; } = 0.72f; public float GmMidG { get; set; } = 0.18f; public float GmMidB { get; set; } = 0.42f;
    public float GmHighR { get; set; } = 1.0f; public float GmHighG { get; set; } = 0.86f; public float GmHighB { get; set; } = 0.55f;

    public float Dehaze { get; set; } = 0f;
    public float WaveAmt { get; set; } = 0f;
    public float WaveFreq { get; set; } = 24f;
    public float WavePhase { get; set; } = 0f;
    public float GlitchAmt { get; set; } = 0f;
    public float GlitchBlocks { get; set; } = 24f;

    public float StShadowR { get; set; } = 0.45f; public float StShadowG { get; set; } = 0.48f; public float StShadowB { get; set; } = 0.55f;
    public float StHighR { get; set; } = 0.55f; public float StHighG { get; set; } = 0.50f; public float StHighB { get; set; } = 0.42f;
    public float StBalance { get; set; } = 0.5f;
    public float StAmount { get; set; } = 0f;
    public float Clarity { get; set; } = 0f;
    public float TiltAmt { get; set; } = 0f;
    public float TiltFocus { get; set; } = 0.5f;
    public float TiltRange { get; set; } = 0.2f;
    public float FlowAmt { get; set; } = 0f;
    public float FlowScale { get; set; } = 6f;
    public float FlowSeed { get; set; } = 3f;

    public int ScopeMode { get; set; } = 0;
    public float ScopeSplit { get; set; } = 0.08f;
    public float ScopeSoft { get; set; } = 0.05f;

    public float EdgeAura { get; set; } = 0f;
    public float EdgeWidth { get; set; } = 1.5f;
    public float EdgeThreshold { get; set; } = 0.07f;
    public float EdgeR { get; set; } = 0.5f; public float EdgeG { get; set; } = 0.82f; public float EdgeB { get; set; } = 1.0f;

    public float Iridescent { get; set; } = 0f;
    public float IridFreq { get; set; } = 3f;
    public float IridShift { get; set; } = 0f;

    public float Prism { get; set; } = 0f;

    public float LeakAmt { get; set; } = 0f;
    public float LeakAngle { get; set; } = 0.8f;
    public float LeakR { get; set; } = 1.0f; public float LeakG { get; set; } = 0.6f; public float LeakB { get; set; } = 0.3f;

    public float AnamAmount { get; set; } = 0f;
    public float AnamThreshold { get; set; } = 0.6f;
    public float AnamLength { get; set; } = 12f;
    public float AnamR { get; set; } = 0.42f; public float AnamG { get; set; } = 0.62f; public float AnamB { get; set; } = 1.0f;

    public float HlRecovery { get; set; } = 0f;
    public float SubjectPop { get; set; } = 0f;

    public float HaloAmount { get; set; } = 0f;
    public float HaloSplit { get; set; } = 0.1f;
    public float HaloR { get; set; } = 1.0f; public float HaloG { get; set; } = 0.95f; public float HaloB { get; set; } = 0.85f;

    public float FrostAmount { get; set; } = 0f;
    public float FrostCoverage { get; set; } = 0.4f;
    public float FrostFeather { get; set; } = 0.4f;

    public float WashAmount { get; set; } = 0f;
    public float WashX { get; set; } = 0.5f;
    public float WashY { get; set; } = 0.3f;
    public float WashR { get; set; } = 1.0f; public float WashG { get; set; } = 0.92f; public float WashB { get; set; } = 0.78f;

    public float CausticsAmt { get; set; } = 0f;
    public float CausticsScale { get; set; } = 9f;
    public float CausticsR { get; set; } = 0.68f; public float CausticsG { get; set; } = 0.90f; public float CausticsB { get; set; } = 1.0f;

    public float ChromaClean { get; set; } = 0f;
    public float Denoise { get; set; } = 0f;
    public float DenoiseEdge { get; set; } = 0.06f;

    public float KuwaharaAmt { get; set; } = 0f;
    public float KuwaharaRadius { get; set; } = 4f;

    public float BgFill { get; set; } = 0f;
    public float BgFillStart { get; set; } = 0.1f;
    public float BgFillFeather { get; set; } = 0.05f;
    public float BgFillR { get; set; } = 0.5f;
    public float BgFillG { get; set; } = 0.5f;
    public float BgFillB { get; set; } = 0.5f;

    public int BgStyle { get; set; } = 0;
    public float BgScale { get; set; } = 8f;
    public float BgAngle { get; set; } = 0f;
    public float BgGrain { get; set; } = 0f;
    public int BgWarp { get; set; } = 0;
    public float BgWarpAmt { get; set; } = 0.5f;
    public float BgWarpScale { get; set; } = 4f;
    public float BgWarpAmt2 { get; set; } = 0.5f;
    public float BgWarpScale2 { get; set; } = 4f;
    public float BgWarpX { get; set; } = 0.5f;
    public float BgWarpY { get; set; } = 0.5f;
    public float BgOffX { get; set; } = 0f;
    public float BgOffY { get; set; } = 0f;
    public float BgScaleY { get; set; } = 8f;
    public float BgSharp { get; set; } = 0f;
    public float BgMidR { get; set; } = 0.20f; public float BgMidG { get; set; } = 0.26f; public float BgMidB { get; set; } = 0.41f;
    public float BgMetallic { get; set; } = 0f;
    public float BgRoughness { get; set; } = 0.5f;
    public float BgSpecular { get; set; } = 0f;
    public float BgNormal { get; set; } = 0f;
    public float BgFresnel { get; set; } = 0f;
    public float BgLightX { get; set; } = -0.4f;
    public float BgLightY { get; set; } = 0.5f;
    public float BgLightZ { get; set; } = 0.7f;
    public float BgLightInt { get; set; } = 0f;
    public float BgCol4R { get; set; } = 1.0f; public float BgCol4G { get; set; } = 0.92f; public float BgCol4B { get; set; } = 0.80f;
    public float BgFbm { get; set; } = 4f;
    public float BgStars { get; set; } = 0f;
    public float BgStarDensity { get; set; } = 40f;
    public float BgStarSize { get; set; } = 0.3f;
    public float BgGlow { get; set; } = 0f;
    public float BgVignette { get; set; } = 0f;
    public float BgVignetteSize { get; set; } = 0.6f;
    public float BgHueVar { get; set; } = 0f;
    public float BgBright { get; set; } = 0f;
    public float BgNebWarp { get; set; } = 0f;
    public float BgNebContrast { get; set; } = 0f;
    public float BgVoidCore { get; set; } = 0f;
    public float BgVoidRing { get; set; } = 0f;
    public float BgTwist { get; set; } = 0f;
    public float BgHaze { get; set; } = 0f;
    public float BgSparkle { get; set; } = 0f;
    public float BgDisperse { get; set; } = 0f;
    public float BgRingWidth { get; set; } = 1f;
    public float BgRing2 { get; set; } = 0f;
    public float BgEmbers { get; set; } = 0f;
    public float BgFlow { get; set; } = 0f;
    public float BgCol5R { get; set; } = 0.25f; public float BgCol5G { get; set; } = 0.33f; public float BgCol5B { get; set; } = 0.515f;
    public float BgCol6R { get; set; } = 0.15f; public float BgCol6G { get; set; } = 0.19f; public float BgCol6B { get; set; } = 0.305f;
    public float BgEmberSize { get; set; } = 0.3f;
    public float BgKeepVfx { get; set; } = 0.85f;
    public float VhsStatic { get; set; } = 0f;
    public float VhsScan { get; set; } = 0f;
    public float VhsScanCount { get; set; } = 300f;
    public float VhsDropout { get; set; } = 0f;
    public float VhsRoll { get; set; } = 0f;
    public float VhsRollPos { get; set; } = 0.3f;
    public float VhsDesat { get; set; } = 0f;
    public float VhsVignette { get; set; } = 0f;
    public float BgCausticAmt { get; set; } = 0f;
    public float BgShafts { get; set; } = 0f;
    public float BgBubbles { get; set; } = 0f;
    public float UwTint { get; set; } = 0f;
    public float UwTintR { get; set; } = 0.10f; public float UwTintG { get; set; } = 0.38f; public float UwTintB { get; set; } = 0.45f;
    public float UwCaustic { get; set; } = 0f;
    public float UwMotes { get; set; } = 0f;
    public float UwShafts { get; set; } = 0f;
    public float UwFog { get; set; } = 0f;
    public float BgReflect { get; set; } = 0f;
    public float BgMatDisp { get; set; } = 0f;
    public float BgAniso { get; set; } = 0f;
    public float BgEnvSharp { get; set; } = 0.5f;
    public float BgEnvR { get; set; } = 0.6f; public float BgEnvG { get; set; } = 0.7f; public float BgEnvB { get; set; } = 1.0f;
    public float BgClearcoat { get; set; } = 0f;
    public int BgGradType { get; set; } = 0;
    public int BgPatMode { get; set; } = 0;
    public float BgPatStrength { get; set; } = 0.5f;
    public float BgPatAngle { get; set; } = 0f;
    public int UnivBase { get; set; } = 1;
    public int UnivNoise { get; set; } = 1;
    public int UnivPattern { get; set; } = 0;
    public int UnivBlend { get; set; } = 5;
    public float UnivNoiseAmt { get; set; } = 0.6f;
    public float UnivNoiseScale { get; set; } = 0.5f;
    public float UnivWarp { get; set; } = 0.3f;
    public float UnivDetail { get; set; } = 0.3f;
    public float BgBTopR { get; set; } = 0.30f;
    public float BgBTopG { get; set; } = 0.40f;
    public float BgBTopB { get; set; } = 0.62f;
    public float BgBBotR { get; set; } = 0.10f;
    public float BgBBotG { get; set; } = 0.12f;
    public float BgBBotB { get; set; } = 0.20f;
    public int BgBStyle { get; set; } = 0;
    public float BgBScale { get; set; } = 8f;
    public float BgBAngle { get; set; } = 0f;
    public float BgBGrain { get; set; } = 0f;
    public int BgBWarp { get; set; } = 0;
    public float BgBWarpAmt { get; set; } = 0.5f;
    public float BgBWarpScale { get; set; } = 4f;
    public float BgBOffX { get; set; } = 0f;
    public float BgBOffY { get; set; } = 0f;
    public float BgBScaleY { get; set; } = 8f;
    public float BgBSharp { get; set; } = 0f;
    public float BgBWarpX { get; set; } = 0.5f;
    public float BgBWarpY { get; set; } = 0.5f;
    public float BgBWarpAmt2 { get; set; } = 0.5f;
    public float BgBWarpScale2 { get; set; } = 4f;
    public float BgBMidR { get; set; } = 0.20f;
    public float BgBMidG { get; set; } = 0.26f;
    public float BgBMidB { get; set; } = 0.41f;
    public float BgBMetallic { get; set; } = 0f;
    public float BgBRoughness { get; set; } = 0.5f;
    public float BgBSpecular { get; set; } = 0f;
    public float BgBNormal { get; set; } = 0f;
    public float BgBFresnel { get; set; } = 0f;
    public float BgBLightX { get; set; } = -0.4f;
    public float BgBLightY { get; set; } = 0.5f;
    public float BgBLightZ { get; set; } = 0.7f;
    public float BgBLightInt { get; set; } = 0f;
    public float BgBCol4R { get; set; } = 1.0f;
    public float BgBCol4G { get; set; } = 0.92f;
    public float BgBCol4B { get; set; } = 0.80f;
    public float BgBFbm { get; set; } = 4f;
    public float BgBStars { get; set; } = 0f;
    public float BgBStarDensity { get; set; } = 40f;
    public float BgBStarSize { get; set; } = 0.3f;
    public float BgBGlow { get; set; } = 0f;
    public float BgBHueVar { get; set; } = 0f;
    public float BgBNebWarp { get; set; } = 0f;
    public float BgBNebContrast { get; set; } = 0f;
    public float BgBTwist { get; set; } = 0f;
    public float BgBHaze { get; set; } = 0f;
    public float BgBSparkle { get; set; } = 0f;
    public float BgBDisperse { get; set; } = 0f;
    public float BgBEmbers { get; set; } = 0f;
    public float BgBFlow { get; set; } = 0f;
    public float BgBCol5R { get; set; } = 0.25f;
    public float BgBCol5G { get; set; } = 0.33f;
    public float BgBCol5B { get; set; } = 0.515f;
    public float BgBCol6R { get; set; } = 0.15f;
    public float BgBCol6G { get; set; } = 0.19f;
    public float BgBCol6B { get; set; } = 0.305f;
    public float BgBEmberSize { get; set; } = 0.3f;
    public float BgBReflect { get; set; } = 0f;
    public float BgBMatDisp { get; set; } = 0f;
    public float BgBAniso { get; set; } = 0f;
    public float BgBEnvSharp { get; set; } = 0.5f;
    public float BgBEnvR { get; set; } = 0.6f;
    public float BgBEnvG { get; set; } = 0.7f;
    public float BgBEnvB { get; set; } = 1.0f;
    public float BgBClearcoat { get; set; } = 0f;
    public int BgBGradType { get; set; } = 0;
    public int BgBPatMode { get; set; } = 0;
    public float BgBPatStrength { get; set; } = 0.5f;
    public float BgBPatAngle { get; set; } = 0f;
    public int BgBUnivBase { get; set; } = 1;
    public int BgBUnivNoise { get; set; } = 1;
    public int BgBUnivPattern { get; set; } = 0;
    public int BgBUnivBlend { get; set; } = 5;
    public float BgBUnivNoiseAmt { get; set; } = 0.6f;
    public float BgBUnivNoiseScale { get; set; } = 0.5f;
    public float BgBUnivWarp { get; set; } = 0.3f;
    public float BgBUnivDetail { get; set; } = 0.3f;
    public float BgBPad0 { get; set; } = 0f;
    public float BgBPad1 { get; set; } = 0f;
    public float BgBPad2 { get; set; } = 0f;
    public int BlendMode { get; set; } = 0;
    public float BlendAngle { get; set; } = 0f;
    public float BlendOffset { get; set; } = 0f;
    public float BlendCx { get; set; } = 0.5f;
    public float BlendCy { get; set; } = 0.5f;
    public float BlendRadius { get; set; } = 0.3f;
    public float BlendEllipse { get; set; } = 1f;
    public float BlendDepthSplit { get; set; } = 0.5f;
    public float BlendDepthRef { get; set; } = 0.5f;
    public float BlendDepthBend { get; set; } = 0f;
    public float BlendFeather { get; set; } = 0.14f;
    public float BlendNoiseAmt { get; set; } = 0.12f;
    public float BlendNoiseScale { get; set; } = 3f;
    public float BlendMatch { get; set; } = 0f;
    public int BlendMix { get; set; } = 0;
    public float BlendMixLevel { get; set; } = 0.45f;
    public float UnivHorizon { get; set; } = 0f;
    public int UnivGround { get; set; } = 0;
    public int UnivOrb { get; set; } = 0;
    public float UnivOrbX { get; set; } = 0.5f;
    public float UnivOrbY { get; set; } = 0.32f;
    public float UnivOrbSize { get; set; } = 0.12f;
    public float UnivRidges { get; set; } = 0f;
    public int UnivParticle { get; set; } = 0;
    public float UnivCaustic { get; set; } = 0f;
    public float UnivShafts { get; set; } = 0f;
    public float BgBUnivCaustic { get; set; } = 0f;
    public float BgBUnivShafts { get; set; } = 0f;
    public int UnivPatBlend { get; set; } = 0;
    public float UnivPatStrength { get; set; } = 0.5f;
    public int BgBUnivPatBlend { get; set; } = 0;
    public float BgBUnivPatStrength { get; set; } = 0.5f;
    public float WetAmount { get; set; } = 0f;
    public float WetShine { get; set; } = 0.6f;
    public float WetRough { get; set; } = 0.25f;
    public float WetDeepen { get; set; } = 0.4f;
    public float WetDroplets { get; set; } = 0f;
    public float WetLightX { get; set; } = 0f;
    public float WetLightY { get; set; } = 0.4f;
    public float WetDepth { get; set; } = 0.12f;
    public float WetHighlight { get; set; } = 0.5f;
    public float WetFresnel { get; set; } = 0.4f;
    public float WetDropSize { get; set; } = 0.5f;
    public float WetDropDensity { get; set; } = 0.5f;
    public float WetDropTrail { get; set; } = 0f;
    public bool EnWet { get; set; } = true;
    public float BgBUnivHorizon { get; set; } = 0f;
    public int BgBUnivGround { get; set; } = 0;
    public int BgBUnivOrb { get; set; } = 0;
    public float BgBUnivOrbX { get; set; } = 0.5f;
    public float BgBUnivOrbY { get; set; } = 0.32f;
    public float BgBUnivOrbSize { get; set; } = 0.12f;
    public float BgBUnivRidges { get; set; } = 0f;
    public int BgBUnivParticle { get; set; } = 0;

    public void LoadBgBInto(PluginConfig s)
    {
        s.UnivPatBlend = BgBUnivPatBlend; s.UnivPatStrength = BgBUnivPatStrength;
        s.UnivCaustic = BgBUnivCaustic; s.UnivShafts = BgBUnivShafts;
        s.UnivHorizon = BgBUnivHorizon; s.UnivGround = BgBUnivGround; s.UnivOrb = BgBUnivOrb; s.UnivOrbX = BgBUnivOrbX; s.UnivOrbY = BgBUnivOrbY; s.UnivOrbSize = BgBUnivOrbSize; s.UnivRidges = BgBUnivRidges; s.UnivParticle = BgBUnivParticle;
        s.BgTopR = BgBTopR; s.BgTopG = BgBTopG; s.BgTopB = BgBTopB;
        s.BgBotR = BgBBotR; s.BgBotG = BgBBotG; s.BgBotB = BgBBotB;
        s.BgStyle = BgBStyle; s.BgScale = BgBScale; s.BgAngle = BgBAngle;
        s.BgGrain = BgBGrain; s.BgWarp = BgBWarp; s.BgWarpAmt = BgBWarpAmt;
        s.BgWarpScale = BgBWarpScale; s.BgOffX = BgBOffX; s.BgOffY = BgBOffY;
        s.BgScaleY = BgBScaleY; s.BgSharp = BgBSharp; s.BgWarpX = BgBWarpX;
        s.BgWarpY = BgBWarpY; s.BgWarpAmt2 = BgBWarpAmt2; s.BgWarpScale2 = BgBWarpScale2;
        s.BgMidR = BgBMidR; s.BgMidG = BgBMidG; s.BgMidB = BgBMidB;
        s.BgMetallic = BgBMetallic; s.BgRoughness = BgBRoughness; s.BgSpecular = BgBSpecular;
        s.BgNormal = BgBNormal; s.BgFresnel = BgBFresnel; s.BgLightX = BgBLightX;
        s.BgLightY = BgBLightY; s.BgLightZ = BgBLightZ; s.BgLightInt = BgBLightInt;
        s.BgCol4R = BgBCol4R; s.BgCol4G = BgBCol4G; s.BgCol4B = BgBCol4B;
        s.BgFbm = BgBFbm; s.BgStars = BgBStars; s.BgStarDensity = BgBStarDensity;
        s.BgStarSize = BgBStarSize; s.BgGlow = BgBGlow; s.BgHueVar = BgBHueVar;
        s.BgNebWarp = BgBNebWarp; s.BgNebContrast = BgBNebContrast; s.BgTwist = BgBTwist;
        s.BgHaze = BgBHaze; s.BgSparkle = BgBSparkle; s.BgDisperse = BgBDisperse;
        s.BgEmbers = BgBEmbers; s.BgFlow = BgBFlow; s.BgCol5R = BgBCol5R;
        s.BgCol5G = BgBCol5G; s.BgCol5B = BgBCol5B; s.BgCol6R = BgBCol6R;
        s.BgCol6G = BgBCol6G; s.BgCol6B = BgBCol6B; s.BgEmberSize = BgBEmberSize;
        s.BgReflect = BgBReflect; s.BgMatDisp = BgBMatDisp; s.BgAniso = BgBAniso;
        s.BgEnvSharp = BgBEnvSharp; s.BgEnvR = BgBEnvR; s.BgEnvG = BgBEnvG;
        s.BgEnvB = BgBEnvB; s.BgClearcoat = BgBClearcoat; s.BgGradType = BgBGradType;
        s.BgPatMode = BgBPatMode; s.BgPatStrength = BgBPatStrength; s.BgPatAngle = BgBPatAngle;
        s.UnivBase = BgBUnivBase; s.UnivNoise = BgBUnivNoise; s.UnivPattern = BgBUnivPattern;
        s.UnivBlend = BgBUnivBlend; s.UnivNoiseAmt = BgBUnivNoiseAmt; s.UnivNoiseScale = BgBUnivNoiseScale;
        s.UnivWarp = BgBUnivWarp; s.UnivDetail = BgBUnivDetail;
        s.PatColOverride = BgBPatColOverride; s.PatColMode = BgBPatColMode;
        s.PatColR = BgBPatColR; s.PatColG = BgBPatColG; s.PatColB = BgBPatColB;
        s.PatCol2R = BgBPatCol2R; s.PatCol2G = BgBPatCol2G; s.PatCol2B = BgBPatCol2B;
        s.PatCol3R = BgBPatCol3R; s.PatCol3G = BgBPatCol3G; s.PatCol3B = BgBPatCol3B;
        s.PatCol4R = BgBPatCol4R; s.PatCol4G = BgBPatCol4G; s.PatCol4B = BgBPatCol4B;
        s.PatCol5R = BgBPatCol5R; s.PatCol5G = BgBPatCol5G; s.PatCol5B = BgBPatCol5B;
        s.PatMat = BgBPatMat; s.PatMatR = BgBPatMatR; s.PatMatG = BgBPatMatG;
        s.PatMatB = BgBPatMatB; s.PatMatTint = BgBPatMatTint;
    }

    public void SaveBgBFrom(PluginConfig s)
    {
        BgBUnivPatBlend = s.UnivPatBlend; BgBUnivPatStrength = s.UnivPatStrength;
        BgBUnivCaustic = s.UnivCaustic; BgBUnivShafts = s.UnivShafts;
        BgBUnivHorizon = s.UnivHorizon; BgBUnivGround = s.UnivGround; BgBUnivOrb = s.UnivOrb; BgBUnivOrbX = s.UnivOrbX; BgBUnivOrbY = s.UnivOrbY; BgBUnivOrbSize = s.UnivOrbSize; BgBUnivRidges = s.UnivRidges; BgBUnivParticle = s.UnivParticle;
        BgBTopR = s.BgTopR; BgBTopG = s.BgTopG; BgBTopB = s.BgTopB;
        BgBBotR = s.BgBotR; BgBBotG = s.BgBotG; BgBBotB = s.BgBotB;
        BgBStyle = s.BgStyle; BgBScale = s.BgScale; BgBAngle = s.BgAngle;
        BgBGrain = s.BgGrain; BgBWarp = s.BgWarp; BgBWarpAmt = s.BgWarpAmt;
        BgBWarpScale = s.BgWarpScale; BgBOffX = s.BgOffX; BgBOffY = s.BgOffY;
        BgBScaleY = s.BgScaleY; BgBSharp = s.BgSharp; BgBWarpX = s.BgWarpX;
        BgBWarpY = s.BgWarpY; BgBWarpAmt2 = s.BgWarpAmt2; BgBWarpScale2 = s.BgWarpScale2;
        BgBMidR = s.BgMidR; BgBMidG = s.BgMidG; BgBMidB = s.BgMidB;
        BgBMetallic = s.BgMetallic; BgBRoughness = s.BgRoughness; BgBSpecular = s.BgSpecular;
        BgBNormal = s.BgNormal; BgBFresnel = s.BgFresnel; BgBLightX = s.BgLightX;
        BgBLightY = s.BgLightY; BgBLightZ = s.BgLightZ; BgBLightInt = s.BgLightInt;
        BgBCol4R = s.BgCol4R; BgBCol4G = s.BgCol4G; BgBCol4B = s.BgCol4B;
        BgBFbm = s.BgFbm; BgBStars = s.BgStars; BgBStarDensity = s.BgStarDensity;
        BgBStarSize = s.BgStarSize; BgBGlow = s.BgGlow; BgBHueVar = s.BgHueVar;
        BgBNebWarp = s.BgNebWarp; BgBNebContrast = s.BgNebContrast; BgBTwist = s.BgTwist;
        BgBHaze = s.BgHaze; BgBSparkle = s.BgSparkle; BgBDisperse = s.BgDisperse;
        BgBEmbers = s.BgEmbers; BgBFlow = s.BgFlow; BgBCol5R = s.BgCol5R;
        BgBCol5G = s.BgCol5G; BgBCol5B = s.BgCol5B; BgBCol6R = s.BgCol6R;
        BgBCol6G = s.BgCol6G; BgBCol6B = s.BgCol6B; BgBEmberSize = s.BgEmberSize;
        BgBReflect = s.BgReflect; BgBMatDisp = s.BgMatDisp; BgBAniso = s.BgAniso;
        BgBEnvSharp = s.BgEnvSharp; BgBEnvR = s.BgEnvR; BgBEnvG = s.BgEnvG;
        BgBEnvB = s.BgEnvB; BgBClearcoat = s.BgClearcoat; BgBGradType = s.BgGradType;
        BgBPatMode = s.BgPatMode; BgBPatStrength = s.BgPatStrength; BgBPatAngle = s.BgPatAngle;
        BgBUnivBase = s.UnivBase; BgBUnivNoise = s.UnivNoise; BgBUnivPattern = s.UnivPattern;
        BgBUnivBlend = s.UnivBlend; BgBUnivNoiseAmt = s.UnivNoiseAmt; BgBUnivNoiseScale = s.UnivNoiseScale;
        BgBUnivWarp = s.UnivWarp; BgBUnivDetail = s.UnivDetail;
        BgBPatColOverride = s.PatColOverride; BgBPatColMode = s.PatColMode;
        BgBPatColR = s.PatColR; BgBPatColG = s.PatColG; BgBPatColB = s.PatColB;
        BgBPatCol2R = s.PatCol2R; BgBPatCol2G = s.PatCol2G; BgBPatCol2B = s.PatCol2B;
        BgBPatCol3R = s.PatCol3R; BgBPatCol3G = s.PatCol3G; BgBPatCol3B = s.PatCol3B;
        BgBPatCol4R = s.PatCol4R; BgBPatCol4G = s.PatCol4G; BgBPatCol4B = s.PatCol4B;
        BgBPatCol5R = s.PatCol5R; BgBPatCol5G = s.PatCol5G; BgBPatCol5B = s.PatCol5B;
        BgBPatMat = s.PatMat; BgBPatMatR = s.PatMatR; BgBPatMatG = s.PatMatG;
        BgBPatMatB = s.PatMatB; BgBPatMatTint = s.PatMatTint;
    }

    public void CopyBTo(PluginConfig d)
    {
        d.BgBUnivPatBlend = BgBUnivPatBlend; d.BgBUnivPatStrength = BgBUnivPatStrength;
        d.BgBUnivCaustic = BgBUnivCaustic; d.BgBUnivShafts = BgBUnivShafts;
        d.BgBTopR = BgBTopR; d.BgBTopG = BgBTopG; d.BgBTopB = BgBTopB; d.BgBBotR = BgBBotR;
        d.BgBBotG = BgBBotG; d.BgBBotB = BgBBotB; d.BgBStyle = BgBStyle; d.BgBScale = BgBScale;
        d.BgBAngle = BgBAngle; d.BgBGrain = BgBGrain; d.BgBWarp = BgBWarp; d.BgBWarpAmt = BgBWarpAmt;
        d.BgBWarpScale = BgBWarpScale; d.BgBOffX = BgBOffX; d.BgBOffY = BgBOffY; d.BgBScaleY = BgBScaleY;
        d.BgBSharp = BgBSharp; d.BgBWarpX = BgBWarpX; d.BgBWarpY = BgBWarpY; d.BgBWarpAmt2 = BgBWarpAmt2;
        d.BgBWarpScale2 = BgBWarpScale2; d.BgBMidR = BgBMidR; d.BgBMidG = BgBMidG; d.BgBMidB = BgBMidB;
        d.BgBMetallic = BgBMetallic; d.BgBRoughness = BgBRoughness; d.BgBSpecular = BgBSpecular; d.BgBNormal = BgBNormal;
        d.BgBFresnel = BgBFresnel; d.BgBLightX = BgBLightX; d.BgBLightY = BgBLightY; d.BgBLightZ = BgBLightZ;
        d.BgBLightInt = BgBLightInt; d.BgBCol4R = BgBCol4R; d.BgBCol4G = BgBCol4G; d.BgBCol4B = BgBCol4B;
        d.BgBFbm = BgBFbm; d.BgBStars = BgBStars; d.BgBStarDensity = BgBStarDensity; d.BgBStarSize = BgBStarSize;
        d.BgBGlow = BgBGlow; d.BgBHueVar = BgBHueVar; d.BgBNebWarp = BgBNebWarp; d.BgBNebContrast = BgBNebContrast;
        d.BgBTwist = BgBTwist; d.BgBHaze = BgBHaze; d.BgBSparkle = BgBSparkle; d.BgBDisperse = BgBDisperse;
        d.BgBEmbers = BgBEmbers; d.BgBFlow = BgBFlow; d.BgBCol5R = BgBCol5R; d.BgBCol5G = BgBCol5G;
        d.BgBCol5B = BgBCol5B; d.BgBCol6R = BgBCol6R; d.BgBCol6G = BgBCol6G; d.BgBCol6B = BgBCol6B;
        d.BgBEmberSize = BgBEmberSize; d.BgBReflect = BgBReflect; d.BgBMatDisp = BgBMatDisp; d.BgBAniso = BgBAniso;
        d.BgBEnvSharp = BgBEnvSharp; d.BgBEnvR = BgBEnvR; d.BgBEnvG = BgBEnvG; d.BgBEnvB = BgBEnvB;
        d.BgBClearcoat = BgBClearcoat; d.BgBGradType = BgBGradType; d.BgBPatMode = BgBPatMode; d.BgBPatStrength = BgBPatStrength;
        d.BgBPatAngle = BgBPatAngle; d.BgBUnivBase = BgBUnivBase; d.BgBUnivNoise = BgBUnivNoise; d.BgBUnivPattern = BgBUnivPattern;
        d.BgBUnivBlend = BgBUnivBlend; d.BgBUnivNoiseAmt = BgBUnivNoiseAmt; d.BgBUnivNoiseScale = BgBUnivNoiseScale; d.BgBUnivWarp = BgBUnivWarp;
        d.BgBUnivDetail = BgBUnivDetail; d.BlendMode = BlendMode; d.BlendAngle = BlendAngle; d.BlendOffset = BlendOffset;
        d.BlendCx = BlendCx; d.BlendCy = BlendCy; d.BlendRadius = BlendRadius; d.BlendEllipse = BlendEllipse;
        d.BlendDepthSplit = BlendDepthSplit; d.BlendDepthRef = BlendDepthRef; d.BlendDepthBend = BlendDepthBend; d.BlendFeather = BlendFeather;
        d.BlendNoiseAmt = BlendNoiseAmt; d.BlendNoiseScale = BlendNoiseScale; d.BlendMatch = BlendMatch; d.BlendMix = BlendMix;
        d.BlendMixLevel = BlendMixLevel;
        d.BgBUnivHorizon = BgBUnivHorizon; d.BgBUnivGround = BgBUnivGround; d.BgBUnivOrb = BgBUnivOrb; d.BgBUnivOrbX = BgBUnivOrbX; d.BgBUnivOrbY = BgBUnivOrbY; d.BgBUnivOrbSize = BgBUnivOrbSize; d.BgBUnivRidges = BgBUnivRidges; d.BgBUnivParticle = BgBUnivParticle;
        d.BgBPatColOverride = BgBPatColOverride; d.BgBPatColMode = BgBPatColMode;
        d.BgBPatColR = BgBPatColR; d.BgBPatColG = BgBPatColG; d.BgBPatColB = BgBPatColB;
        d.BgBPatCol2R = BgBPatCol2R; d.BgBPatCol2G = BgBPatCol2G; d.BgBPatCol2B = BgBPatCol2B;
        d.BgBPatCol3R = BgBPatCol3R; d.BgBPatCol3G = BgBPatCol3G; d.BgBPatCol3B = BgBPatCol3B;
        d.BgBPatCol4R = BgBPatCol4R; d.BgBPatCol4G = BgBPatCol4G; d.BgBPatCol4B = BgBPatCol4B;
        d.BgBPatCol5R = BgBPatCol5R; d.BgBPatCol5G = BgBPatCol5G; d.BgBPatCol5B = BgBPatCol5B;
        d.BgBPatMat = BgBPatMat; d.BgBPatMatR = BgBPatMatR; d.BgBPatMatG = BgBPatMatG;
        d.BgBPatMatB = BgBPatMatB; d.BgBPatMatTint = BgBPatMatTint;
    }
    public void CopyBFrom(PluginConfig s)
    {
        BgBUnivPatBlend = s.BgBUnivPatBlend; BgBUnivPatStrength = s.BgBUnivPatStrength;
        BgBUnivCaustic = s.BgBUnivCaustic; BgBUnivShafts = s.BgBUnivShafts;
        BgBTopR = s.BgBTopR; BgBTopG = s.BgBTopG; BgBTopB = s.BgBTopB; BgBBotR = s.BgBBotR;
        BgBBotG = s.BgBBotG; BgBBotB = s.BgBBotB; BgBStyle = s.BgBStyle; BgBScale = s.BgBScale;
        BgBAngle = s.BgBAngle; BgBGrain = s.BgBGrain; BgBWarp = s.BgBWarp; BgBWarpAmt = s.BgBWarpAmt;
        BgBWarpScale = s.BgBWarpScale; BgBOffX = s.BgBOffX; BgBOffY = s.BgBOffY; BgBScaleY = s.BgBScaleY;
        BgBSharp = s.BgBSharp; BgBWarpX = s.BgBWarpX; BgBWarpY = s.BgBWarpY; BgBWarpAmt2 = s.BgBWarpAmt2;
        BgBWarpScale2 = s.BgBWarpScale2; BgBMidR = s.BgBMidR; BgBMidG = s.BgBMidG; BgBMidB = s.BgBMidB;
        BgBMetallic = s.BgBMetallic; BgBRoughness = s.BgBRoughness; BgBSpecular = s.BgBSpecular; BgBNormal = s.BgBNormal;
        BgBFresnel = s.BgBFresnel; BgBLightX = s.BgBLightX; BgBLightY = s.BgBLightY; BgBLightZ = s.BgBLightZ;
        BgBLightInt = s.BgBLightInt; BgBCol4R = s.BgBCol4R; BgBCol4G = s.BgBCol4G; BgBCol4B = s.BgBCol4B;
        BgBFbm = s.BgBFbm; BgBStars = s.BgBStars; BgBStarDensity = s.BgBStarDensity; BgBStarSize = s.BgBStarSize;
        BgBGlow = s.BgBGlow; BgBHueVar = s.BgBHueVar; BgBNebWarp = s.BgBNebWarp; BgBNebContrast = s.BgBNebContrast;
        BgBTwist = s.BgBTwist; BgBHaze = s.BgBHaze; BgBSparkle = s.BgBSparkle; BgBDisperse = s.BgBDisperse;
        BgBEmbers = s.BgBEmbers; BgBFlow = s.BgBFlow; BgBCol5R = s.BgBCol5R; BgBCol5G = s.BgBCol5G;
        BgBCol5B = s.BgBCol5B; BgBCol6R = s.BgBCol6R; BgBCol6G = s.BgBCol6G; BgBCol6B = s.BgBCol6B;
        BgBEmberSize = s.BgBEmberSize; BgBReflect = s.BgBReflect; BgBMatDisp = s.BgBMatDisp; BgBAniso = s.BgBAniso;
        BgBEnvSharp = s.BgBEnvSharp; BgBEnvR = s.BgBEnvR; BgBEnvG = s.BgBEnvG; BgBEnvB = s.BgBEnvB;
        BgBClearcoat = s.BgBClearcoat; BgBGradType = s.BgBGradType; BgBPatMode = s.BgBPatMode; BgBPatStrength = s.BgBPatStrength;
        BgBPatAngle = s.BgBPatAngle; BgBUnivBase = s.BgBUnivBase; BgBUnivNoise = s.BgBUnivNoise; BgBUnivPattern = s.BgBUnivPattern;
        BgBUnivBlend = s.BgBUnivBlend; BgBUnivNoiseAmt = s.BgBUnivNoiseAmt; BgBUnivNoiseScale = s.BgBUnivNoiseScale; BgBUnivWarp = s.BgBUnivWarp;
        BgBUnivDetail = s.BgBUnivDetail; BlendMode = s.BlendMode; BlendAngle = s.BlendAngle; BlendOffset = s.BlendOffset;
        BlendCx = s.BlendCx; BlendCy = s.BlendCy; BlendRadius = s.BlendRadius; BlendEllipse = s.BlendEllipse;
        BlendDepthSplit = s.BlendDepthSplit; BlendDepthRef = s.BlendDepthRef; BlendDepthBend = s.BlendDepthBend; BlendFeather = s.BlendFeather;
        BlendNoiseAmt = s.BlendNoiseAmt; BlendNoiseScale = s.BlendNoiseScale; BlendMatch = s.BlendMatch; BlendMix = s.BlendMix;
        BlendMixLevel = s.BlendMixLevel;
        BgBUnivHorizon = s.BgBUnivHorizon; BgBUnivGround = s.BgBUnivGround; BgBUnivOrb = s.BgBUnivOrb; BgBUnivOrbX = s.BgBUnivOrbX; BgBUnivOrbY = s.BgBUnivOrbY; BgBUnivOrbSize = s.BgBUnivOrbSize; BgBUnivRidges = s.BgBUnivRidges; BgBUnivParticle = s.BgBUnivParticle;
        BgBPatColOverride = s.BgBPatColOverride; BgBPatColMode = s.BgBPatColMode;
        BgBPatColR = s.BgBPatColR; BgBPatColG = s.BgBPatColG; BgBPatColB = s.BgBPatColB;
        BgBPatCol2R = s.BgBPatCol2R; BgBPatCol2G = s.BgBPatCol2G; BgBPatCol2B = s.BgBPatCol2B;
        BgBPatCol3R = s.BgBPatCol3R; BgBPatCol3G = s.BgBPatCol3G; BgBPatCol3B = s.BgBPatCol3B;
        BgBPatCol4R = s.BgBPatCol4R; BgBPatCol4G = s.BgBPatCol4G; BgBPatCol4B = s.BgBPatCol4B;
        BgBPatCol5R = s.BgBPatCol5R; BgBPatCol5G = s.BgBPatCol5G; BgBPatCol5B = s.BgBPatCol5B;
        BgBPatMat = s.BgBPatMat; BgBPatMatR = s.BgBPatMatR; BgBPatMatG = s.BgBPatMatG;
        BgBPatMatB = s.BgBPatMatB; BgBPatMatTint = s.BgBPatMatTint;
    }
    public bool EnFinalGrade { get; set; } = false;
    public float FinalExposure { get; set; } = 0f;
    public float FinalContrast { get; set; } = 0f;
    public float FinalSat { get; set; } = 0f;
    public float FinalTemp { get; set; } = 0f;
    public float FinalLift { get; set; } = 0f;
    public float FinalGamma { get; set; } = 0f;
    public float FinalGain { get; set; } = 0f;
    public int GroundMode { get; set; } = 0;
    public float GroundCastAngle { get; set; } = 1.2f;
    public float GroundCastLen { get; set; } = 0.35f;
    public float GroundLevel { get; set; } = 0.72f;
    public float GroundShadow { get; set; } = 0f;
    public float GroundRipple { get; set; } = 0.45f;
    public float GroundTintR { get; set; } = 0.06f; public float GroundTintG { get; set; } = 0.06f; public float GroundTintB { get; set; } = 0.08f;
    public float GroundShadowX { get; set; } = 0f;
    public float GroundShadowY { get; set; } = 0.82f;
    public float GroundShadowW { get; set; } = 0.22f;
    public float GroundShadowH { get; set; } = 0.05f;
    public float AnimSpeed { get; set; } = 0f;
    public const int ElemStride = 20;
    public float[] Elem { get; set; } = NewElems();
    private static float[] NewElems()
    {
        var e = new float[8 * ElemStride];
        for (int L = 0; L < 8; L++)
        {
            int i = L * ElemStride;
            e[i + 3] = 0.22f;
            e[i + 4] = 0.12f;
            e[i + 7] = 0.006f;
            e[i + 8] = 0.9f; e[i + 9] = 0.3f; e[i + 10] = 0.2f;
            e[i + 11] = 1f;
            e[i + 14] = 6f;
        }
        return e;
    }

    public bool EnForegroundOn { get; set; } = false;
    public int FgPlaceMode { get; set; } = 0;
    public float FgPlaceSoft { get; set; } = 0.25f;
    public float FgPlaceSize { get; set; } = 0.35f;
    public float FgPlaceAngle { get; set; } = 0f;
    public float FgOpacity { get; set; } = 0.7f;
    public int FgBlendMode { get; set; } = 0;
    public int FgDepthGate { get; set; } = 0;
    public int FgSeamMode { get; set; } = 0;
    public float FgSeamAngle { get; set; } = 0f;
    public float FgSeamOffset { get; set; } = 0f;
    public float FgSeamCx { get; set; } = 0.5f;
    public float FgSeamCy { get; set; } = 0.5f;
    public float FgSeamRadius { get; set; } = 0.4f;
    public float FgSeamEllipse { get; set; } = 1f;
    public float FgSeamDepthSplit { get; set; } = 0.5f;
    public float FgSeamDepthRef { get; set; } = 0.5f;
    public float FgSeamDepthBend { get; set; } = 0f;
    public float FgSeamFeather { get; set; } = 0.15f;
    public float FgSeamNoiseAmt { get; set; } = 0f;
    public float FgSeamNoiseScale { get; set; } = 1f;
    public int FgSeamMix { get; set; } = 0;
    public float FgSeamMixLevel { get; set; } = 0.5f;
    public float FgSeamMatch { get; set; } = 0f;
    public float[] FgField { get; set; } = new float[2 * FgFieldCount + 2];

    private const int FgFieldCount = 111;

    public PluginConfig()
    {
        CopyFgFromScratch(this, 0);
        CopyFgFromScratch(this, 1);
        FgField[6]  = 27;
        FgField[36] = 4;
        FgField[70] = 1;
        FgField[72] = 5;
        FgField[FgFieldCount + 6] = 0;
    }

    public void CopyFgToScratch(PluginConfig s, int idx) {
        int o = idx * FgFieldCount;
        s.BgTopR = FgField[o + 0];
        s.BgTopG = FgField[o + 1];
        s.BgTopB = FgField[o + 2];
        s.BgBotR = FgField[o + 3];
        s.BgBotG = FgField[o + 4];
        s.BgBotB = FgField[o + 5];
        s.BgStyle = (int)FgField[o + 6];
        s.BgScale = FgField[o + 7];
        s.BgAngle = FgField[o + 8];
        s.BgGrain = FgField[o + 9];
        s.BgWarp = (int)FgField[o + 10];
        s.BgWarpAmt = FgField[o + 11];
        s.BgWarpScale = FgField[o + 12];
        s.BgOffX = FgField[o + 13];
        s.BgOffY = FgField[o + 14];
        s.BgScaleY = FgField[o + 15];
        s.BgSharp = FgField[o + 16];
        s.BgWarpX = FgField[o + 17];
        s.BgWarpY = FgField[o + 18];
        s.BgWarpAmt2 = FgField[o + 19];
        s.BgWarpScale2 = FgField[o + 20];
        s.BgMidR = FgField[o + 21];
        s.BgMidG = FgField[o + 22];
        s.BgMidB = FgField[o + 23];
        s.BgMetallic = FgField[o + 24];
        s.BgRoughness = FgField[o + 25];
        s.BgSpecular = FgField[o + 26];
        s.BgNormal = FgField[o + 27];
        s.BgFresnel = FgField[o + 28];
        s.BgLightX = FgField[o + 29];
        s.BgLightY = FgField[o + 30];
        s.BgLightZ = FgField[o + 31];
        s.BgLightInt = FgField[o + 32];
        s.BgCol4R = FgField[o + 33];
        s.BgCol4G = FgField[o + 34];
        s.BgCol4B = FgField[o + 35];
        s.BgFbm = FgField[o + 36];
        s.BgStars = FgField[o + 37];
        s.BgStarDensity = FgField[o + 38];
        s.BgStarSize = FgField[o + 39];
        s.BgGlow = FgField[o + 40];
        s.BgHueVar = FgField[o + 41];
        s.BgNebWarp = FgField[o + 42];
        s.BgNebContrast = FgField[o + 43];
        s.BgTwist = FgField[o + 44];
        s.BgHaze = FgField[o + 45];
        s.BgSparkle = FgField[o + 46];
        s.BgDisperse = FgField[o + 47];
        s.BgEmbers = FgField[o + 48];
        s.BgFlow = FgField[o + 49];
        s.BgCol5R = FgField[o + 50];
        s.BgCol5G = FgField[o + 51];
        s.BgCol5B = FgField[o + 52];
        s.BgCol6R = FgField[o + 53];
        s.BgCol6G = FgField[o + 54];
        s.BgCol6B = FgField[o + 55];
        s.BgEmberSize = FgField[o + 56];
        s.BgReflect = FgField[o + 57];
        s.BgMatDisp = FgField[o + 58];
        s.BgAniso = FgField[o + 59];
        s.BgEnvSharp = FgField[o + 60];
        s.BgEnvR = FgField[o + 61];
        s.BgEnvG = FgField[o + 62];
        s.BgEnvB = FgField[o + 63];
        s.BgClearcoat = FgField[o + 64];
        s.BgGradType = (int)FgField[o + 65];
        s.BgPatMode = (int)FgField[o + 66];
        s.BgPatStrength = FgField[o + 67];
        s.BgPatAngle = FgField[o + 68];
        s.UnivBase = (int)FgField[o + 69];
        s.UnivNoise = (int)FgField[o + 70];
        s.UnivPattern = (int)FgField[o + 71];
        s.UnivBlend = (int)FgField[o + 72];
        s.UnivNoiseAmt = FgField[o + 73];
        s.UnivNoiseScale = FgField[o + 74];
        s.UnivWarp = FgField[o + 75];
        s.UnivDetail = FgField[o + 76];
        s.UnivHorizon = FgField[o + 77];
        s.UnivGround = (int)FgField[o + 78];
        s.UnivOrb = (int)FgField[o + 79];
        s.UnivOrbX = FgField[o + 80];
        s.UnivOrbY = FgField[o + 81];
        s.UnivOrbSize = FgField[o + 82];
        s.UnivRidges = FgField[o + 83];
        s.UnivParticle = (int)FgField[o + 84];
        s.UnivCaustic = FgField[o + 85];
        s.UnivShafts = FgField[o + 86];
        s.UnivPatBlend = (int)FgField[o + 87];
        s.UnivPatStrength = FgField[o + 88];
        s.PatColOverride = FgField[o + 89] != 0f;
        s.PatColMode = (int)FgField[o + 90];
        s.PatColR = FgField[o + 91];
        s.PatColG = FgField[o + 92];
        s.PatColB = FgField[o + 93];
        s.PatCol2R = FgField[o + 94];
        s.PatCol2G = FgField[o + 95];
        s.PatCol2B = FgField[o + 96];
        s.PatCol3R = FgField[o + 97];
        s.PatCol3G = FgField[o + 98];
        s.PatCol3B = FgField[o + 99];
        s.PatCol4R = FgField[o + 100];
        s.PatCol4G = FgField[o + 101];
        s.PatCol4B = FgField[o + 102];
        s.PatCol5R = FgField[o + 103];
        s.PatCol5G = FgField[o + 104];
        s.PatCol5B = FgField[o + 105];
        s.PatMat = (int)FgField[o + 106];
        s.PatMatR = FgField[o + 107];
        s.PatMatG = FgField[o + 108];
        s.PatMatB = FgField[o + 109];
        s.PatMatTint = FgField[o + 110];
    }
    public void CopyFgFromScratch(PluginConfig s, int idx) {
        int o = idx * FgFieldCount;
        FgField[o + 0] = s.BgTopR;
        FgField[o + 1] = s.BgTopG;
        FgField[o + 2] = s.BgTopB;
        FgField[o + 3] = s.BgBotR;
        FgField[o + 4] = s.BgBotG;
        FgField[o + 5] = s.BgBotB;
        FgField[o + 6] = s.BgStyle;
        FgField[o + 7] = s.BgScale;
        FgField[o + 8] = s.BgAngle;
        FgField[o + 9] = s.BgGrain;
        FgField[o + 10] = s.BgWarp;
        FgField[o + 11] = s.BgWarpAmt;
        FgField[o + 12] = s.BgWarpScale;
        FgField[o + 13] = s.BgOffX;
        FgField[o + 14] = s.BgOffY;
        FgField[o + 15] = s.BgScaleY;
        FgField[o + 16] = s.BgSharp;
        FgField[o + 17] = s.BgWarpX;
        FgField[o + 18] = s.BgWarpY;
        FgField[o + 19] = s.BgWarpAmt2;
        FgField[o + 20] = s.BgWarpScale2;
        FgField[o + 21] = s.BgMidR;
        FgField[o + 22] = s.BgMidG;
        FgField[o + 23] = s.BgMidB;
        FgField[o + 24] = s.BgMetallic;
        FgField[o + 25] = s.BgRoughness;
        FgField[o + 26] = s.BgSpecular;
        FgField[o + 27] = s.BgNormal;
        FgField[o + 28] = s.BgFresnel;
        FgField[o + 29] = s.BgLightX;
        FgField[o + 30] = s.BgLightY;
        FgField[o + 31] = s.BgLightZ;
        FgField[o + 32] = s.BgLightInt;
        FgField[o + 33] = s.BgCol4R;
        FgField[o + 34] = s.BgCol4G;
        FgField[o + 35] = s.BgCol4B;
        FgField[o + 36] = s.BgFbm;
        FgField[o + 37] = s.BgStars;
        FgField[o + 38] = s.BgStarDensity;
        FgField[o + 39] = s.BgStarSize;
        FgField[o + 40] = s.BgGlow;
        FgField[o + 41] = s.BgHueVar;
        FgField[o + 42] = s.BgNebWarp;
        FgField[o + 43] = s.BgNebContrast;
        FgField[o + 44] = s.BgTwist;
        FgField[o + 45] = s.BgHaze;
        FgField[o + 46] = s.BgSparkle;
        FgField[o + 47] = s.BgDisperse;
        FgField[o + 48] = s.BgEmbers;
        FgField[o + 49] = s.BgFlow;
        FgField[o + 50] = s.BgCol5R;
        FgField[o + 51] = s.BgCol5G;
        FgField[o + 52] = s.BgCol5B;
        FgField[o + 53] = s.BgCol6R;
        FgField[o + 54] = s.BgCol6G;
        FgField[o + 55] = s.BgCol6B;
        FgField[o + 56] = s.BgEmberSize;
        FgField[o + 57] = s.BgReflect;
        FgField[o + 58] = s.BgMatDisp;
        FgField[o + 59] = s.BgAniso;
        FgField[o + 60] = s.BgEnvSharp;
        FgField[o + 61] = s.BgEnvR;
        FgField[o + 62] = s.BgEnvG;
        FgField[o + 63] = s.BgEnvB;
        FgField[o + 64] = s.BgClearcoat;
        FgField[o + 65] = s.BgGradType;
        FgField[o + 66] = s.BgPatMode;
        FgField[o + 67] = s.BgPatStrength;
        FgField[o + 68] = s.BgPatAngle;
        FgField[o + 69] = s.UnivBase;
        FgField[o + 70] = s.UnivNoise;
        FgField[o + 71] = s.UnivPattern;
        FgField[o + 72] = s.UnivBlend;
        FgField[o + 73] = s.UnivNoiseAmt;
        FgField[o + 74] = s.UnivNoiseScale;
        FgField[o + 75] = s.UnivWarp;
        FgField[o + 76] = s.UnivDetail;
        FgField[o + 77] = s.UnivHorizon;
        FgField[o + 78] = s.UnivGround;
        FgField[o + 79] = s.UnivOrb;
        FgField[o + 80] = s.UnivOrbX;
        FgField[o + 81] = s.UnivOrbY;
        FgField[o + 82] = s.UnivOrbSize;
        FgField[o + 83] = s.UnivRidges;
        FgField[o + 84] = s.UnivParticle;
        FgField[o + 85] = s.UnivCaustic;
        FgField[o + 86] = s.UnivShafts;
        FgField[o + 87] = s.UnivPatBlend;
        FgField[o + 88] = s.UnivPatStrength;
        FgField[o + 89] = s.PatColOverride ? 1f : 0f;
        FgField[o + 90] = s.PatColMode;
        FgField[o + 91] = s.PatColR;
        FgField[o + 92] = s.PatColG;
        FgField[o + 93] = s.PatColB;
        FgField[o + 94] = s.PatCol2R;
        FgField[o + 95] = s.PatCol2G;
        FgField[o + 96] = s.PatCol2B;
        FgField[o + 97] = s.PatCol3R;
        FgField[o + 98] = s.PatCol3G;
        FgField[o + 99] = s.PatCol3B;
        FgField[o + 100] = s.PatCol4R;
        FgField[o + 101] = s.PatCol4G;
        FgField[o + 102] = s.PatCol4B;
        FgField[o + 103] = s.PatCol5R;
        FgField[o + 104] = s.PatCol5G;
        FgField[o + 105] = s.PatCol5B;
        FgField[o + 106] = s.PatMat;
        FgField[o + 107] = s.PatMatR;
        FgField[o + 108] = s.PatMatG;
        FgField[o + 109] = s.PatMatB;
        FgField[o + 110] = s.PatMatTint;
    }

    public bool FgBActive => FgField != null && FgField.Length > FgFieldCount + 6 && FgField[FgFieldCount + 6] > 0.5f;
    public void SetFgBActive(bool on)
    {
        if (FgField != null && FgField.Length > FgFieldCount + 6)
            FgField[FgFieldCount + 6] = on ? 27f : 0f;
    }

    public List<string> Pinned { get; set; } = new();

    public bool EnFrame { get; set; } = false;
    public float FrameCorner { get; set; } = 0.02f;
    public float FrameMat { get; set; } = 0.04f;
    public float FrameOuterCorner { get; set; } = 0.01f;
    public float FrameKeyline { get; set; } = 0.002f;
    public float FrameShadow { get; set; } = 0.35f;
    public float FrameBottom { get; set; } = 0f;
    public float FrameMatR { get; set; } = 0.96f;
    public float FrameMatG { get; set; } = 0.955f;
    public float FrameMatB { get; set; } = 0.94f;
    public float FrameKeyR { get; set; } = 0.15f;
    public float FrameKeyG { get; set; } = 0.15f;
    public float FrameKeyB { get; set; } = 0.16f;
    public bool FrameAlpha { get; set; } = false;
    public float FrameSmooth { get; set; } = 0.6f;
    public bool FrameMatInset { get; set; } = false;

    public bool EnEdge { get; set; } = false;
    public float EdgeErode { get; set; } = 0f;
    public float EdgeDespill { get; set; } = 0f;
    public float EdgeWrap { get; set; } = 0f;
    public float EdgeWrapWidth { get; set; } = 0.3f;
    public float FilmRolloff { get; set; } = 0f;
    public float FilmToe { get; set; } = 0f;
    public float FilmSat { get; set; } = 0f;
    public float LensVig { get; set; } = 0f;
    public float LensCornerSoft { get; set; } = 0f;
    public float ChromaRadial { get; set; } = 0f;
    public float BackdropLightAmt { get; set; } = 0f;
    public float BackdropLightX { get; set; } = 0.5f;
    public float BackdropLightY { get; set; } = 0.4f;
    public float BackdropLightSize { get; set; } = 0.55f;

    public float ZoneNear { get; set; } = 0f;
    public float ZoneNearSoft { get; set; } = 0.02f;
    public int ZoneWet { get; set; } = 2;
    public int ZoneBeauty { get; set; } = 2;
    public int ZoneSkin { get; set; } = 2;
    public int ZoneBacklight { get; set; } = 2;
    public int ZoneShadow { get; set; } = 4;
    public int ZoneBokeh { get; set; } = 4;
    public int ZoneBgPush { get; set; } = 4;
    public int ZoneBgBlur { get; set; } = 4;
    public int ZoneGobo { get; set; } = 7;
    public int ZoneSpot { get; set; } = 7;
    public int ZoneFrost { get; set; } = 7;
    public int ZoneStylize { get; set; } = 7;
    public int ZoneUnderwater { get; set; } = 7;
    public int ZoneVhs { get; set; } = 7;
    public int ZoneRim { get; set; } = 7;
    public int ZoneGround { get; set; } = 7;
    public int ZoneHalo { get; set; } = 7;
    public int ZoneCb { get; set; } = 7;
    public int ZoneTeal { get; set; } = 7;
    public int ZoneSplitTone { get; set; } = 7;
    public int ZoneBleach { get; set; } = 7;
    public int ZoneGradMap { get; set; } = 7;

    public float RimSplit { get; set; } = 0f;
    public float RimSplitAngle { get; set; } = 0f;
    public float RimSplitOffset { get; set; } = 0f;
    public float RimSplitSoft { get; set; } = 0.06f;
    public float Rim2R { get; set; } = 1f;
    public float Rim2G { get; set; } = 0.45f;
    public float Rim2B { get; set; } = 0.40f;
    public float Backlight2R { get; set; } = 1f;
    public float Backlight2G { get; set; } = 0.45f;
    public float Backlight2B { get; set; } = 0.40f;

    public int PatMat { get; set; } = 0;
    public float PatMatR { get; set; } = 1.00f;
    public float PatMatG { get; set; } = 0.78f;
    public float PatMatB { get; set; } = 0.36f;
    public float PatMatRough { get; set; } = 0.35f;
    public float PatMatSheen { get; set; } = 1.0f;
    public float PatMatPos { get; set; } = 0.60f;
    public float PatMatRange { get; set; } = 0.45f;
    public bool PatColOverride { get; set; } = false;
    public float PatColR { get; set; } = 0.90f;
    public float PatColG { get; set; } = 0.86f;
    public float PatColB { get; set; } = 0.78f;
    public int PatColMode { get; set; } = 0;
    public float PatCol2R { get; set; } = 0.42f;
    public float PatCol2G { get; set; } = 0.62f;
    public float PatCol2B { get; set; } = 0.86f;
    public float PatMatTint { get; set; } = 1.0f;
    public bool BgBPatColOverride { get; set; } = false;
    public int BgBPatColMode { get; set; } = 0;
    public float BgBPatColR { get; set; } = 0f;
    public float BgBPatColG { get; set; } = 0f;
    public float BgBPatColB { get; set; } = 0f;
    public float BgBPatCol2R { get; set; } = 0f;
    public float BgBPatCol2G { get; set; } = 0f;
    public float BgBPatCol2B { get; set; } = 0f;
    public float BgBPatCol3R { get; set; } = 0f;
    public float BgBPatCol3G { get; set; } = 0f;
    public float BgBPatCol3B { get; set; } = 0f;
    public float BgBPatCol4R { get; set; } = 0f;
    public float BgBPatCol4G { get; set; } = 0f;
    public float BgBPatCol4B { get; set; } = 0f;
    public float BgBPatCol5R { get; set; } = 0f;
    public float BgBPatCol5G { get; set; } = 0f;
    public float BgBPatCol5B { get; set; } = 0f;
    public int BgBPatMat { get; set; } = 0;
    public float BgBPatMatR { get; set; } = 0f;
    public float BgBPatMatG { get; set; } = 0f;
    public float BgBPatMatB { get; set; } = 0f;
    public float BgBPatMatTint { get; set; } = 0f;
    public float PatCol3R { get; set; } = 0.35f; public float PatCol3G { get; set; } = 0.80f; public float PatCol3B { get; set; } = 0.55f;
    public float PatCol4R { get; set; } = 0.95f; public float PatCol4G { get; set; } = 0.72f; public float PatCol4B { get; set; } = 0.30f;
    public float PatCol5R { get; set; } = 0.85f; public float PatCol5G { get; set; } = 0.35f; public float PatCol5B { get; set; } = 0.55f;

    public bool EnShadow { get; set; } = false;
    public float ShadowAmount { get; set; } = 0f;
    public float ShadowSpread { get; set; } = 0.25f;
    public float ShadowOffsetX { get; set; } = 0.15f;
    public float ShadowOffsetY { get; set; } = -0.12f;
    public float ShadowSoftness { get; set; } = 0.5f;
    public float ShadowR { get; set; } = 0.06f;
    public float ShadowG { get; set; } = 0.06f;
    public float ShadowB { get; set; } = 0.09f;
    public float ShadowContact { get; set; } = 0.35f;
    public float ShadowDepth { get; set; } = 0.10f;

    public bool EnGobo { get; set; } = false;
    public bool EnBeauty { get; set; } = false;
    public bool EnSkin { get; set; } = false;
    public bool EnBacklight { get; set; } = false;
    public bool EnSpot { get; set; } = false;
    public bool EnParticles { get; set; } = false;
    public int GoboPattern { get; set; } = 0;
    public float GoboAmount { get; set; } = 0f;
    public float GoboScale { get; set; } = 6f;
    public float GoboAngle { get; set; } = 0.3f;
    public float GoboSoft { get; set; } = 0.3f;
    public float BeautyAmount { get; set; } = 0f;
    public float BeautyRadius { get; set; } = 1f;
    public float BeautyGlow { get; set; } = 0.5f;
    public float SkinWarmth { get; set; } = 0f;
    public float SkinFlush { get; set; } = 0f;
    public float SkinTintR { get; set; } = 0.95f;
    public float SkinTintG { get; set; } = 0.55f;
    public float SkinTintB { get; set; } = 0.5f;
    public float BacklightAmount { get; set; } = 0f;
    public float BacklightWidth { get; set; } = 0.3f;
    public float BacklightR { get; set; } = 1f;
    public float BacklightG { get; set; } = 0.85f;
    public float BacklightB { get; set; } = 0.6f;
    public float SpotAmount { get; set; } = 0f;
    public float SpotX { get; set; } = 0.5f;
    public float SpotY { get; set; } = 0.45f;
    public float SpotRadius { get; set; } = 0.5f;
    public float SpotEllipse { get; set; } = 1.3f;
    public float SpotSoft { get; set; } = 0.4f;
    public float SpotAngle { get; set; } = 0f;
    public float SpotWarm { get; set; } = 0.3f;
    public int ParticleType { get; set; } = 0;
    public float ParticleAmount { get; set; } = 0f;
    public float ParticleSize { get; set; } = 0.5f;
    public float ParticleFall { get; set; } = 0.5f;
    public float ParticleR { get; set; } = 1f;
    public float ParticleG { get; set; } = 0.6f;
    public float ParticleB { get; set; } = 0.7f;
    public int BokehShape { get; set; } = 0;
    public float BokehAmount { get; set; } = 0f;
    public void SetElem(int slot, int type, float x, float y, float w, float thick,
                        float r, float g, float bcol, float inten,
                        float spin = 0f, bool front = true, float sides = 6f, float h = 0.12f, bool fill = false, float rot = 0f, float glow = 0f)
    {
        int i = slot * ElemStride;
        Elem[i + 0] = type; Elem[i + 1] = x; Elem[i + 2] = y; Elem[i + 3] = w; Elem[i + 4] = h;
        Elem[i + 5] = rot; Elem[i + 6] = spin; Elem[i + 7] = thick;
        Elem[i + 8] = r; Elem[i + 9] = g; Elem[i + 10] = bcol; Elem[i + 11] = inten;
        Elem[i + 12] = fill ? 1f : 0f; Elem[i + 13] = front ? 1f : 0f; Elem[i + 14] = sides; Elem[i + 15] = 0f;
        Elem[i + 16] = glow; Elem[i + 17] = 0f; Elem[i + 18] = 0f; Elem[i + 19] = 0f;
    }

    public bool AnyElementAnimated()
    {
        for (int L = 0; L < 8; L++)
        {
            float ty = Elem[L * ElemStride + 0];
            if (ty >= 0.5f && Math.Abs(Elem[L * ElemStride + 11]) > 0.001f && (Elem[L * ElemStride + 6] != 0f || (ty >= 10.5f && ty < 11.5f))) return true;
        }
        return false;
    }
    public float HudIntensity { get; set; } = 0f;
    public float HudR { get; set; } = 0.95f; public float HudG { get; set; } = 0.24f; public float HudB { get; set; } = 0.16f;
    public float HudReticle { get; set; } = 1f;
    public float HudRadar { get; set; } = 1f;
    public float HudScanline { get; set; } = 0.5f;
    public float HudHex { get; set; } = 0.5f;
    public float HudChroma { get; set; } = 0.5f;
    public float HudFlicker { get; set; } = 0.3f;
    public float HudScale { get; set; } = 0.5f;
    public float HudFrame { get; set; } = 1f;

    public bool ShowGuides { get; set; } = false;
    public bool GuideThirds { get; set; } = true;
    public bool GuideGolden { get; set; } = false;
    public bool GuideCenter { get; set; } = false;
    public bool GuideHorizon { get; set; } = false;
    public float GuideHorizonY { get; set; } = 0.5f;
    public float GuideOpacity { get; set; } = 1f;
    public int ExportAspect { get; set; } = 0;
    public int ExportScale { get; set; } = 1;
    public int ExportFormat { get; set; } = 0;
    public int ExportJpegQuality { get; set; } = 92;
    public bool ShowExportFrame { get; set; } = true;

    public bool DebugShowDepth { get; set; } = false;
    public bool DebugShowGate { get; set; } = false;
    public bool DebugShowClipping { get; set; } = false;

    public bool Bypass { get; set; } = false;

    public bool EnColorBalance { get; set; } = true;
    public bool EnTealOrange { get; set; } = true;
    public bool EnSplitTone { get; set; } = true;
    public bool EnBleach { get; set; } = true;
    public bool EnGradMap { get; set; } = true;
    public bool EnGlow { get; set; } = true;
    public bool EnLens { get; set; } = true;
    public bool EnWarp { get; set; } = true;
    public bool EnStylize { get; set; } = true;
    public bool EnFog { get; set; } = true;
    public bool EnSubjectIso { get; set; } = true;
    public bool EnRim { get; set; } = true;
    public bool EnBackdrop { get; set; } = true;
    public bool EnHalo { get; set; } = true;
    public bool EnFrost { get; set; } = true;
    public bool EnBgFill { get; set; } = true;
    public bool EnBgBlur { get; set; } = true;
    public bool EnTiltShift { get; set; } = true;
    public bool EnDof { get; set; } = true;
    public bool EnVhs { get; set; } = true;
    public bool EnUnderwater { get; set; } = true;
    public bool EnGround { get; set; } = true;
    public bool EnHud { get; set; } = true;
    public bool EnElements { get; set; } = true;
    public string[] ElemImages { get; set; } = new[] { "", "", "", "", "", "", "", "" };

    public bool EnText { get; set; } = true;
    public System.Collections.Generic.List<TextMarker> Texts { get; set; } = new();

    public void ApplyCosmicPreset()
    {
        EnBackdrop = true;
        BgStyle = 14;
        BgRecolor = 1f; BgRecolorStart = 0.06f;
        BgTopR = 0.02f; BgTopG = 0.03f; BgTopB = 0.12f;
        BgMidR = 0.28f; BgMidG = 0.16f; BgMidB = 0.48f;
        BgBotR = 0.12f; BgBotG = 0.22f; BgBotB = 0.60f;
        BgCol4R = 1.0f; BgCol4G = 0.9f; BgCol4B = 0.72f;
        BgScale = 5f; BgScaleY = 5f; BgFbm = 5f; BgHueVar = 0.55f;
        BgOffX = 0f; BgOffY = 0f; BgSharp = 0f;
        BgWarp = 0;
        BgStars = 0.85f; BgStarDensity = 55f; BgStarSize = 0.22f;
        BgGlow = 0.6f; BgVignette = 0.55f; BgVignetteSize = 0.75f; BgBright = 0.15f;
        BgNebWarp = 0f; BgNebContrast = 0f; BgVoidCore = 0f; BgVoidRing = 0f;
        BgTwist = 0f; BgHaze = 0f; BgSparkle = 0f; BgDisperse = 0f;
        BgRingWidth = 1f; BgRing2 = 0f; BgEmbers = 0f; BgFlow = 0f;
        BgCol5R = 0.25f; BgCol5G = 0.33f; BgCol5B = 0.515f;
        BgCol6R = 0.15f; BgCol6G = 0.19f; BgCol6B = 0.305f; BgEmberSize = 0.3f;
        VhsStatic = 0f; VhsScan = 0f; VhsScanCount = 300f; VhsDropout = 0f;
        VhsRoll = 0f; VhsRollPos = 0.3f; VhsDesat = 0f; VhsVignette = 0f;
        BgReflect = 0f; BgMatDisp = 0f; BgAniso = 0f; BgEnvSharp = 0.5f;
        BgEnvR = 0.6f; BgEnvG = 0.7f; BgEnvB = 1.0f; BgClearcoat = 0f;
        BgCausticAmt = 0f; BgShafts = 0f; BgBubbles = 0f;
        UwTint = 0f; UwTintR = 0.10f; UwTintG = 0.38f; UwTintB = 0.45f;
        UwCaustic = 0f; UwMotes = 0f; UwShafts = 0f; UwFog = 0f;
        GroundShadow = 0f;
        AnimSpeed = 0f;
        HudIntensity = 0f;
    }

    public void ApplyVoidPreset()
    {
        EnBackdrop = true;
        BgStyle = 16;
        BgRecolor = 1f; BgRecolorStart = 0.05f;
        BgTopR = 0.020f; BgTopG = 0.012f; BgTopB = 0.047f;
        BgCol5R = 0.102f; BgCol5G = 0.043f; BgCol5B = 0.227f;
        BgMidR = 0.227f; BgMidG = 0.078f; BgMidB = 0.400f;
        BgCol6R = 0.416f; BgCol6G = 0.165f; BgCol6B = 0.620f;
        BgBotR = 0.608f; BgBotG = 0.360f; BgBotB = 0.902f;
        BgCol4R = 0.541f; BgCol4G = 0.420f; BgCol4B = 1.0f;
        BgScale = 3f; BgScaleY = 3f; BgFbm = 6f; BgHueVar = 0.35f;
        BgNebWarp = 0.6f; BgNebContrast = 0.6f;
        BgFlow = 0.2f; BgTwist = 0.35f; BgAngle = 0f;
        BgOffX = 0f; BgOffY = 0f; BgSharp = 0f; BgWarp = 0;
        BgHaze = 0.1f;
        BgStars = 0.25f; BgStarDensity = 55f; BgStarSize = 0.1f; BgSparkle = 0.4f;
        BgEmbers = 0.28f; BgEmberSize = 0.3f;
        BgGlow = 0.4f; BgVignette = 0f;
        BgVoidCore = 0f; BgVoidRing = 0f; BgRing2 = 0f;
        BgRingWidth = 1f; BgDisperse = 0f; BgVignetteSize = 0.6f;
        BgBright = -0.18f;

        Exposure = -0.3f;
        BlackPoint = 0.05f; WhitePoint = 1.4f;
        Contrast = 0.12f; Saturation = -0.14f; Vibrance = 0.05f;
        Lift = 0f; Gamma = 0f; Gain = 0f;
        Vignette = 0.5f;
        EnColorBalance = true; ColorBalance = 0.6f;
        CbShadowR = 0.46f; CbShadowG = 0.48f; CbShadowB = 0.55f;
        CbMidR = 0.49f;  CbMidG = 0.49f;  CbMidB = 0.52f;
        CbHighR = 0.51f; CbHighG = 0.50f; CbHighB = 0.50f;

        EnGlow = true; BloomAmount = 0.05f; BloomThreshold = 0.92f; BloomRadius = 3f;
        Halation = 0f; GodrayAmount = 0f; Orton = 0f; Glamour = 0f; AnamAmount = 0f;
        EnRim = true; RimStrength = 0.4f; RimThreshold = 0.06f; RimWidth = 1f;
        RimR = 0.45f; RimG = 0.32f; RimB = 0.95f;
        SubjectPop = 0.1f;
    }

    public void ApplyHorrorPreset()
    {
        EnVhs = true;
        VhsStatic = 0.35f; VhsScan = 0.5f; VhsScanCount = 320f; VhsDropout = 0.4f;
        VhsRoll = 0.5f; VhsRollPos = 0.22f; VhsDesat = 0.7f; VhsVignette = 0.65f;

        EnLens = true; Chroma = 0.45f; Grain = 0.35f; Vignette = 0.4f;
        Prism = 0f; LeakAmt = 0f; WashAmount = 0f; Letterbox = 0f;

        Exposure = -0.05f; Contrast = 0.22f; Saturation = -0.35f; Vibrance = 0f;
        BlackPoint = 0.06f; WhitePoint = 1.05f; Lift = 0f; Gamma = 0f; Gain = 0f;
        EnColorBalance = true; ColorBalance = 0.5f;
        CbShadowR = 0.48f; CbShadowG = 0.53f; CbShadowB = 0.49f;
        CbMidR = 0.5f;   CbMidG = 0.51f;  CbMidB = 0.49f;
        CbHighR = 0.5f;  CbHighG = 0.5f;  CbHighB = 0.5f;

        EnBackdrop = false; BgRecolor = 0f;
        EnGlow = true; BloomAmount = 0f; Halation = 0f; GodrayAmount = 0f; Orton = 0f; Glamour = 0f; AnamAmount = 0f;
        EnRim = false; RimStrength = 0f;
    }

    public void ApplyHellfirePreset()
    {
        EnBackdrop = true;
        BgStyle = 14;
        BgRecolor = 1f; BgRecolorStart = 0.05f;
        BgTopR = 0.040f; BgTopG = 0.010f; BgTopB = 0.005f;
        BgCol5R = 0.230f; BgCol5G = 0.030f; BgCol5B = 0.010f;
        BgMidR = 0.550f; BgMidG = 0.100f; BgMidB = 0.010f;
        BgCol6R = 0.910f; BgCol6G = 0.350f; BgCol6B = 0.040f;
        BgBotR = 1.0f;   BgBotG = 0.820f; BgBotB = 0.280f;
        BgCol4R = 1.0f;  BgCol4G = 0.600f; BgCol4B = 0.180f;
        BgScale = 5f; BgScaleY = 5f; BgFbm = 6f; BgHueVar = 0.18f;
        BgNebWarp = 0.9f; BgNebContrast = 0.45f;
        BgFlow = 0.4f; BgTwist = 0.12f; BgAngle = 0f;
        BgOffX = 0f; BgOffY = 0f; BgSharp = 0f; BgWarp = 0;
        BgHaze = 0.35f;
        BgStars = 0f; BgSparkle = 0f;
        BgEmbers = 0.75f; BgEmberSize = 0.24f;
        BgGlow = 1.0f; BgVignette = 0f;
        AnimSpeed = 0.5f;
        BgVoidCore = 0f; BgVoidRing = 0f; BgRing2 = 0f;
        BgBright = 0.05f;
        BgGrain = 0.1f;
        BgNormal = 0f; BgSpecular = 0f; BgMetallic = 0f; BgReflect = 0f;
        BgFresnel = 0f; BgClearcoat = 0f; BgLightInt = 0f;
        BgCausticAmt = 0f; BgShafts = 0f; BgBubbles = 0f;
    }

    public void ApplyAquariumPreset()
    {
        EnBackdrop = true;
        BgStyle = 17;
        BgRecolor = 1f; BgRecolorStart = 0.05f;
        BgTopR = 0.02f;  BgTopG = 0.165f; BgTopB = 0.227f;
        BgCol5R = 0.04f; BgCol5G = 0.29f;  BgCol5B = 0.37f;
        BgMidR = 0.082f; BgMidG = 0.478f; BgMidB = 0.588f;
        BgCol6R = 0.231f; BgCol6G = 0.65f; BgCol6B = 0.769f;
        BgBotR = 0.65f;  BgBotG = 0.894f; BgBotB = 0.941f;
        BgCol4R = 0.874f; BgCol4G = 0.968f; BgCol4B = 1.0f;
        BgScale = 4f; BgScaleY = 4f; BgFbm = 5f; BgHueVar = 0f;
        BgNebWarp = 0.5f; BgNebContrast = 0f; BgFlow = 0f; BgTwist = 0f;
        BgOffX = 0f; BgOffY = 0f; BgSharp = 0f; BgWarp = 0; BgAngle = 0f;
        BgHaze = 0f; BgStars = 0f; BgSparkle = 0f; BgEmbers = 0f;
        BgVoidCore = 0f; BgVoidRing = 0f; BgRing2 = 0f; BgVignette = 0f;
        BgGlow = 0.3f; BgBright = 0.05f; BgGrain = 0f;
        BgNormal = 0f; BgSpecular = 0f; BgMetallic = 0f; BgReflect = 0f; BgFresnel = 0f; BgClearcoat = 0f; BgLightInt = 0f;
        BgCausticAmt = 0.6f; BgShafts = 0.5f; BgBubbles = 0.5f; BgEmberSize = 0.35f;
        EnUnderwater = true;
        UwTint = 0.4f; UwTintR = 0.10f; UwTintG = 0.38f; UwTintB = 0.45f;
        UwCaustic = 0.4f; UwMotes = 0.4f; UwShafts = 0.3f; UwFog = 0.4f;
    }

    public void ApplyAuroraPreset()
    {
        EnBackdrop = true;
        BgStyle = 18;
        BgRecolor = 1f; BgRecolorStart = 0.05f;
        BgTopR = 0.020f; BgTopG = 0.039f; BgTopB = 0.094f;
        BgCol5R = 0.23f; BgCol5G = 0.94f; BgCol5B = 0.54f;
        BgMidR = 0.208f; BgMidG = 0.878f; BgMidB = 0.816f;
        BgCol6R = 0.478f; BgCol6G = 0.36f; BgCol6B = 0.94f;
        BgBotR = 0.94f;  BgBotG = 0.376f; BgBotB = 0.753f;
        BgCol4R = 0.81f; BgCol4G = 0.91f; BgCol4B = 1.0f;
        BgScale = 6f; BgScaleY = 6f; BgFbm = 5f; BgHueVar = 0.2f;
        BgNebWarp = 0.6f; BgNebContrast = 0f; BgFlow = 0f;
        BgTwist = 0.2f; BgAngle = 0f;
        BgOffX = 0f; BgOffY = 0f; BgSharp = 0f; BgWarp = 0;
        BgHaze = 0f; BgEmbers = 0f;
        BgStars = 0.5f; BgStarDensity = 62f; BgStarSize = 0.11f; BgSparkle = 0.4f;
        BgGlow = 0.6f; BgVignette = 0f;
        BgVoidCore = 0f; BgVoidRing = 0f; BgRing2 = 0f;
        BgBright = 0f;
        BgNormal = 0f; BgSpecular = 0f; BgMetallic = 0f; BgReflect = 0f; BgFresnel = 0f; BgClearcoat = 0f; BgLightInt = 0f;
        BgCausticAmt = 0f; BgShafts = 0f; BgBubbles = 0f;
    }

    public void ApplySynthwavePreset()
    {
        EnBackdrop = true;
        BgStyle = 19;
        BgRecolor = 1f; BgRecolorStart = 0.05f;
        BgTopR = 0.10f;  BgTopG = 0.043f; BgTopB = 0.227f;
        BgCol5R = 0.294f; BgCol5G = 0.114f; BgCol5B = 0.549f;
        BgMidR = 0.69f;  BgMidG = 0.165f; BgMidB = 0.62f;
        BgCol6R = 0.94f; BgCol6G = 0.314f; BgCol6B = 0.604f;
        BgBotR = 1.0f;   BgBotG = 0.62f;  BgBotB = 0.353f;
        BgCol4R = 0.208f; BgCol4G = 0.94f; BgCol4B = 0.94f;
        BgScale = 9f; BgScaleY = 8f;
        BgOffX = 0f; BgOffY = 0f;
        BgGlow = 0.7f;
        BgStars = 0.35f; BgStarDensity = 55f; BgStarSize = 0.1f; BgSparkle = 0.3f;
        BgBright = 0f; BgHueVar = 0f; BgSharp = 0f; BgWarp = 0;
        BgNebWarp = 0f; BgNebContrast = 0f; BgFlow = 0f; BgTwist = 0f; BgHaze = 0f; BgEmbers = 0f;
        BgVoidCore = 0f; BgVoidRing = 0f; BgRing2 = 0f; BgVignette = 0f;
        BgNormal = 0f; BgSpecular = 0f; BgMetallic = 0f; BgReflect = 0f; BgFresnel = 0f; BgClearcoat = 0f; BgLightInt = 0f;
        BgCausticAmt = 0f; BgShafts = 0f; BgBubbles = 0f;
    }

    public void ApplyBloodMoonPreset()
    {
        EnBackdrop = true;
        BgStyle = 20;
        BgRecolor = 1f; BgRecolorStart = 0.05f;
        BgTopR = 0.04f;  BgTopG = 0.008f; BgTopB = 0.012f;
        BgCol5R = 0.11f; BgCol5G = 0.02f;  BgCol5B = 0.03f;
        BgMidR = 0.227f; BgMidG = 0.039f; BgMidB = 0.055f;
        BgCol6R = 0.369f; BgCol6G = 0.063f; BgCol6B = 0.078f;
        BgBotR = 0.557f; BgBotG = 0.14f;  BgBotB = 0.11f;
        BgCol4R = 0.78f; BgCol4G = 0.27f;  BgCol4B = 0.17f;
        BgScale = 5f; BgScaleY = 12f; BgFbm = 5f;
        BgNebWarp = 0.55f; BgNebContrast = 0.65f; BgTwist = 0.2f;
        BgOffX = 0f; BgOffY = 0f; BgHueVar = 0f; BgSharp = 0f; BgWarp = 0; BgFlow = 0f;
        BgGlow = 0.5f;
        BgStars = 0.25f; BgStarDensity = 42f; BgStarSize = 0.09f; BgSparkle = 0.2f;
        BgBright = -0.05f; BgHaze = 0f; BgEmbers = 0f;
        BgVoidCore = 0f; BgVoidRing = 0f; BgRing2 = 0f; BgVignette = 0f;
        BgNormal = 0f; BgSpecular = 0f; BgMetallic = 0f; BgReflect = 0f; BgFresnel = 0f; BgClearcoat = 0f; BgLightInt = 0f;
        BgCausticAmt = 0f; BgShafts = 0f; BgBubbles = 0f;
    }

    public void ApplyTempeMoonPreset()
    {
        EnBackdrop = true;
        BgStyle = 21;
        BgRecolor = 1f; BgRecolorStart = 0.05f;
        BgTopR = 0.02f;  BgTopG = 0.008f; BgTopB = 0.024f;
        BgCol5R = 0.07f; BgCol5G = 0.016f; BgCol5B = 0.047f;
        BgMidR = 0.165f; BgMidG = 0.031f; BgMidB = 0.055f;
        BgCol6R = 0.263f; BgCol6G = 0.039f; BgCol6B = 0.062f;
        BgBotR = 0.078f; BgBotG = 0.024f; BgBotB = 0.039f;
        BgCol4R = 0.77f; BgCol4G = 0.11f;  BgCol4B = 0.11f;
        BgScale = 4f; BgScaleY = 11f; BgFbm = 5f;
        BgNebWarp = 0.5f; BgNebContrast = 0.5f; BgTwist = 0.15f;
        BgOffX = 0f; BgOffY = 0f; BgHueVar = 0f; BgSharp = 0f; BgWarp = 0; BgFlow = 0f;
        BgGlow = 0.6f;
        BgStars = 0.3f; BgStarDensity = 46f; BgStarSize = 0.09f; BgSparkle = 0.25f;
        BgBright = -0.05f; BgHaze = 0.5f; BgEmbers = 0f;
        EnGround = true;
        GroundLevel = 0.82f;
        GroundShadow = 0.6f; GroundRipple = 0.4f;
        GroundTintR = 0.04f; GroundTintG = 0.05f; GroundTintB = 0.09f;
        BgVoidCore = 0f; BgVoidRing = 0f; BgRing2 = 0f; BgVignette = 0f;
        BgNormal = 0f; BgSpecular = 0f; BgMetallic = 0f; BgReflect = 0f; BgFresnel = 0f; BgClearcoat = 0f; BgLightInt = 0f;
        BgCausticAmt = 0f; BgShafts = 0f; BgBubbles = 0f;
    }

    public void ApplyForgePreset()
    {
        EnBackdrop = true;
        BgStyle = 22;
        BgRecolor = 1f; BgRecolorStart = 0.05f;
        BgTopR = 0.04f;  BgTopG = 0.027f; BgTopB = 0.02f;
        BgCol5R = 0.11f; BgCol5G = 0.055f; BgCol5B = 0.024f;
        BgMidR = 0.353f; BgMidG = 0.118f; BgMidB = 0.03f;
        BgCol6R = 0.541f; BgCol6G = 0.227f; BgCol6B = 0.055f;
        BgBotR = 0.816f; BgBotG = 0.4f;   BgBotB = 0.11f;
        BgCol4R = 1.0f;  BgCol4G = 0.70f;  BgCol4B = 0.28f;
        BgScale = 5f; BgScaleY = 8f; BgFbm = 5f;
        BgOffX = 0f; BgOffY = -0.05f;
        BgGlow = 0.55f; BgHaze = 0.35f;
        BgNebWarp = 0.5f; BgTwist = 0.2f; BgFlow = 0.3f;
        BgEmbers = 0.55f; BgEmberSize = 0.4f;
        BgStars = 0f; BgSparkle = 0f; BgHueVar = 0f; BgSharp = 0f; BgWarp = 0; BgBright = 0f;
        BgVoidCore = 0f; BgVoidRing = 0f; BgRing2 = 0f; BgVignette = 0f;
        BgNormal = 0f; BgSpecular = 0f; BgMetallic = 0f; BgReflect = 0f; BgFresnel = 0f; BgClearcoat = 0f; BgLightInt = 0f;
        BgCausticAmt = 0f; BgShafts = 0f; BgBubbles = 0f;
        EnGround = true;
        GroundLevel = 0.74f;
        GroundShadow = 0.4f;
        GroundTintR = 0.09f; GroundTintG = 0.05f; GroundTintB = 0.03f;
    }

    public void ApplyArtisanPreset()
    {
        EnBackdrop = true;
        BgStyle = 23;
        BgRecolor = 1f; BgRecolorStart = 0.05f;
        BgTopR = 0.078f; BgTopG = 0.094f; BgTopB = 0.227f;
        BgCol5R = 0.165f; BgCol5G = 0.20f;  BgCol5B = 0.345f;
        BgMidR = 0.29f;  BgMidG = 0.333f; BgMidB = 0.47f;
        BgCol6R = 0.60f;  BgCol6G = 0.52f;  BgCol6B = 0.52f;
        BgBotR = 0.847f; BgBotG = 0.706f; BgBotB = 0.541f;
        BgCol4R = 0.94f; BgCol4G = 0.85f;  BgCol4B = 0.70f;
        BgScale = 4f; BgFbm = 4f;
        BgOffX = 0f; BgOffY = 0f;
        BgGlow = 0.5f; BgTwist = 0.1f; BgFlow = 0.15f;
        BgStars = 0f; BgSparkle = 0f; BgEmbers = 0f; BgHaze = 0f; BgHueVar = 0f; BgSharp = 0f; BgWarp = 0; BgBright = 0f;
        BgNebWarp = 0f; BgNebContrast = 0f; BgVoidCore = 0f; BgVoidRing = 0f; BgRing2 = 0f; BgVignette = 0f;
        BgNormal = 0f; BgSpecular = 0f; BgMetallic = 0f; BgReflect = 0f; BgFresnel = 0f; BgClearcoat = 0f; BgLightInt = 0f;
        BgCausticAmt = 0f; BgShafts = 0f; BgBubbles = 0f;
        EnGround = true;
        GroundLevel = 0.82f; GroundShadow = 0.35f;
        GroundTintR = 0.06f; GroundTintG = 0.06f; GroundTintB = 0.07f;
    }

    public void ApplySunsetPreset()
    {
        EnBackdrop = true;
        BgStyle = 24;
        BgRecolor = 1f; BgRecolorStart = 0.05f;
        BgTopR = 0.14f;  BgTopG = 0.106f; BgTopB = 0.306f;
        BgCol5R = 0.29f; BgCol5G = 0.165f; BgCol5B = 0.43f;
        BgMidR = 0.60f;  BgMidG = 0.227f; BgMidB = 0.44f;
        BgCol6R = 0.878f; BgCol6G = 0.416f; BgCol6B = 0.282f;
        BgBotR = 0.969f; BgBotG = 0.784f; BgBotB = 0.353f;
        BgCol4R = 1.0f;  BgCol4G = 0.80f;  BgCol4B = 0.45f;
        BgScaleY = 8f; BgScale = 5f; BgFbm = 4f;
        BgOffX = 0f; BgOffY = 0f;
        BgGlow = 0.6f; BgTwist = 0.1f; BgFlow = 0.2f;
        BgNebWarp = 0.4f; BgNebContrast = 0.4f;
        BgStars = 0f; BgSparkle = 0f; BgEmbers = 0f; BgHaze = 0f; BgHueVar = 0f; BgSharp = 0f; BgWarp = 0; BgBright = 0f;
        BgVoidCore = 0f; BgVoidRing = 0f; BgRing2 = 0f; BgVignette = 0f;
        BgNormal = 0f; BgSpecular = 0f; BgMetallic = 0f; BgReflect = 0f; BgFresnel = 0f; BgClearcoat = 0f; BgLightInt = 0f;
        BgCausticAmt = 0f; BgShafts = 0f; BgBubbles = 0f;
        EnGround = true;
        GroundLevel = 0.85f; GroundShadow = 0.4f;
        GroundTintR = 0.08f; GroundTintG = 0.05f; GroundTintB = 0.04f;
    }

    public void ApplySinEaterPreset()
    {
        EnBackdrop = true;
        BgStyle = 25;
        BgRecolor = 1f; BgRecolorStart = 0.05f;
        BgTopR = 0.024f; BgTopG = 0.039f; BgTopB = 0.071f;
        BgCol5R = 0.047f; BgCol5G = 0.086f; BgCol5B = 0.133f;
        BgMidR = 0.078f; BgMidG = 0.141f; BgMidB = 0.188f;
        BgCol6R = 0.118f; BgCol6G = 0.188f; BgCol6B = 0.220f;
        BgBotR = 0.149f; BgBotG = 0.227f; BgBotB = 0.251f;
        BgCol4R = 0.776f; BgCol4G = 0.863f; BgCol4B = 0.910f;
        BgScale = 5f; BgScaleY = 6f; BgFbm = 5f;
        BgOffX = 0.28f; BgOffY = -0.18f;
        BgGlow = 0.3f;
        BgNebContrast = 0.3f; BgNebWarp = 0.5f;
        BgHaze = 0.5f;
        BgEmbers = 0.4f; BgEmberSize = 0.35f; BgFlow = 0.2f; BgTwist = 0.1f;
        BgGrain = 0.2f;
        BgDisperse = 0.4f; BgSparkle = 0.4f;
        BgStars = 0f; BgHueVar = 0f; BgSharp = 0f; BgWarp = 0; BgBright = 0f;
        BgVoidCore = 0f; BgVoidRing = 0f; BgRing2 = 0f; BgVignette = 0f;
        BgNormal = 0f; BgSpecular = 0f; BgMetallic = 0f; BgReflect = 0f; BgFresnel = 0f; BgClearcoat = 0f; BgLightInt = 0f;
        BgCausticAmt = 0f; BgShafts = 0f; BgBubbles = 0f;
        EnGround = true;
        GroundLevel = 0.85f; GroundShadow = 0.3f;
        GroundTintR = 0.05f; GroundTintG = 0.06f; GroundTintB = 0.08f;
    }

    public void ApplyMagitekHudPreset()
    {
        EnBackdrop = false; BgStyle = 0; BgRecolor = 0f;
        EnHud = true;
        HudIntensity = 1f;
        HudR = 0.95f; HudG = 0.24f; HudB = 0.16f;
        HudFrame = 0f; HudReticle = 0f; HudRadar = 0f;
        HudHex = 0.5f; HudScanline = 0.5f; HudFlicker = 0.3f; HudChroma = 0.6f; HudScale = 0.5f;
        EnElements = true;
        SetElem(0, 10, 0f, 0f, 0.05f, 0.0025f, 0.95f, 0.24f, 0.16f, 1.1f);
        SetElem(1, 7, 0f, 0f, 0.085f, 0.0025f, 0.95f, 0.30f, 0.18f, 0.9f, spin: 0.4f, sides: 9f);
        SetElem(2, 3, 0f, 0f, 0.12f, 0.002f, 0.80f, 0.25f, 0.18f, 0.55f, spin: -0.15f, sides: 6f);
        SetElem(3, 9, 0f, 0f, 0.45f, 0.0025f, 0.95f, 0.24f, 0.16f, 0.9f, h: 0.45f);
        SetElem(4, 12, 0f, -0.40f, 0.36f, 0.0022f, 0.95f, 0.24f, 0.16f, 0.85f, h: 0.05f);
        SetElem(5, 13, -0.42f, -0.34f, 0.07f, 0.0022f, 0.95f, 0.24f, 0.16f, 0.8f);
        SetElem(6, 11, -0.40f, 0.34f, 0.09f, 0.0022f, 0.95f, 0.30f, 0.18f, 0.85f);
        Saturation = 0.9f; Contrast = 1.08f; Vignette = 0.25f;
        AnimSpeed = 0f;
    }

    public void ApplyGposeViewfinderPreset()
    {
        EnBackdrop = false; BgStyle = 0; BgRecolor = 0f;
        EnHud = false;
        EnElements = true;
        float wr = 0.96f, wg = 0.96f, wb = 0.94f;
        SetElem(0, 9, 0f, 0f, 0.46f, 0.0018f, wr, wg, wb, 0.85f, h: 0.46f);
        SetElem(1, 8, 0f, -0.1667f, 0.95f, 0.0011f, wr, wg, wb, 0.35f);
        SetElem(2, 8, 0f, 0.1667f, 0.95f, 0.0011f, wr, wg, wb, 0.35f);
        SetElem(3, 8, -0.1667f, 0f, 0.55f, 0.0011f, wr, wg, wb, 0.35f, rot: 1.5708f);
        SetElem(4, 8, 0.1667f, 0f, 0.55f, 0.0011f, wr, wg, wb, 0.35f, rot: 1.5708f);
        SetElem(5, 2, -0.43f, -0.42f, 0.008f, 0f, 0.95f, 0.20f, 0.15f, 1f);
        SetElem(6, 13, -0.44f, 0.38f, 0.06f, 0.0022f, 1.0f, 0.72f, 0.28f, 0.7f);
        Vignette = 0.12f;
    }

    public void ApplyAoeTelegraphPreset()
    {
        EnBackdrop = false; BgStyle = 0; BgRecolor = 0f;
        EnHud = false;
        EnElements = true;
        SetElem(0, 14, 0f, 0.12f, 0.34f, 0.004f, 1.0f, 0.42f, 0.14f, 1.1f, front: false, h: 0.5f);
        Vignette = 0.1f;
    }

    public void ApplyAetherbloomPreset()
    {
        EnBackdrop = false; BgRecolor = 0f; BgStyle = 0; BgBStyle = 0;
        EnBgFill = false; BgFill = 0f;
        EnForegroundOn = false;
        EnEdge = false;
        EnShadow = false; EnGround = false; EnHalo = false;
        EnGobo = false; EnSpot = false; EnParticles = false;
        EnFrost = false; EnVhs = false; EnUnderwater = false; EnHud = false; EnWet = false;
        EnStylize = false; EnWarp = false; EnFog = false; FogStrength = 0f;

        EnSubjectIso = false; BgPushStrength = 0f;
        EnBgBlur = false; BgBlur = 0f;
        EnDof = false; EnTiltShift = false; TiltAmt = 0f;

        EnGlow = true;
        BloomAmount = 0.85f; BloomThreshold = 0.50f; BloomRadius = 5f;
        Halation = 0.28f; HalationR = 1.0f; HalationG = 0.86f; HalationB = 0.80f;
        Orton = 0.10f; SoftBlurRadius = 4f;
        AnamAmount = 0.26f; AnamThreshold = 0.60f; AnamLength = 14f;
        AnamR = 0.70f; AnamG = 0.85f; AnamB = 1.00f;
        GodrayAmount = 0f;

        EnLens = true;
        FilmRolloff = 0.65f; FilmSat = 0.08f; FilmToe = 0.16f;
        LensVig = 0.26f; LensCornerSoft = 0.14f;
        Vignette = 0.16f; Grain = 0.07f; Chroma = 0.07f; ChromaRadial = 0.90f;
        Sharpen = 0.12f; Clarity = 0.14f;
        Letterbox = 0f; Prism = 0f; LeakAmt = 0f; WashAmount = 0f;

        EnRim = true; RimStrength = 0.08f; RimWidth = 2f; SubjectPop = 0.12f; RimSplit = 0f;
        RimR = 1.0f; RimG = 0.95f; RimB = 0.90f;
        EnSkin = true; SkinWarmth = 0.34f; SkinFlush = 0.14f;
        EnBeauty = true; BeautyAmount = 0.16f; BeautyRadius = 0.90f; BeautyGlow = 0.30f;
        EnBacklight = false; BacklightAmount = 0f;

        EnTealOrange = false; TealOrange = 0f;
        EnColorBalance = false; ColorBalance = 0f;
        EnSplitTone = true; StAmount = 0.14f; StBalance = 0.5f;
        StShadowR = 0.42f; StShadowG = 0.46f; StShadowB = 0.56f;
        StHighR = 0.54f; StHighG = 0.52f; StHighB = 0.48f;
        EnBleach = false; EnGradMap = false;

        Exposure = 0f; Contrast = 0.13f; Saturation = 0.05f; Vibrance = 0.26f;
        Temperature = 0.01f; Tint = 0f;
        Lift = 0.020f; Gamma = 0.010f; Gain = -0.02f;
        BlackPoint = 0.010f; WhitePoint = 1.06f;

        AnimSpeed = 0f;
    }

    public void ApplyTempestPreset()
    {
        BgStyle = 27; BgFbm = 4f; BgScale = 5f; BgScaleY = 5f; BgSharp = 0f;
        UnivBase = 0; UnivNoise = 5; UnivBlend = 5; UnivNoiseAmt = 0.35f; UnivNoiseScale = 0.30f;
        UnivWarp = 0.55f; UnivDetail = 0.5f; UnivParticle = 0; UnivOrb = 0; UnivGround = 0; UnivHorizon = 0f;
        UnivPattern = 27; UnivPatBlend = 1; UnivPatStrength = 0.70f;
        BgFlow = 0.55f; BgAngle = 0.30f;
        BgTopR = 0.06f; BgTopG = 0.16f; BgTopB = 0.10f;
        BgCol5R = 0.16f; BgCol5G = 0.46f; BgCol5B = 0.26f;
        BgMidR = 0.34f; BgMidG = 0.78f; BgMidB = 0.44f;
        BgCol6R = 0.62f; BgCol6G = 0.95f; BgCol6B = 0.70f;
        BgBotR = 0.88f; BgBotG = 1.00f; BgBotB = 0.92f;
        BgCol4R = 0.55f; BgCol4G = 0.95f; BgCol4B = 0.65f;
        BgHaze = 0.10f; BgGlow = 0.12f; BgNebContrast = 0f;
        CopyFgFromScratch(this, 0);

        UnivPattern = 0; UnivPatStrength = 0f;
        UnivNoise = 4; UnivNoiseAmt = 0.55f; UnivNoiseScale = 0.60f;
        UnivBase = 0; UnivWarp = 0.35f; BgFlow = 0.85f; BgAngle = 0.34f;
        BgTopR = 0.44f; BgTopG = 0.48f; BgTopB = 0.56f;
        BgCol5R = 0.30f; BgCol5G = 0.33f; BgCol5B = 0.40f;
        BgMidR = 0.18f; BgMidG = 0.20f; BgMidB = 0.26f;
        BgCol6R = 0.09f; BgCol6G = 0.10f; BgCol6B = 0.14f;
        BgBotR = 0.04f; BgBotG = 0.05f; BgBotB = 0.07f;
        CopyFgFromScratch(this, 1);
        SetFgBActive(true);

        EnForegroundOn = true; FgPlaceMode = 0; FgPlaceSize = 0.34f; FgPlaceSoft = 0.40f;
        FgOpacity = 0.30f; FgBlendMode = 2; FgDepthGate = 2;
        FgSeamMix = 5; FgSeamMixLevel = 0.38f;
        FgSeamMode = 3; FgSeamFeather = 0.26f; FgSeamMatch = 0.25f;

        EnBackdrop = true; BgRecolor = 1f; BgRecolorStart = 0.06f; BgRecolorFeather = 0.02f;
        UnivBase = 1; UnivNoise = 5; UnivBlend = 5; UnivNoiseAmt = 0.55f; UnivNoiseScale = 0.20f;
        UnivWarp = 0.85f; UnivDetail = 0.55f;
        UnivPattern = 0; UnivPatStrength = 0f;
        UnivParticle = 0; UnivHorizon = 0f; UnivGround = 0;
        BgScale = 2f; BgScaleY = 2f; BgFbm = 6f; BgFlow = 0.40f; BgTwist = 0.18f; BgNebContrast = 0.30f;
        BgTopR = 0.26f; BgTopG = 0.29f; BgTopB = 0.36f;
        BgCol5R = 0.15f; BgCol5G = 0.17f; BgCol5B = 0.23f;
        BgMidR = 0.085f; BgMidG = 0.095f; BgMidB = 0.135f;
        BgCol6R = 0.045f; BgCol6G = 0.050f; BgCol6B = 0.078f;
        BgBotR = 0.020f; BgBotG = 0.023f; BgBotB = 0.036f;
        BgCol4R = 0.72f; BgCol4G = 0.80f; BgCol4B = 0.95f;
        BgHaze = 0.16f; BgGlow = 0.12f; BgHueVar = 0.08f;
        BgVignette = 0.32f; BgVignetteSize = 0.84f; BgBright = -0.02f;
        BgKeepVfx = 0.85f;
        BgStars = 0f; BgEmbers = 0f;

        BgBStyle = 27; BgBUnivBase = 0; BgBUnivNoise = 0; BgBUnivBlend = 5;
        BgBUnivNoiseAmt = 0f; BgBUnivNoiseScale = 0.3f;
        BgBUnivPattern = 28; BgBUnivPatBlend = 1; BgBUnivPatStrength = 1.0f;
        BgBScale = 3f; BgBScaleY = 12f; BgBFbm = 4f; BgBOffX = 0f; BgBOffY = 0f;
        BgBTopR = 0.05f; BgBTopG = 0.02f; BgBTopB = 0.10f;
        BgBCol5R = 0.26f; BgBCol5G = 0.10f; BgBCol5B = 0.52f;
        BgBMidR = 0.52f; BgBMidG = 0.28f; BgBMidB = 0.90f;
        BgBCol6R = 0.78f; BgBCol6G = 0.62f; BgBCol6B = 1.00f;
        BgBBotR = 1.00f; BgBBotG = 0.96f; BgBBotB = 1.00f;
        BgBHaze = 0f; BgBGlow = 0.30f;
        BlendMix = 3; BlendMixLevel = 0.5f;
        BlendMode = 3; BlendFeather = 0.14f; BlendMatch = 0f;
        BlendNoiseAmt = 0f;

        PatColOverride = true; PatColMode = 5; PatMat = 0;

        EnEdge = true; EdgeErode = 0.5f; EdgeDespill = 0.6f; EdgeWrap = 0.34f; EdgeWrapWidth = 0.30f;
        EnBgBlur = true; BgBlur = 0.32f; BgBlurStart = 0.13f; SoftBlurRadius = 3.2f;
        EnShadow = true; ShadowAmount = 0.46f; ShadowSpread = 0.34f; ShadowSoftness = 0.66f;
        ShadowContact = 0.34f; ShadowOffsetX = 0.10f; ShadowOffsetY = -0.10f;
        ShadowR = 0.10f; ShadowG = 0.11f; ShadowB = 0.16f; ShadowDepth = 0.10f;
        EnRim = true; RimStrength = 0.16f; SubjectPop = 0.10f; RimSplit = 0f;
        RimR = 0.78f; RimG = 0.86f; RimB = 1.00f;
        EnBacklight = false; BacklightAmount = 0f;
        EnSkin = true; SkinWarmth = 0.24f; SkinFlush = 0.10f;
        EnBeauty = true; BeautyAmount = 0.16f; BeautyRadius = 0.9f; BeautyGlow = 0.26f;
        EnGlow = true; BloomAmount = 0.55f; BloomThreshold = 0.60f; BloomRadius = 5f;
        Halation = 0.14f; HalationR = 0.74f; HalationG = 0.82f; HalationB = 1.0f; Orton = 0.08f;
        EnFog = true; FogStrength = 0.14f; FogStart = 0.34f;
        FogColorR = 0.30f; FogColorG = 0.34f; FogColorB = 0.44f;
        EnSplitTone = true; StAmount = 0.24f; StBalance = 0.5f;
        StShadowR = 0.36f; StShadowG = 0.42f; StShadowB = 0.56f;
        StHighR = 0.54f; StHighG = 0.54f; StHighB = 0.52f;
        Exposure = -0.02f; Contrast = 0.16f; Saturation = -0.06f; Vibrance = 0.16f;
        Temperature = -0.04f; Tint = 0f;
        Lift = 0.022f; Gamma = 0f; Gain = -0.02f;
        BlackPoint = 0.010f; WhitePoint = 1.04f;
        EnLens = true; FilmRolloff = 0.60f; FilmSat = 0.45f; FilmToe = 0.22f;
        LensVig = 0.30f; LensCornerSoft = 0.18f;
        Vignette = 0.16f; Grain = 0.12f; Chroma = 0.06f; ChromaRadial = 0.85f;
        AnimSpeed = 0f;
    }

    public void ApplyHoarfrostPreset()
    {
        BgStyle = 27; BgFbm = 4f; BgScale = 5f; BgScaleY = 5f; BgSharp = 0f;
        UnivBase = 1; UnivNoise = 1; UnivBlend = 5; UnivNoiseAmt = 0.30f; UnivNoiseScale = 0.5f;
        UnivWarp = 0.35f; UnivDetail = 0.5f; UnivParticle = 0; UnivOrb = 0; UnivGround = 0; UnivHorizon = 0f;
        UnivPattern = 25; UnivPatBlend = 1; UnivPatStrength = 0.85f;
        BgTopR = 0.88f; BgTopG = 0.95f; BgTopB = 1.00f;
        BgCol5R = 0.62f; BgCol5G = 0.78f; BgCol5B = 0.92f;
        BgMidR = 0.34f; BgMidG = 0.50f; BgMidB = 0.68f;
        BgCol6R = 0.16f; BgCol6G = 0.26f; BgCol6B = 0.42f;
        BgBotR = 0.06f; BgBotG = 0.11f; BgBotB = 0.20f;
        BgCol4R = 0.80f; BgCol4G = 0.92f; BgCol4B = 1.00f;
        BgHaze = 0.10f; BgGlow = 0.18f; BgHueVar = 0.06f; BgNebContrast = 0f;
        CopyFgFromScratch(this, 0);

        UnivPattern = 0; UnivPatStrength = 0f;
        UnivNoise = 6; UnivNoiseAmt = 0.55f; UnivNoiseScale = 0.22f;
        UnivBase = 1; UnivWarp = 0.5f;
        BgTopR = 0.72f; BgTopG = 0.84f; BgTopB = 0.95f;
        BgCol5R = 0.50f; BgCol5G = 0.65f; BgCol5B = 0.82f;
        BgMidR = 0.30f; BgMidG = 0.42f; BgMidB = 0.60f;
        BgCol6R = 0.16f; BgCol6G = 0.24f; BgCol6B = 0.38f;
        BgBotR = 0.08f; BgBotG = 0.12f; BgBotB = 0.22f;
        CopyFgFromScratch(this, 1);
        SetFgBActive(true);

        EnForegroundOn = true; FgPlaceMode = 0; FgPlaceSize = 0.40f; FgPlaceSoft = 0.34f;
        FgOpacity = 0.62f; FgBlendMode = 2; FgDepthGate = 0;
        FgSeamMix = 1; FgSeamMixLevel = 0.42f;
        FgSeamMode = 3; FgSeamFeather = 0.20f; FgSeamMatch = 0.30f;

        EnBackdrop = true; BgRecolor = 1f; BgRecolorStart = 0.06f; BgRecolorFeather = 0.02f;
        UnivBase = 0; UnivNoise = 1; UnivBlend = 5; UnivNoiseAmt = 0.34f; UnivNoiseScale = 0.30f;
        UnivWarp = 0.40f; UnivDetail = 0.45f;
        UnivPattern = 0; UnivPatStrength = 0f;
        UnivParticle = 2; UnivHorizon = 0.62f; UnivGround = 1;
        BgScale = 3f; BgScaleY = 3f; BgFbm = 5f;
        BgTopR = 0.80f; BgTopG = 0.88f; BgTopB = 0.97f;
        BgCol5R = 0.62f; BgCol5G = 0.74f; BgCol5B = 0.90f;
        BgMidR = 0.44f; BgMidG = 0.58f; BgMidB = 0.78f;
        BgCol6R = 0.28f; BgCol6G = 0.40f; BgCol6B = 0.60f;
        BgBotR = 0.66f; BgBotG = 0.74f; BgBotB = 0.84f;
        BgCol4R = 0.86f; BgCol4G = 0.94f; BgCol4B = 1.00f;
        BgHaze = 0.28f; BgGlow = 0.14f; BgHueVar = 0.10f;
        BgVignette = 0.22f; BgVignetteSize = 0.9f; BgBright = 0.04f;
        BgKeepVfx = 0.85f;
        BgStars = 0f; BgEmbers = 0f;

        BgBStyle = 27; BgBUnivBase = 1; BgBUnivNoise = 1; BgBUnivBlend = 5;
        BgBUnivNoiseAmt = 0.25f; BgBUnivNoiseScale = 0.45f;
        BgBUnivPattern = 25; BgBUnivPatBlend = 1; BgBUnivPatStrength = 0.80f;
        BgBScale = 6f; BgBFbm = 4f; BgBOffX = 0f; BgBOffY = 0.10f;
        BgBTopR = 0.92f; BgBTopG = 0.97f; BgBTopB = 1.00f;
        BgBCol5R = 0.66f; BgBCol5G = 0.80f; BgBCol5B = 0.94f;
        BgBMidR = 0.34f; BgBMidG = 0.48f; BgBMidB = 0.68f;
        BgBCol6R = 0.14f; BgBCol6G = 0.22f; BgBCol6B = 0.38f;
        BgBBotR = 0.05f; BgBBotG = 0.09f; BgBBotB = 0.18f;
        BgBHaze = 0.10f; BgBGlow = 0.16f;
        BlendMix = 1; BlendMixLevel = 0.40f;
        BlendMode = 3; BlendFeather = 0.18f; BlendMatch = 0.35f;
        BlendNoiseAmt = 0.10f; BlendNoiseScale = 2.0f;

        PatColOverride = true; PatColMode = 4;
        PatColR = 0.10f; PatColG = 0.18f; PatColB = 0.30f;
        PatCol2R = 0.30f; PatCol2G = 0.48f; PatCol2B = 0.68f;
        PatCol3R = 0.56f; PatCol3G = 0.76f; PatCol3B = 0.92f;
        PatCol4R = 0.80f; PatCol4G = 0.92f; PatCol4B = 1.00f;
        PatCol5R = 1.00f; PatCol5G = 1.00f; PatCol5B = 1.00f;
        PatMat = 0;

        EnEdge = true; EdgeErode = 0.5f; EdgeDespill = 0.55f; EdgeWrap = 0.30f; EdgeWrapWidth = 0.30f;
        EnBgBlur = true; BgBlur = 0.34f; BgBlurStart = 0.13f; SoftBlurRadius = 3.2f;
        EnShadow = true; ShadowAmount = 0.40f; ShadowSpread = 0.34f; ShadowSoftness = 0.68f;
        ShadowContact = 0.32f; ShadowOffsetX = 0.10f; ShadowOffsetY = -0.09f;
        ShadowR = 0.42f; ShadowG = 0.50f; ShadowB = 0.62f; ShadowDepth = 0.10f;
        EnRim = true; RimStrength = 0.10f; SubjectPop = 0.08f; RimSplit = 0f;
        RimR = 0.82f; RimG = 0.92f; RimB = 1.00f;
        EnBacklight = false; BacklightAmount = 0f;
        EnSkin = true; SkinWarmth = 0.26f; SkinFlush = 0.10f;
        EnBeauty = true; BeautyAmount = 0.18f; BeautyRadius = 0.9f; BeautyGlow = 0.28f;
        EnGlow = true; BloomAmount = 0.22f; BloomThreshold = 0.78f;
        Halation = 0.08f; HalationR = 0.86f; HalationG = 0.94f; HalationB = 1.0f; Orton = 0.08f;
        EnFog = true; FogStrength = 0.16f; FogStart = 0.34f;
        FogColorR = 0.72f; FogColorG = 0.82f; FogColorB = 0.92f;
        EnSplitTone = true; StAmount = 0.22f; StBalance = 0.5f;
        StShadowR = 0.40f; StShadowG = 0.48f; StShadowB = 0.60f;
        StHighR = 0.56f; StHighG = 0.55f; StHighB = 0.52f;
        Exposure = 0.05f; Contrast = 0.07f; Saturation = -0.04f; Vibrance = 0.14f;
        Temperature = -0.05f; Tint = 0.01f;
        Lift = 0.030f; Gamma = 0.020f; Gain = -0.015f;
        BlackPoint = 0.006f; WhitePoint = 1.06f;
        EnLens = true; FilmRolloff = 0.55f; FilmSat = 0.40f; FilmToe = 0.20f;
        LensVig = 0.22f; LensCornerSoft = 0.14f;
        Vignette = 0.10f; Grain = 0.08f; Chroma = 0.05f; ChromaRadial = 0.85f;
        AnimSpeed = 0f;
    }

    public void ApplyEmberfallPreset()
    {
        BgStyle = 27; BgFbm = 5f; BgScale = 4f; BgScaleY = 4f; BgSharp = 0f;
        UnivBase = 0; UnivNoise = 4; UnivBlend = 5; UnivNoiseAmt = 0.40f; UnivNoiseScale = 0.35f;
        UnivWarp = 0.45f; UnivDetail = 0.55f; UnivParticle = 0; UnivOrb = 0; UnivGround = 0; UnivHorizon = 0f;
        UnivPattern = 26; UnivPatBlend = 1; UnivPatStrength = 0.95f;
        BgTopR = 0.20f; BgTopG = 0.05f; BgTopB = 0.01f;
        BgCol5R = 0.45f; BgCol5G = 0.10f; BgCol5B = 0.02f;
        BgMidR = 0.16f; BgMidG = 0.04f; BgMidB = 0.02f;
        BgCol6R = 0.07f; BgCol6G = 0.02f; BgCol6B = 0.01f;
        BgBotR = 0.02f; BgBotG = 0.01f; BgBotB = 0.01f;
        BgCol4R = 1.00f; BgCol4G = 0.62f; BgCol4B = 0.20f;
        BgHaze = 0.08f; BgGlow = 0.30f; BgFlow = 0.35f; BgNebContrast = 0.20f;
        CopyFgFromScratch(this, 0);

        UnivPattern = 0; UnivPatStrength = 0f;
        UnivNoise = 5; UnivNoiseAmt = 0.60f; UnivNoiseScale = 0.18f;
        UnivBase = 0; UnivWarp = 0.75f; UnivDetail = 0.4f; BgFlow = 0.45f;
        BgTopR = 0.30f; BgTopG = 0.26f; BgTopB = 0.24f;
        BgCol5R = 0.20f; BgCol5G = 0.17f; BgCol5B = 0.16f;
        BgMidR = 0.12f; BgMidG = 0.10f; BgMidB = 0.09f;
        BgCol6R = 0.06f; BgCol6G = 0.05f; BgCol6B = 0.05f;
        BgBotR = 0.02f; BgBotG = 0.02f; BgBotB = 0.02f;
        CopyFgFromScratch(this, 1);
        SetFgBActive(true);

        EnForegroundOn = true; FgPlaceMode = 3; FgPlaceSize = 0.46f; FgPlaceSoft = 0.40f;
        FgOpacity = 0.80f; FgBlendMode = 1; FgDepthGate = 0;
        FgSeamMix = 5; FgSeamMixLevel = 0.55f;
        FgSeamMode = 3; FgSeamFeather = 0.24f; FgSeamMatch = 0.20f;

        EnBackdrop = true; BgRecolor = 1f; BgRecolorStart = 0.06f; BgRecolorFeather = 0.02f;
        UnivBase = 1; UnivNoise = 4; UnivBlend = 5; UnivNoiseAmt = 0.40f; UnivNoiseScale = 0.28f;
        UnivWarp = 0.55f; UnivDetail = 0.5f;
        UnivPattern = 0; UnivPatStrength = 0f;
        UnivParticle = 3; UnivHorizon = 0f; UnivGround = 0;
        BgScale = 3f; BgScaleY = 3f; BgFbm = 5f; BgFlow = 0.30f; BgNebContrast = 0.25f;
        BgTopR = 0.42f; BgTopG = 0.13f; BgTopB = 0.03f;
        BgCol5R = 0.26f; BgCol5G = 0.07f; BgCol5B = 0.02f;
        BgMidR = 0.14f; BgMidG = 0.04f; BgMidB = 0.02f;
        BgCol6R = 0.07f; BgCol6G = 0.02f; BgCol6B = 0.015f;
        BgBotR = 0.025f; BgBotG = 0.012f; BgBotB = 0.010f;
        BgCol4R = 1.00f; BgCol4G = 0.58f; BgCol4B = 0.18f;
        BgHaze = 0.20f; BgGlow = 0.26f; BgHueVar = 0.12f;
        BgVignette = 0.34f; BgVignetteSize = 0.82f; BgBright = 0f;
        BgKeepVfx = 0.85f;
        BgEmbers = 0.30f; BgEmberSize = 0.45f; BgStars = 0f;

        BgBStyle = 27; BgBUnivBase = 0; BgBUnivNoise = 4; BgBUnivBlend = 5;
        BgBUnivNoiseAmt = 0.42f; BgBUnivNoiseScale = 0.30f;
        BgBUnivPattern = 26; BgBUnivPatBlend = 1; BgBUnivPatStrength = 0.90f;
        BgBScale = 4f; BgBFbm = 5f; BgBOffX = 0f; BgBOffY = 0f; BgBNebContrast = 0.22f;
        BgBTopR = 0.55f; BgBTopG = 0.16f; BgBTopB = 0.03f;
        BgBCol5R = 0.34f; BgBCol5G = 0.09f; BgBCol5B = 0.02f;
        BgBMidR = 0.16f; BgBMidG = 0.04f; BgBMidB = 0.015f;
        BgBCol6R = 0.06f; BgBCol6G = 0.02f; BgBCol6B = 0.01f;
        BgBBotR = 0.015f; BgBBotG = 0.008f; BgBBotB = 0.006f;
        BgBHaze = 0.10f; BgBGlow = 0.28f;
        BlendMix = 1; BlendMixLevel = 0.34f;
        BlendMode = 3; BlendFeather = 0.20f; BlendMatch = 0.25f;
        BlendNoiseAmt = 0.12f; BlendNoiseScale = 2.4f;

        PatColOverride = true; PatColMode = 4;
        PatColR = 0.14f; PatColG = 0.02f; PatColB = 0.02f;
        PatCol2R = 0.74f; PatCol2G = 0.11f; PatCol2B = 0.02f;
        PatCol3R = 0.98f; PatCol3G = 0.42f; PatCol3B = 0.05f;
        PatCol4R = 1.00f; PatCol4G = 0.80f; PatCol4B = 0.24f;
        PatCol5R = 1.00f; PatCol5G = 0.97f; PatCol5B = 0.88f;
        PatMat = 0;

        EnEdge = true; EdgeErode = 0.5f; EdgeDespill = 0.6f; EdgeWrap = 0.38f; EdgeWrapWidth = 0.32f;
        EnBgBlur = true; BgBlur = 0.26f; BgBlurStart = 0.12f; SoftBlurRadius = 2.8f;
        EnShadow = true; ShadowAmount = 0.50f; ShadowSpread = 0.34f; ShadowSoftness = 0.62f;
        ShadowContact = 0.40f; ShadowOffsetX = 0.14f; ShadowOffsetY = -0.10f;
        ShadowR = 0.10f; ShadowG = 0.05f; ShadowB = 0.04f; ShadowDepth = 0.10f;
        EnRim = true; RimStrength = 0.14f; SubjectPop = 0.10f; RimSplit = 0f;
        RimR = 1.00f; RimG = 0.66f; RimB = 0.30f;
        EnBacklight = true; BacklightAmount = 0.30f; BacklightWidth = 0.30f;
        BacklightR = 1.00f; BacklightG = 0.58f; BacklightB = 0.22f;
        Backlight2R = 1.00f; Backlight2G = 0.58f; Backlight2B = 0.22f;
        EnSkin = true; SkinWarmth = 0.30f; SkinFlush = 0.14f;
        EnBeauty = true; BeautyAmount = 0.20f; BeautyRadius = 0.95f; BeautyGlow = 0.34f;
        EnGlow = true; BloomAmount = 0.34f; BloomThreshold = 0.70f; BloomRadius = 4f;
        Halation = 0.26f; HalationR = 1.0f; HalationG = 0.60f; HalationB = 0.28f; Orton = 0.10f;
        EnFog = false; FogStrength = 0f;
        EnSplitTone = true; StAmount = 0.26f; StBalance = 0.45f;
        StShadowR = 0.34f; StShadowG = 0.40f; StShadowB = 0.52f;
        StHighR = 0.62f; StHighG = 0.50f; StHighB = 0.36f;
        Exposure = -0.02f; Contrast = 0.12f; Saturation = 0f; Vibrance = 0.20f;
        Temperature = 0.08f; Tint = 0.02f;
        Lift = 0.018f; Gamma = 0f; Gain = -0.03f;
        BlackPoint = 0.010f; WhitePoint = 1.05f;
        EnLens = true; FilmRolloff = 0.62f; FilmSat = 0.48f; FilmToe = 0.22f;
        LensVig = 0.30f; LensCornerSoft = 0.18f;
        Vignette = 0.16f; Grain = 0.10f; Chroma = 0.07f; ChromaRadial = 0.85f;
        AnimSpeed = 0f;
    }

    public void ApplyStudioPortraitPreset()
    {
        EnBackdrop = true; BgStyle = 26; BgRecolor = 1f;
        BgRecolorStart = 0.06f; BgRecolorFeather = 0.02f;
        BgGradType = 1; BgPatMode = 0;
        BgOffX = -0.06f; BgOffY = -0.06f; BgAngle = 0f; BgSharp = 0f;
        BgTopR = 0.475f; BgTopG = 0.485f; BgTopB = 0.505f;
        BgCol5R = 0.345f; BgCol5G = 0.355f; BgCol5B = 0.375f;
        BgMidR = 0.235f;  BgMidG = 0.245f;  BgMidB = 0.265f;
        BgCol6R = 0.145f; BgCol6G = 0.152f; BgCol6B = 0.170f;
        BgBotR = 0.082f;  BgBotG = 0.088f;  BgBotB = 0.102f;
        BgScale = 3f; BgFbm = 3f; BgNebContrast = 0.04f;
        BgCol4R = 0.34f; BgCol4G = 0.35f; BgCol4B = 0.37f;
        BgStars = 0f; BgEmbers = 0f; BgGrain = 0f;
        BgVignette = 0.14f; BgVignetteSize = 0.95f; BgBright = 0f;
        BackdropLightAmt = 0.42f; BackdropLightX = 0.38f;
        BackdropLightY = 0.34f; BackdropLightSize = 0.78f;

        EnEdge = true; EdgeErode = 0.5f; EdgeDespill = 0.6f; EdgeWrap = 0.22f; EdgeWrapWidth = 0.30f;

        EnBgBlur = true; BgBlur = 0.58f; BgBlurStart = 0.11f; SoftBlurRadius = 3.6f;
        EnShadow = true; ShadowAmount = 0.44f; ShadowSpread = 0.38f;
        ShadowOffsetX = 0.16f; ShadowOffsetY = -0.09f;
        ShadowSoftness = 0.72f; ShadowContact = 0.34f;
        ShadowR = 0.30f; ShadowG = 0.305f; ShadowB = 0.325f;
        ShadowDepth = 0.10f;

        EnBacklight = false; BacklightAmount = 0f;
        EnRim = true; RimStrength = 0f; SubjectPop = 0.07f; RimSplit = 0f;

        EnBeauty = true; BeautyAmount = 0.24f; BeautyRadius = 0.95f; BeautyGlow = 0.28f;
        EnSkin = true; SkinWarmth = 0.22f; SkinFlush = 0.09f;
        EnWet = false; EnGobo = false; EnSpot = false; EnParticles = false;

        EnGlow = true; BloomAmount = 0.14f; BloomThreshold = 0.82f; BloomRadius = 3f;
        Halation = 0.07f; HalationR = 1.0f; HalationG = 0.88f; HalationB = 0.76f;
        Orton = 0.05f; GodrayAmount = 0f; AnamAmount = 0f;

        EnSplitTone = true; StAmount = 0.14f; StBalance = 0.5f;
        StShadowR = 0.46f; StShadowG = 0.49f; StShadowB = 0.55f;
        StHighR = 0.54f;  StHighG = 0.51f;  StHighB = 0.46f;
        EnTealOrange = false; TealOrange = 0f;
        EnColorBalance = false; ColorBalance = 0f;

        Exposure = 0.02f; Contrast = 0.07f; Saturation = 0f; Vibrance = 0.14f;
        Temperature = 0.03f; Tint = 0.01f;
        Lift = 0.022f; Gamma = 0.015f; Gain = -0.015f;
        BlackPoint = 0.006f; WhitePoint = 1.06f;

        EnLens = true;
        FilmRolloff = 0.58f; FilmSat = 0.38f; FilmToe = 0.20f;
        LensVig = 0.16f; LensCornerSoft = 0.10f;
        Vignette = 0.07f; Grain = 0.07f; Chroma = 0.03f; ChromaRadial = 0.85f;
        Sharpen = 0.14f; Clarity = 0.14f;
        Letterbox = 0f; Prism = 0f; LeakAmt = 0f; WashAmount = 0f;
    }

    public void ApplyStarlitVowPreset()
    {
        EnBackdrop = true; BgStyle = 27; BgRecolor = 1f;
        BgRecolorStart = 0.06f; BgRecolorFeather = 0.02f;
        UnivBase = 1;
        UnivNoise = 1; UnivBlend = 5; UnivNoiseAmt = 0.34f; UnivNoiseScale = 0.22f;
        BgGradType = 1; BgPatMode = 0; BgOffX = 0f; BgOffY = -0.04f; BgSharp = 0f;
        BgTopR = 0.285f; BgTopG = 0.212f; BgTopB = 0.150f;
        BgCol5R = 0.150f; BgCol5G = 0.132f; BgCol5B = 0.168f;
        BgMidR = 0.082f; BgMidG = 0.096f; BgMidB = 0.172f;
        BgCol6R = 0.046f; BgCol6G = 0.059f; BgCol6B = 0.124f;
        BgBotR = 0.024f; BgBotG = 0.032f; BgBotB = 0.072f;
        BgScale = 2f; BgFbm = 5f; BgNebContrast = 0.16f; BgNebWarp = 0.45f; BgHueVar = 0.22f;
        BgStars = 0.55f; BgStarDensity = 32f; BgStarSize = 0.33f; BgSparkle = 0.26f;
        BgCol4R = 0.80f; BgCol4G = 0.86f; BgCol4B = 1.0f;
        BgHaze = 0.30f; BgGlow = 0.16f;
        BgVignette = 0.22f; BgVignetteSize = 0.88f; BgBright = 0f;
        BackdropLightAmt = 0.48f; BackdropLightX = 0.5f;
        BackdropLightY = 0.38f; BackdropLightSize = 0.66f;

        EnEdge = true; EdgeErode = 0.5f; EdgeDespill = 0.55f; EdgeWrap = 0.30f; EdgeWrapWidth = 0.34f;
        EnBgBlur = true; BgBlur = 0.45f; BgBlurStart = 0.12f; SoftBlurRadius = 3.5f;

        EnShadow = true; ShadowAmount = 0.42f; ShadowSpread = 0.34f;
        ShadowOffsetX = 0.12f; ShadowOffsetY = -0.10f;
        ShadowSoftness = 0.60f; ShadowContact = 0.45f;
        ShadowR = 0.045f; ShadowG = 0.050f; ShadowB = 0.085f; ShadowDepth = 0.10f;

        EnBacklight = true; BacklightAmount = 0.26f; BacklightWidth = 0.30f;
        BacklightR = 1.0f; BacklightG = 0.86f; BacklightB = 0.62f;
        EnSpot = true; SpotAmount = 0.20f; SpotX = 0.5f; SpotY = 0.45f;
        SpotRadius = 0.72f; SpotEllipse = 1.25f; SpotSoft = 0.62f; SpotAngle = 0f; SpotWarm = 0.30f;
        EnParticles = false; ParticleType = 0; ParticleAmount = 0f;
        ParticleSize = 0.30f; ParticleFall = 0.22f;
        ParticleR = 1.0f; ParticleG = 0.82f; ParticleB = 0.55f;
        BokehShape = 0; BokehAmount = 0f;

        EnSkin = true; SkinWarmth = 0.34f; SkinFlush = 0.12f;
        EnBeauty = true; BeautyAmount = 0.30f; BeautyRadius = 1.05f; BeautyGlow = 0.50f;
        EnRim = true; RimStrength = 0.05f; SubjectPop = 0.06f;
        EnGlow = true; BloomAmount = 0.20f; BloomThreshold = 0.76f;
        Halation = 0.14f; HalationR = 1.0f; HalationG = 0.80f; HalationB = 0.58f;
        Orton = 0.13f; GodrayAmount = 0f; AnamAmount = 0f;

        EnSplitTone = true; StAmount = 0.30f; StBalance = 0.45f;
        StShadowR = 0.30f; StShadowG = 0.38f; StShadowB = 0.58f;
        StHighR = 0.62f; StHighG = 0.52f; StHighB = 0.36f;

        EnLens = true;
        FilmRolloff = 0.55f; FilmSat = 0.40f; FilmToe = 0.26f;
        LensVig = 0.26f; LensCornerSoft = 0.22f;
        Vignette = 0.10f; Grain = 0.10f; Chroma = 0.06f; ChromaRadial = 0.85f;

        Exposure = 0.02f; Contrast = 0.07f; Saturation = -0.03f; Vibrance = 0.16f;
        Temperature = 0.05f; Tint = 0.01f;
        Lift = 0.030f; Gamma = 0f; Gain = -0.02f;
        BlackPoint = 0.008f; WhitePoint = 1.02f;
    }

    public void ApplyDualityPreset()
    {
        EnBackdrop = true; BgStyle = 27; BgRecolor = 1f;
        BgRecolorStart = 0.06f; BgRecolorFeather = 0.02f;
        BgKeepVfx = 0f;

        UnivBase = 1; UnivNoise = 1; UnivBlend = 5; UnivNoiseAmt = 0.28f; UnivNoiseScale = 0.25f;
        BgGradType = 1; BgPatMode = 0; BgOffX = -0.24f; BgOffY = -0.02f; BgSharp = 0f;
        BgScale = 2f; BgFbm = 4f; BgNebContrast = 0.10f; BgNebWarp = 0.35f;
        BgTopR = 0.36f; BgTopG = 0.56f; BgTopB = 0.24f;
        BgCol5R = 0.185f; BgCol5G = 0.335f; BgCol5B = 0.160f;
        BgMidR = 0.078f; BgMidG = 0.175f; BgMidB = 0.115f;
        BgCol6R = 0.034f; BgCol6G = 0.085f; BgCol6B = 0.070f;
        BgBotR = 0.014f; BgBotG = 0.036f; BgBotB = 0.034f;
        BgHaze = 0.20f; BgGlow = 0.16f; BgHueVar = 0.14f;
        BgCol4R = 0.55f; BgCol4G = 0.80f; BgCol4B = 0.45f;
        BgVignette = 0.26f; BgVignetteSize = 0.9f; BgBright = 0f;

        BgBStyle = 27; BgBUnivBase = 1; BgBUnivNoise = 1; BgBUnivBlend = 5;
        BgBUnivNoiseAmt = 0.26f; BgBUnivNoiseScale = 0.25f;
        BgBOffX = 0.24f; BgBOffY = -0.02f; BgBScale = 2f; BgBFbm = 4f;
        BgBNebContrast = 0.10f; BgBNebWarp = 0.35f;
        BgBTopR = 0.235f; BgBTopG = 0.212f; BgBTopB = 0.215f;
        BgBCol5R = 0.160f; BgBCol5G = 0.122f; BgBCol5B = 0.126f;
        BgBMidR = 0.105f; BgBMidG = 0.062f; BgBMidB = 0.066f;
        BgBCol6R = 0.056f; BgBCol6G = 0.033f; BgBCol6B = 0.038f;
        BgBBotR = 0.022f; BgBBotG = 0.016f; BgBBotB = 0.019f;
        BgBHaze = 0.16f; BgBGlow = 0.10f; BgBHueVar = 0.16f;

        BlendMix = 0;
        BlendMode = 0; BlendAngle = 0f; BlendOffset = 0f;
        BlendFeather = 0.07f;
        BlendNoiseAmt = 0.05f; BlendNoiseScale = 2.0f;
        BlendMatch = 0.30f;

        EnEdge = true; EdgeErode = 0.70f; EdgeDespill = 0.70f; EdgeWrap = 0.35f; EdgeWrapWidth = 0.32f;
        EnBacklight = true; BacklightAmount = 0.34f; BacklightWidth = 0.26f;
        BacklightR = 0.62f; BacklightG = 1.0f; BacklightB = 0.58f;
        Backlight2R = 0.95f; Backlight2G = 0.36f; Backlight2B = 0.34f;
        EnRim = true; RimStrength = 0.20f; SubjectPop = 0.08f; RimWidth = 2f;
        RimR = 0.60f; RimG = 1.0f; RimB = 0.56f;
        Rim2R = 0.95f; Rim2G = 0.34f; Rim2B = 0.32f;
        RimSplit = 1f; RimSplitAngle = 0f; RimSplitOffset = 0f; RimSplitSoft = 0.07f;

        EnBgBlur = true; BgBlur = 0.38f; BgBlurStart = 0.12f; SoftBlurRadius = 3.2f;
        EnShadow = true; ShadowAmount = 0.42f; ShadowSpread = 0.32f;
        ShadowOffsetX = 0f; ShadowOffsetY = -0.10f;
        ShadowSoftness = 0.62f; ShadowContact = 0.35f;
        ShadowR = 0.05f; ShadowG = 0.055f; ShadowB = 0.065f; ShadowDepth = 0.10f;

        EnSkin = true; SkinWarmth = 0.22f; SkinFlush = 0.08f;
        EnBeauty = true; BeautyAmount = 0.18f; BeautyRadius = 0.9f; BeautyGlow = 0.30f;
        EnGlow = true; BloomAmount = 0.18f; BloomThreshold = 0.76f;
        Halation = 0.10f; HalationR = 1.0f; HalationG = 0.86f; HalationB = 0.70f;
        Orton = 0.08f; GodrayAmount = 0f; AnamAmount = 0f;

        EnSplitTone = true; StAmount = 0.22f; StBalance = 0.5f;
        StShadowR = 0.36f; StShadowG = 0.44f; StShadowB = 0.52f;
        StHighR = 0.56f; StHighG = 0.53f; StHighB = 0.44f;

        EnLens = true;
        FilmRolloff = 0.55f; FilmSat = 0.40f; FilmToe = 0.24f;
        LensVig = 0.30f; LensCornerSoft = 0.22f;
        Vignette = 0.14f; Grain = 0.09f; Chroma = 0.06f; ChromaRadial = 0.85f;

        Exposure = 0f; Contrast = 0.10f; Saturation = -0.02f; Vibrance = 0.16f;
        Temperature = 0.01f; Tint = 0f;
        Lift = 0.020f; Gamma = 0f; Gain = -0.02f;
        BlackPoint = 0.010f; WhitePoint = 1.03f;
    }

    public void ApplyOnLocationPreset()
    {
        EnBackdrop = false; BgRecolor = 0f; BgStyle = 0; BgBStyle = 0;
        EnBgFill = false; BgFill = 0f;
        EnForegroundOn = false;
        EnEdge = false;
        EnShadow = false; EnGround = false; EnHalo = false;
        EnGobo = false; EnSpot = false;
        EnParticles = false; EnFrost = false; EnVhs = false;
        EnUnderwater = false; EnHud = false; EnWet = false;
        EnStylize = false; EnWarp = false;

        EnFog = false; FogStrength = 0f;
        EnSubjectIso = false; BgPushStrength = 0f;
        EnTealOrange = false; TealOrange = 0f;
        EnColorBalance = false; ColorBalance = 0f;
        EnSplitTone = false; StAmount = 0f;
        EnBleach = false; EnGradMap = false;

        EnBgBlur = true; BgBlur = 0.10f; BgBlurStart = 0.30f; SoftBlurRadius = 2.5f;
        EnDof = false;

        Exposure = 0.06f;
        Contrast = 0.05f;
        Saturation = 0f; Vibrance = 0.14f;
        Temperature = 0.02f; Tint = 0f;
        Lift = 0.030f; Gamma = 0.030f; Gain = -0.010f;
        BlackPoint = 0.004f; WhitePoint = 1.06f;

        EnLens = true;
        FilmRolloff = 0.50f; FilmSat = 0.30f; FilmToe = 0.14f;
        LensVig = 0.18f; LensCornerSoft = 0.12f;
        Vignette = 0.06f; Grain = 0.06f; Chroma = 0.03f; ChromaRadial = 0.85f;
        Sharpen = 0.18f; Clarity = 0.12f;
        Letterbox = 0f; Prism = 0f; LeakAmt = 0f; WashAmount = 0f;

        EnGlow = true; BloomAmount = 0.10f; BloomThreshold = 0.82f; BloomRadius = 3f;
        Halation = 0.06f; HalationR = 1.0f; HalationG = 0.86f; HalationB = 0.70f;
        Orton = 0.03f; GodrayAmount = 0f; AnamAmount = 0f;

        EnRim = true; RimStrength = 0.04f; RimWidth = 2f; SubjectPop = 0.06f;
        RimR = 1.0f; RimG = 0.95f; RimB = 0.88f; RimSplit = 0f;
        EnSkin = true; SkinWarmth = 0.12f; SkinFlush = 0.04f;
        EnBeauty = true; BeautyAmount = 0.08f; BeautyRadius = 0.8f; BeautyGlow = 0.18f;
        EnBacklight = false; BacklightAmount = 0f;
    }

    public void ApplyChineseInkPreset()
    {
        EnBackdrop = true; BgStyle = 29; BgRecolor = 1f; BgRecolorStart = 0.05f;
        BgTopR = 0.92f; BgTopG = 0.89f; BgTopB = 0.82f;
        BgBotR = 0.06f; BgBotG = 0.06f; BgBotB = 0.07f;
        BgCol4R = 0.10f; BgCol4G = 0.12f; BgCol4B = 0.20f;
        BgMidR = 0.45f; BgMidG = 0.45f; BgMidB = 0.46f;
        BgCol5R = 0.70f; BgCol5G = 0.68f; BgCol5B = 0.63f;
        BgCol6R = 0.28f; BgCol6G = 0.28f; BgCol6B = 0.30f;
        BgScale = 6f; BgScaleY = 7f; BgOffX = 0f; BgOffY = 0f; BgFbm = 5f;
        BgNebContrast = 0.45f;
        BgNebWarp = 0.7f;
        BgHaze = 0.35f;
        BgGlow = 0.3f;
        BgSparkle = 0.35f;
        BgFlow = 0f; BgTwist = 0f; BgStars = 0f; BgEmbers = 0f;
        BgVignette = 0.25f; BgVignetteSize = 0.85f; BgBright = 0.02f;
        BgVoidCore = 0f; BgVoidRing = 0f; BgRing2 = 0f;
        BgCausticAmt = 0f; BgShafts = 0f; BgBubbles = 0f;
        BgNormal = 0f; BgSpecular = 0f; BgMetallic = 0f; BgReflect = 0f;
        BgFresnel = 0f; BgClearcoat = 0f; BgLightInt = 0f;
        BgGrain = 0.12f;
        AnimSpeed = 0f;

        Exposure = 0.02f; Contrast = 0.14f; Saturation = -0.55f; Vibrance = 0f;
        Temperature = 0.04f; Tint = 0f;
        Lift = 0.01f; Gamma = 0f; Gain = 0f;
        BlackPoint = 0.02f; WhitePoint = 1.0f; Clarity = 0.3f;
        EnGlow = true; BloomAmount = 0.12f; BloomThreshold = 0.8f; Orton = 0.08f;
        EnRim = true; RimStrength = 0.5f; RimThreshold = 0.015f; RimWidth = 2f;
        RimR = 0.2f; RimG = 0.2f; RimB = 0.22f;
        SubjectPop = 0.2f;
        EnLens = true; Vignette = 0.3f; Grain = 0.1f; Sharpen = 0.4f; Chroma = 0f;
    }

    public void ApplyEvercoldPreset()
    {
        EnBackdrop = true; BgStyle = 28; BgRecolor = 1f; BgRecolorStart = 0.05f;
        BgTopR = 0.04f; BgTopG = 0.10f; BgTopB = 0.24f;
        BgCol5R = 0.07f; BgCol5G = 0.22f; BgCol5B = 0.44f;
        BgMidR = 0.14f; BgMidG = 0.42f; BgMidB = 0.63f;
        BgCol6R = 0.28f; BgCol6G = 0.58f; BgCol6B = 0.77f;
        BgBotR = 0.52f; BgBotG = 0.78f; BgBotB = 0.92f;
        BgCol4R = 0.78f; BgCol4G = 0.93f; BgCol4B = 1.0f;
        BgScale = 6f; BgScaleY = 7f; BgOffX = 0.1f; BgOffY = 0f; BgFbm = 5f;
        BgNebContrast = 0.35f; BgNebWarp = 0.55f; BgHaze = 0.35f; BgGlow = 0.55f;
        BgFlow = 0f; BgTwist = 0.05f; BgSparkle = 0.85f;
        BgVignette = 0.4f; BgVignetteSize = 0.72f; BgBright = 0f;
        AnimSpeed = 0.4f;

        Exposure = 0f; Contrast = 0.16f; Saturation = -0.10f; Vibrance = 0.08f;
        Temperature = -0.16f; Tint = -0.02f;
        Lift = 0f; Gamma = 0f; Gain = -0.02f;
        BlackPoint = 0.03f; WhitePoint = 1.02f;
        Clarity = 0.28f;
        EnColorBalance = true; ColorBalance = 0.55f;
        CbShadowR = 0.45f; CbShadowG = 0.49f; CbShadowB = 0.57f;
        CbMidR = 0.49f;  CbMidG = 0.50f;  CbMidB = 0.53f;
        CbHighR = 0.51f; CbHighG = 0.51f; CbHighB = 0.52f;

        EnSubjectIso = true; BgPushStart = 0.10f; BgPushStrength = 0.45f;
        EnDof = true; DofFocus = 0.03f; DofRange = 0.10f; DofStrength = 0.5f;

        EnGlow = true; BloomAmount = 0.35f; BloomThreshold = 0.70f; BloomRadius = 3f;
        Orton = 0.1f; Glamour = 0f; Halation = 0f; GodrayAmount = 0f; AnamAmount = 0f;
        EnRim = true; RimStrength = 0.9f; RimThreshold = 0.015f; RimWidth = 2f;
        RimR = 0.45f; RimG = 0.72f; RimB = 1.0f;
        SubjectPop = 0.32f;
        EnFog = true; FogStrength = 0.5f; FogStart = 0.25f;
        FogColorR = 0.74f; FogColorG = 0.84f; FogColorB = 0.95f;
        EnHalo = true; HaloAmount = 0.5f; HaloSplit = 0.12f; HaloR = 0.72f; HaloG = 0.86f; HaloB = 1.0f;
        EnFrost = true; FrostAmount = 0.6f; FrostCoverage = 0.5f; FrostFeather = 0.45f;
        EnLens = true; Vignette = 0.42f; Grain = 0.05f; Sharpen = 0.35f; Chroma = 0f;
    }

    public void ResetLook()
    {
        var def = new PluginConfig();
        foreach (var prop in typeof(PluginConfig).GetProperties())
        {
            if (!prop.CanRead || !prop.CanWrite) continue;
            if (LookStore.Exclude.Contains(prop.Name)) continue;
            if (prop.Name is "Elem" or "ElemImages" or "Texts") continue;
            prop.SetValue(this, prop.GetValue(def));
        }
    }

    public void Save() => Services.PluginInterface.SavePluginConfig(this);

    public static PluginConfig Load()
    {
        var cfg = Services.PluginInterface.GetPluginConfig() as PluginConfig ?? new PluginConfig();
        cfg.MigrateElem();
        cfg.MigrateFg();
        return cfg;
    }

    private void MigrateFg()
    {
        int want = 2 * FgFieldCount + 2;
        if (FgField != null && FgField.Length == want) return;
        var dst = new float[want];
        if (FgField != null && FgField.Length == 180)
            for (int f = 0; f < 2; f++)
                for (int k = 0; k < 89; k++)
                    dst[f * FgFieldCount + k] = FgField[f * 89 + k];
        FgField = dst;
        CarryPatternIdentity();
    }

    internal void CarryPatternIdentity()
    {
        if (!BgBPatColOverride)
        {
            BgBPatColOverride = PatColOverride; BgBPatColMode = PatColMode;
            BgBPatColR = PatColR; BgBPatColG = PatColG; BgBPatColB = PatColB;
            BgBPatCol2R = PatCol2R; BgBPatCol2G = PatCol2G; BgBPatCol2B = PatCol2B;
            BgBPatCol3R = PatCol3R; BgBPatCol3G = PatCol3G; BgBPatCol3B = PatCol3B;
            BgBPatCol4R = PatCol4R; BgBPatCol4G = PatCol4G; BgBPatCol4B = PatCol4B;
            BgBPatCol5R = PatCol5R; BgBPatCol5G = PatCol5G; BgBPatCol5B = PatCol5B;
            BgBPatMat = PatMat; BgBPatMatR = PatMatR; BgBPatMatG = PatMatG;
            BgBPatMatB = PatMatB; BgBPatMatTint = PatMatTint;
        }
        for (int f = 0; f < 2; f++)
        {
            int o = f * FgFieldCount;
            if (FgField[o + 89] != 0f) continue;
            FgField[o + 89] = PatColOverride ? 1f : 0f;
            FgField[o + 90] = PatColMode;
            FgField[o + 91] = PatColR; FgField[o + 92] = PatColG; FgField[o + 93] = PatColB;
            FgField[o + 94] = PatCol2R; FgField[o + 95] = PatCol2G; FgField[o + 96] = PatCol2B;
            FgField[o + 97] = PatCol3R; FgField[o + 98] = PatCol3G; FgField[o + 99] = PatCol3B;
            FgField[o + 100] = PatCol4R; FgField[o + 101] = PatCol4G; FgField[o + 102] = PatCol4B;
            FgField[o + 103] = PatCol5R; FgField[o + 104] = PatCol5G; FgField[o + 105] = PatCol5B;
            FgField[o + 106] = PatMat;
            FgField[o + 107] = PatMatR; FgField[o + 108] = PatMatG; FgField[o + 109] = PatMatB;
            FgField[o + 110] = PatMatTint;
        }
    }

    private void MigrateElem()
    {
        if (Elem != null && Elem.Length == 8 * ElemStride) return;
        var dst = new float[8 * ElemStride];
        if (Elem != null && Elem.Length == 128)
            for (int L = 0; L < 8; L++)
                for (int k = 0; k < 16; k++)
                    dst[L * ElemStride + k] = Elem[L * 16 + k];
        Elem = dst;
    }
}

public sealed class TextMarker
{
    public string Text { get; set; } = "Text";
    public float X { get; set; } = 0.5f;
    public float Y { get; set; } = 0.5f;
    public float Size { get; set; } = 32f;
    public float R { get; set; } = 1f;
    public float G { get; set; } = 1f;
    public float B { get; set; } = 1f;
    public float A { get; set; } = 1f;
    public int Align { get; set; } = 1;
    public bool Outline { get; set; } = true;
}

