using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;


namespace TOne.Analytics.Entities
{
   public class HourlyReportInput
    {
       public CarrierType CarrierType { get; set; }
       public List<string> ConnectionList { get; set; }
       public GenericFilter Filter { get; set; }
       public bool WithSummary { get; set; }
       public TrafficStatisticGroupKeys[] GroupKeys { get; set; }
       public DateTime From { get; set; }
       public DateTime To { get; set; }
    }
}
