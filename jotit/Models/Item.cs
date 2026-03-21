namespace JotIt.Models;

public abstract class Item
{
    public long Id { get; set; }
    public string Body { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
