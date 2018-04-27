using System;
using System.Collections.Generic;
using Vanrise.Notification.Entities;
using Vanrise.Common.Business;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Notification;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.GenericData.MainExtensions.VRActions
{
    public class DataRecordAlertRuleSendEmailAction : VRAction
    {
        public Guid MailMessageTemplateId { get; set; }

        public override void Execute(IVRActionExecutionContext context)
        {
            DataRecordAlertRuleActionEventPayload payload = context.EventPayload as DataRecordAlertRuleActionEventPayload;

            VRComponentTypeManager componentTypeManager = new VRComponentTypeManager();
            VRActionDefinition actionDefinition = componentTypeManager.GetComponentType<VRActionDefinitionSettings, VRActionDefinition>(DefinitionId);

            actionDefinition.Settings.ThrowIfNull("actionDefinition.Settings", DefinitionId);
            DataRecordSendEmailDefinitionSettings dataRecordSendEmailDefinitionSettings = actionDefinition.Settings.ExtendedSettings.CastWithValidate<DataRecordSendEmailDefinitionSettings>("actionDefinition.Settings.ExtendedSettings", DefinitionId);

            Dictionary<string, dynamic> mailTemplateObjects = new Dictionary<string, dynamic>();

            if (!string.IsNullOrEmpty(dataRecordSendEmailDefinitionSettings.DataRecordObjectName))
            {
                DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
                Type dataRecordRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType(payload.DataRecordTypeId);

                dynamic record = Activator.CreateInstance(dataRecordRuntimeType, payload.OutputRecords);

                mailTemplateObjects.Add(dataRecordSendEmailDefinitionSettings.DataRecordObjectName, record);
            }

            if (dataRecordSendEmailDefinitionSettings.ObjectFieldMappings != null && dataRecordSendEmailDefinitionSettings.ObjectFieldMappings.Count > 0)
            {
                VRObjectTypeDefinitionManager objectTypeDefinitionManager = new VRObjectTypeDefinitionManager();
                foreach (ObjectFieldMapping objectFieldMapping in dataRecordSendEmailDefinitionSettings.ObjectFieldMappings)
                {
                    VRObjectTypeDefinition objectTypeDefinition = objectTypeDefinitionManager.GetVRObjectTypeDefinition(objectFieldMapping.VRObjectTypeDefinitionId);
                    objectTypeDefinition.ThrowIfNull<VRObjectTypeDefinition>("objectTypeDefinition", objectFieldMapping.VRObjectTypeDefinitionId);
                    objectTypeDefinition.Settings.ThrowIfNull<VRObjectTypeDefinitionSettings>("objectTypeDefinition.Settings", objectFieldMapping.VRObjectTypeDefinitionId);
                    objectTypeDefinition.Settings.ObjectType.ThrowIfNull<VRObjectType>("objectTypeDefinition.Settings.ObjectType", objectFieldMapping.VRObjectTypeDefinitionId);

                    dynamic objectId;
                    if (!payload.OutputRecords.TryGetValue(objectFieldMapping.DataRecordFieldName, out objectId))
                    {
                        throw new Exception(string.Format("Output Record doesn't have Record Field: {0}", objectFieldMapping.DataRecordFieldName));
                    }

                    VRObjectTypeCreateObjectContext objectTypeCreateObjectContext = new VRObjectTypeCreateObjectContext() { ObjectId = objectId };

                    Object objectInstance = objectTypeDefinition.Settings.ObjectType.CreateObject(objectTypeCreateObjectContext);
                    mailTemplateObjects.Add(objectFieldMapping.ObjectName, objectInstance);
                }
            }

            new VRMailManager().SendMail(this.MailMessageTemplateId, mailTemplateObjects);
        }
    }
}