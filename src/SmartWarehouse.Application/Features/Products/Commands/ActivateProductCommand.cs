using MediatR;

namespace SmartWarehouse.Application.Features.Products.Commands;

public record ActivateProductCommand(int Id) : IRequest;
