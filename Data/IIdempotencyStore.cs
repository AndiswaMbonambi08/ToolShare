using ToolShare.Dtos;

namespace ToolShare.Data;

public record IdempotencyRecord(CreateLoanRequest Request, LoanResponse Response);

public interface IIdempotencyStore
{
    Task<IdempotencyRecord?> GetAsync(string key);
    Task SaveAsync(string key, IdempotencyRecord record);
}