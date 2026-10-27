using ToolShare.Domain;

namespace ToolShare.Dtos;

public record LoanResponse(Guid Id, Guid ToolId, Guid BorrowerId, DateTime CheckedOutDate, DateTime DueDate, LoanStatus Status);