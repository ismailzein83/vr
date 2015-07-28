using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
     public class DestinationVolumeTrafficResult
    {
         public List<ZoneInfo> TopZones { get; set; }

         public TimeValuesRecord ValuesPerDate { get; set; }
    }
}
