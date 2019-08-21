using RecordAnalysis.Business;
using RecordAnalysis.Entities;
using System;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Notification;
using Vanrise.Notification.Entities;

namespace RecordAnalysis.MainExtensions.C4Switch.VRActions.BlockDestinationNumber
{
    public class BlockDestinationNumberAction : VRAction
    {
        public override void Execute(IVRActionExecutionContext context)
        {
            DataRecordAlertRuleActionEventPayload payload = context.EventPayload as DataRecordAlertRuleActionEventPayload;
            Type dataRecordRuntimeType = new DataRecordTypeManager().GetDataRecordRuntimeType(payload.DataRecordTypeId);

            dynamic record = Activator.CreateInstance(dataRecordRuntimeType, payload.OutputRecords);

            string command = $"ADD CALLPRICHK: CSCNAME=\"ALL\", PFX=K'{record.CDPN}, CPFX=K'8, PCDN=\"INVALID\", PT=INHIBITED, FCC=CV45;";
            new C4CommandManager().AddC4Command(C4CommandType.BlockDestinationNumberOnMSC, command);
        }
    }
}