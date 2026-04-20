using JotIt.Data;
using JotIt.Models;
using Microsoft.Data.Sqlite;

namespace JotIt.Tests;

[Collection("ConsoleTests")]
public class TaskCollectionTests : IDisposable
{
    private readonly ItemRepository _repository;
    private readonly string _dbPath;

    public TaskCollectionTests()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"jotit_taskcollection_test_{Guid.NewGuid()}.db");
        _repository = new ItemRepository(_dbPath);
    }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();
        if (File.Exists(_dbPath))
            File.Delete(_dbPath);
    }

    private TaskCollection CreateCollection() => new TaskCollection(_repository);

    [Fact]
    public void Display_NoTasks_PrintsNoTasksFound()
    {
        var collection = CreateCollection();
        var output = new StringWriter();
        Console.SetOut(output);

        collection.Display();

        Assert.Contains("No tasks found.", output.ToString());
    }

    [Fact]
    public void Display_Always_PrintsHeader()
    {
        var collection = CreateCollection();
        var output = new StringWriter();
        Console.SetOut(output);

        collection.Display();

        Assert.Contains("--- Tasks ---", output.ToString());
    }

    [Fact]
    public void Display_SingleTaskNoCategory_PrintsBodyAndDueDate()
    {
        _repository.Add(new TaskItem { Body = "Fix the bug", Category = "", DueDate = "2026-05-01" });
        var collection = CreateCollection();
        var output = new StringWriter();
        Console.SetOut(output);

        collection.Display();

        string result = output.ToString();
        Assert.Contains("Fix the bug", result);
        Assert.Contains("2026-05-01", result);
    }

    [Fact]
    public void Display_TaskWithCategory_PrintsCategoryAndBody()
    {
        _repository.Add(new TaskItem { Body = "Submit report", Category = "Work", DueDate = "2026-06-15" });
        var collection = CreateCollection();
        var output = new StringWriter();
        Console.SetOut(output);

        collection.Display();

        string result = output.ToString();
        Assert.Contains("Work", result);
        Assert.Contains("Submit report", result);
    }

    [Fact]
    public void Display_MultipleTasks_PrintsAllBodiesAndDueDates()
    {
        _repository.Add(new TaskItem { Body = "Task alpha", Category = "", DueDate = "2026-04-20" });
        _repository.Add(new TaskItem { Body = "Task beta", Category = "Work", DueDate = "2026-04-21" });
        _repository.Add(new TaskItem { Body = "Task gamma", Category = "Home", DueDate = "2026-04-22" });
        var collection = CreateCollection();
        var output = new StringWriter();
        Console.SetOut(output);

        collection.Display();

        string result = output.ToString();
        Assert.Contains("Task alpha", result);
        Assert.Contains("Task beta", result);
        Assert.Contains("Task gamma", result);
        Assert.Contains("2026-04-20", result);
        Assert.Contains("2026-04-21", result);
        Assert.Contains("2026-04-22", result);
    }

    [Fact]
    public void Display_OnlyNotesInRepo_PrintsNoTasksFound()
    {
        _repository.Add(new NoteItem { Body = "Just a note", Category = "" });
        var collection = CreateCollection();
        var output = new StringWriter();
        Console.SetOut(output);

        collection.Display();

        Assert.Contains("No tasks found.", output.ToString());
    }

    [Fact]
    public void Display_SingleTask_PrintsTaskId()
    {
        _repository.Add(new TaskItem { Body = "Check the ID", Category = "", DueDate = "2026-09-01" });
        long id = _repository.GetTasks()[0].Id;
        var collection = CreateCollection();
        var output = new StringWriter();
        Console.SetOut(output);

        collection.Display();

        Assert.Contains(id.ToString(), output.ToString());
    }

    [Fact]
    public void Display_Task_PrintsDueLabel()
    {
        _repository.Add(new TaskItem { Body = "Check label", Category = "", DueDate = "2026-07-04" });
        var collection = CreateCollection();
        var output = new StringWriter();
        Console.SetOut(output);

        collection.Display();

        Assert.Contains("Due:", output.ToString());
    }

    [Fact]
    public void Display_MultipleCategories_WorkAppearsBeforePersonal()
    {
        _repository.Add(new TaskItem { Body = "Personal task", Category = "Personal", DueDate = "2026-05-01" });
        _repository.Add(new TaskItem { Body = "Work task", Category = "Work", DueDate = "2026-05-01" });
        var collection = CreateCollection();
        var output = new StringWriter();
        Console.SetOut(output);

        collection.Display();

        string result = output.ToString();
        int workIndex = result.IndexOf("Work task", StringComparison.Ordinal);
        int personalIndex = result.IndexOf("Personal task", StringComparison.Ordinal);
        Assert.True(workIndex < personalIndex, "Work tasks should appear before Personal tasks.");
    }

    [Fact]
    public void Display_SameCategory_EarlierDueDateAppearsFirst()
    {
        _repository.Add(new TaskItem { Body = "Later task", Category = "Work", DueDate = "2026-12-31" });
        _repository.Add(new TaskItem { Body = "Earlier task", Category = "Work", DueDate = "2026-04-25" });
        var collection = CreateCollection();
        var output = new StringWriter();
        Console.SetOut(output);

        collection.Display();

        string result = output.ToString();
        int earlierIndex = result.IndexOf("Earlier task", StringComparison.Ordinal);
        int laterIndex = result.IndexOf("Later task", StringComparison.Ordinal);
        Assert.True(earlierIndex < laterIndex, "Earlier due dates should appear before later due dates within the same category.");
    }
}
