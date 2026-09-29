using FluentValidation;
using ToolShare.Dtos;

namespace ToolShare.Validation.Validators;

public class CreateLoanRequestValidator : AbstractValidator<CreateLoanRequest>
{
    public CreateLoanRequestValidator()
    {
        RuleFor(x => x.ToolId).NotEqual(Guid.Empty);
        RuleFor(x => x.BorrowerId).NotEqual(Guid.Empty);
        RuleFor(x => x.DueDate).GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Due date cannot be in the past.");
    }
}