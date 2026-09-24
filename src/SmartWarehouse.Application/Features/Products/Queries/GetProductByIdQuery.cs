using MediatR;
using SmartWarehouse.Application.Features.Products;

namespace SmartWarehouse.Application.Features.Products.Queries;

public record GetProductByIdQuery(int Id) : IRequest<ProductDto>;
