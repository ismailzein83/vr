using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities.BillingReport.RoutingByCustomer
{
    public class RoutingByCustomerFormatted
    {
        public string Customer { get; set; }

        public string Destination { get; set; }

        public string SaleRate { get; set; }

        public string Supplier { get; set; }
        public string CostRate { get; set; }
        public double? Profit { get; set; }
        public double? SaleNet { get; set; }
        public string SaleNetFormatted { get; set; }
        public double? CostNet { get; set; }
        public string CostNetFormatted { get; set; }
        public string ProfitFormatted { get; set; }
        public string ProfitPerc { get; set; }
        public string CostDuration { get; set; }
        public string SaleDuration { get; set; }

         /// <summary>
        /// DO NOT REMOVE
        /// the purpose of this method is to set the schema in the RDLC file
        /// </summary>
        /// <returns></returns>
        /// 
        public RoutingByCustomerFormatted() { }
        public IEnumerable<RoutingByCustomerFormatted> GetRoutingByCustomerRDLCSchema()
        {
            return null;
        }




    }
}
