using JotIt.Data;

namespace JotIt.Models;

public class NoteCollection
{
    private readonly List<NoteItem> _notes;

    public NoteCollection(ItemRepository repo)
    {
        _notes = repo.GetNotes();
    }

    public void Display()
    {
        Console.WriteLine();
        Console.WriteLine("--- Notes ---");
        if (_notes.Count == 0)
        {
            Console.WriteLine("No notes found.");
            return;
        }

        int maxId = _notes.Max(n => n.Id.ToString().Length);
        int maxCat = _notes.Max(n => n.Category.Length);

        foreach (var note in _notes)
        {
            string id = note.Id.ToString().PadLeft(maxId);
            string cat = maxCat > 0
                ? (string.IsNullOrEmpty(note.Category) ? "".PadRight(maxCat + 2) : $"{note.Category}".PadRight(maxCat + 2))
                : "";
            Console.WriteLine($"[{id}]  [{cat}]  {note.Body}");
        }
    }
}
