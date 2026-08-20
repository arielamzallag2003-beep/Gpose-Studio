using System;
using System.Collections.Generic;
using System.Linq;
using GPoseStudio;
using Xunit;

public class BuiltinSetTests
{
    [Fact]
    public void NoBuiltinCanOverwriteALegacyLook()
    {
        var legacy = LookStore.Legacy.Select(l => l.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var clashes = LookStore.Builtins.Select(b => b.Name).Where(legacy.Contains).ToArray();

        Assert.True(clashes.Length == 0,
            "these built-ins would overwrite a legacy look of the same name: " + string.Join(", ", clashes));
    }

    [Fact]
    public void NamesAreUniqueWithinEachSet()
    {
        foreach (var (label, names) in new (string, IEnumerable<string>)[]
                 {
                     ("built-ins", LookStore.Builtins.Select(b => b.Name)),
                     ("legacy", LookStore.Legacy.Select(l => l.Name)),
                 })
        {
            var dupes = names.GroupBy(n => n, StringComparer.OrdinalIgnoreCase)
                             .Where(g => g.Count() > 1).Select(g => g.Key).ToArray();
            Assert.True(dupes.Length == 0, $"duplicate {label}: " + string.Join(", ", dupes));
        }
    }

    [Fact]
    public void EveryNameCanActuallyBecomeAFile()
    {
        foreach (var name in LookStore.Builtins.Select(b => b.Name).Concat(LookStore.Legacy.Select(l => l.Name)))
            Assert.True(LookName.IsUsable(name, out var err), $"'{name}': {err}");
    }

    [Fact]
    public void EveryBuiltinProducesADifferentLook()
    {
        var seen = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var (name, _, apply) in LookStore.Builtins)
        {
            var c = new PluginConfig();
            apply(c);
            var json = LookStore.Capture(c);
            Assert.False(seen.TryGetValue(json, out var other),
                $"'{name}' produces exactly the same look as '{other}'");
            seen[json] = name;
        }
    }

    [Fact]
    public void ApplyingAPresetNeverThrows()
    {
        foreach (var (name, _, apply) in LookStore.Builtins.Concat(LookStore.Legacy))
        {
            var c = new PluginConfig();
            var ex = Record.Exception(() => apply(c));
            Assert.True(ex == null, $"'{name}' threw: {ex}");
        }
    }

    [Fact]
    public void APresetLeavesTheConfigInAShapeThatCanBeRendered()
    {
        foreach (var (name, _, apply) in LookStore.Builtins.Concat(LookStore.Legacy))
        {
            var c = new PluginConfig();
            apply(c);
            Assert.True(c.Elem.Length == 8 * PluginConfig.ElemStride, $"'{name}' left Elem the wrong length");
            Assert.True(c.FgField.Length == new PluginConfig().FgField.Length, $"'{name}' left FgField the wrong length");
            Assert.True(c.ElemImages.Length == 8, $"'{name}' left ElemImages the wrong length");
        }
    }

    [Fact]
    public void APresetOnlyAimsAtAMaskItCanBeSureOf()
    {
        foreach (var (name, _, apply) in LookStore.Builtins)
        {
            var c = new PluginConfig();
            apply(c);
            for (int i = 0; i < 3; i++)
            {
                if (c.MaskSubscribers(i) == 0) continue;
                Assert.True(c.MaskMode(i) == 2,
                    $"'{name}' aims {c.MaskSubscribers(i)} effect(s) at mask {(char)('A' + i)}, " +
                    $"which is mode {c.MaskMode(i)}. Only a linear mask is the same place in every scene.");
            }
        }
    }

    [Fact]
    public void EveryBuiltinSurvivesBeingSavedAndLoaded()
    {
        foreach (var (name, _, apply) in LookStore.Builtins)
        {
            var src = new PluginConfig();
            apply(src);
            src.CarryPatternIdentity();
            var json = LookStore.Capture(src);

            var dst = new PluginConfig();
            Assert.True(LookStore.Apply(json, dst, LookStore.Part.All), $"'{name}' would not load");
            Assert.Equal(json, LookStore.Capture(dst));
        }
    }

    [Fact]
    public void ALookThatCompositesNothingLeavesNothingComposited()
    {
        foreach (var name in new[] { "Anamorphic", "Night Noir", "Golden Hour" })
        {
            var c = new PluginConfig();
            foreach (var (n, _, apply) in LookStore.Builtins) if (n == name) apply(c);

            Assert.False(c.EnBackdrop && c.BgRecolor > 0f && c.BgStyle > 0, $"'{name}' composites a backdrop");
            Assert.True(c.BgBStyle == 0, $"'{name}' leaves background B on, which draws a seam across the frame");
            Assert.False(c.EnForegroundOn, $"'{name}' leaves the foreground layer on");
            Assert.False(c.EnBgFill && c.BgFill > 0f, $"'{name}' fills the background");
            Assert.True(c.RimSplit == 0f, $"'{name}' splits the rim colour across the frame");
        }
    }

    [Fact]
    public void APresetStartsFromNothingItDidNotChoose()
    {
        foreach (var (name, _, apply) in LookStore.Builtins)
        {
            var fresh = new PluginConfig();
            apply(fresh);

            var dirty = new PluginConfig
            {
                EnBackdrop = true, BgStyle = 14, BgRecolor = 1f, BgBStyle = 19,
                BlendMode = 0, BlendAngle = 0.7f, EnBgFill = true, BgFill = 1f,
                EnForegroundOn = true, RimSplit = 1f, EnFog = true, FogStrength = 0.8f,
                EnVhs = true, VhsStatic = 0.7f, MaskAMode = 1, MaskBMode = 2,
            };
            apply(dirty);

            Assert.Equal(LookStore.Capture(fresh), LookStore.Capture(dirty));
        }
    }

    [Fact]
    public void ClearingFirstGivesTheLookAndNothingElse()
    {
        var src = new PluginConfig();
        foreach (var (n, _, apply) in LookStore.Builtins) if (n == "Low Key") apply(src);
        var json = LookStore.Capture(src);

        var virgin = new PluginConfig();
        LookStore.Apply(json, virgin, LookStore.Part.All);

        var used = new PluginConfig();
        foreach (var (n, _, apply) in LookStore.Builtins) if (n == "Deep Field") apply(used);
        used.ResetLook();
        LookStore.Apply(json, used, LookStore.Part.All);

        Assert.Equal(LookStore.Capture(virgin), LookStore.Capture(used));
    }

    [Fact]
    public void ClearingFirstIsWhatMakesAPartialLoadPredictable()
    {
        var grade = LookStore.Capture(new PluginConfig { Exposure = 0.5f }, false, LookStore.Part.Grade);

        var withBackdrop = new PluginConfig();
        foreach (var (n, _, apply) in LookStore.Builtins) if (n == "Deep Field") apply(withBackdrop);
        Assert.True(withBackdrop.BgStyle > 0, "the fixture needs a backdrop to inherit");

        var inherited = new PluginConfig();
        LookStore.Apply(LookStore.Capture(withBackdrop), inherited, LookStore.Part.All);
        LookStore.Apply(grade, inherited, LookStore.Part.Grade);
        Assert.True(inherited.BgStyle > 0, "without clearing, the backdrop stays");

        var cleared = new PluginConfig();
        LookStore.Apply(LookStore.Capture(withBackdrop), cleared, LookStore.Part.All);
        cleared.ResetLook();
        LookStore.Apply(grade, cleared, LookStore.Part.Grade);
        Assert.Equal(new PluginConfig().BgStyle, cleared.BgStyle);
        Assert.Equal(0.5f, cleared.Exposure);
    }
}
