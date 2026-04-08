using System;
using System.Collections.Generic;
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
            bool useLoyaltyPoints,
            IInvoiceService invoiceService = null,
            INotificationService notificationService = null)
        {
            if (invoiceService == null) invoiceService = new InvoiceService();
            if (notificationService == null) notificationService = new EmailService();
            
            
            SubsciptionValidationService.Validate(customerId, planCode, seatCount, paymentMethod);

            var normalizedPlanCode = Normalize(planCode);
            var normalizedPaymentMethod = Normalize(paymentMethod);

            var customer = RepositoryService.DatabaseSearchByKey(CustomerRepository.Database, customerId);
            if (!customer.IsActive)
                throw new InvalidOperationException("Inactive customers cannot renew subscriptions");

            var plan = RepositoryService.DatabaseSearchByKey(SubscriptionPlanRepository.Database, normalizedPlanCode);

            var baseAmount = plan.GetBaseAmount(seatCount);
            var discountService = new DiscountService();
            discountService.GetDiscount(customer, plan, seatCount, baseAmount);
            if (useLoyaltyPoints && customer.LoyaltyPoints > 0) discountService.ApplyLoyaltyPoints();

            discountService.CalculateSubtotalAfterDiscount();
            
            var notes = discountService.Notes;
            

            var feeService = new FeeService();
            feeService.CalculateFee(normalizedPaymentMethod, ref notes);
            feeService.CalculateSuportFee(includePremiumSupport, normalizedPlanCode, ref notes);

            var paymentFee = (discountService.SubtotalAfterDiscount + feeService.SupportFee) * feeService.Fee;
            var taxRate = customer.GetTaxRate();

            var taxBase = discountService.SubtotalAfterDiscount + feeService.SupportFee + paymentFee;
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
                DiscountAmount = Math.Round(discountService.DiscountAmount, 2, MidpointRounding.AwayFromZero),
                SupportFee = Math.Round(feeService.SupportFee, 2, MidpointRounding.AwayFromZero),
                PaymentFee = Math.Round(paymentFee, 2, MidpointRounding.AwayFromZero),
                TaxAmount = Math.Round(taxAmount, 2, MidpointRounding.AwayFromZero),
                FinalAmount = Math.Round(finalAmount, 2, MidpointRounding.AwayFromZero),
                Notes = notes.Trim(),
                GeneratedAt = DateTime.UtcNow
            };

            invoiceService.SaveInvoice(invoice);

            if (string.IsNullOrWhiteSpace(customer.Email)) return invoice;

            const string subject = "Subscription renewal invoice";
            var body =
                $"Hello {customer.FullName}, your renewal for plan {normalizedPlanCode} " +
                $"has been prepared. Final amount: {invoice.FinalAmount:F2}.";

            notificationService.SendNotification(customer.Email, body, subject);

            return invoice;
        }

        private static string Normalize(string value) => value.Trim().ToUpperInvariant();

        
    }
}