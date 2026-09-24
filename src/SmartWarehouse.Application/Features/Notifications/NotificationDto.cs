namespace SmartWarehouse.Application.Features.Notifications;

public record NotificationDto(
    long Id,
    string Title,
    string Message,
    bool IsRead,
    DateTime CreatedAt);
