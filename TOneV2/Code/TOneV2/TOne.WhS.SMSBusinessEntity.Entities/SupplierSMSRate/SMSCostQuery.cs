using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class SMSCostQuery
    {
        public DateTime EffectiveDate { get; set; }

        public List<int> MobileCountryIds { get; set; }

        public List<int> MobileNetworkIds { get; set; }

        public int? NumberOfOptions { get; set; }

        //public int? LimitResult { get; set; }
    }
}
