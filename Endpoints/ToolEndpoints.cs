using ToolShare.Data;
using ToolShare.Domain.Exceptions;
using ToolShare.Mapping;

namespace ToolShare.Endpoints;

public static class ToolEndpoints
{
    public static void MapToolEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/tools").WithTags("Tools");

        group.MapGet("/", async (IRepository<ToolShare.Domain.Tool> repo) =>
            Results.Ok((await repo.GetAllAsync()).Select(t => t.ToResponse())));

        group.MapGet("/{id:guid}", async (Guid id, IRepository<ToolShare.Domain.Tool> repo) =>
        {
            var tool = await repo.GetByIdAsync(id) ?? throw new NotFoundException("Tool not found.");
            return Results.Ok(tool.ToResponse());
        });
    }
}