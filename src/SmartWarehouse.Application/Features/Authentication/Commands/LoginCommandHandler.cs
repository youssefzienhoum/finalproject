using MediatR;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Exceptions;

namespace SmartWarehouse.Application.Features.Authentication.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.LoginAsync(request.Email, request.Password);

        if (!result.Success)
        {
            throw new UnauthorizedAccessException(string.Join(", ", result.Errors));
        }

        return new AuthResponse(result.Token, result.RefreshToken, string.Empty); // User Id could be returned if needed
    }
}
