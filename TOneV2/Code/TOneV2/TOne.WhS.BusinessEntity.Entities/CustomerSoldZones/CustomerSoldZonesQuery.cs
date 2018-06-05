using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CustomerSoldZonesQuery
    {
        public List<long> ZoneIds { get; set; }

        public string Code { get; set; }

        public int Top { get; set; }

        public int SellingNumberPlanId { get; set; }

        public List<int> CountriesIds { get; set; }

        public List<int> CustomersIds { get; set; }

        public List<int> RoutingProductsIds { get; set; }
        
        public int CurrencyId { get; set; }   

    }

    
}
