﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRMailManager
    {
        public void SendMail(Guid mailMessageTemplateId,  Dictionary<string, dynamic> objects)
        {
            VRMailMessageTemplate mailMessageTemplate = GetMailMessageTemplate(mailMessageTemplateId);
            SendMail(mailMessageTemplate, objects);
        }

        private VRMailMessageTemplate GetMailMessageTemplate(Guid mailMessageTemplateId)
        {
            VRMailMessageTemplate mailMessageTemplate = new VRMailMessageTemplateManager().GetMailMessageTemplate(mailMessageTemplateId);
            if (mailMessageTemplate == null)
                throw new NullReferenceException(String.Format("mailMessageTemplate '{0}'", mailMessageTemplateId));
            if (mailMessageTemplate.Settings == null)
                throw new NullReferenceException(String.Format("mailMessageTemplate.Settings '{0}'", mailMessageTemplateId));
            return mailMessageTemplate;
        }

        private void SendMail(VRMailMessageTemplate mailMessageTemplate, Dictionary<string, dynamic> objects)
        {
            var mailMessageType = GetMailMessageType(mailMessageTemplate.VRMailMessageTypeId);
            Dictionary<string, dynamic> variableValuesObj = new VRObjectManager().EvaluateVariables(mailMessageTemplate.Settings.Variables, objects, mailMessageType.Settings.Objects);
            var mailContext = new VRMailContext { Variables = variableValuesObj };
            string to = EvaluateExpression(mailMessageTemplate.Settings.To, mailContext);
            string cc = EvaluateExpression(mailMessageTemplate.Settings.CC, mailContext);
            string subject = EvaluateExpression(mailMessageTemplate.Settings.Subject, mailContext);
            string body = EvaluateExpression(mailMessageTemplate.Settings.Body, mailContext);
        }

        private static string EvaluateExpression(VRExpression expression, VRMailContext mailContext)
        {
            return RazorEngine.Razor.Parse<VRMailContext>(expression.ExpressionString, mailContext);
        }

        private VRMailMessageType GetMailMessageType(Guid mailMessageTypeId)
        {
            VRMailMessageType mailMessageType = new VRMailMessageTypeManager().GetMailMessageType(mailMessageTypeId);
            if (mailMessageType == null)
                throw new NullReferenceException(String.Format("mailMessageType '{0}'", mailMessageTypeId));
            if (mailMessageType.Settings == null)
                throw new NullReferenceException(String.Format("mailMessageType.Settings '{0}'", mailMessageTypeId));
            if (mailMessageType.Settings.Objects == null)
                throw new NullReferenceException(String.Format("mailMessageType.Settings.Objects '{0}'", mailMessageTypeId));
            return mailMessageType;
        }

        public void SendMail(VRMailMessage mailMessage)
        {
            
        }
    }

    

    public class VRMailContext
    {
        public Dictionary<string, dynamic> Variables { get; set; }
    }
}
