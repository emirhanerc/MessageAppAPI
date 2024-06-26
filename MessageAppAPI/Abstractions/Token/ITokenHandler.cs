using MessageAppAPI.Entities;

namespace MessageAppAPI.Abstractions.Token;

public interface ITokenHandler
{
    Dtos.TokenDto.Token CreateAccessToken(int second, AppUser user);
}