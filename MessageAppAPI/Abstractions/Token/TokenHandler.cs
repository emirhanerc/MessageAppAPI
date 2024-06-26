using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MessageAppAPI.Entities;
using Microsoft.IdentityModel.Tokens;

namespace MessageAppAPI.Abstractions.Token;

public class TokenHandler : ITokenHandler
{
    private readonly IConfiguration _configuration;

    public TokenHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Dtos.TokenDto.Token CreateAccessToken(int second, AppUser user)
    {
        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_configuration["Token:SecurityKey"]));

        SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Role, user.Role)
            
        };

        var token = new JwtSecurityToken(
            audience: _configuration["Token:Audience"],
            issuer: _configuration["Token:Issuer"],
            expires: DateTime.UtcNow.AddSeconds(second),
            notBefore: DateTime.UtcNow,
            claims: claims,
            signingCredentials: signingCredentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        string accessToken = tokenHandler.WriteToken(token);

        var tokenDto = new Dtos.TokenDto.Token()
        {
            AccessToken = accessToken
        };

        return tokenDto;
    }
}