using JotIt.Models;

namespace JotIt.Tests;

[Collection("ConsoleTests")]
public class NoteItemTests
{
    private static StringReader Input(params string[] lines) =>
        new StringReader(string.Join("\n", lines) + "\n");

    [Fact]
    public void Create_InvalidBody_ShouldReturnNull()
    {
        var note = NoteItem.Create("");
        Assert.Null(note);
    }

    [Fact]
    public void Create_ValidInput_ShouldReturnNoteItem()
    {
        var note = NoteItem.Create("This is a test note.", "1");
        Assert.NotNull(note);
        Assert.Equal("This is a test note.", note.Body);
    }
}
