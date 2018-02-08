using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.GenericData.Business;

namespace Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEOnAfterSaveHandlers
{
    public class SendEmailAfterSaveHandler : GenericBEOnAfterSaveHandler
    {
        public override Guid ConfigId
        {
            get { return new Guid("C21FE60A-1AC2-4424-9B27-2A6EB237AB12"); }
        }

        public override void Execute(IGenericBEOnAfterSaveHandlerContext context)
        {
            VRMailManager vrMailManager = new VRMailManager();
            var  emailTemplateEvaluator = vrMailManager.EvaluateMailTemplate(MailTemplateId, context.NewEntity.FieldValues);
            vrMailManager.SendMail(emailTemplateEvaluator.From, emailTemplateEvaluator.To, emailTemplateEvaluator.CC, emailTemplateEvaluator.BCC, emailTemplateEvaluator.Subject, emailTemplateEvaluator.Body);
        }
        public Guid MailTemplateId { get; set; }
    }
}
