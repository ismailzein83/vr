using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.MainExtensions.VRActions
{
    public class EmailAction : VRAction
    {
        public override Guid ConfigId { get { return new Guid("be74a60e-d312-4b4f-bd76-5b7be81abe62"); } }

        public List<VRObjectPropertyVariable> Variables { get; set; }

        public VRExpression To { get; set; }

        public VRExpression CC { get; set; }

        public VRExpression Subject { get; set; }

        public VRExpression Body { get; set; }

        public override void Execute(IVRActionExecutionContext context)
        {
            //var expressionManager = new Vanrise.Common.Business.VRExpressionManager();
            //VRActionExpressionContext expressionContext = new VRActionExpressionContext(context);
            //string to = expressionManager.EvaluateExpression(this.To, this.Variables, expressionContext);
            //string cc = expressionManager.EvaluateExpression(this.CC, this.Variables, expressionContext);
            //string subject = expressionManager.EvaluateExpression(this.Subject, this.Variables, expressionContext);
            //string body = expressionManager.EvaluateExpression(this.Body, this.Variables, expressionContext);
            //EmailTemplateManager emailManager = new EmailTemplateManager();
            //emailManager.SendEmail(to, body, subject);
        }
    }
}
