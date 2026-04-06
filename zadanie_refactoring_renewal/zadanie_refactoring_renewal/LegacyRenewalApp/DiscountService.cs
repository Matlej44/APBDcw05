namespace LegacyRenewalApp;

public class DiscountService
{
    public  decimal DiscountAmount{get; private set;}
    public string Notes{get; private set;}
    private decimal _baseAmount;
    private  int _seatCount = 0;
    private Customer _customer;
    public void GetDiscount(
        Customer customer,
        SubscriptionPlan plan,
        int seatCount,
        decimal baseAmount)
    {
        _customer = customer;
        Notes = string.Empty;
        _baseAmount = baseAmount;
        _seatCount = seatCount;
        var (discount, tempNotes) = customer.GetDiscountBySegment(plan);
        Notes += tempNotes;
        DiscountAmount += _baseAmount*discount;

        (discount, tempNotes) = customer.GetDiscountByYears();
        Notes += tempNotes;
        DiscountAmount += _baseAmount*discount;
        
        GetDiscountBySeat();
    }

    private void GetDiscountBySeat()
    {
        switch (_seatCount)
        {
            case >= 50:
                DiscountAmount += _baseAmount * 0.12m;
                Notes += "large team discount; ";
                break;
            case >= 20:
                DiscountAmount += _baseAmount * 0.08m;
                Notes += "medium team discount; ";
                break;
            case >= 10:
                DiscountAmount += _baseAmount * 0.04m;
                Notes += "small team discount; ";
                break;
        }
    }

    public void ApplyLoyaltyPoints()
    {
        var pointsToUse = _customer.LoyaltyPoints > 200 ? 200 : _customer.LoyaltyPoints;
        DiscountAmount += pointsToUse;
        Notes += $"loyalty points used: {pointsToUse}; ";
    }
    
}