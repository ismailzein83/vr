using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities.BillingReport
{
    public class BusinessCaseStatusQuery
    {

        public DateTime fromDate { get; set; }
        public DateTime? toDate { get; set; }
        public  int customerId { get; set; }
        public int topDestination { get; set; }
        public int currencyId { get; set; }
        public string currencySymbol { get; set; }
        public string currencyName { get; set; }
    }
}
