using System;

namespace LegacyRenewalApp;

public class SubsciptionValidationService
{
    public static void Validate(int customerId, string planCode, int seatCount, string paymentMethod)
    {
        if (customerId <= 0)
        {
            throw new ArgumentException("Customer id must be positive");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(planCode, "Plan code is required");
        ArgumentException.ThrowIfNullOrWhiteSpace(paymentMethod, "Payment method is required");

        if (seatCount <= 0)
        {
            throw new ArgumentException("Seat count must be positive");
        }
    }
}