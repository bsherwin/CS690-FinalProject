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
        Console.SetIn(Input(""));
        var note = NoteItem.Create();
        Assert.Null(note);
    }

    [Fact]
    public void Create_ValidInput_ShouldReturnNoteItem()
    {
        Console.SetIn(Input("This is a test note.", "1"));
        var note = NoteItem.Create();
        Assert.NotNull(note);
        Assert.Equal("This is a test note.", note.Body);
    }

    [Theory]
    [InlineData("1", "")]
    [InlineData("", "")]
    [InlineData("2", "Personal")]
    [InlineData("3", "Work")]
    [InlineData("4", "Home")]    
    public void Create_MapsCategory_Correctly(string input, string expectedCategory)
    {
        Console.SetIn(Input("My Note", input));
        var result = NoteItem.Create();
        Assert.NotNull(result);
        Assert.Equal(expectedCategory, result.Category);
    }

    [Fact]
    public void Create_InvalidCategory_ShouldReturnNull()
    {
        Console.SetIn(Input("My Note", "5"));
        var result = NoteItem.Create();
        Assert.Null(result);
    }

}