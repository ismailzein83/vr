using System;
using Vanrise.GenericData.Notification;
using Vanrise.Notification.Entities;

namespace RecordAnalysis.MainExtensions.C4Switch.VRActions.BlockDestinationNumber
{
    public class BlockDestinationNumberDefinitionSettings : VRActionDefinitionExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("C205294A-1B68-473D-8EC8-AF364E1A52A8"); } }

        public override string RuntimeEditor { get { return "rec-anal-action-c4switch-blockdestinationnumber"; } }

        public Guid DataRecordTypeId { get; set; }

        public string DestinationNumberFieldName { get; set; }

        public override bool IsApplicable(IVRActionDefinitionIsApplicableContext context)
        {
            DataRecordActionTargetType dataRecordActionTargetType = context.Target as DataRecordActionTargetType;
            if (dataRecordActionTargetType == null)
                return false;

            if (dataRecordActionTargetType.DataRecordTypeId != DataRecordTypeId)
                return false;

            if (dataRecordActionTargetType.AvailableDataRecordFieldNames == null || dataRecordActionTargetType.AvailableDataRecordFieldNames.Count == 0)
                return false;

            if (!dataRecordActionTargetType.AvailableDataRecordFieldNames.Contains(DestinationNumberFieldName))
                return false;

            return true;
        }
    }
}