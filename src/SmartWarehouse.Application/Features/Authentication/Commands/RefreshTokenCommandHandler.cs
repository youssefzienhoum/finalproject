using MediatR;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Exceptions;

namespace SmartWarehouse.Application.Features.Authentication.Commands;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IIdentityService _identityService;

    public RefreshTokenCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.RefreshTokenAsync(request.Token, request.RefreshToken);

        if (!result.Success)
        {
            throw new UnauthorizedAccessException(string.Join(", ", result.Errors));
        }

        return new AuthResponse(result.Token, result.RefreshToken, string.Empty);
    }
}
