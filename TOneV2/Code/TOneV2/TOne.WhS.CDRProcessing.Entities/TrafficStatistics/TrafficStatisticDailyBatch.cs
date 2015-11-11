using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class TrafficStatisticDailyBatch : TrafficStatisticBatch<TrafficStatisticDaily>
    {
        static TrafficStatisticDailyBatch()

       {
           TrafficStatisticDaily TrafficStatisticByInterval = new TrafficStatisticDaily();
           Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(TrafficStatisticDailyBatch), "ItemsByKey", "BatchStart", "BatchEnd");
       }
    }
}
