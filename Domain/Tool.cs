namespace ToolShare.Domain;

public class Tool
{
    public Guid Id { get; }
    public string Name { get; private set; } = null!;
    public string Category { get; private set; } = null!;
    public Guid OwnerId { get; }

    public Tool(string name, string category, Guid ownerId, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        OwnerId = ownerId;
        SetName(name);
        SetCategory(category);
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tool name is required.", nameof(name));
        Name = name.Trim();
    }

    private void SetCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category is required.", nameof(category));
        Category = category.Trim();
    }
}