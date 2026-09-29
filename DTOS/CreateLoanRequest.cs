namespace ToolShare.DTOS;

public record CreateLoanRequest(Guid ToolId, Guid BorrowerId, DateTime DueDate);