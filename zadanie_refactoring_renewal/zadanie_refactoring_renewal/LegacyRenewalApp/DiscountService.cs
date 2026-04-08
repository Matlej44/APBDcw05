namespace LegacyRenewalApp;

public class DiscountService
{
    public  decimal DiscountAmount{get; private set;}
    public string Notes{get; private set;}
    private decimal _baseAmount;
    private  int _seatCount = 0;
    private Customer _customer;
    public decimal SubtotalAfterDiscount{ get; private set;}
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
        var (discount, tempNotes) = GetDiscountBySegment(plan);
        Notes += tempNotes;
        DiscountAmount += _baseAmount*discount;

        (discount, tempNotes) = GetDiscountByYears();
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

    private (decimal, string) GetDiscountBySegment(SubscriptionPlan plan)
    {
        decimal discountAmount;
        var notes = string.Empty;
            
        switch (_customer.Segment)
        {
            case "Silver":
                discountAmount = 0.05m;
                notes += "silver discount; ";
                break;
            case "Gold":
                discountAmount =  0.10m;
                notes += "gold discount; ";
                break;
            case "Platinum":
                discountAmount = 0.15m;
                notes += "platinum discount; ";
                break;
            case "Education" when plan.IsEducationEligible:
                discountAmount = 0.20m;
                notes += "education discount; ";
                break;
            default:
                discountAmount = 0;
                break;
        }
        return (discountAmount, notes);
    }

    private (decimal, string) GetDiscountByYears()
    {
        decimal discountAmount = 0;
        var notes = string.Empty;
        switch (_customer.YearsWithCompany)
        {
            case >= 5:
                discountAmount = 0.07m;
                notes += "long-term loyalty discount; ";
                break;
            case >= 2:
                discountAmount = 0.03m;
                notes += "basic loyalty discount; ";
                break;
        }
        return (discountAmount, notes);
    }

    public void CalculateSubtotalAfterDiscount()
    {
        SubtotalAfterDiscount = _baseAmount - DiscountAmount;
        if (SubtotalAfterDiscount >= 300m) return;
        SubtotalAfterDiscount = 300m;
        Notes += "minimum discounted subtotal applied; ";
    }
    
}