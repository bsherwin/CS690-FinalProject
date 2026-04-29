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
        var task = TaskItem.Create("", "1/1/2026");
        Assert.Null(task);
    }

    [Fact]
    public void Create_ValidInput_ShouldReturnTaskItem()
    {
        var task = TaskItem.Create("This is a test task.", "1/1/2026", "1");
        Assert.NotNull(task);
        Assert.Equal("This is a test task.", task.Body);
    }

    [Fact]
    public void Create_ReturnsNull_WhenDateFormatIsInvalid()
    {
        var result = TaskItem.Create("My Task", "25/12/2026");
        Assert.Null(result);
    }
}
