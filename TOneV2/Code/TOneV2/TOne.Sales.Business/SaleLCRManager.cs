using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Net.Configuration;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;
using TOne.Sales.Data;
using TOne.Sales.Entities;

namespace TOne.Sales.Business
{
    public class SaleLCRManager
    {
        public bool SavePriceList(string customerId, List<SaleLcrRate> saleLcrRates, string currencyId, bool sendEmail)
        {
            RateManager rateManager = new RateManager();
            PriceListManager priceListManager = new PriceListManager();
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            CarrierAccount carrierAccount = carrierAccountManager.GetCarrierAccount(customerId);
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            CarrierProfile carrierProfile = carrierProfileManager.GetCarrierProfile(carrierAccount.ProfileId);
            PriceList priceList = new PriceList()
            {
                BeginEffectiveDate = DateTime.Now,
                CurrencyId = currencyId,
                CustomerId = customerId,
                Description = "Created Price List From Mobile.",
                SupplierId = "SYS",
                EndEffectiveDate = null
            };
            int priceListId = 0;
            priceListManager.SavePriceList(priceList, out priceListId);
            if (priceListId > 0)
            {
                rateManager.UpdateRateEED(MapEndedRates(saleLcrRates), customerId);
                rateManager.SaveRates(MapNewRates(saleLcrRates, priceListId, currencyId, customerId));
                IStateBackupDataManager stateBackupManager = SalesDataManagerFactory.GetDataManager<IStateBackupDataManager>();
                StateBackup stateBackup = stateBackupManager.Create(StateBackupType.Customer, customerId);
                int stateBackupId = 0;
                stateBackupManager.Save(stateBackup, out stateBackupId);
                if (sendEmail)
                    SendMail(carrierProfile.BillingEmail.Split(new char[] { ',', ';' }).ToList(), "Mobile Rate Change PriceList", "", null);
            }
            return true;
        }
        public static void SendMail(List<string> to, string subject, string body, Attachment attacment)
        {
            System.Configuration.Configuration oConfig = null;
            if (System.Web.HttpContext.Current != null)
            {
                oConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            }
            else
            {
                oConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            }
            var mailSettings = oConfig.GetSectionGroup("system.net/mailSettings") as MailSettingsSectionGroup;

            if (mailSettings != null)
            {
                int port = mailSettings.Smtp.Network.Port;
                string from = mailSettings.Smtp.From;
                string host = mailSettings.Smtp.Network.Host;
                string pwd = mailSettings.Smtp.Network.Password;
                string uid = mailSettings.Smtp.Network.UserName;

                var message = new MailMessage
                {
                    From = new MailAddress(@from)
                };
                foreach (string toEmail in to)
                    message.To.Add(new MailAddress(toEmail));

                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = body;
                if (attacment != null)
                    message.Attachments.Add(attacment);

                var client = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(uid, pwd),
                    EnableSsl = false
                };


                client.Send(message);

            }
        }
        private List<Rate> MapNewRates(List<SaleLcrRate> saleLcrRates, int priceListId, string currencyId, string customerId)
        {
            List<Rate> rates = new List<Rate>();
            foreach (SaleLcrRate lcrRate in saleLcrRates)
            {
                Rate rate = new Rate()
                {
                    PriceListId = priceListId,
                    BeginEffectiveDate = lcrRate.OldRate <= lcrRate.NewRate ? DateTime.Now.AddDays(7) : lcrRate.BED,
                    EndEffectiveDate = lcrRate.EED,
                    CurrencyID = currencyId,
                    CustomerId = customerId,
                    SupplierId = "SYS",
                    NormalRate = lcrRate.NewRate,
                    ServicesFlag = lcrRate.ServiceFlag,
                    Change = lcrRate.OldRate < lcrRate.NewRate ? BusinessEntity.Entities.Change.Increase : Change.Decrease,
                    ZoneId = lcrRate.ZoneId,
                    OffPeakRate = lcrRate.NewRate,
                    WeekendRate = lcrRate.NewRate
                };
                rates.Add(rate);
            }
            return rates;
        }

        List<Rate> MapEndedRates(List<SaleLcrRate> saleLcrRates)
        {
            List<Rate> rates = new List<Rate>();
            foreach (SaleLcrRate lcrRate in saleLcrRates)
            {
                Rate rate = new Rate()
                {
                    PriceListId = lcrRate.PriceListId,
                    EndEffectiveDate = lcrRate.OldRate < lcrRate.NewRate ? DateTime.Now.AddDays(7) : lcrRate.BED,
                    ZoneId = lcrRate.ZoneId
                };
                rates.Add(rate);
            }
            return rates;
        }
    }
}
