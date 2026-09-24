using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWarehouse.Application.Features.Warehouses.Commands;
using SmartWarehouse.Application.Features.Warehouses.Queries;

namespace SmartWarehouse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WarehousesController : ControllerBase
{
    private readonly IMediator _mediator;

    public WarehousesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateWarehouseCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, UpdateWarehouseCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("Id in route does not match Id in command.");
        }

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetWarehousesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetWarehouseByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost("{warehouseId}/employees/{employeeId}")]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<IActionResult> AssignEmployee(int warehouseId, string employeeId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new AssignEmployeeToWarehouseCommand(warehouseId, employeeId), cancellationToken);
        return Ok();
    }

    [HttpDelete("{warehouseId}/employees/{employeeId}")]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<IActionResult> RemoveEmployee(int warehouseId, string employeeId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new RemoveEmployeeFromWarehouseCommand(warehouseId, employeeId), cancellationToken);
        return NoContent();
    }

    [HttpGet("{warehouseId}/employees")]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<IActionResult> GetEmployees(int warehouseId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetWarehouseEmployeesQuery(warehouseId), cancellationToken);
        return Ok(result);
    }
}
