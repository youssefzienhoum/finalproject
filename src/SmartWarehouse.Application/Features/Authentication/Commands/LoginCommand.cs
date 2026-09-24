using MediatR;

namespace SmartWarehouse.Application.Features.Authentication.Commands;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;
