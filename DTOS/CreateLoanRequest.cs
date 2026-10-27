namespace ToolShare.Dtos;

public record CreateLoanRequest(Guid ToolId, Guid BorrowerId, DateTime DueDate);