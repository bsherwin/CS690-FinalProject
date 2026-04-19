using JotIt.Data;
using JotIt.Models;

namespace JotIt.Tests;

public class ItemRepositoryTests
{
    private readonly ItemRepository _repository;
    
    public ItemRepositoryTests()
    {
        var _dbPath = Path.Combine(Path.GetTempPath(), "jotit_test.db");
        _repository = new ItemRepository(_dbPath);
    }

    [Fact]
    public void AddTaskItem_ShouldAddItemToRepository() {
        // Arrange
        var taskItem = new TaskItem
        {
            Body = "Test Task",
            Category = "Work",
            DueDate = DateTime.Today.AddDays(7).ToString("yyyy-MM-dd")
        };

        // Act
        _repository.Add(taskItem);
        var items = _repository.GetTasks();

        // Assert
        Assert.Contains(items, item => item.Body == "Test Task" && item.Category == "Work" && ((TaskItem)item).DueDate == taskItem.DueDate);
    }

    [Fact]
    public void AddNoteItem_ShouldAddItemToRepository() {
        // Arrange
        var noteItem = new NoteItem
        {
            Body = "Test Note",
            Category = "Personal"
        };

        // Act
        _repository.Add(noteItem);
        var items = _repository.GetNotes();

        // Assert
        Assert.Contains(items, item => item.Body == "Test Note" && item.Category == "Personal");
    }
    
}