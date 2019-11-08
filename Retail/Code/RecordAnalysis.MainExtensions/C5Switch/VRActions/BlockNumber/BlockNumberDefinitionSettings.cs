using System;
using Vanrise.GenericData.Notification;
using Vanrise.Notification.Entities;

namespace RecordAnalysis.MainExtensions.C5Switch.VRActions.BlockNumber
{
    public class BlockNumberDefinitionSettings : VRActionDefinitionExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("25cd0ddc-bbd4-41ba-8515-fda7816f22de"); } }

        public override string RuntimeEditor { get { return "rec-anal-action-c5switch-blocknumber"; } }

        public Guid DataRecordTypeId { get; set; }

        public string NumberFieldName { get; set; }

        public override bool IsApplicable(IVRActionDefinitionIsApplicableContext context)
        {
            DataRecordActionTargetType dataRecordActionTargetType = context.Target as DataRecordActionTargetType;
            if (dataRecordActionTargetType == null)
                return false;

            if (dataRecordActionTargetType.DataRecordTypeId != DataRecordTypeId)
                return false;

            if (dataRecordActionTargetType.AvailableDataRecordFieldNames == null || dataRecordActionTargetType.AvailableDataRecordFieldNames.Count == 0)
                return false;

            if (!dataRecordActionTargetType.AvailableDataRecordFieldNames.Contains(NumberFieldName))
                return false;

            return true;
        }
    }
}