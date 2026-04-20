using JotIt.Data;
using JotIt.Models;
using Microsoft.Data.Sqlite;

namespace JotIt.Tests;

[Collection("ConsoleTests")]
public class NoteCollectionTests : IDisposable
{
    private readonly ItemRepository _repository;
    private readonly string _dbPath;

    public NoteCollectionTests()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"jotit_notecollection_test_{Guid.NewGuid()}.db");
        _repository = new ItemRepository(_dbPath);
    }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();
        if (File.Exists(_dbPath))
            File.Delete(_dbPath);
    }

    private NoteCollection CreateCollection() => new NoteCollection(_repository);

    [Fact]
    public void Display_NoNotes_PrintsNoNotesFound()
    {
        var collection = CreateCollection();
        var output = new StringWriter();
        Console.SetOut(output);

        collection.Display();

        Assert.Contains("No notes found.", output.ToString());
    }

    [Fact]
    public void Display_SingleNoteNoCategory_PrintsNoteBody()
    {
        _repository.Add(new NoteItem { Body = "Hello world", Category = "" });
        var collection = CreateCollection();
        var output = new StringWriter();
        Console.SetOut(output);

        collection.Display();

        Assert.Contains("Hello world", output.ToString());
    }

    [Fact]
    public void Display_NoteWithCategory_PrintsCategoryAndBody()
    {
        _repository.Add(new NoteItem { Body = "Buy milk", Category = "Personal" });
        var collection = CreateCollection();
        var output = new StringWriter();
        Console.SetOut(output);

        collection.Display();

        string result = output.ToString();
        Assert.Contains("Personal", result);
        Assert.Contains("Buy milk", result);
    }

    [Fact]
    public void Display_MultipleNotes_PrintsAllBodies()
    {
        _repository.Add(new NoteItem { Body = "Note one", Category = "" });
        _repository.Add(new NoteItem { Body = "Note two", Category = "Work" });
        _repository.Add(new NoteItem { Body = "Note three", Category = "Home" });
        var collection = CreateCollection();
        var output = new StringWriter();
        Console.SetOut(output);

        collection.Display();

        string result = output.ToString();
        Assert.Contains("Note one", result);
        Assert.Contains("Note two", result);
        Assert.Contains("Note three", result);
    }

    [Fact]
    public void Display_OnlyTasksInRepo_PrintsNoNotesFound()
    {
        _repository.Add(new TaskItem { Body = "A task", Category = "", DueDate = "2026-12-01" });
        var collection = CreateCollection();
        var output = new StringWriter();
        Console.SetOut(output);

        collection.Display();

        Assert.Contains("No notes found.", output.ToString());
    }

    [Fact]
    public void Display_Always_PrintsHeader()
    {
        var collection = CreateCollection();
        var output = new StringWriter();
        Console.SetOut(output);

        collection.Display();

        Assert.Contains("--- Notes ---", output.ToString());
    }

    [Fact]
    public void Display_SingleNote_PrintsNoteId()
    {
        _repository.Add(new NoteItem { Body = "Check the ID", Category = "" });
        long id = _repository.GetNotes()[0].Id;
        var collection = CreateCollection();
        var output = new StringWriter();
        Console.SetOut(output);

        collection.Display();

        Assert.Contains(id.ToString(), output.ToString());
    }

    [Fact]
    public void Display_MultipleCategories_WorkAppearsBeforePersonal()
    {
        _repository.Add(new NoteItem { Body = "Personal note", Category = "Personal" });
        _repository.Add(new NoteItem { Body = "Work note", Category = "Work" });
        var collection = CreateCollection();
        var output = new StringWriter();
        Console.SetOut(output);

        collection.Display();

        string result = output.ToString();
        int workIndex = result.IndexOf("Work note", StringComparison.Ordinal);
        int personalIndex = result.IndexOf("Personal note", StringComparison.Ordinal);
        Assert.True(workIndex < personalIndex, "Work notes should appear before Personal notes.");
    }
}
