using ToolShare.Domain;

namespace ToolShare.Data;

public interface ILoanRepository : IRepository<Loan>
{
    Task<Loan?> GetActiveLoanForToolAsync(Guid toolId);
}