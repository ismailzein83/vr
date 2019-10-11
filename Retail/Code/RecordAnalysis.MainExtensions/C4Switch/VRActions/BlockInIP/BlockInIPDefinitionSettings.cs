using System;
using Vanrise.GenericData.Notification;
using Vanrise.Notification.Entities;

namespace RecordAnalysis.MainExtensions.C4Switch.VRActions.BlockInIP
{
    public class BlockInIPDefinitionSettings : VRActionDefinitionExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("44581097-A3A0-45B4-ADAE-559E51FBF01C"); } }

        public override string RuntimeEditor { get { return "rec-anal-action-c4switch-blockinip"; } }

        public Guid DataRecordTypeId { get; set; }

        public string InIPFieldName { get; set; }

        public override bool IsApplicable(IVRActionDefinitionIsApplicableContext context)
        {
            DataRecordActionTargetType dataRecordActionTargetType = context.Target as DataRecordActionTargetType;
            if (dataRecordActionTargetType == null)
                return false;

            if (dataRecordActionTargetType.DataRecordTypeId != DataRecordTypeId)
                return false;

            if (dataRecordActionTargetType.AvailableDataRecordFieldNames == null || dataRecordActionTargetType.AvailableDataRecordFieldNames.Count == 0)
                return false;

            if (!dataRecordActionTargetType.AvailableDataRecordFieldNames.Contains(InIPFieldName))
                return false;

            return true;
        }
    }
}