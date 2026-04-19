using JotIt.Data;

namespace JotIt.Models;

public class TaskCollection
{
    private readonly List<TaskItem> _tasks;

    public TaskCollection(ItemRepository repo)
    {
        _tasks = repo.GetTasks();
    }

    public void Display()
    {
        Console.WriteLine();
        Console.WriteLine("--- Tasks ---");
        if (_tasks.Count == 0)
        {
            Console.WriteLine("No tasks found.");
            return;
        }

        int maxId = _tasks.Max(t => t.Id.ToString().Length);
        int maxCat = _tasks.Max(t => t.Category.Length);
        int maxBody = _tasks.Max(t => t.Body.Length);

        foreach (var task in _tasks)
        {
            string id = task.Id.ToString().PadLeft(maxId);
            string cat = maxCat > 0
                ? (string.IsNullOrEmpty(task.Category) ? "".PadRight(maxCat + 2) : $"[{task.Category}]".PadRight(maxCat + 2))
                : "";
            string body = task.Body.PadRight(maxBody);
            Console.WriteLine($"[{id}]  [{cat}]  {body}  (Due: {task.DueDate})");
        }
    }
}
