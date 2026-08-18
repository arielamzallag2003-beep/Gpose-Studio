using System;
using System.Runtime.InteropServices;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.D3DCompiler;
using Vortice.Mathematics;

namespace GPoseStudio;

public sealed class GpuRenderer : IDisposable
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Params
    {
        public float Exposure;
        public float Contrast;
        public float Saturation;
        public float Temperature;
        public float Tint;
        public float Vignette;
        public int SwapRedBlue;
        public int Flip;
        public float FogStart;
        public float FogStrength;
        public float FogColorR;
        public float FogColorG;
        public float FogColorB;
        public float BgPushStart;
        public float BgPushStrength;
        public int HasDepth;
        public float DofFocus;
        public float DofRange;
        public float DofStrength;
        public float DepthUvScaleX;
        public float DepthUvScaleY;
        public float TexelX;
        public float TexelY;
        public int DebugView;
        public float Lift;
        public float Gamma;
        public float Gain;
        public float Vibrance;
        public float Sharpen;
        public float Chroma;
        public float Grain;
        public float Letterbox;
        public float BlackPoint;
        public float WhitePoint;
        public float HueShift;
        public float Bleach;
        public float BleachContrast;
        public float TealOrange;
        public float TealOrangePunch;
        public float ToShadowR; public float ToShadowG; public float ToShadowB;
        public float ToHighR; public float ToHighG; public float ToHighB;
        public float ColorBalance;
        public float CbShadowR; public float CbShadowG; public float CbShadowB;
        public float CbMidR; public float CbMidG; public float CbMidB;
        public float CbHighR; public float CbHighG; public float CbHighB;
        public float FisheyeAmt;
        public float FisheyeZoom;
        public float SwirlAmt;
        public float SwirlRadius;
        public float MosaicSize;
        public float KaleidoSegs;
        public float KaleidoRot;
        public int Pad1;
        public int Pad2;
        public float BloomAmount;
        public float BloomThreshold;
        public float BloomRadius;
        public int Pad3;
        public float Halation;
        public float HalationR; public float HalationG; public float HalationB;
        public float GodrayAmount;
        public float GodrayLightX; public float GodrayLightY;
        public float GodrayDecay; public float GodrayThreshold;
        public float GodrayR; public float GodrayG; public float GodrayB;
        public float RimStrength; public float RimThreshold; public float RimWidth;
        public float RimR; public float RimG; public float RimB;
        public float BgRecolor; public float BgRecolorStart;
        public float BgTopR; public float BgTopG; public float BgTopB;
        public float BgBotR; public float BgBotG; public float BgBotB;
        public float BgBlur; public float BgBlurStart;
        public float Orton; public float Glamour; public float GlamourMist;
        public float SoftBlurRadius;
        public float GradMap;
        public float GmShadowR; public float GmShadowG; public float GmShadowB;
        public float GmMidR; public float GmMidG; public float GmMidB;
        public float GmHighR; public float GmHighG; public float GmHighB;
        public float Dehaze;
        public float WaveAmt; public float WaveFreq; public float WavePhase;
        public float GlitchAmt; public float GlitchBlocks;
        public float StShadowR; public float StShadowG; public float StShadowB;
        public float StHighR; public float StHighG; public float StHighB;
        public float StBalance; public float StAmount;
        public float Clarity;
        public float TiltAmt; public float TiltFocus; public float TiltRange;
        public float FlowAmt; public float FlowScale; public float FlowSeed;
        public int ScopeMode; public float ScopeSplit; public float ScopeSoft;
        public float EdgeAura; public float EdgeWidth; public float EdgeThreshold;
        public float EdgeR; public float EdgeG; public float EdgeB;
        public float Iridescent; public float IridFreq; public float IridShift;
        public float Prism;
        public float LeakAmt; public float LeakAngle;
        public float LeakR; public float LeakG; public float LeakB;
        public float AnamAmount; public float AnamThreshold; public float AnamLength;
        public float AnamR; public float AnamG; public float AnamB;
        public float HlRecovery;
        public float SubjectPop;
        public float HaloAmount; public float HaloSplit;
        public float HaloR; public float HaloG; public float HaloB;
        public float WashAmount; public float WashX; public float WashY;
        public float WashR; public float WashG; public float WashB;
        public float CausticsAmt; public float CausticsScale;
        public float CausticsR; public float CausticsG; public float CausticsB;
        public float ChromaClean;
        public float Denoise; public float DenoiseEdge;
        public float KuwaharaAmt; public float KuwaharaRadius;
        public float BgFill; public float BgFillStart;
        public int BgStyle; public float BgScale; public float BgAngle; public float BgGrain;
        public int Bypass; public int BgWarp; public float BgWarpAmt; public float BgWarpScale;
        public float BgOffX; public float BgOffY; public float BgScaleY; public float BgSharp;
        public float BgWarpX; public float BgWarpY; public float BgWarpAmt2; public float BgWarpScale2;
        public float BgMidR; public float BgMidG; public float BgMidB; public float BgMetallic;
        public float BgRoughness; public float BgSpecular; public float BgNormal; public float BgFresnel;
        public float BgLightX; public float BgLightY; public float BgLightZ; public float BgLightInt;
        public float BgCol4R; public float BgCol4G; public float BgCol4B; public float BgFbm;
        public float BgStars; public float BgStarDensity; public float BgStarSize; public float BgGlow;
        public float BgVignette; public float BgVignetteSize; public float BgHueVar; public float BgBright;
        public float BgNebWarp; public float BgNebContrast; public float BgVoidCore; public float BgVoidRing;
        public float BgTwist; public float BgHaze; public float BgSparkle; public float BgDisperse;
        public float BgRingWidth; public float BgRing2; public float BgEmbers; public float BgFlow;
        public float BgCol5R; public float BgCol5G; public float BgCol5B; public float BgCol6R;
        public float BgCol6G; public float BgCol6B; public float BgEmberSize; public float BgPad0;
        public float VhsStatic; public float VhsScan; public float VhsScanCount; public float VhsDropout;
        public float VhsRoll; public float VhsRollPos; public float VhsDesat; public float VhsVignette;
        public float BgReflect; public float BgMatDisp; public float BgAniso; public float BgEnvSharp;
        public float BgEnvR; public float BgEnvG; public float BgEnvB; public float BgClearcoat;
        public float BgCausticAmt; public float BgShafts; public float BgBubbles; public float BgPadA;
        public float UwTint; public float UwTintR; public float UwTintG; public float UwTintB;
        public float UwCaustic; public float UwMotes; public float UwShafts; public float UwFog;
        public float GroundLevel; public float GroundShadow; public float GroundRipple; public float GroundTintR;
        public float GroundTintG; public float GroundTintB; public float GroundShadowX; public float GroundShadowY;
        public float GroundShadowW; public float GroundShadowH; public float Time; public float AnimSpeed;
        public float HudIntensity; public float HudR; public float HudG; public float HudB;
        public float HudReticle; public float HudRadar; public float HudScanline; public float HudHex;
        public float HudChroma; public float HudFlicker; public float HudScale; public float HudFrame;
        public int BgGradType; public int BgPatMode; public float BgPatStrength; public float BgPatAngle;
        public int UnivBase; public int UnivNoise; public int UnivPattern; public int UnivBlend;
        public float UnivNoiseAmt; public float UnivNoiseScale; public float UnivWarp; public float UnivDetail;
        public float FrostAmount; public float FrostCoverage; public float FrostFeather; public float FrostPad;
        public fixed float Elem[160];
        public float BgBTopR; public float BgBTopG; public float BgBTopB; public float BgBBotR;
        public float BgBBotG; public float BgBBotB; public int BgBStyle; public float BgBScale;
        public float BgBAngle; public float BgBGrain; public int BgBWarp; public float BgBWarpAmt;
        public float BgBWarpScale; public float BgBOffX; public float BgBOffY; public float BgBScaleY;
        public float BgBSharp; public float BgBWarpX; public float BgBWarpY; public float BgBWarpAmt2;
        public float BgBWarpScale2; public float BgBMidR; public float BgBMidG; public float BgBMidB;
        public float BgBMetallic; public float BgBRoughness; public float BgBSpecular; public float BgBNormal;
        public float BgBFresnel; public float BgBLightX; public float BgBLightY; public float BgBLightZ;
        public float BgBLightInt; public float BgBCol4R; public float BgBCol4G; public float BgBCol4B;
        public float BgBFbm; public float BgBStars; public float BgBStarDensity; public float BgBStarSize;
        public float BgBGlow; public float BgBHueVar; public float BgBNebWarp; public float BgBNebContrast;
        public float BgBTwist; public float BgBHaze; public float BgBSparkle; public float BgBDisperse;
        public float BgBEmbers; public float BgBFlow; public float BgBCol5R; public float BgBCol5G;
        public float BgBCol5B; public float BgBCol6R; public float BgBCol6G; public float BgBCol6B;
        public float BgBEmberSize; public float BgBReflect; public float BgBMatDisp; public float BgBAniso;
        public float BgBEnvSharp; public float BgBEnvR; public float BgBEnvG; public float BgBEnvB;
        public float BgBClearcoat; public int BgBGradType; public int BgBPatMode; public float BgBPatStrength;
        public float BgBPatAngle; public int BgBUnivBase; public int BgBUnivNoise; public int BgBUnivPattern;
        public int BgBUnivBlend; public float BgBUnivNoiseAmt; public float BgBUnivNoiseScale; public float BgBUnivWarp;
        public float BgBUnivDetail; public float BgBPad0; public float BgBPad1; public float BgBPad2;
        public int BlendMode; public float BlendAngle; public float BlendOffset; public float BlendCx;
        public float BlendCy; public float BlendRadius; public float BlendEllipse; public float BlendDepthSplit;
        public float BlendDepthRef; public float BlendDepthBend; public float BlendFeather; public float BlendNoiseAmt;
        public float BlendNoiseScale; public float BlendMatch; public int BlendMix; public float BlendMixLevel;
        public float UnivHorizon; public int UnivGround; public int UnivOrb; public float UnivOrbX;
        public float UnivOrbY; public float UnivOrbSize; public float UnivRidges; public int UnivParticle;
        public float UnivCaustic; public float UnivShafts;
        public float BgBUnivCaustic; public float BgBUnivShafts;
        public int UnivPatBlend; public float UnivPatStrength;
        public int BgBUnivPatBlend; public float BgBUnivPatStrength;
        public float WetAmount; public float WetShine; public float WetRough; public float WetDeepen;
        public float WetDroplets; public float WetLightX; public float WetLightY; public float WetDepth;
        public float WetHighlight; public float WetFresnel; public float WetDropSize; public float WetDropDensity;
        public float WetDropTrail; public float Pad16A; public float Pad16B; public float Pad16C;
        public float BgBUnivHorizon; public int BgBUnivGround; public int BgBUnivOrb; public float BgBUnivOrbX;
        public float BgBUnivOrbY; public float BgBUnivOrbSize; public float BgBUnivRidges; public int BgBUnivParticle;
        public int EnForeground; public int FgPlaceMode; public float FgPlaceSoft; public float FgPlaceSize;
        public float FgPlaceAngle; public float FgOpacity; public int FgBlendMode; public int FgDepthGate;
        public int FgSeamMode; public float FgSeamAngle; public float FgSeamOffset; public float FgSeamCx;
        public float FgSeamCy; public float FgSeamRadius; public float FgSeamEllipse; public float FgSeamDepthSplit;
        public float FgSeamDepthRef; public float FgSeamDepthBend; public float FgSeamFeather; public float FgSeamNoiseAmt;
        public float FgSeamNoiseScale; public int FgSeamMix; public float FgSeamMixLevel; public float FgSeamMatch;
        public fixed float FgField[224];
        public int GoboPattern; public float GoboAmount; public float GoboScale; public float GoboAngle;
        public float GoboSoft; public float BeautyAmount; public float BeautyRadius; public float BeautyGlow;
        public float SkinWarmth; public float SkinFlush; public float SkinTintR; public float SkinTintG;
        public float SkinTintB; public float BacklightAmount; public float BacklightWidth; public float BacklightR;
        public float BacklightG; public float BacklightB; public float SpotAmount; public float SpotX;
        public float SpotY; public float SpotRadius; public float SpotEllipse; public float SpotSoft;
        public float SpotAngle; public float SpotWarm; public int ParticleType; public float ParticleAmount;
        public float ParticleSize; public float ParticleFall; public float ParticleR; public float ParticleG;
        public float ParticleB; public int BokehShape; public float BokehAmount; public float BgRecolorFeather;
        public float BgFillR; public float BgFillG; public float BgFillB; public float BgFillFeather;
        public float ShadowAmount; public float ShadowSpread; public float ShadowOffsetX; public float ShadowOffsetY;
        public float ShadowSoftness; public float ShadowR; public float ShadowG; public float ShadowB;
        public float ShadowContact; public float ShadowDepth; public float ShadowPad0; public float ShadowPad1;
        public float EdgeErode; public float EdgeDespill; public float EdgeWrap; public float EdgeWrapWidth;
        public float FilmRolloff; public float FilmToe; public float FilmSat; public float FilmPad0;
        public float LensVig; public float LensCornerSoft; public float ChromaRadial; public float LensPad0;
        public float BackdropLightAmt; public float BackdropLightX; public float BackdropLightY; public float BackdropLightSize;
        public float ZoneNear; public float ZoneNearSoft; public int ZoneWet; public int ZoneBeauty;
        public int ZoneSkin; public int ZoneBacklight; public int ZoneShadow; public int ZoneBokeh;
        public int ZoneBgPush; public int ZoneBgBlur; public int ZonePad0; public int ZonePad1;
        public int ZoneGobo; public int ZoneSpot; public int ZoneFrost; public int ZoneStylize;
        public int ZoneUnderwater; public int ZoneVhs; public int ZoneRim; public int ZoneGround;
        public int ZoneHalo; public int ZoneCb; public int ZoneTeal; public int ZoneSplitTone;
        public int ZoneBleach; public int ZoneGradMap; public int ZonePad2; public int ZonePad3;
        public float RimSplit; public float RimSplitAngle; public float RimSplitOffset; public float RimSplitSoft;
        public float Rim2R; public float Rim2G; public float Rim2B; public float Backlight2R;
        public float Backlight2G; public float Backlight2B; public float RimPad0; public float RimPad1;
        public int PatMat; public float PatMatR; public float PatMatG; public float PatMatB;
        public float PatMatRough; public float PatMatSheen; public float PatMatPos; public float PatMatRange;
        public int PatColOverride; public float PatColR; public float PatColG; public float PatColB;
        public int PatColMode; public float PatCol2R; public float PatCol2G; public float PatCol2B;
        public float PatMatTint; public float PatCol3R; public float PatCol3G; public float PatCol3B;
        public int BgBPatColOverride; public int BgBPatColMode; public float BgBPatColR; public float BgBPatColG;
        public float BgBPatColB; public float BgBPatCol2R; public float BgBPatCol2G; public float BgBPatCol2B;
        public float BgBPatCol3R; public float BgBPatCol3G; public float BgBPatCol3B; public float BgBPatCol4R;
        public float BgBPatCol4G; public float BgBPatCol4B; public float BgBPatCol5R; public float BgBPatCol5G;
        public float BgBPatCol5B; public int BgBPatMat; public float BgBPatMatR; public float BgBPatMatG;
        public float BgBPatMatB; public float BgBPatMatTint; public float PatPadA; public float PatPadB;
        public float PatCol4R; public float PatCol4G; public float PatCol4B; public float PatCol5R;
        public float PatCol5G; public float PatCol5B; public float PatColPad3; public float PatColPad4;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct BloomParams
    {
        public float TexelX; public float TexelY;
        public float DirX; public float DirY;
        public float Threshold; public float Radius;
        public float Pad0; public float Pad1;
    }

    private const string Hlsl = @"
cbuffer P : register(b0) {
    float exposure; float contrast; float saturation; float temperature;
    float tint; float vignette; int swapRB; int flip;
    float fogStart; float fogStrength; float fogColorR; float fogColorG;
    float fogColorB; float bgPushStart; float bgPushStrength; int hasDepth;
    float dofFocus; float dofRange; float dofStrength; float depthUvScaleX;
    float depthUvScaleY; float texelX; float texelY; int debugView;
    float lift; float gamma; float gain; float vibrance;
    float sharpen; float chroma; float grain; float letterbox;
    float blackPoint; float whitePoint; float hueShift; float bleach;
    float bleachContrast; float tealOrange; float tealOrangePunch; float toShadowR;
    float toShadowG; float toShadowB; float toHighR; float toHighG;
    float toHighB; float colorBalance; float cbShadowR; float cbShadowG;
    float cbShadowB; float cbMidR; float cbMidG; float cbMidB;
    float cbHighR; float cbHighG; float cbHighB; float fisheyeAmt;
    float fisheyeZoom; float swirlAmt; float swirlRadius; float mosaicSize;
    float kaleidoSegs; float kaleidoRot; int pad1; int pad2;
    float bloomAmount; float bloomThreshold; float bloomRadius; int pad3;
    float halation; float halationR; float halationG; float halationB;
    float godrayAmount; float godrayLightX; float godrayLightY; float godrayDecay;
    float godrayThreshold; float godrayR; float godrayG; float godrayB;
    float rimStrength; float rimThreshold; float rimWidth; float rimR;
    float rimG; float rimB; float bgRecolor; float bgRecolorStart;
    float bgTopR; float bgTopG; float bgTopB; float bgBotR;
    float bgBotG; float bgBotB; float bgBlur; float bgBlurStart;
    float orton; float glamour; float glamourMist; float softBlurRadius;
    float gradMap; float gmShadowR; float gmShadowG; float gmShadowB;
    float gmMidR; float gmMidG; float gmMidB; float gmHighR;
    float gmHighG; float gmHighB; float dehaze; float waveAmt;
    float waveFreq; float wavePhase; float glitchAmt; float glitchBlocks;
    float stShadowR; float stShadowG; float stShadowB; float stHighR;
    float stHighG; float stHighB; float stBalance; float stAmount;
    float clarity; float tiltAmt; float tiltFocus; float tiltRange;
    float flowAmt; float flowScale; float flowSeed; int scopeMode;
    float scopeSplit; float scopeSoft; float edgeAura; float edgeWidth;
    float edgeThreshold; float edgeR; float edgeG; float edgeB;
    float iridescent; float iridFreq; float iridShift; float prism;
    float leakAmt; float leakAngle; float leakR; float leakG;
    float leakB; float anamAmount; float anamThreshold; float anamLength;
    float anamR; float anamG; float anamB; float hlRecovery;
    float subjectPop; float haloAmount; float haloSplit; float haloR;
    float haloG; float haloB; float washAmount; float washX;
    float washY; float washR; float washG; float washB;
    float causticsAmt; float causticsScale; float causticsR; float causticsG;
    float causticsB; float chromaClean; float denoise; float denoiseEdge;
    float kuwaharaAmt; float kuwaharaRadius; float bgFill; float bgFillStart;
    int bgStyle; float bgScale; float bgAngle; float bgGrain;
    int bypass; int bgWarp; float bgWarpAmt; float bgWarpScale;
    float bgOffX; float bgOffY; float bgScaleY; float bgSharp;
    float bgWarpX; float bgWarpY; float bgWarpAmt2; float bgWarpScale2;
    float bgMidR; float bgMidG; float bgMidB; float bgMetallic;
    float bgRoughness; float bgSpecular; float bgNormal; float bgFresnel;
    float bgLightX; float bgLightY; float bgLightZ; float bgLightInt;
    float bgCol4R; float bgCol4G; float bgCol4B; float bgFbm;
    float bgStars; float bgStarDensity; float bgStarSize; float bgGlow;
    float bgVignette; float bgVignetteSize; float bgHueVar; float bgBright;
    float bgNebWarp; float bgNebContrast; float bgVoidCore; float bgVoidRing;
    float bgTwist; float bgHaze; float bgSparkle; float bgDisperse;
    float bgRingWidth; float bgRing2; float bgEmbers; float bgFlow;
    float bgCol5R; float bgCol5G; float bgCol5B; float bgCol6R;
    float bgCol6G; float bgCol6B; float bgEmberSize; float bgPad0;
    float vhsStatic; float vhsScan; float vhsScanCount; float vhsDropout;
    float vhsRoll; float vhsRollPos; float vhsDesat; float vhsVignette;
    float bgReflect; float bgMatDisp; float bgAniso; float bgEnvSharp;
    float bgEnvR; float bgEnvG; float bgEnvB; float bgClearcoat;
    float bgCausticAmt; float bgShafts; float bgBubbles; float bgPadA;
    float uwTint; float uwTintR; float uwTintG; float uwTintB;
    float uwCaustic; float uwMotes; float uwShafts; float uwFog;
    float groundLevel; float groundShadow; float groundRipple; float groundTintR;
    float groundTintG; float groundTintB; float groundShadowX; float groundShadowY;
    float groundShadowW; float groundShadowH; float time; float animSpeed;
    float hudIntensity; float hudR; float hudG; float hudB;
    float hudReticle; float hudRadar; float hudScanline; float hudHex;
    float hudChroma; float hudFlicker; float hudScale; float hudFrame;
    int bgGradType; int bgPatMode; float bgPatStrength; float bgPatAngle;
    int univBase; int univNoise; int univPattern; int univBlend;
    float univNoiseAmt; float univNoiseScale; float univWarp; float univDetail;
    float frostAmount; float frostCoverage; float frostFeather; float frostPad;
    float4 elem[40];
    float bgBTopR; float bgBTopG; float bgBTopB; float bgBBotR;
    float bgBBotG; float bgBBotB; int bgBStyle; float bgBScale;
    float bgBAngle; float bgBGrain; int bgBWarp; float bgBWarpAmt;
    float bgBWarpScale; float bgBOffX; float bgBOffY; float bgBScaleY;
    float bgBSharp; float bgBWarpX; float bgBWarpY; float bgBWarpAmt2;
    float bgBWarpScale2; float bgBMidR; float bgBMidG; float bgBMidB;
    float bgBMetallic; float bgBRoughness; float bgBSpecular; float bgBNormal;
    float bgBFresnel; float bgBLightX; float bgBLightY; float bgBLightZ;
    float bgBLightInt; float bgBCol4R; float bgBCol4G; float bgBCol4B;
    float bgBFbm; float bgBStars; float bgBStarDensity; float bgBStarSize;
    float bgBGlow; float bgBHueVar; float bgBNebWarp; float bgBNebContrast;
    float bgBTwist; float bgBHaze; float bgBSparkle; float bgBDisperse;
    float bgBEmbers; float bgBFlow; float bgBCol5R; float bgBCol5G;
    float bgBCol5B; float bgBCol6R; float bgBCol6G; float bgBCol6B;
    float bgBEmberSize; float bgBReflect; float bgBMatDisp; float bgBAniso;
    float bgBEnvSharp; float bgBEnvR; float bgBEnvG; float bgBEnvB;
    float bgBClearcoat; int bgBGradType; int bgBPatMode; float bgBPatStrength;
    float bgBPatAngle; int bgBUnivBase; int bgBUnivNoise; int bgBUnivPattern;
    int bgBUnivBlend; float bgBUnivNoiseAmt; float bgBUnivNoiseScale; float bgBUnivWarp;
    float bgBUnivDetail; float bgBPad0; float bgBPad1; float bgBPad2;
    int blendMode; float blendAngle; float blendOffset; float blendCx;
    float blendCy; float blendRadius; float blendEllipse; float blendDepthSplit;
    float blendDepthRef; float blendDepthBend; float blendFeather; float blendNoiseAmt;
    float blendNoiseScale; float blendMatch; int blendMix; float blendMixLevel;
    float univHorizon; int univGround; int univOrb; float univOrbX;
    float univOrbY; float univOrbSize; float univRidges; int univParticle;
    float univCaustic; float univShafts;
    float bgBUnivCaustic; float bgBUnivShafts;
    int univPatBlend; float univPatStrength; int bgBUnivPatBlend; float bgBUnivPatStrength;
    float wetAmount; float wetShine; float wetRough; float wetDeepen;
    float wetDroplets; float wetLightX; float wetLightY; float wetDepth;
    float wetHighlight; float wetFresnel; float wetDropSize; float wetDropDensity;
    float wetDropTrail; float pad16A; float pad16B; float pad16C;
    float bgBUnivHorizon; int bgBUnivGround; int bgBUnivOrb; float bgBUnivOrbX;
    float bgBUnivOrbY; float bgBUnivOrbSize; float bgBUnivRidges; int bgBUnivParticle;
    int enForeground; int fgPlaceMode; float fgPlaceSoft; float fgPlaceSize;
    float fgPlaceAngle; float fgOpacity; int fgBlendMode; int fgDepthGate;
    int fgSeamMode; float fgSeamAngle; float fgSeamOffset; float fgSeamCx;
    float fgSeamCy; float fgSeamRadius; float fgSeamEllipse; float fgSeamDepthSplit;
    float fgSeamDepthRef; float fgSeamDepthBend; float fgSeamFeather; float fgSeamNoiseAmt;
    float fgSeamNoiseScale; int fgSeamMix; float fgSeamMixLevel; float fgSeamMatch;
    float4 fgField[56];
    int goboPattern; float goboAmount; float goboScale; float goboAngle;
    float goboSoft; float beautyAmount; float beautyRadius; float beautyGlow;
    float skinWarmth; float skinFlush; float skinTintR; float skinTintG;
    float skinTintB; float backlightAmount; float backlightWidth; float backlightR;
    float backlightG; float backlightB; float spotAmount; float spotX;
    float spotY; float spotRadius; float spotEllipse; float spotSoft;
    float spotAngle; float spotWarm; int particleType; float particleAmount;
    float particleSize; float particleFall; float particleR; float particleG;
    float particleB; int bokehShape; float bokehAmount; float bgRecolorFeather;
    float bgFillR; float bgFillG; float bgFillB; float bgFillFeather;
    float shadowAmount; float shadowSpread; float shadowOffsetX; float shadowOffsetY;
    float shadowSoftness; float shadowR; float shadowG; float shadowB;
    float shadowContact; float shadowDepth; float shadowPad0; float shadowPad1;
    float edgeErode; float edgeDespill; float edgeWrap; float edgeWrapWidth;
    float filmRolloff; float filmToe; float filmSat; float filmPad0;
    float lensVig; float lensCornerSoft; float chromaRadial; float lensPad0;
    float backdropLightAmt; float backdropLightX; float backdropLightY; float backdropLightSize;
    float zoneNear; float zoneNearSoft; int zoneWet; int zoneBeauty;
    int zoneSkin; int zoneBacklight; int zoneShadow; int zoneBokeh;
    int zoneBgPush; int zoneBgBlur; int zonePad0; int zonePad1;
    int zoneGobo; int zoneSpot; int zoneFrost; int zoneStylize;
    int zoneUnderwater; int zoneVhs; int zoneRim; int zoneGround;
    int zoneHalo; int zoneCb; int zoneTeal; int zoneSplitTone;
    int zoneBleach; int zoneGradMap; int zonePad2; int zonePad3;
    float rimSplit; float rimSplitAngle; float rimSplitOffset; float rimSplitSoft;
    float rim2R; float rim2G; float rim2B; float backlight2R;
    float backlight2G; float backlight2B; float rimPad0; float rimPad1;
    int patMat; float patMatR; float patMatG; float patMatB;
    float patMatRough; float patMatSheen; float patMatPos; float patMatRange;
    int patColOverride; float patColR; float patColG; float patColB;
    int patColMode; float patCol2R; float patCol2G; float patCol2B;
    float patMatTint; float patCol3R; float patCol3G; float patCol3B;
    int bgBPatColOverride; int bgBPatColMode; float bgBPatColR; float bgBPatColG;
    float bgBPatColB; float bgBPatCol2R; float bgBPatCol2G; float bgBPatCol2B;
    float bgBPatCol3R; float bgBPatCol3G; float bgBPatCol3B; float bgBPatCol4R;
    float bgBPatCol4G; float bgBPatCol4B; float bgBPatCol5R; float bgBPatCol5G;
    float bgBPatCol5B; int bgBPatMat; float bgBPatMatR; float bgBPatMatG;
    float bgBPatMatB; float bgBPatMatTint; float patPadA; float patPadB;
    float patCol4R; float patCol4G; float patCol4B; float patCol5R;
    float patCol5G; float patCol5B; float patColPad3; float patColPad4;
};
Texture2D colorTex : register(t0);
Texture2D depthTex : register(t1);
Texture2D bloomTex : register(t2);
Texture2D godrayTex : register(t3);
Texture2D fullBlurTex : register(t4);
Texture2D anamTex : register(t5);
Texture2D haloTex : register(t6);
Texture2D memeTex0 : register(t7);
Texture2D memeTex1 : register(t8);
Texture2D memeTex2 : register(t9);
Texture2D memeTex3 : register(t10);
Texture2D memeTex4 : register(t11);
Texture2D memeTex5 : register(t12);
Texture2D memeTex6 : register(t13);
Texture2D memeTex7 : register(t14);
SamplerState samp : register(s0);

float4 SampleMeme(int i, float2 uv) {
    if (i == 0) return memeTex0.SampleLevel(samp, uv, 0.0);
    if (i == 1) return memeTex1.SampleLevel(samp, uv, 0.0);
    if (i == 2) return memeTex2.SampleLevel(samp, uv, 0.0);
    if (i == 3) return memeTex3.SampleLevel(samp, uv, 0.0);
    if (i == 4) return memeTex4.SampleLevel(samp, uv, 0.0);
    if (i == 5) return memeTex5.SampleLevel(samp, uv, 0.0);
    if (i == 6) return memeTex6.SampleLevel(samp, uv, 0.0);
    return memeTex7.SampleLevel(samp, uv, 0.0);
}
float2 MemeDims(int i) {
    float w = 1.0, h = 1.0;
    if (i == 0) memeTex0.GetDimensions(w, h);
    else if (i == 1) memeTex1.GetDimensions(w, h);
    else if (i == 2) memeTex2.GetDimensions(w, h);
    else if (i == 3) memeTex3.GetDimensions(w, h);
    else if (i == 4) memeTex4.GetDimensions(w, h);
    else if (i == 5) memeTex5.GetDimensions(w, h);
    else if (i == 6) memeTex6.GetDimensions(w, h);
    else memeTex7.GetDimensions(w, h);
    return float2(w, h);
}

static const float2 DISK[8] = {
    float2(1,0), float2(0.7071,0.7071), float2(0,1), float2(-0.7071,0.7071),
    float2(-1,0), float2(-0.7071,-0.7071), float2(0,-1), float2(0.7071,-0.7071)
};

struct VSOut { float4 pos : SV_Position; float2 uv : TEXCOORD0; };
VSOut VS(uint id : SV_VertexID) {
    VSOut o;
    float2 uv = float2((id << 1) & 2, id & 2);
    o.pos = float4(uv * 2.0 - 1.0, 0.0, 1.0);
    o.uv = uv;
    return o;
}

float Linearize(float2 duv) {
    float rz = depthTex.Sample(samp, duv).r;
    float z = 1.0 - rz;
    const float FAR = 1000.0;
    return z / (FAR - z * (FAR - 1.0));
}
float LinearizeL(float2 duv) {
    float rz = depthTex.SampleLevel(samp, duv, 0).r;
    float z = 1.0 - rz;
    const float FAR = 1000.0;
    return z / (FAR - z * (FAR - 1.0));
}

float DepthCoverage(float2 duvC, float2 rad, float linC, float bias, float sgn) {
    float cov = 0.0;
    [unroll] for (int k = 0; k < 16; k++) {
        float a = (float)k * 2.39996323;
        float r = sqrt(((float)k + 0.5) * 0.0625);
        float2 o = float2(cos(a), sin(a)) * r * rad;
        cov += smoothstep(0.0, max(bias, 1e-4), sgn * (LinearizeL(duvC + o) - linC));
    }
    return cov * 0.0625;
}

float Luma(float3 c) { return dot(c, float3(0.299, 0.587, 0.114)); }

float RimSide(float2 uv, float asp) {
    float2 d = uv - 0.5; d.x *= asp;
    float sd = dot(d, float2(cos(rimSplitAngle), sin(rimSplitAngle))) - rimSplitOffset;
    float f = max(rimSplitSoft, 1e-3);
    return smoothstep(-f, f, sd);
}

float ZoneMask(int bits, float lin, float split, float soft) {
    if (hasDepth == 0 || bits == 7) return 1.0;
    if (bits == 0) return 0.0;
    float bg = smoothstep(split, split + max(soft, 1e-3), lin);
    float fg = (zoneNear > 0.0) ? (1.0 - smoothstep(zoneNear, zoneNear + max(zoneNearSoft, 1e-3), lin)) : 0.0;
    fg = min(fg, 1.0 - bg);
    float ch = saturate(1.0 - fg - bg);
    float m = 0.0;
    if ((bits & 1) != 0) m += fg;
    if ((bits & 2) != 0) m += ch;
    if ((bits & 4) != 0) m += bg;
    return saturate(m);
}

float3 Band3(float ph, float3 a, float3 mid, float3 b) {
    ph = frac(ph);
    return ph < 0.33333 ? a : (ph < 0.66667 ? mid : b);
}
float3 Ramp5(float t, float3 a, float3 b, float3 c, float3 d, float3 e) {
    t = saturate(t) * 4.0;
    if (t < 1.0) return lerp(a, b, t);
    if (t < 2.0) return lerp(b, c, t - 1.0);
    if (t < 3.0) return lerp(c, d, t - 2.0);
    return lerp(d, e, t - 3.0);
}

float3 CraftHue(float t) {
    t = frac(t) * 8.0;
    if (t < 1.0) return float3(0.95, 0.76, 0.30);
    if (t < 2.0) return float3(0.42, 0.62, 0.86);
    if (t < 3.0) return float3(0.45, 0.80, 0.52);
    if (t < 4.0) return float3(0.93, 0.86, 0.78);
    if (t < 5.0) return float3(0.82, 0.50, 0.28);
    if (t < 6.0) return float3(0.72, 0.83, 0.44);
    if (t < 7.0) return float3(0.90, 0.48, 0.44);
    return float3(0.85, 0.42, 0.30);
}

float Hash21(float2 p) { return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453); }
float VNoise(float2 p) {
    float2 ip = floor(p), fp = frac(p);
    float2 u = fp * fp * (3.0 - 2.0 * fp);
    float a = Hash21(ip), b = Hash21(ip + float2(1.0, 0.0));
    float cc = Hash21(ip + float2(0.0, 1.0)), d = Hash21(ip + float2(1.0, 1.0));
    return lerp(lerp(a, b, u.x), lerp(cc, d, u.x), u.y);
}
float Fbm(float2 p, int oct) {
    float v = 0.0, a = 0.5, tot = 0.0;
    [loop] for (int k = 0; k < oct; k++) { v += a * VNoise(p); tot += a; p = p * 2.03 + 1.7; a *= 0.5; }
    return v / max(tot, 1e-4);
}
float RidgedFbm(float2 p, int oct) {
    float v = 0.0, a = 0.5, tot = 0.0;
    [loop] for (int k = 0; k < oct; k++) {
        float n = 1.0 - abs(2.0 * VNoise(p) - 1.0);
        v += n * n * a; tot += a; p = p * 2.03 + 1.7; a *= 0.5;
    }
    return v / max(tot, 1e-4);
}
float Voronoi(float2 p) {
    float2 ip = floor(p), fp = frac(p);
    float md = 1.5;
    [loop] for (int j = -1; j <= 1; j++) {
        [loop] for (int i = -1; i <= 1; i++) {
            float2 g = float2((float)i, (float)j);
            float2 o = float2(Hash21(ip + g), Hash21(ip + g + 3.7));
            float2 r = g + o - fp;
            md = min(md, dot(r, r));
        }
    }
    return sqrt(md);
}
float2 Voro2(float2 p) {
    float2 ip = floor(p), fp = frac(p);
    float f1 = 1.5, f2 = 1.5;
    [loop] for (int j = -1; j <= 1; j++) {
        [loop] for (int i = -1; i <= 1; i++) {
            float2 g = float2((float)i, (float)j);
            float2 o = float2(Hash21(ip + g), Hash21(ip + g + 3.7));
            float2 r = g + o - fp; float d = dot(r, r);
            if (d < f1) { f2 = f1; f1 = d; } else if (d < f2) f2 = d;
        }
    }
    return float2(sqrt(f1), sqrt(f2));
}
float BillowFbm(float2 p, int oct) {
    float v = 0.0, a = 0.5, tot = 0.0;
    [loop] for (int k = 0; k < oct; k++) {
        v += abs(2.0 * VNoise(p) - 1.0) * a; tot += a; p = p * 2.03 + 1.7; a *= 0.5;
    }
    return v / max(tot, 1e-4);
}
float AaStep(float e, float x) { float w = max(fwidth(x), 1e-4); return smoothstep(e - w, e + w, x); }

