﻿using Mediation.Generic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.QueueActivators;
using Mediation.Generic.Business;
using Vanrise.Entities;

namespace Mediation.Generic.MainExtensions.MediationOutputHandlers
{
    public class ExecuteOtherMediationOutputHandler : MediationOutputHandler
    {
        public override Guid ConfigId
        {
            get { return new Guid("F1D57186-49CE-4BF9-B4B6-46DDCE93E9EC"); }
        }

        public Guid MediationDefinitionId { get; set; }

        public override void Execute(IMediationOutputHandlerContext context)
        {
            MediationDefinitionManager mediationManager = new MediationDefinitionManager();
            MediationDefinition mediationDefinition = mediationManager.GetMediationDefinition(this.MediationDefinitionId);

            context.DoWhilePreviousRunning(() =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = context.InputQueue.TryDequeue((preparedCdrBatch) =>
                    {
                        MediationRecordsManager mediationRecordsManager = new MediationRecordsManager();
                        List<MediationRecord> mediationRecords = mediationRecordsManager.GenerateMediationRecordsFromBatchRecords(mediationDefinition, mediationDefinition.ParsedRecordTypeId, preparedCdrBatch.BatchRecords);
                       context.WriteTrackingMessage(LogEntryType.Information, "Started storing {0} mediation records", mediationRecords.Count);
                        mediationRecordsManager.SaveMediationRecordsToDB(mediationRecords);
                        context.WriteTrackingMessage(LogEntryType.Information, "Storing {0} mediation records is completed", mediationRecords.Count);
                        context.SetOutputHandlerExecutedOnBatch(preparedCdrBatch);
                    });
                } while (!context.ShouldStop() && hasItem);
            });
        }
    }
}
