namespace JotIt.Models;

public class TaskItem : Item
{
    public string DueDate { get; set; } = string.Empty;

    public static TaskItem? Create(string body, string? category, string dueDate)
    {
        if (body == null || dueDate == null) return null;
        return new TaskItem { Body = body, Category = category, DueDate = dueDate };
    }
}
