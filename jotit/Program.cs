using JotIt.Data;
using JotIt.Models;

var repo = new ItemRepository("jotit.db");
bool running = true;

while (running)
{
    Console.WriteLine();
    Console.WriteLine("=== JotIt ===");
    Console.WriteLine("1. Add");
    Console.WriteLine("2. List");
    Console.WriteLine("3. Delete");
    Console.WriteLine("4. Quit");
    Console.Write("Select an option: ");

    string? choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
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
                    AddNote();
                    break;
                case "2":
                    AddTask();
                    break;
                case "3":
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
            break;
        case "2":
            ListItems();
            break;
        case "3":
            DeleteItem();
            break;
        case "4":
            running = false;
            break;
        default:
            Console.WriteLine("Invalid option. Please try again.");
            break;
    }
}

Console.WriteLine("Goodbye!");

void AddNote()
{
    var note = NoteItem.Create();
    if (note == null) return;

    repo.Add(note);
    Console.WriteLine("Note saved!");
}

void AddTask()
{
    var task = TaskItem.Create();
    if (task == null) return;

    repo.Add(task);
    Console.WriteLine("Task saved!");
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
    new NoteCollection(repo).Display();
    new TaskCollection(repo).Display();

    Console.WriteLine();
    Console.Write("Enter the ID of the item to delete (or 0 to cancel): ");
    string? input = Console.ReadLine();

    if (!int.TryParse(input, out int id) || id <= 0)
    {
        if (id != 0)
            Console.WriteLine("Invalid ID.");
        return;
    }

    if (repo.Delete(id))
        Console.WriteLine("Item deleted.");
    else
        Console.WriteLine("No item found with that ID.");
}
