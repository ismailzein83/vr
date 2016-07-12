using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions.VRActions
{
    public class EmailAction : VRAction
    {
        public List<VRExpressionVariable> Variables { get; set; }

        public VRExpression To { get; set; }

        public VRExpression CC { get; set; }

        public VRExpression Subject { get; set; }

        public VRExpression Body { get; set; }

        public override void Execute(IVRActionContext context)
        {
            var expressionManager = new Vanrise.Common.Business.VRExpressionManager();
            VRActionExpressionContext expressionContext = new VRActionExpressionContext(context);
            string to = expressionManager.EvaluateExpression(this.To, this.Variables, expressionContext);
            string cc = expressionManager.EvaluateExpression(this.CC, this.Variables, expressionContext);
            string subject = expressionManager.EvaluateExpression(this.Subject, this.Variables, expressionContext);
            string body = expressionManager.EvaluateExpression(this.Body, this.Variables, expressionContext);
            EmailTemplateManager emailManager = new EmailTemplateManager();
            emailManager.SendEmail(to, body, subject);
        }
    }
}
