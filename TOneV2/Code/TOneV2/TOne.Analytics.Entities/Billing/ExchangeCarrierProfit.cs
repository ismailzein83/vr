using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.Analytics.Entities
{
    public class ExchangeCarrierProfit
    {
      
        public CarrierAccount CarrierAccount { get; set; }
        public double CustomerProfit { get; set; }
        public double SupplierProfit { get; set; }

    }
}
