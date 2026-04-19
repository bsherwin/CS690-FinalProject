using JotIt.Data;
using JotIt.Models;
using Microsoft.Data.Sqlite;

namespace JotIt.Tests;

public class ItemRepositoryTests : IDisposable
{
    private readonly ItemRepository _repository;
    private readonly string _dbPath;
    
    public ItemRepositoryTests()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"jotit_test_{Guid.NewGuid()}.db");
        _repository = new ItemRepository(_dbPath);
    }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();
        if (File.Exists(_dbPath))
        {
            File.Delete(_dbPath);
        }
    }

    [Fact]
    public void AddTaskItem_ShouldAddItemToRepository() {
        var taskItem = new TaskItem
        {
            Body = "Test Task",
            Category = "Work",
            DueDate = DateTime.Today.AddDays(7).ToString("yyyy-MM-dd")
        };
        _repository.Add(taskItem);
        var items = _repository.GetTasks();
        Assert.Contains(items, item => item.Body == "Test Task" && item.Category == "Work" && ((TaskItem)item).DueDate == taskItem.DueDate);
    }

    [Fact]
    public void AddNoteItem_ShouldAddItemToRepository() {
        var noteItem = new NoteItem
        {
            Body = "Test Note",
            Category = "Personal"
        };
        _repository.Add(noteItem);
        var items = _repository.GetNotes();
        Assert.Contains(items, item => item.Body == "Test Note" && item.Category == "Personal");
    }

    [Fact]
    public void GetNotes_ShouldNotReturnTasks()
    {
        _repository.Add(new TaskItem { Body = "A task", Category = "", DueDate = "2026-12-01" });
        var notes = _repository.GetNotes();
        Assert.Empty(notes);
    }

    [Fact]
    public void GetTasks_ShouldNotReturnNotes()
    {
        _repository.Add(new NoteItem { Body = "A note", Category = "" });
        var tasks = _repository.GetTasks();
        Assert.Empty(tasks);
    }
}