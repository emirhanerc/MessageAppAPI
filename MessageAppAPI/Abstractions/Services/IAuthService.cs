namespace MessageAppAPI.Abstractions.Services;

public interface IAuthService
{
    Task<Dtos.TokenDto.Token> LoginAsync(string usernameOrEmail, string password, int accessTokenLifeTime);
}