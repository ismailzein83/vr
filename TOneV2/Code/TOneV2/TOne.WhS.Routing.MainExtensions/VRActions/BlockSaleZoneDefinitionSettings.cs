using System;
using Vanrise.GenericData.Notification;
using Vanrise.Notification.Entities;

namespace TOne.WhS.Routing.MainExtensions
{
    public class BlockSaleZoneDefinitionSettings : VRActionDefinitionExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("2bde7ae2-cbea-4ff0-a8a0-18afc0f0e495"); } }

        public override string RuntimeEditor { get { return "whs-routing-action-blocksalezone"; } }

        public Guid DataRecordTypeId { get; set; }

        public string CustomerFieldName { get; set; }

        public string SaleZoneFieldName { get; set; }

        public override bool IsApplicable(IVRActionDefinitionIsApplicableContext context)
        {
            DataRecordActionTargetType dataRecordActionTargetType = context.Target as DataRecordActionTargetType;
            if (dataRecordActionTargetType == null)
                return false;

            if (dataRecordActionTargetType.DataRecordTypeId != DataRecordTypeId)
                return false;

            if (dataRecordActionTargetType.AvailableDataRecordFieldNames == null || dataRecordActionTargetType.AvailableDataRecordFieldNames.Count == 0)
                return false;

            if (!string.IsNullOrEmpty(CustomerFieldName) && !dataRecordActionTargetType.AvailableDataRecordFieldNames.Contains(CustomerFieldName))
                return false;

            if (!dataRecordActionTargetType.AvailableDataRecordFieldNames.Contains(SaleZoneFieldName))
                return false;

            return true;
        }
    }
}
