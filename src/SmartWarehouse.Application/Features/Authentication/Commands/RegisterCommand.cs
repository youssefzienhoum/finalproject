using MediatR;

namespace SmartWarehouse.Application.Features.Authentication.Commands;

public record RegisterCommand(
    string Email, 
    string Password, 
    string FirstName, 
    string LastName, 
    string Role) : IRequest<string>;
