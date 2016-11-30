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
        public VRMailEvaluatedTemplate GetEmailTemplate(long invoiceId)
        {
            InvoiceEmailActionManager manager = new InvoiceEmailActionManager();
            return manager.GetEmailTemplate(invoiceId);
        }
        [HttpGet]
        [Route("GetSendEmailAttachmentTypeConfigs")]
        public IEnumerable<SendEmailAttachmentTypeConfig> GetSendEmailAttachmentTypeConfigs()
        {
            InvoiceEmailActionManager manager = new InvoiceEmailActionManager();
            return manager.GetSendEmailAttachmentTypeConfigs();
        }

        
    }
}
