using System;
using System.Collections.Generic;
using Vanrise.Queueing.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class NumberCriteriaBatch : PersistentQueueItem
    {
        static NumberCriteriaBatch()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(NumberCriteriaBatch), "criteriaValues", "number");
        }

        public override string GenerateDescription()
        {
            return String.Format("Number Criteria Batch");
        }

        public String number;

        public Dictionary<int, decimal> criteriaValues;
        
        public override DateTime GetBatchEnd()
        {
            return DateTime.Today;
        }

        public override DateTime GetBatchStart()
        {
            return DateTime.Today;
        }

        public override void SetBatchEnd(DateTime batchEnd)
        {
        }

        public override void SetBatchStart(DateTime batchStart)
        {
        }
    }
}
