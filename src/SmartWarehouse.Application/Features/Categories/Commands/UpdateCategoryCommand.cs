using MediatR;
using SmartWarehouse.Application.Features.Categories;

namespace SmartWarehouse.Application.Features.Categories.Commands;

public record UpdateCategoryCommand(int Id, string Name, string? Description, bool IsActive) : IRequest<CategoryDto>;
