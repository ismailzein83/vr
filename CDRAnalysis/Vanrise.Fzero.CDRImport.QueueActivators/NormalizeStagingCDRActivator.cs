﻿using System;
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
                    cdr.ConnectDateTime = cdr.ConnectDateTime.Add(TimeOffset);

                if (cdr.DisconnectDateTime != null)
                    cdr.DisconnectDateTime = cdr.DisconnectDateTime.Value.Add(TimeOffset);

                cdr.SwitchId = SwitchId;

                normalizationManager.Normalize(cdr);
                normalizationManager.SetAreaCode(cdr);
            }

            outputItems.Add("Save CDRs", cdrBatch);
        }

        public override void OnDisposed()
        {

        }

    }
}
