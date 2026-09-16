using System;
using System.IO;
using GPoseStudio;
using Xunit;

public class LookWatchTests
{
    private static PluginConfig Saved(string name)
    {
        var c = new PluginConfig { Exposure = 0.25f };
        Assert.True(LookStore.Save(name, c, out var err), err);
        return c;
    }

    [Fact]
    public void StampIsStableWhileNothingChanges()
    {
        Saved("watch-stable");
        var first = LookStore.FolderStamp();
        Assert.NotEqual("", first);
        Assert.Equal(first, LookStore.FolderStamp());
    }

    [Fact]
    public void StampMovesWhenALookIsAdded()
    {
        Saved("watch-added-one");
        var before = LookStore.FolderStamp();
        Saved("watch-added-two");
        Assert.NotEqual(before, LookStore.FolderStamp());
    }

    [Fact]
    public void StampMovesWhenALookIsEditedInPlace()
    {
        var c = Saved("watch-edited");
        var before = LookStore.FolderStamp();

        var path = Path.Combine(LookStore.FolderPath, "watch-edited.json");
        c.Exposure = 0.5f;
        File.WriteAllText(path, LookStore.Capture(c));
        File.SetLastWriteTimeUtc(path, File.GetLastWriteTimeUtc(path).AddSeconds(1));

        Assert.NotEqual(before, LookStore.FolderStamp());
    }

    [Fact]
    public void StampMovesWhenALookIsDeleted()
    {
        Saved("watch-deleted");
        var before = LookStore.FolderStamp();
        Assert.True(LookStore.Delete("watch-deleted", out var err), err);
        Assert.NotEqual(before, LookStore.FolderStamp());
    }

    [Fact]
    public void WriteTimeIsDefaultForALookThatIsNotThere()
    {
        Assert.Equal(default, LookStore.WriteTimeOf("watch-never-existed"));
        Assert.Equal(default, LookStore.WriteTimeOf(""));
        Assert.Equal(default, LookStore.WriteTimeOf(null));
    }

    [Fact]
    public void WriteTimeMovesWhenTheFileIsRewritten()
    {
        var c = Saved("watch-rewritten");
        var before = LookStore.WriteTimeOf("watch-rewritten");
        Assert.NotEqual(default, before);

        var path = Path.Combine(LookStore.FolderPath, "watch-rewritten.json");
        File.SetLastWriteTimeUtc(path, before.AddSeconds(2));

        Assert.True(LookStore.WriteTimeOf("watch-rewritten") > before);
    }

    [Fact]
    public void WriteTimeRefusesANameThatIsNotUsable()
    {
        Assert.Equal(default, LookStore.WriteTimeOf("../escape"));
        Assert.Equal(default, LookStore.WriteTimeOf("CON"));
    }
}
