using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWarehouse.Application.Features.SalesOrders.Commands;
using SmartWarehouse.Application.Features.SalesOrders.Queries;
using System.Security.Claims;

namespace SmartWarehouse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SalesOrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public SalesOrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<IActionResult> Create(CreateSalesOrderCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetSalesOrdersQuery query, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSalesOrderByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id}/confirm")]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<IActionResult> Confirm(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _mediator.Send(new ConfirmSalesOrderCommand(id, userId), cancellationToken);
        return Ok();
    }

    [HttpPost("{id}/ship")]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<IActionResult> Ship(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _mediator.Send(new ShipSalesOrderCommand(id, userId), cancellationToken);
        return Ok();
    }

    [HttpPost("{id}/complete")]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<IActionResult> Complete(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new CompleteSalesOrderCommand(id), cancellationToken);
        return Ok();
    }

    [HttpPost("{id}/cancel")]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _mediator.Send(new CancelSalesOrderCommand(id, userId), cancellationToken);
        return Ok();
    }
}
