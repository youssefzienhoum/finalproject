using MediatR;
using SmartWarehouse.Application.Common.Interfaces;
using SmartWarehouse.Domain.Exceptions;

namespace SmartWarehouse.Application.Features.Authentication.Commands;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, string>
{
    private readonly IIdentityService _identityService;

    public RegisterCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<string> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.RegisterUserAsync(
            request.Email, 
            request.Password, 
            request.FirstName, 
            request.LastName, 
            request.Role);

        if (!result.Success)
        {
            throw new BadRequestException(string.Join(", ", result.Errors));
        }

        return result.UserId;
    }
}
