namespace ToolShare.Domain;

public class Member
{
    public Guid Id { get; }
    public string FullName { get; private set; } = null!;
    public string Email { get; private set; } = null!;

    public Member(string fullName, string email, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        SetFullName(fullName);
        SetEmail(email);
    }

    private void SetFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.", nameof(fullName));
        FullName = fullName.Trim();
    }

    private void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("A valid email is required.", nameof(email));
        Email = email.Trim().ToLowerInvariant();
    }
}