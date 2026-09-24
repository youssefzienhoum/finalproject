using MediatR;
using SmartWarehouse.Application.Common.Models;

namespace SmartWarehouse.Application.Features.Notifications.Queries;

public record GetNotificationsQuery(string UserId, int PageNumber = 1, int PageSize = 10, bool UnreadOnly = false) : IRequest<PaginatedList<NotificationDto>>;
