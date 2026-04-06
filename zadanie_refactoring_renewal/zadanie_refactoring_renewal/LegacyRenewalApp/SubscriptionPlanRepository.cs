using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;

namespace LegacyRenewalApp
{
    public class SubscriptionPlanRepository
    {
        public static readonly Dictionary<string, SubscriptionPlan> Database = new Dictionary<string, SubscriptionPlan>
        {
            { "START", new SubscriptionPlan { Code = "START", Name = "Start", MonthlyPricePerSeat = 49m, SetupFee = 120m, IsEducationEligible = false } },
            { "PRO", new SubscriptionPlan { Code = "PRO", Name = "Professional", MonthlyPricePerSeat = 89m, SetupFee = 180m, IsEducationEligible = true } },
            { "ENTERPRISE", new SubscriptionPlan { Code = "ENTERPRISE", Name = "Enterprise", MonthlyPricePerSeat = 149m, SetupFee = 300m, IsEducationEligible = false } }
        };
        
    }
}
