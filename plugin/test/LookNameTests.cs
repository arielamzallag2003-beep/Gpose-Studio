using GPoseStudio;
using Xunit;

public class LookNameTests
{
    [Theory]
    [InlineData("Dusk")]
    [InlineData("Evening Rain")]
    [InlineData("Tempe — The Red That Follows")]
    [InlineData("look_2")]
    public void OrdinaryNamesAreUsable(string name)
    {
        Assert.True(LookName.IsUsable(name, out var err), err);
        Assert.Equal(name.Trim(), LookName.Clean(name));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("///")]
    [InlineData("\\")]
    [InlineData("...")]
    [InlineData("  . ")]
    public void NamesThatCannotBecomeAFileAreRejected(string? name)
    {
        Assert.False(LookName.IsUsable(name, out var err));
        Assert.NotEqual("", err);
        Assert.Equal("", LookName.Clean(name));
    }

    [Theory]
    [InlineData("CON")]
    [InlineData("con")]
    [InlineData("NUL")]
    [InlineData("COM1")]
    [InlineData("LPT9")]
    [InlineData("aux")]
    [InlineData("CON.backup")]
    public void ReservedDeviceNamesAreRejected(string name)
    {
        Assert.False(LookName.IsUsable(name, out var err));
        Assert.Contains("Windows", err);
    }

    [Theory]
    [InlineData("CONs")]
    [InlineData("CONSTANT")]
    [InlineData("NULL")]
    [InlineData("COM10")]
    public void NamesMerelyStartingWithADeviceNameAreFine(string name)
    {
        Assert.True(LookName.IsUsable(name, out var err), err);
    }

    [Fact]
    public void SeparatorsAndTraversalCannotSurvive()
    {
        foreach (var probe in new[] { "../../etc/passwd", @"..\..\config", "C:\\Windows\\evil", "a/b/c" })
        {
            var cleaned = LookName.Clean(probe);
            Assert.DoesNotContain("/", cleaned);
            Assert.DoesNotContain("\\", cleaned);
            Assert.DoesNotContain(":", cleaned);
        }
    }

    [Fact]
    public void TrailingDotsAndSpacesCannotCollideTwoNames()
    {
        Assert.Equal(LookName.Clean("Dusk"), LookName.Clean("Dusk."));
        Assert.Equal(LookName.Clean("Dusk"), LookName.Clean("Dusk   "));
        Assert.Equal(LookName.Clean("Dusk"), LookName.Clean("Dusk. . "));
    }

    [Theory]
    [InlineData("Judari", "Judari.")]
    [InlineData("Judari", "Judari ")]
    [InlineData("Judari", " Judari. ")]
    [InlineData("Judari", "Judari..")]
    public void NamesThatBecomeTheSameFileCleanToTheSameString(string expected, string typed)
    {
        Assert.Equal(expected, LookName.Clean(typed));
        Assert.Equal(LookName.Clean(expected), LookName.Clean(typed));
    }

    [Fact]
    public void LongNamesAreCutAndStayClean()
    {
        var cleaned = LookName.Clean(new string('x', 500));
        Assert.Equal(LookName.MaxLength, cleaned.Length);

        var dotty = LookName.Clean(new string('a', LookName.MaxLength - 1) + "..........");
        Assert.DoesNotContain('.', dotty[^1].ToString());
    }
}
