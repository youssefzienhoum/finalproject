using MediatR;

namespace SmartWarehouse.Application.Features.Dashboard.Queries;

public record GetDashboardQuery() : IRequest<DashboardDto>;
