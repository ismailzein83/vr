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

            string number = payload.OutputRecords.ContainsKey("MSISDN") ? record.MSISDN : record.Number;
            string command = $"Blocking Number: {number}";
            new CommandManager(BEDefinitions.C5CommandBeDefinitionId).AddCommand(CommandType.BlockNumber, command);
        }
    }
}