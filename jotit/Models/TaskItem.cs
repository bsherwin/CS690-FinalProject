namespace JotIt.Models;

public class TaskItem : Item
{
    public string DueDate { get; set; } = string.Empty;

    public static TaskItem? Create()
    {
        Console.Write("Enter your task: ");
        string? body = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(body))
        {
            Console.WriteLine("Task cannot be empty.");
            return null;
        }

        Console.WriteLine("Select a category:");
        Console.WriteLine("1. None (default)");
        Console.WriteLine("2. Personal");
        Console.WriteLine("3. Work");
        Console.WriteLine("4. Home");
        Console.Write("Select an option: ");
        string? catChoice = Console.ReadLine();
        string category = catChoice switch
        {
            "" or "1" => "",
            "2" => "Personal",
            "3" => "Work",
            "4" => "Home",
            _ => "\0"
        };

        if (category == "\0")
        {
            Console.WriteLine("Invalid category.");
            return null;
        }

        Console.Write($"Enter due date (yyyy-MM-dd) [{DateTime.Today:yyyy-MM-dd}]: ");
        string? dueDateInput = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(dueDateInput))
        {
            dueDateInput = DateTime.Today.ToString("yyyy-MM-dd");
        }
        else if (!DateTime.TryParseExact(dueDateInput, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out _))
        {
            Console.WriteLine("Invalid date format.");
            return null;
        }

        return new TaskItem { Body = body, Category = category, DueDate = dueDateInput };
    }

}
