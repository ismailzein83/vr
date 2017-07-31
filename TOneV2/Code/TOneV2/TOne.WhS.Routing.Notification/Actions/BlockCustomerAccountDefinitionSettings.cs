using System;
using Vanrise.GenericData.Notification;
using Vanrise.Notification.Entities;

namespace TOne.WhS.Routing.Notification
{
    public class BlockCustomerAccountDefinitionSettings : VRActionDefinitionExtendedSettings
    {
        public override Guid ConfigId { get { throw new NotImplementedException(); } }

        public override string RuntimeEditor { get { throw new NotImplementedException(); } }

        public Guid DataRecordTypeId { get; set; }

        public string CustomerFieldName { get; set; }

        public override bool IsApplicable(IVRActionDefinitionIsApplicableContext context)
        {
            DataRecordActionTargetType dataRecordActionTargetType = context.Target as DataRecordActionTargetType;
            if (dataRecordActionTargetType == null)
                return false;

            if (dataRecordActionTargetType.DataRecordTypeId != DataRecordTypeId)
                return false;

            if (dataRecordActionTargetType.AvailableDataRecordFieldNames == null || dataRecordActionTargetType.AvailableDataRecordFieldNames.Count == 0)
                return false;

            if (!dataRecordActionTargetType.AvailableDataRecordFieldNames.Contains(CustomerFieldName))
                return false;

            return true;
        }
    }
}
