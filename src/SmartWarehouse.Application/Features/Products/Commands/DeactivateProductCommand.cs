using MediatR;

namespace SmartWarehouse.Application.Features.Products.Commands;

public record DeactivateProductCommand(int Id) : IRequest;
