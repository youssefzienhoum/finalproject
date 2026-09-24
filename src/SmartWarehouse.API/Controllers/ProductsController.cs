using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWarehouse.Application.Features.Products.Commands;
using SmartWarehouse.Application.Features.Products.Queries;

namespace SmartWarehouse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<IActionResult> Create(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<IActionResult> Update(int id, UpdateProductCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("Id in route does not match Id in command.");
        }

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetProductsQuery query, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{id}/activate")]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<IActionResult> Activate(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new ActivateProductCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id}/deactivate")]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeactivateProductCommand(id), cancellationToken);
        return NoContent();
    }
}
