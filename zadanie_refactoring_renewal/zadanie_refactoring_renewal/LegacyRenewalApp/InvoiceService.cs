namespace LegacyRenewalApp;

public class InvoiceService : IInvoiceService
{
    public void SaveInvoice(RenewalInvoice invoice)
    {
        LegacyBillingGateway.SaveInvoice(invoice);
    }
}