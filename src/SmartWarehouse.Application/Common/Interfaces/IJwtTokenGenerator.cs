namespace SmartWarehouse.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(string userId, string email, IList<string> roles);
    string GenerateRefreshToken();
}
