using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWarehouse.Application.Features.PurchaseOrders.Commands;
using SmartWarehouse.Application.Features.PurchaseOrders.Queries;
using System.Security.Claims;

namespace SmartWarehouse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PurchaseOrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public PurchaseOrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Purchasing")]
    public async Task<IActionResult> Create(CreatePurchaseOrderCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetPurchaseOrdersQuery query, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPurchaseOrderByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id}/submit")]
    [Authorize(Roles = "Admin,Purchasing")]
    public async Task<IActionResult> Submit(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new SubmitPurchaseOrderCommand(id), cancellationToken);
        return Ok();
    }

    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Approve(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new ApprovePurchaseOrderCommand(id), cancellationToken);
        return Ok();
    }

    [HttpPost("{id}/receive")]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<IActionResult> Receive(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _mediator.Send(new ReceivePurchaseOrderCommand(id, userId), cancellationToken);
        return Ok();
    }

    [HttpPost("{id}/cancel")]
    [Authorize(Roles = "Admin,Purchasing")]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new CancelPurchaseOrderCommand(id), cancellationToken);
        return Ok();
    }
}
