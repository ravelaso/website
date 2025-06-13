namespace website.Services;


public enum NotificationType
{
    Success,
    Error
}

public class NotificationService
{
    public event Action? OnNotificationUpdated;
    private readonly List<NotificationMessage> _notifications = [];
    public IReadOnlyList<NotificationMessage> Notifications => _notifications;

    public async Task ShowSuccess(string message)
    {
        await AddNotification(message, NotificationType.Success);
    }

    public async Task ShowError(string message)
    {
        await AddNotification(message, NotificationType.Error);
    }

    public void Dismiss(NotificationMessage notification)
    {
        if (!_notifications.Contains(notification)) return;

        notification.Visible = false;
        OnNotificationUpdated?.Invoke();

        _ = Task.Run(async () =>
        {
            await Task.Delay(300); // Wait for fade-out
            _notifications.Remove(notification);
            OnNotificationUpdated?.Invoke();
        });
    }


    private async Task AddNotification(string message, NotificationType type)
    {
        var notification = new NotificationMessage
        {
            Message = message,
            Type = type,
            Visible = false
        };

        _notifications.Add(notification);
        OnNotificationUpdated?.Invoke();

        // Allow DOM to render before applying transition classes
        await Task.Delay(50);
        notification.Visible = true;
        OnNotificationUpdated?.Invoke();

        // Auto-dismiss after 2 seconds
        await Task.Delay(2000);
        if (!_notifications.Contains(notification)) return;

        notification.Visible = false;
        OnNotificationUpdated?.Invoke();

        await Task.Delay(300); // Allow fade-out animation
        _notifications.Remove(notification);
        OnNotificationUpdated?.Invoke();
    }
}

public class NotificationMessage
{
    public string? Message { get; init; }
    public NotificationType Type { get; init; }
    public bool Visible { get; set; }
}