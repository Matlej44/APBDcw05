using System;

namespace LegacyRenewalApp;

public class FeeService
{
    public decimal SupportFee { get; private set; }
    public decimal Fee { get; private set; }
    public void CalculateSuportFee(bool includePremiumSupport, string normalizedPlanCode, ref string notes)
    {
        SupportFee = 0;
        if (!includePremiumSupport) return;
        var enumPlan = Enum.Parse(typeof(NormalizedPlanCodeEnum), normalizedPlanCode);
        SupportFee = ((int)enumPlan) * 1m;
        notes += "premium support included; ";
    }

    public void CalculateFee(string normalizedPaymentMethod, ref string notes)
    {
        Fee = 0;
        switch (normalizedPaymentMethod)
        {
            case "CARD":
                Fee = 0.02m;
                notes += "card payment fee; ";
                break;
            case "BANK_TRANSFER":
                Fee = 0.01m;
                notes += "bank transfer fee; ";
                break;
            case "PAYPAL":
                Fee = 0.035m;
                notes += "paypal fee; ";
                break;
            case "INVOICE":
                notes += "invoice payment; ";
                break;
            default:
                throw new ArgumentException("Unsupported payment method");
        }
    }
}