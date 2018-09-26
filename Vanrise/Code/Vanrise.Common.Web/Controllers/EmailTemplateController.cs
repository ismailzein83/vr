using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Common;
using Vanrise.Common.Business;
namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "EmailTemplate")]
    public class EmailTemplateController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredEmailTemplates")]
        public object GetFilteredEmailTemplates(Vanrise.Entities.DataRetrievalInput<EmailTemplateQuery> input)
        {
            EmailTemplateManager manager = new EmailTemplateManager();
            return GetWebResponse(input, manager.GetFilteredEmailTemplates(input), "Email Templates");
        }

        [HttpPost]
        [Route("UpdateEmailTemplate")]
        public Vanrise.Entities.UpdateOperationOutput<EmailTemplateDetail> UpdateEmailTemplate(EmailTemplate emailTemplate)
        {
            EmailTemplateManager manager = new EmailTemplateManager();
            return manager.UpdateEmailTemplate(emailTemplate);
        }

        [HttpGet]
        [Route("GetEmailTemplate")]
        public EmailTemplate GetEmailTemplate(int emailTemplateId)
        {
            EmailTemplateManager manager = new EmailTemplateManager();
            return manager.GetEmailTemplate(emailTemplateId);
        }
    }
}
