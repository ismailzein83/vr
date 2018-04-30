using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Retail.BusinessEntity.MainExtensions.AccountBulkAction
{
    public class SendRatesAccountBulkAction : AccountBulkActionRuntimeSettings
    {
        public Guid MailMessageTemplateId { get; set; }

        public override void Execute(IAccountBulkActionSettingsContext context)
        {
            Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
            objects.Add("Account", context.Account);
            AccountPackageManager accountPackageManager = new AccountPackageManager();
            var excelContent = accountPackageManager.ExportRates(context.AccountBEDefinitionId, context.Account.AccountId, DateTime.Now, false);
            List<VRMailAttachement> vrMailAttachments = new List<VRMailAttachement>();

            if (excelContent == null || (excelContent.ExcelFileContent==null && excelContent.ExcelFileStream==null))
            {
                context.IsErrorOccured = false;
                context.ErrorMessage = string.Format("The account '{0}' does not have any rates", context.Account.Name);
                return;
            }
            if (excelContent.ExcelFileStream != null)
            {
                VRFile file = new VRFile()
                {
                    Content = excelContent.ExcelFileStream.ToArray()
                };

                vrMailAttachments.Add(new VRMailAttachmentExcel
                {
                    Content = file.Content,
                    Name = string.Format("{0}.xls", context.Account.Name != null ? context.Account.Name : "Account"),
                });
            }
            else if (excelContent.ExcelFileContent != null)
            {

                vrMailAttachments.Add(new VRMailAttachmentExcel
                {
                    Content = excelContent.ExcelFileContent,
                    Name = string.Format("{0}.xls", context.Account.Name != null ? context.Account.Name : "Account"),
                });
            }

            VRMailManager mailManager = new VRMailManager();
            var emailTemplate = mailManager.EvaluateMailTemplate(this.MailMessageTemplateId, objects);
            mailManager.SendMail(emailTemplate.To, emailTemplate.CC, emailTemplate.BCC, emailTemplate.Subject, emailTemplate.Body, vrMailAttachments);
        }
    }
}
