using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class TrafficStatisticByIntervalBatch : TrafficStatisticBatch<TrafficStatisticByInterval>
    {
        static TrafficStatisticByIntervalBatch()

       {
           TrafficStatisticByInterval TrafficStatisticByInterval = new TrafficStatisticByInterval();
           Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(TrafficStatisticByIntervalBatch), "ItemsByKey", "BatchStart", "BatchEnd");
       }
    }
}
