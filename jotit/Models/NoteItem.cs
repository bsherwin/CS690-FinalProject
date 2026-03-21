namespace JotIt.Models;

public class NoteItem : Item
{
    public static NoteItem? Create()
    {
        Console.Write("Enter your note: ");
        string? body = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(body))
        {
            Console.WriteLine("Note cannot be empty.");
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

        return new NoteItem { Body = body, Category = category };
    }
}
