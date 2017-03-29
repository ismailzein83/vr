using System;
using System.Collections.Generic;
using Vanrise.Notification.Entities;

namespace Vanrise.GenericData.Notification
{
    public class DataRecordSendEmailDefinitionSettings : VRActionDefinitionExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("3B904E8C-2AC0-43DB-A4EF-425869D40544"); } }

        public override string RuntimeEditor { get { return "vr-genericdata-datarecord-vraction-sendemail"; } }

        public Guid DataRecordTypeId { get; set; }

        public Guid MailMessageTypeId { get; set; }

        public string DataRecordObjectName { get; set; }

        public List<ObjectFieldMapping> ObjectFieldMappings { get; set; }

        public override bool IsApplicable(IVRActionDefinitionIsApplicableContext context)
        {
            DataRecordActionTargetType dataRecordActionTargetType = context.Target as DataRecordActionTargetType;
            if (dataRecordActionTargetType == null)
                return false;

            if (dataRecordActionTargetType.DataRecordTypeId != DataRecordTypeId)
                return false;

            if (ObjectFieldMappings != null && ObjectFieldMappings.Count > 0)
            {
                if (dataRecordActionTargetType.AvailableDataRecordFieldNames == null || dataRecordActionTargetType.AvailableDataRecordFieldNames.Count == 0)
                    return false;

                foreach (ObjectFieldMapping objectFieldMapping in ObjectFieldMappings)
                {
                    if (!dataRecordActionTargetType.AvailableDataRecordFieldNames.Contains(objectFieldMapping.DataRecordFieldName))
                        return false;
                }
            }

            return true;
        }
    }

    public class ObjectFieldMapping
    {
        public string ObjectName { get; set; }

        public Guid VRObjectTypeDefinitionId { get; set; }

        public string DataRecordFieldName { get; set; }
    }
}