float3 Kuwahara(float2 uv) {
    float2 t = float2(texelX, texelY);
    int R = (int)round(clamp(kuwaharaRadius, 1.0, 5.0));
    float3 m0 = float3(0,0,0), m1 = m0, m2 = m0, m3 = m0;
    float s0 = 0, s1 = 0, s2 = 0, s3 = 0, n0 = 0, n1 = 0, n2 = 0, n3 = 0;
    [loop] for (int dy = -R; dy <= R; dy++) {
        [loop] for (int dx = -R; dx <= R; dx++) {
            float3 col = colorTex.Sample(samp, uv + float2(dx, dy) * t).rgb;
            float l2 = Luma(col); l2 = l2 * l2;
            if (dx <= 0 && dy <= 0) { m0 += col; s0 += l2; n0 += 1.0; }
            if (dx >= 0 && dy <= 0) { m1 += col; s1 += l2; n1 += 1.0; }
            if (dx <= 0 && dy >= 0) { m2 += col; s2 += l2; n2 += 1.0; }
            if (dx >= 0 && dy >= 0) { m3 += col; s3 += l2; n3 += 1.0; }
        }
    }
    m0 /= n0; m1 /= n1; m2 /= n2; m3 /= n3;
    float v0 = s0 / n0 - Luma(m0) * Luma(m0);
    float v1 = s1 / n1 - Luma(m1) * Luma(m1);
    float v2 = s2 / n2 - Luma(m2) * Luma(m2);
    float v3 = s3 / n3 - Luma(m3) * Luma(m3);
    float3 best = m0; float bv = v0;
    if (v1 < bv) { bv = v1; best = m1; }
    if (v2 < bv) { bv = v2; best = m2; }
    if (v3 < bv) { best = m3; }
    return best;
}

float3 HueShift(float3 c, float turns) {
    float a = turns * 6.2831853;
    float3 k = float3(0.57735, 0.57735, 0.57735);
    float ca = cos(a);
    return c * ca + cross(k, c) * sin(a) + k * dot(k, c) * (1.0 - ca);
}

float SegSD(float2 p, float2 a, float2 b) {
    float2 pa = p - a, ba = b - a;
    float h = saturate(dot(pa, ba) / max(dot(ba, ba), 1e-6));
    return length(pa - ba * h);
}

float NgonSD(float2 p, float r, float n) {
    float seg = 6.2831853 / max(n, 3.0);
    float a = atan2(p.y, p.x);
    a = a - seg * floor(a / seg + 0.5);
    return length(p) * cos(a) - r;
}

float2 WarpFisheye(float2 uv, float amt, float zoom, float asp) {
    float2 p = uv - 0.5; p.x *= asp; p /= zoom;
    p *= 1.0 + amt * dot(p, p);
    p.x /= asp; return p + 0.5;
}
float2 WarpSwirl(float2 uv, float amt, float radius, float asp) {
    float2 p = uv - 0.5; p.x *= asp;
    float a = amt * saturate(1.0 - length(p) / max(radius, 1e-3));
    float s = sin(a), co = cos(a);
    p = float2(p.x * co - p.y * s, p.x * s + p.y * co);
    p.x /= asp; return p + 0.5;
}
float2 WarpKaleido(float2 uv, float segs, float rot, float asp) {
    float2 p = uv - 0.5; p.x *= asp;
    float ang = atan2(p.y, p.x) - rot;
    float rad = length(p);
    float seg = 6.2831853 / segs;
    ang = ang - seg * floor(ang / seg);
    ang = min(ang, seg - ang) + rot;
    p = float2(cos(ang), sin(ang)) * rad;
    p.x /= asp; return p + 0.5;
}

float2 ApplyWarp(float2 uv, float asp) {
    if (kaleidoSegs >= 2.0) uv = WarpKaleido(uv, kaleidoSegs, kaleidoRot, asp);
    if (swirlAmt != 0.0) uv = WarpSwirl(uv, swirlAmt, swirlRadius, asp);
    if (fisheyeAmt != 0.0 || fisheyeZoom != 1.0) uv = WarpFisheye(uv, fisheyeAmt, fisheyeZoom, asp);
    if (mosaicSize >= 2.0) {
        float2 cell = float2(mosaicSize * texelX, mosaicSize * texelY);
        uv = (floor(uv / cell) + 0.5) * cell;
    }
    if (waveAmt > 0.0) {
        uv.x += sin(uv.y * waveFreq + wavePhase) * waveAmt;
        uv.y += cos(uv.x * waveFreq + wavePhase) * waveAmt;
    }
    if (glitchAmt > 0.0) {
        float row = floor(uv.y * max(glitchBlocks, 1.0));
        float h = frac(sin(row * 12.9898 + 4.14) * 43758.5453);
        uv.x += (h > 0.7 ? (h - 0.85) * 2.0 : 0.0) * glitchAmt;
    }
    if (flowAmt > 0.0) {
        float2 q = uv * flowScale + flowSeed;
        float2 flow = float2(sin(q.x + cos(q.y * 1.3)), cos(q.y + sin(q.x * 1.3)));
        uv += flow * flowAmt;
    }
    return uv;
}

struct BgParams {
    float bgTopR; float bgTopG; float bgTopB; float bgBotR;
    float bgBotG; float bgBotB; int bgStyle; float bgScale;
    float bgAngle; float bgGrain; int bgWarp; float bgWarpAmt;
    float bgWarpScale; float bgOffX; float bgOffY; float bgScaleY;
    float bgSharp; float bgWarpX; float bgWarpY; float bgWarpAmt2;
    float bgWarpScale2; float bgMidR; float bgMidG; float bgMidB;
    float bgMetallic; float bgRoughness; float bgSpecular; float bgNormal;
    float bgFresnel; float bgLightX; float bgLightY; float bgLightZ;
    float bgLightInt; float bgCol4R; float bgCol4G; float bgCol4B;
    float bgFbm; float bgStars; float bgStarDensity; float bgStarSize;
    float bgGlow; float bgHueVar; float bgNebWarp; float bgNebContrast;
    float bgTwist; float bgHaze; float bgSparkle; float bgDisperse;
    float bgEmbers; float bgFlow; float bgCol5R; float bgCol5G;
    float bgCol5B; float bgCol6R; float bgCol6G; float bgCol6B;
    float bgEmberSize; float bgReflect; float bgMatDisp; float bgAniso;
    float bgEnvSharp; float bgEnvR; float bgEnvG; float bgEnvB;
    float bgClearcoat; int bgGradType; int bgPatMode; float bgPatStrength;
    float bgPatAngle; int univBase; int univNoise; int univPattern;
    int univBlend; float univNoiseAmt; float univNoiseScale; float univWarp;
    float univDetail;
    float univHorizon; int univGround; int univOrb; float univOrbX;
    float univOrbY; float univOrbSize; float univRidges; int univParticle;
    float univCaustic; float univShafts;
    int univPatBlend; float univPatStrength;
    int patColOverride; int patColMode; float patColR; float patColG;
    float patColB; float patCol2R; float patCol2G; float patCol2B;
    float patCol3R; float patCol3G; float patCol3B; float patCol4R;
    float patCol4G; float patCol4B; float patCol5R; float patCol5G;
    float patCol5B; int patMat; float patMatR; float patMatG;
    float patMatB; float patMatTint;
};

struct BgResult {
    float3 pat;
    float2 uv;
    float2 sc;
};

BgParams MakeBg(int idx) {
    BgParams bgp;
    bgp.bgTopR = idx == 0 ? bgTopR : bgBTopR;
    bgp.bgTopG = idx == 0 ? bgTopG : bgBTopG;
    bgp.bgTopB = idx == 0 ? bgTopB : bgBTopB;
    bgp.bgBotR = idx == 0 ? bgBotR : bgBBotR;
    bgp.bgBotG = idx == 0 ? bgBotG : bgBBotG;
    bgp.bgBotB = idx == 0 ? bgBotB : bgBBotB;
    bgp.bgStyle = idx == 0 ? bgStyle : bgBStyle;
    bgp.bgScale = idx == 0 ? bgScale : bgBScale;
    bgp.bgAngle = idx == 0 ? bgAngle : bgBAngle;
    bgp.bgGrain = idx == 0 ? bgGrain : bgBGrain;
    bgp.bgWarp = idx == 0 ? bgWarp : bgBWarp;
    bgp.bgWarpAmt = idx == 0 ? bgWarpAmt : bgBWarpAmt;
    bgp.bgWarpScale = idx == 0 ? bgWarpScale : bgBWarpScale;
    bgp.bgOffX = idx == 0 ? bgOffX : bgBOffX;
    bgp.bgOffY = idx == 0 ? bgOffY : bgBOffY;
    bgp.bgScaleY = idx == 0 ? bgScaleY : bgBScaleY;
    bgp.bgSharp = idx == 0 ? bgSharp : bgBSharp;
    bgp.bgWarpX = idx == 0 ? bgWarpX : bgBWarpX;
    bgp.bgWarpY = idx == 0 ? bgWarpY : bgBWarpY;
    bgp.bgWarpAmt2 = idx == 0 ? bgWarpAmt2 : bgBWarpAmt2;
    bgp.bgWarpScale2 = idx == 0 ? bgWarpScale2 : bgBWarpScale2;
    bgp.bgMidR = idx == 0 ? bgMidR : bgBMidR;
    bgp.bgMidG = idx == 0 ? bgMidG : bgBMidG;
    bgp.bgMidB = idx == 0 ? bgMidB : bgBMidB;
    bgp.bgMetallic = idx == 0 ? bgMetallic : bgBMetallic;
    bgp.bgRoughness = idx == 0 ? bgRoughness : bgBRoughness;
    bgp.bgSpecular = idx == 0 ? bgSpecular : bgBSpecular;
    bgp.bgNormal = idx == 0 ? bgNormal : bgBNormal;
    bgp.bgFresnel = idx == 0 ? bgFresnel : bgBFresnel;
    bgp.bgLightX = idx == 0 ? bgLightX : bgBLightX;
    bgp.bgLightY = idx == 0 ? bgLightY : bgBLightY;
    bgp.bgLightZ = idx == 0 ? bgLightZ : bgBLightZ;
    bgp.bgLightInt = idx == 0 ? bgLightInt : bgBLightInt;
    bgp.bgCol4R = idx == 0 ? bgCol4R : bgBCol4R;
    bgp.bgCol4G = idx == 0 ? bgCol4G : bgBCol4G;
    bgp.bgCol4B = idx == 0 ? bgCol4B : bgBCol4B;
    bgp.bgFbm = idx == 0 ? bgFbm : bgBFbm;
    bgp.bgStars = idx == 0 ? bgStars : bgBStars;
    bgp.bgStarDensity = idx == 0 ? bgStarDensity : bgBStarDensity;
    bgp.bgStarSize = idx == 0 ? bgStarSize : bgBStarSize;
    bgp.bgGlow = idx == 0 ? bgGlow : bgBGlow;
    bgp.bgHueVar = idx == 0 ? bgHueVar : bgBHueVar;
    bgp.bgNebWarp = idx == 0 ? bgNebWarp : bgBNebWarp;
    bgp.bgNebContrast = idx == 0 ? bgNebContrast : bgBNebContrast;
    bgp.bgTwist = idx == 0 ? bgTwist : bgBTwist;
    bgp.bgHaze = idx == 0 ? bgHaze : bgBHaze;
    bgp.bgSparkle = idx == 0 ? bgSparkle : bgBSparkle;
    bgp.bgDisperse = idx == 0 ? bgDisperse : bgBDisperse;
    bgp.bgEmbers = idx == 0 ? bgEmbers : bgBEmbers;
    bgp.bgFlow = idx == 0 ? bgFlow : bgBFlow;
    bgp.bgCol5R = idx == 0 ? bgCol5R : bgBCol5R;
    bgp.bgCol5G = idx == 0 ? bgCol5G : bgBCol5G;
    bgp.bgCol5B = idx == 0 ? bgCol5B : bgBCol5B;
    bgp.bgCol6R = idx == 0 ? bgCol6R : bgBCol6R;
    bgp.bgCol6G = idx == 0 ? bgCol6G : bgBCol6G;
    bgp.bgCol6B = idx == 0 ? bgCol6B : bgBCol6B;
    bgp.bgEmberSize = idx == 0 ? bgEmberSize : bgBEmberSize;
    bgp.bgReflect = idx == 0 ? bgReflect : bgBReflect;
    bgp.bgMatDisp = idx == 0 ? bgMatDisp : bgBMatDisp;
    bgp.bgAniso = idx == 0 ? bgAniso : bgBAniso;
    bgp.bgEnvSharp = idx == 0 ? bgEnvSharp : bgBEnvSharp;
    bgp.bgEnvR = idx == 0 ? bgEnvR : bgBEnvR;
    bgp.bgEnvG = idx == 0 ? bgEnvG : bgBEnvG;
    bgp.bgEnvB = idx == 0 ? bgEnvB : bgBEnvB;
    bgp.bgClearcoat = idx == 0 ? bgClearcoat : bgBClearcoat;
    bgp.bgGradType = idx == 0 ? bgGradType : bgBGradType;
    bgp.bgPatMode = idx == 0 ? bgPatMode : bgBPatMode;
    bgp.bgPatStrength = idx == 0 ? bgPatStrength : bgBPatStrength;
    bgp.bgPatAngle = idx == 0 ? bgPatAngle : bgBPatAngle;
    bgp.univBase = idx == 0 ? univBase : bgBUnivBase;
    bgp.univNoise = idx == 0 ? univNoise : bgBUnivNoise;
    bgp.univPattern = idx == 0 ? univPattern : bgBUnivPattern;
    bgp.univBlend = idx == 0 ? univBlend : bgBUnivBlend;
    bgp.univNoiseAmt = idx == 0 ? univNoiseAmt : bgBUnivNoiseAmt;
    bgp.univNoiseScale = idx == 0 ? univNoiseScale : bgBUnivNoiseScale;
    bgp.univWarp = idx == 0 ? univWarp : bgBUnivWarp;
    bgp.univDetail = idx == 0 ? univDetail : bgBUnivDetail;
    bgp.univHorizon = idx == 0 ? univHorizon : bgBUnivHorizon;
    bgp.univGround = idx == 0 ? univGround : bgBUnivGround;
    bgp.univOrb = idx == 0 ? univOrb : bgBUnivOrb;
    bgp.univOrbX = idx == 0 ? univOrbX : bgBUnivOrbX;
    bgp.univOrbY = idx == 0 ? univOrbY : bgBUnivOrbY;
    bgp.univOrbSize = idx == 0 ? univOrbSize : bgBUnivOrbSize;
    bgp.univRidges = idx == 0 ? univRidges : bgBUnivRidges;
    bgp.univParticle = idx == 0 ? univParticle : bgBUnivParticle;
    bgp.univCaustic = idx == 0 ? univCaustic : bgBUnivCaustic;
    bgp.univShafts = idx == 0 ? univShafts : bgBUnivShafts;
    bgp.univPatBlend = idx == 0 ? univPatBlend : bgBUnivPatBlend;
    bgp.univPatStrength = idx == 0 ? univPatStrength : bgBUnivPatStrength;
    bgp.patColOverride = idx == 0 ? patColOverride : bgBPatColOverride;
    bgp.patColMode = idx == 0 ? patColMode : bgBPatColMode;
    bgp.patColR = idx == 0 ? patColR : bgBPatColR;
    bgp.patColG = idx == 0 ? patColG : bgBPatColG;
    bgp.patColB = idx == 0 ? patColB : bgBPatColB;
    bgp.patCol2R = idx == 0 ? patCol2R : bgBPatCol2R;
    bgp.patCol2G = idx == 0 ? patCol2G : bgBPatCol2G;
    bgp.patCol2B = idx == 0 ? patCol2B : bgBPatCol2B;
    bgp.patCol3R = idx == 0 ? patCol3R : bgBPatCol3R;
    bgp.patCol3G = idx == 0 ? patCol3G : bgBPatCol3G;
    bgp.patCol3B = idx == 0 ? patCol3B : bgBPatCol3B;
    bgp.patCol4R = idx == 0 ? patCol4R : bgBPatCol4R;
    bgp.patCol4G = idx == 0 ? patCol4G : bgBPatCol4G;
    bgp.patCol4B = idx == 0 ? patCol4B : bgBPatCol4B;
    bgp.patCol5R = idx == 0 ? patCol5R : bgBPatCol5R;
    bgp.patCol5G = idx == 0 ? patCol5G : bgBPatCol5G;
    bgp.patCol5B = idx == 0 ? patCol5B : bgBPatCol5B;
    bgp.patMat = idx == 0 ? patMat : bgBPatMat;
    bgp.patMatR = idx == 0 ? patMatR : bgBPatMatR;
    bgp.patMatG = idx == 0 ? patMatG : bgBPatMatG;
    bgp.patMatB = idx == 0 ? patMatB : bgBPatMatB;
    bgp.patMatTint = idx == 0 ? patMatTint : bgBPatMatTint;
    return bgp;
}

#define FG(i) fgField[(i)>>2][(i)&3]
BgParams MakeFg(int idx) {
    BgParams bgp; int o = idx * 111;
    bgp.bgTopR = FG(o + 0);
    bgp.bgTopG = FG(o + 1);
    bgp.bgTopB = FG(o + 2);
    bgp.bgBotR = FG(o + 3);
    bgp.bgBotG = FG(o + 4);
    bgp.bgBotB = FG(o + 5);
    bgp.bgStyle = (int)FG(o + 6);
    bgp.bgScale = FG(o + 7);
    bgp.bgAngle = FG(o + 8);
    bgp.bgGrain = FG(o + 9);
    bgp.bgWarp = (int)FG(o + 10);
    bgp.bgWarpAmt = FG(o + 11);
    bgp.bgWarpScale = FG(o + 12);
    bgp.bgOffX = FG(o + 13);
    bgp.bgOffY = FG(o + 14);
    bgp.bgScaleY = FG(o + 15);
    bgp.bgSharp = FG(o + 16);
    bgp.bgWarpX = FG(o + 17);
    bgp.bgWarpY = FG(o + 18);
    bgp.bgWarpAmt2 = FG(o + 19);
    bgp.bgWarpScale2 = FG(o + 20);
    bgp.bgMidR = FG(o + 21);
    bgp.bgMidG = FG(o + 22);
    bgp.bgMidB = FG(o + 23);
    bgp.bgMetallic = FG(o + 24);
    bgp.bgRoughness = FG(o + 25);
    bgp.bgSpecular = FG(o + 26);
    bgp.bgNormal = FG(o + 27);
    bgp.bgFresnel = FG(o + 28);
    bgp.bgLightX = FG(o + 29);
    bgp.bgLightY = FG(o + 30);
    bgp.bgLightZ = FG(o + 31);
    bgp.bgLightInt = FG(o + 32);
    bgp.bgCol4R = FG(o + 33);
    bgp.bgCol4G = FG(o + 34);
    bgp.bgCol4B = FG(o + 35);
    bgp.bgFbm = FG(o + 36);
    bgp.bgStars = FG(o + 37);
    bgp.bgStarDensity = FG(o + 38);
    bgp.bgStarSize = FG(o + 39);
    bgp.bgGlow = FG(o + 40);
    bgp.bgHueVar = FG(o + 41);
    bgp.bgNebWarp = FG(o + 42);
    bgp.bgNebContrast = FG(o + 43);
    bgp.bgTwist = FG(o + 44);
    bgp.bgHaze = FG(o + 45);
    bgp.bgSparkle = FG(o + 46);
    bgp.bgDisperse = FG(o + 47);
    bgp.bgEmbers = FG(o + 48);
    bgp.bgFlow = FG(o + 49);
    bgp.bgCol5R = FG(o + 50);
    bgp.bgCol5G = FG(o + 51);
    bgp.bgCol5B = FG(o + 52);
    bgp.bgCol6R = FG(o + 53);
    bgp.bgCol6G = FG(o + 54);
    bgp.bgCol6B = FG(o + 55);
    bgp.bgEmberSize = FG(o + 56);
    bgp.bgReflect = FG(o + 57);
    bgp.bgMatDisp = FG(o + 58);
    bgp.bgAniso = FG(o + 59);
    bgp.bgEnvSharp = FG(o + 60);
    bgp.bgEnvR = FG(o + 61);
    bgp.bgEnvG = FG(o + 62);
    bgp.bgEnvB = FG(o + 63);
    bgp.bgClearcoat = FG(o + 64);
    bgp.bgGradType = (int)FG(o + 65);
    bgp.bgPatMode = (int)FG(o + 66);
    bgp.bgPatStrength = FG(o + 67);
    bgp.bgPatAngle = FG(o + 68);
    bgp.univBase = (int)FG(o + 69);
    bgp.univNoise = (int)FG(o + 70);
    bgp.univPattern = (int)FG(o + 71);
    bgp.univBlend = (int)FG(o + 72);
    bgp.univNoiseAmt = FG(o + 73);
    bgp.univNoiseScale = FG(o + 74);
    bgp.univWarp = FG(o + 75);
    bgp.univDetail = FG(o + 76);
    bgp.univHorizon = FG(o + 77);
    bgp.univGround = (int)FG(o + 78);
    bgp.univOrb = (int)FG(o + 79);
    bgp.univOrbX = FG(o + 80);
    bgp.univOrbY = FG(o + 81);
    bgp.univOrbSize = FG(o + 82);
    bgp.univRidges = FG(o + 83);
    bgp.univParticle = (int)FG(o + 84);
    bgp.univCaustic = FG(o + 85);
    bgp.univShafts = FG(o + 86);
    bgp.univPatBlend = (int)FG(o + 87);
    bgp.univPatStrength = FG(o + 88);
    bgp.patColOverride = (int)FG(o + 89);
    bgp.patColMode = (int)FG(o + 90);
    bgp.patColR = FG(o + 91);
    bgp.patColG = FG(o + 92);
    bgp.patColB = FG(o + 93);
    bgp.patCol2R = FG(o + 94);
    bgp.patCol2G = FG(o + 95);
    bgp.patCol2B = FG(o + 96);
    bgp.patCol3R = FG(o + 97);
    bgp.patCol3G = FG(o + 98);
    bgp.patCol3B = FG(o + 99);
    bgp.patCol4R = FG(o + 100);
    bgp.patCol4G = FG(o + 101);
    bgp.patCol4B = FG(o + 102);
    bgp.patCol5R = FG(o + 103);
    bgp.patCol5G = FG(o + 104);
    bgp.patCol5B = FG(o + 105);
    bgp.patMat = (int)FG(o + 106);
    bgp.patMatR = FG(o + 107);
    bgp.patMatG = FG(o + 108);
    bgp.patMatB = FG(o + 109);
    bgp.patMatTint = FG(o + 110);
    return bgp;
}

float SeamWeight(float2 uv, float lin, float asp) {
    if (blendMode == 3) return 1.0;
    float s;
    if (blendMode == 1) {
        float2 d = uv - float2(blendCx, blendCy);
        d.x *= asp;
        d.y *= max(blendEllipse, 0.01);
        s = length(d) - max(blendRadius, 0.0);
    } else if (blendMode == 2) {
        s = lin - blendDepthSplit;
    } else {
        float2 d = uv - 0.5;
        d.x *= asp;
        s = dot(d, float2(cos(blendAngle), sin(blendAngle))) - blendOffset;
    }
    if (blendNoiseAmt > 0.0)
        s += (Fbm(uv * max(blendNoiseScale, 0.01), 3) - 0.5) * blendNoiseAmt;
    if (blendDepthBend != 0.0)
        s += (lin - blendDepthRef) * blendDepthBend;
    float f = max(blendFeather, 1e-4);
    return smoothstep(-f, f, s);
}

float SeamWeightFg(float2 uv, float lin, float asp) {
    if (fgSeamMode == 3) return 1.0;
    float s;
    if (fgSeamMode == 1) {
        float2 d = uv - float2(fgSeamCx, fgSeamCy);
        d.x *= asp;
        d.y *= max(fgSeamEllipse, 0.01);
        s = length(d) - max(fgSeamRadius, 0.0);
    } else if (fgSeamMode == 2) {
        s = lin - fgSeamDepthSplit;
    } else {
        float2 d = uv - 0.5;
        d.x *= asp;
        s = dot(d, float2(cos(fgSeamAngle), sin(fgSeamAngle))) - fgSeamOffset;
    }
    if (fgSeamNoiseAmt > 0.0)
        s += (Fbm(uv * max(fgSeamNoiseScale, 0.01), 3) - 0.5) * fgSeamNoiseAmt;
    if (fgSeamDepthBend != 0.0)
        s += (lin - fgSeamDepthRef) * fgSeamDepthBend;
    float f = max(fgSeamFeather, 1e-4);
    return smoothstep(-f, f, s);
}

