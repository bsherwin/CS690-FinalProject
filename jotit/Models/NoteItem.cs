namespace JotIt.Models;

public class NoteItem : Item
{
    public static NoteItem? Create(string body, string? category = null)
    {
        if (string.IsNullOrEmpty(body)) return null;
        return new NoteItem { Body = body, Category = category };
    }
}
