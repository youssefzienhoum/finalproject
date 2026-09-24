using MediatR;

namespace SmartWarehouse.Application.Features.Categories.Commands;

public record DeleteCategoryCommand(int Id) : IRequest;
