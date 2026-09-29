using ToolShare.Data;
using ToolShare.Domain;
using ToolShare.Domain.Exceptions;
using ToolShare.Dtos;
using ToolShare.Mapping;

namespace ToolShare.Services;

public class LoanService : ILoanService
{
    private readonly ILoanRepository _loanRepo;
    private readonly IRepository<Tool> _toolRepo;
    private readonly IRepository<Member> _memberRepo;
    private readonly IIdempotencyStore _idempotencyStore;

    public LoanService(ILoanRepository loanRepo, IRepository<Tool> toolRepo, IRepository<Member> memberRepo, IIdempotencyStore idempotencyStore)
    {
        _loanRepo = loanRepo;
        _toolRepo = toolRepo;
        _memberRepo = memberRepo;
        _idempotencyStore = idempotencyStore;
    }

    public async Task<LoanResponse> CheckOutAsync(CreateLoanRequest request, string idempotencyKey)
    {
        var existing = await _idempotencyStore.GetAsync(idempotencyKey);
        if (existing is not null)
        {
            if (existing.Request != request)
                throw new IdempotencyConflictException("This idempotency key was already used with a different request.");
            return existing.Response;
        }

        var tool = await _toolRepo.GetByIdAsync(request.ToolId)
            ?? throw new NotFoundException("Tool not found.");
        var borrower = await _memberRepo.GetByIdAsync(request.BorrowerId)
            ?? throw new NotFoundException("Member not found.");

        var activeLoan = await _loanRepo.GetActiveLoanForToolAsync(tool.Id);
        if (activeLoan is not null)
            throw new ConflictException($"'{tool.Name}' is already checked out.");

        var loan = new Loan(tool.Id, borrower.Id, request.DueDate);
        await _loanRepo.AddAsync(loan);

        var response = loan.ToResponse();
        await _idempotencyStore.SaveAsync(idempotencyKey, new IdempotencyRecord(request, response));

        return response;
    }

    public async Task ReturnAsync(Guid loanId)
    {
        var loan = await _loanRepo.GetByIdAsync(loanId)
            ?? throw new NotFoundException("Loan not found.");

        loan.Return();
    }
}