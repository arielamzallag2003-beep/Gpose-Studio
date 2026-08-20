using System;
using System.Collections.Generic;
using GPoseStudio;
using Xunit;

public class ElementImagesTests
{
    [Theory]
    [InlineData("overlay.png")]
    [InlineData("light leak 3.jpg")]
    [InlineData("a.b.c.png")]
    public void OurOwnNamesAreRecognised(string name)
        => Assert.True(ElementImages.IsStoredName(name));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(@"C:\Users\someone\Pictures\overlay.png")]
    [InlineData("sub/overlay.png")]
    [InlineData(@"sub\overlay.png")]
    [InlineData("../overlay.png")]
    [InlineData(@"..\..\config.json")]
    [InlineData("..")]
    public void AnythingThatIsNotJustANameIsNot(string? value)
        => Assert.False(ElementImages.IsStoredName(value));

    [Fact]
    public void ANameResolvesInsideTheFolder()
    {
        var got = ElementImages.Resolve(@"C:\cfg\elements", "overlay.png");
        Assert.Equal(System.IO.Path.Combine(@"C:\cfg\elements", "overlay.png"), got);
    }

    [Fact]
    public void AnOldAbsolutePathStillResolvesToItself()
    {
        const string p = @"C:\Users\someone\Pictures\overlay.png";
        Assert.Equal(p, ElementImages.Resolve(@"C:\cfg\elements", p));
    }

    [Theory]
    [InlineData("../../secrets.json")]
    [InlineData(@"..\..\secrets.json")]
    [InlineData("sub/thing.png")]
    public void ARelativePathThatIsNotOursResolvesToNothing(string stored)
    {
        Assert.Equal("", ElementImages.Resolve(@"C:\cfg\elements", stored));
    }

    [Fact]
    public void NothingResolvesToNothing()
    {
        Assert.Equal("", ElementImages.Resolve(@"C:\cfg\elements", null));
        Assert.Equal("", ElementImages.Resolve(@"C:\cfg\elements", "   "));
    }

    [Fact]
    public void TwoFilesOfTheSameNameBothSurvive()
    {
        var taken = new HashSet<string> { "overlay.png" };
        Assert.Equal("overlay (2).png", ElementImages.UniqueName(taken.Contains, "overlay.png"));

        taken.Add("overlay (2).png");
        Assert.Equal("overlay (3).png", ElementImages.UniqueName(taken.Contains, "overlay.png"));
    }

    [Fact]
    public void AFreeNameIsLeftAlone()
        => Assert.Equal("overlay.png", ElementImages.UniqueName(_ => false, "overlay.png"));

    [Fact]
    public void ANameIsCleanedIntoSomethingThatCanBeAFile()
    {
        var got = ElementImages.UniqueName(_ => false, "my:overlay?.png");
        Assert.True(ElementImages.IsStoredName(got), $"'{got}' is not a plain filename");
        Assert.EndsWith(".png", got);
    }

    [Fact]
    public void AReservedDeviceNameIsMovedOutOfTheWay()
    {
        var got = ElementImages.UniqueName(_ => false, "CON.png");
        Assert.False(LookName.IsReservedDeviceName(got), $"'{got}' is still a device");
    }

    [Fact]
    public void ANameThatCleansAwayToNothingStillBecomesAFile()
    {
        var got = ElementImages.UniqueName(_ => false, "///.png");
        Assert.True(ElementImages.IsStoredName(got));
    }

    [Theory]
    [InlineData("a.png", true)]
    [InlineData("a.JPG", true)]
    [InlineData("a.dds", true)]
    [InlineData("a.exe", false)]
    [InlineData("a.mp4", false)]
    [InlineData("a", false)]
    public void OnlyImageExtensionsAreAccepted(string path, bool ok)
        => Assert.Equal(ok, ElementImages.IsAllowedExtension(path));

    [Fact]
    public void ImportingSomethingThatIsNotThereFailsWithoutThrowing()
    {
        Assert.False(ElementImages.Import(@"C:\no\such\file.png", out var name, out var err));
        Assert.Equal("", name);
        Assert.NotEqual("", err);
    }
}
