namespace LegacyRenewalApp
{
    public class Customer
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Segment { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public int YearsWithCompany { get; set; }
        public int LoyaltyPoints { get; set; }
        public bool IsActive { get; set; }

        public decimal GetTaxRate()
        {
            var taxRate = Country switch
            {
                "Poland" => 0.23m,
                "Germany" => 0.19m,
                "Czech Republic" => 0.21m,
                "Norway" => 0.25m,
                _ => 0.20m
            };
            return taxRate;
        }
        public (decimal, string) GetDiscountBySegment(SubscriptionPlan plan)
        {
            decimal discountAmount;
            var notes = string.Empty;
            
            switch (Segment)
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

        public (decimal, string) GetDiscountByYears()
        {
            decimal discountAmount = 0;
            var notes = string.Empty;
            switch (YearsWithCompany)
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
    }
}