BgResult EvalBackdrop(float2 baseUv, BgParams bgp, float asp) {
    float3 col1 = float3(bgp.bgTopR, bgp.bgTopG, bgp.bgTopB);
    float3 col2 = float3(bgp.bgBotR, bgp.bgBotG, bgp.bgBotB);

    float2 uv = baseUv;
    float2 cen = float2(bgp.bgWarpX, bgp.bgWarpY);
    if ((bgp.bgWarp & 1) != 0) { float2 u = uv - cen + 0.5; u = WarpSwirl(u, bgp.bgWarpAmt * 3.14159, 0.9, asp); uv = u + cen - 0.5; }
    if ((bgp.bgWarp & 2) != 0) { float2 u = uv - cen + 0.5; u = WarpFisheye(u, bgp.bgWarpAmt * 2.0, 1.0, asp); uv = u + cen - 0.5; }
    if ((bgp.bgWarp & 4) != 0) { float2 u = uv - cen + 0.5; u = WarpKaleido(u, max(bgp.bgWarpScale, 2.0), bgp.bgAngle, asp); uv = u + cen - 0.5; }
    if ((bgp.bgWarp & 8) != 0) {
        uv.x += sin(uv.y * bgp.bgWarpScale2 * 6.2831853) * bgp.bgWarpAmt2 * 0.1;
        uv.y += cos(uv.x * bgp.bgWarpScale2 * 6.2831853) * bgp.bgWarpAmt2 * 0.1;
    }
    if ((bgp.bgWarp & 16) != 0) {
        float2 rp = uv - cen; rp.x *= asp;
        float rd = length(rp);
        uv += (rp / max(rd, 1e-3)) * sin(rd * bgp.bgWarpScale2 * 20.0) * bgp.bgWarpAmt2 * 0.05;
    }

    if (bgp.bgTwist != 0.0 && bgp.bgStyle != 16) {
        float2 d = uv - 0.5; d.x *= asp;
        float ang = bgp.bgTwist * 3.0 * saturate(1.0 - length(d));
        float sa = sin(ang), ca = cos(ang);
        d = float2(d.x * ca - d.y * sa, d.x * sa + d.y * ca);
        d.x /= asp; uv = d + 0.5;
    }
    float3 col3 = float3(bgp.bgMidR, bgp.bgMidG, bgp.bgMidB);
    uv -= float2(bgp.bgOffX, bgp.bgOffY);
    float2 wsuv = float2(uv.x, 1.0 - uv.y);
    float2 sc = float2(max(bgp.bgScale, 1.0), max(bgp.bgScaleY, 1.0));
    float scx = sc.x;
    float2 pc = uv - 0.5; pc.x *= asp;
    float3 pat = col1;
    float t = -1.0;
    if (bgp.bgStyle == 2) t = wsuv.y;
    else if (bgp.bgStyle == 3) t = length(pc) * 1.6;
    else if (bgp.bgStyle == 4) t = wsuv.x;
    else if (bgp.bgStyle == 5) {
        float ph = (uv.x * cos(bgp.bgAngle) + uv.y * sin(bgp.bgAngle)) * scx;
        pat = Band3(ph, col1, col3, col2);
    }
    else if (bgp.bgStyle == 6) {
        float2 cell = uv * sc;
        float3 sq = lerp(col1, col2, fmod(floor(cell.x) + floor(cell.y), 2.0));
        float2 gg = abs(frac(cell) - 0.5);
        pat = lerp(sq, col3, smoothstep(0.45, 0.5, max(gg.x, gg.y)));
    }
    else if (bgp.bgStyle == 7) {
        float d = length(frac(uv * sc) - 0.5);
        float fill = 1.0 - smoothstep(0.24, 0.30, d);
        float ring = smoothstep(0.24, 0.30, d) - smoothstep(0.34, 0.40, d);
        pat = lerp(lerp(col1, col2, fill), col3, saturate(ring));
    }
    else if (bgp.bgStyle == 8) t = (wsuv.x + wsuv.y) * 0.5;
    else if (bgp.bgStyle == 9) {
        float ph = (atan2(pc.y, pc.x) * scx + length(pc) * scx * 6.2831853 + bgp.bgAngle) / 6.2831853;
        pat = Band3(ph, col1, col3, col2);
    }
    else if (bgp.bgStyle == 10) {
        float2 gg = abs(frac(uv * sc) - 0.5);
        float lx = smoothstep(0.42, 0.48, gg.x), ly = smoothstep(0.42, 0.48, gg.y);
        pat = lerp(lerp(col1, col2, max(lx, ly)), col3, lx * ly);
    }
    else if (bgp.bgStyle == 11) {
        float ph = (atan2(pc.y, pc.x) - bgp.bgAngle) * scx / 6.2831853;
        pat = Band3(ph, col1, col3, col2);
    }
    else if (bgp.bgStyle == 12) t = 0.5 + 0.5 * sin(length(pc) * scx * 6.2831853 - bgp.bgAngle);
    else if (bgp.bgStyle == 13) t = VNoise(uv * sc);
    else if (bgp.bgStyle == 14) {
        int oc = (int)clamp(bgp.bgFbm, 1.0, 6.0);
        float tA = time * animSpeed;
        float2 np = uv * sc * 0.5;
        np.y *= (1.0 + bgp.bgFlow * 3.0);
        np.y -= tA * 0.30;
        if (bgp.bgNebWarp > 0.0) {
            float2 q = float2(Fbm(np + 1.3 + tA * 0.15, oc), Fbm(np + 7.8, oc));
            np += (q - 0.5) * bgp.bgNebWarp * 3.0;
        }
        float n = saturate((Fbm(np, oc) - 0.15) * 1.7);
        n = saturate(n + RidgedFbm(np * 2.4 + 5.0, oc) * n * 0.35);
        if (bgp.bgNebContrast > 0.0)
            n = pow(saturate((n - 0.5) * (1.0 + bgp.bgNebContrast * 3.0) + 0.5), 1.0 + bgp.bgNebContrast * 2.0);
        t = n;
    }
    else if (bgp.bgStyle == 15) pat = col1;
    else if (bgp.bgStyle == 16) {
        int oc = (int)clamp(bgp.bgFbm, 1.0, 6.0);
        float2 p = uv * sc;
        if (bgp.bgTwist != 0.0) {
            float2 d = uv - 0.5; d.x *= asp;
            float ang = bgp.bgTwist * 2.0 * length(d);
            float s = sin(ang), co = cos(ang);
            p = float2(p.x * co - p.y * s, p.x * s + p.y * co);
        }
        p.y *= (1.0 + bgp.bgFlow * 2.0);
        float2 w = float2(Fbm(p + 1.3, oc), Fbm(p + 7.8, oc));
        p += (w - 0.5) * (1.0 + bgp.bgNebWarp * 3.0);
        float veins = RidgedFbm(p, oc);
        float body = Fbm(p * 0.7, oc);
        float n = saturate(body * 0.4 + veins * 0.85);
        n = pow(n, 1.6 + bgp.bgNebContrast * 3.5);
        float2 e = (uv - 0.5); e.x *= asp;
        n *= 1.0 - 0.55 * smoothstep(0.35, 1.05, length(e));
        t = saturate(n * 1.4);
    }
    else if (bgp.bgStyle == 17) t = wsuv.y;
    else if (bgp.bgStyle == 18) {
        int oc = (int)clamp(bgp.bgFbm, 1.0, 6.0);
        float3 c5 = float3(bgp.bgCol5R, bgp.bgCol5G, bgp.bgCol5B);
        float3 c6 = float3(bgp.bgCol6R, bgp.bgCol6G, bgp.bgCol6B);
        float sy = wsuv.y;
        pat = lerp(col1 * 0.3, col1 * 1.2, saturate(sy));
        float wv = 0.4 + bgp.bgNebWarp;
        float pf = 24.0 + max(bgp.bgScale, 1.0) * 7.0;
        float3 aur = float3(0.0, 0.0, 0.0);
        [unroll] for (int L = 0; L < 2; L++) {
            float fl = L;
            float2 vpt = float2(0.5, -0.55 - fl * 0.25);
            float ang = (uv.x - vpt.x) / (sy - vpt.y);
            float edge = 0.5 - fl * 0.08
                       + sin(ang * 3.0 + fl * 2.0 + bgp.bgTwist * 3.0) * 0.06 * wv
                       + (Fbm(float2(ang * 2.0 + fl * 5.0, 0.7), oc) - 0.5) * 0.18 * wv;
            float up = edge - sy;
            float gate = smoothstep(0.0, 0.02, up);
            float vfade = gate * exp(-max(up, 0.0) * (2.2 + fl * 1.2));
            float pill = pow(0.5 + 0.5 * sin(ang * pf + Fbm(float2(ang * 10.0, fl), 2) * 8.0), 2.2);
            float pbright = 0.2 + 0.8 * Fbm(float2(ang * 3.0 + fl * 7.0, 0.3), oc);
            float ah = saturate(up * 2.2);
            float3 acol = (ah < 0.34) ? lerp(c5, col3, ah / 0.34)
                        : (ah < 0.67) ? lerp(col3, c6, (ah - 0.34) / 0.33)
                                      : lerp(c6, col2, (ah - 0.67) / 0.33);
            aur += acol * vfade * pbright * (0.28 + 0.72 * pill) * (1.0 - fl * 0.35);
        }
        aur *= (1.0 - smoothstep(0.82, 1.0, sy));
        if (bgp.bgHueVar > 0.0) aur = HueShift(aur, (Fbm(uv * 3.0 + 5.0, 3) - 0.5) * bgp.bgHueVar * 0.25);
        pat += aur * 1.25;
        pat += c5 * smoothstep(0.78, 1.0, sy) * 0.05;
    }
    else if (bgp.bgStyle == 19) {
        float3 c5 = float3(bgp.bgCol5R, bgp.bgCol5G, bgp.bgCol5B);
        float3 c6 = float3(bgp.bgCol6R, bgp.bgCol6G, bgp.bgCol6B);
        float3 neon = float3(bgp.bgCol4R, bgp.bgCol4G, bgp.bgCol4B);
        float sy = wsuv.y;
        float horizon = clamp(0.55 + bgp.bgOffY, 0.2, 0.85);
        float gl = 0.6 + bgp.bgGlow * 2.0;

        float skyT = saturate(sy / max(horizon, 0.05));
        pat = Ramp5(skyT, col1, c5, col3, c6, col2);

        float sunR = 0.10 + saturate(bgp.bgScaleY / 20.0) * 0.16;
        float2 sp = float2((uv.x - 0.5) * asp, sy - horizon);
        float sdist = length(sp) / sunR;
        float yy = sp.y / sunR;
        float3 sunCol = lerp(float3(1.0, 0.9, 0.4), col2, saturate(yy * 0.5 + 0.5));
        float vis = 1.0;
        if (yy > -0.15) {
            float bandY = saturate((yy + 0.15) / 1.15);
            vis = step(frac(bandY * 9.0), 1.0 - bandY * 0.85);
        }
        float sunMask = smoothstep(1.0, 0.97, sdist) * vis;
        pat = lerp(pat, sunCol, sunMask);
        pat += lerp(float3(1.0, 0.5, 0.5), col2, 0.5) * exp(-pow(max(sdist - 1.0, 0.0) * 3.5, 2.0)) * gl * 0.25;

        if (sy > horizon) {
            float fy = max(sy - horizon, 1e-4);
            float z = 0.16 / fy;
            float gd = max(bgp.bgScale, 1.0);
            float hc = z * gd * 0.5;
            float hgw = fwidth(hc);
            float hg = abs(frac(hc + 0.5) - 0.5);
            float hLine = (1.0 - smoothstep(0.0, hgw * 1.3, hg))
                        + (1.0 - smoothstep(0.0, hgw * 7.0, hg)) * 0.4;
            hLine *= 1.0 - smoothstep(0.35, 0.6, hgw);
            float vc = (uv.x - 0.5) * z * gd * 2.0;
            float vgw = fwidth(vc);
            float vg = abs(frac(vc + 0.5) - 0.5);
            float vLine = (1.0 - smoothstep(0.0, vgw * 1.3, vg))
                        + (1.0 - smoothstep(0.0, vgw * 7.0, vg)) * 0.4;
            float grid = saturate(max(hLine, vLine)) * smoothstep(0.0, 0.06, fy);
            float3 floorCol = col1 * 0.18;
            pat = floorCol + neon * grid * gl * (0.5 + 0.5 * saturate(fy * 2.5));
        }
        pat += neon * exp(-pow((sy - horizon) * 60.0, 2.0)) * gl * 0.5;
    }
    else if (bgp.bgStyle == 20) {
        int oc = (int)clamp(bgp.bgFbm, 1.0, 6.0);
        float3 c5 = float3(bgp.bgCol5R, bgp.bgCol5G, bgp.bgCol5B);
        float3 c6 = float3(bgp.bgCol6R, bgp.bgCol6G, bgp.bgCol6B);
        float3 moonC = float3(bgp.bgCol4R, bgp.bgCol4G, bgp.bgCol4B);
        float sy = wsuv.y;
        pat = Ramp5(saturate(sy), col1, c5, col3, c6, col2);

        float2 mc = float2(0.5 + bgp.bgOffX, 0.52 + bgp.bgOffY);
        float mr = 0.14 + saturate(bgp.bgScaleY / 20.0) * 0.18;
        float2 mp = float2((uv.x - mc.x) * asp, sy - mc.y);
        float md = length(mp) / mr;
        float halo = exp(-pow(max(md - 1.0, 0.0) * 2.2, 2.0));
        pat += moonC * halo * (0.25 + bgp.bgGlow * 0.55);
        float surf = 0.62 + 0.38 * Fbm(mp * (7.0 / mr) + 3.0, oc);
        float3 moon = moonC * surf * (1.0 - smoothstep(0.55, 1.0, md) * 0.45);
        pat = lerp(pat, moon, smoothstep(1.0, 0.965, md));

        float2 cp = float2(uv.x * (2.5 + bgp.bgScale * 0.25) + bgp.bgTwist * 2.0, sy * 6.0);
        float2 cw = float2(Fbm(cp + 1.3, oc), Fbm(cp + 7.8, oc));
        cp += (cw - 0.5) * (1.0 + bgp.bgNebWarp * 2.5);
        float cloud = smoothstep(0.42, 0.72, Fbm(cp, oc)) * saturate(0.4 + bgp.bgNebContrast);
        pat = lerp(pat, col1 * 0.25, cloud * 0.85);
        pat += col2 * smoothstep(0.78, 1.0, sy) * 0.12;
    }
    else if (bgp.bgStyle == 21) {
        int oc = (int)clamp(bgp.bgFbm, 1.0, 6.0);
        float3 c5 = float3(bgp.bgCol5R, bgp.bgCol5G, bgp.bgCol5B);
        float3 c6 = float3(bgp.bgCol6R, bgp.bgCol6G, bgp.bgCol6B);
        float3 iris = float3(bgp.bgCol4R, bgp.bgCol4G, bgp.bgCol4B);
        float3 teal = float3(0.15, 0.84, 0.73);
        float sy = wsuv.y;
        float hy = clamp(groundLevel, 0.2, 0.95);
        pat = Ramp5(saturate(sy), col1, c5, col3, c6, col2);

        float2 np = float2(uv.x * 2.2, sy * 2.8) + 4.0;
        np += (float2(Fbm(np + 2.3, oc), Fbm(np + 9.1, oc)) - 0.5) * 0.9;
        pat += col3 * Fbm(np * 0.6, oc) * 0.09;
        pat += iris * pow(RidgedFbm(np, oc), 2.6) * 0.07 * smoothstep(0.2, 0.95, sy);

        float2 rp0 = float2(uv.x * 2.2 + bgp.bgTwist, sy * 1.5 - bgp.bgFlow);
        rp0 += (float2(Fbm(rp0 + 1.1, oc), Fbm(rp0 * 1.3 + 5.5, oc)) - 0.5) * 1.4;
        float ribbon = pow(0.5 + 0.5 * sin(rp0.x * 2.4 + rp0.y * 1.3), 2.5);
        float aeth = ribbon * smoothstep(0.25, 0.95, sy) * (0.4 + 0.6 * Fbm(rp0, oc));
        pat += teal * aeth * (0.06 + 0.09 * bgp.bgGlow);

        float2 mc = float2(0.5 + bgp.bgOffX, 0.42 + bgp.bgOffY);
        float mr = 0.14 + saturate(bgp.bgScaleY / 20.0) * 0.18;
        float2 mp = float2((uv.x - mc.x) * asp, sy - mc.y);
        float md = length(mp) / mr;
        float ang = atan2(mp.y, mp.x);
        pat += iris * exp(-pow(max(md - 1.0, 0.0) * 2.2, 2.0)) * (0.22 + bgp.bgGlow * 0.5);
        pat += teal * exp(-pow((md - 1.0) * 6.0, 2.0)) * (0.7 + bgp.bgGlow) * 1.1;

        float fib = 0.5 + 0.5 * sin(ang * 58.0 + Fbm(float2(ang * 6.0, md * 4.0), oc) * 9.0);
        fib = lerp(0.55, fib, smoothstep(0.30, 1.0, md));
        float mott = 0.6 + 0.4 * Fbm(mp * (6.5 / mr) + 3.0, oc);
        float3 irisCol = iris * mott * (0.5 + 0.5 * fib);
        irisCol += iris * 0.32 * exp(-pow((md - 0.5) * 8.0, 2.0));
        irisCol *= 1.0 - smoothstep(0.7, 0.98, md) * 0.35;
        float pupil = smoothstep(0.25, 0.30, md);
        irisCol *= 0.12 + 0.88 * pupil;
        float2 cl = mp - float2(-0.34, -0.34) * mr;
        irisCol += teal * exp(-pow(length(cl) / (mr * 0.11), 2.0)) * (0.85 + bgp.bgGlow);
        float ringTex = 0.72 + 0.28 * Fbm(float2(ang * 4.0, 1.0), oc);
        irisCol = lerp(irisCol, teal * ringTex, smoothstep(0.82, 0.97, md));
        pat = lerp(pat, irisCol, smoothstep(1.0, 0.965, md));

        float aboveFloor = 1.0 - smoothstep(hy - 0.05, hy + 0.05, sy);
        float2 cpF = float2(uv.x * 1.3 + bgp.bgTwist * 0.6, sy * 3.0);
        float cloudF = smoothstep(0.5, 0.86, Fbm(cpF + Fbm(cpF, oc), oc));
        pat = lerp(pat, col1 * 0.55, cloudF * 0.28 * aboveFloor);
        float2 cp = float2(uv.x * (2.5 + bgp.bgScale * 0.25) + bgp.bgTwist * 2.0 + time * animSpeed * 0.4, sy * 6.0);
        float2 cw = float2(Fbm(cp + 1.3, oc), Fbm(cp + 7.8, oc));
        cp += (cw - 0.5) * (1.0 + bgp.bgNebWarp * 2.5);
        float cloud = smoothstep(0.44, 0.74, Fbm(cp, oc)) * saturate(0.35 + bgp.bgNebContrast);
        pat = lerp(pat, col1 * 0.20, cloud * 0.85 * aboveFloor);

        float g = smoothstep(0.0, 0.04, sy - hy);
        if (g > 0.0) {
            float depth = saturate((sy - hy) / max(1.0 - hy, 0.05));
            float ripple = (Fbm(float2(uv.x * 7.0, depth * 5.0 + bgp.bgTwist), oc) - 0.5) * 0.045 * (0.3 + depth);
            float rsy = 2.0 * hy - sy;
            float2 rmp = float2((uv.x - mc.x) * asp + ripple, rsy - mc.y);
            float rmd = length(rmp) / mr;
            float3 refl = iris * exp(-pow(max(rmd - 1.0, 0.0) * 2.6, 2.0)) * 0.5;
            refl += teal * exp(-pow((rmd - 1.0) * 6.5, 2.0)) * 0.6;
            refl += iris * (0.3 + 0.4 * Fbm(rmp * (6.0 / mr), oc)) * smoothstep(1.0, 0.9, rmd) * 0.5;
            refl *= 1.0 - depth * 0.65;
            float grain = Fbm(float2(uv.x / (depth * 1.8 + 0.15), depth * 5.0), oc);
            float3 floorCol = col1 * 0.30 + refl;
            floorCol += teal * (0.05 + 0.10 * grain) * (1.0 - depth * 0.4);
            floorCol += teal * exp(-pow((sy - hy) * 40.0, 2.0)) * 0.35;
            pat = lerp(pat, floorCol, g);
        }

        float2 vgp = uv - 0.5; vgp.x *= asp;
        pat *= 1.0 - smoothstep(0.42, 1.05, length(vgp)) * 0.72;
    }
    else if (bgp.bgStyle == 22) {
        int oc = (int)clamp(bgp.bgFbm, 1.0, 6.0);
        float3 c5 = float3(bgp.bgCol5R, bgp.bgCol5G, bgp.bgCol5B);
        float3 c6 = float3(bgp.bgCol6R, bgp.bgCol6G, bgp.bgCol6B);
        float3 spark = float3(bgp.bgCol4R, bgp.bgCol4G, bgp.bgCol4B);
        float3 hot = float3(1.0, 0.72, 0.32);
        float3 whiteHot = float3(1.0, 0.95, 0.82);
        float3 craft = float3(0.40, 0.85, 1.0);
        float sy = wsuv.y;
        float mline = clamp(groundLevel, 0.3, 0.95);
        float gl = 0.6 + bgp.bgGlow;

        pat = Ramp5(saturate(sy), col1, c5, col3, c6, col2);

        float shim = sin(sy * 40.0 + uv.x * 20.0) * 0.004 * bgp.bgHaze * smoothstep(0.2, 1.0, sy);
        float2 uvh = float2(uv.x + shim, sy);

        float2 fc = float2(0.5 + bgp.bgOffX, 0.5 + bgp.bgOffY);
        float2 fp = float2((uvh.x - fc.x) * asp, sy - fc.y);
        float fd = length(fp);
        float core = exp(-fd * fd * (13.0 - saturate(bgp.bgScaleY / 12.0) * 5.0));
        float2 flp = float2(uvh.x * asp * 4.0, sy * 4.5 - bgp.bgFlow * 2.0);
        flp += (float2(Fbm(flp + 1.0, oc), Fbm(flp + 8.0, oc)) - 0.5) * (1.0 + bgp.bgNebWarp * 2.5);
        float flame = pow(RidgedFbm(flp, oc), 2.5) * smoothstep(fc.y + 0.35, fc.y - 0.25, sy) * smoothstep(0.62, 0.1, fd);
        pat += lerp(hot, whiteHot, core * core) * core * (0.5 + bgp.bgGlow * 0.5);
        pat += hot * flame * (0.4 + bgp.bgGlow * 0.4);

        if (sy > mline) {
            float mdep = (sy - mline) / max(1.0 - mline, 0.05);
            float2 lp = float2(uvh.x * asp * 3.0, (sy - mline) * 5.0 - bgp.bgFlow * 1.5);
            lp += (Fbm(lp, oc) - 0.5) * 0.6;
            float cracks = pow(RidgedFbm(lp * 1.4, oc), 3.0);
            float crust = smoothstep(0.3, 0.8, Fbm(lp * 0.8 + 3.0, oc));
            float3 molten = lerp(col3 * 0.6, col1 * 0.3, crust);
            molten += lerp(hot, whiteHot, cracks) * cracks * (0.8 + bgp.bgGlow * 0.6) * (0.5 + 0.5 * mdep);
            pat = lerp(pat, molten, smoothstep(0.0, 0.03, sy - mline));
            pat += hot * exp(-pow((sy - mline) * 30.0, 2.0)) * (0.3 + bgp.bgGlow * 0.3);
        }

        if (bgp.bgEmbers > 0.0) {
            float2 sp2 = float2(uvh.x * asp, sy) * (14.0 + bgp.bgScale * 2.0);
            sp2.y += bgp.bgFlow * 6.0 + time * animSpeed * 3.0 + bgp.bgTwist * sp2.x * 0.1;
            float2 sc2 = floor(sp2);
            float sh = Hash21(sc2 + 3.3);
            if (sh > 0.78) {
                float2 jit = 0.3 + 0.4 * float2(Hash21(sc2 + 1.7), Hash21(sc2 + 9.1));
                float sd2 = dot(frac(sp2) - jit, frac(sp2) - jit);
                float tw = 0.5 + 0.5 * sin(sh * 60.0 + sp2.y * 3.0);
                float near = smoothstep(0.7, 0.0, fd);
                pat += spark * exp(-sd2 * lerp(80.0, 26.0, saturate(bgp.bgEmberSize))) * frac(sh * 37.0) * tw * bgp.bgEmbers * (0.4 + near) * 1.4;
            }
        }

        float2 ap = float2(uvh.x * asp, sy) * 8.0;
        ap.y += bgp.bgFlow * 1.5;
        float2 acl = floor(ap);
        float ah = Hash21(acl + 44.1);
        if (ah > 0.86) {
            float2 jit = 0.3 + 0.4 * float2(Hash21(acl + 2.2), Hash21(acl + 7.7));
            float ad2 = dot(frac(ap) - jit, frac(ap) - jit);
            float sel = frac(ah * 13.0);
            float3 cc = sel < 0.14 ? float3(1.00, 0.80, 0.30)
                      : sel < 0.28 ? float3(0.55, 0.75, 1.00)
                      : sel < 0.42 ? float3(0.45, 0.90, 0.50)
                      : sel < 0.56 ? float3(0.96, 0.92, 0.85)
                      : sel < 0.70 ? float3(0.80, 0.45, 0.22)
                      : sel < 0.84 ? float3(0.70, 0.85, 0.45)
                      :              craft;
            float tw = 0.5 + 0.5 * sin(ah * 80.0 + sy * 20.0);
            pat += cc * exp(-ad2 * 26.0) * tw * (0.5 + bgp.bgGlow * 0.4);
        }

        float2 vgp = uv - 0.5; vgp.x *= asp;
        pat *= 1.0 - smoothstep(0.5, 1.15, length(vgp)) * 0.55;
    }
    else if (bgp.bgStyle == 23) {
        int oc = (int)clamp(bgp.bgFbm, 1.0, 6.0);
        float3 c5 = float3(bgp.bgCol5R, bgp.bgCol5G, bgp.bgCol5B);
        float3 c6 = float3(bgp.bgCol6R, bgp.bgCol6G, bgp.bgCol6B);
        float3 cglow = float3(bgp.bgCol4R, bgp.bgCol4G, bgp.bgCol4B);
        float sy = wsuv.y;

        pat = Ramp5(saturate(sy), col1, c5, col3, c6, col2);
        float2 gp = float2((uv.x - (0.5 + bgp.bgOffX)) * asp, sy - (0.55 + bgp.bgOffY));
        pat += cglow * exp(-dot(gp, gp) * 2.2) * (0.14 + bgp.bgGlow * 0.22);
        float2 hp = float2(uv.x * 2.0 + bgp.bgTwist, sy * 1.4);
        pat += cglow * Fbm(hp + Fbm(hp, oc), oc) * 0.05;

        [loop] for (int bl = 0; bl < 2; bl++) {
            float far = (bl == 0) ? 1.0 : 0.0;
            float scl = lerp(6.0, 3.0, far) + bgp.bgScale * lerp(0.6, 0.3, far);
            float2 bp = float2(uv.x * asp, sy) * scl + float2(bgp.bgTwist, -bgp.bgFlow - time * animSpeed * 0.5) * (far > 0.5 ? 0.4 : 0.9);
            float op = lerp(0.5, 0.32, far);
            [unroll] for (int oy = -1; oy <= 1; oy++)
            [loop] for (int ox = -1; ox <= 1; ox++) {
                float2 cell = floor(bp) + float2(ox, oy);
                float h = Hash21(cell + bl * 23.7);
                if (h > 0.56) {
                    float2 ctr = cell + float2(Hash21(cell + 1.7), Hash21(cell + 9.3));
                    float dist = length(bp - ctr);
                    float rad = lerp(0.16, 0.44, frac(h * 17.0)) * lerp(1.5, 1.0, far);
                    float3 cc = CraftHue(frac(h * 8.0 + bl * 0.37));
                    float disc = smoothstep(rad, rad * 0.55, dist);
                    float edge = exp(-pow((dist - rad * 0.82) * (5.0 / rad), 2.0)) * 0.6;
                    pat += cc * (disc * 0.4 + edge * 0.35) * op * (0.6 + bgp.bgGlow * 0.5);
                }
            }
        }

        float2 sp = float2(uv.x * asp, sy) * (26.0 + bgp.bgScale) + float2(bgp.bgTwist, -bgp.bgFlow) * 1.5;
        float2 scl2 = floor(sp);
        float sh = Hash21(scl2 + 71.3);
        if (sh > 0.93) {
            float2 d = frac(sp) - 0.5;
            float tw = 0.5 + 0.5 * sin(sh * 90.0 + sy * 30.0);
            pat += CraftHue(frac(sh * 8.0 + 0.5)) * exp(-dot(d, d) * 40.0) * tw * 0.5;
        }

        float2 vgp = uv - 0.5; vgp.x *= asp;
        pat *= 1.0 - smoothstep(0.62, 1.2, length(vgp)) * 0.35;
    }
    else if (bgp.bgStyle == 24) {
        int oc = (int)clamp(bgp.bgFbm, 1.0, 6.0);
        float3 c5 = float3(bgp.bgCol5R, bgp.bgCol5G, bgp.bgCol5B);
        float3 c6 = float3(bgp.bgCol6R, bgp.bgCol6G, bgp.bgCol6B);
        float3 sunC = float3(bgp.bgCol4R, bgp.bgCol4G, bgp.bgCol4B);
        float3 sunHot = float3(1.0, 0.97, 0.88);
        float sy = wsuv.y;
        float hz = clamp(0.45 + saturate(bgp.bgScaleY / 16.0) * 0.30, 0.30, 0.85);
        float sunx = 0.5 + bgp.bgOffX;
        float sunR = 0.13;

        pat = Ramp5(saturate(sy / max(hz, 0.05)), col1, c5, col3, c6, col2);

        float inSky = smoothstep(hz, hz - 0.03, sy);
        float2 clp = float2(uv.x * (1.6 + bgp.bgScale * 0.15) + bgp.bgTwist, sy * 7.0);
        clp += (Fbm(clp, oc) - 0.5) * (1.0 + bgp.bgNebWarp * 2.0);
        float cl = smoothstep(0.5, 0.78, Fbm(clp, oc)) * saturate(0.4 + bgp.bgNebContrast) * inSky;
        float3 cloudCol = lerp(col3 * 0.5, c6 * 1.25, smoothstep(hz - 0.3, hz, sy));
        pat = lerp(pat, cloudCol, cl);

        float2 sp = float2((uv.x - sunx) * asp, sy - hz);
        float sd = length(sp) / sunR;
        pat += sunC * exp(-sd * sd * 1.8) * (0.5 + bgp.bgGlow * 0.8);
        if (sy <= hz) pat = lerp(pat, lerp(sunHot, sunC, saturate(sd * 0.9)), smoothstep(1.0, 0.92, sd));

        if (sy > hz) {
            float depth = saturate((sy - hz) / max(1.0 - hz, 0.05));
            float ripple = (Fbm(float2(uv.x * 10.0, depth * 8.0 + bgp.bgFlow + time * animSpeed * 2.0), oc) - 0.5) * 0.02 * (0.2 + depth);
            float rsy = 2.0 * hz - sy;
            float3 water = Ramp5(saturate((rsy + ripple * 4.0) / max(hz, 0.05)), col1, c5, col3, c6, col2) * lerp(0.85, 0.5, depth);
            float2 rsp = float2((uv.x - sunx) * asp + ripple * 3.0, rsy - hz);
            water += sunC * exp(-dot(rsp, rsp) / (sunR * sunR) * 2.0) * (0.4 + bgp.bgGlow * 0.6) * (1.0 - depth * 0.5);
            float pathW = 0.015 + depth * 0.16;
            float band = smoothstep(pathW, 0.0, abs((uv.x - sunx) * asp));
            float shimmer = pow(0.5 + 0.5 * sin(sy * 130.0 + Fbm(float2(uv.x * 24.0, sy * 50.0), 3) * 12.0), 2.5);
            water += sunHot * band * shimmer * (0.35 + 0.65 * depth) * (0.6 + bgp.bgGlow);
            pat = lerp(pat, water, smoothstep(0.0, 0.015, sy - hz));
            pat += sunC * exp(-pow((sy - hz) * 55.0, 2.0)) * 0.3;
        }

        pat += sunC * exp(-length(float2((uv.x - sunx) * asp, sy - hz)) * 1.2) * 0.05;
        float2 vgp = uv - 0.5; vgp.x *= asp;
        pat *= 1.0 - smoothstep(0.65, 1.25, length(vgp)) * 0.3;
    }
    else if (bgp.bgStyle == 25) {
        int oc = (int)clamp(bgp.bgFbm, 1.0, 6.0);
        float3 c5 = float3(bgp.bgCol5R, bgp.bgCol5G, bgp.bgCol5B);
        float3 c6 = float3(bgp.bgCol6R, bgp.bgCol6G, bgp.bgCol6B);
        float3 lightC = float3(bgp.bgCol4R, bgp.bgCol4G, bgp.bgCol4B);
        float3 soul = float3(0.63, 0.69, 0.88);
        float sy = wsuv.y;

        pat = Ramp5(saturate(sy), col1, c5, col3, c6, col2);
        float lum0 = dot(pat, float3(0.299, 0.587, 0.114));
        pat = lerp(pat, float3(lum0, lum0, lum0) * float3(0.88, 0.96, 1.08), saturate(bgp.bgNebContrast) * 0.6);

        float2 lp = float2((uv.x - (0.5 + bgp.bgOffX)) * asp, sy - (0.35 + bgp.bgOffY));
        float ld = length(lp);
        pat += lightC * exp(-ld * ld * 2.6) * (0.13 + bgp.bgGlow * 0.30);
        float la = atan2(lp.y, lp.x);
        float rays = pow(0.5 + 0.5 * sin(la * 20.0 + Fbm(float2(la * 3.0, 1.0), oc) * 6.0), 3.0);
        float rayGlow = rays * exp(-ld * 1.7) * smoothstep(0.04, 0.3, ld);
        pat += lightC * rayGlow * (0.05 + bgp.bgGlow * 0.14);

        float2 dp = lp * 3.2;
        dp += (float2(Fbm(dp + 3.0, oc), Fbm(dp + 8.0, oc)) - 0.5) * 2.2;
        float dend = pow(RidgedFbm(dp, oc), 5.0) * exp(-ld * 1.25);
        pat += HueShift(lightC, (Fbm(dp, oc) - 0.5) * 0.12) * dend * (0.3 + bgp.bgGlow) * 0.5;

        float ringR = 0.30 + saturate(bgp.bgScaleY / 12.0) * 0.22;
        float dr = 0.008 + bgp.bgDisperse * 0.03;
        float3 ring3 = float3(exp(-pow((ld * (1.0 + dr) - ringR) * 17.0, 2.0)),
                              exp(-pow((ld - ringR) * 17.0, 2.0)),
                              exp(-pow((ld * (1.0 - dr) - ringR) * 17.0, 2.0)));
        pat += ring3 * lightC * (0.12 + bgp.bgGlow * 0.20);

        float2 mp = float2(uv.x * 2.2 + bgp.bgTwist, sy * 1.8 - bgp.bgFlow - time * animSpeed);
        mp += (float2(Fbm(mp + 1.0, oc), Fbm(mp * 1.2 + 5.0, oc)) - 0.5) * 2.0;
        float mist = pow(0.5 + 0.5 * sin(mp.x * 2.0 + mp.y * 1.2), 2.0) * Fbm(mp, oc);
        pat += soul * mist * (0.09 + bgp.bgHaze * 0.22);

        float2 cp = float2(uv.x * asp, sy) * (3.0 + bgp.bgScale * 0.6);
        cp += (float2(Fbm(cp + 1.0, oc), Fbm(cp + 6.0, oc)) - 0.5) * (0.6 + bgp.bgNebWarp * 1.5);
        float crystal = pow(RidgedFbm(cp * 1.2, oc), 4.0);
        float edge = saturate(smoothstep(0.35, 0.95, length((uv - 0.5) * float2(asp, 1.0))) + smoothstep(0.6, 1.0, sy) * 0.7);
        pat += HueShift(lightC, (Fbm(cp * 0.5, oc) - 0.5) * 0.18) * crystal * edge * (0.35 + bgp.bgGlow) * 0.55;
        pat += lightC * pow(crystal, 1.6) * edge * 0.28;

        if (bgp.bgEmbers > 0.0) {
            float2 fp = float2(uv.x * asp, sy) * (10.0 + bgp.bgScale);
            fp.y += bgp.bgFlow * 4.0 + time * animSpeed * 2.0;
            fp.x += bgp.bgTwist * fp.y * 0.1;
            float2 fcl = floor(fp);
            float fh = Hash21(fcl + 13.7);
            if (fh > 0.82) {
                float2 jit = 0.3 + 0.4 * float2(Hash21(fcl + 2.1), Hash21(fcl + 8.3));
                float2 d = (frac(fp) - jit) * float2(1.0, 0.6);
                pat += lerp(soul, lightC, frac(fh * 5.0)) * exp(-dot(d, d) * lerp(70.0, 24.0, saturate(bgp.bgEmberSize))) * frac(fh * 31.0) * bgp.bgEmbers * (0.7 + rayGlow * 2.2);
            }
        }

        if (bgp.bgSparkle > 0.0) {
            float2 ffp = float2(uv.x * asp * 5.0, sy * 3.5 + bgp.bgFlow * 1.2 + time * animSpeed * 1.5);
            ffp.x += sin(ffp.y * 1.5 + Fbm(float2(ffp.y, 0.0), 2) * 3.0) * 0.4;
            float2 fcell = floor(ffp);
            float feh = Hash21(fcell + 51.7);
            if (feh > 0.9) {
                float2 fj = 0.5 + 0.3 * float2(sin(feh * 30.0), cos(feh * 47.0));
                float2 fd = (frac(ffp) - fj) * float2(2.4, 0.5);
                pat += lightC * exp(-dot(fd, fd) * 9.0) * bgp.bgSparkle * (0.5 + rayGlow) * 0.6;
            }
        }

        if (bgp.bgGrain > 0.0) {
            float gn = frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            pat += (gn - 0.5) * bgp.bgGrain * 0.22;
        }
        float2 vgp = uv - 0.5; vgp.x *= asp;
        pat *= 1.0 - smoothstep(0.45, 1.12, length(vgp)) * 0.7;
    }
    else if (bgp.bgStyle == 26) {
        int oc = (int)clamp(bgp.bgFbm, 1.0, 6.0);
        float3 c5 = float3(bgp.bgCol5R, bgp.bgCol5G, bgp.bgCol5B);
        float3 c6 = float3(bgp.bgCol6R, bgp.bgCol6G, bgp.bgCol6B);
        float3 patCol = float3(bgp.bgCol4R, bgp.bgCol4G, bgp.bgCol4B);
        float2 gc = float2(0.5 + bgp.bgOffX, 0.5 + bgp.bgOffY);
        float2 gp = uv - gc; gp.x *= asp;
        float gt;
        if (bgp.bgGradType == 1) gt = length(gp) * 1.6;
        else if (bgp.bgGradType == 2) gt = (abs(gp.x) + abs(gp.y)) * 1.6;
        else if (bgp.bgGradType == 3) gt = frac((atan2(gp.y, gp.x) + bgp.bgAngle) / 6.2831853 + 1.0);
        else { float ca = cos(bgp.bgAngle), sa = sin(bgp.bgAngle); gt = dot(gp, float2(ca, sa)) + 0.5; }
        if (bgp.bgSharp > 0.0) gt = saturate((gt - 0.5) * (1.0 + bgp.bgSharp * 12.0) + 0.5);
        pat = Ramp5(saturate(gt), col1, c5, col3, c6, col2);
        if (bgp.bgPatMode > 0 && bgp.bgPatStrength > 0.0) {
            float pca = cos(bgp.bgPatAngle), psa = sin(bgp.bgPatAngle);
            float2 pp = float2(gp.x * pca - gp.y * psa, gp.x * psa + gp.y * pca);
            float sc = max(bgp.bgScale, 1.0);
            float sz = 0.05 + saturate(bgp.bgScaleY / 20.0) * 0.35;
            float m = 0.0;
            if (bgp.bgPatMode == 1) m = step(0.5, frac(pp.x * sc));
            else if (bgp.bgPatMode == 2) m = fmod(floor(pp.x * sc) + floor(pp.y * sc), 2.0);
            else if (bgp.bgPatMode == 3) { float2 cc = frac(pp * sc) - 0.5; m = smoothstep(sz + 0.14, sz, length(cc)); }
            else if (bgp.bgPatMode == 4) { float2 gg = abs(frac(pp * sc) - 0.5); m = 1.0 - smoothstep(sz * 0.5, sz * 0.5 + 0.04, min(gg.x, gg.y)); }
            else if (bgp.bgPatMode == 5) m = step(0.5, frac(length(gp) * sc));
            else if (bgp.bgPatMode == 6) { float a = atan2(gp.y, gp.x); m = step(0.5, frac(a / 6.2831853 * max(sc, 2.0))); }
            else { float a = atan2(gp.y, gp.x); m = step(0.5, frac(a / 6.2831853 + length(gp) * sc)); }
            pat = lerp(pat, patCol, saturate(m) * bgp.bgPatStrength);
        }
        if (bgp.bgNebContrast > 0.0) {
            float2 np = uv * (1.5 + bgp.bgScale * 0.25) + 3.0;
            np += (Fbm(np, oc) - 0.5) * bgp.bgNebWarp * 2.0;
            float n = Fbm(np, oc);
            pat = lerp(pat, lerp(pat * (0.4 + n), patCol, 0.3), bgp.bgNebContrast);
        }
    }
    else if (bgp.bgStyle == 27) {
        int uoc = (int)clamp(bgp.bgFbm, 1.0, 6.0);
        float3 uc1 = col1, uc2 = float3(bgp.bgCol5R, bgp.bgCol5G, bgp.bgCol5B), uc3 = col3;
        float3 uc4 = float3(bgp.bgCol6R, bgp.bgCol6G, bgp.bgCol6B), uc5 = col2;
        float3 uacc = float3(bgp.bgCol4R, bgp.bgCol4G, bgp.bgCol4B);
        float tA = time * animSpeed;
        float sy = wsuv.y;
        float uang = bgp.bgAngle + time * bgp.bgFlow;
        float uca = cos(uang), usa = sin(uang);
        float2 rp = float2(pc.x * uca - pc.y * usa, pc.x * usa + pc.y * uca);

        if (bgp.bgTwist != 0.0) {
            float ang2 = bgp.bgTwist * 3.0 * saturate(1.0 - length(rp));
            float s2 = sin(ang2), c2 = cos(ang2);
            rp = float2(rp.x * c2 - rp.y * s2, rp.x * s2 + rp.y * c2);
        }
        float2 wp = rp;
        if (bgp.univWarp > 0.0) {
            float uf = 1.0 + bgp.univDetail * 3.0;
            float2 q = float2(Fbm(rp * uf + 11.3, uoc), Fbm(rp * uf + 7.1, uoc));
            wp = rp + (q - 0.5) * bgp.univWarp * 2.0;
        }

        float gt;
        if (bgp.univBase == 1) gt = length(wp) * 1.4;
        else if (bgp.univBase == 2) gt = (abs(wp.x) + abs(wp.y)) * 1.4;
        else if (bgp.univBase == 3) gt = frac(atan2(wp.y, wp.x) / 6.2831853 + 1.0);
        else if (bgp.univBase == 4) gt = abs(wp.x) * 2.0;
        else if (bgp.univBase == 5) gt = frac(length(wp) * 2.0 + atan2(wp.y, wp.x) / 6.2831853);
        else if (bgp.univBase == 6) {
            float2 vpt = float2(0.0, -0.85);
            float fan = (wp.x - vpt.x) / max(wp.y - vpt.y, 0.05);
            gt = frac(fan * (1.0 + max(bgp.bgScale, 1.0) * 0.25) + 0.5);
        }
        else gt = wp.y + 0.5;

        float n = 0.5;
        if (bgp.univNoise > 0) {
            float2 np = wp * (1.5 + bgp.univNoiseScale * 3.0) + 5.0;
            np.y *= (1.0 + bgp.bgFlow * 3.0);
            np.y -= tA * 0.30;
            if (bgp.univNoise == 1) n = Fbm(np, uoc);
            else if (bgp.univNoise == 2) n = RidgedFbm(np, uoc);
            else if (bgp.univNoise == 3) n = Voronoi(np);
            else if (bgp.univNoise == 4) n = abs(Fbm(np, uoc) * 2.0 - 1.0);
            else if (bgp.univNoise == 5) { float2 q2 = float2(Fbm(np, uoc), Fbm(np + 3.1, uoc)); n = Fbm(np + (q2 - 0.5) * 3.0, uoc); }
            else if (bgp.univNoise == 6) n = BillowFbm(np, uoc);
            else if (bgp.univNoise == 7) n = 0.5 + 0.5 * sin((np.x + np.y) * 1.7 + (Fbm(np * 0.6, uoc) - 0.5) * 9.0);
            else if (bgp.univNoise == 8) n = 0.5 + 0.5 * sin(length(np - 8.0) * 2.4 + (Fbm(np * 0.5, uoc) - 0.5) * 6.0);
            else if (bgp.univNoise == 9)  { float2 vc = Voro2(np); n = 1.0 - smoothstep(0.0, 0.07, vc.y - vc.x); }
            else if (bgp.univNoise == 10) { float2 vd = Voro2(np); n = 1.0 - smoothstep(0.12, 0.5, vd.x); }
            else { float w1 = sin(np.x * 6.2831), w2 = sin(np.y * 6.2831); n = saturate(0.5 + 0.42 * w1 * w2 + (Fbm(np, uoc) - 0.5) * 0.5); }
            if (bgp.univNoise == 1 || bgp.univNoise == 4 || bgp.univNoise == 5 || bgp.univNoise == 6)
                n = saturate((n - 0.5) * 1.4 + 0.5);
            if (bgp.bgNebContrast > 0.0)
                n = pow(saturate((n - 0.5) * (1.0 + bgp.bgNebContrast * 3.0) + 0.5), 1.0 + bgp.bgNebContrast * 2.0);
        }

        float tt = gt;
        if (bgp.univNoise > 0 && bgp.univBlend == 0) tt = gt + (n - 0.5) * bgp.univNoiseAmt * 2.0;
        if (bgp.bgSharp > 0.0) tt = (tt - 0.5) * (1.0 + bgp.bgSharp * 12.0) + 0.5;
        pat = Ramp5(saturate(tt), uc1, uc2, uc3, uc4, uc5);
        if (bgp.univNoise > 0 && bgp.univBlend > 0) {
            float ns = saturate(bgp.univNoiseAmt);
            if (bgp.univBlend == 1) pat += uacc * n * bgp.univNoiseAmt;
            else if (bgp.univBlend == 2) pat *= lerp(1.0, saturate(n * 1.6), ns);
            else if (bgp.univBlend == 3) pat = lerp(pat, pat * (n * 2.0), ns);
            else if (bgp.univBlend == 4) pat = lerp(pat, uacc, saturate(n) * ns);
            else pat *= lerp(1.0, 0.35 + 1.3 * saturate(n), ns);
        }

        float2 orbP = float2((uv.x - bgp.univOrbX) * asp, sy - bgp.univOrbY);
        float orbD = length(orbP);
        float orbR = max(bgp.univOrbSize, 0.005);
        if (bgp.univOrb > 0) {
            if (bgp.univOrb == 1)
                pat = lerp(pat, uacc, smoothstep(orbR, orbR * 0.94, orbD));
            else if (bgp.univOrb == 2) {
                float rr = abs(orbD - orbR) / max(orbR * 0.18, 1e-3);
                pat += uacc * exp(-rr * rr) * (0.6 + bgp.bgGlow);
            }
            float hg = orbD / max(orbR, 1e-3);
            pat += uacc * exp(-hg * hg * 0.55) * (0.20 + bgp.bgGlow * 0.7);
        }

        if (bgp.univRidges > 0.0) {
            [unroll] for (int R = 0; R < 3; R++) {
                float fr = (float)R;
                float baseY = 0.5 + fr * 0.10 + bgp.bgOffY;
                float amp = bgp.univRidges * (0.16 - fr * 0.035);
                float ridge = RidgedFbm(float2(uv.x * asp * (0.9 + fr * 0.8) + fr * 9.0 + bgp.bgOffX * 2.0, fr * 3.7), uoc);
                float h = baseY - ridge * amp;
                float m3 = smoothstep(h + 0.005, h - 0.005, sy);
                pat = lerp(pat, lerp(uc1, uc3, 0.35 - fr * 0.1), m3 * (0.75 - fr * 0.18));
            }
        }

        if (bgp.univHorizon > 0.0) {
            float hz = clamp(bgp.univHorizon, 0.05, 0.95);
            if (sy > hz) {
                float gd = saturate((sy - hz) / max(1.0 - hz, 0.05));
                float2 gp = float2((uv.x - 0.5) * asp, gd + 0.05);
                float3 gcol = lerp(uc5, uc1, gd * 0.55);
                if (bgp.univGround == 1) {
                    float rt = saturate((hz - (sy - hz) * 0.85) / max(hz, 0.05));
                    gcol = Ramp5(rt, uc1, uc2, uc3, uc4, uc5) * 0.82;
                    float gx = abs((uv.x - bgp.univOrbX) * asp) * 6.0;
                    gcol += uacc * exp(-gx * gx) * 0.25 * gd;
                } else if (bgp.univGround == 2) {
                    float pz = 1.0 / max(gd, 0.02);
                    float gx2 = frac((uv.x - 0.5) * asp * pz * 0.5 + 0.5);
                    float gy2 = frac(pz * 0.5 - tA * 0.5);
                    float lw = 0.03 + gd * 0.05;
                    float gl = max(smoothstep(lw, 0.0, min(gx2, 1.0 - gx2)), smoothstep(lw, 0.0, min(gy2, 1.0 - gy2)));
                    gcol = lerp(uc1 * 0.35, uacc, gl * (0.8 + bgp.bgGlow * 0.5));
                } else if (bgp.univGround == 3) {
                    float rip = sin((gd * 26.0) + Fbm(gp * 3.0 + float2(0.0, tA * 0.4), 3) * 6.0);
                    gcol = lerp(uc5, uc4, saturate(0.5 + 0.35 * rip)) * (0.85 + 0.25 * gd);
                    gcol += uacc * pow(saturate(rip), 6.0) * (0.2 + bgp.bgGlow * 0.4) * (1.0 - gd);
                } else if (bgp.univGround == 4) {
                    float crack = pow(RidgedFbm(gp * (4.0 + bgp.bgScale * 0.5), uoc), 3.0);
                    gcol = lerp(uc5, uc1, gd * 0.6) * (0.85 + 0.2 * Voronoi(gp * 5.0));
                    gcol += uacc * crack * (0.25 + bgp.bgGlow * 0.3);
                } else if (bgp.univGround == 5) {
                    float cell = Voronoi(gp * (3.0 + bgp.bgScale * 0.4) + float2(0.0, tA * 0.15));
                    float vein = smoothstep(0.28, 0.0, cell);
                    float3 crust = lerp(uc1, uc2, gd) * 0.5;
                    gcol = lerp(crust, uacc * 1.4, vein * (0.7 + bgp.bgGlow * 0.5));
                    gcol += uc4 * pow(vein, 2.0) * 0.5;
                }
                pat = lerp(pat, gcol, smoothstep(0.0, 0.015, sy - hz));
                float he = (sy - hz) * 45.0;
                pat += uacc * exp(-he * he) * (0.15 + bgp.bgGlow * 0.25);
            }
        }

        if (bgp.univPattern > 0 && bgp.univPatStrength > 0.0) {
            float usc = max(bgp.bgScale, 1.0);
            float usz = 0.05 + saturate(bgp.bgScaleY / 20.0) * 0.35;
            float2 pp = wp;
            int pm = bgp.univPattern;
            float m2 = 0.0;
            if (pm == 1) m2 = AaStep(0.5, frac(pp.x * usc));
            else if (pm == 2) { float2 f = frac(pp * usc); m2 = abs(AaStep(0.5, f.x) - AaStep(0.5, f.y)); }
            else if (pm == 3) { float2 cc = frac(pp * usc) - 0.5; m2 = 1.0 - AaStep(usz, length(cc)); }
            else if (pm == 4) { float2 g = abs(frac(pp * usc) - 0.5); m2 = AaStep(0.5 - usz * 0.5, max(g.x, g.y)); }
            else if (pm == 5) m2 = AaStep(0.5, frac(length(rp) * usc));
            else if (pm == 6) { float a = atan2(rp.y, rp.x) / 6.2831853; m2 = AaStep(0.5, frac(a * max(usc, 2.0))); }
            else if (pm == 7) { float2 h = pp * usc; h.x *= 1.1547; h.y += 0.5 * fmod(floor(h.x), 2.0); float2 f2 = abs(frac(h) - 0.5); m2 = 1.0 - AaStep(0.36, max(f2.x, f2.y)); }
            else if (pm == 8) m2 = 0.5 + 0.5 * sin((pp.x + pp.y) * usc * 3.1415927);
            else if (pm == 9) { float2 t = pp * usc; t.x += t.y * 0.5; float2 f = frac(t); m2 = AaStep(0.0, f.x + f.y - 1.0); }
            else if (pm == 10) { float2 d = abs(frac(pp * usc) - 0.5); m2 = AaStep(0.5, d.x + d.y); }
            else if (pm == 11) { float2 b = pp * usc; b.x += 0.5 * floor(b.y); float2 g = abs(frac(b) - 0.5); m2 = 1.0 - AaStep(0.42, max(g.x * 0.6, g.y)); }
            else if (pm == 12) { float2 s = pp * usc; s.y *= 1.15; s.x += 0.5 * fmod(floor(s.y), 2.0); float2 cc = frac(s) - float2(0.5, 0.0); m2 = 1.0 - AaStep(0.5, length(cc)); }
            else if (pm == 13) { float2 z = pp * usc; float tri = abs(frac(z.y) - 0.5) * 2.0; m2 = AaStep(0.5, frac(z.x + tri)); }
            else if (pm == 14) { float2 c = pp * usc; float2 i = floor(c), f = frac(c); if (Hash21(i) < 0.5) f.x = 1.0 - f.x; float d = min(abs(length(f) - 0.5), abs(length(f - 1.0) - 0.5)); m2 = 1.0 - AaStep(0.12, d); }
            else if (pm == 15) {
                float2 c = pp * usc * 0.6; float2 ip = floor(c), fp = frac(c);
                float f1 = 8.0, f2 = 8.0;
                [unroll] for (int jj = -1; jj <= 1; jj++)
                [unroll] for (int ii = -1; ii <= 1; ii++) {
                    float2 g = float2(ii, jj);
                    float2 o = float2(Hash21(ip + g), Hash21(ip + g + 3.7));
                    float d = length(g + o - fp);
                    if (d < f1) { f2 = f1; f1 = d; } else if (d < f2) f2 = d;
                }
                m2 = 1.0 - AaStep(0.02 + usz * 0.12, f2 - f1);
            }
            else if (pm == 16) { float2 f = frac(pp * usc) - 0.5; float d = min(min(length(f - float2(0.5, 0.0)), length(f + float2(0.5, 0.0))), min(length(f - float2(0.0, 0.5)), length(f + float2(0.0, 0.5)))); m2 = 1.0 - AaStep(0.5, d); }
            else if (pm == 17) { float2 c = pp * usc; float2 i = floor(c); float over = fmod(i.x + i.y, 2.0); float2 f = frac(c); m2 = lerp(AaStep(0.5, f.x), AaStep(0.5, f.y), over); }
            else if (pm == 18) { float ang = atan2(rp.y, rp.x) / 6.2831853; m2 = AaStep(0.5, frac(length(rp) * usc * 0.5 + ang * max(usc, 2.0))); }
            else if (pm == 19) {
                float2 c = pp * usc * float2(3.6, 2.2);
                float2 ip = floor(c), fp = frac(c);
                float h = Hash21(ip);
                float colB = 0.22 + 0.78 * Hash21(float2(ip.x, 7.3));
                float rowB = 0.55 + 0.45 * Hash21(float2(2.7, ip.y));
                if (h > 0.20) {
                    float2 q = fp - 0.5;
                    float g;
                    if (frac(h * 17.3) > 0.5)
                        g = 1.0 - AaStep(0.10, max(abs(q.x) * 3.4, abs(q.y) * 1.20));
                    else {
                        float o = max(abs(q.x) * 2.7, abs(q.y) * 1.20);
                        g = (1.0 - AaStep(0.44, o)) * AaStep(0.25, o);
                    }
                    m2 = g * colB * rowB * (0.40 + 0.60 * frac(h * 53.1));
                    if (frac(h * 91.7) > 0.982) m2 = g;
                }
            }
            else if (pm == 20) {
                float2 c = pp * usc * 2.4; float2 ip = floor(c), fp = frac(c);
                float h = Hash21(ip);
                float w = 0.020 + usz * 0.022;
                float2 q = fp - 0.5;
                float d;
                if (h < 0.30) d = abs(q.y);
                else if (h < 0.58) d = abs(q.x);
                else if (h < 0.72) d = min(abs(q.x), abs(q.y));
                else if (h < 0.86) d = min(abs(length(fp) - 0.5), abs(length(fp - 1.0) - 0.5));
                else d = min(abs(length(fp - float2(1.0, 0.0)) - 0.5), abs(length(fp - float2(0.0, 1.0)) - 0.5));
                float trace = 1.0 - AaStep(w, d);
                float rr = length(q);
                float via = 0.0;
                if (frac(h * 37.1) > 0.86)
                    via = max(1.0 - AaStep(w * 1.6, rr), (1.0 - AaStep(w * 0.7, abs(rr - 0.10))));
                m2 = max(trace * 0.55, via);
            }
            else if (pm == 21) {
                float2 c = pp * usc; c.y *= 2.0;
                c.x += 0.5 * fmod(floor(c.y), 2.0);
                float2 f = float2(frac(c.x) - 0.5, frac(c.y));
                float r = length(float2(f.x, f.y * 0.62));
                m2 = (1.0 - AaStep(0.5, r)) * AaStep(0.5, frac(r * 4.0));
            }
            else if (pm == 22) {
                float2 f = abs(frac(pp * usc) - 0.5);
                float star = min(max(f.x, f.y), (f.x + f.y) * 0.70711);
                float w = 0.022 + usz * 0.05;
                m2 = 1.0 - AaStep(w, abs(star - 0.30));
            }
            else if (pm == 23) {
                float2 c = pp * usc * 0.62; c.y *= 1.25;
                float2 ip = floor(c);
                float2 f = float2(frac(c.x) - 0.5, frac(c.y));
                float alt = fmod(ip.x, 2.0);
                float lift = alt > 0.5 ? 0.0 : 0.06;
                float wHeavy = 0.030 + usz * 0.030;
                float wLight = 0.014 + usz * 0.016;
                float aL = abs(length(f - float2(-0.30, 0.24 + lift)) - 0.72);
                float aR = abs(length(f - float2( 0.30, 0.24 + lift)) - 0.72);
                float arch = (f.y > 0.24) ? min(aL, aR) : 9.0;
                float jamb = (f.y <= 0.28) ? abs(abs(f.x) - 0.44) : 9.0;
                float sill = abs(f.y - 0.06);
                float heavy = min(min(arch, jamb), sill);
                float2 sfl = float2(abs(f.x) - 0.21, f.y);
                float bL = abs(length(sfl - float2(-0.12, 0.30)) - 0.30);
                float bR = abs(length(sfl - float2( 0.12, 0.30)) - 0.30);
                float sub = (f.y > 0.28 && f.y < 0.70) ? min(bL, bR) : 9.0;
                float mull = (f.y > 0.28 && f.y < 0.62) ? abs(f.x) : 9.0;
                float rose = abs(length(f - float2(0.0, 0.80 + lift)) - 0.115);
                float cusp = abs(length(f - float2(0.0, 0.80 + lift)) - 0.052);
                float light = min(min(sub, mull), min(rose, cusp));
                m2 = max(1.0 - AaStep(wHeavy, heavy), (1.0 - AaStep(wLight, light)) * 0.72);
            }
            else if (pm == 24) {
                float2 c = pp * usc * 0.5; float2 ip = floor(c), fp = frac(c);
                float ln = 9.0, st = 0.0;
                [loop] for (int cj = -1; cj <= 0; cj++)
                [loop] for (int ci = -1; ci <= 0; ci++) {
                    float2 o = float2((float)ci, (float)cj), g = ip + o;
                    float hg = Hash21(g);
                    float2 a0 = o + float2(hg, Hash21(g + 3.7));
                    float2 n1 = g + float2(1.0, 0.0), n2 = g + float2(0.0, 1.0);
                    float2 b1 = o + float2(1.0, 0.0) + float2(Hash21(n1), Hash21(n1 + 3.7));
                    float2 b2 = o + float2(0.0, 1.0) + float2(Hash21(n2), Hash21(n2 + 3.7));
                    if (frac(hg * 23.3) > 0.45) ln = min(ln, SegSD(fp, a0, b1));
                    if (frac(hg * 47.9) > 0.45) ln = min(ln, SegSD(fp, a0, b2));
                    float mag = 0.010 + 0.030 * frac(hg * 13.7);
                    float d0 = length(fp - a0);
                    float core = 1.0 - AaStep(mag, d0);
                    float glow = exp(-d0 / max(mag * 2.2, 1e-4)) * 0.55;
                    float sp = 0.0;
                    if (frac(hg * 71.3) > 0.86) {
                        float2 r = fp - a0;
                        float axis = min(abs(r.x), abs(r.y));
                        sp = (1.0 - AaStep(mag * 0.20, axis)) * exp(-d0 / max(mag * 6.0, 1e-4)) * 0.7;
                    }
                    st = max(st, max(core, max(glow, sp)));
                }
                m2 = max(st, (1.0 - AaStep(0.007, ln)) * 0.26);
                float2 dp = pp * usc * 2.6; float2 dip = floor(dp);
                float dh = Hash21(dip + 5.5);
                if (dh > 0.90) {
                    float dd = length(frac(dp) - float2(Hash21(dip + 1.3), Hash21(dip + 9.1)));
                    m2 = max(m2, exp(-dd / 0.035) * 0.20);
                }
            }
            else if (pm == 25) {
                float2 fz = pp * usc * 0.85;
                float2 ipf = floor(fz), ff = frac(fz);
                float bloom = 0.0;
                [loop] for (int fj = -1; fj <= 1; fj++)
                [loop] for (int fi = -1; fi <= 1; fi++) {
                    float2 o = float2((float)fi, (float)fj), g = ipf + o;
                    float2 nuc = o + float2(Hash21(g + 2.1), Hash21(g + 6.3));
                    float2 d2 = ff - nuc;
                    float r = length(d2);
                    if (r > 1.25) continue;
                    float2 w = d2 * (2.3 + Hash21(g + 4.4) * 1.4);
                    w += (float2(Fbm(w * 1.4 + g, 3), Fbm(w * 1.4 + g + 11.0, 3)) - 0.5) * 1.1;
                    float vein = pow(RidgedFbm(w * 2.4, 4), 2.0);
                    float ang = atan2(d2.y, d2.x);
                    float arms = 5.0 + floor(Hash21(g + 9.7) * 4.0);
                    float bias = 0.62 + 0.38 * sin(ang * arms + Fbm(d2 * 4.0 + g, 2) * 3.2);
                    float fall = saturate(1.0 - r * 0.92);
                    fall *= fall;
                    bloom = max(bloom, saturate(vein * bias * 1.35) * fall);
                }
                m2 = bloom;
            }
            else if (pm == 26) {
                float up = saturate(1.0 - sy);
                float2 fl = pp * usc;
                fl.y *= 0.62 - up * 0.34;
                fl.y -= tA * 1.5;
                fl.x += sin(fl.y * 1.5 + tA * 1.7) * 0.26 * (0.30 + up * 1.05);
                fl.x += sin(fl.y * 3.9 - tA * 2.3) * 0.09 * up * up;
                float n = Fbm(fl, 5);

                float body = saturate((n - 0.40) * 3.2);
                float fine = Fbm(fl * 2.7 + 13.0, 3);
                body = saturate(body - up * up * (0.72 - fine) * 0.85);

                float sheet = body * (1.0 - body) * 4.0;

                float crest = Fbm(float2(fl.x * 0.55, tA * 0.35), 2);
                float fuel = saturate(sy * 1.30);
                m2 = saturate((body * 0.88 + sheet * 0.62) * (0.25 + fuel * 1.65) - up * (0.70 - crest * 0.55));
            }
            else if (pm == 27) {
                float2 P = pp * usc * 1.15;
                float2 drift = float2(0.0, tA * 0.14);
                float e = 0.055;
                float p0 = Fbm(P + drift, 3);
                float pdx = Fbm(P + float2(e, 0.0) + drift, 3);
                float pdy = Fbm(P + float2(0.0, e) + drift, 3);
                float2 v = float2(pdy - p0, -(pdx - p0)) / e;
                v = v / max(length(v), 1e-4);
                float acc = 0.0;
                float2 q = P - v * 0.30;
                [loop] for (int k = 0; k < 9; k++) {
                    acc += VNoise(q * 19.0 + float2(0.0, tA * 0.5));
                    q += v * 0.075;
                }
                float lic = acc * 0.11111;
                m2 = saturate((lic - 0.52) * 6.5);
            }
            else if (pm == 28) {
                float2 P = pp * usc * 0.30;
                float slot = floor(P.x);
                float sh = Hash21(float2(slot, 3.7));
                float tc = tA * 1.15 + sh * 7.0;
                float seed = floor(tc);
                float fire = step(0.30, Hash21(float2(seed, slot + 11.3)));
                float flash = exp(-frac(tc) * 6.0) * fire;
                float strike = 0.45 + 0.55 * flash;
                float disp = 0.0, amp = 0.20, fq = 1.0;
                [unroll] for (int o = 0; o < 5; o++) {
                    disp += (VNoise(float2(P.y * fq * 2.2 + slot * 7.0, seed * 3.1 + (float)o * 5.0)) - 0.5) * amp;
                    amp *= 0.5; fq *= 2.0;
                }
                float x = frac(P.x) - 0.5;
                float dch = abs(x - disp);
                float w = 0.004 + usz * 0.028;
                float cq = dch / w;
                float core = exp(-cq * cq);
                float gq = dch / (w * 6.5);
                float corona = exp(-gq * gq);
                float bd = abs(x - disp - (VNoise(float2(P.y * 9.0 + slot * 3.3, seed)) - 0.5) * 0.16);
                float bq = bd / (w * 1.7);
                float br = exp(-bq * bq) * step(0.55, Hash21(float2(floor(P.y * 6.0), seed + slot)));
                m2 = saturate((core * 1.25 + corona * 0.45 + br * 0.55) * strike);
            }
            else m2 = AaStep(0.5, frac(pp.x * usc));

            float2 mld = float2(bgp.bgLightX, bgp.bgLightY);
            mld = (dot(mld, mld) < 1e-4) ? float2(0.6, -0.8) : normalize(mld);
            float2 msv = (baseUv - 0.5) * float2(asp, 1.0);
            float mg = saturate(dot(msv, mld) * (0.55 + saturate(patMatRange) * 1.70) + 0.5);

            float3 pig;
            if (bgp.patColOverride == 0) pig = uacc;
            else if (bgp.patColMode == 1) pig = lerp(float3(bgp.patColR, bgp.patColG, bgp.patColB), float3(bgp.patCol2R, bgp.patCol2G, bgp.patCol2B), mg);
            else if (bgp.patColMode == 2) pig = Ramp5(mg, float3(bgp.patColR, bgp.patColG, bgp.patColB),
                                                      float3(bgp.patCol2R, bgp.patCol2G, bgp.patCol2B),
                                                      float3(bgp.patCol3R, bgp.patCol3G, bgp.patCol3B),
                                                      float3(bgp.patCol4R, bgp.patCol4G, bgp.patCol4B),
                                                      float3(bgp.patCol5R, bgp.patCol5G, bgp.patCol5B));
            else if (bgp.patColMode == 3) pig = Ramp5(mg, uc1, uc2, uc3, uc4, uc5);
            else if (bgp.patColMode == 5) pig = Ramp5(saturate(m2), uc1, uc2, uc3, uc4, uc5);
            else if (bgp.patColMode == 4) pig = Ramp5(saturate(m2), float3(bgp.patColR, bgp.patColG, bgp.patColB),
                                                                float3(bgp.patCol2R, bgp.patCol2G, bgp.patCol2B),
                                                                float3(bgp.patCol3R, bgp.patCol3G, bgp.patCol3B),
                                                                float3(bgp.patCol4R, bgp.patCol4G, bgp.patCol4B),
                                                                float3(bgp.patCol5R, bgp.patCol5G, bgp.patCol5B));
            else pig = float3(bgp.patColR, bgp.patColG, bgp.patColB);
            float3 patCol = pig;

            if (bgp.patMat > 0) {
                float bw = 0.08 + saturate(patMatRough) * 0.38;
                float t = (mg - saturate(patMatPos)) / bw;
                float band = exp(-t * t);
                float leaf = 0.94 + 0.12 * Fbm(msv * 9.0 + 4.0, 2);
                float streak = 0.975 + 0.025 * sin(dot(msv, float2(-mld.y, mld.x)) * 52.0);
                float3 refl = float3(bgp.patMatR, bgp.patMatG, bgp.patMatB);
                float3 base = lerp(pig, refl, saturate(bgp.patMatTint)) * leaf * streak;
                float sheen = max(patMatSheen, 0.0);
                float3 metal;
                if (bgp.patMat == 1) {
                    float3 shade = base * float3(0.52, 0.32, 0.15);
                    float3 pale  = lerp(base, refl * 0.35 + float3(0.72, 0.70, 0.64), 0.62);
                    metal = (mg < 0.5) ? lerp(shade, base, mg * 2.0) : lerp(base, pale, (mg - 0.5) * 2.0);
                    metal += pale * band * 0.60 * sheen;
                } else if (bgp.patMat == 2) {
                    metal = base * (0.62 + 0.48 * mg) + lerp(float3(1.0, 1.0, 1.0), refl, 0.35) * band * 0.70 * sheen;
                } else {
                    metal = HueShift(base, mg * 0.55 - 0.22) * (0.58 + 0.60 * mg) + band * 0.45 * sheen;
                }
                float pk = max(max(metal.r, metal.g), metal.b);
                patCol = metal / (1.0 + max(pk - 0.85, 0.0) * 0.85);
            }

            float ps = saturate(bgp.univPatStrength);
            if (bgp.univPatBlend == 1) pat += patCol * m2 * ps * (0.6 + bgp.bgGlow);
            else if (bgp.univPatBlend == 2) {
                float3 sh = (bgp.patMat > 0 || bgp.patColOverride != 0) ? saturate(patCol) * 0.55 : float3(0.3, 0.3, 0.3);
                pat *= lerp(float3(1.0, 1.0, 1.0), lerp(float3(1.0, 1.0, 1.0), sh, ps), m2);
            }
            else pat = lerp(pat, patCol, m2 * ps);
        }

        if (bgp.univCaustic > 0.0) {
            float2 cp = pc * (3.0 + bgp.bgScale * 0.4) + float2(bgp.bgFlow, tA * 0.4);
            float c1 = sin(cp.x * 1.7 + Fbm(cp, 3) * 3.5);
            float c2 = sin(cp.y * 1.5 + Fbm(cp + 5.0, 3) * 3.5);
            float caus = pow(saturate(c1 * c2), 3.0);
            pat += uacc * caus * bgp.univCaustic * (0.5 + bgp.bgGlow * 0.5);
        }

        if (bgp.univShafts > 0.0) {
            float2 src = float2((bgp.univOrb > 0 ? bgp.univOrbX : 0.5) - 0.5, (bgp.univOrb > 0 ? bgp.univOrbY : 0.0)) * float2(asp, 1.0);
            float2 d2 = pc - src;
            float ang = atan2(d2.x, d2.y + 0.001);
            float beams = pow(0.5 + 0.5 * sin(ang * 22.0 + Fbm(float2(ang * 3.0, tA * 0.2), 3) * 4.0), 3.0);
            float reach = smoothstep(1.4, 0.1, length(d2));
            pat += uacc * beams * reach * bgp.univShafts * (0.35 + bgp.bgGlow * 0.4);
        }

        if (bgp.univParticle > 0) {
            float pdens = 14.0 + max(bgp.bgStarDensity, 4.0) * 0.7;
            float psz = 0.05 + saturate(bgp.bgStarSize) * 0.28;
            float drift = (bgp.univParticle == 2) ? 1.0 : ((bgp.univParticle == 3) ? -1.4 : ((bgp.univParticle == 6) ? 0.9 : 0.25));
            float2 sp = float2(uv.x * asp, sy) * pdens;
            sp.y += tA * drift * 2.0;
            sp.x += sin(sp.y * 0.4 + (bgp.univParticle == 6 ? tA : 0.0)) * (bgp.univParticle == 6 ? 1.1 : 0.5);
            float2 pcel = floor(sp);
            float ph = Hash21(pcel + 3.1);
            float gate = (bgp.univParticle == 1) ? 0.955 : 0.90;
            if (ph > gate) {
                float2 pj = float2(Hash21(pcel + 1.7), Hash21(pcel + 8.3));
                float pd = length(frac(sp) - clamp(pj, 0.2, 0.8));
                float3 pcol;
                if (bgp.univParticle == 4 || bgp.univParticle == 6) {
                    float pick = frac(ph * 5.0);
                    pcol = pick < 0.2 ? uc1 : (pick < 0.4 ? uc2 : (pick < 0.6 ? uc3 : (pick < 0.8 ? uc4 : uacc)));
                } else pcol = (bgp.univParticle == 3) ? uacc : lerp(float3(1.0, 1.0, 1.0), uacc, 0.35);
                float sparkA;
                if (bgp.univParticle == 4)
                    sparkA = smoothstep(psz * 1.6, psz * 1.2, pd) * 0.5 + smoothstep(psz * 1.25, psz * 1.1, pd) * 0.3;
                else if (bgp.univParticle == 5)
                    sparkA = smoothstep(psz, psz * 0.75, pd) * 0.35 + smoothstep(psz * 0.55, psz * 0.4, pd) * 0.2;
                else if (bgp.univParticle == 6) {
                    float2 pf = (frac(sp) - clamp(pj, 0.2, 0.8)); pf.y *= 1.8;
                    sparkA = smoothstep(psz * 1.3, psz * 0.3, length(pf)) * 0.7;
                } else
                    sparkA = exp(-pd * pd * (60.0 / max(psz, 0.02)));
                pat += pcol * sparkA * (0.55 + bgp.bgGlow * 0.5);
            }
        }

        if (bgp.bgHaze > 0.0)
            pat += uacc * smoothstep(0.4, 0.9, Fbm(wp * 0.9 + 3.7, 3)) * bgp.bgHaze * 0.4;
        if (bgp.bgHueVar > 0.0) pat = HueShift(pat, (n - 0.5) * bgp.bgHueVar);
        pat += uacc * bgp.bgGlow * saturate(n) * 0.5;
    }
    else if (bgp.bgStyle == 28) {
        int oc = (int)clamp(bgp.bgFbm, 1.0, 6.0);
        float3 c5 = float3(bgp.bgCol5R, bgp.bgCol5G, bgp.bgCol5B);
        float3 c6 = float3(bgp.bgCol6R, bgp.bgCol6G, bgp.bgCol6B);
        float3 iceC = float3(bgp.bgCol4R, bgp.bgCol4G, bgp.bgCol4B);
        float sy = wsuv.y;
        float hz = clamp(0.52 + saturate(bgp.bgScaleY / 16.0) * 0.30, 0.4, 0.92);
        float tA = time * animSpeed;
        float skyT = saturate(sy / max(hz, 0.05));

        pat = Ramp5(skyT, col1, c5, col3, c6, col2);
        float veil = Fbm(float2(uv.x * 2.2 + bgp.bgTwist, sy * 3.2 - bgp.bgFlow * 0.5), oc);
        pat = lerp(pat, pat * 1.06 + iceC * 0.06, smoothstep(0.55, 0.95, veil) * smoothstep(hz, 0.05, sy) * 0.5);

        float2 lp = float2((uv.x - (0.5 + bgp.bgOffX)) * asp, sy - (hz - 0.05));
        float ld = length(lp);
        pat += iceC * exp(-ld * ld * 7.0) * (0.10 + bgp.bgGlow * 0.18);
        float sang = atan2(lp.x, -lp.y);
        float shaft = pow(0.5 + 0.5 * sin(sang * 26.0 + Fbm(float2(sang * 3.0, tA * 0.3), 3) * 4.0), 3.0);
        pat += iceC * shaft * smoothstep(hz + 0.1, 0.12, sy) * smoothstep(0.55, 0.12, ld) * bgp.bgGlow * 0.07;

        if (bgp.bgNebContrast > 0.0) {
            float2 ap = float2(uv.x * 3.0, sy * 6.0 - bgp.bgFlow - tA * 0.4);
            float aur = smoothstep(0.35, 0.9, Fbm(ap, oc)) * smoothstep(hz, 0.0, sy);
            pat += lerp(iceC, c5, 0.4) * aur * bgp.bgNebContrast * 0.4;
        }

        if (sy > hz) {
            float depth = saturate((sy - hz) / max(1.0 - hz, 0.05));
            float3 iceFar  = lerp(col2, float3(0.50, 0.78, 0.92), 0.35);
            float3 iceDeep = float3(0.08, 0.30, 0.48);
            float3 ice = lerp(iceFar, iceDeep, depth * 0.85);
            float2 icp = float2((uv.x - 0.5) * asp, depth + 0.15) * (5.0 + bgp.bgScale * 0.7);
            float vd = Voronoi(icp);
            ice *= 0.82 + 0.28 * smoothstep(0.15, 0.9, vd);
            float crack = pow(RidgedFbm(icp * 0.9, oc), 3.0);
            ice += lerp(float3(0.45, 0.78, 1.0), float3(0.9, 0.97, 1.0), depth) * crack * (0.35 + bgp.bgNebWarp * 0.4);
            float2 cap = icp * 0.8 + float2(bgp.bgFlow, tA * 0.35);
            float caust = pow(saturate(sin(cap.x * 1.7 + Fbm(cap, 3) * 3.5) * sin(cap.y * 1.5 + Fbm(cap + 5.0, 3) * 3.5)), 3.0);
            ice += iceC * caust * depth * (0.20 + bgp.bgGlow * 0.3);
            float glint = pow(0.5 + 0.5 * sin(uv.x * 150.0 + Fbm(float2(uv.x * 32.0, depth * 44.0), 3) * 11.0 + tA), 12.0);
            ice += float3(0.95, 0.98, 1.0) * glint * depth * (bgp.bgSparkle * 0.6 + 0.1);
            pat = lerp(pat, ice, smoothstep(0.0, 0.02, sy - hz));
            float hedge = (sy - hz) * 45.0;
            pat += iceC * exp(-hedge * hedge) * 0.22;
        }

        float2 mp = float2(uv.x * 2.0 + bgp.bgTwist, sy * 2.0 - bgp.bgFlow - tA * 0.5);
        mp += (Fbm(mp, oc) - 0.5) * 1.5;
        float mist = Fbm(mp, oc) * smoothstep(hz + 0.12, hz - 0.06, sy) * smoothstep(hz - 0.22, hz - 0.02, sy);
        pat = lerp(pat, lerp(pat, float3(0.85, 0.92, 1.0), 0.6), mist * (0.14 + bgp.bgHaze * 0.4));

        float2 cp = float2(uv.x * asp, sy) * (3.0 + bgp.bgScale * 0.5);
        cp += (Fbm(cp, oc) - 0.5) * (0.6 + bgp.bgNebWarp * 1.2);
        float crystal = pow(RidgedFbm(cp * 1.1, oc), 4.0);
        float cedge = saturate(smoothstep(0.60, 1.06, length((uv - 0.5) * float2(asp, 1.0))));
        pat += float3(0.82, 0.91, 1.0) * crystal * cedge * (0.16 + bgp.bgGlow * 0.35);

        {
            float2 dp = float2(uv.x * asp, sy) * (60.0 + bgp.bgScale);
            dp.y += tA * 2.0;
            float2 dcell = floor(dp);
            float dh = Hash21(dcell + 3.1);
            if (dh > 0.95) {
                float2 dj = float2(Hash21(dcell + 1.7), Hash21(dcell + 8.3));
                float dd = length(frac(dp) - dj);
                float spark = smoothstep(0.12, 0.0, dd) * (0.5 + 0.5 * sin(tA * 4.0 + dh * 30.0));
                pat += float3(0.90, 0.95, 1.0) * spark * (bgp.bgSparkle * 0.5 + 0.1);
            }
        }

        [unroll] for (int L = 0; L < 3; L++) {
            float scl = (L == 0) ? (13.0 + bgp.bgScale) : ((L == 1) ? (24.0 + bgp.bgScale) : (42.0 + bgp.bgScale * 1.6));
            float spd = (L == 0) ? 0.8 : ((L == 1) ? 1.2 : 1.8);
            float2 sfp = float2(uv.x * asp * scl, sy * scl);
            sfp.y += (bgp.bgFlow + tA) * scl * spd * 0.12;
            sfp.x += sin(sfp.y * 0.35 + (float)L * 2.0) * 0.6;
            float2 cell = floor(sfp);
            float h = Hash21(cell + (float)L * 17.3);
            if (h > 0.82) {
                float2 jit = float2(Hash21(cell + 1.3), Hash21(cell + 4.7));
                float2 d = frac(sfp) - jit;
                float flake = exp(-dot(d, d) * (26.0 + (float)L * 22.0));
                pat += float3(0.97, 0.99, 1.0) * flake * (0.95 - 0.18 * (float)L);
            }
        }

        pat *= lerp(0.70, 1.0, saturate(sy / max(hz, 0.05)));
        float2 vgp = uv - 0.5; vgp.x *= asp;
        pat *= 1.0 - smoothstep(0.5, 1.2, length(vgp)) * 0.32;
    }
    else if (bgp.bgStyle == 29) {
        int oc = (int)clamp(bgp.bgFbm, 1.0, 6.0);
        float tA = time * animSpeed;
        float3 paper = col1;
        float3 inkC = lerp(col2, float3(bgp.bgCol4R, bgp.bgCol4G, bgp.bgCol4B), 0.25);
        float sy = wsuv.y;

        float mottle = Fbm(pc * 2.2 + 3.0, 3);
        float fibre = Fbm(float2(uv.x * asp * 240.0, uv.y * 36.0), 2);
        pat = paper * (0.94 + 0.09 * mottle) * (0.985 + 0.03 * fibre);

        [unroll] for (int R = 0; R < 3; R++) {
            float fr = (float)R;
            float baseY = 0.46 + fr * 0.11 + bgp.bgOffY;
            float amp = (0.13 - fr * 0.025) * (0.5 + saturate(bgp.bgScaleY / 12.0));
            float freq = 0.9 + fr * 0.8;
            float ridge = RidgedFbm(float2(uv.x * asp * freq + fr * 9.0 + bgp.bgOffX * 2.0, fr * 3.7), oc);
            float h = baseY - ridge * amp;
            float m = smoothstep(h + 0.005, h - 0.005, sy);
            float dens = (0.34 - fr * 0.08) * (0.55 + bgp.bgNebContrast * 0.9);
            float grad = saturate(1.0 - (sy - h) * 3.2);
            pat = lerp(pat, inkC, m * dens * (0.32 + 0.68 * grad));
        }

        float2 ip = pc * (0.8 + bgp.bgScale * 0.13);
        ip += (Fbm(ip * 1.4 + 4.0 + tA * 0.05, oc) - 0.5) * (0.5 + bgp.bgNebWarp * 2.2);
        float inkF = Fbm(ip, oc);
        float thr = 0.56 - bgp.bgNebContrast * 0.18;
        float wash = smoothstep(thr, thr + 0.15, inkF);
        float dry = Fbm(float2(ip.x * 9.0, ip.y * 1.1), 3);
        wash *= smoothstep(0.22, 0.62, dry * 0.65 + 0.5);
        float rim = 1.0 - smoothstep(0.0, 0.045, abs(inkF - thr));
        float inkAmt = saturate(wash * (0.7 + bgp.bgGlow * 0.35) + rim * 0.4);
        pat = lerp(pat, inkC, inkAmt * (0.55 + bgp.bgHaze * 0.45));

        if (bgp.bgSparkle > 0.0) {
            float2 sp = pc * (13.0 + bgp.bgScale);
            float2 scell = floor(sp);
            float sh = Hash21(scell + 2.7);
            if (sh > 0.93) {
                float2 sj = float2(Hash21(scell + 1.1), Hash21(scell + 5.3));
                float sd = length(frac(sp) - sj);
                pat = lerp(pat, inkC, smoothstep(0.17, 0.02, sd) * bgp.bgSparkle * 0.85);
            }
        }
    }
    if (t >= 0.0) {
        t = saturate(t);
        if (bgp.bgSharp > 0.0) t = saturate((t - 0.5) * (1.0 + bgp.bgSharp * 12.0) + 0.5);
        float3 col5 = float3(bgp.bgCol5R, bgp.bgCol5G, bgp.bgCol5B);
        float3 col6 = float3(bgp.bgCol6R, bgp.bgCol6G, bgp.bgCol6B);
        pat = Ramp5(t, col1, col5, col3, col6, col2);
    }

    if (bgp.bgNormal > 0.0 || bgp.bgSpecular > 0.0 || bgp.bgMetallic > 0.0 || bgp.bgFresnel > 0.0 ||
        bgp.bgLightInt > 0.0 || bgp.bgReflect > 0.0 || bgp.bgClearcoat > 0.0) {
        float3 mCol4 = float3(bgp.bgCol4R, bgp.bgCol4G, bgp.bgCol4B);
        float pl = Luma(pat);
        float3 N = normalize(float3(-ddx(pl) * bgp.bgNormal * 60.0, -ddy(pl) * bgp.bgNormal * 60.0, 1.0));
        float2 vp = (baseUv - 0.5) * float2(asp, 1.0);
        float3 V = normalize(float3(-vp * 1.3, 1.0));
        float3 L = normalize(float3(bgp.bgLightX, bgp.bgLightY, max(bgp.bgLightZ, 0.05)));
        float3 H = normalize(L + V);
        float ndl = saturate(dot(N, L));
        float ndv = saturate(dot(N, V));
        float ndh = saturate(dot(N, H));

        float3 F0 = lerp(float3(0.04, 0.04, 0.04), max(pat, 0.02), bgp.bgMetallic);
        float fbase = pow(1.0 - ndv, 5.0);
        float3 F = F0 + (1.0 - F0) * fbase;

        float shin = exp2(lerp(2.0, 11.0, 1.0 - saturate(bgp.bgRoughness)));
        float3 Ha = normalize(float3(H.x * (1.0 - bgp.bgAniso * 0.85), H.y, H.z));
        float ndh_a = saturate(dot(N, Ha));
        float spec = pow(lerp(ndh, ndh_a, bgp.bgAniso), shin) * (shin * 0.06 + 1.0);
        float3 specular = F * spec * bgp.bgSpecular;

        float reflAmt = max(bgp.bgReflect, bgp.bgMetallic * 0.7);
        float3 refl = float3(0.0, 0.0, 0.0);
        if (reflAmt > 0.0) {
            float3 R = reflect(-V, N);
            float3 envc = float3(bgp.bgEnvR, bgp.bgEnvG, bgp.bgEnvB);
            float up = R.y;
            float3 env = lerp(envc * 0.04, envc * 0.95, smoothstep(-0.35, 0.4, up));
            env += envc * 1.1 * exp(-up * up * 12.0);
            env += mCol4 * pow(saturate(dot(R, L)), lerp(8.0, 500.0, saturate(bgp.bgEnvSharp))) * 3.0;
            env *= lerp(float3(1.0, 1.0, 1.0), pat + 0.15, bgp.bgMetallic);
            refl = env * F * reflAmt;
        }

        float coat = pow(ndh, 700.0) * bgp.bgClearcoat;

        float3 diff = pat * lerp(1.0, ndl, saturate(bgp.bgLightInt)) * (1.0 - 0.85 * bgp.bgMetallic);

        float3 rim = float3(pow(1.0 - ndv, 5.0 - bgp.bgMatDisp * 2.0),
                            fbase,
                            pow(1.0 - ndv, 5.0 + bgp.bgMatDisp * 2.0)) * bgp.bgFresnel;

        pat = diff + specular + refl + coat.xxx + rim * mCol4;
    }
    BgResult r; r.pat = pat; r.uv = uv; r.sc = sc;
    return r;
}

