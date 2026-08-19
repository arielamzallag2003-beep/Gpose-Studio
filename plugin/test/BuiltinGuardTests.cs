using GPoseStudio;
using Xunit;

public class BuiltinGuardTests
{
    [Fact]
    public void AMissingFileIsWritten()
        => Assert.True(BuiltinGuard.MayOverwrite(null, null));

    [Fact]
    public void AFileMatchingWhatWeGeneratedIsReplaced()
    {
        var content = "{\"Exposure\":0.5}";
        Assert.True(BuiltinGuard.MayOverwrite(content, BuiltinGuard.Hash(content)));
    }

    [Fact]
    public void AnEditedFileIsKept()
    {
        var generated = "{\"Exposure\":0.5}";
        var edited = "{\"Exposure\":0.9}";
        Assert.False(BuiltinGuard.MayOverwrite(edited, BuiltinGuard.Hash(generated)));
    }

    [Fact]
    public void AFileOfUnknownProvenanceIsKept()
    {
        Assert.False(BuiltinGuard.MayOverwrite("{\"Exposure\":0.5}", null));
        Assert.False(BuiltinGuard.MayOverwrite("{\"Exposure\":0.5}", ""));
    }

    [Fact]
    public void TheOldBareVersionMarkerStillReads()
    {
        var state = BuiltinGuard.Parse("59");
        Assert.Equal(59, state.Version);
        Assert.Empty(state.Hashes);
    }

    [Fact]
    public void TheNewMarkerRoundTrips()
    {
        var state = new BuiltinGuard.State { Version = 60 };
        state.Hashes["Studio Portrait"] = BuiltinGuard.Hash("x");

        var back = BuiltinGuard.Parse(BuiltinGuard.Write(state));

        Assert.Equal(60, back.Version);
        Assert.Equal(state.Hashes["Studio Portrait"], back.Hashes["Studio Portrait"]);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("{ this is not json")]
    public void ACorruptOrAbsentMarkerReadsAsFreshRatherThanThrowing(string? text)
    {
        var state = BuiltinGuard.Parse(text);
        Assert.Equal(0, state.Version);
        Assert.Empty(state.Hashes);
    }

    [Fact]
    public void HashingIsStableAndDiscriminating()
    {
        Assert.Equal(BuiltinGuard.Hash("abc"), BuiltinGuard.Hash("abc"));
        Assert.NotEqual(BuiltinGuard.Hash("abc"), BuiltinGuard.Hash("abd"));
    }
}
