namespace LegacyRenewalApp;

public class EmailService : INotificationService
{
    
    public void SendNotification(string to, string body, string subject = "Subscription renewal invoice")
    {
        LegacyBillingGateway.SendEmail(to, subject, body);
    }
}