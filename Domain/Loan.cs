using ToolShare.Domain.Exceptions;

namespace ToolShare.Domain;

public class Loan
{
    public Guid Id { get; }
    public Guid ToolId { get; }
    public Guid BorrowerId { get; }
    public DateTime CheckedOutDate { get; }
    public DateTime? ReturnDate { get; private set; }
    public LoanStatus Status { get; private set; }

    public Loan(Guid toolId, Guid borrowerId)
    {
        Id = Guid.NewGuid();
        ToolId = toolId;
        BorrowerId = borrowerId;
        LoanDate = DateTime.UtcNow;
        Status = LoanStatus.CheckedOut;
    }

    public void Return()
    {
        if (Status == LoanStatus.Returned)
            throw new InvalidOperationException("Loan has already been returned.");

        ReturnDate = DateTime.UtcNow;
        Status = LoanStatus.Returned;
    }
}