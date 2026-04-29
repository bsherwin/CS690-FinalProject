using JotIt.Data;
using JotIt.Models;

namespace JotIt;

public class CliHandler(ItemRepository repo)
{
    public void Handle(string[] args)
    {
        string verb = args[0].ToLower();

        if (verb == "--help" || verb == "help")
        {
            PrintHelp();
            return;
        }

        switch (verb)
        {
            case "add":
                CliAdd(args[1..]);
                break;
            case "list":
                CliList(args[1..]);
                break;
            case "delete":
                CliDelete(args[1..]);
                break;
            case "change":
                CliChange(args[1..]);
                break;
            default:
                Console.WriteLine($"Unknown command: {verb}");
                PrintHelp();
                break;
        }
    }

    void CliAdd(string[] args)
    {
        if (args.Length == 0) { PrintHelp(); return; }

        string type = args[0].ToLower();
        var (body, flags) = ParseArgs(args[1..]);

        if (string.IsNullOrWhiteSpace(body))
        {
            Console.WriteLine("Body cannot be empty.");
            return;
        }

        string category = flags.GetValueOrDefault("--category", "");

        switch (type)
        {
            case "note":
                var note = NoteItem.Create(body, category);
                if (note == null) return;
                repo.Add(note);
                Console.WriteLine("Note saved!");
                break;
            case "task":
                string? dueDate = Prompts.GetDueDate(flags.GetValueOrDefault("--due", DateTime.Today.ToString("yyyy-MM-dd")));
                if (dueDate is null) return;
                var task = TaskItem.Create(body, dueDate, category);
                if (task == null) return;
                repo.Add(task);
                Console.WriteLine("Task saved!");
                break;
            default:
                Console.WriteLine($"Unknown type: {args[0]}");
                PrintHelp();
                break;
        }
    }

    void CliList(string[] args)
    {
        string filter = args.Length > 0 ? args[0].ToLower() : "all";
        switch (filter)
        {
            case "notes":
                new NoteCollection(repo).Display();
                break;
            case "tasks":
                new TaskCollection(repo).Display();
                break;
            default:
                if (filter != "all")
                    Console.WriteLine($"Unknown filter '{filter}', showing all.");
                new NoteCollection(repo).Display();
                new TaskCollection(repo).Display();
                break;
        }
    }

    void CliDelete(string[] args)
    {
        if (args.Length == 0 || !long.TryParse(args[0], out long id))
        {
            Console.WriteLine("Usage: jotit delete <id>");
            return;
        }

        if (repo.Delete(id))
            Console.WriteLine("Item deleted.");
        else
            Console.WriteLine("No item found with that ID.");
    }

    void CliChange(string[] args)
    {
        if (args.Length == 0 || !long.TryParse(args[0], out long id))
        {
            Console.WriteLine("Usage: jotit change <id> [note|task] [--due yyyy-MM-dd] [--category <string>]");
            return;
        }

        var item = repo.GetItemById(id);
        if (item == null) { Console.WriteLine("No item found with that ID."); return; }

        string? typeArg = args.Length > 1 && !args[1].StartsWith("--") ? args[1].ToLower() : null;
        if (typeArg is not null and not ("note" or "task"))
        {
            Console.WriteLine("Usage: jotit change <id> [note|task] [--due yyyy-MM-dd] [--category <string>]");
            return;
        }

        var (_, flags) = ParseArgs(args[(typeArg is null ? 1 : 2)..]); 

        if (typeArg == "note" && item is TaskItem)
        {
            repo.UpdateDueDate(id, null);
            Console.WriteLine("Task converted to note.");
        }
        else if (typeArg == "task" && item is NoteItem)
        {
            string? dueDate = Prompts.GetDueDate(flags.GetValueOrDefault("--due", DateTime.Today.ToString("yyyy-MM-dd")));
            if (dueDate is null) return;
            repo.UpdateDueDate(id, dueDate);
            Console.WriteLine("Note converted to task.");
        }
        else if (typeArg == "note" && item is NoteItem)
        {
            Console.WriteLine("Item is already a note.");
        }
        else if (typeArg == "task" && item is TaskItem)
        {
            if (!flags.ContainsKey("--due") && !flags.ContainsKey("--category"))
                Console.WriteLine("Item is already a task.");
        }
        else if (typeArg is null && !flags.ContainsKey("--category") && !flags.ContainsKey("--due"))
        {
            Console.WriteLine("Usage: jotit change <id> [note|task] [--due yyyy-MM-dd] [--category <string>]");
            return;
        }

        if (flags.TryGetValue("--category", out string? category))
        {
            repo.UpdateCategory(id, category);
            Console.WriteLine($"Category updated to '{category}'.");
        }

        if (flags.TryGetValue("--due", out string? dueDateFlag))
        {
            if (typeArg == "note")
                Console.WriteLine("Warning: --due is ignored when changing to a note.");
            else if (item is NoteItem && typeArg is null)
                Console.WriteLine($"Error: item {id} is a note and cannot have a due date. To convert it to a task, run: jotit change {id} task --due {dueDateFlag}");
            else if (item is TaskItem)
            {
                string? dueDate = Prompts.GetDueDate(dueDateFlag);
                if (dueDate is null) return;
                repo.UpdateDueDate(id, dueDate);
                Console.WriteLine($"Due date updated to '{dueDate}'.");
            }
        }
    }

    static (string body, Dictionary<string, string> flags) ParseArgs(string[] args)
    {
        var flags = new Dictionary<string, string>();
        var bodyTokens = new List<string>();

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].StartsWith("--") && i + 1 < args.Length)
            {
                flags[args[i]] = args[i + 1];
                i++;
            }
            else if (!args[i].StartsWith("--"))
            {
                bodyTokens.Add(args[i]);
            }
        }

        return (string.Join(" ", bodyTokens), flags);
    }

    static void PrintHelp()
    {
        Console.WriteLine();
        Console.WriteLine("Usage: jotit <command> [options]");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  add note <body> [--category <string>]");
        Console.WriteLine("  add task <body> [--due yyyy-MM-dd] [--category <string>]");
        Console.WriteLine("  list [notes|tasks]");
        Console.WriteLine("  delete <id>");
        Console.WriteLine("  change <id> [note|task] [--due yyyy-MM-dd] [--category <string>]");
        Console.WriteLine();
        Console.WriteLine("Run without arguments to launch the interactive menu.");
    }
}
