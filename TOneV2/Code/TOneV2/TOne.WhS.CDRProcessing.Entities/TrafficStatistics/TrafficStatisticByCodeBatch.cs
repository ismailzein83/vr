using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class TrafficStatisticByCodeBatch : TrafficStatisticBatch<TrafficStatisticByCode>
    {
        static TrafficStatisticByCodeBatch()

       {
           TrafficStatisticByCode trafficStatisticByCode = new TrafficStatisticByCode();
           Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(TrafficStatisticByCodeBatch), "ItemsByKey", "BatchStart", "BatchEnd");
       }
    }
}
