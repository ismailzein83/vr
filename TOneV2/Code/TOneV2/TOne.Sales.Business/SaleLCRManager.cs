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
using Aspose.Cells;
using System.Net.Mime;
using System.IO;
using TOne.Business;
using TOne.Entities;

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
                IStateBackupDataManager stateBackupManager = SalesDataManagerFactory.GetDataManager<IStateBackupDataManager>();
                StateBackup stateBackup = stateBackupManager.Create(StateBackupType.Customer, customerId);
                int stateBackupId = 0;
                stateBackupManager.Save(stateBackup, out stateBackupId);
                rateManager.UpdateRateEED(MapEndedRates(saleLcrRates, customerId), customerId);
                List<Rate> newRates = MapNewRates(saleLcrRates, priceListId, currencyId, customerId);
                rateManager.SaveRates(newRates);
                Workbook wbk = new Workbook();
                wbk.Worksheets.RemoveAt("Sheet1");
                Vanrise.Common.Utilities.ActivateAspose();
                wbk.FileName = string.Format("PriceList_{0}", carrierAccount.CarrierAccountName);
                wbk.FileFormat = FileFormatType.Excel97To2003;
                CreateRatesWorkSheet(wbk, "Rates", newRates);
                MemoryStream stream = wbk.SaveToStream();
                stream.Seek(0, SeekOrigin.Begin);
                byte[] bytes = stream.GetBuffer();
                priceListManager.SavePricelistData(bytes, priceListId);

                if (sendEmail)
                    SendMail(stream, "Mobile Rate Change PriceList", newRates, carrierProfile);

            }
            return true;
        }
        public System.Text.StringBuilder GetBody(CarrierProfile profile)
        {
            System.Text.StringBuilder bMailBody = new System.Text.StringBuilder("");

            bMailBody.AppendLine("<html><head>");
            bMailBody.AppendLine("<title ></title>");
            bMailBody.AppendLine("<style type=\"text/css\">");
            bMailBody.AppendLine("a:link, a:visited");
            bMailBody.AppendLine("{color: #034af3;}");
            bMailBody.AppendLine("a:hover");
            bMailBody.AppendLine("{color:#1d60ff;text-decoration:none;}");
            bMailBody.AppendLine("a:active");
            bMailBody.AppendLine("{color:#034af3;}");
            bMailBody.AppendLine("Input.textEntry");
            bMailBody.AppendLine("{width:320px;border: 1px solid #ccc;}");
            bMailBody.AppendLine("tr:nth-child(even)");
            bMailBody.AppendLine("{background-color: #f2f2f2}");
            bMailBody.AppendLine("th, td {text-align: left;padding: 8px;border-bottom: 1px solid #ddd;}");
            bMailBody.AppendLine("th,td {height: 50px; vertical-align: bottom;}");
            bMailBody.AppendLine("</style>");
            bMailBody.AppendLine("</head>");
            bMailBody.AppendLine("<body style=\"font-size: .80em;font-family: Helvetica Neue, Lucida Grande, Segoe UI, Arial, Helvetica, Verdana, sans-serif;margin: 0px;padding: 0px;color: #696969;\">");
            bMailBody.AppendLine("<div style=\"width:100%;background-color:#fff;border: 1px solid #496077;\"><div style=\"width:100%;background-color:#4b6c9e;\">");
            bMailBody.AppendLine("<table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\"><tr><td style=\"padding-left:20px;padding-top:5px;font-size:14px;background-color:#4b6c9e;\">");
            bMailBody.AppendLine("<strong style=\"color:White;\"> Mobile Rate Change</strong></td></tr></table></div>");
            bMailBody.AppendLine("<div style=\"width: 100%;\">");
            bMailBody.AppendLine("</br>");
            bMailBody.AppendLine(string.Format("<span>Dear {0},</span>", profile.Name));
            bMailBody.AppendLine(string.Format("<span>Rates are updated. Check attachment.</span>"));
            bMailBody.AppendLine("</div></div></body></html>");

            return bMailBody;
        }

        private static void CreateRatesWorkSheet(Workbook workbook, string workSheetName, List<Rate> newRates)
        {
            Worksheet worksheet = workbook.Worksheets.Add(workSheetName);
            int lstRouteCompareResultCount = newRates.Count();

            worksheet.Cells[0, 0].PutValue("Destination");
            worksheet.Cells[0, 1].PutValue("Code");
            worksheet.Cells[0, 2].PutValue("Rate");
            worksheet.Cells[0, 3].PutValue("I/D");
            worksheet.Cells[0, 4].PutValue("Effective Date");
            for (int i = 0; i < lstRouteCompareResultCount; i++)
            {
                ZoneManager zoneManager = new ZoneManager();
                CodeManager codeManager = new CodeManager();

                List<Code> codes = codeManager.GetCodes(newRates[i].ZoneId, DateTime.Now);
                foreach (Code code in codes)
                {
                    worksheet.Cells[i + 1, 0].PutValue(zoneManager.GetZone(newRates[i].ZoneId).Name);
                    worksheet.Cells[i + 1, 1].PutValue(code.Value);
                    worksheet.Cells[i + 1, 2].PutValue(newRates[i].NormalRate);
                    worksheet.Cells[i + 1, 3].PutValue(newRates[i].Change == Change.Increase ? "I" : newRates[i].Change == Change.Decrease ? "D" : "N");
                    worksheet.Cells[i + 1, 4].PutValue(String.Format(@"{0: dd-MM-yyyy}", newRates[i].BeginEffectiveDate));
                }

            }
            worksheet.Cells.SetColumnWidth(0, 80);
            worksheet.Cells.SetColumnWidth(1, 15);
            worksheet.Cells.SetColumnWidth(2, 10);
            worksheet.Cells.SetColumnWidth(3, 5);
            worksheet.Cells.SetColumnWidth(4, 25);
        }

        public void SendMail(MemoryStream stream, string subject, List<Rate> newRates, CarrierProfile carrierProfile)
        {
            List<string> to = carrierProfile.PricingEmail.Split(new char[] { ',', ';' }).ToList();
            stream.Seek(0, SeekOrigin.Begin);
            Attachment attacment = new Attachment(stream, "Price List.xls");

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
                message.Body = GetBody(carrierProfile).ToString();
                if (attacment != null)
                    message.Attachments.Add(attacment);

                var client = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(uid, pwd),
                    EnableSsl = mailSettings.Smtp.Network.EnableSsl
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
                    BeginEffectiveDate = lcrRate.BED,
                    EndEffectiveDate = lcrRate.EED,
                    CurrencyID = currencyId,
                    CustomerId = customerId,
                    SupplierId = "SYS",
                    NormalRate = lcrRate.NewRate,
                    ServicesFlag = lcrRate.ServiceFlag,
                    Change = lcrRate.OldRate == lcrRate.NewRate ? BusinessEntity.Entities.Change.None : lcrRate.OldRate < lcrRate.NewRate ? BusinessEntity.Entities.Change.Increase : Change.Decrease,
                    ZoneId = lcrRate.ZoneId,
                    OffPeakRate = lcrRate.NewRate,
                    WeekendRate = lcrRate.NewRate
                };
                rates.Add(rate);
            }
            return rates;
        }

        List<Rate> MapEndedRates(List<SaleLcrRate> saleLcrRates, string customerId)
        {
            RateManager rateManager = new RateManager();
            List<Rate> rates = new List<Rate>();
            List<SaleLcrRate> newRates = new List<SaleLcrRate>();
            Rate rate = null;
            foreach (SaleLcrRate lcrRate in saleLcrRates)
            {
                List<Rate> currentRates = rateManager.GetRates(customerId, lcrRate.ZoneId, lcrRate.BED);
                foreach (Rate currentRate in currentRates)
                {
                    if (!lcrRate.EED.HasValue)
                    {
                        rate = new Rate()
                        {
                            PriceListId = currentRate.PriceListId,
                            EndEffectiveDate = lcrRate.BED,
                            ZoneId = lcrRate.ZoneId
                        };
                    }
                    else if (lcrRate.BED <= currentRate.BeginEffectiveDate && lcrRate.EED <= currentRate.EndEffectiveDate)
                    {
                        rate = new Rate()
                        {
                            PriceListId = currentRate.PriceListId,
                            EndEffectiveDate = currentRate.BeginEffectiveDate,
                            ZoneId = lcrRate.ZoneId
                        };

                        newRates.Add(new SaleLcrRate()
                        {
                            BED = lcrRate.EED.Value,
                            EED = currentRate.EndEffectiveDate,
                            NewRate = currentRate.NormalRate,
                            OldRate = currentRate.NormalRate,
                            ZoneId = currentRate.ZoneId,
                            PriceListId = currentRate.PriceListId,
                            ServiceFlag = currentRate.ServicesFlag
                        });
                    }

                    else if (lcrRate.BED <= currentRate.BeginEffectiveDate && lcrRate.EED >= currentRate.EndEffectiveDate)
                    {
                        rate = new Rate()
                        {
                            PriceListId = currentRate.PriceListId,
                            EndEffectiveDate = currentRate.BeginEffectiveDate,
                            ZoneId = lcrRate.ZoneId
                        };
                    }
                    else if (lcrRate.BED >= currentRate.BeginEffectiveDate && lcrRate.EED >= currentRate.EndEffectiveDate)
                    {

                        rate = new Rate()
                        {
                            PriceListId = currentRate.PriceListId,
                            EndEffectiveDate = lcrRate.BED,
                            ZoneId = lcrRate.ZoneId
                        };
                    }
                    if (rate != null)
                        rates.Add(rate);
                    rate = null;
                }
            }
            saleLcrRates.AddRange(newRates);
            return rates;
        }
    }
}
