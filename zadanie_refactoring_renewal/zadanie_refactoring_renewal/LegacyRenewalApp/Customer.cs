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
    }
}
