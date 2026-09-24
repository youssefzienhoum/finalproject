using MediatR;
using SmartWarehouse.Application.Features.Categories;

namespace SmartWarehouse.Application.Features.Categories.Queries;

public record GetCategoriesQuery() : IRequest<List<CategoryDto>>;