float4 PS(VSOut i) : SV_Target {
    float2 suv = float2(i.uv.x, 1.0 - i.uv.y);
    float asp = texelX > 0.0 ? texelY / texelX : 1.0;

    if (bypass != 0) {
        float2 buv = suv;
        if (flip != 0) buv.y = 1.0 - buv.y;
        float3 bc = colorTex.Sample(samp, buv).rgb;
        if (swapRB != 0) bc = bc.bgr;
        return float4(saturate(bc), 1.0);
    }

    float2 cuv = suv;
    if (flip != 0) cuv.y = 1.0 - cuv.y;
    cuv = ApplyWarp(cuv, asp);
    float2 duv = ((flip != 0) ? ApplyWarp(suv, asp) : cuv) * float2(depthUvScaleX, depthUvScaleY);

    float lin = (hasDepth != 0) ? Linearize(duv) : 0.0;

    if (debugView == 1) {
        if (hasDepth == 0) return float4(1.0, 0.0, 0.0, 1.0);
        float v = pow(saturate(lin), 0.4);
        return float4(v, v, v, 1.0);
    }
    if (debugView == 2) {
        if (hasDepth == 0) return float4(1.0, 0.0, 0.0, 1.0);
        float mb = (bgRecolor > 0.0 && bgStyle > 0)
                 ? smoothstep(bgRecolorStart, bgRecolorStart + max(bgRecolorFeather, 0.003), lin) * bgRecolor : 0.0;
        float mf = (bgFill > 0.0)
                 ? smoothstep(bgFillStart, bgFillStart + max(bgFillFeather, 0.003), lin) * bgFill : 0.0;
        float3 sc = colorTex.SampleLevel(samp, i.uv, 0).rgb;
        float g = dot(sc, float3(0.299, 0.587, 0.114)) * 0.40;
        float3 o = float3(g, g, g);
        o = lerp(o, float3(1.00, 0.15, 0.85), saturate(mb) * 0.85);
        o = lerp(o, float3(0.15, 0.95, 1.00), saturate(mf) * 0.55);
        return float4(o, 1.0);
    }

    float coc = 0.0;
    if (hasDepth != 0 && dofStrength > 0.0)
        coc = saturate((abs(lin - dofFocus) - dofRange) / max(dofRange, 0.05)) * dofStrength;

    float3 c;
    if (coc > 0.003) {
        float2 r = coc * 14.0 * float2(texelX, texelY);
        c = colorTex.Sample(samp, cuv).rgb;
        [unroll] for (int k = 0; k < 8; k++)
            c += colorTex.Sample(samp, cuv + DISK[k] * r).rgb;
        c /= 9.0;
    } else if (chroma > 0.0) {
        float2 cdv = cuv - 0.5;
        float crr = saturate(dot(cdv, cdv) * 4.0);
        float2 off = cdv * chroma * 0.03 * lerp(1.0, crr * 1.8, saturate(chromaRadial));
        c.r = colorTex.Sample(samp, cuv + off).r;
        c.g = colorTex.Sample(samp, cuv).g;
        c.b = colorTex.Sample(samp, cuv - off).b;
    } else {
        c = colorTex.Sample(samp, cuv).rgb;
    }
    if (sharpen > 0.0) {
        float2 tx = float2(texelX, 0.0), ty = float2(0.0, texelY);
        float3 blur = (colorTex.Sample(samp, cuv + tx).rgb + colorTex.Sample(samp, cuv - tx).rgb
                     + colorTex.Sample(samp, cuv + ty).rgb + colorTex.Sample(samp, cuv - ty).rgb) * 0.25;
        c += (c - blur) * (sharpen * 3.0);
    }
    if (swapRB != 0) c = c.bgr;

    if (prism > 0.0) {
        float2 pd = (cuv - 0.5) * prism * 0.03;
        float3 psum = float3(0.0, 0.0, 0.0);
        [unroll] for (int ps = 0; ps < 5; ps++) {
            float pt = ps / 4.0;
            float3 ptint = 0.5 + 0.5 * cos(6.2831853 * pt + float3(0.0, 2.094, 4.188));
            psum += colorTex.Sample(samp, cuv + pd * (pt - 0.5) * 2.0).rgb * ptint;
        }
        psum /= 2.5;
        if (swapRB != 0) psum = psum.bgr;
        c = lerp(c, psum, prism);
    }

    if (chromaClean > 0.0) {
        float2 ce = float2(texelX, texelY) * 1.5;
        float3 nb = (colorTex.Sample(samp, cuv + float2(ce.x, 0.0)).rgb + colorTex.Sample(samp, cuv - float2(ce.x, 0.0)).rgb
                   + colorTex.Sample(samp, cuv + float2(0.0, ce.y)).rgb + colorTex.Sample(samp, cuv - float2(0.0, ce.y)).rgb) * 0.25;
        if (swapRB != 0) nb = nb.bgr;
        c = lerp(c, nb * (Luma(c) / max(Luma(nb), 0.001)), chromaClean);
    }

    if (denoise > 0.0) {
        float2 de = float2(texelX, texelY) * 1.5;
        float3 dsum = c; float dw = 1.0;
        float2 doff[4] = { float2(de.x,0.0), float2(-de.x,0.0), float2(0.0,de.y), float2(0.0,-de.y) };
        [unroll] for (int dn = 0; dn < 4; dn++) {
            float3 s = colorTex.Sample(samp, cuv + doff[dn]).rgb;
            if (swapRB != 0) s = s.bgr;
            float w = exp(-dot(s - c, s - c) / max(denoiseEdge * denoiseEdge, 1e-4));
            dsum += s * w; dw += w;
        }
        c = lerp(c, dsum / dw, denoise);
    }

    if (kuwaharaAmt > 0.0) {
        float3 kw = Kuwahara(cuv);
        if (swapRB != 0) kw = kw.bgr;
        c = lerp(c, kw, kuwaharaAmt);
    }

    if (bgBlur > 0.0 || orton > 0.0 || glamour > 0.0 || clarity > 0.0 || tiltAmt > 0.0) {
        float3 fb = fullBlurTex.Sample(samp, cuv).rgb;
        if (swapRB != 0) fb = fb.bgr;
        if (clarity > 0.0) c += (c - fb) * clarity * 1.5;
        if (tiltAmt > 0.0) {
            float m = smoothstep(tiltRange, tiltRange + 0.15, abs(suv.y - tiltFocus)) * tiltAmt;
            c = lerp(c, fb, m);
        }
        if (hasDepth != 0 && bgBlur > 0.0) {
            float m = ZoneMask(zoneBgBlur, lin, bgBlurStart, 0.1) * bgBlur;
            c = lerp(c, fb, m);
        }
        if (orton > 0.0) {
            float3 sc = 1.0 - (1.0 - c) * (1.0 - fb);
            c = lerp(c, sc, orton);
        }
        if (glamour > 0.0) {
            float3 sc = 1.0 - (1.0 - c) * (1.0 - fb * 0.7);
            c = lerp(c, sc, glamour);
            c = c + glamour * glamourMist * (1.0 - c) * 0.5;
        }
    }

    if (hasDepth != 0 && bgFill > 0.0) {
        float fm = smoothstep(bgFillStart, bgFillStart + max(bgFillFeather, 0.003), lin) * bgFill;
        c = lerp(c, float3(bgFillR, bgFillG, bgFillB), fm);
    }

    float3 cGradeIn = c;
    c *= exp2(exposure);
    if (blackPoint > 0.0 || whitePoint != 1.0)
        c = saturate((c - blackPoint) / max(whitePoint - blackPoint, 1e-3));
    c.r *= 1.0 + temperature;
    c.b *= 1.0 - temperature;
    c.g *= 1.0 + tint;
    c = pow(max(c * (1.0 + gain) + lift, 0.0), 1.0 / (1.0 + gamma));
    c = (c - 0.5) * (1.0 + contrast) + 0.5;
    float luma = dot(c, float3(0.299, 0.587, 0.114));
    c = luma + (c - luma) * (1.0 + saturation);
    float vmx = max(c.r, max(c.g, c.b));
    float vmn = min(c.r, min(c.g, c.b));
    float vlum = dot(c, float3(0.299, 0.587, 0.114));
    c = lerp(float3(vlum, vlum, vlum), c, 1.0 + vibrance * (1.0 - saturate(vmx - vmn)));

    if (hueShift != 0.0) c = HueShift(c, hueShift);

    if (stAmount > 0.0) {
        float3 zpre = c;
        float l = Luma(c);
        float sw = saturate(1.0 - l / max(stBalance, 0.01));
        float hw = saturate((l - stBalance) / max(1.0 - stBalance, 0.01));
        float3 off = (float3(stShadowR, stShadowG, stShadowB) - 0.5) * sw
                   + (float3(stHighR, stHighG, stHighB) - 0.5) * hw;
        c += stAmount * 2.0 * off;
        c = lerp(zpre, c, ZoneMask(zoneSplitTone, lin, scopeSplit, scopeSoft));
    }

    if (colorBalance > 0.0) {
        float3 zpre = c;
        float l = Luma(c);
        float sw = saturate(1.0 - l * 2.0);
        float hw = saturate(l * 2.0 - 1.0);
        float mw = 1.0 - sw - hw;
        float3 off = (float3(cbShadowR, cbShadowG, cbShadowB) - 0.5) * sw
                   + (float3(cbMidR, cbMidG, cbMidB) - 0.5) * mw
                   + (float3(cbHighR, cbHighG, cbHighB) - 0.5) * hw;
        c += colorBalance * 2.0 * off;
        c = lerp(zpre, c, ZoneMask(zoneCb, lin, scopeSplit, scopeSoft));
    }

    if (tealOrange > 0.0) {
        float3 zpre = c;
        float l = Luma(c);
        float3 t = lerp(float3(toShadowR, toShadowG, toShadowB),
                        float3(toHighR, toHighG, toHighB), smoothstep(0.15, 0.85, l));
        c = lerp(c, c * t * 1.6, tealOrange);
        float gl = Luma(c);
        c = lerp(float3(gl, gl, gl), c, tealOrangePunch);
        c = lerp(zpre, c, ZoneMask(zoneTeal, lin, scopeSplit, scopeSoft));
    }

    if (bleach > 0.0) {
        float3 zpre = c;
        float l = Luma(c);
        float3 sv = float3(l, l, l);
        float3 hard = (l < 0.5) ? (2.0 * c * sv) : (1.0 - 2.0 * (1.0 - c) * (1.0 - sv));
        hard = (hard - 0.5) * bleachContrast + 0.5;
        c = lerp(c, hard, bleach);
        c = lerp(zpre, c, ZoneMask(zoneBleach, lin, scopeSplit, scopeSoft));
    }

    if (dehaze > 0.0) {
        c = (c - 0.5) * (1.0 + dehaze * 0.6) + 0.5;
        float dl = Luma(c);
        c = lerp(float3(dl, dl, dl), c, 1.0 + dehaze * 0.4);
        c = max(c - dehaze * 0.03, 0.0);
    }

    if (gradMap > 0.0) {
        float3 zpre = c;
        float gl = saturate(Luma(c));
        float3 gm = (gl < 0.5)
            ? lerp(float3(gmShadowR, gmShadowG, gmShadowB), float3(gmMidR, gmMidG, gmMidB), gl * 2.0)
            : lerp(float3(gmMidR, gmMidG, gmMidB), float3(gmHighR, gmHighG, gmHighB), (gl - 0.5) * 2.0);
        c = lerp(c, gm, gradMap);
        c = lerp(zpre, c, ZoneMask(zoneGradMap, lin, scopeSplit, scopeSoft));
    }

    if (hlRecovery > 0.0)
        c = lerp(c, c / (1.0 + max(c - 0.7, 0.0) * 2.0), hlRecovery);

    if (hasDepth != 0 && scopeMode != 0) {
        float sm = (scopeMode == 1)
            ? (1.0 - smoothstep(scopeSplit, scopeSplit + scopeSoft, lin))
            : smoothstep(scopeSplit, scopeSplit + scopeSoft, lin);
        c = lerp(cGradeIn, c, sm);
    }

    if (iridescent > 0.0) {
        float il = Luma(c);
        float phase = il * iridFreq + iridShift + (i.uv.x + i.uv.y) * 2.0;
        float3 sheen = 0.5 + 0.5 * cos(phase + float3(0.0, 2.094, 4.188));
        c = lerp(c, c * sheen * 1.6, iridescent * 0.5);
    }

    if (edgeAura > 0.0) {
        float3 zpre = c;
        float2 ew = float2(texelX, texelY) * max(edgeWidth, 1.0);
        float gx = Luma(colorTex.Sample(samp, cuv + float2(ew.x, 0.0)).rgb) - Luma(colorTex.Sample(samp, cuv - float2(ew.x, 0.0)).rgb);
        float gy = Luma(colorTex.Sample(samp, cuv + float2(0.0, ew.y)).rgb) - Luma(colorTex.Sample(samp, cuv - float2(0.0, ew.y)).rgb);
        float edge = sqrt(gx * gx + gy * gy);
        c += float3(edgeR, edgeG, edgeB) * smoothstep(edgeThreshold, edgeThreshold * 2.0, edge) * edgeAura;
        c = lerp(zpre, c, ZoneMask(zoneStylize, lin, scopeSplit, scopeSoft));
    }

    if (hasDepth != 0 && bgRecolor > 0.0 && bgStyle > 0) {
        float m = smoothstep(bgRecolorStart, bgRecolorStart + max(bgRecolorFeather, 0.003), lin) * bgRecolor;
        float3 sceneC = c;

        float band = 0.0;
        if ((edgeErode > 0.0 || edgeDespill > 0.0 || edgeWrap > 0.0) && m < 0.999) {
            float2 ex2 = float2(texelX, texelY) * (2.0 + edgeWrapWidth * 22.0);
            band = DepthCoverage(duv, ex2, lin, 0.06, 1.0);
        }
        if (edgeErode > 0.0)
            m = max(m, smoothstep(0.30, 0.80, band) * edgeErode * bgRecolor);
      if (m > 0.001 || band > 0.002) {
        float3 pat = float3(0.0, 0.0, 0.0), patB = float3(0.0, 0.0, 0.0);
        float2 uv = float2(0.0, 0.0), sc = float2(1.0, 1.0);
        int nbg = (bgBStyle > 0) ? 2 : 1;
        [loop] for (int bi = 0; bi < nbg; bi++) {
            BgResult r = EvalBackdrop(i.uv, MakeBg(bi), asp);
            if (bi == 0) { pat = r.pat; uv = r.uv; sc = r.sc; }
            else patB = r.pat;
        }

        float wB = 0.0;
        if (bgBStyle > 0) {
            float wS = SeamWeight(i.uv, lin, asp);
            if (blendMatch > 0.0) {
                float la = Luma(pat), lb = Luma(patB);
                float bandM = 1.0 - abs(wS * 2.0 - 1.0);
                float target = lerp(la, lb, wS);
                pat  *= lerp(1.0, target / max(la, 1e-3), blendMatch * bandM);
                patB *= lerp(1.0, target / max(lb, 1e-3), blendMatch * bandM);
            }
            float3 mixed = patB;
            float soft = max(blendFeather, 0.02) * 2.0;
            float lvl = saturate(blendMixLevel);
            if (blendMix == 1) {
                wB = wS * smoothstep(lvl - soft, lvl + soft, Luma(patB));
            } else if (blendMix == 2) {
                wB = wS * (1.0 - smoothstep(lvl - soft, lvl + soft, Luma(patB)));
            } else if (blendMix == 3) {
                mixed = 1.0 - (1.0 - saturate(pat)) * (1.0 - saturate(patB));
                wB = wS;
            } else if (blendMix == 4) {
                mixed = max(pat, patB);
                wB = wS;
            } else if (blendMix == 5) {
                mixed = pat * lerp(1.0, patB * 1.8, lvl);
                wB = wS;
            } else if (blendMix == 6) {
                float m = Fbm(i.uv * float2(asp, 1.0) * max(blendNoiseScale, 0.5) * 2.0 + 9.0, 4);
                wB = wS * smoothstep(lvl - soft, lvl + soft, m);
            } else {
                wB = wS;
            }
            float react = 1.0 - abs(wB * 2.0 - 1.0);
            float contrast = saturate(abs(Luma(pat) - Luma(patB)) * 1.5 + distance(pat, patB) * 0.5);
            float3 hot = (Luma(pat) > Luma(patB)) ? pat : patB;
            pat = lerp(pat, mixed, wB);
            pat += min(hot * 0.5 + 0.08, 0.45) * react * contrast * 0.2;
        }

        float3 col4 = float3(bgCol4R, bgCol4G, bgCol4B);
        float nebGate = lerp((bgStyle == 13 || bgStyle == 14) ? 1.0 : 0.0,
                             (bgBStyle == 13 || bgBStyle == 14) ? 1.0 : 0.0, wB);
        if (bgHueVar > 0.0 && nebGate > 0.0) {
            float hv = VNoise(uv * sc * 0.7 + 11.0) - 0.5;
            pat = HueShift(pat, hv * bgHueVar * 0.3 * nebGate);
        }
        if (bgHaze > 0.0) {
            float hz = Fbm(uv * sc * 0.25 + 3.7, 3);
            pat += col4 * smoothstep(0.4, 0.85, hz) * bgHaze * 0.7;
        }
        if (bgGlow > 0.0) {
            float b = smoothstep(0.35, 0.85, Luma(pat));
            pat += lerp(pat, col4, 0.5) * b * bgGlow * 1.5;
        }
        float sky15 = lerp((bgStyle == 15) ? 1.0 : 0.0, (bgBStyle == 15) ? 1.0 : 0.0, wB);
        if (bgStars > 0.0 || sky15 > 0.0) {
            float amt = (bgStars <= 0.0) ? sky15 : bgStars;
            float2 sp = float2(i.uv.x * asp, i.uv.y);
            float dens = max(bgStarDensity, 4.0);
            float3 star = float3(0.0, 0.0, 0.0);
            [unroll] for (int li = 0; li < 2; li++) {
                float2 gs = sp * dens * (li == 0 ? 1.0 : 2.3);
                float2 cell = floor(gs);
                float h = Hash21(cell + float2(li * 19.0, li * 7.0));
                float bright = frac(h * 91.7);
                float2 jit = float2(Hash21(cell + 4.3), Hash21(cell + 8.9));
                float2 fc = frac(gs) - clamp(jit, 0.18, 0.82);
                float sz = lerp(0.05, 0.34, saturate(bgStarSize)) * (0.4 + 0.6 * bright);
                float s = smoothstep(sz, 0.0, length(fc)) * step(0.86, h) * (0.4 + 0.6 * bright);
                if (bgSparkle > 0.0 && h > 0.93) {
                    float spikeX = smoothstep(sz * 5.0, 0.0, length(float2(fc.x, fc.y * 7.0)));
                    float spikeY = smoothstep(sz * 5.0, 0.0, length(float2(fc.x * 7.0, fc.y)));
                    s += (spikeX + spikeY) * bgSparkle * (0.3 + 0.7 * bright);
                }
                star += col4 * s;
            }
            pat += star * amt * 1.5;
        }
        if (bgEmbers > 0.0) {
            float2 ep = float2(i.uv.x * asp, i.uv.y) * 18.0;
            float2 ec = floor(ep);
            float eh = Hash21(ec + 55.3);
            if (eh > 0.7) {
                float2 jit = 0.3 + 0.4 * float2(Hash21(ec + 1.7), Hash21(ec + 9.1));
                float d2 = dot(frac(ep) - jit, frac(ep) - jit);
                float k = lerp(90.0, 22.0, saturate(bgEmberSize));
                pat += col4 * exp(-d2 * k) * frac(eh * 37.0) * bgEmbers * 0.6;
            }
        }
        if (bgVignette > 0.0) {
            float2 vp = i.uv - 0.5; vp.x *= asp;
            float vd = length(vp) / (0.75 * max(bgVignetteSize, 0.15));
            pat *= lerp(1.0, saturate(1.0 - vd * vd), saturate(bgVignette));
        }
        if (bgVoidCore > 0.0 || bgVoidRing > 0.0 || bgRing2 > 0.0) {
            float2 vp = i.uv - 0.5; vp.x *= asp;
            float r = length(vp) / (0.75 * max(bgVignetteSize, 0.15));
            float core = 1.0 - smoothstep(0.0, 1.0, r);
            pat *= lerp(1.0, 1.0 - saturate(bgVoidCore), core);
            float rw = 6.0 / max(bgRingWidth, 0.15);
            float dr = bgDisperse * 0.05;
            float3 ring = float3(exp(-pow((r * (1.0 + dr) - 1.0) * rw, 2.0)),
                                 exp(-pow((r - 1.0) * rw, 2.0)),
                                 exp(-pow((r * (1.0 - dr) - 1.0) * rw, 2.0)));
            pat += ring * col4 * bgVoidRing * 1.5;
            if (bgRing2 > 0.0)
                pat += col4 * exp(-pow((r - 1.7) * rw * 0.5, 2.0)) * bgRing2;
        }
        if (bgBright != 0.0) pat *= (1.0 + bgBright);

        if (bgCausticAmt > 0.0 || bgShafts > 0.0 || bgBubbles > 0.0) {
            float surf = saturate(1.0 - i.uv.y);
            float2 au = float2(i.uv.x * asp, i.uv.y);
            if (bgCausticAmt > 0.0) {
                float2 cp = uv * sc * 2.5;
                float2 wct = float2(Fbm(cp + 1.3, 3), Fbm(cp + 7.8, 3));
                cp += (wct - 0.5) * (1.0 + bgNebWarp * 2.0);
                float ca = pow(RidgedFbm(cp, 4), 3.5);
                pat += col4 * ca * bgCausticAmt * (0.3 + 0.7 * surf) * 1.3;
            }
            if (bgShafts > 0.0) {
                float beams = 0.5 + 0.5 * sin(i.uv.x * (7.0 + sc.x) + Fbm(float2(i.uv.x * 2.5, 0.7), 3) * 5.0);
                pat += col4 * pow(beams, 3.0) * surf * surf * bgShafts * 0.6;
            }
            if (bgBubbles > 0.0) {
                float2 bp = au * 11.0;
                float2 bc = floor(bp);
                float bh = Hash21(bc + 21.7);
                if (bh > 0.72) {
                    float2 jit = 0.3 + 0.4 * float2(Hash21(bc + 2.3), Hash21(bc + 8.1));
                    float2 bl = frac(bp) - jit;
                    float rad = lerp(0.10, 0.24, frac(bh * 41.0)) * (0.5 + saturate(bgEmberSize));
                    float ring = smoothstep(0.05, 0.0, abs(length(bl) - rad));
                    float hi = smoothstep(rad * 0.5, 0.0, length(bl - float2(-rad * 0.35, rad * 0.35)));
                    pat += (ring + hi * 0.6) * float3(0.72, 0.86, 1.0) * bgBubbles * 0.8;
                }
            }
        }

        if (bgGrain > 0.0) {
            float gn = frac(sin(dot(i.uv, float2(12.9898, 78.233))) * 43758.5453);
            pat += (gn - 0.5) * bgGrain * 0.3;
        }
        if (backdropLightAmt != 0.0) {
            float2 lp = i.uv - float2(backdropLightX, backdropLightY); lp.x *= asp;
            float ld = length(lp) / max(backdropLightSize, 0.05);
            float fall = exp(-ld * ld * 1.4);
            float la = saturate(abs(backdropLightAmt)) * sign(backdropLightAmt);
            pat *= lerp(1.0 - max(la, 0.0) * 0.8 + max(-la, 0.0) * 0.5, 1.0 + max(la, 0.0) * 0.55, fall);
        }

        if (edgeDespill > 0.0 && band > 0.002 && m < 0.999) {
            float2 tx2 = float2(texelX, texelY);
            float2 gd = float2(LinearizeL(duv + float2(tx2.x, 0.0)) - LinearizeL(duv - float2(tx2.x, 0.0)),
                               LinearizeL(duv + float2(0.0, tx2.y)) - LinearizeL(duv - float2(0.0, tx2.y)));
            if (dot(gd, gd) > 1e-12) {
                gd = normalize(gd);
                float3 inward = colorTex.SampleLevel(samp, cuv - gd * tx2 * 3.0, 0).rgb;
                if (swapRB != 0) inward = inward.bgr;
                c = lerp(c, inward, saturate(band * 1.6) * edgeDespill * (1.0 - m));
            }
        }

        c = lerp(c, pat, m);

        if (edgeWrap > 0.0) {
            float w = saturate(band * 1.4) * (1.0 - m) * edgeWrap;
            if (w > 0.001) c = 1.0 - (1.0 - saturate(c)) * (1.0 - saturate(pat) * w);
        }
        if (bgPad0 > 0.0) {
            float3 vfxGlow = max(sceneC - 0.30, 0.0);
            c += m * vfxGlow * bgPad0 * 1.3;
        }
      }
    }

    if (hasDepth != 0 && groundShadow > 0.0) {
        float3 zpre = c;
        float notSubject = smoothstep(bgRecolorStart, bgRecolorStart + max(bgRecolorFeather, 0.003), lin);
        float2 sp = i.uv - float2(0.5 + groundShadowX, groundShadowY);
        sp.x *= asp;
        float2 hs = float2(max(groundShadowW, 0.02), max(groundShadowH, 0.01));
        float ed = length(sp / hs);
        float soft = 0.15 + groundRipple;
        float shadow = smoothstep(1.0, max(1.0 - soft, 0.02), ed) * groundShadow * notSubject;
        c *= lerp(float3(1.0, 1.0, 1.0), float3(groundTintR, groundTintG, groundTintB), shadow);
        c = lerp(zpre, c, ZoneMask(zoneGround, lin, scopeSplit, scopeSoft));
    }

    if (hasDepth != 0 && shadowAmount > 0.0) {
        float notSubj = ZoneMask(zoneShadow, lin, shadowDepth, 0.04);
        if (notSubj > 0.001) {
            float2 tx = float2(texelX, texelY);
            float2 srad = tx * (3.0 + shadowSpread * 70.0);
            float2 soff = float2(shadowOffsetX, -shadowOffsetY) * tx * 70.0;
            float cov = DepthCoverage(duv + soff, srad, lin, 0.015, -1.0);
            float sh = pow(saturate(cov), lerp(0.55, 2.4, saturate(shadowSoftness)));
            if (shadowContact > 0.0) {
                float cc = DepthCoverage(duv + soff * 0.35, srad * 0.28, lin, 0.010, -1.0);
                sh = saturate(sh + pow(saturate(cc), 0.8) * shadowContact);
            }
            c = lerp(c, c * float3(shadowR, shadowG, shadowB), saturate(sh * shadowAmount) * notSubj);
        }
    }

    if (hasDepth != 0 && subjectPop > 0.0) {
        float3 zpre = c;
        float sp = (1.0 - smoothstep(0.08, 0.13, lin)) * subjectPop;
        c = (c - 0.5) * (1.0 + sp * 0.4) + 0.5;
        float spl = Luma(c);
        c = lerp(float3(spl, spl, spl), c, 1.0 + sp * 0.5);
        c = lerp(zpre, c, ZoneMask(zoneRim, lin, scopeSplit, scopeSoft));
    }

    if (hasDepth != 0 && wetAmount > 0.0) {
        float subj = ZoneMask(zoneWet, lin, wetDepth, 0.08) * wetAmount;
        float2 we = float2(texelX, texelY) * 2.0;
        float lL = Luma(colorTex.Sample(samp, cuv - float2(we.x, 0.0)).rgb);
        float lR = Luma(colorTex.Sample(samp, cuv + float2(we.x, 0.0)).rgb);
        float lD = Luma(colorTex.Sample(samp, cuv - float2(0.0, we.y)).rgb);
        float lU = Luma(colorTex.Sample(samp, cuv + float2(0.0, we.y)).rgb);
        float lp = (Luma(c) + lL + lR + lD + lU) * 0.2;
        float relief = lerp(20.0, 8.0, saturate(wetRough));
        float3 N = normalize(float3((lL - lR) * relief, (lD - lU) * relief, 1.0));
        float3 V = float3(0.0, 0.0, 1.0);
        float3 H = normalize(normalize(float3(wetLightX, wetLightY, 0.75)) + V);
        float shin = lerp(120.0, 18.0, saturate(wetRough));
        float lit = smoothstep(0.25, 0.85, lp);

        float glint = pow(saturate(dot(N, H)), shin) * (0.35 + 0.65 * lit);
        float hiFollow = pow(saturate((lp - 0.55) * 2.2), 2.0) * wetHighlight * 1.4;
        float fres = pow(1.0 - saturate(N.z), 3.0) * wetFresnel * (0.4 + 0.6 * lit);
        float sheen = (glint * wetShine + hiFollow + fres);

        float3 wc = c;
        float df = lerp(1.0, lerp(0.52, 0.80, smoothstep(0.05, 0.6, lp)), wetDeepen);
        wc *= df;
        float wl = Luma(wc);
        wc = lerp(float3(wl, wl, wl), wc, 1.0 + wetDeepen * 0.35);
        c = lerp(c, wc, subj);

        if (wetDroplets > 0.0) {
            float dens = lerp(42.0, 150.0, saturate(wetDropDensity));
            float stretch = 1.0 + wetDropTrail * 4.0;
            float2 dp = float2(i.uv.x * asp, i.uv.y / stretch) * dens;
            float2 dcell = floor(dp);
            float gate = lerp(0.86, 0.5, saturate(wetDroplets));
            if (Hash21(dcell + 2.3) > gate) {
                float2 f = frac(dp) - float2(Hash21(dcell + 1.1), Hash21(dcell + 5.7));
                f.y *= stretch;
                float rad = lerp(3.0, 1.4, saturate(wetDropSize));
                float bead = smoothstep(1.0, 0.0, length(f) * rad);
                float hi = pow(saturate(1.0 - length(f * rad + 0.4)), 3.0);
                c += (hi * 1.6 - bead * 0.12) * bead * wetDroplets * subj;
            }
        }

        c += sheen * subj;
    }

    if (hasDepth != 0 && bgPushStrength > 0.0) {
        float b = ZoneMask(zoneBgPush, lin, bgPushStart, max(1.0 - bgPushStart, 0.05)) * bgPushStrength;
        float g = dot(c, float3(0.299, 0.587, 0.114));
        c = lerp(c, float3(g, g, g) * 0.7, b);
    }

    if (hasDepth != 0 && fogStrength > 0.0) {
        float t = max(0.0, lin - fogStart);
        float f = saturate(1.0 - exp(-fogStrength * 12.0 * t));
        c = lerp(c, float3(fogColorR, fogColorG, fogColorB), f);
    }

    if (hasDepth != 0 && rimStrength > 0.0) {
        float2 ro = float2(texelX, texelY) * max(rimWidth, 1.0);
        float behind = max(max(Linearize(duv + float2(ro.x, 0.0)), Linearize(duv - float2(ro.x, 0.0))),
                           max(Linearize(duv + float2(0.0, ro.y)), Linearize(duv - float2(0.0, ro.y)))) - lin;
        float rim = smoothstep(rimThreshold, rimThreshold * 3.0, behind);
        float3 rcol = lerp(float3(rimR, rimG, rimB), float3(rim2R, rim2G, rim2B),
                           RimSide(i.uv, asp) * saturate(rimSplit));
        c += rcol * rim * rimStrength;
    }

    if (bloomAmount > 0.0) {
        float3 bl = bloomTex.Sample(samp, cuv).rgb; if (swapRB != 0) bl = bl.bgr;
        c += bl * bloomAmount;
    }
    if (halation > 0.0) {
        float3 hl = bloomTex.Sample(samp, cuv).rgb; if (swapRB != 0) hl = hl.bgr;
        c += hl * float3(halationR, halationG, halationB) * halation;
    }

    if (godrayAmount > 0.0) {
        float3 gr = godrayTex.Sample(samp, cuv).rgb; if (swapRB != 0) gr = gr.bgr;
        c += gr * float3(godrayR, godrayG, godrayB) * godrayAmount;
    }

    if (anamAmount > 0.0) {
        float3 an = anamTex.Sample(samp, cuv).rgb; if (swapRB != 0) an = an.bgr;
        c += an * float3(anamR, anamG, anamB) * anamAmount;
    }

    if (hasDepth != 0 && haloAmount > 0.0) {
        float3 zpre = c;
        float bm = haloTex.Sample(samp, cuv).r;
        float bg = smoothstep(haloSplit, haloSplit + 0.05, lin);
        c += float3(haloR, haloG, haloB) * (bm * bg * haloAmount);
        c = lerp(zpre, c, ZoneMask(zoneHalo, lin, scopeSplit, scopeSoft));
    }

    if (washAmount > 0.0) {
        float wd = length(i.uv - float2(washX, washY));
        c += float3(washR, washG, washB) * saturate(1.0 - wd * 1.5) * washAmount * 0.6;
    }

    if (causticsAmt > 0.0) {
        float2 cp = i.uv * causticsScale;
        float ca = sin(cp.x * 1.3 + sin(cp.y * 0.9)) * cos(cp.y * 1.1 + cos(cp.x * 0.7));
        ca = pow(saturate(ca * 0.5 + 0.5), 3.0);
        c += float3(causticsR, causticsG, causticsB) * ca * causticsAmt * 0.5;
    }

    if (leakAmt > 0.0) {
        float2 ld = i.uv - 0.5;
        float proj = ld.x * cos(leakAngle) + ld.y * sin(leakAngle);
        float leak = pow(saturate(proj + 0.5), 2.0) * leakAmt;
        c += float3(leakR, leakG, leakB) * leak;
    }

    if (hasDepth != 0 && (skinWarmth > 0.0 || skinFlush > 0.0)) {
        float subj = ZoneMask(zoneSkin, lin, wetDepth, 0.10);
        float lum = Luma(c);
        float3 tint = float3(skinTintR, skinTintG, skinTintB);
        float mid = smoothstep(0.18, 0.5, lum) * (1.0 - smoothstep(0.75, 1.0, lum));
        c += tint * skinWarmth * mid * subj * 0.5;
        c = lerp(c, c * lerp(float3(1.0, 1.0, 1.0), tint * 1.4, 0.5), skinFlush * subj * 0.6);
    }

    if (hasDepth != 0 && beautyAmount > 0.0) {
        float subj = ZoneMask(zoneBeauty, lin, wetDepth, 0.10);
        float2 gr = float2(texelX, texelY) * (2.0 + beautyRadius * 8.0);
        float3 soft = colorTex.Sample(samp, cuv).rgb * 0.25;
        soft += colorTex.Sample(samp, cuv + float2(gr.x, 0.0)).rgb * 0.125;
        soft += colorTex.Sample(samp, cuv - float2(gr.x, 0.0)).rgb * 0.125;
        soft += colorTex.Sample(samp, cuv + float2(0.0, gr.y)).rgb * 0.125;
        soft += colorTex.Sample(samp, cuv - float2(0.0, gr.y)).rgb * 0.125;
        soft += colorTex.Sample(samp, cuv + gr).rgb * 0.0625;
        soft += colorTex.Sample(samp, cuv - gr).rgb * 0.0625;
        soft += colorTex.Sample(samp, cuv + float2(gr.x, -gr.y)).rgb * 0.0625;
        soft += colorTex.Sample(samp, cuv + float2(-gr.x, gr.y)).rgb * 0.0625;
        if (swapRB != 0) soft = soft.bgr;
        float3 glow = 1.0 - (1.0 - saturate(c)) * (1.0 - saturate(soft));
        c = lerp(c, glow, beautyAmount * subj);
        float hb = smoothstep(0.6, 1.0, Luma(soft));
        c += soft * hb * beautyGlow * beautyAmount * subj * 0.5;
    }

    if (hasDepth != 0 && backlightAmount > 0.0) {
        float subj = ZoneMask(zoneBacklight, lin, wetDepth, 0.10);
        if (subj > 0.001) {
            float2 bo = float2(texelX, texelY) * (4.0 + backlightWidth * 26.0);
            float glow = DepthCoverage(duv, bo, lin, 0.06, 1.0);
            glow = smoothstep(0.02, 0.75, glow);
            float3 bcol = lerp(float3(backlightR, backlightG, backlightB),
                              float3(backlight2R, backlight2G, backlight2B),
                              RimSide(i.uv, asp) * saturate(rimSplit));
            c += bcol * glow * subj * backlightAmount * 2.0;
        }
    }

    if (goboAmount > 0.0) {
        float3 zpre = c;
        float2 gp = float2(i.uv.x * asp, i.uv.y);
        float gca = cos(goboAngle), gsa = sin(goboAngle);
        gp = float2(gp.x * gca - gp.y * gsa, gp.x * gsa + gp.y * gca) * max(goboScale, 0.5);
        float gsoft = max(goboSoft, 0.02);
        float lit;
        if (goboPattern == 0) lit = smoothstep(0.5 - gsoft, 0.5 + gsoft, frac(gp.y));
        else if (goboPattern == 1) { float2 bd = min(frac(gp), 1.0 - frac(gp)); lit = 1.0 - smoothstep(0.10 + gsoft, 0.0, min(bd.x, bd.y)); }
        else if (goboPattern == 2) lit = smoothstep(0.28, 0.5, Voronoi(gp));
        else lit = smoothstep(0.38, 0.68, Fbm(gp * 0.7, 4));
        c *= lerp(1.0, lerp(0.28, 1.0, lit), saturate(goboAmount));
        c = lerp(zpre, c, ZoneMask(zoneGobo, lin, scopeSplit, scopeSoft));
    }

    if (spotAmount > 0.0) {
        float3 zpre = c;
        float2 sp = i.uv - float2(spotX, spotY); sp.x *= asp;
        float sca = cos(spotAngle), ssa = sin(spotAngle);
        sp = float2(sp.x * sca - sp.y * ssa, sp.x * ssa + sp.y * sca);
        sp.y *= max(spotEllipse, 0.1);
        float sd = length(sp) / max(spotRadius, 0.05);
        float pool = 1.0 - smoothstep(1.0 - max(spotSoft, 0.02), 1.0 + max(spotSoft, 0.02), sd);
        c *= lerp(1.0 - saturate(spotAmount), 1.0, pool);
        c = lerp(c, c * float3(1.06, 0.99, 0.88), spotWarm * pool);
        c = lerp(zpre, c, ZoneMask(zoneSpot, lin, scopeSplit, scopeSoft));
    }

    if (filmSat > 0.0) {
        float l = Luma(c);
        c = lerp(c, float3(l, l, l), smoothstep(0.55, 1.15, l) * filmSat * 0.6);
    }
    if (filmRolloff > 0.0) {
        float k = lerp(1.0, 0.42, saturate(filmRolloff));
        float3 hi = max(c - k, 0.0);
        float3 comp = (1.0 - k) * (1.0 - exp(-hi / max(1.0 - k, 1e-3)));
        c = min(c, k) + lerp(hi, comp, saturate(filmRolloff));
    }
    if (filmToe > 0.0)
        c += filmToe * 0.045 * (1.0 - smoothstep(0.0, 0.28, Luma(c)));

    if (lensVig > 0.0 || lensCornerSoft > 0.0) {
        float2 lv = i.uv - 0.5; lv.x *= asp;
        float r2 = saturate(dot(lv, lv) * 2.6);
        if (lensVig > 0.0) {
            float fall = 1.0 - lensVig * (r2 * r2 * 0.85 + r2 * 0.25);
            c *= saturate(fall);
            float lc = Luma(c);
            c = lerp(c, float3(lc, lc, lc), r2 * r2 * lensVig * 0.25);
        }
        if (lensCornerSoft > 0.0) {
            float amt = r2 * r2 * lensCornerSoft;
            if (amt > 0.002) {
                float2 sx = float2(texelX, 0.0) * 1.5, sy = float2(0.0, texelY) * 1.5;
                float3 rw = colorTex.SampleLevel(samp, cuv, 0).rgb;
                float3 rb = (colorTex.SampleLevel(samp, cuv + sx, 0).rgb + colorTex.SampleLevel(samp, cuv - sx, 0).rgb
                           + colorTex.SampleLevel(samp, cuv + sy, 0).rgb + colorTex.SampleLevel(samp, cuv - sy, 0).rgb) * 0.25;
                if (swapRB != 0) { rw = rw.bgr; rb = rb.bgr; }
                c -= (rw - rb) * saturate(amt) * 1.6;
            }
        }
    }

    float2 dv = i.uv - 0.5;
    c *= saturate(1.0 - vignette * 2.0 * dot(dv, dv));

    if (grain > 0.0) {
        float n = frac(sin(dot(i.uv, float2(12.9898, 78.233))) * 43758.5453);
        c += (n - 0.5) * grain * 0.15;
    }
    if (uwTint > 0.0 || uwCaustic > 0.0 || uwMotes > 0.0 || uwShafts > 0.0 || uwFog > 0.0) {
        float3 zpre = c;
        float2 uvv = i.uv;
        float2 auw = float2(uvv.x * asp, uvv.y);
        float3 tint = float3(uwTintR, uwTintG, uwTintB);
        if (uwFog > 0.0 && hasDepth != 0)
            c = lerp(c, tint, saturate(lin) * uwFog);
        if (uwTint > 0.0) {
            float gl = Luma(c);
            c = lerp(c, tint * (0.4 + gl), uwTint * 0.6);
        }
        if (uwCaustic > 0.0) {
            float2 cp = auw * 5.0;
            float2 w = float2(Fbm(cp + 1.3, 3), Fbm(cp + 7.8, 3));
            cp += (w - 0.5) * 1.5;
            float ca = pow(RidgedFbm(cp, 4), 3.0);
            c += ca * uwCaustic * (0.25 + 0.75 * Luma(c)) * float3(0.7, 0.95, 1.0);
        }
        if (uwShafts > 0.0) {
            float beams = 0.5 + 0.5 * sin(uvv.x * 10.0 + Fbm(float2(uvv.x * 2.5, 0.7), 3) * 5.0);
            c += pow(beams, 3.0) * saturate(1.0 - uvv.y) * uwShafts * 0.25 * float3(0.8, 0.95, 1.0);
        }
        if (uwMotes > 0.0) {
            float2 mp = auw * 40.0;
            float2 mc = floor(mp);
            float mh = Hash21(mc + 12.3);
            if (mh > 0.93) {
                float2 ml = frac(mp) - 0.5 - (float2(Hash21(mc + 1.0), Hash21(mc + 2.0)) - 0.5) * 0.6;
                c += exp(-dot(ml, ml) * 30.0) * uwMotes * 0.4 * float3(0.85, 0.95, 1.0);
            }
        }
        c = lerp(zpre, c, ZoneMask(zoneUnderwater, lin, scopeSplit, scopeSoft));
    }

    if (vhsStatic > 0.0 || vhsScan > 0.0 || vhsDropout > 0.0 || vhsRoll > 0.0 || vhsDesat > 0.0 || vhsVignette > 0.0) {
        float3 zpre = c;
        float2 uvv = i.uv;
        if (vhsDesat > 0.0) {
            float gl = Luma(c);
            c = lerp(c, float3(gl, gl, gl) * float3(0.94, 1.04, 0.97), vhsDesat);
        }
        if (vhsRoll > 0.0) {
            float band = exp(-pow((uvv.y - frac(vhsRollPos)) * 7.0, 2.0));
            c += band * vhsRoll * 0.35;
            c = lerp(c, float3(1.0, 1.0, 1.0), band * vhsRoll * 0.12);
        }
        if (vhsDropout > 0.0) {
            float row = floor(uvv.y * 240.0);
            float rh = frac(sin(row * 91.17) * 43758.5453);
            float streak = step(1.0 - vhsDropout * 0.15, rh);
            float sn = frac(sin(dot(float2(uvv.x, row), float2(12.99, 78.23))) * 43758.5453);
            c = lerp(c, float3(sn, sn, sn), streak * 0.85);
        }
        if (vhsScan > 0.0) {
            float lines = 0.5 + 0.5 * sin(uvv.y * max(vhsScanCount, 60.0) * 3.14159);
            c *= 1.0 - vhsScan * 0.6 * lines;
        }
        if (vhsStatic > 0.0) {
            float sn = frac(sin(dot(uvv * float2(1920.0, 1080.0), float2(12.9898, 78.233))) * 43758.5453);
            float patch = 0.55 + 0.45 * VNoise(uvv * 6.0);
            c = lerp(c, float3(sn, sn, sn), saturate(vhsStatic * patch));
        }
        if (vhsVignette > 0.0) {
            float2 dvv = uvv - 0.5;
            c *= saturate(1.0 - vhsVignette * 2.2 * dot(dvv, dvv));
        }
        c = lerp(zpre, c, ZoneMask(zoneVhs, lin, scopeSplit, scopeSoft));
    }

    if (hudIntensity > 0.0) {
        float3 hud = float3(hudR, hudG, hudB);
        float2 P = i.uv - 0.5; P.x *= asp;
        float2 aP = abs(P);
        float sc = 0.7 + hudScale * 0.6;
        float t = 0.0018;
        float ink = 0.0;
        float frameX = 0.46 * asp, frameY = 0.46;

        {
            float2 bc = float2(frameX, frameY);
            float bl = 0.05 * sc;
            float fr = max(smoothstep(t, 0.0, SegSD(aP, float2(bc.x - bl, bc.y), bc)),
                           smoothstep(t, 0.0, SegSD(aP, float2(bc.x, bc.y - bl), bc)));
            float sy = -0.40;
            float per = 0.05 * sc;
            float idx = floor(P.x / per + 0.5);
            float major = (fmod(abs(idx), 5.0) < 0.5) ? 1.0 : 0.0;
            float tl = lerp(0.012, 0.024, major) * sc;
            float ly = P.y - sy;
            float within = step(-0.001, ly) * step(ly, tl) * step(abs(P.x), 0.36);
            fr = max(fr, smoothstep(t, 0.0, abs(P.x - idx * per)) * within);
            fr = max(fr, smoothstep(t, 0.0, SegSD(P, float2(-0.018, sy - 0.03), float2(0.0, sy - 0.006))));
            fr = max(fr, smoothstep(t, 0.0, SegSD(P, float2(0.018, sy - 0.03), float2(0.0, sy - 0.006))));
            [unroll] for (int di = 0; di < 4; di++) {
                float len = 0.045 + 0.03 * frac(sin((float)di * 12.7) * 43.0);
                float2 a = float2(-0.42, -0.34) + float2(0.0, (float)di * 0.022);
                fr = max(fr, smoothstep(t, 0.0, SegSD(P, a, a + float2(len * sc, 0.0))));
            }
            ink = max(ink, fr * hudFrame);
        }

        if (hudReticle > 0.0) {
            float rr = 0.05 * sc;
            float rd = length(P);
            float ring = smoothstep(t * 1.3, 0.0, abs(rd - rr));
            float dotC = smoothstep(t * 1.6, 0.0, rd - t);
            float tickV = smoothstep(t, 0.0, SegSD(float2(P.x, abs(P.y)), float2(0.0, rr * 1.25), float2(0.0, rr * 1.75)));
            float tickH = smoothstep(t, 0.0, SegSD(float2(abs(P.x), P.y), float2(rr * 1.25, 0.0), float2(rr * 1.75, 0.0)));
            float2 lb = float2(rr * 2.1, rr * 2.1); float bl2 = rr * 0.7;
            float box = max(smoothstep(t, 0.0, SegSD(aP, float2(lb.x - bl2, lb.y), lb)),
                            smoothstep(t, 0.0, SegSD(aP, float2(lb.x, lb.y - bl2), lb)));
            ink = max(ink, (ring + dotC + tickV + tickH + box) * hudReticle);
        }

        if (hudRadar > 0.0) {
            float2 rc = P - float2(-0.40, 0.34);
            float rd2 = length(rc);
            float radR = 0.09 * sc;
            if (rd2 < radR * 1.15) {
                float g = smoothstep(t, 0.0, abs(rd2 - radR));
                g = max(g, smoothstep(t, 0.0, abs(rd2 - radR * 0.6)) * 0.7);
                g = max(g, smoothstep(t, 0.0, abs(rc.x)) * step(rd2, radR) * 0.6);
                g = max(g, smoothstep(t, 0.0, abs(rc.y)) * step(rd2, radR) * 0.6);
                float da = frac((atan2(rc.y, rc.x) - time * 1.6) / 6.2831853);
                g = max(g, (1.0 - da) * step(rd2, radR) * 0.5);
                ink = max(ink, g * hudRadar);
            }
        }

        if (hudHex > 0.0) {
            float ms = 26.0 / sc;
            float m1 = abs(frac(P.x * ms) - 0.5);
            float m2 = abs(frac((P.x * 0.5 + P.y * 0.866) * ms) - 0.5);
            float m3 = abs(frac((P.x * 0.5 - P.y * 0.866) * ms) - 0.5);
            ink = max(ink, smoothstep(0.06, 0.0, min(min(m1, m2), m3)) * hudHex * 0.25);
        }

        c += hud * ink * hudIntensity * 1.3;
        if (hudScanline > 0.0) c *= 1.0 - hudScanline * 0.12 * (0.5 + 0.5 * sin(i.uv.y * 900.0));
        if (hudFlicker > 0.0)  c *= 1.0 - hudFlicker * 0.05 * (0.5 + 0.5 * sin(time * 47.0 + i.uv.y * 3.0));
        c = lerp(c, c * 0.35 + hud * 0.015, smoothstep(0.42, 0.72, length(P)) * hudChroma);
    }

    [loop] for (int Li = 0; Li < 8; Li++) {
        int bi = Li * 5;
        float4 ea = elem[bi + 0], eb = elem[bi + 1], ecol = elem[bi + 2], ef = elem[bi + 3], eg = elem[bi + 4];
        int etype = (int)(ea.x + 0.5);
        float einten = ecol.w;
        if (etype == 0 || abs(einten) <= 0.001) continue;
        float emask = (ef.y > 0.5 || hasDepth == 0) ? 1.0 : smoothstep(bgRecolorStart, bgRecolorStart + max(bgRecolorFeather, 0.003), lin);
        if (emask <= 0.001) continue;
        float2 Pe = i.uv - float2(0.5 + ea.y, 0.5 + ea.z); Pe.x *= asp;
        float erot = eb.y + time * eb.z;
        float ecs = cos(erot), esn = sin(erot);
        Pe = float2(Pe.x * ecs - Pe.y * esn, Pe.x * esn + Pe.y * ecs);
        float er = max(ea.w, 0.008), eh = max(eb.x, 0.006), et = max(eb.w, 0.002);
        float esides = clamp(ef.z, 3.0, 12.0), efill = ef.x;
        float esy = (ef.w > 0.001) ? ef.w : er;
        float2 aPe = abs(Pe);
        float2 Pr = float2(Pe.x, Pe.y * (er / max(esy, 0.008)));
        float2 aPr = abs(Pr);
        float cov = 0.0;
        float sd = 1e9;
        if (etype == 18) {
            float2 dims = MemeDims(Li);
            float ia = (dims.y > 0.5) ? dims.x / dims.y : 1.0;
            float ehh = (ef.w > 0.001) ? esy : er / max(ia, 0.01);
            float2 iuv = float2(Pe.x / er, -Pe.y / ehh) * 0.5 + 0.5;
            if (iuv.x >= 0.0 && iuv.x <= 1.0 && iuv.y >= 0.0 && iuv.y <= 1.0) {
                float4 img = SampleMeme(Li, iuv);
                c = lerp(c, img.rgb * ecol.rgb, img.a * saturate(einten) * emask);
            }
            continue;
        }
        if (etype == 1) { sd = length(Pr) - er; cov = efill > 0.5 ? smoothstep(er, er * 0.98, length(Pr)) : smoothstep(et, 0.0, abs(sd)); }
        else if (etype == 2) { sd = length(Pr) - er; cov = smoothstep(er, er * 0.98, length(Pr)); }
        else if (etype == 3) { sd = NgonSD(Pr, er, esides); cov = efill > 0.5 ? smoothstep(0.004, -0.004, sd) : smoothstep(et, 0.0, abs(sd)); }
        else if (etype == 4) { float sg = 6.2831853 / esides; float k = atan2(Pr.y, Pr.x); k = k - sg * floor(k / sg + 0.5); sd = length(Pr) - lerp(er, er * 0.45, abs(k) / (sg * 0.5)); cov = efill > 0.5 ? smoothstep(0.004, -0.004, sd) : smoothstep(et, 0.0, abs(sd)); }
        else if (etype == 5) { float arm = er * 0.28; float2 q1 = aPr - float2(er, arm); float2 q2 = aPr - float2(arm, er); sd = min(length(max(q1, 0.0)) + min(max(q1.x, q1.y), 0.0), length(max(q2, 0.0)) + min(max(q2.x, q2.y), 0.0)); cov = efill > 0.5 ? smoothstep(0.004, -0.004, sd) : smoothstep(et, 0.0, abs(sd)); }
        else if (etype == 6) { float2 q = aPe - float2(er, esy); sd = length(max(q, 0.0)) + min(max(q.x, q.y), 0.0); cov = efill > 0.5 ? smoothstep(0.004, -0.004, sd) : smoothstep(et, 0.0, abs(sd)); }
        else if (etype == 7) { float ang = atan2(Pr.x, -Pr.y); float av = smoothstep(3.1415927 * saturate(esides / 12.0) + 0.06, 3.1415927 * saturate(esides / 12.0) - 0.02, abs(ang)); sd = abs(length(Pr) - er) + (1.0 - av) * 1.0; cov = (efill > 0.5 ? smoothstep(er, er * 0.97, length(Pr)) : smoothstep(et, 0.0, abs(length(Pr) - er))) * av; }
        else if (etype == 8) { sd = SegSD(Pe, float2(-er, 0.0), float2(er, 0.0)); cov = smoothstep(et, 0.0, sd); }
        else if (etype == 9) { float bl = min(0.055, er * 0.5); float2 bc = float2(er * asp, eh); cov = max(smoothstep(et, 0.0, SegSD(aPe, float2(bc.x - bl, bc.y), bc)), smoothstep(et, 0.0, SegSD(aPe, float2(bc.x, bc.y - bl), bc))); }
        else if (etype == 10) { float rd = length(Pr); float ring = smoothstep(et * 1.3, 0.0, abs(rd - er)); float dotc = smoothstep(et * 1.6, 0.0, rd - et); float tv = smoothstep(et, 0.0, SegSD(float2(Pr.x, abs(Pr.y)), float2(0.0, er * 1.25), float2(0.0, er * 1.7))); float th2 = smoothstep(et, 0.0, SegSD(float2(aPr.x, Pr.y), float2(er * 1.25, 0.0), float2(er * 1.7, 0.0))); cov = ring + dotc + tv + th2; }
        else if (etype == 11) { float rd = length(Pr); if (rd < er * 1.12) { cov = smoothstep(et, 0.0, abs(rd - er)); cov = max(cov, smoothstep(et, 0.0, abs(rd - er * 0.6)) * 0.7); cov = max(cov, smoothstep(et, 0.0, abs(Pr.x)) * step(rd, er) * 0.6); cov = max(cov, smoothstep(et, 0.0, abs(Pr.y)) * step(rd, er) * 0.6); float da = frac((atan2(Pr.y, Pr.x) - time * 1.6) / 6.2831853); cov = max(cov, (1.0 - da) * step(rd, er) * 0.5); } }
        else if (etype == 12) { float per = max(eh, 0.015); float idx = floor(Pe.x / per + 0.5); float mj = (fmod(abs(idx), 5.0) < 0.5) ? 1.0 : 0.0; float tl = lerp(0.012, 0.024, mj); float within = step(0.0, Pe.y) * step(Pe.y, tl) * step(abs(Pe.x), er); cov = smoothstep(et, 0.0, abs(Pe.x - idx * per)) * within; cov = max(cov, smoothstep(et, 0.0, SegSD(Pe, float2(-0.016, -0.03), float2(0.0, -0.008)))); cov = max(cov, smoothstep(et, 0.0, SegSD(Pe, float2(0.016, -0.03), float2(0.0, -0.008)))); }
        else if (etype == 13) { [unroll] for (int tb = 0; tb < 4; tb++) { float ln = er * (0.5 + 0.5 * frac(sin((float)tb * 12.7) * 43.0)); float2 a2 = float2(0.0, (float)tb * 0.022); cov = max(cov, smoothstep(et, 0.0, SegSD(Pe, a2, a2 + float2(ln, 0.0)))); } }
        else if (etype == 14) { float2 Pt = float2(Pe.x, Pe.y * (1.0 + eh * 4.0)); float rd = length(Pt); cov = smoothstep(er, er * 0.88, rd) * 0.3 + smoothstep(et * 3.0, 0.0, abs(rd - er)); }
        else if (etype == 15) { float2 Pt = float2(Pe.x, Pe.y * (1.0 + eh * 4.0)); float rd = length(Pt); float inr = er * 0.5; cov = smoothstep(er, er * 0.9, rd) * smoothstep(inr, inr * 1.15, rd) * 0.3 + max(smoothstep(et * 3.0, 0.0, abs(rd - er)), smoothstep(et * 3.0, 0.0, abs(rd - inr))); }
        else if (etype == 16) { float2 Pt = float2(Pe.x, Pe.y * (1.0 + eh * 4.0)); float rd = length(Pt); float cang = atan2(Pt.x, -Pt.y); float hf = 3.1415927 * saturate(esides / 12.0); float within = smoothstep(hf + 0.03, hf - 0.03, abs(cang)) * smoothstep(er, er * 0.97, rd); cov = within * 0.3 + within * smoothstep(et * 3.0, 0.0, abs(rd - er)); }
        else if (etype == 17) { float2 q = abs(Pe) - float2(er, er * 2.6); float d = length(max(q, 0.0)) + min(max(q.x, q.y), 0.0); cov = smoothstep(0.0, -0.008, d) * 0.3 + smoothstep(et * 3.0, 0.0, abs(d)); }
        float eglow = eg.x;
        if (eglow > 0.001 && sd < 1e8) {
            float egw = (eg.y > 0.0005) ? eg.y : 0.045;
            cov = max(cov, saturate(eglow) * exp(-abs(sd) / egw));
        }
        c += ecol.rgb * saturate(cov) * einten * emask;
    }

    if (enForeground != 0) {
        float cov = clamp(fgPlaceSize, 0.0, 0.98);
        float soft = max(fgPlaceSoft, 0.02);
        float ex = min(i.uv.x, 1.0 - i.uv.x), ey = min(i.uv.y, 1.0 - i.uv.y);
        float hx = 1.0 - smoothstep(cov - soft, cov + soft, ex);
        float hy = 1.0 - smoothstep(cov - soft, cov + soft, ey);
        float2 rc = i.uv - 0.5; rc.x *= asp; float rr = length(rc);
        float env;
        if (fgPlaceMode == 0) env = max(hx, hy);
        else if (fgPlaceMode == 1) env = hx * hy;
        else if (fgPlaceMode == 2) env = 1.0 - smoothstep(cov - soft, cov + soft, i.uv.y);
        else if (fgPlaceMode == 3) env = 1.0 - smoothstep(cov - soft, cov + soft, 1.0 - i.uv.y);
        else if (fgPlaceMode == 4) env = 1.0 - smoothstep(cov - soft, cov + soft, i.uv.x);
        else if (fgPlaceMode == 5) env = 1.0 - smoothstep(cov - soft, cov + soft, 1.0 - i.uv.x);
        else if (fgPlaceMode == 6) { float reach = lerp(0.1, 1.2, cov); env = 1.0 - smoothstep(reach - soft, reach + soft, rr); }
        else if (fgPlaceMode == 7) { float2 dir = float2(cos(fgPlaceAngle), sin(fgPlaceAngle)); float g = dot(rc, dir) * 0.5 + 0.5; env = smoothstep(1.0 - cov - soft, 1.0 - cov + soft, g); }
        else env = 1.0;
        float a = saturate(env) * saturate(fgOpacity);
        if (hasDepth != 0 && fgDepthGate == 1) a *= (1.0 - smoothstep(0.08, 0.13, lin));
        else if (hasDepth != 0 && fgDepthGate == 2) a *= smoothstep(0.08, 0.13, lin);
        if (a > 0.001) {
            float3 fgA3 = float3(0.0, 0.0, 0.0), fgB3 = float3(0.0, 0.0, 0.0);
            float2 fgUv = float2(0.0, 0.0), fgSc = float2(1.0, 1.0);
            int nfg = ((int)FG(89 + 6) > 0) ? 2 : 1;
            [loop] for (int ffi = 0; ffi < nfg; ffi++) {
                BgResult fr = EvalBackdrop(i.uv, MakeFg(ffi), asp);
                if (ffi == 0) { fgA3 = fr.pat; fgUv = fr.uv; fgSc = fr.sc; } else fgB3 = fr.pat;
            }
            float3 fgPat = fgA3;
            if (nfg == 2) {
                float wS = SeamWeightFg(i.uv, lin, asp);
                if (fgSeamMatch > 0.0) {
                    float la = Luma(fgA3), lb = Luma(fgB3);
                    float bandM = 1.0 - abs(wS * 2.0 - 1.0);
                    float target = lerp(la, lb, wS);
                    fgA3 *= lerp(1.0, target / max(la, 1e-3), fgSeamMatch * bandM);
                    fgB3 *= lerp(1.0, target / max(lb, 1e-3), fgSeamMatch * bandM);
                }
                float3 mixed = fgB3;
                float soft2 = max(fgSeamFeather, 0.02) * 2.0;
                float lvl = saturate(fgSeamMixLevel);
                float wB;
                if (fgSeamMix == 1) wB = wS * smoothstep(lvl - soft2, lvl + soft2, Luma(fgB3));
                else if (fgSeamMix == 2) wB = wS * (1.0 - smoothstep(lvl - soft2, lvl + soft2, Luma(fgB3)));
                else if (fgSeamMix == 3) { mixed = 1.0 - (1.0 - saturate(fgA3)) * (1.0 - saturate(fgB3)); wB = wS; }
                else if (fgSeamMix == 4) { mixed = max(fgA3, fgB3); wB = wS; }
                else if (fgSeamMix == 5) { mixed = fgA3 * lerp(1.0, fgB3 * 1.8, lvl); wB = wS; }
                else if (fgSeamMix == 6) { float mm = Fbm(i.uv * float2(asp, 1.0) * max(fgSeamNoiseScale, 0.5) * 2.0 + 9.0, 4); wB = wS * smoothstep(lvl - soft2, lvl + soft2, mm); }
                else wB = wS;
                float react = 1.0 - abs(wB * 2.0 - 1.0);
                float contrast = saturate(abs(Luma(fgA3) - Luma(fgB3)) * 1.5 + distance(fgA3, fgB3) * 0.5);
                float3 hot = (Luma(fgA3) > Luma(fgB3)) ? fgA3 : fgB3;
                fgPat = lerp(fgA3, mixed, wB);
                fgPat += min(hot * 0.5 + 0.08, 0.45) * react * contrast * 0.2;
            }

            float3 fc4 = float3(FG(33), FG(34), FG(35));
            float fHueVar = FG(41);
            if (fHueVar > 0.0) { float hv = VNoise(fgUv * fgSc * 0.7 + 11.0) - 0.5; fgPat = HueShift(fgPat, hv * fHueVar * 0.3); }
            float fHaze = FG(45);
            if (fHaze > 0.0) { float hz = Fbm(fgUv * fgSc * 0.25 + 3.7, 3); fgPat += fc4 * smoothstep(0.4, 0.85, hz) * fHaze * 0.7; }
            float fGlow = FG(40);
            if (fGlow > 0.0) { float b = smoothstep(0.35, 0.85, Luma(fgPat)); fgPat += lerp(fgPat, fc4, 0.5) * b * fGlow * 1.5; }
            float fStars = FG(37);
            if (fStars > 0.0) {
                float2 sp = float2(i.uv.x * asp, i.uv.y);
                float dens = max(FG(38), 4.0);
                float3 star = float3(0.0, 0.0, 0.0);
                [unroll] for (int lsi = 0; lsi < 2; lsi++) {
                    float2 gs = sp * dens * (lsi == 0 ? 1.0 : 2.3);
                    float2 cell = floor(gs);
                    float h = Hash21(cell + float2(lsi * 19.0, lsi * 7.0));
                    float bright = frac(h * 91.7);
                    float2 jit = float2(Hash21(cell + 4.3), Hash21(cell + 8.9));
                    float2 fcc = frac(gs) - clamp(jit, 0.18, 0.82);
                    float sz = lerp(0.05, 0.34, saturate(FG(39))) * (0.4 + 0.6 * bright);
                    float s = smoothstep(sz, 0.0, length(fcc)) * step(0.86, h) * (0.4 + 0.6 * bright);
                    star += fc4 * s;
                }
                fgPat += star * fStars * 1.5;
            }
            float fEmb = FG(48);
            if (fEmb > 0.0) {
                float2 ep = float2(i.uv.x * asp, i.uv.y) * 18.0;
                float2 ec = floor(ep);
                float eh = Hash21(ec + 55.3);
                if (eh > 0.7) {
                    float2 jit = 0.3 + 0.4 * float2(Hash21(ec + 1.7), Hash21(ec + 9.1));
                    float2 d2v = frac(ep) - jit; float d2 = dot(d2v, d2v);
                    float k = lerp(90.0, 22.0, saturate(FG(56)));
                    fgPat += fc4 * exp(-d2 * k) * frac(eh * 37.0) * fEmb * 0.6;
                }
            }
            float fGrain = FG(9);
            if (fGrain > 0.0) { float gn = frac(sin(dot(i.uv, float2(12.9898, 78.233))) * 43758.5453); fgPat += (gn - 0.5) * fGrain * 0.3; }

            if (fgBlendMode == 1) c += fgPat * a;
            else if (fgBlendMode == 2) c = 1.0 - (1.0 - saturate(c)) * (1.0 - saturate(fgPat) * a);
            else if (fgBlendMode == 3) c = lerp(c, c * fgPat, a);
            else c = lerp(c, fgPat, a);
        }
    }

    if (particleAmount > 0.0) {
        float3 pcol = float3(particleR, particleG, particleB);
        float fall = time * (0.02 + particleFall * 0.15);
        float dens = lerp(6.0, 20.0, saturate(particleAmount));
        float acc = 0.0;
        [unroll] for (int ppl = 0; ppl < 2; ppl++) {
            float spd = (ppl == 0) ? 1.0 : 0.6;
            float2 uvp = float2(i.uv.x * asp, i.uv.y);
            uvp.y += fall * spd;
            uvp.x += sin(uvp.y * 3.0 + (float)ppl * 2.0) * 0.03;
            float2 gpp = uvp * dens * (ppl == 0 ? 1.0 : 1.7);
            float2 basec = floor(gpp), fr0 = frac(gpp);
            [loop] for (int oy = -1; oy <= 1; oy++) {
            [unroll] for (int ox = -1; ox <= 1; ox++) {
                float2 cell = basec + float2((float)ox, (float)oy);
                float hp = Hash21(cell + float2((float)ppl * 13.0, 3.0));
                float gate = (hp > 0.55) ? 1.0 : 0.0;
                float2 jit = float2(Hash21(cell + 1.1), Hash21(cell + 5.3));
                float2 d = fr0 - float2((float)ox, (float)oy) - jit;
                float sv = 0.55 + 0.9 * frac(hp * 31.7);
                float bv = 0.45 + 0.55 * frac(hp * 53.1);
                float rot = frac(hp * 17.3) * 6.2831853 + time * (frac(hp * 7.1) - 0.5) * 1.2;
                float cs = cos(rot), sn = sin(rot);
                float psz = lerp(6.0, 2.2, saturate(particleSize)) / max(sv, 0.25);
                float2 rd = float2(d.x * cs - d.y * sn, d.x * sn + d.y * cs) * psz;
                float shape;
                if (particleType == 1) {
                    float x = rd.x * 0.8, y = -rd.y * 0.8;
                    float hv = (x * x + y * y - 0.3); hv = hv * hv * hv - x * x * y * y * y;
                    shape = smoothstep(0.03, -0.03, hv);
                } else if (particleType == 2) {
                    float rr = length(rd);
                    float ring = smoothstep(0.14, 0.0, abs(rr - 0.62));
                    float fill = smoothstep(0.66, 0.15, rr) * 0.14;
                    float hi = smoothstep(0.20, 0.0, length(rd - float2(-0.26, -0.26))) * 0.85;
                    shape = ring + fill + hi;
                } else {
                    float2 e = rd * float2(1.5, 0.85);
                    e.x *= 1.0 + 0.45 * rd.y;
                    shape = smoothstep(1.0, 0.15, length(e));
                }
                float tw = 0.65 + 0.35 * sin(time * 2.0 + hp * 20.0);
                acc += saturate(shape) * bv * tw * gate;
            }}
        }
        c += pcol * acc * particleAmount * 0.9;
    }

    if (bokehAmount > 0.0 && hasDepth != 0) {
        float far = ZoneMask(zoneBokeh, lin, 0.15, 0.25);
        if (far > 0.005) {
            float2 bpp = float2(i.uv.x * asp, i.uv.y) * 9.0;
            float2 bbase = floor(bpp), bfr = frac(bpp);
            float accb = 0.0;
            [loop] for (int by = -1; by <= 1; by++) {
            [loop] for (int bx = -1; bx <= 1; bx++) {
                float2 cell = bbase + float2((float)bx, (float)by);
                float hbk = Hash21(cell + 17.0);
                float gate = (hbk > 0.6) ? 1.0 : 0.0;
                float2 jit = float2(Hash21(cell + 2.2), Hash21(cell + 7.4));
                float2 d = bfr - float2((float)bx, (float)by) - jit;
                float sv = 0.55 + 0.8 * frac(hbk * 41.3);
                float rad = max(0.34 * sv, 1e-3);
                float rr = length(d) / rad;
                float disc;
                if (bokehShape == 1) {
                    float x = d.x / rad * 1.1, y = -d.y / rad * 1.1;
                    float hv = (x * x + y * y - 0.3); hv = hv * hv * hv - x * x * y * y * y;
                    disc = smoothstep(0.06, -0.06, hv);
                } else if (bokehShape == 2) {
                    float ang = atan2(d.y, d.x);
                    disc = smoothstep(1.0, 0.88, rr + 0.06 * cos(ang * 6.0));
                } else disc = smoothstep(1.0, 0.88, rr);
                float rim = disc * smoothstep(0.45, 1.0, rr);
                accb += (disc * 0.7 + rim * 0.85) * (0.45 + 0.55 * frac(hbk * 87.1)) * gate;
            }}
            c += accb * far * bokehAmount * (0.30 + Luma(c) * 0.8) * float3(1.0, 0.96, 0.90);
        }
    }

    if (frostAmount > 0.0) {
        float3 zpre = c;
        float2 fc = i.uv - 0.5; fc.x *= asp;
        float er2 = length(fc);
        float reach = lerp(0.85, 0.10, saturate(frostCoverage));
        float edgeM = smoothstep(reach - 0.28, reach + 0.34, er2);
        float2 fp = i.uv * (6.0 + frostFeather * 18.0);
        fp += (Fbm(fp, 4) - 0.5) * 1.5;
        float ice = pow(RidgedFbm(fp, 5), 2.2);
        float m = saturate(edgeM * (0.35 + ice * 1.4)) * saturate(frostAmount);
        float3 frostCol = float3(0.88, 0.95, 1.0);
        c = lerp(c, c * 0.55 + frostCol * 0.72, m);
        float lu = dot(c, float3(0.299, 0.587, 0.114));
        c = lerp(c, lerp(float3(lu, lu, lu), frostCol * lu, 0.5), m * 0.45);
        float sprk = smoothstep(0.986, 1.0, Hash21(floor(i.uv * 900.0)));
        c += frostCol * sprk * edgeM * saturate(frostAmount) * 0.6;
        c = lerp(zpre, c, ZoneMask(zoneFrost, lin, scopeSplit, scopeSoft));
    }

    if (letterbox > 0.0) {
        float bar = letterbox * 0.2;
        if (i.uv.y < bar || i.uv.y > 1.0 - bar) c = float3(0.0, 0.0, 0.0);
    }
    return float4(saturate(c), 1.0);
}";

    private const string BloomHlsl = @"
