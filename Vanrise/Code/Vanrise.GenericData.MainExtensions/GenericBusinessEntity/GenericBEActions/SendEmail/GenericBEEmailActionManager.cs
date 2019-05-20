using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEActions;

namespace Vanrise.GenericData.MainExtensions
{
    public class GenericBEEmailActionManager
    {
        GenericBusinessEntityManager genericBEManager = new GenericBusinessEntityManager();
        GenericBusinessEntityDefinitionManager genericBusinessEntityDefinitionManager = new GenericBusinessEntityDefinitionManager();

        public bool SendEmail(SendEmailActionInput input)
        {
            var genericBE = genericBEManager.GetGenericBusinessEntity(input.GenericBusinessEntityId,input.BusinessEntityDefinitionId);
            
            List<VRMailAttachement> vrMailAttachments = new List<VRMailAttachement>();
            VRMailManager vrMailManager = new VRMailManager();

            if (input.AttachementFileIds != null && input.AttachementFileIds.Count > 0)
            {
                foreach (var attachementFileId in input.AttachementFileIds)
                {
                    vrMailAttachments.Add(vrMailManager.ConvertToGeneralAttachement(attachementFileId));
                }
            }
            vrMailManager.SendMail(input.EmailTemplate.From, input.EmailTemplate.To, input.EmailTemplate.CC, input.EmailTemplate.BCC, input.EmailTemplate.Subject, input.EmailTemplate.Body, vrMailAttachments);
            VRActionLogger.Current.LogObjectCustomAction(new GenericBusinessEntityManager.GenericBusinessEntityLoggableEntity(input.BusinessEntityDefinitionId), "Send E-mail", true, genericBE, string.Format("An e-mail concerning the generic business entity has been sent to {0}", input.EmailTemplate.To));
            return true;
        }
        public EmailTemplateRuntimeEditor GetEmailTemplate(object genericBusinessEntityId,Guid businessEntityDefinitionId, Guid genericBEMailTemplateId, Guid genericBEActionId)
        {
            EmailTemplateRuntimeEditor emailTemplateRuntimeEditor = new EmailTemplateRuntimeEditor();
           
            GenericBusinessEntityManager genericBEManager = new GenericBusinessEntityManager();

            var genericBE = genericBEManager.GetGenericBusinessEntity(genericBusinessEntityId, businessEntityDefinitionId);
            genericBE.ThrowIfNull("genericBE", genericBusinessEntityId);

            var genericBESettings = genericBusinessEntityDefinitionManager.GetGenericBEDefinitionSettings(businessEntityDefinitionId);
            genericBESettings.ThrowIfNull("genericBESettings", genericBusinessEntityId);
            genericBESettings.GenericBEActions.ThrowIfNull("genericBESettings.GenericBEActions", genericBusinessEntityId);

            var genericBEAction = genericBESettings.GenericBEActions.FirstOrDefault(x => x.GenericBEActionId == genericBEActionId);
            genericBEAction.ThrowIfNull("genericBEAction", genericBEActionId);

            var emailGenericBEAction = genericBEAction.Settings as SendEmailGenericBEAction;
            emailGenericBEAction.ThrowIfNull("emailGenericBEAction");

            Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
            if (emailGenericBEAction.EntityObjectName != null)
            {
                DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
                var dataRecordRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType(genericBESettings.DataRecordTypeId);
                var dataRecord = Activator.CreateInstance(dataRecordRuntimeType, genericBE.FieldValues);
                objects.Add(emailGenericBEAction.EntityObjectName, dataRecord);
            }

            if (emailGenericBEAction.SendEmailObjectsInfo!=null && emailGenericBEAction.SendEmailObjectsInfo.Count > 0 && genericBESettings.ExtendedSettings!=null)
            {
                foreach(var objectInfo in emailGenericBEAction.SendEmailObjectsInfo)
                {
                    var objectValue = genericBusinessEntityDefinitionManager.GetExtendedSettingsInfoByType(genericBESettings, objectInfo.InfoType, genericBE);
                    objectValue.ThrowIfNull("objectValue", objectInfo.InfoType);
                    objects.Add(objectInfo.ObjectName, objectValue);
                }
            }

            VRMailManager vrMailManager = new VRMailManager();
            emailTemplateRuntimeEditor.VRMailEvaluatedTemplate = vrMailManager.EvaluateMailTemplate(genericBEMailTemplateId, objects);
            return emailTemplateRuntimeEditor;
        }

        public object GetMailTemplateIdByInfoType(object genericBusinessEntityId, Guid businessEntityDefinitionId, string infoType)
        {
            GenericBusinessEntityManager genericBEManager = new GenericBusinessEntityManager();
            var genericBE = genericBEManager.GetGenericBusinessEntity(genericBusinessEntityId, businessEntityDefinitionId);
            var genericBESettings = genericBusinessEntityDefinitionManager.GetGenericBEDefinitionSettings(businessEntityDefinitionId);
            genericBESettings.ThrowIfNull("genericBESettings", genericBusinessEntityId);
            genericBESettings.ExtendedSettings.ThrowIfNull("genericBESettings.ExtendedSettings", genericBusinessEntityId);

            return genericBESettings.ExtendedSettings.GetInfoByType(new GenericBEExtendedSettingsContext
            {
                InfoType = infoType,
                GenericBusinessEntity = genericBE,
                DefinitionSettings = genericBESettings
            });
        }
    }
}
