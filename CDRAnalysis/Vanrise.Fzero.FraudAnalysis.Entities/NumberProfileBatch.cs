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
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(NumberProfile), "subscriberNumber", "fromDate", "toDate", "countOutCalls", "diffOutputNumb",
                "countOutInter","diffOutputNumb", "countOutInter", "countInInter", "callOutDurAvg", "countOutFail", "countInFail", "totalOutVolume",
                "totalInVolume", "diffInputNumbers", "countOutSMS", "totalIMEI", "totalBTS", "isOnNet", "totalDataVolume",
                "periodId", "countInCalls", "callInDurAvg", "countOutOnNet", "countInOnNet", "countOutOffNet", "countInOffNet");
        }

        public override string GenerateDescription()
        {
            return String.Format("Number Profile Batch of {0} Number Profiles", numberProfiles.Count);
        }

        public List<NumberProfile> numberProfiles;

    }
}
