using JotIt;
using JotIt.Data;
using JotIt.Models;

var repo = new ItemRepository("jotit.db");

if (args.Length > 0)
{
    new CliHandler(repo).Handle(args);
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
            AddNote();
            break;
        }
        case "2":
        {
            AddTask();
            break;
        }
        case "3":
            break;
        default:
            Console.WriteLine("Invalid option. Please try again.");
            break;
    }
}

void AddNote()
{
    string? body = Prompts.GetBody();
    if (body == null) return;
    string? category = Prompts.GetCategory();
    var note = NoteItem.Create(body, category);
    if (note != null) { repo.Add(note); Console.WriteLine("Note saved!"); }
}

void AddTask()
{
    string? body = Prompts.GetBody();
    if (body == null) return;
    string? category = Prompts.GetCategory();
    string? dueDate = Prompts.GetDueDate();
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

    string? dueDate = Prompts.GetDueDate();
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

void ChangeItem()
{
    new NoteCollection(repo).Display();
    new TaskCollection(repo).Display();

    Console.WriteLine();
    Console.Write("Enter the ID of the item to change (or 0 to cancel): ");
    string? input = Console.ReadLine();

    if (!int.TryParse(input, out int id) || id <= 0)
    {
        if (id != 0)
            Console.WriteLine("Invalid ID.");
        return;
    }

    var item = repo.GetById(id);
    if (item == null)
    {
        Console.WriteLine("No item found with that ID.");
        return;
    }

    if (item is NoteItem)
    {
        Console.Write($"Enter due date (yyyy-MM-dd) [{DateTime.Today:yyyy-MM-dd}]: ");
        string? dueDateInput = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(dueDateInput))
            dueDateInput = DateTime.Today.ToString("yyyy-MM-dd");
        else if (!DateTime.TryParseExact(dueDateInput, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out _))
        {
            Console.WriteLine("Invalid date format.");
            return;
        }

        repo.SetDueDate(id, dueDateInput);
        Console.WriteLine("Note converted to task.");
    }
    else
    {
        repo.SetDueDate(id, null);
        Console.WriteLine("Task converted to note.");
    }
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
