using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Vanrise.Web.Base;
using System.Web;
using System.IO;
using Vanrise.Invoice.Entities;
using Vanrise.Entities;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
namespace Vanrise.Invoice.MainExtensions
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "InvoiceEmailAction")]
    public class InvoiceEmailActionController:BaseAPIController
    {
        [HttpPost]
        [Route("SendEmail")]
        public void SendEmail(SendEmailActionInput input)
        {
            InvoiceEmailActionManager manager = new InvoiceEmailActionManager();
            manager.SendEmail(input);
        }
        [HttpGet]
        [Route("GetEmailTemplate")]
        public EmailTemplateRuntimeEditor GetEmailTemplate(long invoiceId, Guid invoiceMailTemplateId, Guid invoiceActionId)
        {
            InvoiceEmailActionManager manager = new InvoiceEmailActionManager();
            return manager.GetEmailTemplate(invoiceId, invoiceMailTemplateId, invoiceActionId);
        }
        [HttpGet]
        [Route("GetSendEmailAttachmentTypeConfigs")]
        public IEnumerable<SendEmailAttachmentTypeConfig> GetSendEmailAttachmentTypeConfigs()
        {
            InvoiceEmailActionManager manager = new InvoiceEmailActionManager();
            return manager.GetSendEmailAttachmentTypeConfigs();
        }

        [HttpGet]
        [Route("DownloadAttachment")]
        public HttpResponseMessage DownloadAttachment(long invoiceId, Guid attachmentId)
        {
            InvoiceEmailActionManager manager = new InvoiceEmailActionManager();
            var mailAttachement= manager.DownloadAttachment(invoiceId, attachmentId);
            MemoryStream memStreamRate = new System.IO.MemoryStream();
            memStreamRate.Write(mailAttachement.Content, 0, mailAttachement.Content.Length);
            memStreamRate.Seek(0, System.IO.SeekOrigin.Begin);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            memStreamRate.Position = 0;
            response.Content = new StreamContent(memStreamRate);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = mailAttachement.Name
            };
            return response;
        }
    }
}
