using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWarehouse.Application.Features.Inventory.Queries;

namespace SmartWarehouse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetInventoryQuery query, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{productId}/{warehouseId}")]
    public async Task<IActionResult> GetByProductAndWarehouse(int productId, int warehouseId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetInventoryQuery(productId, warehouseId), cancellationToken);
        
        var inventory = result.Items.FirstOrDefault();
        if (inventory == null)
        {
            return NotFound();
        }

        return Ok(inventory);
    }

    [HttpGet("/api/warehouses/{warehouseId}/inventory")]
    public async Task<IActionResult> GetWarehouseInventory(int warehouseId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new GetInventoryQuery(WarehouseId: warehouseId, PageNumber: pageNumber, PageSize: pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
