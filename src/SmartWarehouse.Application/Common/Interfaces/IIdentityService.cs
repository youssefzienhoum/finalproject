namespace SmartWarehouse.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<(bool Success, string UserId, string[] Errors)> RegisterUserAsync(string email, string password, string firstName, string lastName, string role);
    Task<(bool Success, string Token, string RefreshToken, string[] Errors)> LoginAsync(string email, string password);
    Task<(bool Success, string Token, string RefreshToken, string[] Errors)> RefreshTokenAsync(string token, string refreshToken);
}
