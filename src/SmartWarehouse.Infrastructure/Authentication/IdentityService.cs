using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Entities;

namespace SmartWarehouse.Infrastructure.Authentication;

public class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IApplicationDbContext _context;

    public IdentityService(
        UserManager<User> userManager,
        IJwtTokenGenerator jwtTokenGenerator,
        IApplicationDbContext context)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _context = context;
    }

    public async Task<(bool Success, string UserId, string[] Errors)> RegisterUserAsync(
        string email, string password, string firstName, string lastName, string role)
    {
        var user = new User
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return (false, string.Empty, result.Errors.Select(e => e.Description).ToArray());
        }

        var roleResult = await _userManager.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
        {
            return (false, string.Empty, roleResult.Errors.Select(e => e.Description).ToArray());
        }

        return (true, user.Id, Array.Empty<string>());
    }

    public async Task<(bool Success, string Token, string RefreshToken, string[] Errors)> LoginAsync(
        string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !user.IsActive)
        {
            return (false, string.Empty, string.Empty, new[] { "Invalid email or password." });
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
        if (!isPasswordValid)
        {
            return (false, string.Empty, string.Empty, new[] { "Invalid email or password." });
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email!, roles);
        var refreshTokenString = _jwtTokenGenerator.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenString,
            ExpiresAt = DateTime.UtcNow.AddDays(7), // Typically from config
            IsRevoked = false
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(CancellationToken.None);

        return (true, token, refreshTokenString, Array.Empty<string>());
    }

    public async Task<(bool Success, string Token, string RefreshToken, string[] Errors)> RefreshTokenAsync(
        string token, string refreshToken)
    {
        // Simple implementation - in real world you validate the expired token's signature first
        var storedToken = await _context.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == refreshToken);

        if (storedToken == null || storedToken.IsRevoked || storedToken.IsExpired || !storedToken.User.IsActive)
        {
            return (false, string.Empty, string.Empty, new[] { "Invalid or expired refresh token." });
        }

        var roles = await _userManager.GetRolesAsync(storedToken.User);
        var newToken = _jwtTokenGenerator.GenerateToken(storedToken.User.Id, storedToken.User.Email!, roles);
        var newRefreshTokenString = _jwtTokenGenerator.GenerateRefreshToken();

        // Revoke the old one
        storedToken.IsRevoked = true;
        storedToken.ReplacedByToken = newRefreshTokenString;
        storedToken.RevokedReason = "Rotated";

        var newRefreshToken = new RefreshToken
        {
            UserId = storedToken.UserId,
            Token = newRefreshTokenString,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync(CancellationToken.None);

        return (true, newToken, newRefreshTokenString, Array.Empty<string>());
    }
}
