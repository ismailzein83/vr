using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class SupplierSMSRateDetail
    {
        public string MobileCountryName { get; set; }
        public string MobileNetworkName { get; set; }
        public decimal CurrentRate { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyDescription { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
