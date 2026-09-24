using MediatR;

namespace SmartWarehouse.Application.Features.Notifications.Commands;

public record MarkNotificationAsReadCommand(long Id, string UserId) : IRequest;
public record MarkAllNotificationsAsReadCommand(string UserId) : IRequest;
