using ToolShare.Domain;
using ToolShare.Dtos;

namespace ToolShare.Mapping;

public static class LoanMapping
{
    public static LoanResponse ToResponse(this Loan loan)
        => new(loan.Id, loan.ToolId, loan.BorrowerId, loan.CheckedOutDate, loan.DueDate, loan.Status);
}