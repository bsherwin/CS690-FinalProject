namespace JotIt.Models;

public class TaskItem : Item
{
    public string DueDate { get; set; } = string.Empty;

    public static TaskItem? Create(string body, string dueDate, string? category = null)
    {
        if (string.IsNullOrEmpty(body) || string.IsNullOrEmpty(dueDate)) return null;
        if (!DateTime.TryParse(dueDate, out _)) return null;
        return new TaskItem { Body = body, Category = category, DueDate = dueDate };
    }
}
