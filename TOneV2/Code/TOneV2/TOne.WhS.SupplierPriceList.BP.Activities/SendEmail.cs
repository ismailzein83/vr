using System;
using System.Net.Mail;
using Vanrise.Entities;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Common.Business;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    public class SendEmail : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            SendSupplierEmail(context);
        }
        private void SendSupplierEmail(CodeActivityContext context)
        {
            IImportSPLContext splContext = context.GetSPLParameterContext();
            int supplierId = splContext.SupplierId;

            var carrierAccountManager = new CarrierAccountManager();
            var supplierPricelistSettings = carrierAccountManager.GetSupplierPricelistSettings(supplierId);

            if (!supplierPricelistSettings.SendEmail.HasValue || !supplierPricelistSettings.SendEmail.Value || !supplierPricelistSettings.DefaultSupplierPLMailTemplateId.HasValue)
                return;

            context.WriteTrackingMessage(LogEntryType.Information, "Start sending email.");

            if (!supplierPricelistSettings.DefaultSupplierPLMailTemplateId.HasValue)
                throw new NullReferenceException("supplierPricelistSettings.DefaultSupplierPLMailTemplateId");

            Guid salePlmailTemplateId = supplierPricelistSettings.DefaultSupplierPLMailTemplateId.Value;
            CarrierAccount supplier = carrierAccountManager.GetCarrierAccount(supplierId);
            var objects = new Dictionary<string, dynamic>
            {
                {"Supplier", supplier},
            };
            var vrMailManager = new VRMailManager();
            VRMailEvaluatedTemplate evaluatedTemplate = vrMailManager.EvaluateMailTemplate(salePlmailTemplateId, objects);
            Vanrise.Common.Business.ConfigManager configManager = new Vanrise.Common.Business.ConfigManager();
            EmailSettingData emailSettingData = configManager.GetSystemEmail();

            MailMessage objMail = new MailMessage
            {
                From = new MailAddress(emailSettingData.SenderEmail),
                Subject = evaluatedTemplate.Subject,
                Body = evaluatedTemplate.Body,
                IsBodyHtml = true
            };
            if (evaluatedTemplate.To != null)
            {
                foreach (string toEmail in evaluatedTemplate.To.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    objMail.To.Add(toEmail);
            }
            if (evaluatedTemplate.CC != null)
            {
                foreach (string ccEmail in evaluatedTemplate.CC.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    objMail.CC.Add(ccEmail);
            }
            SmtpClient client = vrMailManager.GetSMTPClient(emailSettingData);
            try
            {
                client.Send(objMail);
                context.WriteTrackingMessage(LogEntryType.Information, "Finish sending email.");
            }
            catch (Exception exc)
            {
                context.WriteTrackingMessage(LogEntryType.Error, "Email sending failed", exc.Message);
            }
        }

    }
}
