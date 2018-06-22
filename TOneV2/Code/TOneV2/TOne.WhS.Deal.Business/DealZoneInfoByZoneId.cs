using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Business
{
    public class DealZoneInfoByZoneId : Dictionary<long, SortedList<DateTime, DealZoneInfo>>
    {
    }
}