cbuffer B : register(b0) {
    float texelX; float texelY; float dirX; float dirY;
    float threshold; float radius; float bpad0; float bpad1;
};
Texture2D src : register(t0);
SamplerState samp : register(s0);
struct VSOut { float4 pos : SV_Position; float2 uv : TEXCOORD0; };

float4 BrightPS(VSOut i) : SV_Target {
    float2 cu = float2(i.uv.x, 1.0 - i.uv.y);
    float3 c = src.Sample(samp, cu).rgb;
    float l = dot(c, float3(0.299, 0.587, 0.114));
    float k = saturate((l - threshold) / max(1.0 - threshold, 1e-3));
    return float4(c * k, 1.0);
}

float4 BlurPS(VSOut i) : SV_Target {
    float2 cu = float2(i.uv.x, 1.0 - i.uv.y);
    float2 st = float2(dirX, dirY) * float2(texelX, texelY) * radius;
    float3 s = src.Sample(samp, cu).rgb * 0.227027;
    s += (src.Sample(samp, cu + st) + src.Sample(samp, cu - st)).rgb * 0.1945946;
    s += (src.Sample(samp, cu + st * 2.0) + src.Sample(samp, cu - st * 2.0)).rgb * 0.1216216;
    s += (src.Sample(samp, cu + st * 3.0) + src.Sample(samp, cu - st * 3.0)).rgb * 0.054054;
    s += (src.Sample(samp, cu + st * 4.0) + src.Sample(samp, cu - st * 4.0)).rgb * 0.016216;
    return float4(s, 1.0);
}

