using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TABS.Components
{
    public partial class Engine
    {
        #region Properties

        internal static log4net.ILog log = log4net.LogManager.GetLogger(typeof(Engine));
        public static Dictionary<int, AutoInvoiceSetting> Settings { get { return AutoInvoiceSetting.All.Where(va => va.Value.StartDate.Date == DateTime.Today.Date).ToDictionary(k => k.Key, v => v.Value); } }
        public static NHibernate.ISession NHiberNateNewSession { get { return TABS.DataConfiguration.CurrentSession; } }
        protected static TABS.CustomTimeZoneInfo TimeInfo
        {
            get
            {
                return TABS.CustomTimeZoneInfo.All[0];
            }
        }

        #endregion

        #region Invoice Functions

        protected static int GetMaxLoggerID()
        {
            if (AutoInvoiceLogger.All.Count == 0)
                return 0;
            return (AutoInvoiceLogger.All.Max(l => l.BatchID));
        }

        public static void IssueInvoice(SecurityEssentials.User user)
        {

            int maxID = GetMaxLoggerID() + 1;
            List<AutoInvoiceLogger> lstLogger = new List<AutoInvoiceLogger>();

            #region Email Body Initialization

            StringBuilder mailBody = new StringBuilder();

            mailBody.Append("<html><head>");
            mailBody.Append("<title ></title>");
            mailBody.Append("<style type=\"text/css\">");
            mailBody.Append("a:link, a:visited");
            mailBody.Append("{color: #034af3;}");
            mailBody.Append("a:hover");
            mailBody.Append("{color:#1d60ff;text-decoration:none;}");
            mailBody.Append("a:active");
            mailBody.Append("{color:#034af3;}");
            mailBody.Append("Input.textEntry");
            mailBody.Append("{width:320px;border: 1px solid #ccc;}");
            mailBody.Append("</style>");
            mailBody.Append("</head>");
            mailBody.Append("<body style=\"width:90%;font-size: .80em;font-family: Helvetica Neue, Lucida Grande, Segoe UI, Arial, Helvetica, Verdana, sans-serif;margin: 0px;padding-left: 10px;color: #696969;\">");
            mailBody.Append("<div style=\"width:100%;background-color:#fff;border: 1px solid #496077;\">");
            mailBody.Append("<table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\"><tr><td style=\"text-align: center; padding-left:20px;padding-top:5px;font-size:14px;background-color:#CC0033;\">");

            mailBody.Append("<strong style=\"font-size: 1.4em;color:White;\">TOne Automatic Invoice Report</strong></td></tr>");
            mailBody.Append("<tr><td colspan=\"2\">&nbsp;</td></tr>");

            #endregion

            foreach (TABS.AutoInvoiceSetting setting in Settings.Values)
            {
                mailBody.Append(String.Format("<tr><td colspan=\"2\"><h2 style=\"font-size: 1.4em;background-color:#CC0033;font-size:large; color: #fff;font-variant: small-caps;text-transform: none;font-weight: 200;margin-bottom: 0px;\"> Automatic Invoice Generation for {0} Date: {1} </h2></td></tr>", setting.Description, DateTime.Now.Date));
                DateTime startDate = setting.StartDate;
                DateTime fromDate = new DateTime();
                DateTime toDate = new DateTime();
                DateTime newDate = new DateTime();

                bool IsConflict = false;
                bool IsLocked = false;
                bool isRegenerated = false;
                bool isCDRConflict = false;
                bool IsSent = false;
                bool isGenerated = false;

                #region Update Dates

                AutoGenerateInvoice type = (AutoGenerateInvoice)Enum.ToObject(typeof(AutoGenerateInvoice), setting.Type);
                switch (type)
                {
                    case AutoGenerateInvoice.Weekly:
                        fromDate = startDate.AddDays(-7);
                        toDate = startDate.AddDays(-1);
                        newDate = startDate.AddDays(7);
                        break;
                    case AutoGenerateInvoice.BiWeekly:
                        fromDate = startDate.AddDays(-14);
                        toDate = startDate.AddDays(-1);
                        newDate = startDate.AddDays(14);
                        break;
                    case AutoGenerateInvoice.SemiMonthly:
                        fromDate = startDate.AddDays(-15);
                        toDate = startDate.AddDays(-1);
                        newDate = startDate.AddDays(15);
                        break;
                    case AutoGenerateInvoice.Monthly:
                        fromDate = startDate.AddMonths(-1);
                        toDate = startDate.AddDays(-1);
                        newDate = startDate.AddMonths(1);
                        break;
                    case AutoGenerateInvoice.BiMonthly:
                        fromDate = startDate.AddMonths(-2);
                        toDate = startDate.AddDays(-1);
                        newDate = startDate.AddMonths(2);
                        break;
                    case AutoGenerateInvoice.Period:
                        fromDate = startDate.AddDays(-setting.DayNumber);
                        toDate = startDate.AddDays(-1);
                        newDate = startDate.AddDays(setting.DayNumber);
                        break;
                    case AutoGenerateInvoice.SpecificDay:
                        fromDate = startDate.AddMonths(-1).AddDays(1);
                        toDate = startDate.AddDays(-1);
                        newDate = startDate.AddMonths(1);
                        break;
                    default:
                        fromDate = new DateTime();
                        toDate = new DateTime();
                        newDate = new DateTime();
                        break;
                }

                #endregion

                try
                {
                    foreach (CarrierAccount account in setting.Accounts)
                    {
                        mailBody.Append("<tr><td><div style=\"width: 80%;\">");
                        mailBody.Append("<table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">");
                        mailBody.Append("<tr><td colspan=\"2\">&nbsp;</td></tr>");
                        mailBody.Append(string.Format("<tr><td colspan=\"2\" style=\"border-bottom-style:solid;border-bottom-color:#CC0033;\" width=\"40%\"><strong style=\"font-style:italic;color:#CC0033;\">{0}</strong></td>", account.Name));
                        TABS.Billing_Invoice regeneratedInvoice = null;
                        CheckConflictWithInvoices(out IsConflict, out IsLocked, out isRegenerated, out regeneratedInvoice, account, fromDate, toDate);
                        if (IsConflict && !isRegenerated)
                        {
                            mailBody.Append(string.Format("<tr><td  width=\"40%\">Other invoices found in the system between {0:yyyy-MM-dd} and {1:yyyy-MM-dd} for Customer {2}</td></tr>", fromDate, toDate, account.Name));
                            log.Error(string.Format("Other invoices found in the system between {0:yyyy-MM-dd} and {1:yyyy-MM-dd} for Customer {2}", fromDate, toDate, account.Name));

                            continue;
                        }

                        if (IsLocked)
                        {
                            mailBody.Append(string.Format("<tr><td  width=\"40%\">The invoice that you are trying to regenerate is locked. Please check the invoice, period ({0:yyyy-MM-dd} to {1:yyyy-MM-dd}) for Customer {2}</td></tr>", fromDate, toDate, account.Name));
                            log.Error(string.Format("<tr><td  width=\"40%\">The invoice that you are trying to regenerate is locked. Please check the invoice, period ({0:yyyy-MM-dd} to {1:yyyy-MM-dd}) for Customer {2}</td></tr>", fromDate, toDate, account.Name));
                            continue;
                        }

                        if (isRegenerated)
                        {
                            mailBody.Append(string.Format("<tr><td  width=\"40%\">Invoice is regenerated for Customer {0} from date {1:yyyy-MM-dd} to date {2:yyyy-MM-dd}</td></tr>", account.Name, fromDate, toDate));
                            log.InfoFormat("Invoice is regenerated for Customer {0} from date {1:yyyy-MM-dd} to date {2:yyyy-MM-dd}", account.Name, fromDate, toDate);
                        }
                        if (setting.CheckUnpricedCDRs)
                        {
                            isCDRConflict = LookForErronousPricedCDRs(account, fromDate, toDate);
                            if (isCDRConflict)
                            {
                                mailBody.Append(string.Format("<tr><td  width=\"40%\">Their is Unpriced CDRs for Customer {0} from date {1:yyyy-MM-dd} to date {2:yyyy-MM-dd}</td></tr>", account.Name, fromDate, toDate));
                                log.InfoFormat("Their is Unpriced CDRs for Customer {0} from date {1:yyyy-MM-dd} to date {2:yyyy-MM-dd}", account.Name, fromDate, toDate);
                                continue;
                            }

                        }
                        string SERIAL = !isRegenerated ? string.Empty : regeneratedInvoice.SerialNumber;
                        int invoiceID = -1;
                        if (account.AccountType != AccountType.Termination)
                        {
                            Exception ex;
                            int index = 0;

                            foreach (CarrierAccount customer in GetRelatedCustomers(account))
                            {
                                try
                                {
                                    TABS.Billing_Invoice issuedInvoice = TABS.DataHelper.InvoiceIssue(customer.CarrierAccountID
                                                                 , fromDate
                                                                 , toDate
                                                                 , DateTime.Now
                                                                 , DateTime.Now.AddDays(customer.CarrierProfile.DuePeriod)
                                                                 , TimeInfo
                                                                 , 0//user.ID
                                                                 , SERIAL
                                                                 , out ex);

                                    if (index == 0 && ex == null)
                                        SERIAL = issuedInvoice.SerialNumber;
                                    else if (ex != null)
                                    {
                                        mailBody.Append(string.Format("<tr><td  width=\"40%\">Invoice was not created to {0} from {1:yyyy-MM-dd} to {2:yyyy-MM-dd} : {3}</td></tr>", customer, fromDate, toDate, ex.Message));
                                        log.ErrorFormat("Invoice was not created to {0} from {1:yyyy-MM-dd} to {2:yyyy-MM-dd} : {3}", customer, fromDate, toDate, ex.Message);
                                    }
                                    index++;

                                    if (issuedInvoice != null)
                                    {
                                        invoiceID = issuedInvoice.InvoiceID;
                                        issuedInvoice.IsAutomatic = true;
                                        SaveInvoiceData(issuedInvoice);
                                        isGenerated = true;

                                        mailBody.Append(string.Format("<tr><td  width=\"40%\">A new Invoice is Created to {0} from {1:yyyy-MM-dd} to {2:yyyy-MM-dd}</td></tr>", customer, fromDate, toDate));
                                        log.InfoFormat("A new Invoice is Created to {0} from {1:yyyy-MM-dd} to {2:yyyy-MM-dd}", customer, fromDate, toDate);
                                    }
                                    if (ex == null)
                                    {
                                        if (setting.SendEmail)
                                        {
                                            TABS.Components.Event ev = new TABS.Components.Event(TABS.EventType.InvoiceGeneration, issuedInvoice);

                                            Exception exception;
                                            try
                                            {
                                                SendAutomatedMail(TABS.MailTemplateType.InvoiceGeneration, ev, out exception, issuedInvoice);
                                                IsSent = true;
                                            }
                                            catch (Exception exp)
                                            {
                                                exception = exp;
                                            }
                                            if (exception != null)
                                            {
                                                mailBody.Append(string.Format("<tr><td  width=\"40%\">Error Sending Automated Mail for Customer {1}: {0}</td></tr>", exception.Message, customer.Name));
                                                log.ErrorFormat("Error Sending Automated Mail for Customer {1}: {0}", exception.Message, customer.Name);
                                            }
                                            else
                                            {
                                                mailBody.Append(string.Format("<tr><td  width=\"40%\">A mail was sent to {0} including Invoice from {1:yyyy-MM-dd} to {2:yyyy-MM-dd}</td></tr>", customer, fromDate, toDate));
                                                log.InfoFormat("A mail was sent to {0} including Invoice from {1:yyyy-MM-dd} to {2:yyyy-MM-dd}", customer, fromDate, toDate);
                                            }
                                        }
                                    }
                                }
                                catch (Exception exception)
                                {
                                    mailBody.Append(string.Format("<tr><td  width=\"40%\">Invoice was not created to {0} from {1:yyyy-MM-dd} to {2:yyyy-MM-dd} : {3}</td></tr>", customer, fromDate, toDate, exception.Message));
                                    log.ErrorFormat("Invoice was not created to {0} from {1:yyyy-MM-dd} to {2:yyyy-MM-dd} : {3}", customer, fromDate, toDate, exception.Message);
                                    mailBody.Append("<tr><td colspan=\"2\">&nbsp;</td></tr>");
                                    continue;
                                }

                            }
                        }

                        #region Add Logger

                        lstLogger.Add(new AutoInvoiceLogger
                        {
                            BatchID = maxID,
                            CustomerID = account.CarrierAccountID,
                            Type = setting.Type,
                            SettingID = setting.SettingID,
                            IsCDRConflict = isCDRConflict ? 'Y' : 'N',
                            IsSent = IsSent ? 'Y' : 'N',
                            IsGenerated = isGenerated ? 'Y' : 'N',
                            GeneratedDate = isGenerated ? (DateTime?)DateTime.Now : null,
                            InvoiceID = invoiceID
                        });

                        #endregion

                        mailBody.Append("</table></div></td></tr>");
                    }

                    #region Save

                    Exception exSetting = null;
                    Exception exLogger = null;
                    using (NHibernate.ITransaction transaction = NHiberNateNewSession.BeginTransaction())
                    {
                        setting.StartDate = newDate;
                        NHiberNateNewSession.Flush();
                        NHiberNateNewSession.Clear();
                        SaveOrUpdate(setting, out exSetting);

                        if (exSetting != null)
                        {
                            log.Error(string.Format("Error Updating Setting, {0}", setting.Description), exSetting);
                            transaction.Rollback();
                            ClearNhiberNateSession();
                            mailBody.Append("<tr><td  width=\"40%\">Error Saving Automatic Invoice Data</td></tr>");
                        }

                        foreach (AutoInvoiceLogger logger in lstLogger)
                        {
                            ObjectAssembler.SaveOrUpdate(logger, transaction, false, out exLogger);
                        }
                        if (exLogger != null)
                        {
                            log.Error(string.Format("Error Saving Loggers, {0}", setting.Description), exLogger);
                            transaction.Rollback();
                            ClearNhiberNateSession();
                            mailBody.Append("<tr><td  width=\"40%\">Error Saving Automatic Invoice Data</td></tr>");
                        }
                        else
                            transaction.Commit();
                        ClearAllCashedCollections();
                    }

                    #endregion
                }
                catch (Exception ex1)
                {
                    mailBody.Append(string.Format("Error generating Automatic Invoice Task. {0}", ex1.Message));
                    log.Error("Error generating Automatic Invoice Task.", ex1);
                }
                finally
                {
                    mailBody.Append("</table></div></body></html>");
                    SendEmailAccountManager(mailBody.ToString());
                }
            }
        }

        private static Telerik.Reporting.IReportDocument GetReportByCustomCode(TABS.Billing_Invoice invoice)
        {
            Telerik.Reporting.IReportDocument report = null;
            string customcode = string.Empty;
            customcode = TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.BillingInvoiceCustomCode].Value.ToString();
            if (customcode == string.Empty)
                return null;
            List<TABS.Billing_Invoice> billinginvoicelist = new List<TABS.Billing_Invoice>();
            billinginvoicelist.Add(invoice);

            List<TABS.Billing_Invoice_Detail> details = new List<TABS.Billing_Invoice_Detail>();

            details = invoice != null ? invoice.Billing_Invoice_Details.ToList() : null;

            TABS.Billing_Invoice Copy = CloneInvoice(invoice);
            Copy.Billing_Invoice_Details = details;
            report = TABS.AutomaticInvoiceReports.BillingInvoiceGenerator.GetBillingInvoiceWorkSheet(customcode, Copy, 0);

            return report;
        }

        private static TABS.Billing_Invoice CloneInvoice(TABS.Billing_Invoice Invoice)
        {
            TABS.Billing_Invoice copy = new TABS.Billing_Invoice();
            copy.Amount = Invoice.Amount;
            copy.BeginDate = Invoice.BeginDate;
            copy.Billing_Invoice_Costs = Invoice.Billing_Invoice_Costs;
            copy.Billing_Invoice_Details = Invoice.Billing_Invoice_Details;
            copy.Currency = Invoice.Currency;
            copy.Customer = Invoice.Customer;
            //copy.Data = Invoice.Data;
            //copy.DefinitionDisplay = Invoice.DefinitionDisplay;
            copy.DueDate = Invoice.DueDate;
            copy.Duration = Invoice.Duration;
            copy.EndDate = Invoice.EndDate;
            copy.FileName = Invoice.FileName;
            //copy.Identifier = Invoice.Identifier;
            copy.InvoiceAttachement = Invoice.InvoiceAttachement;
            copy.InvoiceID = Invoice.InvoiceID;
            copy.InvoiceNotes = Invoice.InvoiceNotes;
            copy.InvoicePrintedNote = Invoice.InvoicePrintedNote;
            //copy.IsImported = Invoice.IsImported;
            copy.IsLocked = Invoice.IsLocked;
            copy.IsPaid = Invoice.IsPaid;
            copy.IsSent = Invoice.IsSent;
            copy.IssueDate = Invoice.IssueDate;
            copy.NumberOfCalls = Invoice.NumberOfCalls;
            copy.PaidAmount = Invoice.PaidAmount;
            copy.PaidDate = Invoice.PaidDate;
            copy.SerialNumber = Invoice.SerialNumber;
            copy.SourceFileBytes = Invoice.SourceFileBytes;
            copy.SourceFileName = Invoice.SourceFileName;
            copy.Supplier = Invoice.Supplier;
            copy.User = Invoice.User;
            copy.UserTrackingEnabled = Invoice.UserTrackingEnabled;
            copy.VatValue = Invoice.VatValue;
            return copy;
        }

        protected static void SaveInvoiceData(TABS.Billing_Invoice invoice)
        {
            Exception ex = null;
            Telerik.Reporting.IReportDocument report = null;
            report = GetReportByCustomCode(invoice);


            if (report != null)
            {
                Telerik.Reporting.Processing.ReportProcessor renderer = new Telerik.Reporting.Processing.ReportProcessor();
                var result = renderer.RenderReport("PDF", report, new System.Collections.Hashtable());
                byte[] buffer = result.DocumentBytes;

                string filename = "";
                filename = string.Format("{0}_Invoice ({1:yyyy-MM-dd} to {2:yyyy-MM-dd}).{3}", invoice.Customer, invoice.BeginDate, invoice.EndDate, "PDF");

                invoice.SourceFileBytes = buffer;
                invoice.SourceFileName = filename;
                invoice.Data.ID = invoice.InvoiceID;

                TABS.ObjectAssembler.Save(invoice.Data, out ex);
            }
            else
            {
                log.Error("Note that the invoice was issued with no Invoice Data due to an error in Custom Code. Please review.");
            }
        }

        protected static void CheckConflictWithInvoices(out bool IsConflict, out bool isLocked, out bool isRegenerated, out TABS.Billing_Invoice regeneratedInvoice, CarrierAccount customer, DateTime fromDate, DateTime toDate)
        {
            List<TABS.Billing_Invoice> invoices = new List<TABS.Billing_Invoice>();
            foreach (CarrierAccount carrier in GetRelatedCustomers(customer))
            {
                var _invoices = TABS.ObjectAssembler.GetBillingInvoices(TABS.DataConfiguration.OpenSession(), customer, TABS.CarrierAccount.SYSTEM, fromDate, toDate);
                invoices.AddRange(_invoices);
            }

            isRegenerated = invoices.Any(i => i.BeginDate == fromDate && i.EndDate == toDate);


            if (isRegenerated)
                regeneratedInvoice = invoices.First(i => i.BeginDate == fromDate && i.EndDate == toDate);
            else
                regeneratedInvoice = null;
            IsConflict = invoices != null && invoices.Count > 0 && !isRegenerated;

            isLocked = isRegenerated && invoices.Any(i => i.IsLocked);
        }

        protected static List<TABS.CarrierAccount> GetRelatedCustomers(TABS.CarrierAccount customer)
        {
            List<TABS.CarrierAccount> customers = new List<TABS.CarrierAccount>();

            if (!customer.CarrierProfile.InvoiceByProfile) { customers.Add(customer); return customers; }

            foreach (var item in TABS.CarrierAccount.Customers)
                if (item.CarrierProfile.Equals(customer.CarrierProfile))
                    customers.Add(item);

            return customers;
        }

        protected static bool LookForErronousPricedCDRs(CarrierAccount account, DateTime Fromdate, DateTime ToDate)
        {
            bool found = false;
            var data = TABS.DataHelper.ExecuteScalar(@"EXEC [bp_ErroneousPricedCDR]
	                                                        @CarrierAccountID = @P1,
                                                            @IsSale = @P2,    
	                                                        @FromDate = @P3,
	                                                        @TillDate = @P4"
                , account.CarrierAccountID, "Y", Fromdate, ToDate);

            if (data != null)
            {
                int rowsCount = 0;
                int.TryParse(data.ToString(), out rowsCount);

                if (rowsCount > 0)
                {
                    found = true;
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    string messgae = string.Format(@"Their is {0} unpriced cdrs for Customer {1}"
                           , rowsCount
                           , account
                           , string.Format("'fromDate={0}&toDate={1}&carrier={2}&IsSale={3}'", Fromdate, ToDate, account.CarrierAccountID, "Y"));

                }
            }

            return found;
        }

        #endregion

        #region Email Functions

        private static void SendEmailAccountManager(string body)
        {
            TABS.SpecialSystemParameters.SmtpInfo info = new TABS.SpecialSystemParameters.SmtpInfo();
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();

            mail.From = new System.Net.Mail.MailAddress(info.User);
            mail.To.Add(CarrierAccount.SYSTEM.CarrierProfile.BillingEmail);
            mail.Subject = "Automatic Invoice Generator Report";
            mail.IsBodyHtml = true;
            mail.Body = body;

            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(info.Host);

            smtp.Timeout = (int)TABS.SystemParameter.SMTP_Timeout.NumericValue * 60 * 1000;
            //bool isSent = false;
            Exception ex;
            TABS.Components.EmailSender.Send(mail, out ex);
            if (ex != null)
                logMail.Error(ex.Message);
            else
                logMail.Info("Automatic invoice Generator Result mail was sent to Account Manager");
        }

        public static void SendAutomatedMail(TABS.MailTemplateType type, object context, out Exception ex, Billing_Invoice invoice)
        {
            System.Net.Mail.MailMessage mail = TABS.SpecialSystemParameters.EmailDetailsEvaluator.GetMailMessage(context);
            mail.Attachments.Add(new System.Net.Mail.Attachment(GetAttachedStream(invoice), string.Format("{0}.{1}", invoice.SerialNumber, "pdf"), "application/pdf"));
            TABS.Components.EmailSender.Send(mail, out ex);
        }

        private static System.IO.MemoryStream GetAttachedStream(Billing_Invoice invoice)
        {
            System.IO.MemoryStream memStream = new System.IO.MemoryStream();

            if (invoice != null)
            {
                byte[] buffer = invoice.Data.SourceFileBytes;
                memStream.Write(buffer, 0, buffer.Length);
            }
            memStream.Seek(0, System.IO.SeekOrigin.Begin);
            return memStream;
        }

        #endregion

        #region NHibernate Functions

        private static void ClearNhiberNateSession()
        {
            NHiberNateNewSession.Flush();
            NHiberNateNewSession.Clear();
            NHiberNateNewSession.Close();
        }

        private static void ClearAllCashedCollections()
        {
            AutoInvoiceLogger.ClearCachedCollections();
            AutoInvoiceSetting.ClearCachedCollections();

        }

        public static bool Update(object obj, out Exception ex)
        {
            bool success = false;
            try
            {
                NHiberNateNewSession.Update(obj);
                success = true;
                ex = null;
            }
            catch (Exception e)
            {
                ex = e;
                success = false;
            }
            return success;
        }

        public static bool SaveOrUpdate(object obj, out Exception ex)
        {
            bool success = false;
            try
            {
                NHiberNateNewSession.SaveOrUpdate(obj);
                success = true;
                ex = null;
            }
            catch (Exception e)
            {
                ex = e;
                success = false;
            }
            return success;
        }

        #endregion

    }
}
