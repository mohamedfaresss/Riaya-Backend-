using Riaya.Domain.Entities;

namespace Riaya.Application.Auth.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
}