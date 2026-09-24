using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWarehouse.Application.Features.InventoryMovements.Queries;

namespace SmartWarehouse.API.Controllers;

[ApiController]
[Route("api/inventory/movements")]
[Authorize]
public class InventoryMovementsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryMovementsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetInventoryMovementsQuery query, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetInventoryMovementByIdQuery(id), cancellationToken);
        return Ok(result);
    }
}
