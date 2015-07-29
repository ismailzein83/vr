using System;
using System.Collections.Generic;
using Vanrise.Queueing.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class NumberProfileBatch : PersistentQueueItem
    {
        static NumberProfileBatch()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(NumberProfileBatch), "NumberProfiles");
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(NumberProfile),"AggregateValues",  "SubscriberNumber", "FromDate", "ToDate",  "IsOnNet",  "PeriodId");
        }

        public override string GenerateDescription()
        {
            return String.Format("Number Profile Batch of {0} Number Profiles", NumberProfiles.Count);
        }

        public List<NumberProfile> NumberProfiles;

    }
}
