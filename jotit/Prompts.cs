namespace JotIt;

public static class Prompts
{
    public static string? GetBody()
    {
        Console.Write("Enter your note: ");
        string? body = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(body))
        {
            Console.WriteLine("Note cannot be empty.");
            return null;
        }
        return body;
    }

    public static string? GetCategory()
    {
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
        return category;
    }

    public static string? GetDueDate(string? initial = null)
    {
        string? input;

        if (initial is not null)
        {
            input = initial; // CLI path: validate the provided value directly
        }
        else
        {
            // Interactive path: prompt the user
            Console.Write($"Enter due date (yyyy-MM-dd) [{DateTime.Today:yyyy-MM-dd}]: ");
            input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
                return DateTime.Today.ToString("yyyy-MM-dd");
        }

        if (!DateTime.TryParseExact(input, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out _))
        {
            Console.WriteLine("Invalid date format. Expected yyyy-MM-dd.");
            return null;
        }
        return input;
    }
}
