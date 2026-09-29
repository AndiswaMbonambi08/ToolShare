using ToolShare.Domain;
using ToolShare.Dtos;

namespace ToolShare.Mapping;

public static class ToolMapping
{
    public static ToolResponse ToResponse(this Tool tool)
        => new(tool.Id, tool.Name, tool.Category, tool.OwnerId);
}