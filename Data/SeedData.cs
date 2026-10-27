using ToolShare.Domain;

namespace ToolShare.Data;

public static class SeedData
{
    public static readonly Guid ThaboId  = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid LeratoId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid DrillId  = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public static readonly Guid LadderId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
}

// Runs during host start-up, before the server accepts requests.
public class DataSeeder : IHostedService
{
    private readonly IRepository<Member> _members;
    private readonly IRepository<Tool> _tools;

    public DataSeeder(IRepository<Member> members, IRepository<Tool> tools)
    {
        _members = members;
        _tools = tools;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _members.AddAsync(new Member("Thabo Nkosi", "thabo@example.com", SeedData.ThaboId));
        await _members.AddAsync(new Member("Lerato Dube", "lerato@example.com", SeedData.LeratoId));

        await _tools.AddAsync(new Tool("Drill", "Power Tools", SeedData.ThaboId, SeedData.DrillId));
        await _tools.AddAsync(new Tool("Ladder", "General", SeedData.LeratoId, SeedData.LadderId));
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}