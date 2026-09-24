using MediatR;
using SmartWarehouse.Application.Common.Models;
using SmartWarehouse.Application.Features.Products;

namespace SmartWarehouse.Application.Features.Products.Queries;

public record GetProductsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Search = null,
    int? CategoryId = null,
    bool? IsActive = null,
    string? SortBy = null,
    string? SortDirection = "asc") : IRequest<PaginatedList<ProductDto>>;
