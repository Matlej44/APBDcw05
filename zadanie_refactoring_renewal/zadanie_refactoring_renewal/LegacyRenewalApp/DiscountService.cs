namespace LegacyRenewalApp;

public static class DiscountService
{
    private static decimal _discountAmount;
    private static string _notes = string.Empty;
    private static decimal _baseAmount;
    private static int _seatCount = 0;
    public static (decimal, string) GetDiscount(
        Customer customer,
        SubscriptionPlan plan,
        int seatCount,
        decimal baseAmount)
    {
        _baseAmount = baseAmount;
        _seatCount = seatCount;
        var (discount, tempNotes) = customer.GetDiscountBySegment(plan);
        _notes += tempNotes;
        _discountAmount += _baseAmount*discount;

        (discount, tempNotes) = customer.GetDiscountByYears();
        _notes += tempNotes;
        _discountAmount += _baseAmount*discount;
        
        GetDiscountBySeat();
        
        return (_discountAmount, _notes);
    }

    private static void GetDiscountBySeat()
    {
        switch (_seatCount)
        {
            case >= 50:
                _discountAmount += _baseAmount * 0.12m;
                _notes += "large team discount; ";
                break;
            case >= 20:
                _discountAmount += _baseAmount * 0.08m;
                _notes += "medium team discount; ";
                break;
            case >= 10:
                _discountAmount += _baseAmount * 0.04m;
                _notes += "small team discount; ";
                break;
        }
    }
}