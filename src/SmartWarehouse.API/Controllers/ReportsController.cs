using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWarehouse.Application.Features.Reports.Queries;
using SmartWarehouse.Domain.Enums;

namespace SmartWarehouse.API.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get paginated inventory report with optional filters.
    /// </summary>
    [HttpGet("inventory")]
    public async Task<IActionResult> GetInventoryReport(
        [FromQuery] int? warehouseId,
        [FromQuery] int? productId,
        [FromQuery] int? categoryId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetInventoryReportQuery(warehouseId, productId, categoryId, pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get paginated sales report with optional filters.
    /// </summary>
    [HttpGet("sales")]
    public async Task<IActionResult> GetSalesReport(
        [FromQuery] SalesOrderStatus? status,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSalesReportQuery(status, fromDate, toDate, pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get paginated purchase report with optional filters.
    /// </summary>
    [HttpGet("purchases")]
    public async Task<IActionResult> GetPurchaseReport(
        [FromQuery] PurchaseOrderStatus? status,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPurchaseReportQuery(status, fromDate, toDate, pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get paginated stock movement report with optional filters.
    /// </summary>
    [HttpGet("stock-movements")]
    public async Task<IActionResult> GetStockMovementReport(
        [FromQuery] int? warehouseId,
        [FromQuery] int? productId,
        [FromQuery] MovementType? movementType,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetStockMovementReportQuery(warehouseId, productId, movementType, fromDate, toDate, pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get paginated low stock report with optional category filter.
    /// </summary>
    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStockReport(
        [FromQuery] int? categoryId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetLowStockReportQuery(categoryId, pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
