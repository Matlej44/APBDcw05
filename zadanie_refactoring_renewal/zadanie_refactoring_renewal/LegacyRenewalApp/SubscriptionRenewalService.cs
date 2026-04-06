using System;
using System.Linq;

namespace LegacyRenewalApp
{
    public class SubscriptionRenewalService()
    {
        public RenewalInvoice CreateRenewalInvoice(
            int customerId,
            string planCode,
            int seatCount,
            string paymentMethod,
            bool includePremiumSupport,
            bool useLoyaltyPoints)
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


            var normalizedPlanCode = Normalize(planCode);
            var normalizedPaymentMethod = Normalize(paymentMethod);

            var customer = CustomerRepository.Database.Where(k => k.Key == customerId).Select(x => x.Value)
                .FirstOrDefault();
            if (customer == null) throw new ArgumentException($"Customer with id {customerId} does not exist");
            if (!customer.IsActive)
                throw new InvalidOperationException("Inactive customers cannot renew subscriptions");

            var plan = SubscriptionPlanRepository.Database.Where(k => k.Key.Contains(normalizedPlanCode))
                .Select(x => x.Value).FirstOrDefault();
            if (plan == null) throw new ArgumentException($"Plan with code {normalizedPlanCode} does not exist");

            var baseAmount = plan.GetBaseAmount(seatCount);
            var discountService = new DiscountService();
            discountService.GetDiscount(customer, plan, seatCount, baseAmount);
            if (useLoyaltyPoints && customer.LoyaltyPoints > 0) discountService.ApplyLoyaltyPoints();

            var discountAmount = discountService.DiscountAmount;
            var notes = discountService.Notes;


            var subtotalAfterDiscount = baseAmount - discountAmount;
            if (subtotalAfterDiscount < 300m)
            {
                subtotalAfterDiscount = 300m;
                notes += "minimum discounted subtotal applied; ";
            }

            var supportFee = 0m;
            if (includePremiumSupport)
            {
                var enumPlan = Enum.Parse(typeof(NormalizedPlanCodeEnum), normalizedPlanCode);
                supportFee = ((int)enumPlan) * 1m;
                notes += "premium support included; ";
            }
            var fee = 0m;
            switch (normalizedPaymentMethod)
            {
                case "CARD":
                    fee =  0.02m;
                    notes += "card payment fee; ";
                    break;
                case "BANK_TRANSFER":
                    fee =  0.01m;
                    notes += "bank transfer fee; ";
                    break;
                case "PAYPAL":
                    fee = 0.035m;
                    notes += "paypal fee; ";
                    break;
                case "INVOICE":
                    notes += "invoice payment; ";
                    break;
                default:
                    throw new ArgumentException("Unsupported payment method");
            }
            var paymentFee = (subtotalAfterDiscount + supportFee) * fee;
            var taxRate = customer.GetTaxRate();

            var taxBase = subtotalAfterDiscount + supportFee + paymentFee;
            var taxAmount = taxBase * taxRate;
            var finalAmount = taxBase + taxAmount;

            if (finalAmount < 500m)
            {
                finalAmount = 500m;
                notes += "minimum invoice amount applied; ";
            }

            var invoice = new RenewalInvoice
            {
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{customerId}-{normalizedPlanCode}",
                Customer = customer,
                PlanCode = normalizedPlanCode,
                PaymentMethod = normalizedPaymentMethod,
                SeatCount = seatCount,
                BaseAmount = Math.Round(baseAmount, 2, MidpointRounding.AwayFromZero),
                DiscountAmount = Math.Round(discountAmount, 2, MidpointRounding.AwayFromZero),
                SupportFee = Math.Round(supportFee, 2, MidpointRounding.AwayFromZero),
                PaymentFee = Math.Round(paymentFee, 2, MidpointRounding.AwayFromZero),
                TaxAmount = Math.Round(taxAmount, 2, MidpointRounding.AwayFromZero),
                FinalAmount = Math.Round(finalAmount, 2, MidpointRounding.AwayFromZero),
                Notes = notes.Trim(),
                GeneratedAt = DateTime.UtcNow
            };

            LegacyBillingGateway.SaveInvoice(invoice);

            if (string.IsNullOrWhiteSpace(customer.Email)) return invoice;
            
            const string subject = "Subscription renewal invoice";
            var body =
                $"Hello {customer.FullName}, your renewal for plan {normalizedPlanCode} " +
                $"has been prepared. Final amount: {invoice.FinalAmount:F2}.";

            LegacyBillingGateway.SendEmail(customer.Email, subject, body);

            return invoice;
        }

        private static string Normalize(string value) => value.Trim().ToUpperInvariant();
    }
}