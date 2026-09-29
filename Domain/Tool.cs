namespace ToolShare.Domain;

public class Tool
{
    public Guid Id { get; }
    public string FullName { get; private set; }
    public string Category { get; private set; }
    public Guid OwnerId { get; }

    public Tool(string fullName, string category, Guid ownerId)
    {
        Id = Guid.NewGuid();
        OwnerId = ownerId;
        SetFullName(fullName);
        SetCategory(category);
    }

    private void SetFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Tool name is required.", nameof(fullName));
        FullName = fullName.Trim();
    }

    private void SetCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category is required.", nameof(category));
        Category = category.Trim();
    }
}