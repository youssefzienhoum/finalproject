using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartWarehouse.Application.Features.Authentication.Commands;

namespace SmartWarehouse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(new { UserId = result });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    // Logout typically just clears tokens on client side, 
    // or if we maintain a revoke list, we could revoke it here via a RevokeTokenCommand.
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new { Message = "Logged out successfully" });
    }
}
