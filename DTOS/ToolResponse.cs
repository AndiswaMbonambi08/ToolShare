namespace ToolShare.DTOS;

public record ToolResponse(Guid Id, string FullName, string Category, Guid OwnerId);