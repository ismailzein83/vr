using RecordAnalysis.Business;
using RecordAnalysis.Entities;
using System;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Notification;
using Vanrise.Notification.Entities;

namespace RecordAnalysis.MainExtensions.C4Switch.VRActions.BlockInInterconnection
{
    public class BlockInInterconnectionAction : VRAction
    {
        public override void Execute(IVRActionExecutionContext context)
        {
            DataRecordAlertRuleActionEventPayload payload = context.EventPayload as DataRecordAlertRuleActionEventPayload;
            Type dataRecordRuntimeType = new DataRecordTypeManager().GetDataRecordRuntimeType(payload.DataRecordTypeId);

            dynamic record = Activator.CreateInstance(dataRecordRuntimeType, payload.OutputRecords);
            var inInterconnectionId = (int?)record.InInterconnection;

            if (!inInterconnectionId.HasValue)
                return;

            C4Interconnection interconnection = new C4InterconnectionManager().GetC4Interconnection(inInterconnectionId.Value);
            if (interconnection == null)
                return;

            string command = $"Blocking In Interconnection";
            new C4CommandManager().AddC4Command(C4CommandType.BlockInterconnection, command);

            //C4SwitchInterconnectionManager c4SwitchInterconnectionManager = new C4SwitchInterconnectionManager();

            //var switchInterconnectionEntitiesByInterconnection = c4SwitchInterconnectionManager.GetC4SwitchInterconnectionEntitiesByInterconnection();

            //List<C4SwitchInterconnectionEntity> c4SwitchInterconnectionEntities = switchInterconnectionEntitiesByInterconnection.GetRecord(inInterconnectionId.Value);
            //if (c4SwitchInterconnectionEntities == null || c4SwitchInterconnectionEntities.Count == 0)
            //    return;

            //foreach (var c4SwitchInterconnectionEntity in c4SwitchInterconnectionEntities)
            //{
            //    if (c4SwitchInterconnectionEntity.Settings.Trunks == null || c4SwitchInterconnectionEntity.Settings.Trunks.Count == 0)
            //        continue;

            //    var switchName = new GenericBusinessEntityManager().GetGenericBusinessEntityName(c4SwitchInterconnectionEntity.SwitchId, C4SwitchManager.BeDefinitionId);

            //    foreach (var trunk in c4SwitchInterconnectionEntity.Settings.Trunks)
            //    {
            //        string command = $"Blocking In Trunk: {trunk.TrunkName} on {switchName}";
            //        new C4CommandManager().AddC4Command(C4CommandType.BlockIP, command);
            //    }
            //}
        }
    }
}