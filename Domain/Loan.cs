using ToolShare.Domain.Exceptions;

namespace ToolShare.Domain;

public class Loan
{
    public Guid Id { get; } = Guid.NewGuid();
    public Guid ToolId { get; }
    public Guid BorrowerId { get; }
    public DateTime CheckedOutDate { get; } = DateTime.UtcNow;
    public DateTime DueDate { get; }
    public DateTime? ReturnDate { get; private set; }
    public LoanStatus Status { get; private set; } = LoanStatus.CheckedOut;

    public Loan(Guid toolId, Guid borrowerId, DateTime dueDate)
    {
        if (dueDate < CheckedOutDate.Date)
            throw new ArgumentException("Due date cannot be before checkout.", nameof(dueDate));

        ToolId = toolId;
        BorrowerId = borrowerId;
        DueDate = dueDate;
    }

    public void Return()
    {
        if (Status == LoanStatus.Returned)
            throw new ConflictException("Loan has already been returned.");

        ReturnDate = DateTime.UtcNow;
        Status = LoanStatus.Returned;
    }
}