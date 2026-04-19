namespace JotIt.Models;

public abstract class Item
{
    public long Id { get; set; }
    public string Body { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"[{Id}] {(string.IsNullOrEmpty(Category) ? "No Category" : Category)}]  {Body} ";
    }
}
