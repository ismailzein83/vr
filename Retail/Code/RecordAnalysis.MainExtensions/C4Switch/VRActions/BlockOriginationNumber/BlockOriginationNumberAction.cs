using RecordAnalysis.Business;
using RecordAnalysis.Entities;
using System;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Notification;
using Vanrise.Notification.Entities;

namespace RecordAnalysis.MainExtensions.C4Switch.VRActions.BlockOriginationNumber
{
    public class BlockOriginationNumberAction : VRAction
    {
        public override void Execute(IVRActionExecutionContext context)
        {
            DataRecordAlertRuleActionEventPayload payload = context.EventPayload as DataRecordAlertRuleActionEventPayload;
            Type dataRecordRuntimeType = new DataRecordTypeManager().GetDataRecordRuntimeType(payload.DataRecordTypeId);

            dynamic record = Activator.CreateInstance(dataRecordRuntimeType, payload.OutputRecords);

            string command = $"ADD CALLPRICHK: CSCNAME=\"ALL\", PFX=K'8, CPFX=K'{record.CGPN}, PCDN=\"INVALID\", PT=INHIBITED, FCC=CV45;";
            new CommandManager(BEDefinitions.C4CommandBeDefinitionId).AddCommand(CommandType.BlockOriginationNumberOnMSC, command);
        }
    }
}