using System;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Queueing.Entities;

namespace Vanrise.Fzero.CDRImport.QueueActivators
{
    public class NormalizeCDRActivator : QueueActivator
    {
        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
            throw new NotImplementedException();
        }

        public override void OnDisposed()
        {

        }

        public override void ProcessItem(IQueueActivatorExecutionContext context)
        {
            ImportedCDRBatch cdrBatch = (ImportedCDRBatch)context.ItemToProcess;
            PSTN.BusinessEntity.Business.NormalizationRuleManager normalizationManager = new PSTN.BusinessEntity.Business.NormalizationRuleManager();
            PSTN.BusinessEntity.Business.SwitchManager switchManager = new PSTN.BusinessEntity.Business.SwitchManager();
            PSTN.BusinessEntity.Entities.Switch currentSwitch;
            currentSwitch = switchManager.GetSwitchByDataSourceId(cdrBatch.Datasource);
            int? SwitchId = null;
            TimeSpan TimeOffset = new TimeSpan();
            if (currentSwitch != null)
            {
                SwitchId = currentSwitch.SwitchId;
                TimeOffset = currentSwitch.TimeOffset;
            }

            if (cdrBatch.CDRs != null)
            {
                foreach (var cdr in cdrBatch.CDRs)
                {
                    if (cdr.ConnectDateTime != null)
                        cdr.ConnectDateTime = cdr.ConnectDateTime.Add(TimeOffset);

                    if (cdr.DisconnectDateTime != null)
                        cdr.DisconnectDateTime = cdr.DisconnectDateTime.Value.Add(TimeOffset);

                    cdr.SwitchId = SwitchId;

                    normalizationManager.Normalize(cdr);
                    normalizationManager.SetAreaCode(cdr);
                }
                context.OutputItems.Add("Save Normal CDRs", cdrBatch);
            }
        }
    }
}
