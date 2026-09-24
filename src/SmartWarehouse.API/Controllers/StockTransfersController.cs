using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWarehouse.Application.Features.StockTransfers.Commands;
using SmartWarehouse.Application.Features.StockTransfers.Queries;
using System.Security.Claims;

namespace SmartWarehouse.API.Controllers;

[ApiController]
[Route("api/stock-transfers")]
[Authorize]
public class StockTransfersController : ControllerBase
{
    private readonly IMediator _mediator;

    public StockTransfersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<IActionResult> Create(CreateStockTransferCommand command, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";
        var commandWithUser = command with { UserId = userId };
        var result = await _mediator.Send(commandWithUser, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetStockTransfersQuery query, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetStockTransferByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id}/complete")]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<IActionResult> Complete(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _mediator.Send(new CompleteStockTransferCommand(id, userId), cancellationToken);
        return Ok();
    }

    [HttpPost("{id}/cancel")]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _mediator.Send(new CancelStockTransferCommand(id, userId), cancellationToken);
        return Ok();
    }
}
