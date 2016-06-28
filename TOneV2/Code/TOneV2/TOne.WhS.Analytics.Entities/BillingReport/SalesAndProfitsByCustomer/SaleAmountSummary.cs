using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities.BillingReport
{
    public class SaleAmountSummary
    {
        public double SaleAmount { get; set; }
        public string FormattedSaleAmount { get; set; }
        public string Customer { get; set; }

        /// <summary>
        /// DO NOT REMOVE
        /// the purpose of this method is to set the schema in the RDLC file
        /// </summary>
        /// <returns></returns>
        /// 
        public SaleAmountSummary() { }
        public IEnumerable<SaleAmountSummary> GetSaleAmountSummaryRDLCSchema()
        {
            return null;
        }

    }
}
