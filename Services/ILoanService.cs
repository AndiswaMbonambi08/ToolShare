using ToolShare.Dtos;

namespace ToolShare.Services;

public interface ILoanService
{
    Task<LoanResponse> CheckOutAsync(CreateLoanRequest request, string idempotencyKey);
    Task ReturnAsync(Guid loanId);
}