using ToolShare.Domain;
using ToolShare.Dtos;

namespace ToolShare.Mapping;

public static class MemberMapping
{
    public static MemberResponse ToResponse(this Member member)
        => new(member.Id, member.Name, member.Email);
}