namespace LegacyRenewalApp;

public interface INotificationService
{
    public void SendNotification(string to, string body, string subject);
}