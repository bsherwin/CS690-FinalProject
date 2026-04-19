using JotIt.Models;

namespace JotIt.Tests;

[Collection("ConsoleTests")]
public class TaskItemTests
{
    private static StringReader Input(params string[] lines) =>
        new StringReader(string.Join("\n", lines) + "\n");

    [Fact]
    public void Create_InvalidBody_ShouldReturnNull()
    {
        Console.SetIn(Input(""));
        var task = TaskItem.Create();
        Assert.Null(task);
    }

    [Fact]
    public void Create_ValidInput_ShouldReturnTaskItem()
    {
        Console.SetIn(Input("This is a test task.", "1"));
        var task = TaskItem.Create();
        Assert.NotNull(task);
        Assert.Equal("This is a test task.", task.Body);
    }

    [Theory]
    [InlineData("1", "")]
    [InlineData("", "")]
    [InlineData("2", "Personal")]
    [InlineData("3", "Work")]
    [InlineData("4", "Home")]    
    public void Create_MapsCategory_Correctly(string input, string expectedCategory)
    {
        Console.SetIn(Input("My Task", input));
        var result = TaskItem.Create();
        Assert.NotNull(result);
        Assert.Equal(expectedCategory, result.Category);
    }

    [Fact]
    public void Create_InvalidCategory_ShouldReturnNull()
    {
        Console.SetIn(Input("My Task", "5"));
        var result = TaskItem.Create();
        Assert.Null(result);
    }

    [Fact]
    public void Create_ReturnsNull_WhenDateFormatIsInvalid()
    {
        Console.SetIn(Input("My Task", "1", "25/12/2026"));
        var result = TaskItem.Create();
        Assert.Null(result);
    }


}