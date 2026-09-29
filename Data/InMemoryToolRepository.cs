using System.Collections.Concurrent;
using ToolShare.Domain;

namespace ToolShare.Data;

public class InMemoryToolRepository : IRepository<Tool>
{
    private readonly ConcurrentDictionary<Guid, Tool> _tools = new();

    public Task<IReadOnlyList<Tool>> GetAllAsync()
        => Task.FromResult((IReadOnlyList<Tool>)_tools.Values.ToList());

    public Task<Tool?> GetByIdAsync(Guid id)
        => Task.FromResult(_tools.GetValueOrDefault(id));

    public Task AddAsync(Tool tool)
    {
        _tools[tool.Id] = tool;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        _tools.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}