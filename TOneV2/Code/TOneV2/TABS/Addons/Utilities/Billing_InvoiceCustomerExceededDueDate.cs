using System;

namespace TABS.Addons.Utilities
{
    public class Billing_InvoiceCustomerExceededDueDate
    {
        public Billing_InvoiceCustomerExceededDueDate(Billing_Invoice invoice)
        {
            SerialNumber = invoice.SerialNumber;
            Customer = TABS.CarrierAccount.All[invoice.Customer.CarrierAccountID];
            DueDate = invoice.DueDate;
        }
        public string SerialNumber { get; set; }
        public TABS.CarrierAccount Customer { get; set; }
        public DateTime DueDate { get; set; }
    }
}
