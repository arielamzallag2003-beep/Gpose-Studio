using System.Collections.Generic;
using GPoseStudio;
using Xunit;

public class TextMarkerTests
{
    [Fact]
    public void AnOldPixelSizeBecomesAFraction()
    {
        var c = new PluginConfig();
        c.Texts.Add(new TextMarker { Size = 32f });
        c.MigrateTextSize();
        Assert.Equal(32f / PluginConfig.LegacyTextReferenceHeight, c.Texts[0].Size, 5);
    }

    [Fact]
    public void MigratingTwiceDoesNotShrinkItAgain()
    {
        var c = new PluginConfig();
        c.Texts.Add(new TextMarker { Size = 200f });
        c.MigrateTextSize();
        var once = c.Texts[0].Size;
        c.MigrateTextSize();
        Assert.Equal(once, c.Texts[0].Size);
        Assert.True(once <= 1f);
    }

    [Fact]
    public void ANewFractionIsLeftAlone()
    {
        var c = new PluginConfig();
        c.Texts.Add(new TextMarker { Size = 0.06f });
        c.MigrateTextSize();
        Assert.Equal(0.06f, c.Texts[0].Size);
    }

    [Fact]
    public void ADefaultMarkerIsAlreadyAFraction()
    {
        Assert.True(new TextMarker().Size <= 1f);
    }

    [Fact]
    public void MigratingSurvivesAnEmptyOrNullList()
    {
        var c = new PluginConfig();
        c.MigrateTextSize();
        c.Texts.Add(null!);
        c.MigrateTextSize();
    }

    [Fact]
    public void EveryCaptionFieldSurvivesTheLookRoundTrip()
    {
        var src = new PluginConfig();
        src.Texts.Add(new TextMarker
        {
            Text = "two\nlines", X = 0.2f, Y = 0.8f, Size = 0.09f,
            R = 0.5f, G = 0.25f, B = 0.125f, A = 0.75f, Align = 2, Outline = true,
            Font = "Georgia", Bold = true, Italic = true,
            OutlineWidth = 0.17f, OutlineR = 0.1f, OutlineG = 0.2f, OutlineB = 0.3f,
        });

        var dst = new PluginConfig();
        LookStore.Apply(LookStore.Capture(src), dst, LookStore.Part.All);

        var a = src.Texts[0];
        var b = Assert.Single(dst.Texts);
        Assert.Equal(a.Text, b.Text);
        Assert.Equal(a.Size, b.Size);
        Assert.Equal(a.Align, b.Align);
        Assert.Equal(a.Font, b.Font);
        Assert.Equal(a.Bold, b.Bold);
        Assert.Equal(a.Italic, b.Italic);
        Assert.Equal(a.OutlineWidth, b.OutlineWidth);
        Assert.Equal(a.OutlineR, b.OutlineR);
        Assert.Equal(a.OutlineB, b.OutlineB);
    }

    [Fact]
    public void EveryFieldOnAMarkerActuallyTravels()
    {
        var src = new PluginConfig();
        var m = new TextMarker();
        var t = typeof(TextMarker);

        foreach (var p in t.GetProperties())
        {
            if (!p.CanRead || !p.CanWrite) continue;
            if (p.PropertyType == typeof(float)) p.SetValue(m, 0.3125f);
            else if (p.PropertyType == typeof(int)) p.SetValue(m, 2);
            else if (p.PropertyType == typeof(bool)) p.SetValue(m, true);
            else if (p.PropertyType == typeof(string)) p.SetValue(m, "carried");
        }
        src.Texts.Add(m);

        var json = LookStore.Capture(src);
        foreach (var p in t.GetProperties())
            if (p.CanRead && p.CanWrite)
                Assert.True(json.Contains("\"" + p.Name + "\""), $"TextMarker.{p.Name} is not in a saved look");

        var dst = new PluginConfig();
        LookStore.Apply(json, dst, LookStore.Part.All);
        var back = Assert.Single(dst.Texts);
        foreach (var p in t.GetProperties())
            if (p.CanRead && p.CanWrite)
                Assert.True(Equals(p.GetValue(m), p.GetValue(back)),
                    $"TextMarker.{p.Name} did not survive the round trip");
    }

