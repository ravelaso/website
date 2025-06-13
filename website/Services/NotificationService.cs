namespace website.Services;

public class NotificationService
{
    public event Action? OnNotificationUpdated;
    private readonly List<NotificationMessage> _notifications = [];
    public IReadOnlyList<NotificationMessage> Notifications => _notifications;

    public void ShowSuccess(string message)
    {
        AddNotification(message, "success");
    }

    public void ShowError(string message)
    {
        AddNotification(message, "error");
    }

    public void Dismiss(NotificationMessage notification)
    {
        _notifications.Remove(notification);
        OnNotificationUpdated?.Invoke();
    }

    private async void AddNotification(string message, string type)
    {
        var notification = new NotificationMessage
        {
            Message = message,
            Type = type,
        };
        _notifications.Add(notification);
        OnNotificationUpdated?.Invoke();

        await Task.Delay(2000); // Auto-dismiss after 2 seconds
        if (!_notifications.Contains(notification)) return;
        _notifications.Remove(notification);
        OnNotificationUpdated?.Invoke();
    }
}

public class NotificationMessage
{
    public string? Message { get; init; }
    public string? Type { get; init; }
}