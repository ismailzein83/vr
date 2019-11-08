using RecordAnalysis.Business;
using RecordAnalysis.Entities;
using System;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Notification;
using Vanrise.Notification.Entities;

namespace RecordAnalysis.MainExtensions.C4Switch.VRActions.BlockInIP
{
    public class BlockInIPAction : VRAction
    {
        public override void Execute(IVRActionExecutionContext context)
        {
            DataRecordAlertRuleActionEventPayload payload = context.EventPayload as DataRecordAlertRuleActionEventPayload;
            Type dataRecordRuntimeType = new DataRecordTypeManager().GetDataRecordRuntimeType(payload.DataRecordTypeId);

            dynamic record = Activator.CreateInstance(dataRecordRuntimeType, payload.OutputRecords);
            //var switchId = (int?)record.Switch;
            //string command;

            //if (switchId.HasValue)
            //{
            //    var switchName = new GenericBusinessEntityManager().GetGenericBusinessEntityName(switchId.Value, C4SwitchManager.BeDefinitionId);
            //    command = $"Blocking In Trunk: {record.InTrunk} on {switchName}";
            //}
            //else
            //{
            string command = $"Firewall blacklist item {record.InIP}";
            //}

            new CommandManager().AddCommand(CommandType.BlockIP, command);
        }
    }
}