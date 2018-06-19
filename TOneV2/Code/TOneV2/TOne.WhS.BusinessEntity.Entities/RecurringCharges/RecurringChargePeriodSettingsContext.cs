using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
     public class RecurringChargePeriodSettingsContext : IRecurringChargePeriodSettingsContext
    {
         public DateTime FromDate { set;  get; }
         public DateTime ToDate { set;  get; }
         public List<DateTime> Periods { set; get; }
    }
}
