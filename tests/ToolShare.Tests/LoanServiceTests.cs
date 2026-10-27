using ToolShare.Data;
using ToolShare.Domain;
using ToolShare.Domain.Exceptions;
using ToolShare.Dtos;
using ToolShare.Services;

namespace ToolShare.Tests;

public class LoanServiceTests
{
    private readonly LoanService _service;
    private readonly Tool _tool;
    private readonly Member _borrower;

    public LoanServiceTests()
    {
        var members = new InMemoryMemberRepository();
        var tools = new InMemoryToolRepository();
        var loans = new InMemoryLoanRepository();

        var owner = new Member("Owner One", "owner@example.com");
        _borrower = new Member("Borrower Two", "borrower@example.com");
        _tool = new Tool("Drill", "Power Tools", owner.Id);

        members.AddAsync(owner).GetAwaiter().GetResult();
        members.AddAsync(_borrower).GetAwaiter().GetResult();
        tools.AddAsync(_tool).GetAwaiter().GetResult();

        _service = new LoanService(loans, tools, members, new InMemoryIdempotencyStore());
    }

    private CreateLoanRequest NewRequest()
        => new(_tool.Id, _borrower.Id, DateTime.UtcNow.AddDays(7));

    private static string NewKey() => Guid.NewGuid().ToString();

    [Fact]
    public async Task CheckOut_ValidRequest_ReturnsCheckedOutLoan()
    {
        var response = await _service.CheckOutAsync(NewRequest(), NewKey());

        Assert.Equal(_tool.Id, response.ToolId);
        Assert.Equal(_borrower.Id, response.BorrowerId);
        Assert.Equal(LoanStatus.CheckedOut, response.Status);
    }

    [Fact]
    public async Task CheckOut_ToolAlreadyOut_ThrowsConflict()
    {
        await _service.CheckOutAsync(NewRequest(), NewKey());

        await Assert.ThrowsAsync<ConflictException>(
            () => _service.CheckOutAsync(NewRequest(), NewKey()));
    }

    [Fact]
    public async Task CheckOut_UnknownTool_ThrowsNotFound()
    {
        var request = new CreateLoanRequest(Guid.NewGuid(), _borrower.Id, DateTime.UtcNow.AddDays(7));

        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.CheckOutAsync(request, NewKey()));
    }

    [Fact]
    public async Task CheckOut_ReplaySameKeyAndBody_ReturnsSameLoan()
    {
        var request = NewRequest();
        var key = NewKey();

        var first = await _service.CheckOutAsync(request, key);
        var replay = await _service.CheckOutAsync(request, key);

        Assert.Equal(first.Id, replay.Id);
    }

    [Fact]
    public async Task CheckOut_SameKeyDifferentBody_ThrowsIdempotencyConflict()
    {
        var request = NewRequest();
        var key = NewKey();
        await _service.CheckOutAsync(request, key);

        var different = request with { DueDate = request.DueDate.AddDays(1) };

        await Assert.ThrowsAsync<IdempotencyConflictException>(
            () => _service.CheckOutAsync(different, key));
    }

    [Fact]
    public async Task Return_Twice_ThrowsConflict()
    {
        var loan = await _service.CheckOutAsync(NewRequest(), NewKey());
        await _service.ReturnAsync(loan.Id);

        await Assert.ThrowsAsync<ConflictException>(() => _service.ReturnAsync(loan.Id));
    }

    [Fact]
    public async Task Delete_ActiveLoan_ThrowsConflict()
    {
        var loan = await _service.CheckOutAsync(NewRequest(), NewKey());

        await Assert.ThrowsAsync<ConflictException>(() => _service.DeleteAsync(loan.Id));
    }

    [Fact]
    public async Task CheckOut_ManyParallelRequests_ExactlyOneSucceeds()
    {
        var request = NewRequest();

        var attempts = Enumerable.Range(0, 20).Select(_ => Task.Run(async () =>
        {
            try
            {
                await _service.CheckOutAsync(request, NewKey());
                return true;
            }
            catch (ConflictException)
            {
                return false;
            }
        }));

        var results = await Task.WhenAll(attempts);

        Assert.Equal(1, results.Count(succeeded => succeeded));
    }
}