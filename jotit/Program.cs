using JotIt.Data;
using JotIt.Models;

var repo = new ItemRepository("jotit.db");

if (args.Length > 0)
{
    HandleCli(args);
    return;
}

bool running = true;

while (running)
{
    Console.WriteLine();
    Console.WriteLine("=== JotIt ===");
    Console.WriteLine("1. Add");
    Console.WriteLine("2. Change");
    Console.WriteLine("3. List");
    Console.WriteLine("4. Delete");
    Console.WriteLine("5. Quit");
    Console.Write("Select an option: ");

    string? choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            AddItem();
            break;
        case "2":
            ChangeItem();
            break;
        case "3":
            ListItems();
            break;
        case "4":
            DeleteItem();
            break;
        case "5":
            running = false;
            break;
        default:
            Console.WriteLine("Invalid option. Please try again.");
            break;
    }
}

Console.WriteLine("Goodbye!");

string? GetBody()
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

string? GetCategory()
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

string? GetDueDate(string? initial = null)
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

void AddItem() {
    Console.WriteLine();
    Console.WriteLine("--- Add ---");
    Console.WriteLine("1. New Note");
    Console.WriteLine("2. New Task");
    Console.WriteLine("3. Back");
    Console.Write("Select an option: ");

    string? addChoice = Console.ReadLine();

    switch (addChoice)
    {
        case "1":
        {
            string? body = GetBody();
            if (body == null) return;
            string? category = GetCategory();
            var note = NoteItem.Create(body, category);
            if (note != null) { repo.Add(note); Console.WriteLine("Note saved!"); }
            break;
        }
        case "2":
        {
            string? body = GetBody();
            if (body == null) return;
            string? category = GetCategory();
            string? dueDate = GetDueDate();
            if (dueDate == null) {
                var note = NoteItem.Create(body, category);
                if (note != null) { 
                    repo.Add(note);
                    Console.WriteLine("Task created as a note due to invalid date.");
                    Console.WriteLine("Note saved!"); 
                }
                return;
            }
            var task = TaskItem.Create(body, category, dueDate);
            if (task != null) { repo.Add(task); Console.WriteLine("Task saved!"); }
            break;
        }
        case "3":
            break;
        default:
            Console.WriteLine("Invalid option. Please try again.");
            break;
    }
}

int SelectNoteOrTask() {
    new NoteCollection(repo).Display();
    new TaskCollection(repo).Display();

    Console.WriteLine();
    Console.Write("Enter item ID (or 0 to cancel): ");
    string? input = Console.ReadLine();

    if (!int.TryParse(input, out int id) || id <= 0)
    {
        if (id != 0)
            Console.WriteLine("Invalid ID.");
        return 0;
    }
    return id;
}

void ChangeItem()
{
    int id = SelectNoteOrTask();
    if (id == 0) return;  // 0 means cancel

    var item = repo.GetItemById(id);
    if (item == null)
    {
        Console.WriteLine("No item found with that ID.");
        return;
    }

    if (item is NoteItem note)
        ConvertNoteToTask(note);
    else if (item is TaskItem task)
        ConvertTaskToNote(task);
}

void ConvertNoteToTask(NoteItem note)
{
    Console.WriteLine();
    Console.WriteLine(note.ToString());

    string? dueDate = GetDueDate();
    if (dueDate == null) return;
    repo.UpdateDueDate(note.Id, dueDate);
    Console.WriteLine("Note converted to task.");
}

void ConvertTaskToNote(TaskItem task)
{
    Console.WriteLine();
    Console.WriteLine(task.ToString());

    repo.UpdateDueDate(task.Id, null);
    Console.WriteLine("Task converted to note.");
}

void ListItems()
{
    Console.WriteLine();
    Console.WriteLine("--- List ---");
    Console.WriteLine("1. All Items");
    Console.WriteLine("2. Notes");
    Console.WriteLine("3. Tasks");
    Console.WriteLine("4. Back");
    Console.Write("Select an option: ");

    string? listChoice = Console.ReadLine();

    switch (listChoice)
    {
        case "1":
            new NoteCollection(repo).Display();
            new TaskCollection(repo).Display();
            break;
        case "2":
            new NoteCollection(repo).Display();
            break;
        case "3":
            new TaskCollection(repo).Display();
            break;
        case "4":
            break;
        default:
            Console.WriteLine("Invalid option. Please try again.");
            break;
    }
}

void DeleteItem()
{
    int id = SelectNoteOrTask();
    if (id == 0) return; // 0 means cancel

    if (repo.Delete(id))
        Console.WriteLine("Item deleted.");
    else
        Console.WriteLine("No item found with that ID.");
}

void HandleCli(string[] args)
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
            string? dueDate = GetDueDate(flags.GetValueOrDefault("--due", DateTime.Today.ToString("yyyy-MM-dd")));
            if (dueDate is null) return;
            var task = TaskItem.Create(body, category, dueDate);
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
            if (filter != "all") {
                Console.WriteLine($"Unknown filter '{filter}', showing all.");
            }
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
        Console.WriteLine("Usage: jotit change <id> [--due yyyy-MM-dd]");
        return;
    }

    var item = repo.GetItemById(id);
    if (item == null) { Console.WriteLine("No item found with that ID."); return; }

    var (_, flags) = ParseArgs(args[1..]);

    if (item is NoteItem)
    {
        string? dueDate = GetDueDate(flags.GetValueOrDefault("--due", DateTime.Today.ToString("yyyy-MM-dd")));
        if (dueDate is null) return;
        repo.UpdateDueDate(id, dueDate);
        Console.WriteLine("Note converted to task.");
    }
    else
    {
        repo.UpdateDueDate(id, null);
        Console.WriteLine("Task converted to note.");
    }
}

(string body, Dictionary<string, string> flags) ParseArgs(string[] args)
{
    var flags = new Dictionary<string, string>();
    var bodyTokens = new List<string>();

    for (int i = 0; i < args.Length; i++)
    {
        if (args[i].StartsWith("--") && i + 1 < args.Length)
        {
            flags[args[i]] = args[i + 1];
            i++; // skip the value
        }
        else if (!args[i].StartsWith("--"))
        {
            bodyTokens.Add(args[i]);
        }
    }

    return (string.Join(" ", bodyTokens), flags);
}

void PrintHelp()
{
    Console.WriteLine();
    Console.WriteLine("Usage: jotit <command> [options]");
    Console.WriteLine();
    Console.WriteLine("Commands:");
    Console.WriteLine("  add note <body> [--category <string>]");
    Console.WriteLine("  add task <body> [--due yyyy-MM-dd] [--category <string>]");
    Console.WriteLine("  list [notes|tasks]");
    Console.WriteLine("  delete <id>");
    Console.WriteLine("  change <id> [--due yyyy-MM-dd]");
    Console.WriteLine();
    Console.WriteLine("Run without arguments to launch the interactive menu.");
}
