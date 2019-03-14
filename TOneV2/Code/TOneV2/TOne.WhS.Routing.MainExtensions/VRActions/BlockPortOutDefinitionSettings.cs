using System;
using Vanrise.GenericData.Notification;
using Vanrise.Notification.Entities;

namespace TOne.WhS.Routing.MainExtensions.VRActions
{
    public class BlockPortOutDefinitionSettings : VRActionDefinitionExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("8BC7B9F4-872B-4189-AA6D-3C252E6C4019"); } }

        public override string RuntimeEditor { get { return "whs-routing-action-blockportout"; } }

        public Guid DataRecordTypeId { get; set; }

        public string SwitchFieldName { get; set; }

        public string PortOutFieldName { get; set; }

        public override bool IsApplicable(IVRActionDefinitionIsApplicableContext context)
        {
            DataRecordActionTargetType dataRecordActionTargetType = context.Target as DataRecordActionTargetType;
            if (dataRecordActionTargetType == null)
                return false;

            if (dataRecordActionTargetType.DataRecordTypeId != DataRecordTypeId)
                return false;

            if (dataRecordActionTargetType.AvailableDataRecordFieldNames == null || dataRecordActionTargetType.AvailableDataRecordFieldNames.Count == 0)
                return false;

            if (!dataRecordActionTargetType.AvailableDataRecordFieldNames.Contains(SwitchFieldName))
                return false;

            if (!dataRecordActionTargetType.AvailableDataRecordFieldNames.Contains(PortOutFieldName))
                return false;

            return true;
        }
    }
}
