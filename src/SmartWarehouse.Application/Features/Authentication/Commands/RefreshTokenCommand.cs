using MediatR;

namespace SmartWarehouse.Application.Features.Authentication.Commands;

public record RefreshTokenCommand(string Token, string RefreshToken) : IRequest<AuthResponse>;
