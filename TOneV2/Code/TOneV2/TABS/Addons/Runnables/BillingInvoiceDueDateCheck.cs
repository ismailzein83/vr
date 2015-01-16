using System;

namespace TABS.Addons.Runnables
{
    [NamedAddon("T.One - Invoice Due Date Customer Check", "Raise Alerts in case of an Unpaid Invoices.")]
    public class BillingInvoiceDueDateCheck : RunnableBase
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(BillingInvoiceDueDateCheck));

        public override void Run()
        {
            foreach (TABS.Billing_Invoice invoice in TABS.ObjectAssembler.GetBillingInvoices(null, TABS.CarrierAccount.SYSTEM, DateTime.Now.AddDays(-60).Date, DateTime.Now.Date))
            {
                if (!invoice.IsPaid && invoice.DueDate <= DateTime.Now.Date)
                {
                    TABS.Alert alert = new Alert();
                    alert.Created = DateTime.Now;
                    alert.IsVisible = true;
                    alert.Source = "Billing Invoice Due Date Check";
                    alert.Progress = TABS.AlertProgress.Negative;
                    alert.Level = AlertLevel.High;
                    string description = alert.Description = string.Format("Invoice #{0} for: {1} not Paid, Due Date: {2}", invoice.SerialNumber, invoice.Customer, invoice.DueDate.ToString("dd MMM yyyy"));
                    alert.Tag = "Billing Invoices Alerts";
                    Exception ex;
                    TABS.ObjectAssembler.SaveOrUpdate(alert, out ex);

                    try
                    {
                        TABS.Addons.Utilities.Billing_InvoiceCustomerExceededDueDate invoiceNotPaid = new TABS.Addons.Utilities.Billing_InvoiceCustomerExceededDueDate(invoice);
                        System.Net.Mail.MailMessage message = TABS.SpecialSystemParameters.EmailDetailsEvaluator.GetMailMessage(invoiceNotPaid);
                        TABS.Components.EmailSender.Send(message, out ex);
                        log.Info(string.Format("Email was sent (to SMTP) from Billing Invoice Due Date Check, {0}.", description));
                    }
                    catch (Exception exception)
                    {
                        log.Error(string.Format("Error sending email ( from Billing Invoice Due Date Check ), {0}, exception: {1}.", description, exception.ToString()));
                    }
                }
            }
        }
        public override string Status { get { return string.Empty; } }
    }
}
