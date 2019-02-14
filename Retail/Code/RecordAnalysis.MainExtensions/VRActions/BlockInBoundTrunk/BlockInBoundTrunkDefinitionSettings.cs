using System;
using Vanrise.GenericData.Notification;
using Vanrise.Notification.Entities;

namespace RecordAnalysis.MainExtensions.VRActions.BlockInBoundTrunk
{
    public class BlockInBoundTrunkDefinitionSettings : VRActionDefinitionExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("44581097-A3A0-45B4-ADAE-559E51FBF01C"); } }

        public override string RuntimeEditor { get { return "rec-anal-action-blockinboundtrunk"; } }

        public Guid DataRecordTypeId { get; set; }

        public string SwitchFieldName { get; set; }

        public string InTrunkFieldName { get; set; }

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

            if (!dataRecordActionTargetType.AvailableDataRecordFieldNames.Contains(InTrunkFieldName))
                return false;

            return true;
        }
    }
}