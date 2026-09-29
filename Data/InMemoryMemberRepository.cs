using System.Collections.Concurrent;
using ToolShare.Domain;

namespace ToolShare.Data;

public class InMemoryMemberRepository : IRepository<Member>
{
    private readonly ConcurrentDictionary<Guid, Member> _members = new();

    public Task<IReadOnlyList<Member>> GetAllAsync()
        => Task.FromResult((IReadOnlyList<Member>)_members.Values.ToList());

    public Task<Member?> GetByIdAsync(Guid id)
        => Task.FromResult(_members.GetValueOrDefault(id));

    public Task AddAsync(Member member)
    {
        _members[member.Id] = member;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        _members.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}