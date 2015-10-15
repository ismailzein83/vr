using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Queueing.Entities;

namespace Vanrise.Fzero.CDRImport.QueueActivators
{
    public class NormalizeStagingCDRActivator : QueueActivator
    {
       
        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
            ImportedStagingCDRBatch cdrBatch = (ImportedStagingCDRBatch)item;

            PSTN.BusinessEntity.Business.NormalizationRuleManager normalizationManager = new PSTN.BusinessEntity.Business.NormalizationRuleManager();

            PSTN.BusinessEntity.Business.TrunkManager switchTrunkManager = new PSTN.BusinessEntity.Business.TrunkManager();
            PSTN.BusinessEntity.Entities.TrunkInfo currentTrunk = new PSTN.BusinessEntity.Entities.TrunkInfo();

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

            foreach(var cdr in cdrBatch.StagingCDRs)
            {
                if (cdr.ConnectDateTime != null)
                    cdr.ConnectDateTime = cdr.ConnectDateTime.Value.Add(TimeOffset);

                if (cdr.DisconnectDateTime != null)
                    cdr.DisconnectDateTime = cdr.DisconnectDateTime.Value.Add(TimeOffset);

                if (cdr.InTrunkSymbol != null && cdr.InTrunkSymbol != string.Empty)
                {
                    currentTrunk = switchTrunkManager.GetTrunkBySymbol(cdr.InTrunkSymbol);
                    if (currentTrunk != null)
                        cdr.InTrunkId = currentTrunk.TrunkId;
                }
                if (cdr.OutTrunkSymbol != null && cdr.OutTrunkSymbol != string.Empty)
                {
                    currentTrunk = switchTrunkManager.GetTrunkBySymbol(cdr.OutTrunkSymbol);
                    if (currentTrunk != null)
                        cdr.OutTrunkId = currentTrunk.TrunkId;
                }
                cdr.SwitchID = SwitchId;


                normalizationManager.Normalize(cdr);
            }

            outputItems.Add("Save CDRs", cdrBatch);
        }

        public override void OnDisposed()
        {

        }

    }
}
