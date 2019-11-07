using RecordAnalysis.Business;
using RecordAnalysis.Entities;
using System;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Notification;
using Vanrise.Notification.Entities;

namespace RecordAnalysis.MainExtensions.C5Switch.VRActions.BlockNumber
{
    public class BlockNumberAction : VRAction
    {
        public override void Execute(IVRActionExecutionContext context)
        {
            DataRecordAlertRuleActionEventPayload payload = context.EventPayload as DataRecordAlertRuleActionEventPayload;
            Type dataRecordRuntimeType = new DataRecordTypeManager().GetDataRecordRuntimeType(payload.DataRecordTypeId);

            dynamic record = Activator.CreateInstance(dataRecordRuntimeType, payload.OutputRecords);

            string command = $"Blocking Number: {record.MSISDN}";
            new C4CommandManager().AddC4Command(C4CommandType.BlockDestinationNumberOnMSC, command);
        }
    }
}