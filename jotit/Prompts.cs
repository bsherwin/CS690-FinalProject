namespace JotIt;

public static class Prompts
{
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
