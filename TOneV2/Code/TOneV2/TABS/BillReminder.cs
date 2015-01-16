using System;
using System.Collections.Generic;
using System.Linq;

namespace TABS
{
    public class BillReminder
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(BillReminder));

        public string carrierAccountIDs { get; set; }
        public int RemindBefore { get; set; }
        public bool SendToCustomer { get; set; }
        public int CheckInvoicesBefore { get; set; }
        public int DaysRemaining { get; private set; }
        public TABS.Billing_Invoice Invoice { get; private set; }

        private List<TABS.CarrierAccount> GetCarrierAccounts()
        {
            List<TABS.CarrierAccount> accounts = new List<TABS.CarrierAccount>();
            if (carrierAccountIDs != null)
            {
                string[] accountIDs = carrierAccountIDs.Split(',');

                foreach (var ID in accountIDs)
                {
                    accounts.Add(TABS.CarrierAccount.All.ContainsKey(ID) ? TABS.CarrierAccount.All[ID]
                        : TABS.ObjectAssembler.Get<TABS.CarrierAccount>(ID));

                }
            }
            return accounts;

        }



        public void SendReminders()
        {

            List<TABS.CarrierAccount> accounts = GetCarrierAccounts();
            List<TABS.Billing_Invoice> invoices = null;

            foreach (var acount in accounts)
            {
                invoices = TABS.ObjectAssembler.GetBillingInvoices(acount, TABS.CarrierAccount.SYSTEM, DateTime.Now.AddDays(-this.CheckInvoicesBefore).Date, DateTime.Now.Date)
                      .Where(c => c.DueDate.AddDays(-RemindBefore) == DateTime.Today && (!c.IsPaid)).ToList();

                foreach (var invoice in invoices)
                {

                    TABS.Alert alert = new TABS.Alert();
                    alert.Created = DateTime.Now;
                    alert.IsVisible = true;
                    alert.Source = "Billing Invoice Reminder";
                    alert.Progress = TABS.AlertProgress.Negative;
                    alert.Level = TABS.AlertLevel.Medium;
                    alert.Tag = "Billing Reminder Alerts";
                    Exception ex;
                    TABS.ObjectAssembler.SaveOrUpdate(alert, out ex);


                    log.Info("Sending Email Form Billing Reminder...");

                    this.Invoice = invoice;

                    System.Net.Mail.MailMessage message = TABS.SpecialSystemParameters.EmailDetailsEvaluator.GetMailMessage(this);
                    if (SendToCustomer)
                    {
                        message.To.Add(acount.CarrierProfile.BillingEmail);
                    }

                    bool created = TABS.Components.EmailSender.Send(message, out ex);

                    if (null != ex)
                    {
                        log.Error(string.Format("Error sending email ( from Billing Reminder Due Date Check ), exception: {0}.", ex.ToString()));
                    }
                }

            }
        }

    }
}
