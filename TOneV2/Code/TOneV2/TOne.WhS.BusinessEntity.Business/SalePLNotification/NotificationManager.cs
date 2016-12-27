using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class NotificationManager
    {
        #region Private Fields

        private SaleZoneManager _saleZoneManager = new SaleZoneManager();

        private CarrierAccountManager _carrierAccountManager = new CarrierAccountManager();

        private SalePriceListManager _salePriceListManager = new SalePriceListManager();

        private VRFileManager _fileManager = new VRFileManager();

        private UserManager _userManager = new UserManager();

        private VRMailManager _vrMailManager = new VRMailManager();

        #endregion

        #region Public Methods

        public IEnumerable<int> SendNotification(int initiatorId, IEnumerable<int> customerIds, long processInstanceId)
        {
            List<int> failedCustomerIdsToSendEmailFor = new List<int>();
            foreach (int customerId in customerIds)
            {
                Guid salePLMailTemplateId = _carrierAccountManager.GetSalePLMailTemplateId(customerId);
                this.SendMail(salePLMailTemplateId, customerId, initiatorId, processInstanceId, failedCustomerIdsToSendEmailFor);
            }

            return failedCustomerIdsToSendEmailFor;
        }

        #endregion

        #region Private Methods

        private void SendMail(Guid salePLMailTemplateId, int customerId, int initiatorId, long processInstanceId, List<int> failedCustomersToSendEmail)
        {
            CarrierAccount customer = _carrierAccountManager.GetCarrierAccount(customerId);

            SalePriceList customerPricelist = _salePriceListManager.GetPriceListByCustomerAndProcessInstanceId(processInstanceId, customerId);
            if (customerPricelist == null)
                throw new DataIntegrityValidationException(string.Format("Customer with id {0} does not have a price list", customerId));

            VRFile file = _fileManager.GetFile(customerPricelist.FileId);

            if (file == null || file.Content == null)
                throw new DataIntegrityValidationException(string.Format("No price list file saved for customer {0}", customer.CarrierAccountId));

            MemoryStream memoryStream = new MemoryStream(file.Content);
            memoryStream.Position = 0;

            var attachment = new Attachment(memoryStream, "SalePriceList.xlsx");
            attachment.ContentType = new ContentType("application/vnd.ms-excel");
            attachment.TransferEncoding = TransferEncoding.Base64;
            attachment.NameEncoding = Encoding.UTF8;
            attachment.Name = "SalePriceList.xls";

            User initiator = _userManager.GetUserbyId(initiatorId);

            Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
            objects.Add("Customer", customer);
            objects.Add("User", initiator);
            objects.Add("Sale Pricelist", customerPricelist);

            VRMailEvaluatedTemplate evaluatedTemplate = _vrMailManager.EvaluateMailTemplate(salePLMailTemplateId, objects);

            Vanrise.Common.Business.ConfigManager configManager = new Vanrise.Common.Business.ConfigManager();
            EmailSettingData emailSettingData = configManager.GetSystemEmail();

            MailMessage objMail = new MailMessage();

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
            objMail.From = new MailAddress(emailSettingData.SenderEmail);
            objMail.Subject = evaluatedTemplate.Subject;
            objMail.Body = evaluatedTemplate.Body;
            objMail.IsBodyHtml = true;
            objMail.Priority = MailPriority.High;
            objMail.Attachments.Add(attachment);

            SmtpClient client = _vrMailManager.GetSMTPClient(emailSettingData);
            try
            {
                client.Send(objMail);
            }
            catch (Exception)
            {
                failedCustomersToSendEmail.Add(customer.CarrierAccountId);
            }
            
        }

        #endregion
      
    }
}
