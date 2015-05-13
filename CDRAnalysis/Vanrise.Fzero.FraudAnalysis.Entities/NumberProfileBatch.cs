using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class NumberProfileBatch : PersistentQueueItem
    {
        static NumberProfileBatch()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(NumberProfileBatch), "numberProfiles");
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(NumberProfile), "SubscriberNumber", "FromDate", "ToDate", "CountOutCalls", "DiffOutputNumb",
                "CountOutInter","DiffOutputNumb", "CountOutInter", "CountInInter", "CallOutDurAvg", "CountOutFail", "CountInFail", "TotalOutVolume",
                "TotalInVolume", "DiffInputNumbers", "CountOutSMS", "TotalIMEI", "TotalBTS", "IsOnNet", "TotalDataVolume",
                "PeriodId", "CountInCalls", "CallInDurAvg", "CountOutOnNet", "CountInOnNet", "CountOutOffNet", "CountInOffNet");
        }

        public override string GenerateDescription()
        {
            return String.Format("Number Profile Batch of {0} Number Profiles", numberProfiles.Count);
        }

        public List<NumberProfile> numberProfiles;

    }
}
