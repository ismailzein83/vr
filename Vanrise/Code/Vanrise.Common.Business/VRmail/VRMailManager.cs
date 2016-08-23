using System;
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

        private void SendMail(VRMailMessageTemplate mailMessageTemplate, Dictionary<string, dynamic> objects)
        {
            var mailMessageType = GetMailMessageType(mailMessageTemplate.VRMailMessageTypeId);

            //Dictionary<string, dynamic> variableValuesObj = new VRObjectManager().EvaluateVariables(mailMessageTemplate.Settings.Variables, objects, mailMessageType.Settings.Objects);

            var mailContext = new VRMailContext(mailMessageType, objects);
            string to = EvaluateExpression(mailMessageTemplate.Settings.To, mailContext);
            string cc = EvaluateExpression(mailMessageTemplate.Settings.CC, mailContext);
            string subject = EvaluateExpression(mailMessageTemplate.Settings.Subject, mailContext);
            string body = EvaluateExpression(mailMessageTemplate.Settings.Body, mailContext);
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


        private static string EvaluateExpression(VRExpression expression, VRMailContext mailContext)
        {
            return RazorEngine.Razor.Parse<VRMailContext>(expression.ExpressionString, mailContext);
        }

        public void SendMail(VRMailMessage mailMessage)
        {
            
        }
    }

    
    public class VRMailContext
    {
        //public Dictionary<string, dynamic> ObjectVariables { get; set; }

        Dictionary<string, dynamic> objects;
        VRMailMessageType mailMessageType;
        Dictionary<string, VRObjectVariable> objectVariables = new Dictionary<string, VRObjectVariable>();
        //List<VRObjectTypeDefinition> objectTypeDefinitions = new List<VRObjectTypeDefinition>();

        public VRMailContext(VRMailMessageType mailMessageType, Dictionary<string, dynamic> objects) 
        {
            this.mailMessageType = mailMessageType;
            this.objects = objects;

            objectVariables = mailMessageType.Settings.Objects;

            //foreach (var objectVaribale in mailMessageType.Settings.Objects)
            //{
            //    objectTypeDefinitions.Add(GetObjectTypeDefinitions(objectVaribale.Value.VRObjectTypeDefinitionId));
            //}
        }

        public dynamic GetVal(string objectName, string propertyName)
        {
            dynamic obj;
            //VRObjectType objectType;

            VRObjectTypePropertyDefinition objectTypePropertyDefinition;

            GetObjectAndType(objectName, propertyName, out obj, out objectTypePropertyDefinition);

            var propertyEvaluatorContext = new VRObjectPropertyEvaluatorContext
            {
                Object = obj,
                ObjectType = null
            };

            dynamic variableValue = objectTypePropertyDefinition.PropertyEvaluator.GetPropertyValue(propertyEvaluatorContext);

            return variableValue;
        }

        private void GetObjectAndType(string objectName, string propertyName, out dynamic obj, out VRObjectTypePropertyDefinition objectTypePropertyDefinition)
        {
            objectTypePropertyDefinition = null;

            VRObjectVariable objectVariable;
            if (objects.TryGetValue(objectName, out obj) && objectVariables.TryGetValue(objectName, out objectVariable))
            {
                VRObjectTypeDefinition objectTypeDefinition = GetObjectTypeDefinition(objectVariable.VRObjectTypeDefinitionId);

                foreach (var property in objectTypeDefinition.Settings.Properties)
                    if (property.Name == propertyName){
                        objectTypePropertyDefinition = property;
                        break;
                    }
            }
        }

        private VRObjectTypeDefinition GetObjectTypeDefinition(Guid objectTypeDefinitionId)
        {
            VRObjectTypeDefinition objectTypeDefinition = new VRObjectTypeDefinitionManager().GetVRObjectTypeDefinition(objectTypeDefinitionId);
            if (objectTypeDefinition == null)
                throw new NullReferenceException(String.Format("objectTypeDefinition '{0}'", objectTypeDefinitionId));
            if (objectTypeDefinition.Settings == null)
                throw new NullReferenceException(String.Format("objectTypeDefinition.Settings '{0}'", objectTypeDefinitionId));

            return objectTypeDefinition;
        }
    }
}
