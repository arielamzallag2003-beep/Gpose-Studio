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
}
