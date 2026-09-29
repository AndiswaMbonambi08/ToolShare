using Microsoft.AspNetCore.Mvc;
using ToolShare.Data;
using ToolShare.Domain.Exceptions;
using ToolShare.Dtos;
using ToolShare.Mapping;
using ToolShare.Services;
using ToolShare.Validation;

namespace ToolShare.Endpoints;

public static class LoanEndpoints
{
    public static void MapLoanEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/loans").WithTags("Loans");

        group.MapGet("/", async (IRepository<ToolShare.Domain.Loan> repo) =>
            Results.Ok((await repo.GetAllAsync()).Select(l => l.ToResponse())));

        group.MapGet("/{id:guid}", async (Guid id, IRepository<ToolShare.Domain.Loan> repo) =>
        {
            var loan = await repo.GetByIdAsync(id) ?? throw new NotFoundException("Loan not found.");
            return Results.Ok(loan.ToResponse());
        });

        group.MapPost("/", async (
            CreateLoanRequest request,
            [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
            ILoanService service) =>
        {
            if (string.IsNullOrWhiteSpace(idempotencyKey))
                throw new RequestValidationException("An Idempotency-Key header is required to check out a tool.");

            var response = await service.CheckOutAsync(request, idempotencyKey);
            return Results.Created($"/api/loans/{response.Id}", response);
        })
        .WithSummary("Check out a tool")
        .WithDescription("""
            Requires an Idempotency-Key header. Fails 404 if the tool or member does not exist,
            409 if the tool is already checked out, 422 if the same key is reused with a different body.
            """)
        .Produces<LoanResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
        .AddEndpointFilter<ValidationFilter<CreateLoanRequest>>();

        group.MapPost("/{id:guid}/return", async (Guid id, ILoanService service) =>
        {
            await service.ReturnAsync(id);
            return Results.NoContent();
        });

        group.MapDelete("/{id:guid}", async (Guid id, IRepository<ToolShare.Domain.Loan> repo) =>
        {
            var loan = await repo.GetByIdAsync(id) ?? throw new NotFoundException("Loan not found.");
            await repo.DeleteAsync(id);
            return Results.NoContent();
        });
    }
}