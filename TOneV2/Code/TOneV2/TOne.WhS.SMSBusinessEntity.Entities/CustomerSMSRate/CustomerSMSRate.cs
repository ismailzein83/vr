using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class CustomerSMSRate : IDateEffectiveSettings
    {
        public long ID { get; set; }

        public int PriceListID { get; set; }

        public int MobileNetworkID { get; set; }

        public decimal Rate { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}