using RecordAnalysis.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.GenericData.Business;

namespace RecordAnalysis.Business
{
    public class C4SwitchInterconnectionOnBeforeSaveHandler : GenericBEOnBeforeInsertHandler
    {
        public override Guid ConfigId { get { return new Guid("40FF1961-EC19-44AB-A738-0F32E54F5164"); } }


        public override void Execute(IGenericBEOnBeforeInsertHandlerContext context)
        {
            C4SwitchInterconnectionManager c4SwitchInterconnectionManager = new C4SwitchInterconnectionManager();
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();

            context.ThrowIfNull("context");
            context.GenericBusinessEntity.ThrowIfNull("context.GenericBusinessEntity");

            C4SwitchInterconnectionEntityToSave entityToSave = c4SwitchInterconnectionManager.C4SwitchInterconnectionEntityToSaveMapper(context.GenericBusinessEntity.FieldValues);
            entityToSave.ThrowIfNull("c4SwitchInterconnectionEntity");

            var trunks = entityToSave.Settings.Trunks;
            var switchInterconnectionEntitiesByInterconnection = c4SwitchInterconnectionManager.GetC4SwitchInterconnectionEntitiesByInterconnection();

            List<C4SwitchInterconnectionEntity> c4SwitchInterconnectionEntities = switchInterconnectionEntitiesByInterconnection.GetRecord(entityToSave.InterconnectionId);
            if (c4SwitchInterconnectionEntities != null && c4SwitchInterconnectionEntities.Count > 0)
            {
                C4SwitchInterconnectionEntity existingEntity = c4SwitchInterconnectionEntities.FindRecord(itm => itm.SwitchId == entityToSave.SwitchId);
                if (existingEntity != null)
                {
                    switch (context.OperationType)
                    {
                        case HandlerOperationType.Add:
                            AddSwitchAlreadyMappedErrorMessage(context, genericBusinessEntityManager, entityToSave.SwitchId, entityToSave.InterconnectionId);
                            break;

                        case HandlerOperationType.Update:
                            if (existingEntity.SwitchInterconnectionId != entityToSave.SwitchInterconnectionId)
                                AddSwitchAlreadyMappedErrorMessage(context, genericBusinessEntityManager, entityToSave.SwitchId, entityToSave.InterconnectionId);
                            break;

                        default: throw new NotSupportedException(string.Format("HandlerOperationType {0} not supported.", context.OperationType));
                    }
                }
            }

            var trunkInterconnectionBySwitch = c4SwitchInterconnectionManager.GetAllTrunkInterconnectionBySwitch();
            var interconnectionByTrunkDict = trunkInterconnectionBySwitch.GetRecord(entityToSave.SwitchId);

            HashSet<string> trunkNames = new HashSet<string>();
            HashSet<string> duplicatedTrunks = new HashSet<string>();

            foreach (var trunk in trunks)
            {
                if (trunkNames.Contains(trunk.TrunkName))
                {
                    duplicatedTrunks.Add(trunk.TrunkName);
                    continue;
                }

                trunkNames.Add(trunk.TrunkName);
                C4SwitchInterconnectionEntity relatedEntity = interconnectionByTrunkDict.GetRecord(trunk.TrunkName);
                if (relatedEntity == null)
                    continue;

                switch (context.OperationType)
                {
                    case HandlerOperationType.Add:
                        AddTrunkAlreadyMappedErrorMessage(context, genericBusinessEntityManager, relatedEntity, trunk.TrunkName);
                        break;

                    case HandlerOperationType.Update:
                        if (relatedEntity.InterconnectionId != entityToSave.InterconnectionId)
                            AddTrunkAlreadyMappedErrorMessage(context, genericBusinessEntityManager, relatedEntity, trunk.TrunkName);
                        break;
                }
            }

            if (duplicatedTrunks.Count == 1)
            {
                context.OutputResult.Messages.Add($"Trunk '{duplicatedTrunks.First()}' is duplicated'");
            }
            else if (duplicatedTrunks.Count > 1)
                context.OutputResult.Messages.Add($"Trunks '{string.Join(", ", duplicatedTrunks)}' are duplicated'");


            if (context.OutputResult.Messages.Count > 0)
                context.OutputResult.Result = false;
        }

        private static void AddTrunkAlreadyMappedErrorMessage(IGenericBEOnBeforeInsertHandlerContext context, GenericBusinessEntityManager genericBusinessEntityManager, C4SwitchInterconnectionEntity entity, string trunkName)
        {
            string switchName = genericBusinessEntityManager.GetGenericBusinessEntityName(entity.SwitchId, C4SwitchManager.BeDefinitionId);
            string interconnectionName = genericBusinessEntityManager.GetGenericBusinessEntityName(entity.InterconnectionId, C4InterconnectionManager.BeDefinitionId);

            context.OutputResult.Messages.Add($"Trunk '{trunkName}' has already been mapped to the Switch '{switchName}' for the Interconnection '{interconnectionName}'");
            return;
        }

        private static void AddSwitchAlreadyMappedErrorMessage(IGenericBEOnBeforeInsertHandlerContext context, GenericBusinessEntityManager genericBusinessEntityManager, int switchId, int interconnectionId)
        {
            string switchName = genericBusinessEntityManager.GetGenericBusinessEntityName(switchId, C4SwitchManager.BeDefinitionId);
            string interconnectionName = genericBusinessEntityManager.GetGenericBusinessEntityName(interconnectionId, C4InterconnectionManager.BeDefinitionId);

            context.OutputResult.Messages.Add($"Interconnection '{interconnectionName}' has already been mapped to the Switch '{switchName}'");
            return;
        }
    }
}
