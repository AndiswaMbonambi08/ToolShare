using System.Collections.Concurrent;

namespace ToolShare.Data;

public class InMemoryIdempotencyStore : IIdempotencyStore
{
    private readonly ConcurrentDictionary<string, IdempotencyRecord> _records = new();

    public Task<IdempotencyRecord?> GetAsync(string key)
        => Task.FromResult(_records.GetValueOrDefault(key));

    public Task SaveAsync(string key, IdempotencyRecord record)
    {
        _records[key] = record;
        return Task.CompletedTask;
    }
}