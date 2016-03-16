using InterConnect.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.QueueActivators;

namespace InterConnect.BusinessEntity.Business.SummaryTesting
{
    public class GenerateStatsQueueActivator : Vanrise.Queueing.Entities.QueueActivator
    {

        public override void ProcessItem(Vanrise.Queueing.Entities.IQueueActivatorExecutionContext context)
        {            
            DataRecordBatch batch = context.ItemToProcess as DataRecordBatch;
            var rawData = batch.GetBatchRecords(10);
            TrafficStatsManager manager = new TrafficStatsManager();
            var summaryBatches = manager.ConvertRawItemsToBatches(rawData, () => new TrafficStatsBatch());
            foreach(var summaryBatch in summaryBatches)
            {
                context.OutputItems.Add("Save Traffic Stats", summaryBatch);
            }
        }
    }
}
