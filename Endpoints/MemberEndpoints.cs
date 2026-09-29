using ToolShare.Data;
using ToolShare.Domain.Exceptions;
using ToolShare.Mapping;

namespace ToolShare.Endpoints;

public static class MemberEndpoints
{
    public static void MapMemberEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/members").WithTags("Members");

        group.MapGet("/", async (IRepository<ToolShare.Domain.Member> repo) =>
            Results.Ok((await repo.GetAllAsync()).Select(m => m.ToResponse())));

        group.MapGet("/{id:guid}", async (Guid id, IRepository<ToolShare.Domain.Member> repo) =>
        {
            var member = await repo.GetByIdAsync(id) ?? throw new NotFoundException("Member not found.");
            return Results.Ok(member.ToResponse());
        });
    }
}