float4 GodrayPS(VSOut i) : SV_Target {
    float2 cu = float2(i.uv.x, 1.0 - i.uv.y);
    float2 light = float2(dirX, dirY);
    const int N = 48;
    float2 delta = (cu - light) / float(N);
    float illum = 1.0;
    float3 accum = float3(0.0, 0.0, 0.0);
    float2 coord = cu;
    [loop] for (int s = 0; s < N; s++) {
        coord -= delta;
        float3 col = src.Sample(samp, coord).rgb;
        float l = dot(col, float3(0.299, 0.587, 0.114));
        accum += col * saturate((l - threshold) / max(1.0 - threshold, 1e-3)) * illum;
        illum *= radius;
    }
    return float4(accum / float(N) * 3.0, 1.0);
}

float4 HaloMaskPS(VSOut i) : SV_Target {
    float2 cu = float2(i.uv.x, 1.0 - i.uv.y) * float2(dirX, dirY);
    float rz = src.Sample(samp, cu).r;
    float z = 1.0 - rz;
    float lin = z / (1000.0 - z * 999.0);
    float m = 1.0 - smoothstep(threshold, threshold + 0.05, lin);
    return float4(m, m, m, 1.0);
}";

    private readonly ID3D11Device _device;
    private readonly ID3D11DeviceContext _immediate;
    private ID3D11VertexShader _vs = null!;
    private ID3D11PixelShader _ps = null!;
    private ID3D11PixelShader _brightPs = null!;
    private ID3D11PixelShader _blurPs = null!;
    private ID3D11PixelShader _godrayPs = null!;
    private ID3D11PixelShader _haloMaskPs = null!;
    private ID3D11Buffer _bloomCbuf = null!;
    private ID3D11SamplerState _sampler = null!;
    private ID3D11ShaderResourceView _fallbackSrv = null!;
    private ID3D11Buffer _cbuf = null!;
    private ID3D11RasterizerState _raster = null!;
    private ID3D11DepthStencilState _depthOff = null!;

    private ID3D11Texture2D? _outTex;
    private ID3D11RenderTargetView? _rtv;
    private ID3D11ShaderResourceView? _outSrv;
    private int _w, _h;
    private ID3D11Texture2D? _staging;
    private int _stagingW, _stagingH;
    private ID3D11Texture2D? _bloomTexA, _bloomTexB;
    private ID3D11RenderTargetView? _bloomRtvA, _bloomRtvB;
    private ID3D11ShaderResourceView? _bloomSrvA, _bloomSrvB;
    private ID3D11Texture2D? _blurTexC, _blurTexD;
    private ID3D11RenderTargetView? _blurRtvC, _blurRtvD;
    private ID3D11ShaderResourceView? _blurSrvC, _blurSrvD;
    private ID3D11Texture2D? _anamTexE, _anamTexF;
    private ID3D11RenderTargetView? _anamRtvE, _anamRtvF;
    private ID3D11ShaderResourceView? _anamSrvE, _anamSrvF;
    private ID3D11Texture2D? _haloTexG, _haloTexH;
    private ID3D11RenderTargetView? _haloRtvG, _haloRtvH;
    private ID3D11ShaderResourceView? _haloSrvG, _haloSrvH;
    private int _bloomW, _bloomH;
    private bool _loggedFirstRender;

    private static ReadOnlySpan<byte> ShaderBytes(string module, string entry, string source, string profile)
    {
        var name = $"GPoseStudio.Shaders.{module}_{entry}.cso";
        try
        {
            using var s = typeof(GpuRenderer).Assembly.GetManifestResourceStream(name);
            if (s != null)
            {
                var buf = new byte[s.Length];
                s.ReadExactly(buf);
                return buf;
            }
            Services.Log.Warning($"Precompiled shader {name} missing — compiling at runtime (slow).");
        }
        catch (Exception ex)
        {
            Services.Log.Warning(ex, $"Could not load precompiled {name} — compiling at runtime.");
        }
        return Compiler.Compile(source, entry, module + ".hlsl", profile).Span.ToArray();
    }

    public GpuRenderer(nint deviceHandle)
    {
        _device = new ID3D11Device(deviceHandle);
        _immediate = _device.ImmediateContext;

        _vs = _device.CreateVertexShader(ShaderBytes("Hlsl", "VS", Hlsl, "vs_5_0"));
        _ps = _device.CreatePixelShader(ShaderBytes("Hlsl", "PS", Hlsl, "ps_5_0"));
        _brightPs = _device.CreatePixelShader(ShaderBytes("BloomHlsl", "BrightPS", BloomHlsl, "ps_5_0"));
        _blurPs = _device.CreatePixelShader(ShaderBytes("BloomHlsl", "BlurPS", BloomHlsl, "ps_5_0"));
        _godrayPs = _device.CreatePixelShader(ShaderBytes("BloomHlsl", "GodrayPS", BloomHlsl, "ps_5_0"));
        _haloMaskPs = _device.CreatePixelShader(ShaderBytes("BloomHlsl", "HaloMaskPS", BloomHlsl, "ps_5_0"));
        _bloomCbuf = _device.CreateBuffer(new BufferDescription(
            (uint)Marshal.SizeOf<BloomParams>(), BindFlags.ConstantBuffer, ResourceUsage.Default));

        _sampler = _device.CreateSamplerState(new SamplerDescription
        {
            Filter = Filter.MinMagMipLinear,
            AddressU = TextureAddressMode.Clamp,
            AddressV = TextureAddressMode.Clamp,
            AddressW = TextureAddressMode.Clamp,
            ComparisonFunc = ComparisonFunction.Always,
            MaxLOD = float.MaxValue,
        });

        unsafe
        {
            uint px = 0;
            using var ftex = _device.CreateTexture2D(new Texture2DDescription
            {
                Width = 1, Height = 1, MipLevels = 1, ArraySize = 1,
                Format = Format.R8G8B8A8_UNorm, SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Immutable, BindFlags = BindFlags.ShaderResource,
            }, new[] { new SubresourceData((nint)(&px), 4) });
            _fallbackSrv = _device.CreateShaderResourceView(ftex);
        }

        _raster = _device.CreateRasterizerState(new RasterizerDescription
        {
            CullMode = CullMode.None,
            FillMode = FillMode.Solid,
            DepthClipEnable = true,
        });
        _depthOff = _device.CreateDepthStencilState(new DepthStencilDescription
        {
            DepthEnable = false,
            DepthWriteMask = DepthWriteMask.Zero,
            DepthFunc = ComparisonFunction.Always,
        });

        _cbuf = _device.CreateBuffer(new BufferDescription(
            (uint)Marshal.SizeOf<Params>(), BindFlags.ConstantBuffer, ResourceUsage.Default));
    }

    private void EnsureSize(int w, int h)
    {
        if (_outTex != null && _w == w && _h == h) return;
        _outSrv?.Dispose(); _rtv?.Dispose(); _outTex?.Dispose();
        _w = w; _h = h;

        _outTex = _device.CreateTexture2D(new Texture2DDescription
        {
            Width = (uint)w, Height = (uint)h, MipLevels = 1, ArraySize = 1,
            Format = Format.R8G8B8A8_UNorm,
            SampleDescription = new SampleDescription(1, 0),
            Usage = ResourceUsage.Default,
            BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
        });
        _rtv = _device.CreateRenderTargetView(_outTex);
        _outSrv = _device.CreateShaderResourceView(_outTex);

        _bloomSrvA?.Dispose(); _bloomRtvA?.Dispose(); _bloomTexA?.Dispose();
        _bloomSrvB?.Dispose(); _bloomRtvB?.Dispose(); _bloomTexB?.Dispose();
        _blurSrvC?.Dispose(); _blurRtvC?.Dispose(); _blurTexC?.Dispose();
        _blurSrvD?.Dispose(); _blurRtvD?.Dispose(); _blurTexD?.Dispose();
        _bloomW = Math.Max(1, w / PrepassDiv); _bloomH = Math.Max(1, h / PrepassDiv);
        (_bloomTexA, _bloomRtvA, _bloomSrvA) = MakeRtTexture(_bloomW, _bloomH);
        (_bloomTexB, _bloomRtvB, _bloomSrvB) = MakeRtTexture(_bloomW, _bloomH);
        (_blurTexC, _blurRtvC, _blurSrvC) = MakeRtTexture(_bloomW, _bloomH);
        (_blurTexD, _blurRtvD, _blurSrvD) = MakeRtTexture(_bloomW, _bloomH);

        _anamSrvE?.Dispose(); _anamRtvE?.Dispose(); _anamTexE?.Dispose();
        _anamSrvF?.Dispose(); _anamRtvF?.Dispose(); _anamTexF?.Dispose();
        (_anamTexE, _anamRtvE, _anamSrvE) = MakeRtTexture(_bloomW, _bloomH);
        (_anamTexF, _anamRtvF, _anamSrvF) = MakeRtTexture(_bloomW, _bloomH);

        _haloSrvG?.Dispose(); _haloRtvG?.Dispose(); _haloTexG?.Dispose();
        _haloSrvH?.Dispose(); _haloRtvH?.Dispose(); _haloTexH?.Dispose();
        (_haloTexG, _haloRtvG, _haloSrvG) = MakeRtTexture(_bloomW, _bloomH);
        (_haloTexH, _haloRtvH, _haloSrvH) = MakeRtTexture(_bloomW, _bloomH);
    }

    private (ID3D11Texture2D, ID3D11RenderTargetView, ID3D11ShaderResourceView) MakeRtTexture(int w, int h)
    {
        var tex = _device.CreateTexture2D(new Texture2DDescription
        {
            Width = (uint)w, Height = (uint)h, MipLevels = 1, ArraySize = 1,
            Format = Format.R8G8B8A8_UNorm,
            SampleDescription = new SampleDescription(1, 0),
            Usage = ResourceUsage.Default,
            BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
        });
        return (tex, _device.CreateRenderTargetView(tex), _device.CreateShaderResourceView(tex));
    }

    public (int Width, int Height, byte[] Rgba)? ReadbackLastOutput()
    {
        if (_outTex == null || _w <= 0 || _h <= 0) return null;

        if (_staging == null || _stagingW != _w || _stagingH != _h)
        {
            _staging?.Dispose();
            _staging = _device.CreateTexture2D(new Texture2DDescription
            {
                Width = (uint)_w, Height = (uint)_h, MipLevels = 1, ArraySize = 1,
                Format = Format.R8G8B8A8_UNorm,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Staging,
                BindFlags = BindFlags.None,
                CPUAccessFlags = CpuAccessFlags.Read,
            });
            _stagingW = _w; _stagingH = _h;
        }

        var c = _immediate;
        c.CopyResource(_staging, _outTex);
        var map = c.Map(_staging, 0, MapMode.Read, Vortice.Direct3D11.MapFlags.None);
        try
        {
            var bytes = new byte[_w * _h * 4];
            int rowBytes = _w * 4;
            for (int y = 0; y < _h; y++)
                Marshal.Copy(map.DataPointer + y * (int)map.RowPitch, bytes, y * rowBytes, rowBytes);
            return (_w, _h, bytes);
        }
        finally
        {
            c.Unmap(_staging, 0);
        }
    }

    private const int PrepassDiv = 2;
    private const float RadiusComp = 4f / PrepassDiv;

    private float _resScale = 1f;

    public nint Render(nint colorSrvPtr, nint depthSrvPtr, int w, int h, in Params p, nint[]? memeSrvPtrs = null, float resScale = 1f)
    {
        if (colorSrvPtr == 0 || w <= 0 || h <= 0) return 0;
        _resScale = resScale;
        EnsureSize(w, h);

        using var colorSrv = new ID3D11ShaderResourceView(colorSrvPtr);
        colorSrv.AddRef();

        ID3D11ShaderResourceView? depthSrv = null;
        if (depthSrvPtr != 0)
        {
            depthSrv = new ID3D11ShaderResourceView(depthSrvPtr);
            depthSrv.AddRef();
        }

        var memeSrvs = new ID3D11ShaderResourceView?[8];
        for (int mi = 0; mi < 8; mi++)
        {
            nint ptr = memeSrvPtrs != null && mi < memeSrvPtrs.Length ? memeSrvPtrs[mi] : 0;
            if (ptr == 0) continue;
            try { var srv = new ID3D11ShaderResourceView(ptr); srv.AddRef(); memeSrvs[mi] = srv; }
            catch { memeSrvs[mi] = null; }
        }

        var c = _immediate;

        var oldRtvs = new ID3D11RenderTargetView[1];
        c.OMGetRenderTargets(1, oldRtvs, out var oldDsv);
        var oldViewports = c.RSGetViewports<Viewport>().ToArray();

        c.RSSetState(_raster);
        c.OMSetDepthStencilState(_depthOff, 0);
        c.VSSetShader(_vs);
        c.PSSetSampler(0, _sampler);
        c.IASetInputLayout(null);
        c.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);

        bool bloom = p.BloomAmount > 0f || p.Halation > 0f;
        bool godray = p.GodrayAmount > 0f;
        bool fullblur = p.BgBlur > 0f || p.Orton > 0f || p.Glamour > 0f || p.Clarity > 0f || p.TiltAmt > 0f;
        bool anam = p.AnamAmount > 0f;
        bool halo = p.HaloAmount > 0f && depthSrv != null;
        if (bloom) RenderBloom(colorSrv, p);
        if (godray) RenderGodrays(colorSrv, p);
        if (fullblur) RenderFullBlur(colorSrv, p);
        if (anam) RenderAnamorphic(colorSrv, p);
        if (halo) RenderHalo(depthSrv!, p);

        c.UpdateSubresource(p, _cbuf);

        c.OMSetRenderTargets(_rtv!);
        c.RSSetViewport(new Viewport(0, 0, w, h, 0f, 1f));
        c.PSSetShader(_ps);
        c.PSSetShaderResource(0, colorSrv);
        if (depthSrv != null) c.PSSetShaderResource(1, depthSrv);
        if (bloom) c.PSSetShaderResource(2, _bloomSrvA!);
        if (godray) c.PSSetShaderResource(3, _bloomSrvB!);
        if (fullblur) c.PSSetShaderResource(4, _blurSrvC!);
        if (anam) c.PSSetShaderResource(5, _anamSrvE!);
        if (halo) c.PSSetShaderResource(6, _haloSrvG!);
        for (int mi = 0; mi < 8; mi++) c.PSSetShaderResource((uint)(7 + mi), memeSrvs[mi] ?? _fallbackSrv);
        c.PSSetConstantBuffer(0, _cbuf);
        c.Draw(3, 0);

        depthSrv?.Dispose();
        for (int mi = 0; mi < 8; mi++) memeSrvs[mi]?.Dispose();

        c.OMSetRenderTargets(1, oldRtvs, oldDsv);
        if (oldViewports.Length > 0) c.RSSetViewports(oldViewports);

        oldRtvs[0]?.Dispose();
        oldDsv?.Dispose();

        if (!_loggedFirstRender)
        {
            _loggedFirstRender = true;
            Services.Log.Info(
                $"GpuRenderer first render OK: {w}x{h}, srcSrv=0x{colorSrvPtr:X}, outSrv=0x{_outSrv!.NativePointer:X}, " +
                $"exposure={p.Exposure}, temp={p.Temperature}, swapRB={p.SwapRedBlue}, flip={p.Flip}");
        }

        return _outSrv!.NativePointer;
    }

    private void RenderBloom(ID3D11ShaderResourceView colorSrv, in Params p)
    {
        var c = _immediate;
        c.RSSetViewport(new Viewport(0, 0, _bloomW, _bloomH, 0f, 1f));
        c.PSSetConstantBuffer(0, _bloomCbuf);

        SetBloomCb(0f, 0f, p.BloomThreshold, p.BloomRadius);
        c.OMSetRenderTargets(_bloomRtvA!);
        c.PSSetShader(_brightPs);
        c.PSSetShaderResource(0, colorSrv);
        c.Draw(3, 0);

        c.PSSetShader(_blurPs);
        for (int it = 0; it < 2; it++)
        {
            SetBloomCb(1f, 0f, p.BloomThreshold, p.BloomRadius * _resScale * RadiusComp);
            c.OMSetRenderTargets(_bloomRtvB!);
            c.PSSetShaderResource(0, _bloomSrvA!);
            c.Draw(3, 0);

            SetBloomCb(0f, 1f, p.BloomThreshold, p.BloomRadius * _resScale * RadiusComp);
            c.OMSetRenderTargets(_bloomRtvA!);
            c.PSSetShaderResource(0, _bloomSrvB!);
            c.Draw(3, 0);
        }
    }

    private void RenderHalo(ID3D11ShaderResourceView depthSrv, in Params p)
    {
        var c = _immediate;
        c.RSSetViewport(new Viewport(0, 0, _bloomW, _bloomH, 0f, 1f));
        c.PSSetConstantBuffer(0, _bloomCbuf);

        SetBloomCb(p.DepthUvScaleX, p.DepthUvScaleY, p.HaloSplit, 0f);
        c.OMSetRenderTargets(_haloRtvG!);
        c.PSSetShader(_haloMaskPs);
        c.PSSetShaderResource(0, depthSrv);
        c.Draw(3, 0);

        c.PSSetShader(_blurPs);
        SetBloomCb(1f, 0f, 0f, 3f * _resScale * RadiusComp);
        c.OMSetRenderTargets(_haloRtvH!);
        c.PSSetShaderResource(0, _haloSrvG!);
        c.Draw(3, 0);

        SetBloomCb(0f, 1f, 0f, 3f * _resScale * RadiusComp);
        c.OMSetRenderTargets(_haloRtvG!);
        c.PSSetShaderResource(0, _haloSrvH!);
        c.Draw(3, 0);

        SetBloomCb(1f, 0f, 0f, 7f * _resScale * RadiusComp);
        c.OMSetRenderTargets(_haloRtvH!);
        c.PSSetShaderResource(0, _haloSrvG!);
        c.Draw(3, 0);

        SetBloomCb(0f, 1f, 0f, 7f * _resScale * RadiusComp);
        c.OMSetRenderTargets(_haloRtvG!);
        c.PSSetShaderResource(0, _haloSrvH!);
        c.Draw(3, 0);
    }

    private void RenderAnamorphic(ID3D11ShaderResourceView colorSrv, in Params p)
    {
        var c = _immediate;
        c.RSSetViewport(new Viewport(0, 0, _bloomW, _bloomH, 0f, 1f));
        c.PSSetConstantBuffer(0, _bloomCbuf);

        SetBloomCb(0f, 0f, p.AnamThreshold, 0f);
        c.OMSetRenderTargets(_anamRtvE!);
        c.PSSetShader(_brightPs);
        c.PSSetShaderResource(0, colorSrv);
        c.Draw(3, 0);

        c.PSSetShader(_blurPs);
        float r = Math.Max(1f, p.AnamLength);
        SetBloomCb(1f, 0f, 0f, r * _resScale * RadiusComp);
        c.OMSetRenderTargets(_anamRtvF!);
        c.PSSetShaderResource(0, _anamSrvE!);
        c.Draw(3, 0);

        SetBloomCb(1f, 0f, 0f, r * _resScale * RadiusComp);
        c.OMSetRenderTargets(_anamRtvE!);
        c.PSSetShaderResource(0, _anamSrvF!);
        c.Draw(3, 0);
    }

    private void RenderFullBlur(ID3D11ShaderResourceView colorSrv, in Params p)
    {
        var c = _immediate;
        c.RSSetViewport(new Viewport(0, 0, _bloomW, _bloomH, 0f, 1f));
        c.PSSetConstantBuffer(0, _bloomCbuf);
        c.PSSetShader(_blurPs);
        float r = Math.Max(1f, p.SoftBlurRadius);

        SetBloomCb(1f, 0f, 0f, r * _resScale * RadiusComp);
        c.OMSetRenderTargets(_blurRtvD!);
        c.PSSetShaderResource(0, colorSrv);
        c.Draw(3, 0);

        SetBloomCb(0f, 1f, 0f, r * RadiusComp);
        c.OMSetRenderTargets(_blurRtvC!);
        c.PSSetShaderResource(0, _blurSrvD!);
        c.Draw(3, 0);
    }

    private void SetBloomCb(float dirX, float dirY, float threshold, float radius)
    {
        var bp = new BloomParams
        {
            TexelX = 1f / _bloomW, TexelY = 1f / _bloomH,
            DirX = dirX, DirY = dirY, Threshold = threshold, Radius = radius,
        };
        _immediate.UpdateSubresource(bp, _bloomCbuf);
    }

    private void RenderGodrays(ID3D11ShaderResourceView colorSrv, in Params p)
    {
        var c = _immediate;
        c.RSSetViewport(new Viewport(0, 0, _bloomW, _bloomH, 0f, 1f));
        c.PSSetConstantBuffer(0, _bloomCbuf);
        SetBloomCb(p.GodrayLightX, p.GodrayLightY, p.GodrayThreshold, p.GodrayDecay);
        c.OMSetRenderTargets(_bloomRtvB!);
        c.PSSetShader(_godrayPs);
        c.PSSetShaderResource(0, colorSrv);
        c.Draw(3, 0);
    }

    public void Dispose()
    {
        _staging?.Dispose();
        _bloomSrvA?.Dispose(); _bloomRtvA?.Dispose(); _bloomTexA?.Dispose();
        _bloomSrvB?.Dispose(); _bloomRtvB?.Dispose(); _bloomTexB?.Dispose();
        _blurSrvC?.Dispose(); _blurRtvC?.Dispose(); _blurTexC?.Dispose();
        _blurSrvD?.Dispose(); _blurRtvD?.Dispose(); _blurTexD?.Dispose();
        _anamSrvE?.Dispose(); _anamRtvE?.Dispose(); _anamTexE?.Dispose();
        _anamSrvF?.Dispose(); _anamRtvF?.Dispose(); _anamTexF?.Dispose();
        _haloSrvG?.Dispose(); _haloRtvG?.Dispose(); _haloTexG?.Dispose();
        _haloSrvH?.Dispose(); _haloRtvH?.Dispose(); _haloTexH?.Dispose();
        _bloomCbuf?.Dispose(); _brightPs?.Dispose(); _blurPs?.Dispose(); _godrayPs?.Dispose(); _haloMaskPs?.Dispose();
        _outSrv?.Dispose(); _rtv?.Dispose(); _outTex?.Dispose();
        _depthOff?.Dispose(); _raster?.Dispose();
        _cbuf?.Dispose(); _sampler?.Dispose(); _ps?.Dispose(); _vs?.Dispose();
        _fallbackSrv?.Dispose();
        _immediate?.Dispose();
    }
}

