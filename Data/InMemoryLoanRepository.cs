using System.Collections.Concurrent;
using ToolShare.Domain;

namespace ToolShare.Data;

public class InMemoryLoanRepository : ILoanRepository
{
    private readonly ConcurrentDictionary<Guid, Loan> _loans = new();

    public Task<IReadOnlyList<Loan>> GetAllAsync()
        => Task.FromResult((IReadOnlyList<Loan>)_loans.Values.ToList());

    public Task<Loan?> GetByIdAsync(Guid id)
        => Task.FromResult(_loans.GetValueOrDefault(id));

    public Task AddAsync(Loan loan)
    {
        _loans[loan.Id] = loan;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        _loans.TryRemove(id, out _);
        return Task.CompletedTask;
    }

    public Task<Loan?> GetActiveLoanForToolAsync(Guid toolId)
        => Task.FromResult(_loans.Values.FirstOrDefault(l => l.ToolId == toolId && l.Status == LoanStatus.CheckedOut));
}