    [Fact]
    public void ThePlateBannerFieldsSurviveALook()
    {
        var c = new PluginConfig();
        c.Texts.Add(new TextMarker { Text = "CHALLENGER APPROACHING", PlateExtend = 2.75f, PlateFade = 0.32f });

        var back = new PluginConfig();
        Assert.True(LookStore.Apply(LookStore.Capture(c), back, LookStore.Part.All));

        var t = Assert.Single(back.Texts);
        Assert.Equal(2.75f, t.PlateExtend);
        Assert.Equal(0.32f, t.PlateFade);
    }

    [Fact]
    public void APlateDoesNotRunOnOrFadeUnlessAsked()
    {
        var t = new TextMarker();
        Assert.Equal(0f, t.PlateExtend);
        Assert.Equal(0f, t.PlateFade);
    }

    [Fact]
    public void EveryCaptionSettingChangesTheKeyThePreviewCachesOn()
    {
        var missed = new List<string>();
        foreach (var p in typeof(TextMarker).GetProperties())
        {
            if (!p.CanRead || !p.CanWrite) continue;
            var t = new TextMarker();
            string before = t.PixelKey(40f);

            object? moved = p.PropertyType switch
            {
                var x when x == typeof(float) => (float)p.GetValue(t)! + 0.37f,
                var x when x == typeof(int) => (int)p.GetValue(t)! + 1,
                var x when x == typeof(bool) => !(bool)p.GetValue(t)!,
                var x when x == typeof(string) => (string?)p.GetValue(t) == "moved" ? "other" : "moved",
                _ => null,
            };
            if (moved is null) continue;

            p.SetValue(t, moved);
            if (t.PixelKey(40f) == before) missed.Add(p.Name);
        }

        Assert.True(missed.Count == 0, "not in the preview's cache key: " + string.Join(", ", missed));
    }

    [Fact]
    public void CloningACaptionKeepsEverySetting()
    {
        var t = new TextMarker();
        foreach (var p in typeof(TextMarker).GetProperties())
        {
            if (!p.CanRead || !p.CanWrite) continue;
            object? moved = p.PropertyType switch
            {
                var x when x == typeof(float) => (float)p.GetValue(t)! + 0.41f,
                var x when x == typeof(int) => (int)p.GetValue(t)! + 3,
                var x when x == typeof(bool) => !(bool)p.GetValue(t)!,
                var x when x == typeof(string) => "moved",
                _ => null,
            };
            if (moved is not null) p.SetValue(t, moved);
        }

        var copy = t.Clone();

        var lost = new List<string>();
        foreach (var p in typeof(TextMarker).GetProperties())
        {
            if (!p.CanRead || !p.CanWrite) continue;
            if (!Equals(p.GetValue(copy), p.GetValue(t))) lost.Add(p.Name);
        }
        Assert.True(lost.Count == 0, "dropped by Clone: " + string.Join(", ", lost));
        Assert.NotSame(t, copy);
    }

    [Fact]
    public void TheSizeItWasRasterisedAtIsInTheKeyToo()
    {
        var t = new TextMarker { Text = "CHALLENGER" };
        Assert.NotEqual(t.PixelKey(40f), t.PixelKey(80f));
    }

    [Fact]
    public void TheTurnAwaySurvivesALookAndStartsAtNone()
    {
        Assert.Equal(0f, new TextMarker().Yaw);

        var c = new PluginConfig();
        c.Texts.Add(new TextMarker { Text = "A new foe has appeared!", Yaw = -0.62f });

        var back = new PluginConfig();
        Assert.True(LookStore.Apply(LookStore.Capture(c), back, LookStore.Part.All));
        Assert.Equal(-0.62f, Assert.Single(back.Texts).Yaw);
    }
}
