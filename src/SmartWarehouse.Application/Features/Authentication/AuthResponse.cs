namespace SmartWarehouse.Application.Features.Authentication;

public record AuthResponse(string Token, string RefreshToken, string UserId);
