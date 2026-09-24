using MediatR;
using SmartWarehouse.Application.Features.Categories;

namespace SmartWarehouse.Application.Features.Categories.Commands;

public record CreateCategoryCommand(string Name, string? Description) : IRequest<CategoryDto>;
